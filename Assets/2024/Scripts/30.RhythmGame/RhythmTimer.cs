using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RhythmGame
{
    public class RhythmTimer : MonoBehaviour
    {
        [Header("타이머")]
        [SerializeField] float bufferTime = 5; // 뒤에 노래 끝나고 텀 두는 시간
        [SerializeField] TextMeshProUGUI timerText; // 타이머를 표시할 TextMeshProUGUI

        [Header("점수")]
        [SerializeField] TextMeshProUGUI greenScore;
        [SerializeField] TextMeshProUGUI orangeScore;
        [SerializeField] Record record;

        [Header("게임승리")]
        [SerializeField] GameObject greenVictoryUI;
        [SerializeField] GameObject orangeVictoryUI;
        [SerializeField] GameObject drawUI;
        [SerializeField] GameObject gameCanvas;

        float remainingTime;
        float musicEndTime; // 음악이 끝나는 시간
        public bool isTimerRunning = false;
        Coroutine countdownCoroutine; // 코루틴 인스턴스 저장 변수
        GameManager gameManager;

        void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        public void StartTimer()
        {
            // 음악이 끝나는 시간을 저장 (버퍼타임 제외)
            musicEndTime = SoundMGR.Instance.bgmSource.clip.length;

            // 초기 남은 시간을 음악 길이 + 버퍼타임으로 설정
            remainingTime = musicEndTime + bufferTime;

            timerText.gameObject.SetActive(true);
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
                timerText.text = string.Format("{0:00} : {1:00}", displayMinutes, displaySeconds);

                // 1초 대기
                yield return new WaitForSeconds(1.0f);

                // 남은 시간 감소
                remainingTime -= 1.0f;

                // 음악이 끝나는 시점에 게임 종료 처리
                if (remainingTime <= bufferTime && gameManager.isStartGame)
                {
                    gameManager.isStartGame = false; // 음악이 끝난 시점에 게임 종료
                }
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
            if (greenScore != null && orangeScore != null)
            {
                CheckScore();
            }
        }

        public void ResumeTimer()
        {
            if (!isTimerRunning && countdownCoroutine == null)
            {
                countdownCoroutine = StartCoroutine(StartCountdown());
            }
        }

        void CheckScore()
        {
            // 그린팀과 오렌지팀의 점수 비교
            int greenScoreValue = int.Parse(greenScore.text.Replace(",", ""));
            int orangeScoreValue = int.Parse(orangeScore.text.Replace(",", ""));
            int bestScore = Mathf.Max(greenScoreValue, orangeScoreValue); // 더 큰 값 찾기

            record.SaveRecord(bestScore);   // 베스트 스코어 저장

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
}

