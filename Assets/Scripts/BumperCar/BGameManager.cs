using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using static Settings;
using LGM.CraneGame;

namespace BumperCar
{
    public class BGameManager : MonoBehaviour //범퍼카 게임 매니저
    {
        public GameObject GameOver; //게임 오버되면 나올 오브젝트
        public GameObject Finish; //게임 클리어 시 나올 오브젝트
        public GameObject start; //게임 시작 시 나올 오브젝트

        public GameObject Enemyspawner; //자동차 스포너
        public GameObject EnemyspawnerTwo; //자동차 스포너2

        public GameObject miniCar; //미니맵 자동차
        public GameObject effect; //플레이어 자동차 이펙트

        public TextMeshProUGUI LapText; //자동차 바퀴수 텍스트

        public TextMeshProUGUI OneLapText; //한 바퀴 표시 텍스트
        public TextMeshProUGUI TwoLapText; //두 바퀴 표시 텍스트
        public TextMeshProUGUI LastLapText; //마지막 바퀴 표시 텍스트

        public Animator BG; //배경 애니메이터
        public Animator Ready; //출발선 애니메이터

        float timer; //총 시간초
        int Lap; //바퀴수

        public static bool one; //한 바퀴 스테이지
        public static bool two; //두 바퀴 스테이지
        public static bool last; //마지막 바퀴 스테이지

        // Start is called before the first frame update

        public ZoZoBasePatton<BGameManager> zozo;
        public EnumClass stateClass;
        public GameUI gameUI;
        public ScreenProsess screenProsess;

        MinimapMove minimap;
        private void Awake()
        {
            minimap = FindObjectOfType<MinimapMove>();
            


            stateClass = new EnumClass();
            #region 공용 스테이트 패턴 

            ActionProcess.Enter_StateListener(Init, null, 
                ()=> 
                {
                    StartBtn();
                    
                }, null);

            zozo = new ZoZoBasePatton<BGameManager>();
            zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
            #endregion
        }

        void Init()
        {
            timer = 34; //처음 초를 34초로

           // GameOver.SetActive(false); //게임 시작하면 게임오버 오브젝트 끔
           // Finish.SetActive(false); //게임 시작하면 게임클리어 오브젝트 끔
           // start.SetActive(true); //게임 시작하면 나올 오브젝트 활성화

            Enemyspawner.SetActive(false); //게임 시작하면 자동차 스포너 끔
            EnemyspawnerTwo.SetActive(false); //게임 시작하면 자동차 스포너 끔
            miniCar.SetActive(false); //게임 시작하면 미니맵 자동차 끔

            effect.SetActive(false); //게임 시작하면 플레이어 자동차 이펙트 끔

            PlayerMove.gameStart = false; //게임 시작했을 때 게임스타트 값을 false로
            CarMove.GameOver = false; //게임 시작했을 때 게임오버 값을 false로

            LapText.text = ""; //게임 시작했을 때 바퀴 수 나오지 않음
            OneLapText.text = ""; //게임 시작했을 때 한 바퀴 나오지 않음
            TwoLapText.text = ""; //게임 시작했을 때 두 바퀴 나오지 않음
            LastLapText.text = ""; //게임 시작했을 때 마지막 바퀴 나오지 않음

        }

        private void logic()
        {
            if (PlayerMove.gameStart) //만약 게임스타트 값이 true라면
            {
                timer -= Time.deltaTime; //타이머를 점점 감소
                effect.SetActive(true); //플레이어 자동차 이펙트 킴
                LapText.text = $"{Lap}/3"; //현재 바퀴수 텍스트 표시

                BG.SetTrigger("1Lap"); //배경화면 애니메이션 트리거 1Lap을 체크
                Ready.SetTrigger("Ready"); //출발선 애니메이션 트리거 Ready를 체크

                if (timer <= 34 && timer > 25.5f) //만약 타이머가 34초에서 25.5초 사이라면
                {
                    one = false; //한 바퀴 값을 false로 
                    two = false; //두 바퀴 값을 false로
                    last = false; //세 바퀴 값을 false로

                    Lap = 0; //현재 바퀴 수는 0으로 변경
                }
                else if (timer <= 25.5 && timer > 17) //만약 타이머가 25.5초에서 17초 사이라면
                {
                    if (timer <= 25.5 && timer > 25.49f)
                        GameObject.Find("SoundManager").GetComponent<CarSound>().LapSound(); //24초에서 23.99초 사이에 LapSound 재생

                    BG.SetTrigger("2Lap"); //배경화면 애니메이션 트리거 2Lap을 체크

                    one = true; //한 바퀴 값을 true로
                    two = false; //두 바퀴 값을 false로
                    last = false; //마지막 바퀴 값을 false로 

                    Lap = 1; //현재 바퀴 수는 1로 변경
                    OneLapText.text = "LAP 1"; //한 바퀴 텍스트 표시
                    OneLapText.DOFade(0, 4); //한 바퀴 텍스트 표시 후 4초동안 페이드 아웃
                }
                else if (timer <= 17 && timer > 8.5f) //만약 타이머가 17초에서 9.5초 사이라면
                {
                    if (timer <= 17 && timer > 16.99f)
                        GameObject.Find("SoundManager").GetComponent<CarSound>().LapSound(); //16초에서 15.99초 사이에 LapSound 재생

                    BG.SetTrigger("3Lap"); //배경화면 애니메이션 트리거 3Lap을 체크

                    one = false; //한 바퀴 값을 false로
                    two = true; //두 바퀴 값을 true로
                    last = false; //마지막 바퀴 값을 false로

                    EnemyspawnerTwo.SetActive(true); //자동차 스포너2를 킴
                    Lap = 2; //현재 바퀴 수 2로 변경

                    TwoLapText.text = "LAP 2"; //두 바퀴 텍스트 표시
                    TwoLapText.DOFade(0, 4); //두 바퀴 텍스트 표시 후 4초동안 페이드 아웃
                }
                else if (timer <= 8.5f && timer > 0) //만약 타이머가 8초에서 0초 사이라면
                {
                    if (timer <= 8.5f && timer > 8.49f)
                        GameObject.Find("SoundManager").GetComponent<CarSound>().LapSound(); //8초에서 7.99초 사이에 LapSound 재생

                    one = false; //한 바퀴 값을 false로
                    two = false; //두 바퀴 값을 false로
                    last = true; //마지막 바퀴 값을 true로

                    Lap = 3; //현재 바퀴 수를 3으로 변경
                    LastLapText.text = "FINAL LAP"; //마지막 바퀴 텍스트 표시
                    LastLapText.DOFade(0, 4); //마지막 바퀴 텍스트 표시 후 4초동안 페이드 아웃
                }
                else if (timer <= 0) //타이머가 0초 이하라면
                {
                    BG.speed = 0; //배경화면 애니메이션 스피드를 0으로
                    Ready.speed = 0; //출발선 애니메이션 스피드를 0으로

                    PlayerMove.gameStart = false; //게임스타트 값을 false로

                    effect.SetActive(false); //플레이어 자동차 이펙트 끔
                    Enemyspawner.SetActive(false); //자동차 스포너 끔
                    EnemyspawnerTwo.SetActive(false); //자동차 스포너2 끔
                    miniCar.SetActive(false); //미니맵 자동차 끔

                    //Finish.SetActive(true); //게임 클리어 오브젝트 활성화
                    LapText.text = ""; //현재 바퀴 수 텍스트 끔

                    stateClass.resultState = GameResult.Success;
                    zozo.Change(GameState.GameResult);

                }
            }

            if (CarMove.GameOver) //게임 오버가 true라면
            {
                BG.speed = 0; //배경 애니메이션 스피드 0
                Ready.speed = 0; //출발선 애니메이션 스피드 0

                // GameOver.SetActive(true); //게임 오버 오브젝트 활성화
                stateClass.resultState = GameResult.Fail;
                zozo.Change(GameState.GameResult);

                Enemyspawner.SetActive(false); //자동차 스포너 끔
                EnemyspawnerTwo.SetActive(false); //자동차 스포너2 끔
                miniCar.SetActive(false); //미니맵 자동차 끔
                LapText.text = ""; //현재 바퀴 수 텍스트 끔
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (zozo != null) zozo.MGR.Excute(() =>
            {
                logic();
                minimap.UpdateLogic();
            });


           
        }

        public void StartBtn() //시작 버튼을 눌렀을 때
        {
            miniCar.SetActive(true); //미니맵 자동차 활성화
            Enemyspawner.SetActive(true); //자동차 스포너 활성화
            //start.SetActive(false); //시작화면 비활성화
            PlayerMove.gameStart = true; //게임스타트 값을 true로 변경
        }

        public void HomeBtn() //홈버튼을 눌렀을 때
        {
            SceneManager.LoadSceneAsync(0); //0번 씬으로 이동
        }
    }
}
