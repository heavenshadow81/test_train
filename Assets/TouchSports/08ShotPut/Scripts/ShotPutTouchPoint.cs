using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.T_Sports.Common;
namespace ML.T_Sports.ShotPut
{
    public class ShotPutTouchPoint : MonoBehaviour
    {
        public int Player;
        public Camera CameraPlayerA;
        public ShotPutCamera ShotPutCameraA;
        public Transform PlayerATr;
        public Transform TargetTrA;
        public float DistanceA;
        public float CoolA;

        public Camera CameraPlayerB;
        public ShotPutCamera ShotPutCameraB;
        public Transform PlayerBTr;
        public Transform TargetTrB;
        public float DistanceB;
        public float CoolB;

        public Camera CameraPlayerC;
        public ShotPutCamera ShotPutCameraC;
        public Transform PlayerCTr;
        public Transform TargetTrC;
        public float DistanceC;
        public float CoolC;


        public float power;
        public GameObject ShotPutBall;

        public float balltime;
        public float ballheight;

        public EFMPlayer Throwing;
        public Transform BallOP;
        
        void Update()
        {
            if (CoolA > 0)
                CoolA -= Time.deltaTime;
            else if (CoolA <= 0)
                CoolA = 0;

            if (CoolB > 0)
                CoolB -= Time.deltaTime;
            else if (CoolB <= 0)
                CoolB = 0;

            if (CoolC > 0)
                CoolC -= Time.deltaTime;
            else if (CoolC <= 0)
                CoolC = 0;

            int TouchCount = TouchModule.TouchModuleInput.touchCount;
            if (TouchCount != 0)
            {
                Touch[] touchs = TouchModule.TouchModuleInput.touches;
                for (int i = 0; i < touchs.Length; i++)
                {
                    if (touchs[i].phase != TouchPhase.Began) continue;
                    Vector3 pos = touchs[i].position;
                    float areaA = 1920 / Player;
                    float areaB = areaA * 2;
                    float areaC = 1920;

                    if (pos.x < areaA && CoolA <= 0)
                    {
                        if (!ShotPutManager.instance.CheckScoreState(0))
                        {
                            //Debug.Log("플레이어A 는 FINISH 상태입니다.");
                            return;
                        }
                        CoolA = 3;
                        ShotPutManager.instance.SetPlayerAngle(0, false);
                        ShotBall(CameraPlayerA, ShotPutCameraA, PlayerATr, TargetTrA, pos, DistanceA,0);
                    }
                    else if (pos.x >= areaA && pos.x < areaB && CoolB <= 0)
                    {
                        if (!ShotPutManager.instance.CheckScoreState(1))
                        {
                            //Debug.Log("플레이어B 는 FINISH 상태입니다.");
                            return;
                        }
                        CoolB = 3;
                        ShotPutManager.instance.SetPlayerAngle(1, false);
                        ShotBall(CameraPlayerB, ShotPutCameraB, PlayerBTr, TargetTrB, pos, DistanceB,1);
                    }
                    else if (pos.x >= areaB && pos.x < areaC && CoolC <= 0)
                    {
                        if (!ShotPutManager.instance.CheckScoreState(2))
                        {
                            //Debug.Log("플레이어C 는 FINISH 상태입니다.");
                            return;
                        }
                        CoolC = 3;
                        ShotPutManager.instance.SetPlayerAngle(2, false);
                        ShotBall(CameraPlayerC, ShotPutCameraC, PlayerCTr, TargetTrC, pos, DistanceC,2);
                    }

                }
            }
        }
        public bool OutCheck(float distance, Vector3 pos)
        {
            float Distmax = 30f;
            float Distmin = 6.85f;
            
            //Debug.Log("거리 " + distance);
            //최소 거리에 미치지 못하면 False
            if (distance < Distmin)
            {
                //Debug.Log("최소거리 부족 아웃");
                return false;
            }
            //플레이어가 2명 이상인경우 좌우 아웃이 없음
            if (Player >= 2)
                return true;
            distance -= Distmin;

            float distancePer = distance / (Distmax - Distmin);
            //Debug.Log("거리 비율 " + distancePer);
            float minx_per = (0.9f * distancePer * 100)+399;
            //Debug.Log("minx 비율 " + minx_per);
            if (pos.x < minx_per)
            {
                //Debug.Log("왼쪽 아웃");
                return false;
            }

            float maxx_per = 1541 - (1 * distancePer * 100);
            //Debug.Log("maxx 비율 " + maxx_per);
            if (pos.x > maxx_per)
            {
                //Debug.Log("오른쪽 아웃");
                return false;
            }
            return true;            
        }
        public void ShotBall(Camera PlayerCamera, ShotPutCamera shotputcam, Transform PlayerTr, Transform TargetTr, Vector3 TouchPos, float distance, int idx)
        {
            Ray ray = PlayerCamera.ScreenPointToRay(TouchPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform.name == "TouchArea")
                {
                    PlayerTr.LookAt(hit.point);
                    distance = 8.4f;
                    //0도 이하의 경우
                    if (TouchPos.y > 0 && TouchPos.y <= 645)
                    {
                        distance += TouchPos.y * 0.009457f;
                    }
                    else if (TouchPos.y > 645 && TouchPos.y <= 837)
                    {
                        TouchPos.y -= 645;
                        distance += TouchPos.y * 0.236979f;
                    }
                    else if (TouchPos.y > 837 && TouchPos.y <= 1080)
                    {
                        TouchPos.y -= 837;
                        //Distance += touch_y * -0.187242f;
                        distance = 60 + (TouchPos.y * -0.187242f);
                    }
                    TargetTr.position = SetTargetPos(PlayerTr, distance, idx);
                    Vector3 tmp = TargetTr.position;
                    tmp.y = 0.2f;
                    TargetTr.position = tmp;
                    ShotPutBall ball = Instantiate(ShotPutBall, PlayerTr.position, PlayerTr.rotation).GetComponent<ShotPutBall>();
                    ball.transform.parent = BallOP;
                    float returntime = distance / 18f;

                    if (returntime < 2f)
                        returntime = 2;

                    //공의 최대높이 계산 
                    ballheight = distance / 6;
                    if (ballheight < PlayerTr.position.y)
                        ballheight = PlayerTr.position.y + 1;

                    //공의 채공 시간 계산
                    balltime = 1f / 60f * distance;
                    if (balltime < 0.2f)
                        balltime = 0.2f;

                    float dist_m = distance / 2;

                    bool outcheck = OutCheck(dist_m, TouchPos);
                    ball.SetBallShot(TargetTr.position, ballheight, balltime, dist_m, idx, outcheck);
                    shotputcam.CameraGo(ball.MyTr, returntime);
                    Throwing.EFMRandomPlay();
                    //ball.ShotBall(power);
                }
            }
        }
        public Vector3 SetTargetPos(Transform PlayerTr, float Distance, int idx)
        {

            float angle = PlayerTr.rotation.eulerAngles.y;           

            Vector3 v3Source = PlayerTr.position;    // 중심이 되는 오브젝트 
            Vector3 v3Distance = Vector3.forward * Distance;    // 거리벡터(forward는 Y축 기준으로 앞을 보고있는 벡터. 필요에따라 수정 필요) 
            Quaternion qRotate = Quaternion.Euler(0f, angle, 0f);  // 회전할 각도(Y축 기준 회전. 필요에따라 수정 필요) 
            Vector3 v3TargetPoint = qRotate * v3Distance;    // 원점을 기준으로 거리와 각도를 연산한 후, 벡터 
            Vector3 v3Dest = v3Source + v3TargetPoint;    // 중심이 되는 오브젝트에서 해당 거리와 각도만큼 이동한 곳의 좌표. 

            return v3Dest;
        }

        public void SetTouchPlayer(int PlayerValues)
        {
            Player = PlayerValues;
        }
        

             
    }
}
