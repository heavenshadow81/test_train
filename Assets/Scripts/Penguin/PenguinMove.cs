using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Penguin
{
    public class PenguinMove : MonoBehaviour
    {
        public static PenguinMove instance; //펭귄무브 클래스를 싱글톤으로 설정

        public int x = 0; //행 변수
        public int y = 0; //열 변수
        public static Transform position; //플레이어 위치 저장

        Animator anim; //애니메이션
        SpriteRenderer sprite; //스프라이트 이미지

        private void OnEnable()
        {
            sprite = GetComponent<SpriteRenderer>(); //스프라이트 이미지 컴포넌트 
            anim = GetComponent<Animator>(); //애니메이션 컴포넌트

                instance =this; //내자신을 instance로 넣어줍니다.
        }

        private void Update()
        {
            position = gameObject.transform; //플레이어 위치
        }

        //private void OnCollisionEnter(Collision collision) //collision과 닿았을 때
        //{
        //    if (collision.gameObject.tag == "Finish") //collision의 태그가 Finish 라면
        //    {
        //        collision.gameObject.SetActive(false); //부딪힌 collsion 비활성화
        //        anim.SetTrigger("Finish"); //애니메이션 피니쉬 재생

        //        GameManager.gameStart = false; //게임스타트값을 false로
        //    }
        //}

        private void OnTriggerEnter(Collider collision) //collision과 닿았을 때
        {
            if (collision.gameObject.tag == "Finish") //collision의 태그가 Finish 라면
            {
                collision.gameObject.SetActive(false); //부딪힌 collsion 비활성화
                anim.SetTrigger("Finish"); //애니메이션 피니쉬 재생

                GameManager.gameStart = false; //게임스타트값을 false로
            }
        }



        public void Left() //레프트 함수
        {
            anim.SetTrigger("Left"); //애니메이션 레프트 재생
            sprite.flipX = true; //이미지를 X축으로 반전
        }
        public void Right() //라이트 함수
        {
            anim.SetTrigger("Right"); //애니메이션 라이트 재생
            sprite.flipX = false; //이미지 X축 반전을 비활성화
        }
        public void Up() //업 함수
        {
            anim.SetTrigger("Up"); //애니메이션 업 재생
        }
        public void Down() //다운 함수
        {
            anim.SetTrigger("Down"); //애니메이션 다운 재생
        }

        public void Score() //스코어 + 함수
        {
            GameManager.score += 3; //스코어 
        }
    }
}
