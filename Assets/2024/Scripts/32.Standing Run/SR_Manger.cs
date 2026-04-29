using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using ML.PlaywallKids.Aquarium;

public class SR_Manger : PlayManager_PlayGround
{
    [SerializeField] Image[] buttons; //클릭할 버튼

    [SerializeField] TextMeshProUGUI scoreText; //스코어 텍스
    int score; //스코어

    protected override void Init()
    {
        base.Init();

        SettingSlot();
    }
    void SettingSlot()
    {
        //랜덤한 버튼 색상과 바꾸고 스케일 1.1로 변경
        int random = Random.Range(0, buttons.Length);

        buttons[random].color = Color.yellow;
        buttons[random].transform.DOScale(1.1f, 0.2f);

        //버튼 활성화
        buttons[random].transform.GetComponent<BoxCollider2D>().enabled = true;

        buttons[random].transform.tag = TeamNameString;
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

            if(btn.tag==TeamNameString)
            {
                SoundMGR.Instance.SoundPlay("PlayGround_Click");

                //화이트 색상으로 바꿈
                btn.color = Color.white;

                //터치 안되게 막고 스케일 1로 변경
                btn.transform.GetComponent<BoxCollider2D>().enabled = false;
                btn.transform.tag = "Untagged";

                btn.transform.DOScale(1f, 0.2f).OnComplete(()=>
                {
                    btn.transform.GetComponent<BoxCollider2D>().enabled = true;
                });

                ShowScore();

                //파티클 생성 후 2초 뒤 삭제
                GameObject particle = Instantiate(effect, btn.transform.position, Quaternion.identity);
                Destroy(particle, 2f);
            }
            else if(hit.collider.name.Contains(TeamNameString))
            {
                DeductScore();

                SoundMGR.Instance.SoundPlay("PlayGround_Moving");

                btn.transform.GetComponent<BoxCollider2D>().enabled = false;
                btn.transform.DOScale(0.8f, 0.1f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                {
                    btn.transform.GetComponent<BoxCollider2D>().enabled = true;
                });
            }
        }
    }

    void ShowScore()
    {
        SoundMGR.Instance.SoundPlay("PlayGround_Reset");

        //스코어 상승 후 스코어텍스트 작아졌다 커지면서 변경
        score++;
        scoreText.transform.DOScale(0, 0.2f).OnComplete(() => 
        {
            scoreText.text = score.ToString();
            scoreText.transform.DOScale(1, 0.2f);
            SettingSlot();
        });
    }

    void DeductScore()
    {
        //스코어 감소 후 스코어텍스트 작아졌다 커지면서 변경
        if (score > 0)
        {
            score--;
            scoreText.transform.DOScale(0, 0.2f).OnComplete(() =>
            {
                scoreText.text = score.ToString();
                scoreText.transform.DOScale(1, 0.2f);
            });
        }
    }

    public override void CorrectAnswer(GameObject spider)
    {
    }

    public override void WrongAnswer(GameObject spider)
    {
    }
}
