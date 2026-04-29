using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using DG.Tweening;

public class Lunge_Manager : PlayManager_PlayGround
{
    [Header("버튼")]
    [SerializeField] Image[] buttons; //클릭할 버튼
    [SerializeField] Sprite[] btnImage; //바뀔 버튼이미지

    [Header("점수")]
    int score; // 점수
    [SerializeField]TextMeshProUGUI scoreText; //점수 텍스트
    private Coroutine scoreCoroutine; // 점수 증가 코루틴

    protected override void Init()
    {
        base.Init();

        StartCoroutine(ButtonChange());
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        
        // 터치 활성화
        isTouchable = true;

        // 터치/마우스 위치를 월드 좌표로 변환
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

        // 터치/마우스 위치에서 카드 찾기
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        // Collider가 감지되었고, 해당 객체의 태그가 "Green"인 경우
        if (hit.collider != null)
        {
            Image btn = hit.collider.transform.GetComponent<Image>();

            if (hit.collider.CompareTag("Green"))
            {
                SoundMGR.Instance.SoundPlay("PlayGround_Click");

                score++;
                scoreText.text = score.ToString();

                //파티클 생성 후 2초 뒤 삭제
                GameObject particle = Instantiate(effect, hit.transform.position, Quaternion.identity);
                Destroy(particle, 2f);
            }
            else
            {
                SoundMGR.Instance.SoundPlay("PlayGround_Moving");

                btn.transform.GetComponent<BoxCollider2D>().enabled = false;
                btn.transform.DOScale(0.8f, 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                {
                    btn.transform.GetComponent<BoxCollider2D>().enabled = true;
                });
            }
        }
    }


    IEnumerator ButtonChange()
    {
        int previousRandom = -1; // 이전 랜덤 값을 저장할 변수

        while (true) // 반복되는 루프 안에서 사용
        {
            int random;

            // 이전 값과 다른 값을 생성할 때까지 반복
            do
            {
                random = Random.Range(0, buttons.Length);
            } 
            while (random == previousRandom);
            {
                previousRandom = random;
            }

            // 버튼 초록색으로 바꾸고 스케일 1.1로 변경
            buttons[random].sprite = btnImage[1];
            buttons[random].transform.tag = "Green";
            buttons[random].transform.DOScale(1.1f, 0.2f);

            yield return new WaitForSeconds(2);

            // 버튼 원래 상태로 복원
            buttons[random].sprite = btnImage[0];
            buttons[random].transform.tag = "Orange";
            buttons[random].transform.DOScale(1f, 0.2f);

            if (scoreCoroutine != null)
            {
                StopCoroutine(scoreCoroutine);
                scoreCoroutine = null;
            }
        }
    }
    public override void CorrectAnswer(GameObject spider)
    {
    }

    public override void WrongAnswer(GameObject spider)
    {
    }
}