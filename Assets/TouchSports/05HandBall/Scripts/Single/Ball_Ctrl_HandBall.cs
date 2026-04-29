using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.T_Sports.Common;

namespace ML.T_Sports.HandBall
{
    public class Ball_Ctrl_HandBall : MonoBehaviour
    {
        public int BounceCounter;
        HandBall_Manager game_manager;
        // Use this for initialization
        void Start()
        {
            BounceCounter = 1;
            game_manager = FindObjectOfType<HandBall_Manager>();

        }

        void Update()
        {
                        
        }
        public void Orbit_Rotatation()
        {
            transform.Rotate(Vector3.down * 10.0f);
        }

        IEnumerator StartRotation()
        {
            while (true)
            {
                Orbit_Rotatation();
                yield return new WaitForSeconds(0.01f);
            }

        }



        //충돌체의 테그에 따라, 골/노골로 나뉘어 판단. 판단시 게임 메니져의 함수를 호출.

        private void OnTriggerEnter(Collider other)
        {
            StopAllCoroutines();
            if (other.tag == "Goal" && BounceCounter == 1)
            {
                BounceCounter = 0;

                //player.Play();
                Common.SoundManager.instance.Cheers.EFMRandomPlay();

                Invoke("colldisable", 3.0f);

                Debug.Log("Goal!!!");
                game_manager.Invoke("AddPoint", 2.0f);
            }
            else if (other.tag == "NoGoal_Area" && BounceCounter == 1)
            {
                BounceCounter = 0;

                Invoke("colldisable", 3.0f);

                Debug.Log("NoGoal!!");
                game_manager.Invoke("DePoint", 2.0f);

            }

        }
        // 수정. 골키퍼에 공이 닿으면 바로 노골이었으나, 닿고 들어가도 골로 표기 하도록 수정

        private void OnCollisionEnter(Collision collision)
        {
            StopAllCoroutines();

            if (collision.gameObject.tag == "Keeper" && BounceCounter == 1)
            {

                //BounceCounter = 0;
                Debug.Log("Nogoal");
                //game_manager.DePoint();
                GetComponent<Rigidbody>().AddForce(Vector3.back * 700);
                Invoke("colldisable", 3.0f);
            }
        }
        // 공이 필드위에 지저 분하게 있는 것을 방지하기 위해 생성후 일정시간이 지나면 삭제되도록 설정.

        public void colldisable()
        {
            if (BounceCounter == 1)
            {
                game_manager.DePoint();
            }
            GetComponent<SphereCollider>().enabled = false;
            Destroy(gameObject, 1.0f);

        }




    }

}