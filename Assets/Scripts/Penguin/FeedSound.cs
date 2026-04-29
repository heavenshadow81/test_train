using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Penguin
{ 
    public class FeedSound : MonoBehaviour
    {
        public AudioClip collect; //콜렉트 오디오 클립
        public AudioClip button; //버튼 오디오 클립
        public AudioClip water; //물장구 오디오 클립

        AudioSource sound; // 오디오 소스

        void OnEnable()
        {
            sound = GetComponent<AudioSource>(); //사운드에 오디오 소스 컴포넌트
        }


        public void BtnSound() //버튼 사운드 함수
        {
            sound.PlayOneShot(button); //버튼 사운드 한 번 재생
            sound.PlayOneShot(water); //물장구 사운드 한 번 재생
        }

        public void CollectSound() //콜렉트 사운드 함수
        {
            sound.PlayOneShot(collect); //콜렉트 사운드 한 번 재생
        }
    }
}

    