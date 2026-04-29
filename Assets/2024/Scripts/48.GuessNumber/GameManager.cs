using DG.Tweening;
using ML.TouchModule.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GuessNumber
{
    public class GameManager : TouchManager_3DTouch
    {
        public static GameManager Instance;

        [Header("정답")]
        public int[] answerNums = new int[4];
        private int answerStack = 0;
        private int wrongStack = 0;
        [SerializeField] private MagicLife life;
        [SerializeField] private Chest chest;

        [Header("스테이지")]
        public int stage = 0;
        [SerializeField] private StageManager[] stageManagers = null;

        [Header("카메라")]
        [SerializeField] private CameraMove cam;
        [SerializeField] private Image fade;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void OnEnable()
        {
            fade.DOFade(0, 0.5f).OnComplete(() => cam.CamMove(() => stageManagers[stage].questionManager.SetQuestion()));
        }

        private void OnDestroy()
        {
            if (Instance != null)
            {
                Instance = null;
            }
        }

        public void SetAnswer()
        {
            for (int i = 0; i < answerNums.Length; i++)
            {
                answerNums[i] = NumberManager.Instance.answerNumberObjects[i].GetNumber();
            }
        }

        public override void HandleInput(Vector2 pos)
        {
            if (TryGetHitOption(pos, out Option selectedOption))
            {
                if (CheckAnswer(selectedOption))
                {
                    SoundMGR.Instance.SoundPlay("Answer");
                    CorrectAnswer(selectedOption);
                }
                else
                {
                    SoundMGR.Instance.SoundPlay("Wrong");
                    WrongAnswer();
                }

                isTouchable = true;
            }
            else
            {
                isTouchable = true;
            }
        }

        private bool TryGetHitOption(Vector2 pos, out Option selectedOption)
        {
            Ray ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("Option"))
            {
                stageManagers[stage].optionManager.SetOptionUntagged();
                selectedOption = hit.collider.GetComponent<Option>();
                return true;
            }

            selectedOption = null;
            return false;
        }

        private void CorrectAnswer(Option selectedOption)
        {
            stageManagers[stage].SetAnswerObject(selectedOption.gameObject, answerStack);
            selectedOption.ShowFire();
            answerStack++;  // answerStack 증가

            if (answerStack < 3)
            {
                // 1초 딜레이 후 SetQuestion 호출
                stageManagers[stage].optionManager.ClearNumber(() =>
                {
                    DOVirtual.DelayedCall(1f, () => stageManagers[stage].questionManager.SetQuestion());
                });
            }
            else
            {
                ShowEnding();  // answerStack이 3에 도달하면 종료 처리
            }
        }



        private void WrongAnswer()
        {
            cam.CamShake();
            wrongStack++;
            life.LifeDelete();

            if (wrongStack < 3)
            {
                stageManagers[stage].optionManager.ClearNumber(() => stageManagers[stage].questionManager.SetQuestion());
            }
        }

        private void ShowEnding()
        {
            if (stage < stageManagers.Length - 1)
            {
                answerStack = 0;
                stageManagers[stage].optionManager.ClearNumber();
                cam.CamMove(() =>
                    stageManagers[stage].ShowAnswers(() =>
                        stageManagers[stage].GoToNextStage(() =>
                            cam.CamMove(() => stageManagers[stage].questionManager.SetQuestion())
                        )
                    )
                );
            }
            else
            {
                stageManagers[stage].optionManager.ClearNumber();
                cam.CamMove(() =>
                    stageManagers[stage].ShowAnswers(() =>
                        cam.CamMove(() => chest.RotateChestTop(3f, () =>
                        GameClear()))
                    )
                );
            }
        }

        private bool CheckAnswer(Option option)
        {
            if (answerNums.Length != option.GetOptionNumber().Length)
                return false;

            Array.Sort(answerNums);
            Array.Sort(option.GetOptionNumber());

            return !answerNums.Where((t, i) => t != option.GetOptionNumber()[i]).Any();
        }

        // 게임 클리어 처리 함수
        private void GameClear()
        {
            victoryUI.SetActive(true);
            SoundMGR.Instance.bgmSource.Stop();
            SoundMGR.Instance.SoundPlay("win");
        }
    }
}
