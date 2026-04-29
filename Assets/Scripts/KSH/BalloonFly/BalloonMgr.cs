using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace Bax.P0.Client.UnityWorld.BalloonGame
{
    
    public class BalloonMgr : MonoBehaviour 
    {
        public static BalloonMgr instance;

        public Camera Cam;

        //상태 저장용
        public EnumClass stateClass;
        //UI_1
        public GameUI gameUI;
        //UI_2
        public ScreenProsess screenProsess;

        //화살대
        public CrossBow bow;

        //화살 장전확인용 
        [System.NonSerialized]public Arrow loadArrow;

        //저장할 회전값
        public float saveBowAngle;

        //화살 갯수 확인용 Text
        public TextMeshProUGUI ArrowCountText;
        //화살 최대갯수
        public int ArrowMaxCount;

        //현재 화살갯수
        [SerializeField] private int ArrowCurCnt;
        //현재 화살 갯수 프로퍼티
        public int ArrowCurCount
        {
            get => ArrowCurCnt;
            set
            {
                ArrowCurCnt = value;
                //남은 화살갯수 확인용 Text
                ArrowCountText.text = $"{ArrowCurCnt} / {ArrowMaxCount}";

                //화살이 남아있는지 없는지 확인용 이미지 교체
                if (ArrowCurCnt <= 0) ArrowLeft.sprite = spriteData.ArrowDonLeft;
                else                ArrowLeft.sprite = spriteData.ArrowLeft;
            }
        }

        [System.NonSerialized]  public Queue<int> rndQueue = new Queue<int>();

        //efx 풀링
        public ObjectPool<BalloonEfx> efxPool;
        //balloon 풀링
        public ObjectPool<Balloon> balloonPool;
        //efx 프리팹
        public BalloonEfx efxPrefabs;
        //balloon 프리팹
        public Balloon balloonPrefab;
        //풍선 세팅용 
        public BalloonSetter balloonSetter;
        //남은 풍선 확인용 스프라이트랜더러
        public SpriteRenderer ArrowLeft;
        
        //스크랩터블오브젝트용
        public spriteData spriteData;

        //이미지 로드용
        public LoadSprite loadSprite;

        //Unitask 정지용 Token
        public CancellationTokenSource source = new();


        //구름 위치번호 1 ~ 6 넣고 섞기
        public void CloudqueueSetting()
        {
            //1~6 저장
            for (int i = 1; i < 7; i++) rndQueue.Enqueue(i);

            //리스트로 변환
            var queueList = rndQueue.ToList();

            //랜덤으로 섞기
            for (int i = 0; i < 10; i++)
            {
                int rest = UnityEngine.Random.Range(0, queueList.Count);
                int dest = UnityEngine.Random.Range(0, queueList.Count);

                int temp = queueList[rest];
                queueList[rest] = queueList[dest];
                queueList[dest] = temp;
            }
        }


        ZoZoBasePatton<BalloonMgr> zozo = new ZoZoBasePatton<BalloonMgr>();

        public void OnEnable()
        {
            if(source != null) source.Dispose();
            source = new();

            //이펙트 풀링 세팅
            #region effectPool
            efxPool = new ObjectPool<BalloonEfx>
            (
                () =>
                {
                    var efx = Instantiate(efxPrefabs);
                    efx.Ipool = efxPool;
                    return efx;
                },
                (efx) =>
                {
                    efx.gameObject.SetActive(true);
                },
                (efx) =>
                {
                    efx.gameObject.SetActive(false);
                },
                (efx) =>
                {
                    Destroy(efx.gameObject);
                }, maxSize: 10
            );
            #endregion
            //풍선 풀링 세팅
            #region Balloonpool
            balloonPool = new ObjectPool<Balloon>
            (
                () =>
                {
                    var balloon = Instantiate(balloonPrefab);
                    balloon.Ipool = balloonPool;
                    return balloon;
                },
                (balloon) =>
                {
                    balloon.gameObject.SetActive(true);
                },
                (balloon) =>
                {
                    balloon.gameObject.SetActive(false);
                },
                (balloon) =>
                {
                    Destroy(balloon.gameObject);
                }, maxSize: 30
            );
            #endregion

            instance = this;
            stateClass = new EnumClass();
            //시작할때 화살갯수 MAX 로 설정
            ArrowCurCount = ArrowMaxCount;

            loadSprite = new LoadSprite("BalloonGame");
            CloudqueueSetting();


            #region 공용 스테이트 패턴 

            ActionProcess.Enter_StateListener(null, null, () => 
            {
                bow.ArrowSet();
                balloonSetter.BalloonPositionSet().Forget(); 
            }, null);

            zozo = new ZoZoBasePatton<BalloonMgr>();
            zozo.Init(stateClass , screenProsess ,new ReadyProcess(screenProsess) , new ResultProcess(screenProsess));

            #endregion
        }

        public void OnDisable()
        {
           // loadSprite.MemoryRerease();
            source.Cancel();
            ReadyProcess.sourceCancle?.Invoke();
        }

        public void OnDestroy()
        {
            source.Cancel();
            source.Dispose();
            ReadyProcess.sourceCancle?.Invoke();
            ReadyProcess.sourceDispone?.Invoke();
        }

        private void Update()
        {
            if (zozo != null) zozo.MGR.Excute(null);    //상태가 있다면 상태의 Excute 주기적으로 실행 시킴 (Update)
        }

        /// <summary>
        /// 게임종료 조건
        /// </summary>
        public void GameOver()
        {
            //남은 풍선이 없다면
            if (balloonSetter.BalloonCnt <= 0)
            {
                //if (balloonMgr.ArrowCurCount >= 0 || balloonMgr.ArrowCurCount <= 0)
                {
                    //게임의 상태를 Success
                    stateClass.resultState = GameResult.Success;
                    //Result 상태도 이동
                    zozo.Change(GameState.GameResult);
                }
            }
            //남은 풍선이 있는데
            else if (balloonSetter.BalloonCnt > 0)
            {
                // 남은 화살이 없다면
                if (ArrowCurCount <= 0)
                {
                    //게임의 상태를 Fail
                    stateClass.resultState = GameResult.Fail;
                    //Result 상태로 이동
                    zozo.Change(GameState.GameResult);
                }
            }
        }

      
    }
}
