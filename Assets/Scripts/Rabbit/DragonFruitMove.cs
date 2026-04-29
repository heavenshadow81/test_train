using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rabbit
{
    public class DragonFruitMove : MonoBehaviour
    {
        public GameObject Bombeffect; //생성할 이펙트 오브젝트
        public static bool GameOver; //게임오버 상태 값
        float speed; //속도
        bool die; //용과 상태 값

        Animator anim; //애니메이터 변수

        SpriteRenderer sprite; //스프라이트 이미지 변수

        // Update is called once per frame
        private void OnEnable()
        {
            anim = GetComponent<Animator>(); //애니메이터 컴포넌트
            sprite = GetComponent<SpriteRenderer>(); //스프라이트 컴포넌트
        }
        void Update()
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("dragonDie")) //드래곤다이 애니메이션 재생중이 아니라면
                die = false; //용과 상태 값 false

            if (!die && !anim.GetCurrentAnimatorStateInfo(0).IsName("dragonDie")) //용과 상태값이 false고 드래곤 다이 애니메이션이 재생중이면
            {

                if (!RGameManager.stageOne && !RGameManager.stageTwo && !RGameManager.stageThree) //스테이지가 모두 false 라면
                {
                    transform.Translate(Vector3.down * speed * Time.deltaTime); //용과 아래 방향으로 움직임
                    speed = 5; //스피드 5
                }
                else if (RGameManager.stageOne) //스테이지1이 true 라면
                {
                    transform.Translate(Vector3.down * speed * Time.deltaTime); //용과 아래 방향으로 움직임
                    speed = 6; //스피드 6
                }
                else if (RGameManager.stageTwo) //스테이지2가 true라면
                {
                    transform.Translate(Vector3.down * speed * Time.deltaTime); //용과 아래 방향으로 움직임
                    speed = 7; //스피드 7
                }
                else if (RGameManager.stageThree) //스테이지 3이 true라면
                {
                    transform.Translate(Vector3.down * speed * Time.deltaTime); //용과 아래 방향으로 움직임
                    speed = 9; //스피드 9
                }
            }
            else if (die) //용과 상태 값이 true라면
            { 
                speed = 0; //스피드는 0으로
            }
        }
        void OnTriggerEnter(Collider other) //other에 부딪혔을 때
        {
            if (other.gameObject.tag == "Player") //other의 태그가 Palyer라면

            {
                Instantiate(Bombeffect, transform.position, transform.rotation); //붐 이펙트 생성
                GameObject.Find("SoundManager").GetComponent<RabbitSound>().DropSound(); //드롭 사운드 재생
                Destroy(other.gameObject);//other 게임오브젝트 제거
                
                GameOver = true; //게임오버를 true로
                RabbitMove.gameStart = false; //게임스타트를 false

                Destroy(gameObject);//나 자신 제거
            }
            else if (other.gameObject.tag == "Ground") //Ground에 부딪혔다면
            {
                die = true; //다이를 true로
                GameObject.Find("SoundManager").GetComponent<RabbitSound>().DropSound(); //드롭 사운드 재생

                anim.SetTrigger("DragonDie"); //드래곤다이 애니메이션 재생
                StartCoroutine(Remove()); //리무브 코루틴 활성화
            }
        }

        IEnumerator Remove() //리무브 코루틴
        {
            yield return new WaitForSeconds(0.3f); //0.3초 뒤에
            Destroy(gameObject); //용과 제거
        }
    }
}

