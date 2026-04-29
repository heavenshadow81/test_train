using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card_ACM : MonoBehaviour
{
    Image cardImage;
    TextMeshProUGUI slotText;
    bool isTurned;
    [SerializeField] Sprite backSprite;

    private void Awake()
    {
        cardImage = GetComponent<Image>();
        slotText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetCardBack()
    {
        cardImage.sprite = backSprite;
        slotText.text = "?";
    }
    public void SetCardForward(Sprite newSprite)
    {
        cardImage.sprite = newSprite;
        slotText.text = "";
    }

    public Sprite GetSprite()
    {
        return cardImage.sprite;
    }

    public void TurnCard()
    {
        Quaternion targetRotation = isTurned ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        transform.DORotateQuaternion(targetRotation, 0.3f).OnComplete(() =>
        {
            isTurned = !isTurned; // 상태 변경
        });
    }

    public void SetCardAlphaAndScale()
    {
        Color newColor = cardImage.color;
        bool isAlphaOne = newColor.a == 1;

        newColor.a = isAlphaOne ? 0 : 1; // 현재 알파 값이 1이면 0으로, 0이면 1로 변경
        cardImage.color = newColor;

        // 스케일 변경
        Vector3 newScale = isAlphaOne ? Vector3.zero : Vector3.one * 2;
        transform.localScale = newScale;
    }

    public void SetCardAlphaAndScaleZero()
    {
        Color newColor = cardImage.color;
        newColor.a = 0; 
        cardImage.color = newColor;
        transform.localScale = Vector3.zero;
    }
}
