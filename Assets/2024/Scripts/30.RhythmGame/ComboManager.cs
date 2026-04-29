using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;  // DoTween을 사용하기 위해 추가

namespace RhythmGame
{
    public class ComboManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI txtCombo = null;

        int currentCombo = 0;
        int maxCombo = 0;

        private void Start()
        {
            txtCombo.gameObject.SetActive(false);
        }

        public void IncreaseCombo(int comboNum = 1)
        {
            currentCombo += comboNum;
            txtCombo.text = string.Format("{0:#,##0} COMBO", currentCombo);

            if (maxCombo < currentCombo) maxCombo = currentCombo;

            if (currentCombo > 2)
            {
                txtCombo.gameObject.SetActive(true);
                ComboAnim();
            }
        }

        public void ComboAnim()
        {
            // 텍스트 크기를 1.5배로 키우고, 다시 원래 크기로 돌아오는 애니메이션
            txtCombo.rectTransform.DOScale(3f, 0.2f).SetEase(Ease.OutBack)
                .OnComplete(() => txtCombo.rectTransform.DOScale(1f, 0.2f).SetEase(Ease.InBack));

            // 텍스트 색상을 점점 밝아지게 했다가 다시 어둡게
            txtCombo.DOColor(Color.red, 0.2f).OnComplete(() => txtCombo.DOColor(Color.white, 0.2f));
        }

        public int GetCurrentCombo()
        {
            return currentCombo;
        }

        public void ResetCombo()
        {
            currentCombo = 0;
            txtCombo.text = "0";
            txtCombo.gameObject.SetActive(false);
        }

        public int GetMaxCombo()
        {
            return maxCombo;
        }
    }
}
