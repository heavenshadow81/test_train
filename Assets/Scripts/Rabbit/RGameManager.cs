using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Settings;

namespace Rabbit
{
    public class RGameManager : MonoBehaviour
    {
        public GameObject GameOver; //게임오버 오브젝트 
        public GameObject Enemyspawner; //열매 스포너 오브젝트
        public GameObject start; //스타트화면 오브젝트
        public GameObject end; //게임종료 오브젝트

        public TextMeshProUGUI timeText; //타이머 텍스트
        float timer = 60; //현재 타임 변수

        public static bool stageOne; //1스테이지 값
        public static bool stageTwo; //2스테이지 값
        public static bool stageThree; //3스테이지 값

        public ZoZoBasePatton<RGameManager> zozo;
        public EnumClass stateClass;
        public GameUI gameUI;
        public ScreenProsess screenProsess;

        private RabbitSpawner rabbitSpawner;

        private void Awake()
        {
            rabbitSpawner = FindObjectOfType<RabbitSpawner>();


            stateClass = new EnumClass();
            #region 공용 스테이트 패턴 

            ActionProcess.Enter_StateListener(Init, null, 
                () => 
                {
                    rabbitSpawner.Spawner();
                    RabbitMove.gameStart = true;

                }, null);

            zozo = new ZoZoBasePatton<RGameManager>();
            zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
            #endregion
        }

        private void Init()
        {
            //GameOver.SetActive(false); //게임시작 시 게임오버 오브젝트 비활성화
            //Enemyspawner.SetActive(false); //열매 스포너 오브젝트 비활성화
            //start.SetActive(true); //시작화면 활성화
            //end.SetActive(false); //게임종료 오브젝트 비활성화

            RabbitMove.gameStart = false; //게임스타트 값을 false로
            DragonFruitMove.GameOver = false; //게임오버 값을 false로

            stageOne = false; //1스테이지 false
            stageTwo = false; //2스테이지 false
            stageThree = false; //3스테이지 false
        }

      

        // Update is called once per frame
        void Update()
        {
            if (zozo != null) zozo.MGR.Excute(() => 
            {
                if (RabbitMove.gameStart) //게임스타트가 true라면
                {
                    timer -= Time.deltaTime; //타이머가 점점 감소
                    timeText.text = $"{Math.Round(timer)}"; //타이머 텍스트에 소수점 없이 표시

                    if (timer > 40) //타이머가 40보다 크다면
                    {
                        stageOne = true; //1스테이지 true
                        stageTwo = false; //2스테이지 false
                        stageThree = false; //3스테이지 false
                    }
                    else if (timer <= 40 && timer > 20) //타이머가 40~21초 사이라면
                    {
                        stageOne = false; //1스테이지 false
                        stageTwo = true; //2스테이지 true
                        stageThree = false; //3스테이지 false
                    }
                    else if (timer <= 20 && timer > 0) //타이머가 20~1초 사이라면
                    {
                        stageOne = false; //1스테이지 false
                        stageTwo = false; //2스테이지 false
                        stageThree = true; //3스테이지 true
                    }
                    else if (timer <= 0) //타이머가 0 이하라면
                    {
                        timeText.text = ""; //타이머 텍스트 표시 안함
                        Enemyspawner.SetActive(false); //열매 스포너 비활성화
                        RabbitMove.gameStart = false; //게임스타트 false

                        stateClass.resultState = GameResult.Success;
                        zozo.Change(GameState.GameResult);
                        //end.SetActive(true); //게임종료 오브젝트 활성화
                    }
                }
                else if (DragonFruitMove.GameOver) //게임오버 값이 true라면
                {
                    timeText.text = ""; //타이머 텍스트 표시안함
                    //GameOver.SetActive(true); //게임오버 오브젝트 활성화
                    Enemyspawner.SetActive(false); //열매 스포너 비활성화
                    RabbitMove.gameStart = false; //게임스타트 false

                    stateClass.resultState = GameResult.Fail;
                    zozo.Change(GameState.GameResult);
                }
            });


           
        }
        public void StartBtn() //스타트 버튼 함수
        {
            Enemyspawner.SetActive(true); //열매 스포너 활성화
            start.SetActive(false); //시작화면 비활성화
            RabbitMove.gameStart = true; //게임스타트 true
        }
        public void HomeBtn() //홈버튼 함수
        {
            SceneManager.LoadSceneAsync(0); //0번씬으로 이동
        }
    }
}
