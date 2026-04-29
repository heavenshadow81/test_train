using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.T_Sports.Common
{
    public class BGMFade : MonoBehaviour
    {
        public AudioSource BGMAudio;

        public void BGMFadeOut()
        {
            StartCoroutine(FadeOut());
        }
        IEnumerator FadeOut()
        {
            float vol = BGMAudio.volume;
            while (true)
            {
                vol -= Time.deltaTime;
                BGMAudio.volume = vol;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
