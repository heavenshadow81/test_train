using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LGM
{
    namespace CraneGame
    {
        public class ButtonEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
        {
            public ButtonType type; // 해당 버튼 타입
            public Sprite normalSprite; // 기본 버튼 이미지
            public Sprite pushSprite;   // 클릭 버튼 이미지
            protected ButtonManager bManager;   // ButtonManager 정보 

            private void Awake()
            {
                bManager = ButtonManager.Instance;
            }

            // 마우스 클릭(다운) 시 실행
            public virtual void OnPointerDown(PointerEventData eventData)
            {
                gameObject.SetImage(pushSprite);    // 클릭 이미지 변경
                bManager.type = type;    // 입력 중인 버튼 타입 체크
            }
            // 마우스 클릭(UP)해제 시 실행
            public virtual void OnPointerUp(PointerEventData eventData)
            {
                bManager.type = ButtonType.None;    // 기본 타입으로 변경
                gameObject.SetImage(normalSprite);  // 기본 이미지 변경
            }
            // 마우스가 오브젝트에서 나올 시 실행
            public virtual void OnPointerExit(PointerEventData eventData)
            {
                bManager.type = ButtonType.None;    // 기본 타입으로 변경
                gameObject.SetImage(normalSprite);  // 기본 이미지 변경
            }
        }
    }
}