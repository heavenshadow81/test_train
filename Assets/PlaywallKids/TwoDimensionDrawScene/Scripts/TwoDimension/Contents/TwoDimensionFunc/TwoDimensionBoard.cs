using UnityEngine;
using UnityEngine.Video;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 배경 이미지,영상 설정 클래스
    /// </summary>
    public class TwoDimensionBoard : MonoBehaviour
    {
        public const string szFilePath = "TwoDimensionContents/Contents/Background/";
        public const string szDefaultMovie = "back";
        public const string szDefaultImage = "imgWhiteboard";

        private UIPanel _cPanel;
        public UIPanel cPanel
        {
            get
            {
                if (_cPanel == null)
                {
                    _cPanel = this.gameObject.GetComponent<UIPanel>();
                    if (_cPanel == null)
                    {
                        this.gameObject.AddComponent<UIPanel>();
                        _cPanel = this.gameObject.GetComponent<UIPanel>();
                        _cPanel.cachedTransform.localPosition = new Vector3(0, 0, -100f);
                    }
                    _cPanel.transform.localPosition = new Vector3(0, 0, 30f);
                }
                return _cPanel;
            }
        }

        public Texture2D _img;
        public Texture2D imgBackground
        {
            get
            {
                if (_img == null)
                {// read image name from file .
                    _img = Resources.Load(szFilePath + szDefaultImage) as Texture2D;
                }
                return _img;
            }
            set
            {
                if (value != null)
                { _img = value; }

            }
        }

        public VideoClip _video;
        public VideoClip video
        {
            get
            {
                if (_video == null)
                    _video = Resources.Load<VideoClip>(szFilePath + szMovieName);
                return _video;
            }
            set
            {
                if (value != null)
                    _video = value;
            }
        }

        private UITexture _imgBoard;
        public UITexture imgBoard
        {
            get
            {
                if (_imgBoard == null)
                {
                    GameObject obj = NGUITools.AddChild(cPanel.cachedGameObject);
                    obj.AddComponent<UITexture>();
                    obj.name = "Board";
                    _imgBoard = obj.GetComponent<UITexture>();
                    _imgBoard.SetAnchor(cPanel.cachedTransform);

                }
                return _imgBoard;
            }
        }

        private string _szMovieName;
        public string szMovieName
        {
            get
            {
                if (string.IsNullOrEmpty(_szMovieName))
                { return szDefaultMovie; }
                return _szMovieName;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                { _szMovieName = szDefaultMovie; }
                else
                { _szMovieName = value; }
            }
        }

        private VideoPlayer _videoPlayer;
        private RenderTexture _renderTexture;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (_img != null)
                {
                    imgBoard.mainTexture = imgBackground;
                }

                if (_videoPlayer != null)
                {
                    _videoPlayer.Stop();
                }

                PlayMovie();
            }
        }


        void OnEnable()
        {
            // read from setting file
            //imgBackBoard.renderer.material.mainTexture = movie;

            _videoPlayer = gameObject.GetComponent<VideoPlayer>();
            if (_videoPlayer == null)
                _videoPlayer = gameObject.AddComponent<VideoPlayer>();
            _videoPlayer.playOnAwake = false;
            _videoPlayer.isLooping = true;
            _videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
            _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            _videoPlayer.source = VideoSource.VideoClip;

            if (_img != null)
            {
                imgBoard.mainTexture = imgBackground;
            }
            else
            {
                if (video != null)
                {
                    if (_renderTexture != null && (_renderTexture.width != video.width || _renderTexture.height != video.height))
                    {
                        _renderTexture.Release();
                        _renderTexture = null;
                    }
                    _renderTexture = new RenderTexture((int)video.width, (int)video.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                    _renderTexture.Create();

                    imgBoard.mainTexture = _renderTexture;

                    _videoPlayer.targetTexture = _renderTexture;
                    _videoPlayer.clip = video;
                    _videoPlayer.Play();
                }
                else
                {
                    imgBoard.mainTexture = null;
                }
            }
        }

        void OnDisable()
        {
            if (_videoPlayer != null)
            {
                _videoPlayer.Stop();
                _videoPlayer.targetTexture = null;
                _videoPlayer.clip = null;
            }
            if (_renderTexture != null)
            {
                _renderTexture.Release();
                _renderTexture = null;
            }
            _imgBoard.mainTexture = null;
        }

        public void Destroy()
        {
            if (_video != null)
            {
                if (_videoPlayer != null)
                {
                    _videoPlayer.Stop();
                    _videoPlayer.targetTexture = null;
                    _videoPlayer.clip = null;
                }
                if (_renderTexture != null)
                {
                    _renderTexture.Release();
                    _renderTexture = null;
                }
                _video = null;
            }
            if (_imgBoard != null) _imgBoard = null;
            if (_img != null) _img = null;
            if (imgBoard.mainTexture != null) imgBoard.mainTexture = null;
            if (cPanel.cachedGameObject.activeInHierarchy) cPanel.cachedGameObject.SetActive(false);

            _cPanel = null;
            _imgBoard = null;
            _img = null;
        }

        void PlayMovie()
        {
            if (_videoPlayer != null)
            {
                _videoPlayer.Stop();
            }
            imgBoard.mainTexture = null;
            imgBoard.mainTexture = _videoPlayer.targetTexture;
            _videoPlayer.Play();
        }
    }
}