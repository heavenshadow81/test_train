using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//家府 犁积 淬寸
public class SoundContainers : MonoBehaviour
{
    //家府
    [SerializeField]
    AudioSource soundmaker;
    //家府 努赋
    [SerializeField]
    AudioClip[] sounds;
    private void OnEnable()
    {
        Coding.ContentsController.Instance.SoundPlay += AudioSoundPlay;
    }
    private void OnDisable()
    {
        Coding.ContentsController.Instance.SoundPlay -= AudioSoundPlay;
    }
    //家府 犁积
    void AudioSoundPlay(int index)
    {
        soundmaker.PlayOneShot(sounds[index]);
        print("家府 犁积 肯丰");
    }

}
