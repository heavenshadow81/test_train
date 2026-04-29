using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

public class ColorPlane : MonoBehaviour, IPointerDownHandler
{
    public GameObject particle;
    private Sprite mySprite;
    private CPGameManager gm;
    AudioSource audio;

    private void Start()
    {
        gm = CPGameManager.Instance;
        audio = gameObject.GetComponent<AudioSource>();
    }

    private void ClickEvent()
    {
        gm.countting++; // 카운트 증가
        gm.health.value += gm.recovery; // 시간초 증가
        gm.score += gm.onePoint;
        gm.scoreText.text = (gm.score).ToString("N0");  // 점수를 소수점 0번째 자리까지 문자열로 반환
        Destroy(Instantiate(particle, transform.position, Quaternion.identity), 2f);  // particle생성 후 2초 뒤 삭제
        
        // 발판을 전부 맞춰을때
        if (gm.countting == gm.plane.Count)
        {
            gm.ReSetGame(); // 새로운 발판 세팅
        }
        else
        {
            // 게임 오버가 아니라면 정답 발판 랜덤 색 설정
            gm.Set_Random_InformPlane();
            gameObject.SetActive(false);
        }
    }
    private void MistakeEvent()
    {
        gm.health.value -= gm.minusTime; // 제한 시간 5초 감소
        audio.Play();
    }

    // 마우스 버튼이 내려간 순간 호출
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!gm.gameOver)   // 게임오버 상태 체크
        {
            mySprite = GetComponent<Image>().sprite;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // 타겟발판이랑 같으면 정답, 아니먄 이벤트 이벤트
                if (mySprite == gm.informPlane.sprite)
                {
                    ClickEvent();   // 정답 이벤트
                }
                else
                {
                    MistakeEvent(); // 오답 이벤튼
                }
            }
        }
    }

}
