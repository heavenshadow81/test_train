using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dolphin
{
    public class DolphinSound : MonoBehaviour
    {
        public AudioClip collect; //콜렉트 오디오 클립
        public AudioClip bell; //벨 오디오 클립
        public AudioClip bell2; //벨2번 오디오 클립
        public AudioClip jump; //점프 오디오 클림
        public AudioClip bump; //범프 오디오 클립

        AudioSource sound; //오디오 소스

        void OnEnable()
        {
            sound = GetComponent<AudioSource>(); //오디오 소스를 사운드에 컴포넌트 함
        }

        public void JumpSound1() //점프 사운드 함수
        {
            sound.PlayOneShot(bell); //벨 사운드 한 번 재생
            sound.PlayOneShot(jump); //점프 사운드 한 번 재생
        }
        public void JumpSound2() //점프 사운드 함수
        {
            sound.PlayOneShot(bell2); //벨2 사운드 한 번 재생
            sound.PlayOneShot(jump); //점프 사운드 한 번 재생
        }
        public void CollectSound() //콜렉트 사운드 함수
        {
            sound.PlayOneShot(collect); //콜렉트 사운드 한 번 재생
        }
        public void BumpSound() //범프 사운드 함수
        {
            sound.PlayOneShot(bump); //범프 사운드 한번 재생
        }
    }
}

    