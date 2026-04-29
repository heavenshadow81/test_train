using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class SL_Manger : PlayManager_PlayGround
{
    [SerializeField] Transform btnParent; //클릭할의 부모
    [SerializeField] Sprite[] sprite; //바꿀 이미지
    Image button; //현재 버튼
    int current; //현재 번호
    bool reverse; //현재 번호 반전할지 체크

    [SerializeField] TextMeshProUGUI scoreText; //스코어 텍스트
    int score; //스코어
     

    protected override void Init()
    {
        base.Init();

        ResetSlot();
    }

    void ResetSlot()
    {
        //첫번째 버튼 초록색으로 바꾸고 스케일 1.1로 바꿈
        button = btnParent.GetChild(0).GetComponent<Image>();

        button.sprite = sprite[1];
        button.transform.tag = "Green";
        button.transform.DOScale(1.1f, 0.2f);
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        // 터치 활성화
        isTouchable = true;

        // 터치/마우스 위치를 월드 좌표로 변환
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

        // 터치/마우스 위치에서 카드 찾기
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            Image btn = hit.collider.transform.GetComponent<Image>();

            if(btn.tag=="Green")
            {
                SoundMGR.Instance.SoundPlay("PlayGround_Click");

                //오렌지 색상으로 바꾸고 태그도 변경
                btn.sprite = sprite[0];
                btn.transform.tag = "Orange";

                //터치 안되게 막았다가 스케일 1이되면 다시 터치 활성화
                btn.transform.GetComponent<BoxCollider2D>().enabled = false;
                btn.transform.DOScale(1f, 0.2f).OnComplete(()=>
                {
                    btn.transform.GetComponent<BoxCollider2D>().enabled = true;
                });

                NextButton();

                //파티클 생성 후 2초 뒤 삭제
                GameObject particle = Instantiate(effect, btn.transform.position, Quaternion.identity);
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

    void NextButton()
    {     
        //스코어 상승 후 표기
        score++;
        scoreText.text = score.ToString();

        if (current>=24)
        {
            //현재 번호가 24보다 크면 리셋
            SoundMGR.Instance.SoundPlay("PlayGround_Reset");
            reverse = true;
        }
        else if(current<=0)
        {
            SoundMGR.Instance.SoundPlay("PlayGround_Reset");
            reverse = false;
        }

        if(reverse)
        {
            current--; //현재 번호 감소

            //현재 번호가 24보다 작을때 현재 번호 초록색으로 바꾸고 스케일 1.1로 바꿈
            button = btnParent.GetChild(current).GetComponent<Image>();

            button.sprite = sprite[1];
            button.transform.tag = "Green";
            button.transform.DOScale(1.1f, 0.2f);
        }
        else
        {
            current++; //현재 번호 상승

            //현재 번호가 24보다 작을때 현재 번호 초록색으로 바꾸고 스케일 1.1로 바꿈
            button = btnParent.GetChild(current).GetComponent<Image>();

            button.sprite = sprite[1];
            button.transform.tag = "Green";
            button.transform.DOScale(1.1f, 0.2f);
        }
       
    }

    public override void CorrectAnswer(GameObject spider)
    {
    }

    public override void WrongAnswer(GameObject spider)
    {
    }
}
