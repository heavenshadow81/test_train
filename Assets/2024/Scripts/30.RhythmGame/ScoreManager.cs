using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RhythmGame
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI txtScore = null;

        [SerializeField] int increaseScore = 10;
        int currentScore = 0;

        [SerializeField] float[] weight = null;
        [SerializeField] int comboBonusScore = 10;

        [SerializeField] ComboManager comboManager = null;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            currentScore = 0;
            txtScore.text = "0";
        }

        public void IncreaseScore(int judgementState)
        {
            // 콤보 증가
            comboManager.IncreaseCombo();

            // 콤보 보너스 점수 계산
            int currentCombo = comboManager.GetCurrentCombo();
            int bonusComboScore = (currentCombo / 10) * comboBonusScore;

            // 가중치 계산
            int weightScore = increaseScore + bonusComboScore;
            weightScore = (int)(weightScore * weight[judgementState]);

            // 점수 반영
            currentScore += weightScore;
            txtScore.text = string.Format("{0:#,##0}", currentScore);
        }

        public int GetCurrentScore()
        {
            return currentScore;
        }
    }
}

