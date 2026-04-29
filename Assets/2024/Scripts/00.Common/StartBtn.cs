using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    public GameObject hand;
    public void StartBtn()
    {
        hand.SetActive(true);

        SoundMGR.Instance.SoundPlay("24.¹ßÆÇ");

        SoundMGR.Instance.bgmSource.clip = SoundMGR.Instance.soundData[42].Clip;
        SoundMGR.Instance.bgmSource.Play();

        gameObject.SetActive(false);
    }
}

