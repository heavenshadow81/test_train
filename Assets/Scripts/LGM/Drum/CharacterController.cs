using LGM.Drum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LGM
{
    namespace Drum
    {
        public class CharacterController : MonoBehaviour, IPointerDownHandler
        {
            public CharacterEvent cEvent;

            // 클릭(다운) 시 애니메이션 시작
            public void OnPointerDown(PointerEventData eventData)
            {
                cEvent.AnimationPlay();
            }
        }
    }
}

