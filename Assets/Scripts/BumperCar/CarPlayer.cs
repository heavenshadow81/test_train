using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BumperCar
{
    public class PlayerMove : MonoBehaviour
    {
        Vector3 left = new Vector3(-6.5f, 3.0f, 0); //플레이어 자동차 왼쪽 위치값 저장
        Vector3 center = new Vector3(-6.5f, 0f, 0); //플레이어 자동차 가운데 위치값 저장
        Vector3 right = new Vector3(-6.5f, -3.0f, 0); //플레이어 자동차 오른쪽 위치값 저장
        [SerializeField] float speed = 10f; //플레이어 자동차 속도

        bool leftOn; //왼쪽 버튼 값
        bool centerOn; //가운데 버튼 값
        bool rightOn; //오른쪽 버튼 값

        bool Rhandle; //오른쪽 회전
        bool Lhandle; //왼쪽 회전

        public static bool gameStart; //게임스타트 값

        private void Update()
        {
            if (!leftOn && !centerOn && !rightOn && gameStart) //모든 버튼 값이 false이고 게임스타트가 true일 때
                Shake(); //쉐이크 함수 사용

            if (leftOn) //왼쪽 버튼이 true일 때
            {
                transform.position = Vector3.MoveTowards(transform.position, left, Time.deltaTime * speed); //플레이어 자동차를 왼쪽으로 보냄
                gameObject.transform.eulerAngles = new Vector3(120, -90, 90); //자동차를 왼쪽으로 회전시킴
                Rhandle = true; //오른쪽 회전값을 true로 
                Lhandle = false; //왼쪽 회전값을 false로

                if (transform.position == left) //플레이어 자동차의 위치가 왼쪽에 도착하면
                {
                    Shake(); //쉐이크 함수 사용
                }
            }

            else if (centerOn) //센터 버튼이 true일 때
            {
                if (Rhandle)//오른쪽 회전값이 true라면
                {
                    transform.position = Vector3.MoveTowards(transform.position, center, Time.deltaTime * speed); //플레이어 자동차를 가운데로 보냄
                    gameObject.transform.eulerAngles = new Vector3(70, -90, 90); //플레이어 자동차를 오른쪽으로 회전시킴

                    if (transform.position == center) //플레이어 자동차의 위치가 가운데 도착하면
                    {
                        Shake(); //쉐이크 함수 사용
                    }
                }

                if (Lhandle) //왼쪽 회전값이 true라면
                {
                    transform.position = Vector3.MoveTowards(transform.position, center, Time.deltaTime * speed); //플레이어 자동차를 가운데로 보냄
                    gameObject.transform.eulerAngles = new Vector3(120, -90, 90); //플레이어 자동차를 왼쪽으로 회전시킴

                    if (transform.position == center) //플레이어 자동차의 위치가 가운데 도착하면
                    {
                        Shake(); //쉐이크 함수 사용
                    }
                }
            }
            else if (rightOn) //오른쪽 버튼이 true일 때
            {
                transform.position = Vector3.MoveTowards(transform.position, right, Time.deltaTime * speed); //플레이어 자동차를 오른쪽으로 보냄
                gameObject.transform.eulerAngles = new Vector3(70, -90, 90); //플레이어 자동차를 왼쪽으로 회전시킴
                Rhandle = false; //오른쪽 회전값 false로
                Lhandle = true; //왼쪽 회전값 true로

                if (transform.position == right) //플레이어 자동차가 오른쪽에 도착했다면
                {
                    Shake(); //쉐이크 함수 사용
                }
            }

            if (!gameStart) //게임스타트가 false라면
                gameObject.transform.eulerAngles = new Vector3(90, -90, 90); //자동차를 회전시키지 않음


        }

        public void LeftBtn() //왼쪽 버튼을 눌렀을 때
        {
            leftOn = true; //왼쪽 버튼 값 true
            centerOn = false; //가운데 버튼 값 false
            rightOn = false; //오른쪽 버튼 값 false
            GameObject.Find("SoundManager").GetComponent<CarSound>().BreakSound(); //브레이크 사운드 재생
        }
        public void CenterBtn() //가운데 버튼을 눌렀을 때
        {
            centerOn = true; //가운데 버튼 값 true
            leftOn = false; //왼쪽 버튼 값 false
            rightOn = false; //오른쪽 버튼 값 false
            GameObject.Find("SoundManager").GetComponent<CarSound>().BreakSound(); //브레이크 사운드 재생
        }
        public void RightBtn() //오른쪽 버튼을 눌렀을 때
        {
            rightOn = true; //오른쪽 버튼 값 true
            centerOn = false; //가운데 버튼 값 false
            leftOn = false; //왼쪽 버튼 값 false
            GameObject.Find("SoundManager").GetComponent<CarSound>().BreakSound(); //브레이크 사운드 재생
        }

        public void Shake() //쉐이크 함수
        {
            gameObject.transform.eulerAngles = new Vector3(Random.Range(89.5f, 90.5f), -90, 90); //플레이어 자동차의 x값을 89.5f~90.5f 로 랜덤 지정
        }
    }
}

   
