using Crab;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

//namespace LGM
//{
//    namespace CrabTouch
//    {
//        public class GameManager : MonoBehaviour, Game.IMyGameActions
//        {
//            private Game inputGame; // New Input System

//            public EnumClass stateClass;    // 
//            public GameUI gameUI;   //
//            public ScreenProsess screenProsess; //
//            public ZoZoBasePatton<GameManager> zozo;    // 스테이트 머신

//            public TextMeshProUGUI text;
//            public TextMeshProUGUI posText;
//            public string str0;
//            public string str1;
//            public Camera cam;

//            private void Awake()
//            {
//                #region 공용 스테이트 패턴 
//                stateClass = new EnumClass();
//                ActionProcess.Enter_StateListener(null, null, null, null);

//                zozo = new ZoZoBasePatton<GameManager>();
//                zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
//                #endregion
//            }

//            private void OnEnable()
//            {
//                // New Input System 사용하기 위한 초기화
//                inputGame = new Game();
//                inputGame.Enable();
//                inputGame.MyGame.AddCallbacks(this);
//                EnhancedTouchSupport.Enable();
//                // Down 이벤트 사용하기 위해 입력 이벤트에 등록
//                UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += TouchDownEvent;
//            }
//            // 삭제 시 터치 이벤트 삭제
//            private void OnDisable()
//            {
//                // Down 이벤트 반환하기 위해 입력 이벤트에서 제거
//                UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= TouchDownEvent;
//                EnhancedTouchSupport.Disable();
//                inputGame.Disable();
//            }
//            // 터치 이벤트
//            public void TouchDownEvent(Finger finger)
//            {
//                str0 = "OnTouch";
//                // 마우스 포인터에 Ray를 쏴 Sheep클래스를 보유한 오브젝트가 있을 시 Sheep의 ClickEvent 함수 실행
//                RaycastHit2D hit = (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(finger.currentTouch.screenPosition), Vector2.zero));
//                if (hit)
//                {
//                    str0 = "OnTouch_hit";
//                    if (hit.collider.TryGetComponent(out SpriteRenderer spr))
//                    {
//                        str0 = "OnTouch_hit Color";
//                        spr.color = Color.red;
//                    }
//                }
//            }
//            int a = 0;
//            // 마우스 이벤트
//            public void OnDown(InputAction.CallbackContext context)
//            {
//                if (Settings.instance.mouseToggle.isOn == false) 
//                    return;
//                text.text = "OnDown" + a++;
//                posText.text = cam.ScreenToWorldPoint(Settings.instance.MousePos()).ToString();
//                // 한번만 클릭되도록 체크 값이 1일떄만 실행
//                //if (context.ReadValue<float>() == 1f)
//                {
//                    // 마우스 포인터에 Ray를 쏴 Sheep클래스를 보유한 오브젝트가 있을 시 Sheep의 ClickEvent 함수 실행
//                    RaycastHit2D hit = (Physics2D.Raycast(cam.ScreenToWorldPoint(Settings.instance.MousePos()/*Input.mousePosition*/), Vector2.zero, 0));
//                    if (hit)
//                    {
//                        text.text = "Down_hit";
//                        if (hit.collider.TryGetComponent(out SpriteRenderer spr))
//                        {
//                            text.text = "Down_hit spr";
//                            spr.color = Color.red;
//                        }
//                    }
//                }
//            }
//            public void OnTouch(InputAction.CallbackContext context)
//            {
//                throw new NotImplementedException();
//            }

//            public void OnIsDown(InputAction.CallbackContext context)
//            {
//                throw new NotImplementedException();
//            }
//        }
//    }
//}
