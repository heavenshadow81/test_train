using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatRock
{
    public class SoundManager : MonoBehaviour
    {
        AudioSource sound; //오디오소스 오브젝트
        public AudioClip catSound; //캣사운드 클립
        public AudioClip sadSound; //새드사운드 클립
        public AudioClip btn; //버튼 사운드 클립
        public AudioClip tie; //타이 사운드 클립

        private void OnEnable()
        {
            sound = GetComponent<AudioSource>(); //오디오 소스 컴포넌트
        }

        public void CatSound() //캣사운드 함수
        {
            sound.PlayOneShot(catSound); //캣사운드 한번 재생
        }
        public void BtnSound()
        {
            sound.PlayOneShot(btn);
        }
        public void SadSound()
        {
            sound.PlayOneShot(sadSound);
        }
        public void Tie()
        {
            sound.PlayOneShot(tie);
        }
        public void roundSound()
        {
            sound.Play();
        }

    }
}
