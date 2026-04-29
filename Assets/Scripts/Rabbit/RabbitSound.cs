using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rabbit
{
    public class RabbitSound : MonoBehaviour
    {
        public AudioClip collect; //콜렉트 오디오 클립
        public AudioClip drop; //드롭 오디오 클립
        public AudioClip walk; //워크 오디오 클립

        AudioSource sound; //오디오 소스 변수

        void OnEnable()
        {
            sound = GetComponent<AudioSource>(); //오디오 소스 컴포넌트
        }

        public void DropSound() //드롭 사운드 함수
        {
            sound.PlayOneShot(drop); //드롭 사운드 한 번 재생
        }

        public void WalkSound() //워크 사운드 함수
        {
            sound.PlayOneShot(walk); //워크 사운드 한 번 재생
        }

        public void CollectSound() //콜렉트 사운드 함수
        {
            sound.PlayOneShot(collect); //콜렉트 사운드 한 번 재생
        }
    }
}

    