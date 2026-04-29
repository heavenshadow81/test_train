using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Telephone
{
    public class StartButton : MonoBehaviour
    {
        public void StartBtn()
        {
            SoundMGR.Instance.SoundPlay("24.½ºÀ®");

            SoundMGR.Instance.bgmSource.clip = SoundMGR.Instance.soundData[43].Clip;
            SoundMGR.Instance.bgmSource.Play();

            gameObject.SetActive(false);
        }
    }
}