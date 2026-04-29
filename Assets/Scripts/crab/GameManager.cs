using LGM.SheepRevers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using static Settings;

namespace Crab
{
    public class GameManager : MonoBehaviour, Game.IMyGameActions
    {
        public GameObject Fin; //게임종료 시 나올 오브젝트
        public GameObject Start; //게임시작 시 나올 오브젝트
        public GameObject CrabHouse; //크랩 부모 오브젝트

        public TextMeshProUGUI Timer; //타이머 텍스트
        public TextMeshProUGUI CrabText; //잡은 크랩 수 텍스트

        public static float time; //타이머 변수

        private CrabMove crabMove;
        private CrabSpawner crabSpawner;

        public EnumClass stateClass;    // 현재 상태 관리 (이걸로 현재 상태 보기)
        public GameUI gameUI;
        public ScreenProsess screenProsess;
        private Game inputGame; // New Input System

        public ZoZoBasePatton<GameManager> zozo;

        public static Action<CrabMove> CrabDieEvent = (crab) => { };



        private void Awake()
        {
            crabSpawner = FindObjectOfType<CrabSpawner>();

            #region 공용 스테이트 패턴 
            stateClass = new EnumClass();
            ActionProcess.Enter_StateListener(Init, null, StartBtn, null);

            zozo = new ZoZoBasePatton<GameManager>();
            zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
            #endregion
        }

        private void Init()
        {
            time = 60; //시작 할 때 타임을 60초로
            Timer.text = ""; //시작 할 때 타이머 텍스트는 비어있음
            CrabText.text = ""; //시작 할 때 잡은 크랩 수 비어있음
            CrabMove.CrabDie = 0; //잡은 크랩 수 0으로 시작
            CrabHouse.SetActive(false); //시작 할 때 크랩 스포너 비활성화
        }

        private void OnEnable()
        {
            // New Input System 사용하기 위한 초기화
            inputGame = new Game();
            inputGame.Enable();
            inputGame.MyGame.AddCallbacks(this);
            EnhancedTouchSupport.Enable();
            // Down 이벤트 사용하기 위해 입력 이벤트에 등록
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += TouchDownEvent;
        }
        // 삭제 시 터치 이벤트 삭제
        private void OnDisable()
        {
            // Down 이벤트 반환하기 위해 입력 이벤트에서 제거
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= TouchDownEvent;
            EnhancedTouchSupport.Disable();
            inputGame.Disable();
        }

        void Update()
        {
            if (zozo != null) zozo.MGR.Excute(
            () =>
            {
                time -= Time.deltaTime; //타이머 감소

                Timer.text = $"{System.Math.Round(time)}"; //현재 타이머를 소수점 없이 표시
                CrabText.text = $"{CrabMove.CrabDie}"; //현재 잡은 크랩 수 표시

                if (time <= 0) //타이머가 0이하면
                {
                    //Fin.SetActive(true); //게임종료 오브젝트 활성화
                    CrabHouse.SetActive(false); //크랩 스포너 비활성화
                    time = 0; //타이머를 0으로

                    stateClass.resultState = GameResult.Success;
                    zozo.Change(GameState.GameResult);
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
            //Start.SetActive(false); //시작화면 오브젝트 비활성화
            CrabHouse.SetActive(true); //시작 할 때 크랩 스포너 활성화

            crabSpawner.Spawner();
        }

        // 터치 이벤트
        public void TouchDownEvent(Finger finger)
        {
            // 마우스 포인터에 Ray를 쏴 Sheep클래스를 보유한 오브젝트가 있을 시 Sheep의 ClickEvent 함수 실행
            RaycastHit2D hit = (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(finger.currentTouch.screenPosition), Vector2.zero));
            if (hit)
            {
                if (hit.collider.TryGetComponent(out CrabMove crab))
                {
                    crab.ClickEvent();
                }
            }
        }

        // 마우스 이벤트
        public void OnDown(InputAction.CallbackContext context)
        {
            if (Settings.instance.mouseToggle.isOn == false) return;
            // 한번만 클릭되도록 체크 값이 1일떄만 실행
            //if (context.ReadValue<float>() == 1f)
            {
                // 마우스 포인터에 Ray를 쏴 Sheep클래스를 보유한 오브젝트가 있을 시 Sheep의 ClickEvent 함수 실행
                RaycastHit2D hit = (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Settings.instance.MousePos()), Vector2.zero, 0));
                if (hit)
                {
                    if (hit.collider.TryGetComponent(out CrabMove crab))
                    {
                        crab.ClickEvent();
                    }
                }
            }
        }
        public void OnTouch(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnIsDown(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }
    }
}
