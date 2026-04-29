using Hippo;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

namespace Crab
{
    public class CrabMove : MonoBehaviour/*, IPointerDownHandler*/
    {
        Animator anim; //애니메이터를 저장할 변수

        AudioSource sound; //오디오 소스 변수
        public AudioClip death; //크랩 데스 사운드 오디오 클립
        public AudioClip home; //크랩 홈 사운드 오디오 클립

        public static int CrabDie; //잡은 크랩 수
        public bool die;

        void OnEnable()
        {
            anim = GetComponent<Animator>(); //크랩 애니메이터 컴포넌트
            sound = GetComponent<AudioSource>(); //오디오 소스 컴포넌트
            die = false; //크랩 다이 값을 false로
        }

        public void UpdateLogic()
        {
            float speed = 3f; //이동속도 3

            if (!die) //다이값이 false라면
            {
                if (GameManager.time > 40) //타임이 40보다 크다면
                {
                    //crab의 위치를 벡터 0으로
                    transform.position = Vector3.MoveTowards(transform.localPosition, new Vector3(0,1,0), speed * Time.deltaTime);
                }
                else if (GameManager.time <= 40 && GameManager.time > 20) //타임이 40이하고 20보다 크면
                {
                    speed = 4f; //이동속도 4

                    //crab의 위치를 벡터 0으로
                    transform.position = Vector3.MoveTowards(transform.localPosition, new Vector3(0, 1, 0), speed * Time.deltaTime);
                }
                else if (GameManager.time <= 20 && GameManager.time > 0) //타임이 20이하고 0보다 크면
                {
                    speed = 5f; //이동속도 5

                    //crab의 위치를 벡터 0으로
                    transform.position = Vector3.MoveTowards(transform.localPosition, new Vector3(0, 1, 0), speed * Time.deltaTime);
                }
            }

            if (GameManager.time <= 0) //타임이 0이하라면
            {
                anim.SetTrigger("Death"); //크랩 데스 애니메이션 실행
                die = true; //크랩 다이 값을 true로
            }
        }

        private void Update()
        {
            UpdateLogic();
        }


        void Death() //크랩 삭제 함수
        {
            Destroy(gameObject); //나 자신 제거
        }

        /*public void OnPointerDown(PointerEventData eventData) //터치했을 때
        {
            if (!die)
            {
               // GameManager.CrabDieEvent?.Invoke(this);
                sound.PlayOneShot(death); //크랩 데스 사운드 재생
                anim.SetTrigger("Death"); //크랩 데스 애니메이션 실행

                CrabSpawner.crabCount--; //크랩 현재 수 감소
                CrabDie++; //잡은 크랩 수 증가
                die = true; //크랩 다이 값을 true로
            }
        }*/

        public void ClickEvent() //터치했을 때
        {
            if (!die)
            {
                // GameManager.CrabDieEvent?.Invoke(this);
                sound.PlayOneShot(death); //크랩 데스 사운드 재생
                anim.SetTrigger("Death"); //크랩 데스 애니메이션 실행

                CrabSpawner.crabCount--; //크랩 현재 수 감소
                CrabDie++; //잡은 크랩 수 증가
                die = true; //크랩 다이 값을 true로
            }
        }

        private void OnTriggerEnter2D(Collider2D other) //other와 부딪혔을 때
        {
            if(other.CompareTag("Ground")) //other의 태그가 Ground라면
            {
                StartCoroutine(CrabHome()); //크랩홈 코루틴 실행
            }
        }

        IEnumerator CrabHome() //크랩홈 코루틴
        {
            CrabSpawner.crabCount--; //크랩 현재 수 감소
            sound.PlayOneShot(home); //크랩 홈 사운드 재생
           // GameManager.CrabDieEvent?.Invoke(this);
            yield return new WaitForSeconds(0.3f);

            Destroy(gameObject); //나 자신 삭제
        }
    }
}
