using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace ML.PlaywallKids.DragonPark.Test
{
    public class SimpleFreeDrawingTest : MonoBehaviour
    {
        public UILabel printLog;
        public UIGrid grid;

        public int makeItemCnt = 1;

        public GameObject carPrefab;
        public GameObject robotPrefab;
        public Shader shader;

        public GameObject robotEyePrefab;
        public GameObject robotAntennaPrefab;
        public GameObject robotMouthPrefab;

        public CanvasSplineDrawingSupport[] canvasSplineDrawings;
        string[] carBoneNames = {
                                 FreeDrawingCarBone.kBodyMomentumBone,
                                 FreeDrawingCarBone.kWheelFLBone
                             };

        string[] carWheelNames = {
                                 FreeDrawingCarBone.kWheelFLBone,
                                 FreeDrawingCarBone.kWheelFRBone,
                                 FreeDrawingCarBone.kWheelRLBone,
                                 FreeDrawingCarBone.kWheelRRBone
                             };

        string[] robotBoneNames = {
                                  FreeDrawingRobotBone.kHeadBone,
                                  FreeDrawingRobotBone.kBodyBone,
                                  FreeDrawingRobotBone.kArmLBone,
                                  FreeDrawingRobotBone.kLegLBone
                              };

        string[] robotArmNames = {
                                  FreeDrawingRobotBone.kArmLBone,
                                  FreeDrawingRobotBone.kArmRBone
                             };

        string[] robotLegNames = {
                                  FreeDrawingRobotBone.kLegLBone,
                                  FreeDrawingRobotBone.kLegRBone
                             };


        private List<GameObject> deleteList = new List<GameObject>();

        private FreeDrawingObjectType curType;

        void Start()
        {
            SetType(FreeDrawingObjectType.Car);
        }

        public void SetCarType()
        {
            SetType(FreeDrawingObjectType.Car);
        }

        public void SetRobotType()
        {
            SetType(FreeDrawingObjectType.Robot);
        }

        public void Create1()
        {
            Create(1);
        }

        public void Create10()
        {
            Create(10);
        }

        private void SetType(FreeDrawingObjectType type)
        {
            string[] carSprite = new string[] { "3D_car_step01", "3D_car_step02" };
            string[] robotSprite = new string[] { "3D_robot_step01", "3D_robot_step02", "3D_robot_step03", "3D_robot_step04" };

            string[] spriteName = null;

            curType = type;
            for (int i = 0; i < canvasSplineDrawings.Length; i++)
                canvasSplineDrawings[i].transform.parent.gameObject.SetActive(true);

            if (type == FreeDrawingObjectType.Car)
            {
                canvasSplineDrawings[2].transform.parent.gameObject.SetActive(false);
                canvasSplineDrawings[3].transform.parent.gameObject.SetActive(false);
                spriteName = carSprite;
            }
            else if (type == FreeDrawingObjectType.Robot)
            {
                spriteName = robotSprite;
            }

            for (int i = 0; i < spriteName.Length; i++)
            {
                UISprite sprite = canvasSplineDrawings[i].transform.parent.Find("Sprite").gameObject.GetComponent<UISprite>();
                sprite.spriteName = spriteName[i];
                //sprite.MakePixelPerfect();
            }

            grid.repositionNow = true;
            grid.Reposition();
        }

        private void Create(int makeCnt = 1)    // 만들기 함수 // Defualt 1개 
        {
            makeItemCnt = makeCnt;      // 버튼 클릭으로 생성할 갯수 
            if (curType == FreeDrawingObjectType.Car) // 현재 선택 탭이 자동차
                CreateCar();                    // 자동차 만들기
            else if (curType == FreeDrawingObjectType.Robot)  // 현재 선택 탭이 로봇
                CreateRobot();                          // 로봇 만들기

            if (listMilliseconds.Count >= 50)        // 누적 오브젝트가 50개 이상이면
            {
                WriteToFile();                      // 50번 카운트 기록 로그파일 만들기
            }
        }

        public void CreateCar()     // 자동차 만들기
        {
            for (int k = 0; k < makeItemCnt; k++)   // 입력된 갯수만큼 반복
            {
                // 타이머 시작 ( 현재 시간 저장 )
                MakeStart();

                // 파츠별 2D -> 3D 작업 진행
                for (int i = 0; i < carBoneNames.Length; i++)
                {
                    Spline spline = canvasSplineDrawings[i].spline;
                    if (spline == null) return;
                }

                GameObject newCar = (GameObject)Instantiate(carPrefab);
                newCar.transform.localPosition = new Vector3(22.0f, 0.0f, Random.Range(-2.0f, 2.0f));
                newCar.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                FreeDrawingCarBone bone = newCar.GetComponent<FreeDrawingCarBone>();
                FreeDrawingAnimationControl animationControl = newCar.AddComponent<FreeDrawingAnimationControl>();

                for (int i = 0; i < carBoneNames.Length; i++)
                {
                    Spline spline = canvasSplineDrawings[i].spline;
                    Mesh mesh = FreeDrawingMeshMaker.MakeMesh("car", carBoneNames[i], spline);
                    if (i == 0)
                    {
                        GameObject go = FreeDrawingMeshMaker.MakeGameObject(carBoneNames[i], mesh);
                        bone.SetAccessory(carBoneNames[i], go, true);
                    }
                    else
                    {
                        for (int j = 0; j < carWheelNames.Length; j++)
                        {
                            GameObject go = FreeDrawingMeshMaker.MakeGameObject(carWheelNames[j], mesh);
                            bone.SetAccessory(carWheelNames[j], go, true);
                        }
                    }
                }


                bone.PrepareDefaultAccessories();

                StartCoroutine(BackCar(animationControl));

                animationControl.comeToFront.Back();

                // 타이머 종료 ( 시작 타이머와 현재 시간의 차이값 계산)
                MakeEnd();

                deleteList.Add(animationControl.gameObject);
            }


            // 라인 초기화
            for (int i = 0; i < carBoneNames.Length; i++)
                canvasSplineDrawings[i].Clear();
        }

        IEnumerator BackCar(FreeDrawingAnimationControl con)
        {
            yield return new WaitForSeconds(2f);
            con.comeToFront.Back();
        }

        // 로봇 생성 버튼 클릭
        public void CreateRobot()
        {
            // makeItemCnt 만큼 반복 실행 
            for (int k = 0; k < makeItemCnt; k++)
            {
                // 타이머 시작 ( 현재 시간 저장 )
                MakeStart();

                // 파츠별 2D -> 3D 작업 진행
                #region 2D->3D 변환 로직
                for (int i = 0; i < robotBoneNames.Length; i++)
                {
                    Spline spline = canvasSplineDrawings[i].spline;
                    if (spline == null) return;
                }

                GameObject newRobot = (GameObject)Instantiate(robotPrefab);
                newRobot.transform.localPosition = new Vector3(22.0f, 12.0f, Random.Range(-2.0f, 2.0f));
                newRobot.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                newRobot.transform.localRotation = Quaternion.Euler(0, 90, 0);

                FreeDrawingRobotBone bone = newRobot.GetComponent<FreeDrawingRobotBone>();
                FreeDrawingAnimationControl animationControl = newRobot.AddComponent<FreeDrawingAnimationControl>();

                for (int i = 0; i < robotBoneNames.Length; i++)
                {
                    Spline spline = canvasSplineDrawings[i].spline;
                    Mesh mesh = FreeDrawingMeshMaker.MakeMesh("robot", robotBoneNames[i], spline);
                    if (i < 2)
                    {
                        GameObject go = FreeDrawingMeshMaker.MakeGameObject(robotBoneNames[i], mesh);
                        bone.SetAccessory(robotBoneNames[i], go, true);
                    }
                    else if (i == 2)
                    {
                        for (int j = 0; j < robotArmNames.Length; j++)
                        {
                            GameObject go = FreeDrawingMeshMaker.MakeGameObject(robotArmNames[j], mesh);
                            bone.SetAccessory(robotArmNames[j], go, true);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < robotLegNames.Length; j++)
                        {
                            GameObject go = FreeDrawingMeshMaker.MakeGameObject(robotLegNames[j], mesh);
                            bone.SetAccessory(robotLegNames[j], go, true);
                        }
                    }
                }

                bone.PrepareDefaultAccessories();

                animationControl.Land();
                #endregion

                // 타이머 종료 ( 시작 타이머와 현재 시간의 차이값 계산)
                MakeEnd();

                deleteList.Add(animationControl.gameObject);
            }

            // 라인 초기화
            for (int i = 0; i < robotBoneNames.Length; i++)
                canvasSplineDrawings[i].Clear();
        }

        public void Clear()
        {
            StopAllCoroutines();
            for (int i = 0; i < deleteList.Count; i++)
                Destroy(deleteList[i]);
            deleteList.Clear();

            listMilliseconds.Clear();


            printLog.text = string.Format("Interval : {0}sec\nAverage : {1}sec\nCount : {2}", 0, 0, 0);
        }

        System.DateTime start, end;
        List<int> listMilliseconds = new List<int>();
        private void MakeStart()
        {
            // Start시간 측정
            start = System.DateTime.Now;
        }

        private void MakeEnd()
        {
            // End 시간 측정
            end = System.DateTime.Now;
            // End-Start 시간을 구해 객체 생성시간, 평균 생성시간을 화면에 출력한다.
            TestPrint();
        }

        private void TestPrint()
        {
            // End - Start 시간 차이 계산
            System.TimeSpan span = end - start;

            // 시간차이를 리스트에 등록
            listMilliseconds.Add(Mathf.RoundToInt((float)span.TotalMilliseconds));

            // 평균값 구하기
            int avg = 0;
            for (int i = 0; i < listMilliseconds.Count; i++)
                avg += listMilliseconds[i];
            avg = avg / listMilliseconds.Count;

            // 시간 차이와 평균값, 생성 카운트를 화면에 출력
            printLog.text = string.Format("Interval : {0}sec\nAverage : {1}sec\nCount : {2}",
                listMilliseconds[listMilliseconds.Count - 1] * 0.001f, avg * 0.001f, listMilliseconds.Count);
        }

        public void WriteToFile()
        {// log text 파일을 생성 하는 함수
         //if (!bWriteFile) return;
            string _context = "평가 항목 : 낙서 객체 생성(랜더링)속도\r\n단위 : Sec\r\n개발 목표치 : 0.3 \r\n \r\n";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(_context);

            for (int i = 0, len = listMilliseconds.Count; i < len; ++i)
            {
                _context = string.Format("Count : {0,2}, Interval : {1:F3} sec, Result : {2,10} \r\n",
                    i + 1, listMilliseconds[i] * 0.001f, (0.3f >= listMilliseconds[i] * 0.001f ? "SUCCESS" : "FAIL"));
                sb.Append(_context);
            }

            int avg = 0;
            for (int i = 0; i < listMilliseconds.Count; i++)
                avg += listMilliseconds[i];
            avg = avg / listMilliseconds.Count;
            _context = string.Format("\r\nTotal Count : {0}\r\nAverage : {1}sec\r\nResult : {2}",
                                     listMilliseconds.Count, avg * 0.001f, 0.3f >= avg * 0.001f ? "SUCCESS" : "FAIL");
            sb.Append(_context);
            string fileName = System.DateTime.Now.ToString("yyyy-MM-dd HHmmss");
            System.IO.File.WriteAllText(string.Format("{0}/{1}.txt", Application.dataPath, fileName), sb.ToString(0, sb.Length));

            string _path = Application.dataPath.Replace(@"/", @"\");
            System.Diagnostics.Process.Start("explorer.exe", "/select," + _path);
        }
    }
}