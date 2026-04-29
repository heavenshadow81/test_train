using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class BGMAudioFade : MonoBehaviour
    {
        public AudioSource audio;
        public float MaxVolume;
        public float MinVolume;
        public float MyVolume;
        public bool Fading;

        private void Awake()
        {
            audio = this.GetComponent<AudioSource>();
        }
        public void VoluemeFadeUpStart()
        {
            audio.Play();
            StartCoroutine(VolumeFadeUp());
        }
        public void VoluemeFadeDownStart()
        {
            StartCoroutine(VolumeFadeDown());
        }
        IEnumerator VolumeFadeUp()
        {
            if (!Fading)
            {
                Fading = true;
                while (Fading)
                {
                    MyVolume += Time.deltaTime * 0.2f;
                    if (MyVolume < MaxVolume)
                    {
                        audio.volume = MyVolume;
                    }
                    else
                    {
                        Fading = false;
                        audio.volume = MaxVolume;
                    }
                    yield return new WaitForSeconds(0.1f);
                }
            }            
        }
        IEnumerator VolumeFadeDown()
        {
            if (!Fading)
            {
                Fading = true;
                while (Fading)
                {
                    MyVolume -= Time.deltaTime * 0.35f;
                    if (MyVolume > MinVolume)
                    {
                        audio.volume = MyVolume;
                    }
                    else
                    {
                        Fading = false;
                        audio.volume = MinVolume;
                    }
                    yield return new WaitForSeconds(0.1f);
                }
            }
            audio.Stop();
        }

    }
}

