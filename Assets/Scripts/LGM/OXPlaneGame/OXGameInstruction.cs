using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OXGameInstruction : MonoBehaviour,IPointerDownHandler
{
    private void Awake()
    {
        Time.timeScale = 0;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
