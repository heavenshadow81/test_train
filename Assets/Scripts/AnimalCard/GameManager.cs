using Bax.P0.Client.UnityWorld.PictureGame;
using Crab;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using static Settings;

namespace AnimalCard
{
    public class GameManager : MonoBehaviour , Game.IMyGameActions
    {
        public GameObject Fin; //게임종료 시 나올 오브젝트
        public GameObject Start; //게임시작 시 나올 오브젝트
        public static GameManager instance; //게임매니저 싱글톤으로 활용
        
        List<CardMove> allCards; //모든 카드 리스트

        CardMove flippedCard; //뒤집힌 카드

        bool isFlipping =false; //카드가 뒤집히고 있는지 체크

        int matchesFound = 0; //매치된 수
        int totalMatch = 6; //총 매치 되어야 하는 수

        public ZoZoBasePatton<GameManager> zozo;
        public EnumClass stateClass;
        public GameUI gameUI;
        public ScreenProsess screenProsess;

        public Game gameAction;
        private void Awake()
        {
            instance = this; //시작할 때 변수에 나를 저장함

            stateClass = new EnumClass();
            #region 공용 스테이트 패턴 

            ActionProcess.Enter_StateListener(null, null, () => 
            {
                Board board = FindObjectOfType<Board>(); //보드 스크립트 변수에 저장
                allCards = board.GetCards(); //올카드에 카드리스트 저장
                Fin.SetActive(false); //게임시작할 때 게임종료 오브젝트 false

                isFlipping = false; //시작할 때 카드 뒤집힘 값 false

                //Start.SetActive(true); //시작하면 시작화면 오브젝트 활성화
                StartBtn();

            }, null);

            zozo = new ZoZoBasePatton<GameManager>();
            zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
            #endregion
        }

        private void OnEnable()
        {
            gameAction = new Game();
            gameAction.Enable();
            gameAction.MyGame.SetCallbacks(this);
            EnhancedTouchSupport.Enable();
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += touch_onFingerDown;
        }

        private void OnDisable()
        {
            gameAction.Disable();
            UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= touch_onFingerDown;
            EnhancedTouchSupport.Disable();
        }

        private void touch_onFingerDown(Finger finger)
        {
            if (stateClass.state == GameState.GamePlay)
            { 
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(finger.currentTouch.screenPosition), Vector2.zero);
                if (hit.collider.TryGetComponent<CardMove>(out var card))
                {
                    card.Click();
                }
            }
        }

        IEnumerator FlipAllcardsRoutine() //카드 모두 뒤집 코루틴
        {
            isFlipping =true; //카드 뒤집는중 true

            yield return new WaitForSeconds(0.5f); //0.5초 대기
            FlipAllCards(); //카드 뒤집기 함수 실행
            yield return new WaitForSeconds(2f); //2초 대기
            FlipAllCards(); //카드 뒤집기 함수 실행
            GameObject.Find("SoundManager").GetComponent<SoundManager>().Flip(); //카드 뒤집기 사운드 재생
            yield return new WaitForSeconds(0.5f); //0.5초 대기
            
            isFlipping =false; //카드 뒤집는중 false
        }

        void FlipAllCards() //카드 뒤집기 함수 실행
        {
            foreach (CardMove card in allCards) //모든 카드를 순서대로 불러옴
            {
                card.FlipCard(); //카드 뒤집기 함수 실행
            }     
        }

        public void CardClicked(CardMove card) //카드 클릭 함수
        {
            if(isFlipping)
            {
                return;
            }
                card.FlipCard(); //카드 뒤집기 함수 실행

                if (flippedCard == null) //뒤집힌 카드가 없다면
                {
                    flippedCard = card; //지금 카드를 뒤집힌 카드로 저장
                }
                else
                {
                    StartCoroutine(CheckMatch(flippedCard, card)); //카드 체크 코루틴 실행
                }
        }

        IEnumerator CheckMatch(CardMove card1,CardMove card2) //1번 카드와 2번 카드가 같은지 체크하는 함수
        {
            isFlipping = true; //뒤집히는 중 true

            if(card1.cardID == card2.cardID)  //1번 카드와 2번 카드가 같다면
            {    
                card1.SetMatched(); //1번 카드 매치완료 함수 실행
                card2.SetMatched(); //2번 카드 매치완료 카드 실행
                yield return new WaitForSeconds(0.2f); //0.2초 대기
                GameObject.Find("SoundManager").GetComponent<SoundManager>().Collect(); //정답 사운드 재생

                matchesFound++; //매치된 수 증가

                if(matchesFound == totalMatch)
                {
                    yield return new WaitForSeconds(1f); //1초 대기

                    stateClass.resultState = GameResult.Success;
                    zozo.Change(GameState.GameResult);

                 //   Fin.SetActive(true); //게임종료 오브젝트 true
                }    
            }
            else //아니라면
            { 
                yield return new WaitForSeconds(0.7f); //1초 대기
                card1.FlipCard(); //1번 카드 뒤집기
                card2.FlipCard(); //2번 카드 뒤집기
                GameObject.Find("SoundManager").GetComponent<SoundManager>().Wrong(); //틀림 사운드 재생
                yield return new WaitForSeconds(0.4f); //0.4초 대기
            }

            isFlipping = false; //뒤집히는 중 false
            flippedCard = null; //뒤집힌 카드를 null로
        }

        public void HomeBtn() //홈버튼 함수
        {
            SceneManager.LoadSceneAsync(0); //0번씬 불러오기
        }
        public void StartBtn() //스타트 버튼 함수
        {
            Time.timeScale = 1; //시간이 흐르게 변경
            //Start.SetActive(false); //시작화면 오브젝트 비활성화

            StartCoroutine("FlipAllcardsRoutine"); //카드 모두 뒤집기 코루틴 실행    
        }

        public void OnDown(InputAction.CallbackContext context)
        {
            if (Settings.instance.mouseToggle.isOn == false) return;
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Settings.instance.MousePos()), Vector2.zero);
            if (hit)
            {
                if (stateClass.state == GameState.GamePlay) // 게임 플레이 상태일때만
                {
                    if (hit.collider.TryGetComponent<CardMove>(out var card))
                    {
                        Debug.Log(card);
                        card.Click();
                    }
                }
            }
        }

        public void OnTouch(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnIsDown(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
