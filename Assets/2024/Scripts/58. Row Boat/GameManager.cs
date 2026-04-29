using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RowBoat
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] GameObject winUI;
        [SerializeField] GameObject buttons;
        [SerializeField] MagicTimer timer;
        [SerializeField] Image fadeImage;
        [SerializeField] Boat boat;
        [SerializeField] Animation Finish;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void OnEnable()
        {
            fadeImage.DOFade(0f, 1f).OnComplete(() => buttons.gameObject.SetActive(true));

            timer.OnTimerEnd += GameOver;
        }

        private void OnDestroy()
        {
            if (Instance != null)
            {
                Instance = null;
                timer.OnTimerEnd -= GameOver;
            }
        }

        public void GameClear()
        {
            StartCoroutine(ClearRoutine());
        }

        IEnumerator ClearRoutine()
        {
            timer.PauseTimer();
            buttons.SetActive(false);
            Finish.Play("Finish");

            yield return new WaitForSeconds(3f);

            winUI.SetActive(true);
            SoundMGR.Instance.bgmSource.Stop();
            SoundMGR.Instance.SoundPlay("win");
        }

        private void GameOver()
        {
            buttons.gameObject.SetActive(false);
            boat.gameObject.tag = "Untagged";
        }
    }
}
