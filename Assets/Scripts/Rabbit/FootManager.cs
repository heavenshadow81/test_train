using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rabbit
{
    public class FootManager : MonoBehaviour
    {
        Animator animOne; //1번 발판 애니메이터
        Animator animTwo; //2번 발판 애니메이터
        Animator animThree; //3번 발판 애니메이터
        Animator animFour; //4번 발판 애니메이터
        Animator animFive; //5번 발판 애니메이터

        public static bool one; //1번 발판 상태값
        public static bool two; //2번 발판 상태값
        public static bool three; //3번 발판 상태값
        public static bool four; //4번 발판 상태값
        public static bool five; //5번 발판 상태값

        private void OnEnable()
        {
            animOne = GameObject.Find("greenfootL").GetComponent<Animator>(); //1번 발판 애니메이터 컴포넌트
            animTwo = GameObject.Find("yellowfootL").GetComponent<Animator>(); //2번 발판 애니메이터 컴포넌트
            animThree = GameObject.Find("pinkfoot").GetComponent<Animator>(); //3번 발판 애니메이터 컴포넌트
            animFour = GameObject.Find("yellowfootR").GetComponent<Animator>(); //4번 발판 애니메이터 컴포넌트
            animFive = GameObject.Find("greenfootR").GetComponent<Animator>(); //5번 발판 애니메이터 컴포넌트

            one = false; two = false; three = false; four = false; five = false; //시작할 때 발판 모두 비활성화
        }


        public void greenfootLBtn() //1번 발판 버튼 함수
        {
            GameObject.Find("Player").GetComponent<RabbitMove>().RBMove(); //래빗 무브 함수 실행
            animOne.SetTrigger("greenfootLPush"); //1번 발판 애니메이션 실행

            one = true; //1번 발판 상태값 true
            two = false; //2번 발판 상태값 false
            three = false; //3번 발판 상태값 false
            four = false; //4번 발판 상태값 false
            five = false; //5번 발판 상태값 false
        }
        public void yellowfootLBtn() //2번 발판 버튼 함수
        {
            GameObject.Find("Player").GetComponent<RabbitMove>().RBMove(); //래빗 무브 함수 실행
            animTwo.SetTrigger("yellowfootLPush"); //2번 발판 애니메이션 실행

            one = false; //1번 발판 상태값 false
            two = true; //2번 발판 상태값 true
            three = false; //3번 발판 상태값 false
            four = false; //4번 발판 상태값 false
            five = false; //5번 발판 상태값 false
        }

        public void pinkfootBtn() //3번 발판 버튼 함수
        {   
            GameObject.Find("Player").GetComponent<RabbitMove>().RBMove(); //래빗 무브 함수 실행
            animThree.SetTrigger("pinkfootPush"); //3번 발판 애니메이션 실행

            one = false; //1번 발판 상태값 false
            two = false; //2번 발판 상태값 false
            three = true; //3번 발판 상태값 true
            four = false; //4번 발판 상태값 false
            five = false; //5번 발판 상태값 false
        }

        public void yellowfootRBtn() //4번 발판 버튼 함수
        {
            GameObject.Find("Player").GetComponent<RabbitMove>().RBMove(); //래빗 무브 함수 실행
            animFour.SetTrigger("yellowfootRPush"); //4번 발판 애니메이션 실행

            one = false; //1번 발판 상태값 false
            two = false; //2번 발판 상태값 false
            three = false; //3번 발판 상태값 false
            four = true; //4번 발판 상태값 true
            five = false; //5번 발판 상태값 false
        }

        public void greenfootRBtn() //5번 발판 버튼 함수
        {
            GameObject.Find("Player").GetComponent<RabbitMove>().RBMove(); //래빗 무브 함수 실행
            animFive.SetTrigger("greenfootRPush"); //5번 발판 애니메이션 실행

            one = false; //1번 발판 상태값 false
            two = false; //2번 발판 상태값 false
            three = false; //3번 발판 상태값 false
            four = false; //4번 발판 상태값 false
            five = true; //5번 발판 상태값 true
        }
    }
}