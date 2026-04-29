using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
namespace ML.PlaywallKids.Interaction
{
    using Common;

    public class CoordinateMapperEffect : MonoBehaviour
    {
        //interaction에 있는 매니저
        #region Public variables
        // camera
        public Camera targetCamera;

        // audio source
        public AudioSource audioSource;
        //
        public GameObject goMenu;
        public UITexture screenTexture;

        // videos
        public VideoPlayer videoPlayer;
        public UITexture movieTexture;
        public VideoClip[] heartMovie;
        public Material matKickBall, matLamp;

        // effects
        public GameObject heartEffect, lampEffect;
        public PhysicsMaterial2D lampPhysicsMaterial;

        public InteractionFrisbeeMainController theDiscus;
        public InteractionKickBallMainController kickBalls;
        public InteractionPaintBallManager paintBall;

        public GameObject skate;
        public GameObject travel;
        //스케이팅 퀴즈 종류 설정을 위한 것
        [SerializeField]
        JsonHelper jsonHelper;
        [SerializeField]
        Image skatingIntroUI;
        [SerializeField]
        Sprite[] skatingIntro;

        // background sounds
        public AudioClip heartSound, lampSound, punchSound, throwSound, frisbeeSound, skateSound, travelSound;

        // gravity of the lamp
        public float gravityScale = -0.05f;

        // debug mode
        public bool debug = false;

        #endregion

        #region Private variables

        private const float MOVE_OFFSET = 0.35f;


        private List<int> _heartIDs = new List<int>();

        private List<GameObject> _hearts = new List<GameObject>();

        private Dictionary<GameObject, GameObject> _lamps = new Dictionary<GameObject, GameObject>();

        private Dictionary<int, GameObject> _leftHandColliders = new Dictionary<int, GameObject>();

        private Dictionary<int, GameObject> _rightHandColliders = new Dictionary<int, GameObject>();
        [SerializeField]
        private CoordinateMapperView _mapperView;

        private BigboardContentMode _currentMode = BigboardContentMode.None;

        private VideoClip _currentMovie;
        private RenderTexture _movieRenderTexture;

        private AudioClip _currentSound;

        private TweenRotation cameraTweenRoation;
        private float correctionOffset;
        private UIInteractionController uicontroller;

#if USE_TAG
        private delegate void CallbackFunc(string _value);
        CallbackFunc callbackOnReceiveData;
#endif
        #endregion

        #region Unity Methods

        void Awake()
        {
            uicontroller = FindObjectOfType<UIInteractionController>();
        }

        public void Start()
        {
            cameraTweenRoation = targetCamera.GetComponent<TweenRotation>();
            PreUpdate();

#if UNITY_EDITOR
            debug = true;
#else
        debug = false;
#endif
        }

        public void OnEnable()
        {
            if (_mapperView == null)
            {
                _mapperView = FindObjectOfType<CoordinateMapperView>();
            }
            Debug.Log(_mapperView.name);

            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                    audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.loop = true;
            }

            if (_movieRenderTexture == null)
            {
                _movieRenderTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                _movieRenderTexture.Create();
                movieTexture.mainTexture = _movieRenderTexture;
            }

            if (videoPlayer == null)
            {
                videoPlayer = GetComponent<VideoPlayer>();
                if (videoPlayer == null)
                    videoPlayer = gameObject.AddComponent<VideoPlayer>();
                videoPlayer.playOnAwake = false;
                videoPlayer.isLooping = true;
                videoPlayer.renderMode = VideoRenderMode.RenderTexture;
                videoPlayer.source = VideoSource.VideoClip;
                videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
                videoPlayer.targetTexture = _movieRenderTexture;
            }


            if (kickBalls == null)
                kickBalls = FindObjectOfType<InteractionKickBallMainController>();

            AutoPlayStart();
        }

        public void OnDisable()
        {
            _mapperView = null;
            if (_movieRenderTexture != null)
            {
                _movieRenderTexture.Release();
                _movieRenderTexture = null;
            }
            AutoPlayStop();
        }

        public void Update()
        {
            if (_mapperView != null)
            {
#if USE_TAG
                if (NFCClientSocket.instance.Count > 0)
                {
                    string _value = NFCClientSocket.instance.GetStringValue();
                    if (callbackOnReceiveData != null)
                        callbackOnReceiveData(_value);
                }
#endif

#if UNITY_EDITOR
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (_currentMode == BigboardContentMode.Interaction_Heart) _currentMode = BigboardContentMode.Interaction_Lamplight;
                    else if (_currentMode == BigboardContentMode.Interaction_Lamplight) _currentMode = BigboardContentMode.Interaction_Dish;
                    else if (_currentMode == BigboardContentMode.Interaction_Dish) _currentMode = BigboardContentMode.Interaction_Paints;
                    else if (_currentMode == BigboardContentMode.Interaction_Paints) _currentMode = BigboardContentMode.Interaction_Ball;
                    else if (_currentMode == BigboardContentMode.Interaction_Ball) _currentMode = BigboardContentMode.Interaction_Heart;
                    else _currentMode = BigboardContentMode.Interaction_Ball;

                    NextContent((int)_currentMode);
                }
#endif
                if (audioSource.clip != _currentSound)
                {
                    audioSource.volume -= Time.deltaTime;
                    if (audioSource.volume <= 0.0f)
                    {
                        audioSource.Stop();
                        audioSource.volume = 0.7f;
                        audioSource.clip = _currentSound;
                        audioSource.Play();
                    }
                }

                if (_currentMode == BigboardContentMode.Interaction_Lamplight)
                {
                    float offset = MOVE_OFFSET * cameraTweenRoation.animationCurve.Evaluate(cameraTweenRoation.tweenFactor);

                    correctionOffset = offset;

                    screenTexture.bottomAnchor.relative = 0f - offset;
                    screenTexture.topAnchor.relative = 1f - offset;
                }
            }
#if UNITY_EDITOR
            _ProcessDebugMode();
#endif
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }
        //아무래도 콘텐츠 전환에 쓰는 것 같음..?

        public void PreUpdate()
        {
            if (goMenu != null)
                goMenu.SetActive(_currentMode == BigboardContentMode.None);
            
            _ClearDictionary<int>(_leftHandColliders);
            _ClearDictionary<int>(_rightHandColliders);

            _ClearDictionary(_lamps);
            //_ClearDictionary<GameObject>(_dishes);

            while (_hearts.Count > 0)
            {
                Destroy(_hearts[0]);
                _hearts.RemoveAt(0);
            }
            #region StageFalse
            //기존에 활성화 되어있던 스테이지 오브젝트들 비 활성화
            //foreach (var go in _hearts)
            //    go.SetActive(false);
            _heartIDs.Clear();

            if (theDiscus)
                theDiscus.gameObject.SetActive(false);
            else
                Debug.LogError("접시깨기가 없습니다.");

            if (kickBalls)
                kickBalls.gameObject.SetActive(false);
            else
                Debug.LogError("공차기가 없습니다.");

            if (paintBall)
                paintBall.gameObject.SetActive(false);
            else
                Debug.LogError("물감던지기가 없습니다.");
            if (travel)
            {
                travel.SetActive(false);
            }
            if (skate)
            {
                skate.SetActive(false);
            }
            
            #endregion
            Skybox skyBox = targetCamera.GetComponent<Skybox>();
            skyBox.material = null;

            BarrelDistortionEffect barrelDistortion = targetCamera.GetComponent<BarrelDistortionEffect>();
            if (barrelDistortion != null)
                barrelDistortion.enabled = true;

            targetCamera.transform.localRotation = Quaternion.Euler(cameraTweenRoation.from);
            targetCamera.clearFlags = CameraClearFlags.Depth;

            cameraTweenRoation.enabled = false;
            if (screenTexture)
            {
                screenTexture.bottomAnchor.relative = 0f;
                screenTexture.topAnchor.relative = 1f;
            }

#if USE_TAG
            callbackOnReceiveData = null;
            switch (_currentMode)
            {
                case BigboardContentMode.Interaction_Heart:

                    break;
                case BigboardContentMode.Interaction_Lamplight:

                    break;
                case BigboardContentMode.Interaction_Dish:
                    callbackOnReceiveData = theDiscus.SetStringValue;
                    break;
                case BigboardContentMode.Interaction_Paints:
                    callbackOnReceiveData = paintBall.SetStringValue;
                    break;
                case BigboardContentMode.Interaction_Ball:
                    callbackOnReceiveData = kickBalls.SetStringValue;
                    break;
            }
#endif

            switch (_currentMode)
            {
                case BigboardContentMode.Interaction_Heart:
                    uicontroller.ShowGuidanceObject(false);
                    uicontroller.score.Active = false;
                    uicontroller.uiPointOfScoreManager.Active = false;
                    uicontroller.barTypeTimer.Active = false;
                    uicontroller.circleTypeTimer.Active = false;
                    barrelDistortion.enabled = false;
                    _mapperView.maxUserCount = 6;
                    break;
                case BigboardContentMode.Interaction_Lamplight:
                    targetCamera.clearFlags = CameraClearFlags.Skybox;
                    skyBox.material = matLamp;
                    uicontroller.ShowGuidanceObject(false);
                    uicontroller.score.Active = false;
                    uicontroller.uiPointOfScoreManager.Active = false;
                    uicontroller.barTypeTimer.Active = false;
                    uicontroller.circleTypeTimer.Active = false;
                    cameraTweenRoation.ResetToBeginning();
                    cameraTweenRoation.enabled = true;
                    cameraTweenRoation.PlayForward();

                    _mapperView.maxUserCount = 6;
                    break;
                case BigboardContentMode.Interaction_Dish:
                    targetCamera.clearFlags = CameraClearFlags.Skybox;
                    skyBox.material = matKickBall;

                    theDiscus.gameObject.SetActive(true);
                    _mapperView.maxUserCount = theDiscus.numberOfPlayers = 1;
                    break;
                case BigboardContentMode.Interaction_Paints:
                    barrelDistortion.enabled = false;

                    paintBall.gameObject.SetActive(true);
                    _mapperView.maxUserCount = paintBall.numberOfPlayers = 1;
                    break;
                case BigboardContentMode.Interaction_Ball:
                    targetCamera.clearFlags = CameraClearFlags.Skybox;
                    skyBox.material = matKickBall;
                    _mapperView.maxUserCount = kickBalls.numberOfPlayers = 1;//(Screen.width / Screen.height > 0) ? 2 : 1;
                    kickBalls.gameObject.SetActive(true);
                    break;
                case BigboardContentMode.Interaction_Travel:
                    targetCamera.clearFlags = CameraClearFlags.Skybox;
                    skyBox.material = matKickBall;
                    uicontroller.ShowGuidanceObject(false);
                    uicontroller.score.Active = false;
                    uicontroller.uiPointOfScoreManager.Active = false;
                    uicontroller.barTypeTimer.Active = false;
                    uicontroller.circleTypeTimer.Active = false;
                    screenTexture.mainTexture = null;
                    travel.SetActive(true);
                    break;
                case BigboardContentMode.Interaction_SkatingCloth:
                    targetCamera.clearFlags = CameraClearFlags.Skybox;
                    skyBox.material = matKickBall;
                    uicontroller.ShowGuidanceObject(false);
                    uicontroller.score.Active = false;
                    uicontroller.uiPointOfScoreManager.Active = false;
                    uicontroller.barTypeTimer.Active = false;
                    uicontroller.circleTypeTimer.Active = false;
                    screenTexture.mainTexture = null;
                    jsonHelper.QuizLevel = 2;
                    skatingIntroUI.sprite = skatingIntro[1];
                    skate.SetActive(true);
                    break;
                case BigboardContentMode.Interaction_SkatingFlag:
                    targetCamera.clearFlags = CameraClearFlags.Skybox;
                    skyBox.material = matKickBall;
                    uicontroller.ShowGuidanceObject(false);
                    uicontroller.score.Active = false;
                    uicontroller.uiPointOfScoreManager.Active = false;
                    uicontroller.barTypeTimer.Active = false;
                    uicontroller.circleTypeTimer.Active = false;
                    screenTexture.mainTexture = null;
                    jsonHelper.QuizLevel = 1;
                    skatingIntroUI.sprite = skatingIntro[0];
                    skate.SetActive(true);
                    break;
                case BigboardContentMode.Interaction_SkatingLandmark:
                    targetCamera.clearFlags = CameraClearFlags.Skybox;
                    skyBox.material = matKickBall;
                    uicontroller.ShowGuidanceObject(false);
                    uicontroller.score.Active = false;
                    uicontroller.uiPointOfScoreManager.Active = false;
                    uicontroller.barTypeTimer.Active = false;
                    uicontroller.circleTypeTimer.Active = false;
                    screenTexture.mainTexture = null;
                    jsonHelper.QuizLevel = 3;
                    skatingIntroUI.sprite = skatingIntro[2];
                    skate.SetActive(true);
                    break;
            }

            if (Mathf.Abs(gravityScale) < 0.0001f)
                gravityScale = -0.0001f;
        }
#if UNITY_EDITOR
        private void _ProcessDebugMode()
        {
            if (Input.GetKeyDown(KeyCode.D))
                debug = !debug;

            if (debug)
            {
                Vector3 pos = targetCamera.ScreenToViewportPoint(Input.mousePosition);
                float x = pos.x;
                float y = pos.y;

                switch (_currentMode)
                {
                    case BigboardContentMode.Interaction_Heart:
                        if (Input.GetMouseButton(0))
                            DrawHeart(-1, x, y);
                        break;
                    case BigboardContentMode.Interaction_Lamplight:
                        if (Input.GetMouseButton(0))
                            TakeLamp(-1, new Vector2(x, y), new Vector2(1.0f - x, y));
                        break;
                    case BigboardContentMode.Interaction_Dish:
                        if (Input.GetMouseButton(0))
                            EffectPunch(-1, x, y);
                        break;
                    case BigboardContentMode.Interaction_Paints:
                        if (Input.GetMouseButtonDown(0))
                            EffectThrow(-1, x, y, Vector3.forward, Random.value);
                        break;

                    case BigboardContentMode.Interaction_Ball:
                        if (Input.GetMouseButtonDown(0))
                            EffectKick(-1, x, y);
                        break;
                }
            }
        }
#endif
        #endregion

        #region Backgrounds, Sounds
        //뒷배경 바꾸기...(거의 하트그리기 전용)
        private void _SwapMovies()
        {
            if (_currentMovie != null)
            {
                videoPlayer.Stop();
                videoPlayer.clip = null;
                _currentMovie = null;
            }

            switch (_currentMode)
            {
                case BigboardContentMode.Interaction_Heart:
                    _SetMovie(heartMovie);
                    break;
                case BigboardContentMode.Interaction_Lamplight:
                case BigboardContentMode.Interaction_Dish:
                case BigboardContentMode.Interaction_Paints:
                case BigboardContentMode.Interaction_Ball:
                case BigboardContentMode.Interaction_SkatingCloth:
                case BigboardContentMode.Interaction_SkatingFlag:
                case BigboardContentMode.Interaction_SkatingLandmark:
                case BigboardContentMode.Interaction_Travel:
                    _SetMovie(null);
                    break;
                
            }

            if (_currentMovie != null)
            {
                videoPlayer.clip = _currentMovie;
                videoPlayer.Play();
                movieTexture.gameObject.SetActive(true);
            }
            else
            {
                movieTexture.gameObject.SetActive(false);
            }
        }
        //뒷배경 무비 설정(해상도에 따라 다르게 선택하게....하는 것 같음)
        private void _SetMovie(VideoClip[] textures)
        {
            _currentMovie = null;
            if (textures != null)
            {
                if (textures.Length > 1)
                    _currentMovie = textures[ScreenUtil.screenType == ScreenType.Bigboard2x6 ? 1 : 0];
                else
                    _currentMovie = textures[0];
            }
        }
        //뒷배경 소리 바꾸기
        private void _SwapSounds()
        {
            audioSource.volume = 0.7f;
            _currentSound = _currentMode switch
            {
                BigboardContentMode.Interaction_Heart => heartSound,
                BigboardContentMode.Interaction_Lamplight => lampSound,
                BigboardContentMode.Interaction_Dish => frisbeeSound,
                BigboardContentMode.Interaction_Paints => throwSound,
                BigboardContentMode.Interaction_Ball => punchSound,
                BigboardContentMode.Interaction_Travel => travelSound,
                _ => skateSound,
            };
            //switch (_currentMode)
            //{
            //    case BigboardContentMode.Interaction_Heart:
            //        _currentSound = heartSound;
            //        break;
            //    case BigboardContentMode.Interaction_Lamplight:
            //        _currentSound = lampSound;
            //        break;
            //    case BigboardContentMode.Interaction_Dish:
            //        _currentSound = frisbeeSound;
            //        break;
            //    case BigboardContentMode.Interaction_Paints:
            //        _currentSound = throwSound;
            //        break;
            //    case BigboardContentMode.Interaction_Ball:
            //        _currentSound = punchSound;
            //        break;
            //    case BigboardContentMode.Interaction_Travel:
            //        _currentSound = travelSound;
            //        break;
            //    case BigboardContentMode.Interaction_SkatingCloth:
            //    case BigboardContentMode.Interaction_SkatingFlag:
            //    case BigboardContentMode.Interaction_SkatingLandmark:
            //        _currentSound = skateSound;
            //        break;
            //}
            audioSource.clip = _currentSound;
            audioSource.Play();
        }
        #endregion

        public void DrawHeart(int i, float x, float y)
        {
#if !UNITY_EDITOR
        if( i < 0 )
            return;
#endif

            if (_heartIDs.Contains(i))
                return;

            GameObject go = Instantiate(heartEffect);
            go.transform.parent = targetCamera.transform;
            _heartIDs.Add(i);
            _hearts.Add(go);


            ParticleSystem particle = go.GetComponent<ParticleSystem>();

            if (particle.isPlaying == false)
            {

                //go.transform.position = BarrelDistortionEffect.ConvertToDistorted(targetCamera, PositionType.ViewPort, PositionType.WorldPoint, new Vector3(x, y, 5.0f));
                go.transform.position = targetCamera.ViewportToWorldPoint(new Vector3(x, y, 5.0f));
                //Debug.Log(go.transform.position);

                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;

                particle.Play();

                StartCoroutine(RemoveHeart(i, go));

            }
        }

        IEnumerator RemoveHeart(int id, GameObject go)
        {
            yield return new WaitForSeconds(2f);
            _heartIDs.Remove(id);

            yield return new WaitForSeconds(3f);
            _hearts.Remove(go);
            Destroy(go);
        }

        public void EffectPunch(int i, float x, float y)
        {
            switch (_currentMode)
            {
                case BigboardContentMode.Interaction_Dish:
                    if (theDiscus.CanPlay)
                        theDiscus.PunchDish(targetCamera, x, y);
                    break;
            }
        }


        public void TakeLamp(int i, Vector2 leftHand, Vector2 rightHand)
        {
            if (cameraTweenRoation.isActiveAndEnabled == false)
            {
                HandCheck(i, _leftHandColliders, leftHand);
                HandCheck(i, _rightHandColliders, rightHand);
            }
        }

        public void EffectKick(int i, float x, float y)
        {
            switch (_currentMode)
            {
                case BigboardContentMode.Interaction_Dish:
                    if (theDiscus.CanPlay)
                        theDiscus.PunchDish(targetCamera, x, y);
                    break;

                case BigboardContentMode.Interaction_Ball:
                    if (kickBalls != null)
                        kickBalls.Kick(targetCamera, x, y);
                    break;
            }
        }

        public void EffectThrow(int i, float x, float y, Vector3 navigate, float power)
        {
            if (paintBall != null)
            {
                paintBall.Throw(targetCamera, i, x, y, navigate, power);
            }
        }

        public void EffectThrow(int i, float x, float y)
        {
            //  if (paintBall != null && paintBall.canPlay)
            {
                paintBall.Throw(targetCamera, i, x, y);
            }
        }

        private void HandCheck(int i, Dictionary<int, GameObject> handColliders, Vector2 hand)
        {
            GameObject handCollider = null;
            GameObject lamp = null;

            hand = BarrelDistortionEffect.ConvertToDistorted(targetCamera, PositionType.ViewPort, hand);
            hand = hand - Vector2.up * correctionOffset;

            if (handColliders.ContainsKey(i)) handCollider = handColliders[i];

            if (handCollider != null && _lamps.ContainsKey(handCollider))
            {
                lamp = _lamps[handCollider];
            }

            // Make the new hand collider if it is not available
            if (handCollider == null)
            {
                handCollider = new GameObject(i + " - hand");
                handCollider.transform.parent = targetCamera.transform;
                BoxCollider2D collider2D = handCollider.AddComponent<BoxCollider2D>();
                collider2D.offset = new Vector2(0, -0.1f);
                collider2D.size = new Vector2(0.4f, 0.2f);
                handColliders[i] = handCollider;
            }

            // Move hand's position
            if (float.IsInfinity(hand.x) || float.IsInfinity(hand.y))
                return;

            Vector3 handOffset = targetCamera.ViewportToWorldPoint(new Vector3(hand.x, hand.y, 5f)) - handCollider.transform.position;

            handCollider.transform.position = targetCamera.ViewportToWorldPoint(new Vector3(hand.x, hand.y, 5f));
            handCollider.transform.localRotation = Quaternion.identity;
            handCollider.transform.localScale = Vector3.one;

            // Make the new lamp if it is not available
            bool createdHand = false;
            if (lamp == null)
            {
                createdHand = true;
                lamp = (GameObject)Instantiate(lampEffect);
                lamp.SetActive(true);
                lamp.transform.parent = targetCamera.transform;
                lamp.transform.position = targetCamera.ViewportToWorldPoint(new Vector3(hand.x, hand.y + 0.1f, 2.0f));
                lamp.transform.localRotation = Quaternion.identity;
                _lamps[handCollider] = lamp;
            }

            if (createdHand) handOffset = Vector3.zero;

            Rigidbody2D lampRb = lamp.GetComponent<Rigidbody2D>();
            if (lampRb == null)
            {
                lampRb = lamp.AddComponent<Rigidbody2D>();
                lampRb.mass = 1.0f;
                lampRb.linearDamping = 1.0f;
            }

            if (lampRb.gravityScale >= -0.0001f)
            {
                lampRb.gravityScale = 0.0f;
                lampRb.transform.position = handCollider.transform.position;

                if (handOffset.magnitude > 0.125f)
                {
                    lampRb.gravityScale = gravityScale;
                    lampRb.AddForce(handOffset * 3f, ForceMode2D.Impulse);

                    TweenScale tweenScale = lamp.GetComponent<TweenScale>();
                    tweenScale.ResetToBeginning();
                    tweenScale.PlayForward();

                    TweenRotation tweenRotation = lamp.GetComponent<TweenRotation>();
                    tweenRotation.ResetToBeginning();
                    tweenRotation.PlayForward();

                    EventDelegate evt = new EventDelegate(this, "_LampAnimaionFinish");
                    evt.parameters[0] = new EventDelegate.Parameter(lamp);

                    tweenScale.SetOnFinished(evt);

                    lamp.transform.parent = targetCamera.transform;

                    if (handCollider != null && _lamps.ContainsKey(handCollider))
                    {
                        StartCoroutine(LampInit(handCollider));
                    }
                }
            }
        }

        private void _LampAnimaionFinish(GameObject lamp)
        {
            Destroy(lamp);
        }

        IEnumerator LampInit(GameObject handCollider)
        {
            yield return new WaitForSeconds(5f);

            _lamps[handCollider] = null;
        }

        //컨텐츠 변경
        public void NextContent(int idx)
        {
            if (_mapperView != null)
            {
                BigboardContentMode _nextContents = (BigboardContentMode)idx;
                if (_nextContents == _currentMode) return;
                _currentMode = _nextContents;
                _mapperView._motionType = GetCurrentMotionValue();
                SetImageType();

                PreUpdate();

                _SwapMovies();
                _SwapSounds();

                AutoPlayStart();
            }
        }

        public MotionType GetCurrentMotionValue()
        {
            switch (_currentMode)
            {
                case BigboardContentMode.Interaction_Heart: return MotionType.HeartMotion;
                case BigboardContentMode.Interaction_Lamplight: return MotionType.LampMotion;
                case BigboardContentMode.Interaction_Dish: return MotionType.PunchNKickMotion;
                case BigboardContentMode.Interaction_Ball: return MotionType.KickMotion;
                case BigboardContentMode.Interaction_Paints: return MotionType.ThrowMotion;
                default: return MotionType.None;
            }

        }

        public void SetImageType()
        {
            ImageType type = ImageType.None;
            switch (_currentMode)
            {
                case BigboardContentMode.Interaction_Dish:
                case BigboardContentMode.Interaction_Ball:
                case BigboardContentMode.Interaction_Heart:
                    type = ImageType.ParticleBody;
                    break;

                case BigboardContentMode.Interaction_Paints:
                    type = ImageType.ShadowBody;
                    break;

                case BigboardContentMode.Interaction_Lamplight:
                    if (ScreenUtil.screenType == ScreenType.Bigboard2x6)
                        type = ImageType.ParticleBody;
                    else
                        type = ImageType.Chromakey;
                    break;
            }

            _mapperView.SetImageType(type);
        }

        private void _ClearDictionary<T>(Dictionary<T, GameObject> list)
        {
            foreach (GameObject go in list.Values)
                Destroy(go);
            list.Clear();
        }

        #region AutoPlay
        Coroutine currentTimer;
        IEnumerator AutoPlayTimer()
        {
            float time = SettingsManager.autoPlayMinuteInteraction * 60f;
            yield return new WaitForSeconds(time);

            List<ContentsStoreItemInfo> list = BigboardServerDataManager.GetListUseContentsStoreItemInfo(ContentsStoreItemType.Mode.Interaction);
            ContentsStoreItemInfo curItem = BigboardServerDataManager.GetContentStoreItemInfo(_currentMode);
            if (curItem == null)
                curItem = list[list.Count - 1];

            int index = list.IndexOf(curItem);
            if (index >= 0)
            {
                ContentsStoreItemInfo nextItem = list[(index + 1) % list.Count];
                NextContent(nextItem.seq);
            }
            else
                Debug.LogWarning("Wrong Access Item");
        }

        private void AutoPlayStart()
        {
            AutoPlayStop();
            if (SettingsManager.enablesAutoPlayInteraction)
                currentTimer = StartCoroutine(AutoPlayTimer());
        }

        private void AutoPlayStop()
        {
            if (currentTimer != null)
            {
                StopCoroutine(currentTimer);
                currentTimer = null;
            }
        }
        #endregion
    }
}