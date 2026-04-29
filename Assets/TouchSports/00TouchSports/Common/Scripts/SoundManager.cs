using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.T_Sports.Common
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;
        public EFMPlayer[] EFMs;
        public AudioSource BGM;
        public EFMPlayer Pitching, Cheers, Strike, Bhoo;

        public float DefaultEFMVolume;
        public float DefaultBGMVolume;

        /// <summary>
        /// 효과음 기본 볼륨 세팅(0.5f)
        /// </summary>
        private void Awake()
        {
            instance = this;
            DefaultEFMVolume = 0.5f;
            DefaultBGMVolume = 0.3f;
            EFMs = new EFMPlayer[this.transform.childCount - 1];
            BGM = this.transform.GetChild(0).GetComponent<AudioSource>();
            for (int i = 0; i < EFMs.Length; i++)
            {
                EFMs[i] = this.transform.GetChild(i+1).GetComponent<EFMPlayer>();
                EFMs[i].SetVolume(DefaultEFMVolume);
            }
        }

        /// <summary>
        /// 셋팅에서 변경한 값에따라 효과음 조정(0 ~ 10)
        /// </summary>
        /// <param name="idx"></param>
        public void SetEFMVolume(float vol)
        {
            for (int i = 0; i < EFMs.Length; i++)
            {
                EFMs[i].SetVolume(vol);
            }
        }

        public void SetBGMVolume(float vol)
        {
            BGM.volume = vol;
        }
    }
}
