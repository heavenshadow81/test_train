using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StairGame
{
    public class StatueSpawner : MonoBehaviour
    {
        [SerializeField] Statue statue;
        [SerializeField] GameObject[] effects = null;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SoundMGR.Instance.bgmSource.Stop();
                GameManager.Instance.timer.PauseTimer();
                statue.Fade.FadeIn(1f, PlayEffects);
            }           
        }

        private void PlayEffects()
        {
            StartCoroutine(PlayEffectsWithDelay());
        }

        private IEnumerator PlayEffectsWithDelay()
        {
            foreach (GameObject effect in effects)
            {
                yield return new WaitForSeconds(1f); 
                effect.SetActive(true);
            }

            yield return new WaitForSeconds(2.5f);
            GameManager.Instance.GameClear();
        }


    }
}
