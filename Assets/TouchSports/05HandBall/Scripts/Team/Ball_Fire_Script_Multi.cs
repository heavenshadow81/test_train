using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.T_Sports.Common;
namespace ML.T_Sports.HandBall
{
    public class Ball_Fire_Script_Multi : MonoBehaviour
    {
        //**<summary> 클릭 및 입력이 있을시 해당 지점에 공을 발사 하는 스크립트, (역탄환공식 적용X) 힘의 유사값을 정한뒤 발사.
        //** 지점에 대한 골키퍼의 애니메이션 관장.
        public EFMPlayer Source;

        public Camera cam;
        public Transform origin;
        public GameObject BallObj;
        public Handball_Ctrl_Multi ballpre;

        public Game_Manager_Multi manager;

        public float distance;
        public float value_tmp;
        public float var_2;
        public float power;

        public float xposition;
        public float yposition;

        public int key;

        public Animation keeper;
        

        // Update is called once per frame
        void Update()
        {
            // 골키퍼 애니메이션이 종료 되는 경우 바로 Idle 애니메이션을 플레이 / 게임스테이트 상태에 따라 변경

            if (!keeper.isPlaying)
            {
                keeper.Play("Goal_Keeper_Idle");
                if (manager.gamestate == GameState.End || manager.gamestate == GameState.End001)
                {
                    manager.gamestate = GameState.End001;
                }
                else if (manager.gamestate == GameState.Shooting && manager.compare == true)
                {
                    manager.gamestate = GameState.Ready;
                    manager.compare = false;
                }

            }
            //게임 스테이트가 레디 상태일때 화면 입력을 받으며, 입력은 마우스 클릭으로도 대체가 가능하다.

            if (manager.gamestate == GameState.Ready)
            {
                if (TouchModule.TouchModuleInput.touchCount > 0 && TouchModule.TouchModuleInput.GetTouch(0).phase == TouchPhase.Began)
                {
                    Touch t = TouchModule.TouchModuleInput.GetTouch(0);
                    Ray ray = cam.ScreenPointToRay(t.position);
                    xposition = t.position.x;
                    yposition = t.position.y;

                    RaycastHit hit;
                    //레이케스트를 무시 하는 부분이 필요하여 레이어 마스크 적용

                    int mask = 1 << 9;
                    mask = ~mask;

                    Source.EFMRandomPlay();
                    //타 화면 클릭시 다른 화면의 입력을 받는 경우 때문에 예외처리

                    if (xposition > 0 && xposition < 1920)
                    {


                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
                        {
                            Vector3 objectHit = hit.point;

                            Debug.DrawRay(ray.origin, objectHit, Color.green, 1f);
                            Debug.Log(ray);

                            Transform ball = Instantiate(BallObj, origin.position, Quaternion.identity).transform;

                            Handball_Ctrl_Multi ballPre = ball.GetComponent<Handball_Ctrl_Multi>();

                            ball.rotation = Quaternion.LookRotation(objectHit - origin.transform.position);

                            Vector3 Rot = ball.transform.rotation.eulerAngles;
                            Rot.x -= 4;
                            ball.rotation = Quaternion.Euler(Rot);
                            //화면의 아래 부분 클릭시 고정된 파워로 공이 발사됨.

                            if (yposition < 150)
                            {
                                power = -1000;
                            }
                            else
                            {
                                distance = Vector3.Distance(ball.transform.position, objectHit);

                                float var_1 = 10 - distance;
                                value_tmp = var_1 * var_2;

                                power = 1 * distance + value_tmp;

                            }

                            ball.GetComponent<Rigidbody>().AddForce(ball.forward * -power);

                            ballPre.StartCoroutine("StartRotation");

                            Debug.Log("Ball Spawned");
                            keeperAction();
                            manager.gamestate = GameState.Shooting;
                        }

                    }
                }
            }
        }
        // 골키퍼의 애니메이션 키 값을 각 화면의 지점 좌표로 받으며, 해당 좌표를 클릭시 Switch Case 문으로 각각의 애니메이션 적용.
#region KeeperAnimation
        public void keeperAction()
        {

           // key = (int)Random.Range(0.0f, 5.0f);

          
            if (xposition < 750)
            {
                key = 4;
            }
            else if (xposition > 750 && xposition < 850)
            {
                key = 2;
            }
            else if (xposition > 850 && xposition < 1070)
            {
                key = 5;
            }
            else if (xposition > 1070 && xposition < 1170)
            {
                key = 1;
            }
            else if (xposition > 1170)
            {
                key = 3;
            }

            switch (key)
            {
                case 1:
                    keeper["handball_R_down_block"].speed = 0.65f;

                    keeper.Play("handball_R_down_block");

                    break;
                case 2:
                    keeper["handball_L_down_block"].speed = 0.65f;

                    keeper.Play("handball_L_down_block");
                    break;

                case 3:
                    keeper["handball_R_up_block"].speed = 0.7f;

                    keeper.Play("handball_R_up_block");
                    break;

                case 4:
                    keeper["handball_L_up_block"].speed = 0.7f;

                    keeper.Play("handball_L_up_block");
                    break;

                case 5:

                    keeper.Play("Goal_Keeper_Upper");
                    break;
                default:
                    keeper.Play("Goal_Keeper_Upper");
                    break;

            }

        }

    }
#endregion
}