using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.Pool;
using System.Threading;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;

namespace Bax.P0.Client.UnityWorld.SlicerGame
{
    public class SlicerMgr : MonoBehaviour, Game.IMyGameActions
    {
        public static SlicerMgr instance;

        #region 상태패턴으로 컨트롤 할 변수들 
        public GameUI gameUI;
        public ScreenProsess screenProsess;
        public EnumClass stateClass;
        #endregion


        [Header("음식 프리팹")]
        public Food food;                   //과일 생성할 프리팹

        [Header("점수UI 프리팹")]
        public Log log;                     //텍스트 생성할 프리팹

        [Header("Canvas")]
        public Canvas canvas;               //텍스트 로드 생성시 부모로 넣을 캔버스 

        
        public ParticleSystem punchEfx;     //이팩트 프리팹

        public ObjectPool<Food> foodPulling;    //과일 풀링
        public LogManager<Log> logManager;      //텍스트 풀링용

        //생성한 과일들 저장 및 제거 용
        public List<Food> foodList = new List<Food>();

        public int stage;           //시간이 지날수록 증가

        #region 과일 텍스트
        public TextMeshProUGUI appleText;
        public TextMeshProUGUI watermelonText;
        public TextMeshProUGUI kiwiText;
        public TextMeshProUGUI orangeText;
        #endregion
        #region 쪼갠 과일 카운팅 한 프로퍼티들 
        private float appleCnt;
        public float AppleCnt
        { 
            get => appleCnt;
            set 
            {
                appleCnt = value;
                appleText.text = appleCnt.ToString();
            }
        }

        private float watermelonCnt;
        public float WaterMelonCnt
        { 
            get => watermelonCnt;
            set 
            {
                watermelonCnt = value;
                watermelonText.text = watermelonCnt.ToString();
            }
        }

        private float orangeCnt;
        public float OrangeCnt
        { 
            get => orangeCnt;
            set 
            {
                orangeCnt = value;
                orangeText.text = orangeCnt.ToString();
            }
        }

        private float kiwiCnt;
        public float KiwiCnt
        { 
            get => kiwiCnt;
            set 
            {
                kiwiCnt = value;
                kiwiText.text = kiwiCnt.ToString();
            }
        }

        #endregion

        //Unitask 정지용
        public CancellationTokenSource _sources = new();

        //이미지 로드용
        [NonSerialized] public LoadSprite loadSprite;
        //new InputSystem PC
        public Game InputGame;

        public ZoZoBasePatton<FerrisMgr> zozo = new ZoZoBasePatton<FerrisMgr>();
        float spawnTime = 1f;
        TimerMgr timerMgr;
        private void OnEnable()
        {
            
            if (_sources != null) _sources.Dispose();
            _sources = new();

            //InputSystem 세팅
            InputGame = new Game();
            InputGame.MyGame.Enable();
            InputGame.MyGame.SetCallbacks(this);

            //스크린용터치용 세팅
            EnhancedTouchSupport.Enable();
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += onFingerDown;

            #region 공용 스테이트 패턴 
            ActionProcess.Enter_StateListener(
            null, 
            null,
            () => 
            {
                //게임 타이머 생성 UI 표시 
                timerMgr = new TimerMgr(60f
                                        , gameUI.timerText,
                                        () => { zozo.Change(GameState.GameResult); }
                                        , stateClass, false);
                //과일 출몰하는 난이도
                stage = 1;
                //음식(과일&고기) 들 소환위치 지정
                spawnTimer().Forget();
            },
            () => 
            {
                foreach (var item in foodList)
                {
                    foodPulling.Release(item);
                }
                //과일 리스트 비움
                foodList.Clear();
            });
           

            zozo = new ZoZoBasePatton<FerrisMgr>();
            zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
            #endregion
        }

        private async UniTaskVoid spawnTimer()
        {
            await UniTask.Yield();
            while (stateClass.state == GameState.GamePlay && timerMgr.late >= 0)
            {
                for (int i = 0; i < stage; i++)
                    spawnFood();

                await UniTask.Delay(TimeSpan.FromSeconds(spawnTime), DelayType.UnscaledDeltaTime, cancellationToken: _sources.Token);

                //1 ~ 6   10~70

                // 10 ~ 20  1
                // 20 ~ 30  2
                // 30 ~ 40  3
                // 40 ~ 60  4
                // 60 ~ 70  5
                int time = Mathf.RoundToInt(timerMgr.time - timerMgr.late) + 10;

                stage = time < 50f ? time / 10 : (time / 10) - 1;
            }
        }

        //과일스폰
        private async void spawnFood()
        {
            //랜덤한 각도 세팅
            float angle = UnityEngine.Random.Range(0f, 359f);

            //랜덤한 각도로 원형으로 포지션 저장
            Vector2 spawnPosition = new Vector2
            (
                Mathf.Cos(angle) * Mathf.Rad2Deg * 0.2f,
                Mathf.Sin(angle) * Mathf.Rad2Deg * 0.2f
            );

            await UniTask.Yield(cancellationToken: _sources.Token);
            //과일 오브젝트 생성
            var food = foodPulling.Get();
            //생성위치 저장
            food.createPos = spawnPosition;
            //현재위치 저장
            food.transform.position = spawnPosition;
            //랜덤하게 과일 이미지 로드 
            food.SpriteSetting(UnityEngine.Random.Range(0, (int)FoodKind.MAX));
            //과일 리스트 저장
            foodList.Add(food);

            //랜덤 각도 저장
            float angle2 = UnityEngine.Random.Range(0, 360);
            //바라볼 각도 랜덤계산
            angle = food.Dir
                (
                    new Vector2
                    (
                        Mathf.Cos(angle2) * Mathf.Rad2Deg * 0.08f,
                        Mathf.Sin(angle2) * Mathf.Rad2Deg * 0.08f
                    )
                );
            //각도 저장
            food.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            //이동 속도를 stage값만큼 계산
            food.moveSpeed = 7 + stage;
            food.isDown = false;

            // food.createPos = food.transform.position;
        }

        private void OnDisable()
        {
            _sources.Cancel();
            ReadyProcess.sourceCancle?.Invoke();

            InputGame.MyGame.Disable();

            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= onFingerDown;
            EnhancedTouchSupport.Disable();

        }

        private void OnDestroy()
        {
            _sources.Cancel();
            _sources.Dispose();
            ReadyProcess.sourceCancle?.Invoke();
            ReadyProcess.sourceDispone?.Invoke();
        }

        //스크린터치
        private void onFingerDown(Finger obj)
        {
            Down(obj.currentTouch.screenPosition);
        }
        //Pc 마우스 Down
        public void OnDown(InputAction.CallbackContext context)
        {
            if (Settings.instance.mouseToggle.isOn == false) return;
            if (context.ReadValue<float>() == 1f)
                Down(Settings.instance.MousePos());
        }
        //Touch Screen
        public void OnTouch(InputAction.CallbackContext context)
        {
        }
        public void OnIsDown(InputAction.CallbackContext context)
        {
        }


        //터치
        private async void Down(Vector2 fingerPos)
        {
            if (stateClass.state == GameState.GamePlay)
            {
                //터치한 위치로 레이캐스트  
                var hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(fingerPos), Vector2.zero);

                if (hit2D)
                {
                    //터치한 위치에 과일이 있다면
                    if (hit2D.transform.TryGetComponent<Food>(out var food))
                    {
                        food.isDown = true;
                        //과일 사운드 play
                        SoundMGR.Instance.SoundPlay("08.과일자르기");
                        //과일 위치에 이펙트 생성
                        var exf = Instantiate(punchEfx, food.transform.position, Quaternion.identity);
                        //점수 증가 1점씩
                        food.AddScoreText(1);
                        //쪼갠 과일 생성 (좌,우) 후 날려보냄
                        food.CreateClone(false, new Vector2(-1, 1), 5f);
                        food.CreateClone(true, new Vector2(1, 1), 5f);

                        //점수 증가 (과일의 종류)
                        switch (food.kind)
                        {
                            case FoodKind.Apple:
                                AppleCnt += 1;
                                break;
                            case FoodKind.Kiwi:
                                KiwiCnt += 1;
                                break;
                            case FoodKind.Lemon:
                                OrangeCnt += 1;
                                break;
                            case FoodKind.Watermelon:
                                WaterMelonCnt += 1;
                                break;
                        }
                        //과일 제거
                        food.ReturnObject();
                    }
                }
            }

        }


        private void Awake()
        {
            instance = this;
            stateClass = new EnumClass();

            //과일 풀링 세팅 
            foodPulling = new ObjectPool<Food>(
                () => 
                {
                    Food foodOBJ = Instantiate(food);
                    foodOBJ.Ipool = foodPulling;
                    return foodOBJ;
                },
                (food) => { food.gameObject.SetActive(true); },
                (food) => { food.gameObject.SetActive(false); },
                (food) => { Destroy(food.gameObject); },maxSize: 10
                );

            //텍스트 풀링 생성
            logManager = new LogManager<Log>(canvas.transform, log, 20);
            //이미지 로드 생성
            loadSprite = new LoadSprite("FoodSlicer");
            //상태변환 Intro 상태
            //StateChange(GameState.GameIntro);
        }
    }
}
