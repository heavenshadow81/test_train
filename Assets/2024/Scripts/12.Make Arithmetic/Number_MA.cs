using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Number_MA : MonoBehaviour
{
    Image cardImage;
    TextMeshProUGUI numberText;
    public Sequence turnSequence;
    public RotateAnimation rotateAnimation;

    private void Awake()
    {
        cardImage = GetComponent<Image>();
        numberText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Init()
    {
        numberText.text = "?";
    }

    public int GetNumber()
    {
        // 문자열을 정수로 변환
        if (int.TryParse(numberText.text, out int number))
        {
            return number;
        }
        else
        {
            // 변환에 실패한 경우 예외를 던지거나 기본값을 반환
            throw new System.FormatException("Invalid number format");
            // 또는 return 0; 처럼 기본값을 반환할 수도 있음
        }
    }

    public void SetNumberText<T>(T number)
    {     
        numberText.text = number.ToString(); 
    }

    public void TurnCard()
    {
        // 45도까지 회전했다가 -45도까지 회전하는 애니메이션
        turnSequence = DOTween.Sequence();

        turnSequence.Append(transform.DORotate(new Vector3(0, 180, 0), 0.5f).SetEase(Ease.InOutSine))
                        .Append(transform.DORotate(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.InOutSine));                    
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
