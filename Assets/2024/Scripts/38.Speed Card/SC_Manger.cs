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

public class SC_Manger : PlayManager_PlayGround
{
    [SerializeField] Transform card; //카드 스포너 오브젝트
    [SerializeField] CardSpawner cardSpawner; //카드 스포너 스크립트

    [SerializeField] TextMeshProUGUI blueText; //블루 텍스트
    int blueScore; //블루 스코어
    [SerializeField] TextMeshProUGUI redText; //레드 텍스트
    int redScore; //레드 스코어

    protected override void Init()
    {
        base.Init();

        SettingSlot();
    }
    void SettingSlot()
    {
        blueText.gameObject.SetActive(true);
        redText.gameObject.SetActive(true);
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        // 터치/마우스 위치를 월드 좌표로 변환
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

        // 터치/마우스 위치에서 카드 찾기
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            SoundMGR.Instance.SoundPlay("PlayGround_Card(1)");

            Transform selectCard = hit.collider.transform;
            Transform currentCard = card.transform.GetChild(0);

            if(currentCard.tag== selectCard.tag)
            {
                currentCard.DOMove(selectCard.position, 0.5f).OnComplete(() =>
                {
                    SoundMGR.Instance.SoundPlay("PlayGround_Click");

                    Destroy(currentCard.gameObject); //현재카드 삭제

                    //파티클 생성 후 2초 뒤 삭제
                    GameObject particle = Instantiate(effect, selectCard.position, Quaternion.identity);
                    Destroy(particle, 2f);

                    cardSpawner.CardSpawn(); //카드 생성

                    ShowScore(selectCard.tag);

                    // 터치 활성화
                    isTouchable = true;
                });
            }     
            else
            {
                currentCard.DOMove(selectCard.position, 0.5f).OnComplete(() =>
                {
                    SoundMGR.Instance.SoundPlay("PlayGround_Moving");

                    Destroy(currentCard.gameObject); //현재카드 삭제

                    //파티클 생성 후 2초 뒤 삭제
                    GameObject particle = Instantiate(perfectEffect, selectCard);
                    Destroy(particle, 2f);

                    cardSpawner.CardSpawn(); //카드 생성

                    SubtractScore(selectCard.tag);

                    // 터치 활성화
                    isTouchable = true;
                });
            }
        }
        else
        {
            // 터치 활성화
            isTouchable = true;
        }
    }

    void ShowScore(string card)
    {
        if(card=="Orange")
        {
            redScore++;
            redText.text = redScore.ToString();
        }
        else
        {
            blueScore++;
            blueText.text = blueScore.ToString();
        }
    }

    void SubtractScore(string card)
    {
        if (card == "Orange")
        {
            if (redScore > 0)
            {
                redScore--;
                redText.text = redScore.ToString();
            }
        }
        else
        {
            if (blueScore > 0)
            {
                blueScore--;
                blueText.text = blueScore.ToString();
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
