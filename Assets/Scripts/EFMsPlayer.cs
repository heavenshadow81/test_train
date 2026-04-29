using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class EFMsPlayer : MonoBehaviour
    {
        public AudioSource MyAudioSource;
        public AudioClip[] clips;
        public float volume;

        private void Awake()
        {
            InitValues();
        }
        public void InitValues()
        {
            if (volume == 0)
                volume = 1;

            MyAudioSource = this.GetComponent<AudioSource>();
        }
        public void PlayRandomClips()
        {
            SetVolume(volume);
            int rand = Random.Range(0, clips.Length);
            MyAudioSource.clip = clips[rand];
            MyAudioSource.Play();
        }
        public void SetVolume(float vol)
        {
            MyAudioSource.volume = vol;
        }
    }

}
