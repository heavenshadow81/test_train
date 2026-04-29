using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClimbTheTower
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] Button[] buttons = null;
        [SerializeField] GameObject[] towers = null; // 타워 배열
        [SerializeField] GameObject winUI;
        private MagicTimer timer;
        private int currentTowerIndex = -1; // 현재 활성화된 타워의 인덱스 (처음엔 -1로 설정)

        private void Awake()
        {
            timer = FindObjectOfType<MagicTimer>();
        }

        // 타워를 순차적으로 활성화하는 함수
        public void ChangeTower()
        {
            // 이전 타워 비활성화
            if (currentTowerIndex >= 0)
            {
                towers[currentTowerIndex].SetActive(false);
            }

            // 다음 타워 활성화
            currentTowerIndex++;

            // 더 이상 타워가 없으면 게임 클리어
            if (currentTowerIndex >= towers.Length)
            {
                GameClear();
                return; // 더 이상 진행하지 않음
            }

            towers[currentTowerIndex].SetActive(true); // 다음 타워 활성화
        }

        // 게임 클리어 처리 함수
        private void GameClear()
        {
            winUI.SetActive(true);
            SoundMGR.Instance.bgmSource.Stop();
            SoundMGR.Instance.SoundPlay("win");
        }


        public void SetButtonsEnable(bool enable)
        {
            foreach (Button button in buttons)
            {
                button.enabled = enable;
            }
        }


        public int GetTowerIndex()
        {
            return currentTowerIndex;
        }

        public int GetTowerLength()
        {
            return towers.Length - 1;
        }

        public void PauseTimer()
        {
            timer.PauseTimer();
        }
    }
}
