using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Hippo
{
    public class HippoMove : MonoBehaviour
    {
        Animator anim; //애니메이터를 저장할 변수
        Button btn; //버튼을 저장할 변수
        
        AudioSource sound; //오디오 소스 변수
        public AudioClip death; //하마 데스 사운드 오디오 클립

        public static int hippoDie; //잡은 하마 수

        void OnEnable()
        {
            anim = GetComponent<Animator>(); //하마 애니메이터 컴포넌트
            btn = GetComponentInChildren<Button>(); //자식에 있는 버튼 컴포넌트
            sound = GetComponent<AudioSource>(); //오디오 소스 컴포넌트       
        }

        private void Update()
        {
            if (GameManager.time > 40) //타임이 40보다 크다면
                anim.speed = 1; //애니메이션 속도 1
            else if (GameManager.time <= 40 && GameManager.time > 20) //타임이 40이하고 20보다 크면
                anim.speed = 2; //애니메이션 속도 2
            else if (GameManager.time <= 20 && GameManager.time > 0) //타임이 20이하고 0보다 크면
                anim.speed = 3; //애니메이션 속도 3
        }

        void Death() //하마 삭제 함수
        {
            Destroy(gameObject); //나 자신 제거
        }

        public void HippoDeath() //하마 데스 애니메이션 함수
        {
            sound.PlayOneShot(death); //하마 데스 사운드 재생
            anim.SetTrigger("Death"); //하마 데스 애니메이션 실행
            HippoSpawner.hippoCount--; //하마 현재 수 감소
            hippoDie++; //잡은 하마 수 증가
            btn.gameObject.SetActive(false); //버튼 비활성화
        }

        void BtnFalse() //버튼 비활성화 함수
        {
            btn.gameObject.SetActive(false); //버튼 비활성화
        }

        void BtnTrue() //버튼 활성화 함수
        {
            btn.gameObject.SetActive(true); //버튼 활성화
        }
    }
}
