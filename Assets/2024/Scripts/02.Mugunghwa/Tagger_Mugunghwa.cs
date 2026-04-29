using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using static DOTweenPro;

public class Tagger_Mugunghwa : MonoBehaviour
{
    [Header("텍스트")]
    float textDuration = 1f; // "무궁화 꽃이 피었습니다!" 텍스트 표시 시간

    [Header("고개돌리기")]
    float headTurnDuration = 1f; // 고개 돌리는 시간
    float headTurnTimer = 0f;
    bool isHeadTurned = false;

    [Header("애니메이션")]
    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;

    public TextMeshProUGUI textUI; // 텍스트 UI 오브젝트 참조

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(2f);

        GameManager_Mugunghwa.Instance.isPlaying = true;
        ShowText();
    }
    private void Update()
    {
        if (isHeadTurned) // 고개돌아가 있는 중이고
        {
            if (GameManager_Mugunghwa.Instance.isPlaying) // 플레이중일때만 실행하기
            {
                TurnHead();
            }
        }
    }

    private void TurnHead()
    {
        // 고개 돌리기 시작 이후 경과된 시간
        headTurnTimer += Time.deltaTime;

        // 스프라이트를 뒤집어 고개를 돌리기
        spriteRenderer.sprite = sprites[1];

        // 고개 돌리기 시간이 경과했는지 확인
        if (headTurnTimer <= headTurnDuration)
        {
            // 플레이어가 움직이고 있다면 게임 오버
            if (GameManager_Mugunghwa.Instance.IsPlayerMoving())
            {
                GameManager_Mugunghwa.Instance.GameOver();
                spriteRenderer.sprite = sprites[2];
            }
        }
        else
        {
            // 고개 돌리기 타이머를 초기화하고 스프라이트를 원래대로
            headTurnTimer = 0f;
            spriteRenderer.sprite = sprites[0];
            isHeadTurned = false;

            // 다시 텍스트를 표시
            ShowText();
        }
    }

    private void ShowText()
    {
        SoundMGR.Instance.SoundPlay("무궁화_술래");

        // "무궁화 꽃이 피었습니다!" 텍스트 표시
        textUI.gameObject.SetActive(true);
        textUI.text = "무궁화 꽃이 피었습니다!!";

        // TMPDOText 활용하여 텍스트 찍기 (using static DOTweenPro 해줘야함)
        var textTween = TMPDOText(textUI, textDuration);
        textTween.OnComplete(() =>
        {
            isHeadTurned = true;
            textUI.gameObject.SetActive(false);
        });
    }
}
