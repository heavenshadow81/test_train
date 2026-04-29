using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LGM
{
    namespace AnimalMatch
    {
        public class Card : MonoBehaviour, IPointerDownHandler
        {
            public AudioClip clickClip;
            public bool isClick = false;
            public CardType type;

            public virtual void OnPointerDown(PointerEventData eventData) { }

            // 오브젝트가 클릭됬는지 체크하는 함수
            protected bool ClickCheck()
            {
                if (InputManager.Instance.isAllClick || isClick)
                {
                    return true;
                }
                transform.parent.SetAsLastSibling();
                return false;
            }
        }
    }
}

