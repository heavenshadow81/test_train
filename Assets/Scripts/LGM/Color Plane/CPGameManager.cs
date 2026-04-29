using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum EColor
{
    Red,
    Green,
    Blue,
    Orange,
    Violet,
    Yellow,
    Max
}

public class CPGameManager : Singleton<CPGameManager>
{
    public EnumClass stateClass;
    public ZoZoBasePatton<FishGameManager> zozo;
    public ScreenProsess screenProsess;

    public int countting;   // 밟은 장판 갯수
    public Image informPlane;   // 해답 장판
    public Transform parentPlane;   // 색깔 장판의 부모 객체
    public List<Image> plane;   // 색 장판
    public Dictionary<EColor, Sprite> dicColor; // 색상 이미지 딕셔너리
    
    public TextMeshProUGUI scoreText;   // 점수 텍스트
    public float score; // 점수
    public float onePoint = 100;    // 정답 1개당 획득 점수
    public float minusTime = 5;     // 오답 시 시간 감소 값
    public GameObject gameOverUI;   // 게임오버 UI
    public bool gameOver;   // 게임 종료 체크
    public Slider health;   // 체력바
    public float recovery = 0.1f;   // 정답 1개당 타이머 회복 시간
    public float speed = 1; // 제한시간 감소 속도


    private void Awake()
    {
        if (plane == null)
            plane = new List<Image>();
        if (dicColor == null)
            dicColor = new Dictionary<EColor, Sprite>();

        stateClass = new EnumClass();

        ActionProcess.Enter_StateListener(() =>
        {
            // Dic타입으로 발판 종류 별로 보관
            dicColor.Add(EColor.Red,
                Resources.Load<Sprite>("Color Plane/Sprite/Red Block"));
            dicColor.Add(EColor.Green,
                Resources.Load<Sprite>("Color Plane/Sprite/Green Block"));
            dicColor.Add(EColor.Blue,
                Resources.Load<Sprite>("Color Plane/Sprite/Blue Block"));
            dicColor.Add(EColor.Orange,
                Resources.Load<Sprite>("Color Plane/Sprite/Orange Block"));
            dicColor.Add(EColor.Violet,
                Resources.Load<Sprite>("Color Plane/Sprite/Violet Block"));
            dicColor.Add(EColor.Yellow,
                Resources.Load<Sprite>("Color Plane/Sprite/Yellow Block"));

        }, Wait, ReSetGame, null);

        zozo = new ZoZoBasePatton<FishGameManager>();
        zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
    }

    private void Wait()
    {
        for (int i = 0; i < parentPlane.childCount; i++)
        {
            // 발판의 상위객체를 통해 모든 발판의 Image 종류를 받아온다.
            plane.Add(parentPlane.GetChild(i).GetComponent<Image>());
        }

        //ReSetGame();    // 발판 세팅
    }
    private void Update()
    {
        if (zozo != null) zozo.MGR.Excute(
        () => 
        {
            if (health.value <= 0)  // 체력(시간)이 0이하면 게임 오버 메가
            {
                //gameOverUI.SetActive(true);
                stateClass.resultState = GameResult.Success;
                zozo.Change(GameState.GameResult);
                gameOver = true;
            }
            health.value -= Time.deltaTime * speed; // 스피드만큼 체력 감소
        });
    }

    // 발판 색상 섞이
    public void Swap_Random_ColorPlane(int _count = 1)
    {
        // 현재 인덱스의 sprite와 랜덤한 장판의 sprite 교환
        for (int i = 0; i < _count; i++) 
        {
            int randomIndex = UnityEngine.Random.Range(0, plane.Count); // 랜덤 발판 선택

            // 발판을 서로 썪어서 랜덤으로 배치된것처럼 발판끼리 스왑
            Sprite temp = plane[i % (plane.Count - 1)].sprite;
            plane[i % (plane.Count - 1)].sprite = plane[randomIndex].sprite;
            plane[randomIndex].sprite = temp;
        }
    }

    // 모든 발판 비활성화/활성화
    public void All_Active_Change(bool _active)
    {
        for(int i = 0; i < plane.Count; i++)
        {
            plane[i].gameObject.SetActive(_active);
        }
    }

    // 메인 발판 랜덤 색 설정
    public void Set_Random_InformPlane()
    {
        while (true)
        {
            int color = UnityEngine.Random.Range(0, (int)EColor.Max);

            // 예외 처리) 기존 발판과 같은 색이 나오지 않도록 체크
            if (plane[color].IsActive() &&
                informPlane.sprite != plane[color].sprite) 
            {
                informPlane.sprite = plane[color].sprite;
                return;
            }
        }
    }

    // 발판 초기화
    public void ReSetGame()
    {
        countting = 0;
        All_Active_Change(true);    // 모든 발판 활성화
        Set_Random_InformPlane();   // 찾아야할 발판 랜덤 설정
        Swap_Random_ColorPlane(plane.Count);    // 발판 랜덤 배치(스왑 방식)
    }

    // 오브젝트 비활성화 시 실행할 함수
    public void DisableEvent(GameObject obj)
    {
        countting++;    // 맞춘 갯수 증가
        Set_Random_InformPlane(); // 다음 찾아야할 발판 랜덤 설정
        obj.SetActive(false);   // 밟은 발판은 비활성화
    }
}
