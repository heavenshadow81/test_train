using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerSlot_SQ : MonoBehaviour
{
    [Header("애니메이션")]
    Image slotImage;
    [SerializeField] Color imageColor;
    private float duration = 0.5f;

    [Header("이펙트")]
    [SerializeField] private GameObject correctEffect;
    [SerializeField] private GameObject wrongEffect;

    [Header("텍스트")]
    private TextMeshProUGUI answerText;

    Sequence sequence; // 시퀀스 객체를 클래스 레벨에서 선언

    void Awake()
    {
        slotImage = GetComponent<Image>();
        answerText= GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        PlayAnimation();
    }

    public void PlayAnimation()
    {
        Color originColor = slotImage.color;

        // DOTween 시퀀스를 사용하여 애니메이션 순차적으로 실행
        sequence = DOTween.Sequence(); // 시퀀스를 초기화

        sequence.Append(slotImage.DOColor(imageColor, duration));
        sequence.Append(slotImage.DOColor(originColor, duration));

        // 반복 설정
        sequence.SetLoops(-1); // 무한 반복

        // 시퀀스 실행
        sequence.Play();
    }

    public void StopAnimation()
    {
        if (sequence != null && sequence.IsActive())
        {
            sequence.Kill(); // 시퀀스 종료
            slotImage.color = Color.white; // 애니메이션 종료 후 색상 초기화 (필요한 경우)
        }
    }

    public void SetTextOrigin()
    {
        answerText.fontSize = 45f;
        answerText.text = "?";
        correctEffect.SetActive(false);
    }

    public void SetTextAnswer()
    {
        answerText.fontSize = 60f;
        answerText.text = "";
        correctEffect.SetActive(true);
    }

    public void ActiveWrongEffect()
    {
        wrongEffect.SetActive(true);
    }
}
