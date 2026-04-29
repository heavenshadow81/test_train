using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class SP_Manger : PlayManager_PlayGround
{
    [SerializeField] TextMeshProUGUI scoreText; //스코어 텍스트
    public int score;

    [SerializeField] SP_Planet[] shooter; //트리거를 인식 시킬 콜라이더

    bool cooltime; //터치쿨타임

    protected override void Init()
    {
        base.Init();

        SettingSlot();
    }
    void SettingSlot()
    {
        
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        isTouchable = true;

        if (!cooltime)
        {
            // 터치/마우스 위치를 월드 좌표로 변환
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

            // 터치/마우스 위치에서 카드 찾기
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (hit.collider != null)
            {
                // 터치한 카드가 어떤 태그를 가지고 있는지 확인
                if (hit.collider.CompareTag(TeamNameString))
                {
                    SoundMGR.Instance.SoundPlay("PlayGround_Moving");

                    //내가 클릭한 버튼이 몇번인지 체크
                    TextMeshProUGUI number = hit.collider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    if (number != null)
                    {
                        cooltime= true;

                        //파티클 생성했다 제거
                        GameObject particle = Instantiate(effect, hit.collider.transform.position, Quaternion.identity);
                        Destroy(particle, 1f);

                        //텍스트를 숫자로 변환 후 해당 슈터의 박스콜라이더를 활성화
                        int num = int.Parse(number.text) - 1;
                        BoxCollider2D palnet = shooter[num].gameObject.GetComponent<BoxCollider2D>();
                        palnet.enabled = true;

                        //0.1초 뒤에 콜라이더 비활성화
                        StartCoroutine(TriggerFalse(palnet));
                    }
                }
            }
        }
    }

    IEnumerator TriggerFalse(BoxCollider2D Shooting)
    {
        //0.2초 뒤 다시 박스콜라이더 비활성화
        yield return new WaitForSeconds(0.2f);
        Shooting.enabled = false;

        //현재 스코어 표시
        scoreText.text = $"SCORE : {score}";

        //스코어 5가 될때마다 게이지 올라감
        if (score >= 5)
        {
            score = 0;
            scoreText.text = $"SCORE : {score}";
            ChangeGauge();
        }

        cooltime = false;
    }

    public override void CorrectAnswer(GameObject touched)
    {
    }

    public override void WrongAnswer(GameObject touched)
    {
    }
}
