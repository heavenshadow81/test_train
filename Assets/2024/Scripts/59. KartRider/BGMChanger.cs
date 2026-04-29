using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRider
{
    public class BGMChanger : MonoBehaviour
    {
        [SerializeField] AudioClip nextBGM;
        private bool isChanged = false;

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("KartBody"))
            {
                if (isChanged) return;

                ChangeBGM();
                isChanged = true;
            }
        }

        private void ChangeBGM()
        {
            StartCoroutine(ChangeBGMWithFade());
        }

        private IEnumerator ChangeBGMWithFade(float fadeDuration = 1.5f)
        {
            // 페이드 아웃
            for (float volume = 1f; volume >= 0f; volume -= Time.deltaTime / fadeDuration)
            {
                SoundMGR.Instance.bgmSource.volume = volume;
                yield return null;
            }

            SoundMGR.Instance.bgmSource.Stop();
            SoundMGR.Instance.bgmSource.clip = nextBGM;
            SoundMGR.Instance.bgmSource.Play();

            // 페이드 인
            for (float volume = 0f; volume <= 1f; volume += Time.deltaTime / fadeDuration)
            {
                SoundMGR.Instance.bgmSource.volume = volume;
                yield return null;
            }
        }
    }
}
