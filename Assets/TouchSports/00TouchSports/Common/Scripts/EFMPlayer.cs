using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.T_Sports.Common
{
    public class EFMPlayer : MonoBehaviour
    {
        [SerializeField]
        AudioSource MyAudio;
        public AudioClip[] clips;
        public int idx;
        private void Awake()
        {
            MyAudio = transform.GetChild(0).GetComponent<AudioSource>();
        }
        public void SetVolume(float value)
        {
            for (int i = 0; i < clips.Length; i++)
            {
                MyAudio.volume = value;
            }
        }
        public void EFMRandomPlay()
        {
            MyAudio.clip = clips[Random.Range(0, clips.Length)];
            MyAudio.Play();
            idx++;
            if (idx >= clips.Length)
                idx = 0;
        }
    }
}
