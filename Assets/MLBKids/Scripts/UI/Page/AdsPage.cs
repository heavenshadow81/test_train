using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

namespace ML.MLBKids
{
    public class AdsPage : Page
    {
        public VideoPlayer videoPlayer;
        public RawImage adsImage;

        // render texture
        private RenderTexture _rt;
        
        // ads
        private string[] _adPaths;
        private int _adIndex;

        // body tracking
        private ulong _trackingId = 0;
        private float _bodyDetectionTime = 0.0f;

        public override void Show()
        {
            base.Show();
            SoundManager.StopSFX();
            SoundManager.StopMusicImmediately();
            _Play();

            // body detection
            //KinectHelper.instance.enablesBodyTracking = true;
        }

        public override void Hide()
        {
            _Stop();
            base.Hide();

            // body detection
            _trackingId = 0;
            _bodyDetectionTime = 0;
        }

        public override void Init()
        {
            // Initialize the video player.
            if (videoPlayer == null)
                videoPlayer = FindObjectOfType<VideoPlayer>();

            if (videoPlayer == null)
            {
                Debug.LogError("AdsPage.Init() : No video player found.");
                return;
            }

            _rt = new RenderTexture(1080, 1920, 0);
            _rt.Create();

            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = _rt;
            videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
            adsImage.texture = _rt;
            
            // Finds all video paths.
            _adPaths = Directory.GetFiles(Application.streamingAssetsPath, "*.mp4");

            if (_adPaths == null || _adPaths.Length == 0)
            {
                Debug.LogError("AdsPage.Init() : No ads (video) found.");
                return;
            }

            _adIndex = 0;
            videoPlayer.loopPointReached += (t) =>
            {
                t.url = _adPaths[_adIndex ++ % _adPaths.Length];
                t.Play();
            };

            _Play();
        }

        private void _Play()
        {
            if (videoPlayer != null && _adPaths != null && _adPaths.Length > 0)
            {
                videoPlayer.source = VideoSource.Url;
                videoPlayer.url = _adPaths[_adIndex];
                videoPlayer.time = 0;
                videoPlayer.Play();
            }
        }

        private void _Stop()
        {
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
            }
        }

        
        public void Update()
        {
            
            //ulong trackingId = KinectHelper.instance.trackingId;

            //if (_trackingId != trackingId)
            //{
            //    _bodyDetectionTime = 0;
            //    _trackingId = trackingId;
            //}
            
            if (_trackingId != 0)
            {
                if (!PageManager.instance.isShowingPopup)
                    _bodyDetectionTime += Time.deltaTime;
                else
                    _bodyDetectionTime = 0.0f;

                if (_bodyDetectionTime >= GlobalConstants.instance.bodyDetectionTime)
                {
                    PageManager.instance.GoFromAdsToMenu();
                }
            }
        }

        public void OnDestroy()
        {
            if (_rt != null)
            {
                Destroy(_rt);
                _rt = null;
                adsImage.texture = null;
            }
        }
        #region 추가 함수
        private void OnEnable()
        {
            StartCoroutine(DelayedAction());
        }

        IEnumerator DelayedAction()
        {
            yield return new WaitForSeconds(0.1f);
            PageManager.instance.GoFromAdsToMenu();
            gameObject.SetActive(false);
            yield break;
        }
        #endregion
    }
}