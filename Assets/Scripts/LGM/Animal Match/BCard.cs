using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LGM
{
    namespace AnimalMatch
    {
        // ***각자 ClickA, ClickB에 저장해야함으로 굳이 ACard, BCard로 나눔***
        public class BCard : Card
        {
            public override void OnPointerDown(PointerEventData eventData)
            {
                // 이미 클릭되었거나 클릭 불가 상태인지 체크
                if (ClickCheck())
                {
                    return;
                }
                InputManager.Instance.source.PlayOneShot(clickClip);    // 클릭 사운드 재생
                InputManager.Instance.ClickB = this;    // 클릭된 오브젝트의 정보를 저장
            }
        }
    }
}