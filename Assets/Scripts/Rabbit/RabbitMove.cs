using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rabbit
{
    public class RabbitMove : MonoBehaviour
    {
        Vector3 One =   new Vector3(0.8f,  4.2f, 0); //1번 위치값 저장
        Vector3 Two =   new Vector3(0.8f,  2.2f, 0); //2번 위치값 저장
        Vector3 Three = new Vector3(0.8f,  0.1f, 0); //3번 위치값 저장
        Vector3 Four =  new Vector3(0.8f, -2.2f, 0); //4번 위치값 저장
        Vector3 Five =  new Vector3(0.8f, -4.2f, 0); //5번 위치값 저장

        float speed = 10f; //속도 10으로

        Animator anim; //애니메이터 변수

        public static bool gameStart; //게임스타트 값

        private void OnEnable()
        {
            anim = GetComponent<Animator>(); //애니메이터 컴포넌트
        }
        private void Update()
        {
            if (gameStart) //게임스타트가 true라면
            {

                if (FootManager.one) //1번 발판이 true라면
                {
                    transform.position = Vector3.MoveTowards(transform.position, One, Time.deltaTime * speed); //1번 위치로 이동
                    if (transform.position.y <= 1.2f) //현재 위치의 y값이 1.2이하면
                        RBMove(); //무브 애니메이션 실행
                }
                else if (FootManager.two) //2번 발판이 true라면
                {
                    transform.position = Vector3.MoveTowards(transform.position, Two, Time.deltaTime * speed); //2번 위치로 이동
                    if (transform.position.y <= -1.2f) //현재 위치의 y값이 *1.2이하면
                        RBMove(); //무브 애니메이션 실행
                }
                else if (FootManager.three) //3번 발판이 true라면
                {
                    transform.position = Vector3.MoveTowards(transform.position, Three, Time.deltaTime * speed); //3번 위치로 이동
                }
                else if (FootManager.four) //4번 발판이 true라면
                {
                    transform.position = Vector3.MoveTowards(transform.position, Four, Time.deltaTime * speed); //4번 위치로 이동
                    if (transform.position.y >= 1.0f) //현재 위치의 y값이 1이상 이면
                        RBMove(); //무브 애니메이션 실행
                }
                else if (FootManager.five) //5번 발판이 true라면
                {
                    transform.position = Vector3.MoveTowards(transform.position, Five, Time.deltaTime * speed); //5번 위치로 이동
                    if (transform.position.y >= 1.0f) //현재 위치의 y값이 1이상 이면
                        RBMove(); //무브 애니메이션 실행
                }
            }

        }

        public void Carrot() //당근 함수
        {
            anim.SetTrigger("Carrot"); //당근 애니메이션 실행
            GameObject.Find("SoundManager").GetComponent<RabbitSound>().CollectSound(); //콜렉트 사운드 함수 실행
        }

        public void RBMove() //무브 함수
        {
            anim.SetTrigger("RabbitMove"); //무브 함수 
        }

        public void walkSound() //워크 사운드 함수
        {
            GameObject.Find("SoundManager").GetComponent<RabbitSound>().WalkSound(); //워크 사운드 함수 실행
        }
    }
}

   
