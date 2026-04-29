using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Bax.P0.Client.UnityWorld.PictureGame;




public class FerrisMgr : MonoBehaviour
{
    public static FerrisMgr Instance;

    public CancellationTokenSource _sources = new();

    //하나씩 세팅 인간컬러
    public HumanColor FindColor;
   
    //관람차 List
    public List<Ferris> ferrisList = new List<Ferris>();

    [Header("관람차 프레임")]
    public Transform ferrisFrame;
    [Header("사람 이미지들 나올 UI 프레임")]
    public Questionframe SelectFrame;
   
    [Header("페이드아웃용 Panel")]
    public Image fadeOut80;

    public GameUI gameUI;
    public ScreenProsess screenProsess;
    public EnumClass stateClass;

    //정답원형이미지 풀링
    public PullingMGR<Highlight> hightlightPulling;
    //X 자 풀링
    public PullingMGR<Highlight> wrongPulling;
    //동그라미 프리팹
    public Highlight Highlightprefab;
    //X 프리팹
    public Highlight wrongprefab;
    //생성한 프리팹들의 부모
    public Transform hightlightParent;

    
    //이미지로드 할때 사용할 Color Enum
    public HumanColor[] ColorNames = new HumanColor[6];

    //클리어조건이 들어갈 bool list
    public List<bool> GameClear = new List<bool>();

    //스트리밍에셋폴더 안에서 스프라이트 로드
    public LoadSprite loadSprite;

    public ZoZoBasePatton<FerrisMgr> zozo = new ZoZoBasePatton<FerrisMgr>();

    private void randomColorName()
    {
        //컬러값 차례대로 저장
        for (int i = 0; i < 6; i++)
        {
            ColorNames[i] = FindColor;
            
            FindColor++;
        }
        //섞기
        for (int i = 0; i < 10; i++)
        {
            int rest = UnityEngine.Random.Range(0, ColorNames.Length);
            int dest = UnityEngine.Random.Range(0, ColorNames.Length);

            HumanColor temp = ColorNames[rest];
            ColorNames[rest] = ColorNames[dest];
            ColorNames[dest] = temp;
        }

        //관람차에 해당 컬러 enum 저장
        for (int i = 0; i < ferrisList.Count; i++)
        {
            ferrisList[i].FindColor = ColorNames[i];
        }

    }


    private async UniTask humanEnumSetting()
    {
        randomColorName();
        await UniTask.Yield();
        
    }


    private void Awake()
    {
        Instance = this;

        stateClass = new EnumClass();
        hightlightPulling = new PullingMGR<Highlight>(Highlightprefab , hightlightParent, 6);
        wrongPulling = new PullingMGR<Highlight>(wrongprefab , 6);
        loadSprite = new LoadSprite("FerrisCar");
        //관람차에 해당 컬러Enum 세팅
        humanEnumSetting().Forget();

        #region 공용 스테이트 패턴 

        ActionProcess.Enter_StateListener(null, null, 
            () => { whenAllProcess().Forget(); }, 
            async () => { await when(); });

        zozo = new ZoZoBasePatton<FerrisMgr>();
        zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));

        #endregion
    }

    //시작시 카메라줌 및 카메라 위치 변경
    private async UniTask whenAllProcess()
    {
        await UniTask.Yield();
        SoundMGR.Instance.SoundPlay("18.카메라무브");

        //카메라를 앞으로 땡기고 카메라의 위치 이동
        await UniTask.WhenAll(
            Camera.main.transform.DOMove(new Vector3(0, -3f, -10), 2).WithCancellation(_sources.Token),
            DOTween.To(() => Camera.main.orthographicSize, x => Camera.main.orthographicSize = x, 2, 2).WithCancellation(_sources.Token)
        );
        //관람차안 사람이미지 로드 및 관람차 이동 
        GetAnimalSprite().Forget();
    }

    private async UniTask when()
    {
        //카메라 원상태로 
        //문제 프레임 원상태로 변경
        await UniTask.WhenAll(
            Camera.main.transform.DOMove(new Vector3(0, 0f, -10), 2).WithCancellation(FerrisMgr.Instance._sources.Token),
            DOTween.To(() => Camera.main.orthographicSize, x => Camera.main.orthographicSize = x, 5.4f, 2).WithCancellation(FerrisMgr.Instance._sources.Token),
            FerrisMgr.Instance.SelectFrame.transform.DOMoveY(-6.5f, 1).WithCancellation(FerrisMgr.Instance._sources.Token)
            );
    }

    int idx = 0;

    public async UniTask GetAnimalSprite()
    {
        //관람차가 다 돌았다면 
        if (idx >= 6)
        {
            //맞춰야할 사람들 세팅
            SelectFrame.HumanSelectToSpriteLoad();

            await UniTask.Delay(TimeSpan.FromSeconds(0.4f), DelayType.UnscaledDeltaTime);

            Camera.main.transform.DOMoveY(-3, 2).OnComplete(() => 
            {
                SoundMGR.Instance.SoundPlay("18.문제나옴");
                //맞춰야할 프레임 올라옴
                SelectFrame.transform.DOMoveY(-4.25f, 1);
            });
        }
        else
        {
            //이미지 로드
            await UniTask.WhenAll
            (
                //컬러값으로 머리 몸 왼팔 오른팔 이미지 로드
                loadSprite.LoadSpriteData($"{ColorNames[idx]}HEAD", ferrisList[0].Head),
                loadSprite.LoadSpriteData($"{ColorNames[idx]}BODY", ferrisList[0].Body),
                loadSprite.LoadSpriteData($"{ColorNames[idx]}LEFTARM", ferrisList[0].LeftArm),
                loadSprite.LoadSpriteData($"{ColorNames[idx]}RIGHTARM", ferrisList[0].RightArm)
            );

            //팔 흔들기
            ferrisList[0].ArmPivotLoop().Forget();

            //관람차 번호 증가
            idx++;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), DelayType.UnscaledDeltaTime);
            //문열림
            ferrisList[0].DoorOpen();
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f), DelayType.UnscaledDeltaTime);
            //문닫힘
            ferrisList[0].DoorClose(() =>
            {
                //문이 닫히면 다음 관람차로 
                nextFerris();
            });
        }
    }

    private void nextFerris()
    {
        //처음 값 저장
        var go = ferrisList.ToArray().First();
        //처음값 제거
        ferrisList.RemoveAt(0);
        //저장한 한 값 저장 맨뒤로
        ferrisList.Add(go);
        //60도 돌리기고 6번째 관람차가 나올때까지 재귀
        FerrisMove(60, () => { GetAnimalSprite().Forget(); });
    }
   
    //관람차 전부 60도씩 이동
    public void FerrisMove(float angle , Action action = null ,  float time = 1)
    {
        //관람차 프레임 회전
        ferrisFrame.DORotate(new Vector3(0, 0, ferrisFrame.rotation.eulerAngles.z - angle), time);

        //각자 관람차들 angle값 변경  
        DOTween.To(() => ferrisList[0].angle, x => ferrisList[0].angle = x, ferrisList[0].angle - angle, time);
        DOTween.To(() => ferrisList[1].angle, x => ferrisList[1].angle = x, ferrisList[1].angle - angle, time);
        DOTween.To(() => ferrisList[2].angle, x => ferrisList[2].angle = x, ferrisList[2].angle - angle, time);
        DOTween.To(() => ferrisList[3].angle, x => ferrisList[3].angle = x, ferrisList[3].angle - angle, time);
        DOTween.To(() => ferrisList[4].angle, x => ferrisList[4].angle = x, ferrisList[4].angle - angle, time);
        DOTween.To(() => ferrisList[5].angle, x => ferrisList[5].angle = x, ferrisList[5].angle - angle, time).OnComplete(() => { action?.Invoke();  });
    }


    private void OnDisable()
    {
        ReadyProcess.sourceCancle?.Invoke();
    }

    private void OnDestroy()
    {
        ReadyProcess.sourceCancle?.Invoke();
        ReadyProcess.sourceDispone?.Invoke();
    }

}
