//#define USE_TAG
#define USING_MEMORY_POOL

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

using UIController = TouchMotionUIController;
using ScreenUtil = ML.PlaywallKids.Common.ScreenUtil;

/// <summary>
/// 터치 슬라임 메인 콘트롤러
/// </summary>
public class TouchMotionGameController : MonoBehaviour
{
    /// <summary>
    /// 되돌아가기 씬 이름
    /// </summary>
    const string returnScene = "DragonParkCommonSelectMenu";

    enum EState 
    { NONE = 0, DEFAULT, WAIT_FOR_TAG, GUIDANCE ,NOTICE, READY, PLAY_MINION, PLAY_BOSS, TIME_OUT, KO, GAME_OVER , GENERATE_BOSS, SHAKE_CAMERA,  EVENT }

	#region PUBCLIC_VARIABLES
    /// <summary>
    /// BigboardContentMode.Touch_Slime
    /// </summary>
    public BigboardContentMode currentContent;
	public Camera mainCamera;
    public Camera NGUICamera;

    /// <summary>
    /// 보스 이벤트 씬 때 사용하는 파티클
    /// </summary>
    public ParticleSystem[] particles;

    /// <summary>
    /// 콘텐츠 내 사용 되는 슬라임 프리팹
    /// </summary>
	public TouchMotionSmallObject[]    smallObjPrefabs;           //Slimes
    /// <summary>
    /// 보스 슬라임 프리팹
    /// </summary>
    public TouchMotionBossObject       bossObjPrefab;             //KingSlime(Boss)
    /// <summary>
    /// 배럴 디스토션 참조, 스크린 입력 -> 배럴 왜곡 보정 입력
    /// </summary>
    public BarrelDistortionEffect      barrelEffect;              // is attached at MainCamera

    public AudioClip                   sndCountdown;
    public AudioClip                   sndBrokenGlass;
    public AudioClip                   sndThunder;

    /// <summary>
    /// 보스 이벤트가 있을 경울  이벤트 딜리게이트에 호출 함수 할당(터치 슬라임은 보스전 유, 터치 알파벳은 없음)
    /// </summary>
    public EventDelegate               bossEvent;
	public int score = 0;
#endregion
    #region Range
    [Range(0, 999)]
    public float playTime = 60;
	
	[Range(0.1f, 2.5f)]
	public float shakeDuration;
	[Range(0.1f, 2.5f)]
	public float shakeSensitive;
    /// <summary>
    /// 보스가 생성될 시간 비율 (0~1)
    /// </summary>
    [Range(0f,1f)]
    public float ratioBossActiveTime; //percentage of the playTime
    /// <summary>
    /// 콤보 간격시간
    /// </summary>
    [Range(0.1f, 20f)]
    public float intervalBetweenHit;
    [Range(0.1f, 1f)]
    public float effectVolume;
    #endregion

    #region Public variables
    /// <summary>
    /// 다음 슬라임 생성 시간
    /// </summary>
    public float genTime = 2.0f;
    /// <summary>
    /// 콘텐츠 경과 시간
    /// </summary>
    public float elapsedTime = 0;
    /// <summary>
    /// 콘텐츠 반복 플레이
    /// </summary>
    public bool loopingExcution;
    public bool useTouch;
    /// <summary>
    /// 슬라임 움직임 true : 베지어 커브, false : TweenPosition
    /// </summary>
    public bool useBezierMove;
    public bool BossTest;
    /// <summary>
    /// 카메라 흔들거리는 효과, ShakeCamera 속성내에서 사용
    /// </summary>
    public bool shakeCamera;
    /// <summary>
    /// 슬라임 생성 좌표
    /// </summary>
	public Vector3 genPosition = new Vector3(0.0f, 0.0f, 20.0f);
	public Vector3 viaPosition = new Vector3(0.0f, 0.0f, 10.0f);
    /// <summary>
    /// 목표 좌표
    /// </summary>
	public Vector3 toPosition  = Vector3.zero;
    /// <summary>
    /// 보스가 도달할 좌표
    /// </summary>
    public Vector3 bossPosition = Vector3.zero;
    /// <summary>
    /// 태깅, 준비 가이드
    /// </summary>
    public Sprite tagSprite, readySprite;
    /// <summary>
    /// 가이드 이미지 목록
    /// </summary>
    public Sprite[] guidanceSprites;
	#endregion

	#region Private variables
    /// <summary>
    /// 슬라임 종류 별 포획 수
    /// </summary>
    private int[] catchOutTheNumberOfObject;
    /// <summary>
    /// 레이캐스트를 위한 레이어
    /// </summary>
    private const int iTouchObject =  0x01 << ML.PlaywallKids.Common.LayerConstants.INTERACTION_OBJECT;
    private const float initPitch = 0.7f;
    /// <summary>
    /// 보스 슬라임 객체 참조
    /// </summary>
    private TouchMotionBossObject boss;
    /// <summary>
    /// 슬라임 메모리 풀
    /// </summary>
    private CObjectListToDic<int, TouchMotionSmallObject> objectDictionary;
    /// <summary>
    /// 이동하는 슬라임이 전부 도착하였는지 확인을 위한 컬렉션
    /// </summary>
    private List<BezierMove> listBezierMotors;
    /// <summary>
    /// UI 컨트롤러 참조
    /// </summary>
    private UIController ui;
    private Vector3 cameraPosition;
    private Vector2 minPos;
    private Vector2 maxPos;

	private float _genTime      = 0.0f;
	private float _genWaitTime  = 0.0f;
    /// <summary>
    /// 이전 터치 시간
    /// </summary>
    private float checkIntervalHit = 0f;
    private float fGenerateTimeBoss;
    /// <summary>
    /// 생성 너비
    /// </summary>
    private float _genRegionWidth;
    /// <summary>
    /// 콤보 카운트
    /// </summary>
    private int  cntHit        = 0;
    private int _bonusLife = 0;

	private bool _isPlaying     = false;
    private bool _bBossArrived  = false;
    /// <summary>
    /// 콤보 카운트 효과음 피치용 변수
    /// </summary>
    private bool bDecrease     = false;
    #endregion 

#if USE_TAG //MFC 카드 사용 시 
    #region USE_TAG
    private List<int> userList;

    #endregion
#endif

    #region Property
    private bool _bShakeCamera;
    private bool ShakeCamera
    {
        set
        {
            if(value != _bShakeCamera)
            {
                _bShakeCamera = value;

                if (value && shakeCamera)
                {
                    if (shakeDuration == 0) shakeDuration = 0.5f;
                    if (shakeSensitive == 0) shakeSensitive = 0.2f;
                    StartCoroutine(ShakeCameraProcess(shakeDuration, shakeSensitive));
                }
            }
        }
    }

    private EState _currentState;
    private EState CurrentState
    {
        get
        {   return _currentState;  }

        set
        {
            if (_currentState != value)
            {
                _currentState = value;
                switch(value)
                {
                    case EState.DEFAULT://각종 UI 초기화 단계
                        ui.result.RemoveEvent();
                        for (int i = 0; i < catchOutTheNumberOfObject.Length; ++i)
                            catchOutTheNumberOfObject[i] = 0;
                        ui.timeDisplay.Active = false;
                        ui.countDownDisplay.Active = false;

                        // 화면 페이드 인 효과
                        if (null != ui.fadeController)
                        {
                            ui.fadeController.Active = true;
                            ui.fadeController.alpha = 1f;
                            //페이드 인이 끝나면 콘텐츠 시작
                            ui.fadeController.FadeEffect(false, 0f, new EventDelegate(Play));
                        }
                        else
                            Play();
                        break;
#if USE_TAG
                    case EState.WAIT_FOR_TAG:
                        {
                            ui.guidance.Active = true;
                            ui.guidance.sprite.sprite2D = tagSprite;
                            ui.guidance.label.text = ML.PlaywallKids.Common.NFCConstants.DO_TAGGING;
                        }
                        break;
#endif

                    case EState.NOTICE: //현재는 사용안함
                        {
                            elapsedTime = 0f;
                            ui.countDownDisplay.Active = true;
                           // ui.countDownDisplay.transform.localPosition = new Vector3(0f,0f,0f);
                            ui.countDownDisplay.spriteName = "brown_";
                            ui.countDownDisplay.transform.localPosition = new Vector3(0f, 506f, 0f);
                            ui.countDownDisplay.ChangeNumeric(5);
                            ui.guidance.sprite.sprite2D = readySprite;
                            ui.guidance.label.text = @"앞으로 모이세요";
                        }
                        break;

                    case EState.GUIDANCE: //튜토리얼
                        StartCoroutine(GuidanceProcess());
                        break;

                    case EState.READY: // 게임 시작 전
                        ui.result.events = new EventDelegate(DisplayResult);
                        elapsedTime = 0;
                        if(ui.timeDisplay)
                            ui.timeDisplay.Active = false;
                        if(ui.scoreDisplay)
                            ui.scoreDisplay.Active = false;
                        if(ui.guidance)
                            ui.guidance.Active = false;
                        ui.countDownDisplay.transform.localPosition = Vector3.zero;
                        ui.countDownDisplay.spriteName = @"color_";
                        ui.countDownDisplay.Active = true;
                        ui.comboManager.DisappearDisplay();
                        ML.PlaywallKids.Common.AudioPlay2D.PlayClip(sndCountdown);
                        break;

                    case EState.PLAY_MINION: //체험중
                        StartCoroutine(CheckBezierMotor());
                        ui.countDownDisplay.Active = false;
                        ui.fadeController.gameObject.SetActive(false);
                        
                        ui.timeDisplay.InitTime((int)playTime);
                        ui.timeDisplay.Active =  true;

                        ui.countDownDisplay.Active = false;

                        ui.scoreDisplay.Active = true;
                        ui.scoreDisplay.Score = score;
                        
                        elapsedTime = 0;
                        break;
                    case EState.GENERATE_BOSS: //보스 이벤트
                        ui.timeDisplay.Active = false;
                        ui.scoreDisplay.Active = false;
                        ui.comboManager.DisappearDisplay();
                        _GenerateBoss();
                        break;

                    case EState.PLAY_BOSS: // 보스전
                        ui.timeDisplay.Active = true;
                        ui.scoreDisplay.Active = true;
                        break;

                    
                    case EState.TIME_OUT://시간제한
                        ui.result.Play(ui.result.objGameOver);
                        ui.timeDisplay.Active = false;
                        ui.scoreDisplay.Active = false;
                        ui.comboManager.DisappearDisplay();
                        //popupGameoverMenu.gameObject.SetActive(true); //restart
                        break;
                   
                    case EState.KO: //보스 포획 성공
                        ui.comboManager.DisappearDisplay();
                        ui.timeDisplay.Active = false;
                        ui.scoreDisplay.Active = false;
                        ui.result.Play(ui.result.objCongratulations);
                        //popupGameoverMenu.gameObject.SetActive(true);//restart
                        break;

                    case EState.GAME_OVER:
                        //ui.result.ShowResult(ui.result.objGameOver);
                        //popupGameoverMenu.gameObject.SetActive(true);//restart
                        break;
                }
            }
        }
    }

	BoxCollider mCol;
	BoxCollider col{
		get{
			if( mCol == null)
			{
				mCol = this.gameObject.GetComponent<BoxCollider>();
				if(mCol == null)
				{
					gameObject.AddComponent<BoxCollider>();
					mCol = gameObject.GetComponent<BoxCollider>();
					mCol.size = new Vector3(14f, 4.5f, 3);
				}
			}
			return mCol;
		}
	}

    AudioSource mAudio;
    public AudioSource audioSource
    {
        get
        {
            if(mAudio == null)
            {   mAudio = gameObject.GetComponent<AudioSource>();  }
            return mAudio;
        }
    }

	#endregion

	#region Unity Methods

	void Awake()
	{
#if !UNITY_EDITOR
		if(currentContent != BigboardContentMode.None)
		{
			object obj = SettingsManager.Load( currentContent.ToString() );
			if(obj !=null)
			{
				playTime =  int.Parse( obj.ToString());
				playTime = (playTime + 1) * 60f;
			}else{
				playTime = 60f;
			}
		}else{
			playTime = 60f;
		}
#endif
        listBezierMotors = new List<BezierMove>();
        catchOutTheNumberOfObject = new int[smallObjPrefabs.Length + 1];
        ui = UIController.instance;
        // 메모리 풀 초기화
        objectDictionary = new CObjectListToDic<int, TouchMotionSmallObject>(
            (int idx) =>
            {
                if (idx >= smallObjPrefabs.Length) idx = 0;
                TouchMotionSmallObject smallObject = (TouchMotionSmallObject)Instantiate(smallObjPrefabs[idx]);
                smallObject.gameObject.SetActive(false);
                return smallObject;
            },
            (TouchMotionSmallObject _obj)=>
            {   return _obj == null || !_obj.gameObject.activeInHierarchy;  }
            );

        //난이도 조절 용
        _bonusLife = BigboardServer.cachedSituationalInfo.targetOfAge == ContentsSIModelingInfo.StateTargetOfAge.School ? 1 : 0;

#if USE_TAG
        switch( currentContent)
        {
            case BigboardContentMode.Touch_AlphabetGame:
            case BigboardContentMode.Touch_Slime:
               
                userList = new List<int>();
          //      NFCClientSocket.instance.ContentType = ML.PlaywallKids.Common.NFCConstants.VALUE_TOUCH_SLIME;
                break;
        }
#endif
	}

	void Start () {
		if(mainCamera == null)
			mainCamera = Camera.main;

        if (intervalBetweenHit == 0) intervalBetweenHit = 4f;
        _genRegionWidth = ScreenUtil.NGUIWidth * 0.02f;
        //전체 체험 시간 중 보스 생성 비율에 따라 보스 등장 시간이 달라짐
        fGenerateTimeBoss = playTime - (int)(playTime * ratioBossActiveTime);
		col.center = toPosition;
    
    }

    void OnEnable()
    {
        Vector3 pos = toPosition + bossPosition;
        minPos = mainCamera.ViewportToWorldPoint(new Vector3(0.1f, 0.35f, pos.z));
        maxPos = mainCamera.ViewportToWorldPoint(new Vector3(0.9f, 0.55f, pos.z));
        CurrentState = EState.DEFAULT;
    }

    void OnTriggerEnter(Collider _other)
    {
        ShakeCamera = true; 
    }

    void OnCollisionEnter(Collision  _other)
    {
        ShakeCamera = true; 
    }
 
	public void Update() {

		if(_isPlaying) // 체험 중
        {
            int iTime = 0;
            if (useTouch) _PerformTouches();
            switch(CurrentState)
            {
                case EState.NONE: //최초 진입 단계
                    CurrentState = EState.DEFAULT;
                    break;

#if USE_TAG
                case EState.DEFAULT:
                    CurrentState = EState.WAIT_FOR_TAG;
                    break;

                case EState.WAIT_FOR_TAG:
                    if (CheckUser())
                    {
                        CurrentState = EState.NOTICE;
                    }
                    break;

                case EState.NOTICE:
                    {
                        CheckUser();
                        int iWaitTime = 5;
                        iTime = TickTock();
                        if (iWaitTime - iTime >= 0)
                        {
                            ui.countDownDisplay.ChangeNumeric(iWaitTime - iTime);
                        }
                        else
                        {
                            ui.countDownDisplay.Active = false;
                            CurrentState = EState.GUIDANCE;
                        }
                    }
                    break;
#else
                    case EState.DEFAULT:
                        CurrentState = EState.READY;
                    break;
#endif
                case EState.READY://체험시작전
                    {
                        int iWaitTime = 3;
                        iTime = TickTock();
                        
                        if (!ui.countDownDisplay.Active) ui.countDownDisplay.Active = true;
                        //카운트 다운
                        ui.countDownDisplay.ChangeNumeric(iWaitTime - iTime);

                        if (iWaitTime - iTime <= 0)
                        { CurrentState = EState.PLAY_MINION; }
                        _GenerateSmallObjects();
                    }
                    break;

                case EState.PLAY_MINION://체험 중
                    iTime = TickTock();

                    //UI 체험시간 바
                    ui.timeDisplay.ChangeTime((int)(playTime - iTime));

                    // 보스 생성 시간 체크
                    if (fGenerateTimeBoss > elapsedTime)
                    {  
                        // 슬라임 생성
                        _GenerateSmallObjects();  
                    }
                    else //보스 생성 시간
                    {
                        foreach(var key in objectDictionary.dictionary.Keys)
                        {
                            List<TouchMotionSmallObject> tempList = objectDictionary.dictionary[key];
                            for (int i = 0, len = tempList.Count; i < len; ++i)
                            {
                               if( tempList[i].gameObject.activeInHierarchy)
                               {
                                   //보스가 생성 전 현재 공간 내 단 한마리의 슬라임 확인
                                   tempList = null;
                                   return;
                               }
                            }
                            tempList = null;
                        }

                        if (ratioBossActiveTime == 0f) // 보스전이 없다는 의미
                        { CurrentState = EState.GAME_OVER; }
                        else 
                        { CurrentState = EState.GENERATE_BOSS; }
                    }
                    break;

                case EState.PLAY_BOSS: // 보스전
                    iTime = TickTock();
                    if (playTime - iTime >= 0)
                        ui.timeDisplay.ChangeTime((int)(playTime - iTime)); 
                    break;
            }
            
	
            if (elapsedTime >= playTime && !BossTest) Stop();
        }
        else // 결과 창 출력 단계
        {
            switch (CurrentState)
            {
                case EState.GAME_OVER:

                    break;
                case EState.TIME_OUT:
                    CurrentState = EState.GAME_OVER;
                    break;
                case EState.KO:
                    CurrentState = EState.GAME_OVER;
                    break;
            }
        }
	}

    /// <summary>
    /// 사용자에게 체험 방법 알리는 단계, 현재 사용은 안함, CuirrentState = Estat.GUIDANCE 할당 하면 됨
    /// </summary>
    /// <returns></returns>
    IEnumerator GuidanceProcess()
    {
        string[] _words = new string[] {"나쁜놈을", "때.려.눕.히.자" , "준 비~!"};
        UI2DSprite _image = ui.guidance.sprite;
        int _flagImage = 0;
        int _flagWait = 0;
        for(int i = 0 ; i < 3 ; ++i)
        {
            float _t = 0f;
            do
            {
                _t += Time.deltaTime * 1.5f;
                float _scale =Mathf.Sin(_t * Mathf.PI);
                _image.cachedTransform.localScale = new Vector3(_scale, 1f, 1f);
                if ((_flagImage & 0x01 << i) == 0 &&  _scale < 0.15f)
                {
                    _image.sprite2D = i < guidanceSprites.Length ? guidanceSprites[i] : null;
                    ui.guidance.label.text = _words[i];
                    _flagImage |= 0x01 << i;
                }
                else if ( (0x01 << i &_flagWait) == 0&& 0.995f < _scale )
                {
                    _flagWait |= 0x01 << i;
                    yield return new WaitForSeconds(2f);
                }
                yield return null;
            } while (_t < 1.0f);

            _image.cachedTransform.localScale = Vector3.one;
        }
        CurrentState = EState.READY;
    }
  
    IEnumerator LoadSceneProcess()
    {
        yield return new WaitForSeconds(3.5f);
        ui.fadeController.Active = true;
        ui.fadeController.FadeEffect(true, 1f);
        yield return new WaitForSeconds(1.5f);

        AsyncOperation async = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        while (!async.isDone)
            yield return null;
    }
  
    /// <summary>
    /// 보스 등장 이벤트
    /// </summary>
    /// <returns></returns>
    IEnumerator GenerateBossEventProcess()
    {
		Light[] _lights = GameObject.FindObjectsOfType<Light>();
	    Vector3 _oriPos = mainCamera.transform.localPosition;
        Vector3 _oriRotate = mainCamera.transform.localEulerAngles;
        float _fov = mainCamera.fieldOfView;

		float _speed = 4f;
        float _emittTime = 0f;
        float _activeTIme = 0f;
        if (sndThunder != null)
            ML.PlaywallKids.Common.AudioPlay2D.PlayClip(sndThunder);

        List<GameObject> tempList = new List<GameObject>();

        while (!_bBossArrived) //보스가 도착 하였는지 확인
        {
            _activeTIme += Time.deltaTime;
            if (_activeTIme > _emittTime)
            {
                _emittTime = Random.Range(0.1f, 0.3f);
                _activeTIme = 0f;

                for(int i = 0 ; i< particles.Length ; ++i) //보스 주변 특수효과
                {
                    ParticleSystem p = Instantiate(particles[i])as ParticleSystem;
                    p.transform.parent = boss.cachedTransform;
                    p.transform.localPosition = new Vector3( Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f) );
                    p.transform.localEulerAngles = new Vector3(-90f,0f, 0f);
                    p.Play();
                    tempList.Add(p.gameObject);
                }
            }
            
			for(int i = 0 ; i< _lights.Length ; ++i) // 게임 씬 내 어둡게
			{
                if (_lights[i] == null) continue;
                if (_lights[i].intensity > 0.5f) _lights[i].intensity -= Time.deltaTime;  
            }

            //mainCamera.transform.LookAt(boss.transform);
			Vector3 target = boss.transform.forward * _speed + new Vector3(0, 2f,0);
			mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, target, _speed * Time.deltaTime);

			if(mainCamera.fieldOfView > 20f) mainCamera.fieldOfView -= _speed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < tempList.Count; ++i )
        {   DestroyImmediate(tempList[i]);  }
        tempList = null;
        _speed = 3f;

		bool eventEnd = false;

		do{
            for (int i = 0; i < _lights.Length; ++i) // 게임 씬 내 빛을 다시 밝게
			{
                if (_lights[i] == null) continue;
				if(_lights[i].intensity < 1f) _lights[i].intensity += 2f* Time.deltaTime;
				else _lights[i].intensity = 1f;
            }

			Vector3 _pos = Vector3.Lerp(mainCamera.transform.localPosition, _oriPos, _speed * Time.deltaTime);
			Vector3 _angle = Vector3.Lerp(mainCamera.transform.localEulerAngles, _oriRotate, _speed * Time.deltaTime);
            mainCamera.transform.localPosition = _pos;
			_angle.y= 0f;
			mainCamera.transform.localEulerAngles  = _angle;
			if(mainCamera.fieldOfView  < _fov) mainCamera.fieldOfView += Time.deltaTime * _speed;
			else mainCamera.fieldOfView = _fov;
			Vector3 value = (_pos - _oriPos ) + (_angle - _oriRotate);
			eventEnd =  ((value == Vector3.zero )&&  (mainCamera.fieldOfView - _fov == 0) );
			yield return new WaitForEndOfFrame();
		}while(!eventEnd);

		mainCamera.transform.localPosition = _oriPos;
		mainCamera.transform.localEulerAngles = _oriRotate;
    }

    /// <summary>
    /// 슬라임 움직임 이동 체크 하여 카메라 전면까지 이동하였는지 확인, 전면에 도달 하였으면 카메라에 깨진 유리 이펙트 생성
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckBezierMotor()
    {
        GameObject _go = this.gameObject;
        do
        {
            for (int i = 0, len = listBezierMotors.Count; i < len; )
            {
                if (!listBezierMotors[i].IsArrived)
                    ++i;
                else
                {
                    if (sndBrokenGlass != null)
                        ML.PlaywallKids.Common.AudioPlay2D.PlayClip(sndBrokenGlass);

                    --len;
                    Vector3 _view = mainCamera.WorldToViewportPoint(listBezierMotors[i].transform.position);
                    _view.z = 0f;
                    if (ui.brokenGlassManager)
                        ui.brokenGlassManager.Display(NGUICamera, _view);
                    listBezierMotors.RemoveAt(i);
                }
            }
            yield return null;
        } while (_go.activeInHierarchy);
    }

    IEnumerator ShakeCameraProcess( float duration, float sensitive)
    {
        float fDelatTime = 0;
        cameraPosition = mainCamera.transform.localPosition;
        
        while(duration > fDelatTime)
        {
            fDelatTime += Time.deltaTime;
            Vector3 temp = Vector3.zero;

            mainCamera.transform.localPosition = cameraPosition;
            temp.x = Random.Range(-sensitive, sensitive);
            temp.y = Random.Range(-sensitive, sensitive);

            mainCamera.transform.localPosition += temp;
            yield return new WaitForEndOfFrame();
        }
        ShakeCamera = false;
        mainCamera.transform.localPosition = cameraPosition;
    }

    /// <summary>
    /// 게임 결과 출력 이벤트
    /// </summary>
    /// <returns></returns>
    IEnumerator DisplayResultProcess()
    {
        yield return new WaitForSeconds(5f);
        ui.result.objCongratulations.gameObject.SetActive(false);
        ui.result.objGameOver.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        if(ui.result && ui.result.objResult)
        {
            TouchMotionResultDisplay _result = ui.result.objResult.GetComponent<TouchMotionResultDisplay>();

            if (_result.points == null || _result.points.Length == 0) _result.points = new int[2 + catchOutTheNumberOfObject.Length];

            int _sum = 0;
            for (int i = 0, len = catchOutTheNumberOfObject.Length; i < len; ++i)
            {
                _result.points[i] = catchOutTheNumberOfObject[i];
                _sum += _result.points[i];
            }
            _result.points[catchOutTheNumberOfObject.Length] = _sum;
            _result.points[_result.points.Length - 1] = score;
            ui.result.RemoveEvent();
            ui.result.Play(_result.gameObject);
            _result.Display();
        }

        yield return new WaitForSeconds(5f);
        if (loopingExcution)
        {
            Restart();
        }else
            ExitGame();
    }
#if UNITY_EDITOR
    public void OnDrawGizmos() {
		Vector3 size = new Vector3(_genRegionWidth, 2.0f, 1.0f);
		Vector3 size2 = new Vector3(_genRegionWidth * 0.5f, 2.0f, 1.0f);
		Vector3 size3 = new Vector3(_genRegionWidth * 0.08f, 2.0f, 1.0f);
		
		Gizmos.color = new Color(0.0f, 0.0f, 1.0f, 0.25f);
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawCube(genPosition, size);
		
		Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.25f);
		Gizmos.DrawCube(viaPosition, size2);
		
		Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 0.25f);
		Gizmos.DrawCube(toPosition, size3);


		UnityEditor.Handles.color = Color.white;

		UnityEditor.Handles.matrix = transform.localToWorldMatrix;
		UnityEditor.Handles.Label(genPosition + Vector3.up * 3.0f, "벌레들이 여기에 생성됩니다.");
		UnityEditor.Handles.ArrowHandleCap(0, genPosition + Vector3.up * 2.0f, Quaternion.Euler(90, 0, 0), 1.5f, EventType.Repaint);

		UnityEditor.Handles.Label((viaPosition + genPosition) * 0.5f + Vector3.up * 3.0f, "이동 경로 1");
		//UnityEditor.Handles.ArrowCap(0, viaPosition * 0.35f + genPosition * 0.65f, Quaternion.FromToRotation(genPosition, viaPosition), (viaPosition - genPosition).magnitude * 0.3f);

		UnityEditor.Handles.Label(viaPosition + Vector3.up * 3.0f, "경유 지점");
		UnityEditor.Handles.ArrowHandleCap(0, viaPosition + Vector3.up * 2.0f, Quaternion.Euler(90, 0, 0), 1.5f, EventType.Repaint);

		UnityEditor.Handles.Label((viaPosition + toPosition) * 0.5f + Vector3.up * 3.0f, "이동 경로 2");
		//UnityEditor.Handles.ArrowCap(0, viaPosition * 0.65f + toPosition * 0.35f, Quaternion.FromToRotation(viaPosition, toPosition), (viaPosition - toPosition).magnitude * 0.3f);

		UnityEditor.Handles.Label(toPosition + Vector3.up * 3.0f, "도착 지점");
		UnityEditor.Handles.ArrowHandleCap(0, toPosition + Vector3.up * 2.0f, Quaternion.Euler(90, 0, 0), 1.5f, EventType.Repaint);
	}
#endif
    #endregion

    #region Game Play

    public void Play() {
		if(!_isPlaying) {
            if (effectVolume == 0) effectVolume = 0.7f;

            elapsedTime      = 0;
            checkIntervalHit = 0;
            
            _isPlaying     = true;
            _bBossArrived  = false;
           
            audioSource.pitch = initPitch;
            audioSource.volume = effectVolume;
            

#if !USE_TAG
            ui.countDownDisplay.Active = true;
            CurrentState   = EState.READY;
#endif

        }
	}

    public void GenerateBossEvent()
    { StartCoroutine(GenerateBossEventProcess()); }

	public void Stop() {
		if(_isPlaying) {
			_isPlaying = false;

			var bugs = GetComponentsInChildren<TouchMotionSmallObject>();
			foreach(var bug in bugs) {
				bug.Die();
			}
            if (boss!= null)
            {   boss.Die();  }
            CurrentState = EState.TIME_OUT;
		}
	}

    public Vector3 WorldToBarrelScreen(Vector3 pos)
    {
        pos = mainCamera.WorldToViewportPoint(pos);
        pos = mainCamera.ViewportToScreenPoint(pos);
        pos = barrelEffect.GetOriginalScreenPosFromDistorted(pos);
        pos = mainCamera.ScreenToViewportPoint(pos);
        pos = NGUICamera.ViewportToWorldPoint(pos);
        pos.z = 0f;
        return pos;
    }

    public void Restart()
    {
        StartCoroutine(LoadSceneProcess()); 
    }

	public void ExitGame()
	{ StartCoroutine(LoadSceneProcess()); }

    private void DisplayResult()
    {
#if USE_TAG
        SendDataSet();
#endif
        StartCoroutine(DisplayResultProcess());
    }

#if USE_TAG
    
    private void SendDataSet()
    {
        StartCoroutine(SendSataSetProcess());
    }

    IEnumerator SendSataSetProcess()
    {
        for (int i = 0, len = userList.Count; i < len; ++i)
        {
            int _seq = userList[i];

            NFCUserInfo.SendData( _seq, score);
            yield return null;
        }
    }

    private bool CheckUser()
    {
        if (NFCClientSocket.instance.Count > 0)
        {
            string _value = NFCClientSocket.instance.GetStringValue();
            Dictionary<string, object> _dic = MiniJSON.Json.Deserialize(_value) as Dictionary<string, object>;

            bool bUserName = _dic.ContainsKey(ML.PlaywallKids.Common.NFCConstants.KEY_USER_NAME);
            bool bSeq = _dic.ContainsKey(ML.PlaywallKids.Common.NFCConstants.KEY_CONTENTS_SEQ);
            if (bUserName & bSeq)
            {
                string _name = _dic[ML.PlaywallKids.Common.NFCConstants.KEY_USER_NAME].ToString();
                GameObject _go = NGUITools.AddChild(ui.gameObject, ui.popupWindow.gameObject);
                _go.transform.localScale = Vector3.zero;
                _go.transform.localPosition = ui.popupWindow.transform.localPosition;
                _go.SetActive(true);
                _go.GetComponent<UITouchMotionGuidance>().label.text = string.Format("{0}친구 환영해요", _name);
                TweenPosition _tweemPos = TweenPosition.Begin(_go, 0.5f, _go.transform.localPosition - new Vector3(0, 500f, 0));
                TweenScale _tween = TweenScale.Begin(_go, 1f, Vector3.one);
                _tween.onFinished.Add(new EventDelegate(() => Destroy(_tween.gameObject, 1f)));
                int _seq = int.Parse( _dic[ML.PlaywallKids.Common.NFCConstants.KEY_CONTENTS_SEQ].ToString() );
                if (!userList.Contains(_seq))
                    userList.Add(_seq);
            }
            return true;
        }
        return false;
    }
#endif

    /// <summary>
    /// 보스 생성 및 보스 설정 초기화
    /// </summary>
    private void _GenerateBoss()
    {
        Vector3 pos = toPosition + bossPosition;
        genPosition.y = minPos.y;

        boss = (TouchMotionBossObject)Instantiate(bossObjPrefab);
        boss.gameObject.SetActive(true);
        boss.transform.parent = transform;
        boss.transform.localPosition = genPosition;
        boss.transform.localRotation = Quaternion.Euler(0, 180f, 0); 
        boss.transform.localScale = boss.transform.localScale;
        boss.SetArea(minPos.x, maxPos.x, minPos.y, maxPos.y);


        if (bossEvent != null) bossEvent.Execute();
        //StartCoroutine(GenerateBossEventProcess());
        TweenPosition tp = TweenPosition.Begin(boss.gameObject, 3.5f, viaPosition);
        tp.onFinished.Add(new EventDelegate(() =>
        {   
            TweenPosition tp2 = TweenPosition.Begin(boss.gameObject, 3f, pos);
            
            tp2.onFinished.Add(new EventDelegate(() =>
            {
                if (_bBossArrived) return;
                {
                    if (boss.cam == null)
                    { boss.cam = this.mainCamera; }
                    _bBossArrived = true;
                    boss.Ready();
                    CurrentState = EState.PLAY_BOSS;
                }
            }));
        }
        )
        );
    }

    /// <summary>
    /// 슬라임 스폰 함수
    /// </summary>
	private void _GenerateSmallObjects() {
		if(_genWaitTime >= _genTime) {
			_genTime = genTime * Random.Range(2f, 3f) / ((elapsedTime * 2.0f + playTime) / playTime );
			_genWaitTime = 0.0f;
			
		//	Debug.Log(string.Format("Bug Game - The next bug will be generated in {0:0.00} seconds.", _genTime));
		}
		else {
			_genWaitTime += Time.deltaTime;

			if(_genWaitTime >= _genTime && smallObjPrefabs.Length > 0) {
#if USING_MEMORY_POOL
                int idx = Random.Range(0, smallObjPrefabs.Length);
                TouchMotionSmallObject smallObject = objectDictionary.getObject(idx);
                smallObject.bDestroy = false;

                if (smallObject.render != null)
                    smallObject.render.materials[0].SetFloat("_Risks", 0);
#else
      			int idx = Random.Range(0, smallObjPrefabs.Length);
				TouchMotionSmallObject prefab = smallObjPrefabs[idx];
				TouchMotionSmallObject smallObject = (TouchMotionSmallObject)Instantiate(prefab);
                smallObject.transform.localScale    = prefab.transform.localScale;
                smallObject.bDestroy  = true;
#endif
                if (smallObject == null) return;

                float wOffset = Random.Range (-_genRegionWidth * 0.5f, _genRegionWidth * 0.5f);
				smallObject.gameObject.SetActive(true);
                smallObject.life += _bonusLife;
				smallObject.transform.parent        = transform;
                Vector3 pos= genPosition + Vector3.right * wOffset * 3f;
                pos.y += Random.Range(-1f, 4f);
                smallObject.transform.localPosition = pos;
                smallObject.transform.localRotation = Quaternion.Euler(0, 180f, 0);
                smallObject.cam                     = this.mainCamera;
                smallObject.nguiCam                 = this.NGUICamera;
                
                if (useBezierMove)
                {
                    BezierMove bezier = smallObject.gameObject.GetComponent<BezierMove>();
                    bezier.target = toPosition + Vector3.right * wOffset * 0.085f;
                    
#if USING_MEMORY_POOL
                    bezier.disableType = BezierMove.EType.DISABLE;
#else
      			    bezier.disableType = BezierMove.EType.DESTROY;
#endif
                    listBezierMotors.Add(bezier);
                }
                else
                {
                    TweenPosition tp = TweenPosition.Begin(smallObject.gameObject, smallObject.moveTime * 0.5f, viaPosition + Vector3.right * wOffset * 0.5f);
                    tp.onFinished.Add(new EventDelegate(() =>
                    {
                        TweenPosition tp2 = TweenPosition.Begin(smallObject.gameObject, smallObject.moveTime * 0.5f, toPosition + Vector3.right * wOffset * 0.92f);
                        tp2.onFinished.Add(new EventDelegate(() =>
                        {
                            smallObject.Die(true);
                        }));
                    }));
                }
          //      Debug.Log(string.Format("Bug Game - The new bug {0} is generated.", bug.name));
			}
		}
	}
    
    /// <summary>
    /// 사용자 터치입력
    /// </summary>
	private void _PerformTouches() {
		if(mainCamera == null) return;

		for(int i = 0; i < CustomInput.touchCount; i++) {
			TouchInfo touch = CustomInput.GetTouch(i);

			if(touch.phase == TouchInfo.Phase.Begin) {
                //스크린 좌표 -> 베럴 디스토션 보정 좌표
                Vector2 inputPos = barrelEffect.GetDistoredScreenPosFromOriginal(touch.position);
                Ray ray = mainCamera.ScreenPointToRay(inputPos);
				RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 200f, iTouchObject))
                {
   					if(hitInfo.collider != null &&  hitInfo.collider.gameObject != null )
                    {
                        if (checkIntervalHit < intervalBetweenHit)
                        {
                            ++cntHit;
                            ui.comboManager.DisplayCombo(cntHit);
                            
                            // 콤보 효과음 피치 조절
                            if (cntHit % 10 == 0)
                            { bDecrease = !bDecrease; }
                            audioSource.pitch += (bDecrease ? -0.1f : 0.1f);
                        }
                        audioSource.Play();
                        checkIntervalHit = 0f;

                        if(hitInfo.collider.gameObject.GetComponent<TouchMotionSmallObject>() != null)
                        {
                            Vector3 pos = WorldToBarrelScreen(hitInfo.point);
                          //  pointManager.ActivateNum(pos, _cntHit);
                            TouchMotionSmallObject target = hitInfo.collider.gameObject.GetComponent<TouchMotionSmallObject>();
                            if (target.Hit())
                            {
                                int n = target.Score;
                                string _name = "Slime";
                                int _index = target.name.IndexOf(_name);
                                string _Kind = target.name.Substring(_index + _name.Length);
                                switch(_Kind)
                                {
                                    case "Small(Clone)":
                                        catchOutTheNumberOfObject[0] += 1;
                                        break;
                                    case "Middle(Clone)":
                                        catchOutTheNumberOfObject[1] += 1;
                                        break;
                                    case "Big(Clone)":
                                        catchOutTheNumberOfObject[2] += 1;
                                        break;
                                        
                                }
                                if (ui.pointManager)
                                    ui.pointManager.DisplayScore(pos, n);
                                score += n;
                                ui.scoreDisplay.Score = score;
                            }
                        }
                        else if (hitInfo.collider.gameObject.GetComponent<TouchMotionBossObject>() != null)
                        {
                            TouchMotionBossObject bugBoss = hitInfo.collider.gameObject.GetComponent<TouchMotionBossObject>();
                            if (bugBoss.Hit())
                            {
                                catchOutTheNumberOfObject[3] += 1;
                                score += 100;
                                CurrentState = EState.KO;
                            }
                            else
                            {   score += 1;  }

                            ui.scoreDisplay.Score = score;
                        }else
                        {
                            Debug.Log("hitInfo.collider.gameObject name : " + hitInfo.collider.gameObject.name);
                        }
                    }
				}
			}
		}
	}

    /// <summary>
    /// 콘텐츠 체험 시간 계산
    /// </summary>
    /// <returns></returns>
    private int TickTock()
    {
        checkIntervalHit += Time.deltaTime;
        if (checkIntervalHit >= intervalBetweenHit)
        {
            cntHit = 0;
            checkIntervalHit = 0;
            bDecrease = false;
            audioSource.pitch = initPitch;
            ui.comboManager.DisappearDisplay();
        }

        elapsedTime += Time.deltaTime;
        return (int)elapsedTime;
    }

	#endregion
}


/*
   private void _PerformTouches() {
        if(mainCamera == null) return;

        for(int i = 0; i < CustomInput.touchCount; i++) {
            TouchInfo touch = CustomInput.GetTouch(i);

            if(touch.phase == TouchInfo.Phase.Begin) {
                Vector2 inputPos = barrelEffect.GetDistoredScreenPosFromOriginal(touch.position);
                Ray ray = mainCamera.ScreenPointToRay(inputPos);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 200f, iTouchObject))
                {
                    if(hitInfo.collider != null &&  hitInfo.collider.gameObject != null )
                    {
                        if(hitInfo.collider.gameObject.GetComponent<TouchMotionBugObject>() != null)
                        {
                            TouchMotionBugObject bug = hitInfo.collider.gameObject.GetComponent<TouchMotionBugObject>();

                            if (bug.Hit())
                            {
                                int n = bug.Score;
                                Vector3 pos = mainCamera.WorldToViewportPoint(hitInfo.point);
                                Vector3 screenPos = barrelEffect.GetOriginalScreenPosFromDistorted(mainCamera.ViewportToScreenPoint(pos));
                                pos = mainCamera.ScreenToViewportPoint(screenPos);
                                pos = NGUICamera.ViewportToWorldPoint(pos);
                                pos.z = 0f;
                                pointManager.ActivateNum(pos, n);
                                score += n;
                                scoreDisplay.ChangScore(UINumericSplit.NumericSplit(score));
                            }
                        }
                        else if (hitInfo.collider.gameObject.GetComponent<TouchMotionBossObject>() != null)
                        {
                            TouchMotionBossObject bugBoss = hitInfo.collider.gameObject.GetComponent<TouchMotionBossObject>();
                            if (bugBoss.Hit())
                            {
                                score += 100;
                                scoreDisplay.ChangScore(UINumericSplit.NumericSplit(score));
                                currentState = EState.KO;
                           
                                
                            }
                        }else
                        {
                            Debug.Log("hitInfo.collider.gameObject name : " + hitInfo.collider.gameObject.name);
                        }
                    }
                }
            }
        }
    }
 */
 


