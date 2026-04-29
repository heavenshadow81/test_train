using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LGM
{
    namespace OXPlaneGame
    {
        public class StartUI : MonoBehaviour, IPointerDownHandler
        {
            public GameObject uiObj;
            public void OnPointerDown(PointerEventData eventData)
            {
                GameManager.Instance.clickRock = false;
                uiObj.SetActive(false);
            }
        }
    }
}

