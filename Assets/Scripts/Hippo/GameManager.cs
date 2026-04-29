using Bax.P0.Client.UnityWorld.MonkeyGame;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Bax.P0.Client.UnityWorld.MonkeyGame.MonkeyMgr;
using static Settings;

namespace Hippo
{
    public class GameManager : MonoBehaviour
    {
        public GameObject Fin; //게임종료 시 나올 오브젝트
        public GameObject Start; //시작화면 오브젝트
        public GameObject HippoSpawn; //하마 스포너 오브젝트

        public TextMeshProUGUI Timer; //타이머 텍스트
        public TextMeshProUGUI hippoText; //잡은 하마 수 텍스트

        public static float time; //타이머 변수

        public EnumClass stateClass;
        public GameUI gameUI;
        public ScreenProsess screenProsess;
        public ZoZoBasePatton<GameManager> zozo;


        private void Awake()
        {
            stateClass = new EnumClass();

            #region 공용 스테이트 패턴 

            ActionProcess.Enter_StateListener(Init, null, () => { HippoSpawn.SetActive(true); }, null);
            zozo = new ZoZoBasePatton<GameManager>();
            zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));

            #endregion
        }

        void Init()
        {

            time = 60; //시작 할 때 타임을 60초로
            Timer.text = ""; //시작 할 때 타이머 텍스트는 비어있음
            hippoText.text = ""; //시작 할 때 잡은 하마 수 비어있음

            //Fin.SetActive(false); //시작 할 때 게임종료 오브젝트 비활성화
            HippoSpawn.SetActive(false); //시작 할 때 하마 스포너 비활성화
            HippoMove.hippoDie = 0; //잡은 하마 수 0으로 시작

        }

        // Update is called once per frame
        void Update()
        {
            zozo.MGR.Excute(
            () => 
            {
                time -= Time.deltaTime; //타이머 감소

                Timer.text = $"{System.Math.Round(time)}"; //현재 타이머를 소수점 없이 표시
                hippoText.text = $"{HippoMove.hippoDie}"; //현재 잡은 하마 수 표시

                if (time <= 0) //타이머가 0이하면
                {
                    //Fin.SetActive(true); //게임종료 오브젝트 활성화
                    stateClass.resultState = GameResult.Success;
                    zozo.Change(GameState.GameResult);
                    HippoSpawn.SetActive(false); //하마 스포너 비활성화
                    time = 0; //타이머를 0으로
                }
            });
        }
        public void HomeBtn() //홈버튼 함수
        {
            SceneManager.LoadSceneAsync(0); //0번 씬으로 이동
        }
        public void StartBtn() //스타트 버튼 함수
        {
            Time.timeScale = 1; //시간이 흐르게 변경
            HippoSpawn.SetActive(true); //하마 스포너 활성화
            Start.SetActive(false); //시작화면 오브젝트 비활성화
        }
    }
}
