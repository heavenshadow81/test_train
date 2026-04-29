using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BumperCar
{
    public class CarSound : MonoBehaviour
    {
        public AudioClip Start; //스타트 오디오 클립
        public AudioClip Break; //브레이크 오디오 클립
        public AudioClip Engine; //엔진 오디오 클립
        public AudioClip Boom; //붐 오디오 클립
        public AudioClip lapSound; //한 바퀴 돌 때 사운드
        public AudioClip btn; //버튼 사운드

        public GameObject EngineSound; //엔진 오디오 오브젝트

        AudioSource sound; //오디오 사운드

        void OnEnable()
        {
            EngineSound.SetActive(false); //게임 시작할 때 엔진 오디오 오브젝트 비활성화
            sound = GetComponent<AudioSource>(); //사운드에 오디오 사운드를 컴포넌트
        }

        private void Update()
        {
            if (PlayerMove.gameStart) //게임스타트가 true라면
            {
                sound.mute = false; //오디오 사운드 뮤트 해제

                if (sound.clip != Engine) //사운드 클립이 엔진이 아니라면
                {
                    sound.PlayOneShot(Start); //스타트 사운드 한번 재생
                    sound.clip = Engine; //오디오 클립을 엔진으로 바꿈
                    sound.mute = true; //오디오 사운드 뮤트
                    EngineSound.SetActive(true); //엔진 사운드 오브젝트 활성화
                }
            }
        }
        public void BreakSound() //브레이크 사운드
        {
            sound.PlayOneShot(Break); //브레이크 사운드를 한번 재생
        }

        public void BoomSound() //붐 사운드
        {
            sound.PlayOneShot(Boom); //붐 사운드를 한번 재생
        }

        public void LapSound() //한 반퀴 돌 때 사운드
        {
            sound.PlayOneShot(lapSound); //바퀴 사운드를 한번 재생
        }

        public void btnSound() //버튼 사운드
        {
            sound.PlayOneShot(btn); //버튼 사운드를 한번 재생
        }
    }
}

    