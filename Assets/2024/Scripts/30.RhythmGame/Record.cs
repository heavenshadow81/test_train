using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RhythmGame
{
    public class Record : MonoBehaviour
    {
        [Header("레코드")]
        [SerializeField] TextMeshProUGUI recordText;

        [Header("점수")]
        [SerializeField] TextMeshProUGUI greenScore;
        [SerializeField] TextMeshProUGUI orangeScore;

        [Header("색상")]
        Color normalColor = Color.white;
        [SerializeField] Color highlightColor; // 베스트 스코어를 넘을 때의 색상

        RhythmTimer timer;
        private int bestScore = 0;

        private void Awake()
        {
            timer = FindObjectOfType<RhythmTimer>();
        }

        private void Start()
        {
            LoadRecord(); // 기존 베스트 스코어 로드

            StartCoroutine(CheckScoresInRealtime());
        }

        // 실시간으로 점수를 확인하는 코루틴
        IEnumerator CheckScoresInRealtime()
        {
            while(timer.isTimerRunning)
            {
                int green = int.Parse(greenScore.text.Replace(",", "")); // greenScore 텍스트 값을 int로 변환
                int orange = int.Parse(orangeScore.text.Replace(",", "")); // orangeScore 텍스트 값을 int로 변환

                // greenScore와 orangeScore 중 더 큰 값을 선택
                int currentScore = Mathf.Max(green, orange);

                // 더 큰 값이 bestScore를 넘으면 하이라이트 처리
                if (currentScore > bestScore)
                {
                    // greenScore가 더 크면 greenScore를 하이라이트
                    if (green == currentScore)
                    {
                        greenScore.color = highlightColor;
                        orangeScore.color = normalColor; // orangeScore는 기본 색상으로 유지
                    }
                    // orangeScore가 더 크면 orangeScore를 하이라이트
                    else if (orange == currentScore)
                    {
                        orangeScore.color = highlightColor;
                        greenScore.color = normalColor; // greenScore는 기본 색상으로 유지
                    }

                    recordText.color = highlightColor;
                    recordText.text = string.Format("{0:#,##0}", currentScore);
                }


                yield return new WaitForSeconds(0.5f); // 0.5초마다 점수 확인
            }       
        }

        // 점수를 저장하는 함수
        public void SaveRecord(int newScore)
        {
            // 기존 스코어와 새로운 스코어 비교 후, 더 높은 점수를 저장
            int currentBestScore = PlayerPrefs.GetInt("RhythmScore", 0);
            if (newScore > currentBestScore)
            {
                PlayerPrefs.SetInt("RhythmScore", newScore);
                PlayerPrefs.Save(); // 저장
            }
        }

        // 스코어를 불러오는 함수
        public void LoadRecord()
        {
            // 불러온 스코어를 텍스트에 표시
            bestScore = PlayerPrefs.GetInt("RhythmScore", 0);
            recordText.text = string.Format("{0:#,##0}", bestScore);
        }
    }
}
