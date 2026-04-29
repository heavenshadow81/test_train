using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CookingGame
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("페이드")]
        [SerializeField] Image fadeImage;
        [SerializeField] CameraMove cam;

        [Header("성공")]
        [SerializeField] GameObject successUI;
        [SerializeField] GameObject emptyDish;
        [SerializeField] GameObject finishDish;

        [Header("시작")]
        [SerializeField] GameObject fire;
        [SerializeField] GameObject steak;
        [SerializeField] FryingPan fryingPan;

        [Header("타이머")]
        [SerializeField] MagicTimer timer;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            if (Instance != null)
            {
                Instance = null;
            }
        } 

        public void GameStart()
        {
            StartCoroutine(StartRoutine());
        }

        IEnumerator StartRoutine()
        {
            yield return new WaitForSeconds(2f);

            fadeImage.DOFade(1f, 0.5f).OnComplete(() =>
            {
                cam.MoveCam();
                fadeImage.DOFade(0f, 0.5f).OnComplete(() =>
                {
                    fryingPan.enabled = true;
                    fire.SetActive(true);
                    steak.SetActive(true);
                });
            });
        }

        public void GameClear()
        {
            timer.PauseTimer();
            SoundMGR.Instance.bgmSource.Stop();

            fadeImage.DOFade(1f, 1f).OnComplete(() =>
            {
                cam.MoveCam(2);
                fire.SetActive(false);
                steak.SetActive(false);
                emptyDish.SetActive(false);
                finishDish.SetActive(true);
                fadeImage.DOFade(0f, 0.5f).OnComplete(() =>
                {
                    SoundMGR.Instance.SoundPlay("Finish");
                    DOVirtual.DelayedCall(2f, () =>
                    {
                        successUI.SetActive(true);
                        SoundMGR.Instance.SoundPlay("win");
                    });
                });
            });
        }
    }
}
