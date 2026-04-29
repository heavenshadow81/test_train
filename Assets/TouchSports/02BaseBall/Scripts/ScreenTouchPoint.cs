using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.T_Sports.Common;
namespace ML.T_Sports.BaseBall
{
    public class ScreenTouchPoint : MonoBehaviour
    {
        public enum Mod
        {
            Single,
            Team
        }
        public Mod mod;
        public GameObject Shot_Obj;
        public int players;

        public Transform[] CameraTr;
        public Transform TeamCameraTr;
        public float CoolA, CoolB, CoolC;


        public ScoreManager[] Scores;
        public TeamScoreManager MyTeamScoreManager;
        
        float power;
        float distance;
        float value_tmp;
        float val_2;

        public int camnum;
        void Update()
        {
            if (mod == Mod.Single)
            {
                //CheckKeyDown(MyScoreManager);
                if (BaseBallSingleManager.instance.state == BasaeBallState.Play)
                {
                    TouchCheck();
                    if (CoolA > 0)
                        CoolA -= Time.deltaTime;
                    else
                        CoolA = 0;

                    if (CoolB > 0)
                        CoolB -= Time.deltaTime;
                    else
                        CoolB = 0;

                    if (CoolC > 0)
                        CoolC -= Time.deltaTime;
                    else
                        CoolC = 0;
                }
            }
            else if (mod == Mod.Team)
            {
                //CheckKeyDown(MyTeamScoreManager);
                if (BaseBallTeamManager.instance.state == BasaeBallState.Play)
                {
                    TouchCheck(MyTeamScoreManager);
                    if (CoolA > 0)
                        CoolA -= Time.deltaTime;
                    else
                        CoolA = 0;
                }
            }

            
            
        }
        public void TouchCheck()
        {
            int TouchCount = TouchModule.TouchModuleInput.touchCount;
            if (TouchCount != 0)
            {
                Touch[] touchs = TouchModule.TouchModuleInput.touches;
                for (int i = 0; i < touchs.Length; i++)
                {
                    if (touchs[i].phase != TouchPhase.Began) continue;
                    Vector3 pos = touchs[i].position;
                    float areaA = 1920 / players;
                    float areaB = areaA * 2;
                    float areaC = areaA * 3;
                    if (pos.x < areaA && CoolA <= 0)
                    {
                        SetTouchRay(pos, Scores[0], CameraTr[0]);
                        CoolA = 1.5f;                        
                    }
                    else if (pos.x >= areaA && pos.x < areaB && CoolB <= 0)
                    {
                        SetTouchRay(pos, Scores[1], CameraTr[1]);
                        CoolB = 1.5f;
                    }
                    else if (pos.x >= areaB && pos.x < areaC && CoolC <= 0)
                    {
                        SetTouchRay(pos, Scores[2], CameraTr[2]);
                        CoolC = 1.5f;
                    }
                }
            }
        }
        public void SetTouchRay(Vector3 inputPoint, ScoreManager scoremanager, Transform CamTr)
        {
            if (scoremanager.BallChance <= 0)
            {
                Debug.Log("BallChance is 0");
                return;
            }

            Camera RayCamera = CamTr.GetComponent<Camera>();
            Ray ray = RayCamera.ScreenPointToRay(inputPoint);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform.tag == "Ball")
                {
                    return;
                }
                scoremanager.UsingBallCount();
                Transform ballTr = Instantiate(Shot_Obj, CamTr.position, Quaternion.identity).transform;
                Ball ball = ballTr.GetComponent<Ball>();

                //생성된 볼이 타깃(hit.point)를 바라보도록 회전
                ballTr.rotation = Quaternion.LookRotation(hit.point - CamTr.transform.position);
                //회전 x축에 -8 추가(위로 살짝 올림)
                Vector3 rot = ballTr.rotation.eulerAngles;
                rot.x -= 8;
                ballTr.rotation = Quaternion.Euler(rot);

                //볼이 생성된 위치와 타깃지점의 거리 측정
                distance = Vector3.Distance(CamTr.position, hit.point);

                //val_1의 10은 거리 기준점(상수값)
                //거리가 10을 넘는지점은 상탄, 거리가 10이 안되는지점은 하탄 남.
                //거리가 10이 넘는지점은 파워를 감소(마이너스 상수), 10이 안되는지점은 파워를 증가(플러스 사웃)를 추가해야함.
                //예) 스트라이크존 거리 7.5(val_1 = 2.5)
                //예) 뒤쪽 팬스 거리 11(val_1 = -1)
                float val_1 = 10 - distance;

                //파워 증감치(Val_1)에 가변값 상수(val_2)를 곱함.
                value_tmp = val_1 * val_2;

                //파워값은 100 * 거리 + value_tmp;
                power = 110 * distance + value_tmp;

                Rigidbody tmp = ballTr.GetComponent<Rigidbody>();
                tmp.AddForce(ballTr.forward * power);

                ball.SetRandomSpin();
            }
        }

        public void TouchCheck(TeamScoreManager scores)
        {
            int TouchCount = TouchModule.TouchModuleInput.touchCount;
            if (TouchCount != 0)
            {
                Touch[] touchs = TouchModule.TouchModuleInput.touches;
                for (int i = 0; i < touchs.Length; i++)
                {
                    if (touchs[i].phase != TouchPhase.Began) continue;
                    Vector3 pos = touchs[i].position;
                    if (CoolA <= 0)
                    {
                        SetTouchRay(pos, MyTeamScoreManager, TeamCameraTr);
                    }
                        
                }
            }
        }
        public void SetTouchRay(Vector3 inputPoint, TeamScoreManager scoremanager, Transform CamTr)
        {
            if (scoremanager.BallChance <= 0)
            {
                Debug.Log("BallChance is 0");
                return;
            }
            if (!BaseBallTeamManager.instance.Pitching)
            {
                return;
            }
            if (BaseBallTeamManager.instance.state != BasaeBallState.Play)
                return;

            CoolA = 2.3f;
            Camera RayCamera = CamTr.GetComponent<Camera>();
            Ray ray = RayCamera.ScreenPointToRay(inputPoint);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform.tag == "Ball")
                {
                    return;
                }
                scoremanager.UsingBallCount();
                Transform ballTr = Instantiate(Shot_Obj, CamTr.position, Quaternion.identity).transform;
                Ball ball = ballTr.GetComponent<Ball>();

                //생성된 볼이 타깃(hit.point)를 바라보도록 회전
                ballTr.rotation = Quaternion.LookRotation(hit.point - CamTr.transform.position);
                //회전 x축에 -8 추가(위로 살짝 올림)
                Vector3 rot = ballTr.rotation.eulerAngles;
                rot.x -= 8;
                ballTr.rotation = Quaternion.Euler(rot);

                //볼이 생성된 위치와 타깃지점의 거리 측정
                distance = Vector3.Distance(CamTr.position, hit.point);

                //val_1의 10은 거리 기준점(상수값)
                //거리가 10을 넘는지점은 상탄, 거리가 10이 안되는지점은 하탄 남.
                //거리가 10이 넘는지점은 파워를 감소(마이너스 상수), 10이 안되는지점은 파워를 증가(플러스 사웃)를 추가해야함.
                //예) 스트라이크존 거리 7.5(val_1 = 2.5)
                //예) 뒤쪽 팬스 거리 11(val_1 = -1)
                float val_1 = 10 - distance;

                //파워 증감치(Val_1)에 가변값 상수(val_2)를 곱함.
                value_tmp = val_1 * val_2;

                //파워값은 100 * 거리 + value_tmp;
                power = 100 * distance + value_tmp;

                Rigidbody tmp = ballTr.GetComponent<Rigidbody>();
                tmp.AddForce(ballTr.forward * power);

                ball.SetRandomSpin();
            }
        }
        /*
        public void CheckKeyDown(TeamScoreManager scoremanager)
        {
            if (!BaseBallTeamManager.instance.Pitching)
            {
                return;
            }
            if (scoremanager.BallChance > 0 && Input.GetKeyDown(KeyCode.Mouse0))
            {
                BaseBallTeamManager.instance.Pitching = false;
                if (BaseBallTeamManager.instance.state != BasaeBallState.Play)
                    return;
                Vector3 inputPoint = Input.mousePosition;
                float input_x = inputPoint.x;

                float max = camnum * (1920 / players);
                float min = max - (1920 / players);
                //Debug.Log(players);
                //Debug.Log(camnum + "/" + input_x);
                //Debug.Log(min + "/" + max);

                if (input_x > min && input_x < max)
                {
                    Ray ray = RayCamera.ScreenPointToRay(inputPoint);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100f))
                    {
                        if (hit.transform.tag == "Ball")
                        {
                            return;
                        }
                        scoremanager.UsingBallCount();
                        Transform ballTr = Instantiate(Shot_Obj, CameraTr.position, Quaternion.identity).transform;
                        Ball ball = ballTr.GetComponent<Ball>();

                        //생성된 볼이 타깃(hit.point)를 바라보도록 회전
                        ballTr.rotation = Quaternion.LookRotation(hit.point - CameraTr.transform.position);
                        //회전 x축에 -8 추가(위로 살짝 올림)
                        Vector3 rot = ballTr.rotation.eulerAngles;
                        rot.x -= 8;
                        ballTr.rotation = Quaternion.Euler(rot);

                        //볼이 생성된 위치와 타깃지점의 거리 측정
                        distance = Vector3.Distance(CameraTr.position, hit.point);

                        //val_1의 10은 거리 기준점(상수값)
                        //거리가 10을 넘는지점은 상탄, 거리가 10이 안되는지점은 하탄 남.
                        //거리가 10이 넘는지점은 파워를 감소(마이너스 상수), 10이 안되는지점은 파워를 증가(플러스 사웃)를 추가해야함.
                        //예) 스트라이크존 거리 7.5(val_1 = 2.5)
                        //예) 뒤쪽 팬스 거리 11(val_1 = -1)
                        float val_1 = 10 - distance;

                        //파워 증감치(Val_1)에 가변값 상수(val_2)를 곱함.
                        value_tmp = val_1 * val_2;

                        //파워값은 100 * 거리 + value_tmp;
                        power = 100 * distance + value_tmp;

                        Rigidbody tmp = ballTr.GetComponent<Rigidbody>();
                        tmp.AddForce(ballTr.forward * power);

                        ball.SetRandomSpin();
                    }
                }

            }
        }
        public void CheckKeyDown(ScoreManager scoremanager)
        {
            if (scoremanager.BallChance > 0 && Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (BaseBallSingleManager.instance.state != BasaeBallState.Play)
                    return;
                Vector3 inputPoint = Input.mousePosition;
                float input_x = inputPoint.x;

                float max = camnum * (1920 / players);
                float min = max - (1920 / players);
                //Debug.Log(players);
                //Debug.Log(camnum + "/" + input_x);
                //Debug.Log(min + "/" + max);

                if (input_x > min && input_x < max)
                {
                    Ray ray = RayCamera.ScreenPointToRay(inputPoint);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100f))
                    {
                        if (hit.transform.tag == "Ball")
                        {
                            return;
                        }
                        scoremanager.UsingBallCount();
                        Transform ballTr = Instantiate(Shot_Obj, CameraTr.position, Quaternion.identity).transform;
                        Ball ball = ballTr.GetComponent<Ball>();

                        //생성된 볼이 타깃(hit.point)를 바라보도록 회전
                        ballTr.rotation = Quaternion.LookRotation(hit.point - CameraTr.transform.position);
                        //회전 x축에 -8 추가(위로 살짝 올림)
                        Vector3 rot = ballTr.rotation.eulerAngles;
                        rot.x -= 8;
                        ballTr.rotation = Quaternion.Euler(rot);

                        //볼이 생성된 위치와 타깃지점의 거리 측정
                        distance = Vector3.Distance(CameraTr.position, hit.point);

                        //val_1의 10은 거리 기준점(상수값)
                        //거리가 10을 넘는지점은 상탄, 거리가 10이 안되는지점은 하탄 남.
                        //거리가 10이 넘는지점은 파워를 감소(마이너스 상수), 10이 안되는지점은 파워를 증가(플러스 사웃)를 추가해야함.
                        //예) 스트라이크존 거리 7.5(val_1 = 2.5)
                        //예) 뒤쪽 팬스 거리 11(val_1 = -1)
                        float val_1 = 10 - distance;

                        //파워 증감치(Val_1)에 가변값 상수(val_2)를 곱함.
                        value_tmp = val_1 * val_2;

                        //파워값은 100 * 거리 + value_tmp;
                        power = 100 * distance + value_tmp;

                        Rigidbody tmp = ballTr.GetComponent<Rigidbody>();
                        tmp.AddForce(ballTr.forward * power);

                        ball.SetRandomSpin();
                    }
                }

            }
        }
*/
    }
}
