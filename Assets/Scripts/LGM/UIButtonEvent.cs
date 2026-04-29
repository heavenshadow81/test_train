using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIButtonEvent : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent clickEvent;   // click 시 호출할 함수형 변수

    public void OnPointerDown(PointerEventData eventData)
    {
        clickEvent.Invoke();    // clickEvent에 들어 있는 이벤트 실행
    }
}
