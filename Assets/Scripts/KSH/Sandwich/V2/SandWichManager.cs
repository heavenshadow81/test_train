using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;


//샌드위치 부속품 타입
public enum SandKind
{
    None, Bread, Cheese, Fri, Ham, Lettuce, Tomato , MAX
}

public class SandWichManager : MonoBehaviour
{
    public static SandWichManager instance;
    //왼쪽(보기) 스포너
    public L_Spawner Lspawner;
    //오른쪽(보기) 스포너
    public R_Spawner Rspawner;

    /// <summary>
    /// 샌드위치는 7층까지 
    /// </summary>
    public int sandCnt;

    public bool isDown = false;
    //이미지 로드
    public LoadSprite loadSprite;
    //샌드위치 부속 프리팹
    public Sand sandPrefabs;
    //샌드위치 부속 오브젝트 풀
    public ObjectPool<Sand> sandPool;
    //애니메이션 커브
    public AnimationCurve curve;
    //트윈 정지용 토큰
    public CancellationTokenSource _source = new();

    //게임중이냐
    public bool IsGame = false;
    //결과 저장용 리스트
    public List<bool> resultList = new List<bool>();

    public float gameSpeed = 0.1f;

    private float money;
    public float Money
    {
        get => money;
        set
        {
            money = value;
            MoneySlider.value = money;
            //머니가 10000이 넘을경우
            if (money >= 10000)
            {
                Debug.Log("게임종료");
                IsGame = false;
                //결과를 성공으로 변경
                stateClass.resultState = GameResult.Success;
                //Result 상태로 전환
                zozo.Change(GameState.GameResult);
            }
        }
    }
    //시간슬라이더
    public Slider TimeSlider;
    //머니슬라이더
    public Slider MoneySlider;
    //정답 파티클
    public ParticleSystem ClearEfx;
    //실패 파티클
    public ParticleSystem UnClearEfx;
    //사운드
    public SandSound soundMgr;
    
    public GameUI gameUI;
    public ScreenProsess screenProsess;
    public EnumClass stateClass;
    public ZoZoBasePatton<SandWichManager> zozo;
    //타임매니저
    public TimerMgr timerMgr;
    /// <summary>
    /// 비우기
    /// </summary>
    public async void Trash()
    {
        //샌드위치를 만드는 중이고, 게임플레이중일때
        if (Rspawner.LaiseSandList.Count > 0 && stateClass.state == GameState.GamePlay )
        {
            //사운드 
            soundMgr.Trash();
            if (IsGame)
            {
                IsGame = false;
                //우측 샌드 전부 비움
                await Rspawner.SandDelete();
                IsGame = true;
            }
        }
    }

    private void OnEnable()
    {
        if (_source != null) _source.Dispose();
        _source = new();
    }

    private void OnDisable()
    {
        _source.Cancel();
    }

    private void OnDestroy()
    {
        _source.Cancel();
        _source.Dispose();
    }

    public void Awake()
    {
        instance = this;
        //샌드위치 부속 이미지 로드용
        loadSprite = new LoadSprite("Sandwich");
        stateClass = new EnumClass();
        //샌드위치 부속 오브젝트풀 생성
        sandPool = new ObjectPool<Sand>(
            () => 
            {
                var sw = Instantiate(sandPrefabs);
                sw.Ipool = sandPool;
                return sw;
            },
            (sw) => { sw.gameObject.SetActive(true); },
            (sw) => { sw.gameObject.SetActive(false); },
            (sw) => { Destroy(sw.gameObject); }, 
            maxSize: 1
            );

        #region 공용 스테이트 패턴 

        ActionProcess.Enter_StateListener(null, null,
            () =>
            {
                ActionProcess.SandAllRaise?.Invoke();

                timerMgr = new TimerMgr(60f, gameUI.timerText,
                    () => { zozo.Change(GameState.GameResult); }, stateClass, true);

            }, null);

        zozo = new ZoZoBasePatton<SandWichManager>();
        zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));

        #endregion

    }



    public void Update()
    {
        //타임매니저안에 진행값을 타임슬라이더로 저장
        if (zozo != null) zozo.MGR.Excute(()=> { TimeSlider.value = timerMgr.late; });
    }
}