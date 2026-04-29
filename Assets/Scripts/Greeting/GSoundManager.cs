using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WorldCulture
{
    public class GsoundManager : MonoBehaviour
    {
        public AudioClip kr; //한국 오디오 클립
        public AudioClip krFood; //한국 음식 오디오 클립
        public AudioClip krFlower; //한국 꽃 오디오 클립
        public AudioClip krAnimal; //한국 동물 오디오 클립
        public AudioClip krStructure; //한국 건물 오디오 클립

        public AudioClip uk; //영국 오디오 클립
        public AudioClip ukFood; //영국 음식 오디오 클립
        public AudioClip ukFlower; //영국 꽃 오디오 클립
        public AudioClip ukAnimal; //영국 동물 오디오 클립
        public AudioClip ukStructure; //영국 건물 오디오 클립

        public AudioClip cn; //중국 오디오 클립
        public AudioClip cnFood; //중국 음식 오디오 클립
        public AudioClip cnFlower; //중국 꽃 오디오 클립
        public AudioClip cnAnimal; //중국 동물 오디오 클립
        public AudioClip cnStructure; //중국 건물 오디오 클립

        public AudioClip jp; //일본 오디오 클립
        public AudioClip jpFood; //일본 음식 오디오 클립
        public AudioClip jpFlower; //일본 꽃 오디오 클립
        public AudioClip jpAnimal; //일본 동물 오디오 클립
        public AudioClip jpStructure; //일본 건물 오디오 클립

        public AudioClip sp; //스페인 오디오 클립
        public AudioClip spFood; //스페인 음식 오디오 클립
        public AudioClip spFlower; //스페인 꽃 오디오 클립
        public AudioClip spAnimal; //스페인 동물 오디오 클립
        public AudioClip spStructure; //스페인 건물 오디오 클립

        public AudioClip homeBtn; //홈버튼 오디오 클립

        AudioSource sound; //오디오 소스 변수

        void OnEnable()
        {
            sound = GetComponent<AudioSource>(); //사운드에 오디오 소스 컴포넌트
        }

        public void KrPlay() //한국 사운드 함수
        {
            sound.clip=kr; //오디오 클립 한국 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void KrFoodPlay() //한국 음식 사운드 함수
        {
            sound.clip = krFood; //오디오 클립 한국 음식 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void KrFlowerPlay() //한국 꽃 사운드 함수
        {
            sound.clip = krFlower; //오디오 클립 한국 꽃 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void KrStructurePlay() //한국 건물 사운드 함수
        {
            sound.clip = krStructure; //오디오 클립 한국 건물 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void KrAnimalPlay() //한국 동물 사운드 함수
        {
            sound.clip = krAnimal; //오디오 클립 한국 동물 사운드로 변경
            sound.Play(); //오디오 플레이 
        }

        public void UkPlay() //영국 사운드 함수
        {
            sound.clip = uk; //오디오 클립 영국 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void UkFoodPlay() //영국 음식 사운드 함수
        {
            sound.clip = ukFood; //오디오 클립 영국 음식 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void UkFlowerPlay() //영국 꽃 사운드 함수
        {
            sound.clip = ukFlower; //오디오 클립 영국 꽃 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void UkStructurePlay() //영국 건물 사운드 함수
        {
            sound.clip = ukStructure; //오디오 클립 영국 건물 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void UkAnimalPlay() //영국 동물 사운드 함수
        {
            sound.clip = ukAnimal; //오디오 클립 영국 동물 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void CnPlay() //중국 사운드 함수
        {
            sound.clip = cn; //오디오 클립 중국 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void CnFoodPlay() //중국 음식 사운드 함수
        {
            sound.clip = cnFood; //오디오 클립 중국 음식 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void CnFlowerPlay() //중국 꽃 사운드 함수
        {
            sound.clip = cnFlower; //오디오 클립 중국 꽃 사운드로 변경
            sound.Play(); //오디오 플레이 
        }

        public void CnStructurePlay() //중국 건물 사운드 함수
        {
            sound.clip = cnStructure; //오디오 클립 중국 건물 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void CnAnimalPlay() //중국 동물 사운드 함수
        {
            sound.clip = cnAnimal; //오디오 클립 중국 동물 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void JpPlay() //일본 사운드 함수
        {
            sound.clip = jp; //오디오 클립 일본 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void JpFoodPlay() //일본 음식 사운드 함수
        {
            sound.clip = jpFood; //오디오 클립 일본 음식 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void JpFlowerPlay() //일본 꽃 사운드 함수
        {
            sound.clip = jpFlower; //오디오 클립 일본 꽃 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void JpStructurePlay() //일본 건물 사운드 함수
        {
            sound.clip = jpStructure; //오디오 클립 일본 건물 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void JpAnimalPlay() //일본 동물 사운드 함수
        {
            sound.clip = jpAnimal; //오디오 클립 일본 동물 사운드로 변경
            sound.Play(); //오디오 플레이 
        }

        public void SpPlay() //스페인 사운드 함수
        {
            sound.clip = sp; //오디오 클립 스페인 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void SpFoodPlay() //스페인 음식 사운드 함수
        {
            sound.clip = spFood; //오디오 클립 스페인 음식 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void SpFlowerPlay() //스페인 꽃 사운드 함수
        {
            sound.clip = spFlower; //오디오 클립 스페인 꽃 사운드로 변경
            sound.Play(); //오디오 플레이 
        }

        public void SpStructurePlay() //스페인 건물 사운드 함수
        {
            sound.clip = spStructure; //오디오 클립 스페인 건물 사운드로 변경
            sound.Play(); //오디오 플레이 
        }
        public void SpAnimalPlay() //스페인 동물 사운드 함수
        {
            sound.clip = spAnimal; //오디오 클립 스페인 동물 사운드로 변경
            sound.Play(); //오디오 플레이 
        }

        public void HomeBtn() //홈버튼 함수
        {
            sound.PlayOneShot(homeBtn); //홈버튼 사운드 한 번재생
        }
    }
}
