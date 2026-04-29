using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnderTheSea
{ 
    public class SoundManager : MonoBehaviour
    {
        AudioSource sound; //오디오소스 오브젝트
        public AudioClip correctSound; //코렉트 사운드 클립
        public AudioClip wrongSound; //롱 사운드 클립

        private void OnEnable()
        {
            sound = GetComponent<AudioSource>(); //오디오 소스 컴포넌트
        }

        public void Correct() //코렉트 사운드 함수
        {
            sound.PlayOneShot(correctSound); //코렉트 사운드 한번 재생
        }
        public void Wrong() //롱 사운드 함수
        {
            sound.PlayOneShot(wrongSound); //롱 사운드 한번 재생
        }
        public void Jelly()
        {
            sound.Play(); //사운드 재생
        }


    }
}
