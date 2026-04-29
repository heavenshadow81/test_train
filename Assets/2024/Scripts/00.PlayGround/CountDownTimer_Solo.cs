using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class CountdownTimer_Solo : MonoBehaviour
{
    [Header("타이머")]
    [SerializeField, Range(0, 59)] int minutes = 0; // 초기 분 설정
    [SerializeField, Range(0, 59)] int seconds = 0; // 초기 초 설정
    [SerializeField] TextMeshProUGUI timerText; // 타이머를 표시할 TextMeshProUGUI

    [Header("게임승리")]
    [SerializeField] GameObject gameOver;
    [SerializeField] GameObject gameCanvas;

    float remainingTime;
    bool isTimerRunning = false;
    Coroutine countdownCoroutine; // 코루틴 인스턴스 저장 변수

    void Start()
    {
        if(GameManager_PlayGround.Instance != null)
        {
            GameManager_PlayGround.Instance.OnGameStart += StartTimer;
        }       
    }

    void OnDisable()
    {
        if (GameManager_PlayGround.Instance != null)
        {
            GameManager_PlayGround.Instance.OnGameStart -= StartTimer;
        }
    }

    void StartTimer()
    {
        // 초기 남은 시간을 초 단위로 설정
        remainingTime = minutes * 60 + seconds;
        countdownCoroutine = StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        isTimerRunning = true;

        while (remainingTime > 0)
        {
            // 타이머가 10초 이하일 때 텍스트 색상 변경
            if (remainingTime <= 10)
            {
                timerText.color = Color.red; // 예: 빨간색으로 변경
            }
            else
            {
                timerText.color = Color.white; // 기본 색상(흰색)으로 변경
            }

            // 타이머 텍스트 업데이트 
            //{0}은 첫 번째 인수를 참조하고, {1}은 두 번째 인수를 참조
            //:00은 숫자가 한 자리일 경우 앞에 0을 추가하여 두 자리로 
            timerText.text = remainingTime.ToString();

            // 1초 대기
            yield return new WaitForSeconds(1.0f);

            // 남은 시간 감소
            remainingTime -= 1.0f;
        }

        // 타이머가 0이 되었을 때 처리
        timerText.text = "0";
        isTimerRunning = false;
        countdownCoroutine = null; // 코루틴이 끝난 후 null로 설정
        OnTimerEnd();
    }

    protected virtual void OnTimerEnd()
    {
        SoundMGR.Instance.SoundPlay("PlayGround_Victory");

        DOTween.KillAll();

        gameCanvas.SetActive(false);
        gameOver.SetActive(true);
    }

    public void PauseTimer()
    {
        if (countdownCoroutine != null)
        {
            Debug.Log("스탑");
            StopCoroutine(countdownCoroutine);
            isTimerRunning = false;
            countdownCoroutine = null; // 코루틴이 멈춘 후 null로 설정
        }
    }

    public void ResumeTimer()
    {
        if (!isTimerRunning && countdownCoroutine == null)
        {
            countdownCoroutine = StartCoroutine(StartCountdown());
        }
    }

    public void AddTime(float additionalSeconds)
    {
        // 남은 시간에 초를 추가
        remainingTime += additionalSeconds;

        // 만약 남은 시간이 0보다 크고 타이머가 멈춰있으면 재개
        if (!isTimerRunning && remainingTime > 0 && countdownCoroutine == null)
        {
            countdownCoroutine = StartCoroutine(StartCountdown());
        }
    }

    public void SubtractTime(float subtractSeconds)
    {
        // 남은 시간에서 초를 빼기
        remainingTime -= subtractSeconds;

        // 시간이 0보다 작아지지 않도록 보장
        if (remainingTime < 0)
        {
            remainingTime = 0;
        }

        // 만약 타이머가 멈춰있고 남은 시간이 0보다 크다면 타이머를 재개
        if (!isTimerRunning && remainingTime > 0 && countdownCoroutine == null)
        {
            countdownCoroutine = StartCoroutine(StartCountdown());
        }
    }
}
