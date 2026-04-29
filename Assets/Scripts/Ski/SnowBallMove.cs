using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class SnowBallMove : MonoBehaviour
{
    public GameObject Bombeffect; //부딪혔을 때 소환 될 이펙트
    public static bool GameOver; //게임오버 확인값
    float speed; //속도

    void Update()
    {
        if (!SGameManager.stageOne &&!SGameManager.stageTwo && !SGameManager.stageThree) //스테이지가 모두 비활성화 라면
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime); //앞으로 이동
            Shake(); //쉐이크 함수 활성화
            speed = 10; //속도 10
        }
        else if (SGameManager.stageOne) //스테이지 1이 활성화 되어있다면
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime); //앞으로 이동
            Shake(); //쉐이크 함수 활성화
            speed = 12; //속도 12
        }
        else if (SGameManager.stageTwo) //스테이지 2가 활성화 되어있다면
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime); //앞으로 이동
            Shake(); //쉐이크 함수 활성화
            speed = 15; //속도 15
        }
        else if (SGameManager.stageThree) //스테이지 3이 활성화 되어있다면
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime); //앞으로 이동
            Shake(); //쉐이크 함수 활성화
            speed = 18; //속도 18
        }
    }

    void OnTriggerEnter(Collider other) //other에 부딪혔을 때
    { 
        if (other.gameObject.tag == "Player") //other의 태그가 Player라면

        {
            Destroy(other.gameObject); //other 파괴
            Instantiate(Bombeffect, transform.position, transform.rotation); //부딪힌 위치에 이펙트 생성
            GameObject.Find("SoundManager").GetComponent<SkiSound>().BoomSound(); //붐 사운드 재생

        }
        Destroy(gameObject); //나 자신 파괴
        GameOver = true; //게임오버 활성화
        SkiMove.gameStart = false; //게임스타트 비활성화
    }

    public void Shake() //쉐이크 함수
    {
        gameObject.transform.eulerAngles = new Vector3(Random.Range(0.5f, -0.5f), 0, 0); //rotation x값을 -0.5~0.5로 랜덤하게 지정
    }
}

