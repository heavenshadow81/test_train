using LGM.OXPlaneGame;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LGM
{
    namespace SheepRevers
    {
        public class UIManager : Singleton<UIManager>
        {
            private GameManager manager;
            public TextMeshProUGUI score;   // 점수 텍스트
            public GameObject gameClear;
            public GameObject gameOver;

            private void Awake()
            {
                manager = GameManager.Instance;
            }

            public void UpdateLogic()
            {
                score.text = manager.score + "/" + manager.maxScore;
                if (manager.score >= manager.maxScore)
                {
                    GameManager.Instance.stateClass.resultState = GameResult.Success;
                    GameManager.Instance.zozo.Change(GameState.GameResult);
                }
            }
        }
    }
}

