using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkiSound : MonoBehaviour
{
    public AudioClip Break; //브레이크 사운드 클립
    public AudioClip Boom; //붐 사운드 클립
    public AudioClip correct; //코렉트 사운드 클립

    AudioSource sound; //오디오 소스 변수

    void OnEnable()
    {
        sound = GetComponent<AudioSource>(); //오디오 소스 불러옴
    }

    public void BreakSound() //브레이크 사운드 함수
    {
        sound.PlayOneShot(Break); //브레이크 사운드 한 번 재생
    }

    public void BoomSound() //붐 사운드 함수
    {
        sound.PlayOneShot(Boom); //붐 사운드 한 번 재생
    }

    public void CorrectSound() //코렉트 사운드 함수
    {
        sound.PlayOneShot(correct); //코렉트 사운드 한 번 재생
    }
}

    