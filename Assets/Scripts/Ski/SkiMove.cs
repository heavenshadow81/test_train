using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiMove : MonoBehaviour
{
    Vector3 left = new Vector3 (-6.5f, 3.0f, 0); //왼쪽 위치 값
    Vector3 center = new Vector3 (-6.5f, 0f, 0); //가운데 위치 값
    Vector3 right = new Vector3 (-6.5f, -3.0f, 0); //오른쪽 위치 값
    [SerializeField] float speed = 10f; //기본 스피드 10

    bool leftOn; //왼쪽 버튼 확인 값
    bool centerOn; //가운데 버튼 확인 값
    bool rightOn; //오른쪽 버튼 확인 값

    bool Rhandle; //오른쪽 회전 값
    bool Lhandle; //왼쪽 회전 값

    public static bool gameStart; //게임시작 확인 값

    private void Update()
    {
        if (!leftOn&&!centerOn&&!rightOn&&gameStart) //게임스타트가 활성화 중이고 아무 버튼도 안눌렸다면
            Shake(); //쉐이크 함수 활성화

        if (leftOn) //왼쪽 버튼이 눌렸다면
        {
            transform.position = Vector3.MoveTowards(transform.position, left, Time.deltaTime * speed); //왼쪽 위치로 이동
            gameObject.transform.eulerAngles = new Vector3(120, -90, 90); //로테이션 값을 왼쪽으로 회전
            Rhandle = true; //오른쪽 회전 확인 값 활성화
            Lhandle = false; //왼쪽 버튼 확인 값 비활성화

            if (transform.position == left) //플레이어 위치가 left에 도착했다면
            {
                Shake(); //쉐이크 함수 활성화
            }
        }

        else if (centerOn) //센터 버튼이 눌렸다면
        {
            if (Rhandle) //오른쪽 회전 값이 활성화 라면
            {
                gameObject.transform.eulerAngles = new Vector3(70, -90, 90); //오른쪽으로 횐전
                transform.position = Vector3.MoveTowards(transform.position, center, Time.deltaTime * speed); //센터 위치로 이동
                if (transform.position == center) //센터 위치에 도착했다면
                {
                    Shake(); //쉐이크 함수 활성화
                }
            }

            if (Lhandle) //왼쪽 회전 값이 활성화 라면
            {
                gameObject.transform.eulerAngles = new Vector3(120, -90, 90); //왼쪽으로 회전
                transform.position = Vector3.MoveTowards(transform.position, center, Time.deltaTime * speed); //센터 위치로 이동

                if (transform.position == center) //센터 위치에 도착했다면
                {
                    Shake(); //쉐이크 함수 활성화
                }
            }
        }
        else if (rightOn) //오른쪽 버튼을 눌렀다면
        {
            transform.position = Vector3.MoveTowards(transform.position, right, Time.deltaTime * speed); //오른쪽 위치로 이동
            gameObject.transform.eulerAngles = new Vector3(70, -90, 90); //오른쪽으로 회전
            Rhandle = false; //오른쪽 회전 비활성화
            Lhandle = true; //왼쪽 회전 비활성화

            if (transform.position == right) //오른쪽 위치에 도착했다면
            { 
                Shake(); //쉐이크 함수 실행
            }
        }

        if(!gameStart) //게임스타트가 비활성화 라면
            gameObject.transform.eulerAngles = new Vector3(90, -90, 90); //회전값 정면
    }
    public void LeftBtn() //왼쪽버튼 함수
    {
        leftOn = true; //왼쪽 확인값 활성화
        centerOn = false; //센터 확인값 비활성화
        rightOn = false; //오른쪽 확인값 비활성화
        GameObject.Find("SoundManager").GetComponent<SkiSound>().BreakSound(); //브레이크 사운드 재생
    }
    public void CenterBtn() //센터버튼 함수
    {
        centerOn = true; //센터 확인값 활성화
        leftOn = false; //왼쪽 확인값 비활성화
        rightOn = false; //오른쪽 확인값 비활성화
        GameObject.Find("SoundManager").GetComponent<SkiSound>().BreakSound(); //브레이크 사운드 재생
    }
    public void RightBtn() //오른쪽버튼 함수
    {
        rightOn = true; //오른쪽 확인값 활성화
        centerOn = false; //가운데 확인값 비활성화
        leftOn = false; //왼쪽 확인값 비활성화
        GameObject.Find("SoundManager").GetComponent<SkiSound>().BreakSound(); //브레이크 사운드 재생
    }

    public void Shake() //쉐이크 함수
    {
        gameObject.transform.eulerAngles = new Vector3(Random.Range(89.5f,90.5f), -90, 90); //rotation x 값을 89.5~90.5로 랜덤하게 지정
    }
   
}

   
