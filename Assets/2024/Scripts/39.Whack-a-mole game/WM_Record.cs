using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

    public class WM_Record : MonoBehaviour
    {
        [Header("레코드")]
        [SerializeField] TextMeshProUGUI recordText;

        [Header("점수")]
        [SerializeField] TextMeshProUGUI scoreText;

        [Header("색상")]
        [SerializeField] Color highlightColor; // 베스트 스코어를 넘을 때의 색상

        private int bestScore = 0;

        private void Start()
        {
            LoadRecord(); // 기존 베스트 스코어 로드

            StartCoroutine(CheckScoresInRealtime());
        }

        // 실시간으로 점수를 확인하는 코루틴
        IEnumerator CheckScoresInRealtime()
        {
            while(true)
            {
                int score = int.Parse(scoreText.text.Replace(",", "")); // score 텍스트 값을 int로 변환


                // 더 큰 값이 bestScore를 넘으면 하이라이트 처리
                if (score > bestScore)
                {
                    recordText.text = string.Format("{0:#,##0}", score);
                }

                yield return new WaitForSeconds(0.5f); // 0.5초마다 점수 확인
            }       
        }

        // 점수를 저장하는 함수
        public void SaveRecord(int newScore)
        {
            // 기존 스코어와 새로운 스코어 비교 후, 더 높은 점수를 저장
            int currentBestScore = PlayerPrefs.GetInt("WMScore", 0);
            if (newScore > currentBestScore)
            {
                PlayerPrefs.SetInt("WMScore", newScore);
                PlayerPrefs.Save(); // 저장
            }
        }

        // 스코어를 불러오는 함수
        public void LoadRecord()
        {
            // 불러온 스코어를 텍스트에 표시
            bestScore = PlayerPrefs.GetInt("WMScore", 0);
            recordText.text = string.Format("{0:#,##0}", bestScore);
        }
    }