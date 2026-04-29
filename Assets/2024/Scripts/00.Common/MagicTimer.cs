using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MagicTimer : MonoBehaviour
{
    [Header("타이머")]
    [SerializeField] Slider timerSlider; //타이머 슬라이드
    [SerializeField] float totalTime = 60f;  // 기본 타이머 시간 (Inspector에서 조절 가능)
    private float timeRemaining;
    bool isStart; //게임 시작 체크

    [SerializeField] MagicUI UIscript;  // UI스크립트;

    [Header("스코어")]
    public Action OnTimerEnd;
    [SerializeField] private bool isScore = false;

    // Start is called before the first frame update
    void Start()
    {
        // 타이머 시작 시 슬라이더 초기 설정
        if (timerSlider != null)
        {
            timerSlider.maxValue = totalTime;  // 슬라이더의 최대 값을 총 시간으로 설정
            timerSlider.value = totalTime;     // 슬라이더의 현재 값을 총 시간으로 설정
        }

        timeRemaining = totalTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerSlider != null && isStart)
        {
            if (timeRemaining > 0)
            {
                // 남은 시간을 줄이고 슬라이더의 값도 줄임
                timeRemaining -= Time.deltaTime;
                timerSlider.value = timeRemaining;
            }
            else
            {
                if (!isScore)
                {
                    UIscript.EndGame();

                }

                OnTimerEnd?.Invoke();
                isStart = false;
            }
        }

    }

    public void PauseTimer()
    {
        isStart = false;
    }

    public void ResumeTimer()
    {
        isStart = true;
    }
}
