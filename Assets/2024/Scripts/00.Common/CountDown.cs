using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class CountDown : MonoBehaviour
{
    private SoundMGR soundMGR;
    public static event Action OnCountdownFinished; // 카운트다운 종료 이벤트
    public static bool gameStart;
    public TextMeshProUGUI[] count;
    int number;

    private void Awake()
    {
        soundMGR = FindObjectOfType<SoundMGR>();
    }
    void OnEnable()
    {
        gameStart = false;
        Invoke("PlaySound", 0.3f);
        Invoke("CountChange", 0.7f); //게임 시작 후 0.7초 뒤에 카운트 숫자 변경
    }

    private void Start()
    {
        SoundMGR.Instance.bgmSource.Stop();
    }

    void PlaySound()
    {
        SoundMGR.Instance.SoundPlay("PlayGround_CountDown"); // 사운드 재생
    }

    void CountChange()
    {
        if (gameStart) return; // 이미 게임이 시작되었으면 함수 종료

        for (int i = 0; i < count.Length; i++)
        {
            if (count[i].gameObject.activeSelf)
            {
                count[i].transform.DOScale(0, 0.3f).OnComplete(ShowCount);
            }
        }

        if(number>=3)
        {
            gameObject.GetComponent<Image>().DOFade(0, 0.3f).OnComplete(() =>
            {
                gameStart = true; // 게임 시작 플래그 설정
                OnCountdownFinished?.Invoke(); // 게임 시작 이벤트 호출
                SoundMGR.Instance.bgmSource.Play();
            }); // 어두운 배경 꺼짐
        }

        number++;
        //print(number);
    }
    
    void ShowCount()
    {
        for (int i = 1; i < count.Length; i++)
        {
            if (number == i) //현재 숫자 체크
            {
                count[i-1].gameObject.SetActive(false); //전의 숫자 비활성화
                count[i].gameObject.SetActive(true); //현재 숫자 활성화
            }
        }
        Invoke("CountChange", 0.7f);
    }
}
