using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace LGM
{
    namespace OXPlaneGame
    {
        public class PlaneEvent : MonoBehaviour, Game.IMyGameActions
        {
            protected Sequence scaleClick;
            protected AudioSource audioSource;
            protected GameManager manager;
            public Vector2Int id;
            public GameObject obj;
            private Vector3 clickScale;
            private Vector3 beforeScale;

            private Game inputGame; // New Input System

            protected virtual void Awake()
            {
                TryGetComponent(out audioSource);   // AudioSource 존재 시 저장
                manager = GameManager.Instance;
                obj = gameObject.transform.parent.gameObject;

                clickScale = transform.localScale * 0.95f;
                beforeScale = transform.localScale;

                // 섬 클릭 시 크기 작아졌다가 원래 크기로 복구
                scaleClick = DOTween.Sequence().SetAutoKill(false).Pause().
                    Append(transform.DOScale(clickScale, 0.5f)).
                    Append(transform.DOScale(beforeScale, 0.5f)); 
            }
            /*protected virtual void OnMouseDown()    // 오브젝트 클릭 시 실행 함수
            {
                PlaneDownEvent();
            }*/
            protected virtual void PlaneDownEvent()   // 섬 클릭 시 이벤트
            {
                // 여러번 밟히지 않게 한번 밟으면 비활성화
                GetComponent<Collider2D>().enabled = false;
            }
            public void PlaneShotAudio(AudioClip clip)
            {
                audioSource.PlayOneShot(clip);
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
                    if (hit.collider.TryGetComponent(out PlaneEvent plane))
                    {
                        plane.PlaneDownEvent();
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
                        if (hit.collider.TryGetComponent(out PlaneEvent plane))
                        {
                            plane.PlaneDownEvent();
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

