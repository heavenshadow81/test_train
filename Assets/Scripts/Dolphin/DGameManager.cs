using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Settings;

namespace Dolphin
{
    public class DGameManager : MonoBehaviour
    {
        public GameObject GameOver; //게임오버일 때 나올 오브젝트
        public GameObject Finish; //게임 클리어 시 나올 오브젝트
        public GameObject start; //스타트 화면 오브젝트
        public GameObject Enemyspawner; //장애물 스포너 오브젝트

        public static bool gameStart; //게임 스타트 값

        public TextMeshProUGUI timer; //타이머 텍스트
        float time = 60; //현재 타임

        public GameObject[] fish; //피쉬라이프 UI 오브젝트 
        public static int fishLife = -1; //피쉬라이프 -1로 시작


        public EnumClass stateClass;
        public GameUI gameUI;
        public ScreenProsess screenProsess;
        public ZoZoBasePatton<PianoManager> zozo;


        private CircleSpawner circleSpawner;


        private void Awake()
        {
            circleSpawner = FindObjectOfType<CircleSpawner>();

            stateClass = new EnumClass();

            #region 공용 스테이트 패턴 

            ActionProcess.Enter_StateListener(Init, null, 
                () => 
                {
                    circleSpawner.CircleInit();
                    gameStart = true;
                }, null);

            zozo = new ZoZoBasePatton<PianoManager>();
            zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
            #endregion
        }

        private void Init()
        {
           // GameOver.SetActive(false); //게임 시작할 때 게임오버 오브젝트 비활성화
           // Enemyspawner.SetActive(false); //게임 시작할 때 장애물 스포너 비활성화
           // Finish.SetActive(false); //게임 시작할 때 게임클리어 오브젝트 비활성화
           // start.SetActive(true); //시작할 때 스타트화면 활성화

            fishLife = -1; //게임 시작할 때 피쉬라이프 -1
            gameStart = false; //게임 스타트 값 false

            for (int i = 0; i < fish.Length; i++)
            {
                fish[i].SetActive(false); //피쉬라이프 오브젝트 모두 비활성화
            }
        }


       
        // Update is called once per frame
        void Update()
        {
            if (zozo != null) zozo.MGR.Excute(() => 
            {
                if (gameStart) //게임스타트가 true라면
                {
                    timer.text = $"{Math.Round(time)}"; //타이머 텍스트에 현재 타임 소수점 제외하고 표시
                    time -= Time.deltaTime; //현재 타임 점점 감소

                    if (fishLife >= 5) // 피쉬 라이프가 5 이상이라면
                    {
                        gameStart = false; //게임스타트 값 false
                        stateClass.resultState = GameResult.Success;
                        zozo.Change(GameState.GameResult);
                        //Finish.SetActive(true); //게임 클리어 오브젝트 활성화
                        //Enemyspawner.SetActive(false); //장애물 스포너 비활성화
                    }
                    else if (time <= 0 && fishLife < 5) //현재 타임이 0초 이하고 피쉬 라이프가 5 미만이라면
                    {
                        gameStart = false; //게임스타트 값 false
                        stateClass.resultState = GameResult.Fail;
                        zozo.Change(GameState.GameResult);
                        //GameOver.SetActive(true); //게임오버 오브젝트 활성화
                        //Enemyspawner.SetActive(false); //장애물 스포너 비활성화
                        //Time.timeScale = 0; //시간 멈춤
                    }
                }

                for (int i = 0; i < fish.Length; i++)
                {
                    if (fishLife == i) //피쉬 라이프의 값이 i와 같다면
                    {
                        fish[i].SetActive(true); //피쉬 라이프 UI를 값에 맞게 활성화
                    }
                    else if (fishLife < i) //피쉬 라이프 값이 i보다 작을 경우
                    {
                        fish[i].SetActive(false); //피쉬 라이프 UI를 값에 맞게 비활성화
                    }
                }

                if (fishLife < -1) //피쉬라이프 값이 -1보다 작다면
                    fishLife = -1; //피쉬라이프 값을 -1로 변경

                if (fishLife > 5) //피쉬라이프 값이 5보다 크다면
                    fishLife = 5; //피쉬라이프 값을 5로 변경
            });

           
        }
        public void StartBtn() //스타트 버튼 함수
        {
            Enemyspawner.SetActive(true); //장애물 스포너 활성화
            start.SetActive(false); //시작할 때 스타트화면 비활성화
            gameStart = true; //게임스타트 true
            Time.timeScale = 1; //시간이 다시 흐르게 변경
        }
        public void HomeBtn() //홈버튼 함수
        {
            SceneManager.LoadSceneAsync(0); //0번 씬으로 이동
        }
    }
}
