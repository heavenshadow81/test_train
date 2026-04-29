using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 게임 씬 시작시 활성화 되는 안내문구를 씬 아무데나 클릭해도 닫히게 하는 스크립트
public class EmptySpaceTouchEffect : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    GameObject alertObject;

    [SerializeField]
    GameObject emptyObject;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (alertObject.activeSelf)
        {
            alertObject.SetActive(false);
            emptyObject.SetActive(false);
        }
    }
}
