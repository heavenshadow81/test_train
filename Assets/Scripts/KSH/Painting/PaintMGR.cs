using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PaintMGR : MonoBehaviour
{
    //싱글톤용
    public static PaintMGR Instance;
    //저장용 컬러
    public Color PaintColor = Color.black;
    //스탬프 프리팹
    public Stamp StampPrefab;
    //스탬프 리스트
    public List<Stamp> stampList = new List<Stamp>();
    //로드용
    public LoadSprite loadSprite;
    //솔팅
    public int sorting;
    //파티클 프리팹
    public GameObject[] particlePrefabs;
    //스탬프 오브젝트풀 용
    public ObjectPool<Stamp> stampPool;

    public EnumClass stateClass;
    public ScreenProsess screenProsess;
    public GameUI gameUI;

    public ZoZoBasePatton<PaintMGR> zozo;
    private void Awake()
    {
        //간단 풀링
        Instance = this;
        //로드 이미지 폴더 세팅
        loadSprite = new LoadSprite("Painting/foot");
        //솔팅 초기값 세팅
        sorting = -100;
        //스탬프 오브젝트 풀 생성
        stampPool = new ObjectPool<Stamp>(
            () => 
            {
                var stamp = Instantiate(StampPrefab);
                stamp.Ipool = stampPool;
                return stamp;
            },
            (stamp) => { stamp.gameObject.SetActive(true); },
            (stamp) => { stamp.gameObject.SetActive(false); },
            (stamp) => { Destroy(stamp.gameObject); },maxSize: 30
            );

        stateClass = new EnumClass();

        #region 공용 스테이트 패턴 

        ActionProcess.Enter_StateListener(null, null, null, null);

        zozo = new ZoZoBasePatton<PaintMGR>();
        zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
        #endregion
    }
}
