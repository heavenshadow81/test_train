using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalCard
{
    public class SoundManager : MonoBehaviour
    {
        AudioSource sound; //오디오 소스 변수

        public AudioClip flip; //카드 뒤집기 사운드
        public AudioClip collect; //정답 사운드
        public AudioClip wrong; //틀림 사운드
        void OnEnable()
        {
            sound = GetComponent<AudioSource>(); //오디오 소스 컴포넌트
        }
        public void Flip() //카드 뒤집기 사운드 함수
        {
            sound.Play(); //카드 뒤집기 사운드 재생
        }
        public void Collect() //카드 정답 사운드 함수
        {
            sound.PlayOneShot(collect); //정답 사운드 한 번 재생
        }
        public void Wrong() //카드 틀림 사운드 함수
        {
            sound.PlayOneShot(wrong); //카드 틀림 사운드 한 번 재생
        }
    }
}
