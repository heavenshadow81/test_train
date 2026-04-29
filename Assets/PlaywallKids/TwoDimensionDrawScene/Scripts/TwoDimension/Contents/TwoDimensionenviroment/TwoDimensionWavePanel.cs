using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 파도 영상 제어 클래스
    /// </summary>
    public class TwoDimensionWavePanel : MonoBehaviour, IEvent
    {

        /// <summary>
        /// 젖은 모래 텍스쳐
        /// </summary>
        public UISprite wetSandy;
        /// <summary>
        /// 지우기 기능의 튜브 모델링 메뉴 버튼
        /// </summary>
        public Transform objTube;
        public Material _material;
        /// <summary>
        /// 파도 영상
        /// </summary>
        public VideoClip[] videos;
        public RenderTexture[] renderTextures;
        /// <summary>
        /// 캔버스 클래스 참조 및 관리 클래스
        /// </summary>
        public TwoDimensionPanel canvasPanel;

        #region CONSTANTS
        private const string FILE_PATH = "TwoDimensionContents/Contents/Materials/";
        private const string MATERIAL = "customTranceparance";
        private static float WAVE_HEIGHT
        {
            // 해상도가 1440p일 때 1000이 되도록 설정함 (1440 * 0.6944 = 1000)
            get { return UtilityScript.height * 0.6944f; }
        }
        #endregion CONSTANTS

        #region PROTPERTIES
        public Material customMaterial
        {
            get
            {
                if (_material == null)
                {
                    _material = Resources.Load(FILE_PATH + MATERIAL) as Material;
                }
                return _material;
            }
        }

        private EEventType currentEvent
        {
            get
            { return _currentEvent; }

            set
            {
                if (value != _currentEvent || EEventType.NONE == value)
                {
                    _currentEvent = value;
                    switch (value)
                    {
                        case EEventType.ACTIVATE:
                            if (waveCoroutine != null) StopCoroutine(waveCoroutine);
                            waveCoroutine = StartCoroutine(WaveProcess());
                            Debug.Log(waveCoroutine);
                            break;

                        case EEventType.ALARM:
                            bEvent = true;
                            break;
                        case EEventType.INTERACTION:
                            if (bEvent)
                            { bEvent = false; }
                            break;
                        case EEventType.NONE:
                            bEvent = false;
                            bBottom = false;
                            break;
                    }
                }
            }
        }

        private UIPanel cPanel
        {
            get
            {
                if (_cPanel == null)
                {
                    _cPanel = this.GetComponent<UIPanel>();
                    if (_cPanel == null)
                    {
                        this.gameObject.AddComponent<UIPanel>();
                        _cPanel = this.GetComponent<UIPanel>();
                    }
                }
                return _cPanel;
            }
        }
        #endregion PROTPERTIES

        #region PROPERTY_VARIABLES
        EEventType _currentEvent;
        UIPanel _cPanel;
        #endregion PROPERTY_VARIABLES

        #region PRIVATE_VARIABLES
        bool bBottom;
        /// <summary>
        /// 파도 이벤트 중
        /// </summary>
        bool bEvent;
        /// <summary>
        /// 현재 모래 변환 이벤트 true이면 모래 변환 이벤트 재호출 방지용 변수
        /// </summary>
        bool bDryingSand;
        UITexture waveBoard;
        Canvas_ _canvas;
        /// <summary>
        /// 현재 재생 중인 파도 이벤트 코루틴
        /// </summary>
        Coroutine waveCoroutine;
        #endregion

        #region COLLECTIONS
        List<VideoPlayer> _videoPlayers;
        List<RenderTexture> _renderTextures;
        #endregion

        //  GameObject gBoard;
        void Awake()
        {
            waveBoard = NGUITools.AddChild<UITexture>(cPanel.cachedGameObject);
            waveBoard.name = "waveBoard";
            wetSandy.name = "wetSandy";
            //	board.SetAnchor(cPanel.cachedGameObject);
            waveBoard.material = customMaterial;
            //waveBoard.mainTexture = renderTextures.Length > 0 ? renderTextures[0] : null;

            float h = UIRoot.list[0].activeHeight;
            float w = Screen.width * h / Screen.height;
            // 파도 텍스쳐 해상도 설정
            waveBoard.SetDimensions(Mathf.RoundToInt(w), Mathf.RoundToInt(h * 1.1f));

            //젖은 모래 해상도 설정
            wetSandy.width = (int)w;
            wetSandy.height = (int)(h * 1.1f);

            bDryingSand = false;
            //waveBoard.cachedTransform.localPosition = new Vector3(0, fWave_h, -68f);

            if (canvasPanel)
                _canvas = canvasPanel.cSketchCanvas;
        }

        void OnEnable()
        {
            if (_videoPlayers == null)
                _videoPlayers = new List<VideoPlayer>();
            if (_renderTextures == null)
                _renderTextures = new List<RenderTexture>();

            for (int i = 0; i < videos.Length; i++)
            {
                VideoClip videoClip = videos[i];
                VideoPlayer videoPlayer = null;
                RenderTexture renderTexture = null;

                // Video player
                if (_videoPlayers.Count <= i)
                {
                    videoPlayer = gameObject.AddComponent<VideoPlayer>();
                    videoPlayer.playOnAwake = false;
                    videoPlayer.isLooping = true;
                    videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                    videoPlayer.renderMode = VideoRenderMode.RenderTexture;
                    videoPlayer.source = VideoSource.VideoClip;
                    _videoPlayers.Add(videoPlayer);
                }
                else
                {
                    videoPlayer = _videoPlayers[i];
                }

                // Render Texture
                if (renderTextures.Length > i)
                    renderTexture = renderTextures[i];
                if (renderTexture == null)
                {
                    renderTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                    renderTexture.Create();
                }
                _renderTextures.Add(renderTexture);

                // Play
                if (videoClip != null)
                {
                    videoPlayer.clip = videoClip;
                    videoPlayer.targetTexture = renderTexture;
                    videoPlayer.Play();
                }
            }

            waveBoard.mainTexture = _renderTextures.Count > 0 ? _renderTextures[0] : null;
            // 파도 최초 위치 설정
            waveBoard.cachedTransform.localPosition = new Vector3(0, WAVE_HEIGHT, -68f);
            // 젖은 모래 최초 위치 설정
            wetSandy.cachedTransform.localPosition = new Vector3(0, WAVE_HEIGHT, 0);
            currentEvent = EEventType.NONE;
        }

        /// <summary>
        /// 파도 이벤트 코루틴
        /// </summary>
        /// <returns></returns>
        IEnumerator WaveProcess()
        {
            if (!bDryingSand) wetSandy.alpha = 1f;

            // 파도가 밑까지 내려왔는지 체크 용 변수
            bBottom = false;

            yield return new WaitForEndOfFrame();


            Color color = Color.white;
            color.a = 0f;
            float tubeRatio = 0.5f;
            int canvas_w = (int)_canvas.textureSize.x;
            //텍스쳐 height 길이
            int canvas_h = (int)_canvas.textureSize.y;
            // Debug.Log(canvas_w);
            // Debug.Log(canvas_h);

            do // 파도가 아래로 내려가는 단계
            {
                // Vector3.zero 는 화면 정중앙, 파도 영상의 위치가 Vector3.zero 이면  화면 전체를 덮음
                Vector3 dir = Vector3.zero - waveBoard.cachedTransform.localPosition;
                // 전체 높이에서 현재 파도 위치의 비율
                float ratio = dir.y / WAVE_HEIGHT;

                // float -> int -> float으로 변환 하여 소수점 일정 이하 값 절삭
                ratio = (int)(ratio * -100);
                ratio *= 0.01f;
                int offset_y = (int)(canvas_h * ratio);

                //Debug.Log("ori height : "+ offset_y);
                //텍스쳐의 top height 부터 현재 offset 까지 top down 방향으로 텍스쳐 데이터 초기화
                int clearHeight = (canvas_h - offset_y);
                //Debug.Log( "clear height : " + clearHeight);

                dir = dir * Time.deltaTime;
                //// 순간 이동거리 - 현재 단계에서는 사용을 안함
                //curDist = dir.sqrMagnitude;

                // 캔버스 텍스쳐의 일정 영역만 데이터 초기화
                _canvas.FillRect(Color.clear, 0, offset_y, canvas_w, clearHeight);

                // if ((int)fBetweenDist > (int)curDist && (int)curDist > 2f)
                if (ratio > 0.009f) // 파도 하강 중
                {
                    float curWaveHeight = waveBoard.cachedTransform.localPosition.y;
                    float curWetSandyHeight = wetSandy.cachedTransform.localPosition.y;

                    // 파도 영상을 아래로 이동
                    waveBoard.cachedTransform.localPosition += dir;

                    if (curWetSandyHeight - curWaveHeight >= 0)
                    {
                        if (wetSandy.alpha < 1f)
                            wetSandy.alpha += Time.deltaTime * 10f;
                        wetSandy.cachedTransform.localPosition += dir;
                    }
                    objTube.localPosition += dir * tubeRatio;
                }
                else
                { //파도 도착
                    _canvas.ClearCanvas();
                    wetSandy.alpha = 1f;
                    bBottom = true;
                }

                yield return new WaitForFixedUpdate();
            } while (!bBottom); //파도가 화면 아래로 내려가는 단계

            yield return new WaitForFixedUpdate();

            // 현재 프레임의 파도의 이동 거리
            float curDist = 0;
            // 화면 정중앙(vector3.zero)과 현재 파도의 거리
            float fBetweenDist = 0;

            if (!bDryingSand)
            {
                bDryingSand = true;
                StartCoroutine(DrySandProcess());
            }

            // 파도 영상의 최초 위치
            Vector3 top = new Vector3(0, WAVE_HEIGHT, 0);
            bool bTop = false;

            do // 파동 영상 위로 이동
            {
                Vector3 dir = top - waveBoard.cachedTransform.localPosition;
                fBetweenDist = dir.sqrMagnitude;

                dir = dir * Time.deltaTime * 3f;
                curDist = dir.sqrMagnitude;

                if ((int)fBetweenDist > (int)curDist)
                {
                    objTube.localPosition += dir * tubeRatio;
                    waveBoard.cachedTransform.localPosition += dir;
                }
                else
                { // 원래 위치 도착
                    bTop = true;
                    waveBoard.cachedTransform.localPosition = top;
                }

                yield return new WaitForEndOfFrame();
            } while (!bTop);
            bEvent = false;
            objTube.GetComponent<DynamicButton>().currentState = EState.IDLE;

        }

        /// <summary>
        /// 젖은 모래를 마른 모래로 변환 하는 코루틴
        /// </summary>
        /// <returns></returns>
        IEnumerator DrySandProcess()
        {
            do
            {
                if (!bEvent)
                {
                    // 아래에서 위로 이동
                    wetSandy.cachedTransform.localPosition += Vector3.up * 110f * Time.deltaTime;
                    // 투명 처리 함
                    wetSandy.alpha -= 0.05f * Time.deltaTime;
                }
                yield return new WaitForEndOfFrame();
            } while (wetSandy.alpha > 0f);

            yield return new WaitForEndOfFrame();//젖은 모래 -> 마른 모래 변환 완료

            bDryingSand = false;
            wetSandy.alpha = 1f;
            Vector3 pos = wetSandy.cachedTransform.localPosition;
            pos.y = WAVE_HEIGHT;
            wetSandy.cachedTransform.localPosition = pos;
        }

        void OnDisable()
        {
            if (waveCoroutine != null)
            {
                StopCoroutine(waveCoroutine);
                waveCoroutine = null;
            }

            foreach (VideoPlayer videoPlayer in _videoPlayers)
            {
                if (videoPlayer.isPlaying)
                {
                    videoPlayer.Stop();
                    videoPlayer.clip = null;
                    videoPlayer.targetTexture = null;
                }
            }

            for (int i = 0; i < _renderTextures.Count; i++)
            {
                if (i >= renderTextures.Length)
                {
                    _renderTextures[i].Release();
                }
            }
            _renderTextures.Clear();
        }

        public bool StateInPlay()
        {
            return true;
        }

        public bool StateEventReady()
        {
            currentEvent = EEventType.ALARM;

            return bEvent;
        }
        public bool StateEventActivates()
        {
            currentEvent = EEventType.ACTIVATE;

            return !bEvent;
        }
    }
}