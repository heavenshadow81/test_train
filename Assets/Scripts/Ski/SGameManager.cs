using ShipRun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Settings;

public class SGameManager : MonoBehaviour
{
    public GameObject GameOver; //게임오버 오브젝트
    public GameObject GameStart; //게임스타트 오브젝트
    public GameObject GameClear; //게임클리어 오브젝트

    public GameObject Enemyspawner; //에너미스포너 오브젝트

    public static int Score; //스코어 변수
    public TextMeshProUGUI scoreText; //스코어 텍스트
    public TextMeshProUGUI timeText; //타임 텍스트
    float timer = 60; //타임 변수

    public static bool stageOne; //스테이지1 확인값
    public static bool stageTwo; //스테이지2 확인값
    public static bool stageThree; //스테이지3 확인값

    public Animator BG; //배경 애니메이션

    public ZoZoBasePatton<SGameManager> zozo;
    public EnumClass stateClass;
    public GameUI gameUI;
    public ScreenProsess screenProsess;

    private void Awake()
    {
        stateClass = new EnumClass();
        #region 공용 스테이트 패턴 

        ActionProcess.Enter_StateListener(Init, null, StartBtn, null);

        zozo = new ZoZoBasePatton<SGameManager>();
        zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
        #endregion
    }
    void Init() //시작할 때
    {

//        GameOver.SetActive(false); //게임오버 오브젝트 비활성화
  //      GameStart.SetActive(true); //게임스타트 오브젝트 활성화
    //    GameClear.SetActive(false); //게임클리어 오브젝트 비활성화

        Enemyspawner.SetActive(false); //에너미스포너 비활성화
        SkiMove.gameStart = false; //게임스타트 비활성화
        SnowBallMove.GameOver = false; //게임오버 비활성화

        Score = 0; //스코어 0
        scoreText.text = ""; //텍스트 비활성화

        stageOne = false; //스테이지1 비활성화
        stageTwo = false; //스테이지2 비활성화
        stageThree = false; //스테이지3 비활성화
    }
    // Update is called once per frame

    void updateLogic()
    {
        if (SkiMove.gameStart && !SnowBallMove.GameOver) //게임스타트가 활성화고 게임오버가 비활성화 라면
        {

            BG.SetTrigger("Start"); //배경 애니메이션을 스타트로

            scoreText.text = $"{Score}점"; //스코어 텍스트에 스코어 변수 표시
            timer -= Time.deltaTime; //타임이 점점 줄어들게
            timeText.text = $"{Math.Round(timer)}"; //타임텍스트에 타임 소수점 없이 표시

            if (timer > 40) //타이머가 40보다 크다면
            {
                stageOne = true; //스테이지 1 활성화
                stageTwo = false; //스테이지 2 비활성화
                stageThree = false; //스테이지 3 비활성화
            }
            else if (timer <= 40 && timer > 20) //타이머가 40이하고 20보다 크다면
            {
                stageOne = false; //스테이지 1 비활성화
                stageTwo = true; //스테이지 2 활성화
                stageThree = false; //스테이지 3 비활성화
            }
            else if (timer <= 20 && timer > 0) //타이머가 20이하고 0보다 크다면
            {
                stageOne = false; //스테이지 1 비활성화
                stageTwo = false; //스테이지 2 비활성화
                stageThree = true; //스테이지 3 활성화
            }
            else if (timer <= 0) //타이머가 0보다 작다면
            {
                stageOne = false; //스테이지 1 비활성화
                stageTwo = false; //스테이지 2 비활성화
                stageThree = false; //스테이지 3 비활성화

                timeText.text = ""; //타임텍스트 비활성화
                //GameClear.SetActive(true); //게임클리어 오브젝트 활성화
                stateClass.resultState = GameResult.Success;
                zozo.Change(GameState.GameResult);


                Enemyspawner.SetActive(false); //에너미 스포너 비활성화
                SkiMove.gameStart = false; //게임스타트 비활성화
            }

        }
        else if (SnowBallMove.GameOver) //게임오버가 활성화라면
        {
            BG.speed = 0; //배경 애니메이션 속도 0

            stageOne = false; //스테이지 1 비활성화
            stageTwo = false; //스테이지 2 비활성화
            stageThree = false; //스테이지 3 비활성화

            timeText.text = ""; //타임텍스트 비활성화
            //GameOver.SetActive(true); //게임오버 오브젝트 활성화
            stateClass.resultState = GameResult.Fail;
            zozo.Change(GameState.GameResult);
            Enemyspawner.SetActive(false); //에너미 스포너 비활성화
            SkiMove.gameStart = false; //게임스타트 비활성화
        }
    }
    void Update()
    {
        if (zozo != null) zozo.MGR.Excute(
            () => 
            {
                updateLogic();
            });
    }
    public void StartBtn() //스타트 버튼 함수
    {
        Enemyspawner.SetActive(true); //에너미 스포너 활성화
      //  GameStart.SetActive(false); //게임스타트 오브젝트 비활성화
        SkiMove.gameStart = true; //게임스타트 활성화
    }
}
