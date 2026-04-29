using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicLife : MonoBehaviour
{
    [SerializeField] Animator[] heart; //하트 애니메이션
    [SerializeField] MagicUI UIscript; //UI 스크립트
    int life; //라이프 변수

    public Action OnTimerEnd;
    [SerializeField] private bool isEnding = false;

    private void Start()
    {
        life = 3; //처음 라이프 3으로 시작
    }

    public void LifeDelete()
    {
        //라이프가 남아 있을 때
        if (life > 0)
        {
            //라이프 감소
            life--;
            for (int i = 0; i < heart.Length; i++)
            {
                if (life == i)
                {
                    //라이프 없어지는 애니메이션 재생
                    heart[i].SetTrigger("Die");
                }
            }
        }

        if (life == 0 &&!isEnding)
        {
            UIscript.EndGame(); //게임 종료 화면 재생
        }
        else if (life == 0 && isEnding)
        {
            OnTimerEnd?.Invoke(); //액션 실행
        }
    }
}
