using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace ClimbTheTower
{
    public class OpeningCutHandler : MonoBehaviour
    {
        private GameManager gameManager;
        private MagicTimer timer;
        private PlayableDirector playableDirector;
        private float startDuration = 2f;

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
            timer = FindObjectOfType<MagicTimer>();
            playableDirector = GetComponent<PlayableDirector>();
        }

        private void OnEnable()
        {
            timer.PauseTimer();
            playableDirector.stopped += OnPlayableDirectorStopped;
        }

        private void OnDisable()
        {
            playableDirector.stopped -= OnPlayableDirectorStopped;
        }

        // 타임라인이 종료될 때 호출되는 함수
        private void OnPlayableDirectorStopped(PlayableDirector director)
        {
            StartCoroutine(StartRoutine());
        }

        IEnumerator StartRoutine()
        {
            yield return new WaitForSeconds(startDuration);

            timer.ResumeTimer();
            gameManager.SetButtonsEnable(true);
        }
    }

}
