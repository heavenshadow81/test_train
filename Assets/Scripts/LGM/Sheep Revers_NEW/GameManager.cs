using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using static Settings;

namespace LGM
{
    namespace SheepRevers
    {
        public class GameManager : Singleton<GameManager>, Game.IMyGameActions
        {

            public EnumClass stateClass;
            public ZoZoBasePatton<FishGameManager> zozo;
            public ScreenProsess screenProsess;

            private Game inputGame; // New Input System

            public Queue<GameObject> life = new Queue<GameObject>();    // 목숨
            public Transform lifeParent;    // 목숨 오브젝트의 상위 객체
            public Trapeze[] trapezes;  // 오른쪽, 왼쪽 그네
            public List<GameObject> carrots;    // 당근들
            public int maxCarrot = 5; // 활성화할 당근 갯수
            public int score = 0;   // 현재 점수
            public int maxScore = 30;   // 게임 클리어 점수
            public int number = 5;  // 양 생성 시 부여할 Layer 값(점점 앞으로 정렬되도록)

            private void Awake()
            {
                stateClass = new EnumClass();

                ActionProcess.Enter_StateListener(Init,
                null, play, null);

                zozo = new ZoZoBasePatton<FishGameManager>();
                zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
            }

            void Init()
            {
                for (int i = 0; i < lifeParent.childCount; i++)
                {
                    Transform child = lifeParent.GetChild(i);   // 목숨 Transform 정보
                    life.Enqueue(child.GetChild(0).gameObject); // 뺼때 바로 삭제되는 큐 타입 사용
                }
            }

            int random;
            private void play()
            {
                for (int i = 0; i < maxCarrot; i++)
                {
                    // 비활성화된 랜덤한 당근을 중복되지 않도록 maxCarrot만큼 활성화
                    random = Random.Range(0, carrots.Count);
                    // 활성화 되있는거 선택 시 다시 선택
                    if (carrots[random].activeSelf)
                    {
                        i--;
                        continue;
                    }
                    carrots[random].SetActive(true);
                }
            }

            private void Update()
            {
                if (zozo != null) zozo.MGR.Excute(() => 
                {
                    UIManager.Instance.UpdateLogic();
                });
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
            // 터치 이벤트
            private void TouchDownEvent(Finger finger)
            {
                // 마우스 포인터에 Ray를 쏴 Sheep클래스를 보유한 오브젝트가 있을 시 Sheep의 ClickEvent 함수 실행
                RaycastHit2D hit = (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(finger.currentTouch.screenPosition), Vector2.zero));
                if (hit)
                {
                    if (hit.collider.TryGetComponent(out Sheep sheep))
                    {
                        sheep.ClickEvent();
                    }
                }
            }
            // 목숨 감소 이벤트
            public void MinusLife()
            {
                if (life.TryDequeue(out GameObject rock))
                {
                    rock.SetActive(true);
                }
            }
            // 비어있는 당근 활성화
            public void ActiveCarrot()
            {
                int random;
                // 당근이 최대치가 될 때까지 활성화
                while (carrots.ACount() < maxCarrot + 1)  
                {
                    // 비활성화된 랜덤한 당근을 maxCarrot만큼 활성화
                    random = Random.Range(0, carrots.Count);
                    if (!carrots[random].activeSelf)
                    {
                        carrots[random].SetActive(true);
                    }
                }
            }

            public void OnDown(InputAction.CallbackContext context)
            {
                if (Settings.instance.mouseToggle.isOn == false) return;
                // 한번만 클릭되도록 체크 값이 1일떄만 실행
                if (context.ReadValue<float>() == 1f)
                {
                    // 마우스 포인터에 Ray를 쏴 Sheep클래스를 보유한 오브젝트가 있을 시 Sheep의 ClickEvent 함수 실행
                    RaycastHit2D hit = (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Settings.instance.MousePos()), Vector2.zero, 0));
                    if (hit)
                    {
                        if (hit.collider.TryGetComponent(out Sheep sheep))
                        {
                            sheep.ClickEvent();
                        }
                    }
                }
            }

            public void OnTouch(InputAction.CallbackContext context)
            {
                
            }

            public void OnIsDown(InputAction.CallbackContext context)
            {
                
            }
        }
    }
}

