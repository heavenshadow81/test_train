using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class SS_Manger : PlayManager_PlayGround
{
    [SerializeField] Image[] buttons; //클릭할 버튼
    [SerializeField] Sprite[] sprite; //바꿀 이미지

    [SerializeField] TextMeshProUGUI scoreText; //스코어 텍스
    int score; //스코어

    protected override void Init()
    {
        base.Init();

        SettingSlot();
    }
    void SettingSlot()
    {
        //랜덤한 버튼 색상과 태그 그린으로 바꾸고 스케일 1.1로 변경
        int random = Random.Range(0, buttons.Length);

        buttons[random].sprite = sprite[1];
        buttons[random].transform.tag = "Green";
        buttons[random].transform.DOScale(1.1f, 0.2f);
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

                ShowScore();

                //파티클 생성 후 2초 뒤 삭제
                GameObject particle = Instantiate(effect, btn.transform.position, Quaternion.identity);
                Destroy(particle, 2f);
            }
            else
            {
                SoundMGR.Instance.SoundPlay("PlayGround_Moving");

                btn.transform.GetComponent<BoxCollider2D>().enabled = false;
                btn.transform.DOScale(0.8f, 0.1f).SetLoops(2,LoopType.Yoyo).OnComplete(()=>
                {
                    btn.transform.GetComponent<BoxCollider2D>().enabled = true;
                });

                //파티클 생성 후 2초 뒤 삭제
                GameObject particle = Instantiate(perfectEffect, btn.transform.position, Quaternion.identity);
                Destroy(particle, 2f);

                DeductScore();
            }
     
        }
    }

    void ShowScore()
    {
        SoundMGR.Instance.SoundPlay("PlayGround_Reset");

        //스코어 상승 후 다시 셋팅
        score++;
        scoreText.text = score.ToString();
        SettingSlot();
    }
    void DeductScore()
    {
        //스코어 상승 후 다시 셋팅
        if (score > 0)
        {
            score--;
            scoreText.text = score.ToString();
        }
    }

    public override void CorrectAnswer(GameObject spider)
    {
    }

    public override void WrongAnswer(GameObject spider)
    {
    }
}
