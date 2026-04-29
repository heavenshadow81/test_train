using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slot_SQ : MonoBehaviour
{
    private TextMeshProUGUI slotText;
    [SerializeField] private Transform targetTransform;
    Vector3 originalPosition; 
    private float duration = 0.5f;


    private void Awake()
    {
        slotText = GetComponentInChildren<TextMeshProUGUI>();
        originalPosition = transform.position;
    }

    public string GetText()
    {
        return slotText.text;
    }

    public void MoveSlot(TweenCallback onCompleteCallback)
    {
        transform.DOMove(targetTransform.position, duration).OnComplete(onCompleteCallback);
    }

    public void ReturnMoveSlot(TweenCallback onCompleteCallback = null)
    {
        gameObject.transform.localScale = Vector3.zero;
        transform.DOMove(originalPosition, duration).OnComplete(() =>
        {
            gameObject.transform.localScale = Vector3.one * 2;
            onCompleteCallback?.Invoke(); // 콜백 실행
        });
    }
}
