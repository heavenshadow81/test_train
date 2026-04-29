using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace BumperCar
{
    public class CarMove : MonoBehaviour
    {
        public GameObject Bombeffect; //생성할 이펙트 오브젝트
        public static bool GameOver; //게임오버 값을 스타틱으로 
        float speed; //자동차의 스피드

        void Update()
        {

            if (!BGameManager.one && !BGameManager.two && !BGameManager.last) //현재 바퀴 수가 0바퀴라면
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime); //자동차를 left 방향으로 이동 

                speed = 11; //스피드는 11로
            }
            else if (BGameManager.one) //현재 바퀴 수가 1바퀴라면
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime); //자동차를 left 방향으로 이동 

                speed = 12; //스피드는 12으로
            }
            else if (BGameManager.two) //현재 바퀴 수가 2바퀴라면
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime); //자동차를 left 방향으로 이동 

                speed = 13; //스피드는 13로
            }
            else if (BGameManager.last) //현재 마지막 바퀴라면
            {
                transform.Translate(Vector3.left * speed * Time.deltaTime); //자동차를 left 방향으로 이동 

                speed = 14; //스피드는 14로
            }
        }
        void OnTriggerEnter(Collider other) //트리거가 other와 닿았을 때
        {
            if (other.gameObject.tag == "Player") //만약 other의 태그가 Player라면
            {
                GameObject.Find("SoundManager").GetComponent<CarSound>().BoomSound(); //붐 사운드 재생
                Instantiate(Bombeffect, transform.position, transform.rotation); //붐이펙트 나와 같은 위치에 생성

                Destroy(other.gameObject); //플레이어 삭제
                GameOver = true; //게임오버 값을 true로
                PlayerMove.gameStart = false; //게임스타트 값을 false로

            }
            else if (other.gameObject.tag != "Enemy") //만약 other의 태그가 Enemy가 아니라면
                Destroy(gameObject); //나 자신을 삭제
        }
    }
}

