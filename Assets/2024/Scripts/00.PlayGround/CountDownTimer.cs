using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class CountdownTimer : MonoBehaviour
{
    [Header("타이머")]
    [SerializeField, Range(0, 59)] int minutes = 0; // 초기 분 설정
    [SerializeField, Range(0, 59)] int seconds = 0; // 초기 초 설정
    [SerializeField] TextMeshProUGUI timerText; // 타이머를 표시할 TextMeshProUGUI

    [Header("점수")]
    [SerializeField] TextMeshProUGUI greenScore;
    [SerializeField] TextMeshProUGUI orangeScore;

    [Header("게임승리")]
    [SerializeField] GameObject greenVictoryUI;
    [SerializeField] GameObject orangeVictoryUI;
    [SerializeField] GameObject drawUI;
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
            // 분과 초 계산
            int displayMinutes = Mathf.FloorToInt(remainingTime / 60);
            int displaySeconds = Mathf.FloorToInt(remainingTime % 60);

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
            timerText.text = string.Format("{0:00}:{1:00}", displayMinutes, displaySeconds);

            // 1초 대기
            yield return new WaitForSeconds(1.0f);

            // 남은 시간 감소
            remainingTime -= 1.0f;
        }

        // 타이머가 0이 되었을 때 처리
        timerText.text = "00:00";
        isTimerRunning = false;
        countdownCoroutine = null; // 코루틴이 끝난 후 null로 설정
        OnTimerEnd();
    }

    protected virtual void OnTimerEnd()
    {
        DOTween.KillAll();

        // 타이머가 끝났을 때의 처리
        if(greenScore != null && orangeScore != null)
        {
            CheckScore();
        }     
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

    void CheckScore()
    {
        // 그린팀과 오렌지팀의 점수 비교
        int greenScoreValue = int.Parse(greenScore.text);
        int orangeScoreValue = int.Parse(orangeScore.text);

        SoundMGR.Instance.SoundPlay("PlayGround_Victory");

        gameCanvas.SetActive(false);

        if (greenScoreValue > orangeScoreValue)
        {
            // 그린팀 승리
            greenVictoryUI.SetActive(true);
        }
        else if (orangeScoreValue > greenScoreValue)
        {
            // 오렌지팀 승리
            orangeVictoryUI.SetActive(true);
        }
        else
        {
            // 무승부 처리
            if (drawUI != null)
            {
                drawUI.SetActive(true);
            }
            else
            {
                greenVictoryUI.SetActive(true);
                orangeVictoryUI.SetActive(true);
            }

        }
    }
}
