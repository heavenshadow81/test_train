using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rabbit
{
    public class CarrotMove : MonoBehaviour
    {
        Animator anim; //애니메이터 변수
        public GameObject Bombeffect; //생설할 이펙트 오브젝트
        float speed; //스피드

        private void OnEnable()
        {
            anim = GetComponent<Animator>(); //애니메이터 컴포넌트
        }

        private void Update()
        {
            UpdateLogic();
        }


        public void UpdateLogic()
        {
            if (RabbitMove.gameStart) //게임스타트가 true라면
            {
                if (!RGameManager.stageOne && !RGameManager.stageTwo && !RGameManager.stageThree) //1,2,3 스테이지 모두 false 라면
                {
                    transform.Translate(Vector3.down * speed * Time.deltaTime); //아래로 이동
                    speed = 4; //속도는 4
                }
                else if (RGameManager.stageOne) //1스테이지가 true라면
                {
                    transform.Translate(Vector3.down * speed * Time.deltaTime); //아래로 이동
                    speed = 6; //속도는 6
                }
                else if (RGameManager.stageTwo) //2스테이지가 true라면
                {
                    transform.Translate(Vector3.down * speed * Time.deltaTime); //아래로 이동
                    speed = 7; //속도는 7
                }
                else if (RGameManager.stageThree) //3스테이지가 true라면
                {
                    transform.Translate(Vector3.down * speed * Time.deltaTime); //아래로 이동
                    speed = 8; //속도는 8
                }
            }
        }


      
        void OnTriggerEnter(Collider other) //other와 부딪혔을 때
        {
            if (other.gameObject.tag == "Player") //other의 태그가 Player라면
            {
                Destroy(gameObject); //나 자신을 파괴함
                Instantiate(Bombeffect, transform.position, transform.rotation); //나와 같은 위치에 이펙트 생성
                GameObject.Find("Player").GetComponent<RabbitMove>().Carrot(); //캐롯 사운드 재생
            }
            else if (other.gameObject.tag == "Ground") //other의 태그가 Ground라면
            {
                DragonFruitMove.GameOver = true; //게임오버를 true로
                RabbitMove.gameStart = false; //게임 스타트를 false로
                anim.SetTrigger("CarrotDie"); //캐롯다이 애니메이션 재생
                speed = 0; //스피드 0으로 변경
                GameObject.Find("SoundManager").GetComponent<RabbitSound>().DropSound(); //드롭 사운드 재생
            }
        }
    }
}
