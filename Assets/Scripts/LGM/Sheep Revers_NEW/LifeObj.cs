using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGM
{
    namespace SheepRevers
    {
        public class LifeObj : MonoBehaviour
        {
            private AudioSource audioSource;

            private void Awake()
            {
                audioSource = GetComponent<AudioSource>();
            }

            public void GameOverCheck()
            {
                DropSound();    // 사운드 출력
                if (GameManager.Instance.life.Count <= 0)
                {
                    GameManager.Instance.stateClass.resultState = GameResult.Fail;
                    GameManager.Instance.zozo.Change(GameState.GameResult);
                }
            }

            public void DropSound()
            {
                audioSource.Play();
            }
        }
    }
}
