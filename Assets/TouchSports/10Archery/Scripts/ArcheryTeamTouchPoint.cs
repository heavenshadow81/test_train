using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.T_Sports.Common;
namespace ML.T_Sports.Archery
{
    
    public class ArcheryTeamTouchPoint : MonoBehaviour
    {
        public int Player;
        public Camera CameraPlayerA;
        public ArcheryCameraMoving CameraMovingA;
        public Transform PlayerATr;
        public Transform TargetCenterA;

        public Camera CameraPlayerB;
        public ArcheryCameraMoving CameraMovingB;
        public Transform PlayerBTr;
        public Transform TargetCenterB;

        public float DistanceA;
        public float CoolA;
        public float CoolB;
        

        public float ArrowSpeed;
        public GameObject ArrowObj;

        public EFMPlayer Shooting;
        public bool RoundCheck;
        public bool ChangeCheck;
        public Transform ArrowOP;
        void Update()
        {
            if (CoolA > 0)
                CoolA -= Time.deltaTime;
            else if (CoolA <= 0)
            {
                CoolA = 0;
            }
            if (CoolB > 0)
                CoolB -= Time.deltaTime;
            else if (CoolB <= 0)
            {
                CoolB = 0;
            }

            if (ArcheryTeamManager.instance == null)
                return;


            int TouchCount = TouchModule.TouchModuleInput.touchCount;
            if (TouchCount != 0)
            {
                if (ArcheryTeamManager.instance.state != ArcheryTeamState.Play)
                {
                    //Play상황이아니면 입력에대해 작용하지 않음.
                    Debug.Log(ArcheryTeamManager.instance.state);
                    return;
                }
                Touch[] touchs = TouchModule.TouchModuleInput.touches;
                for (int i = 0; i < touchs.Length; i++)
                {
                    if (touchs[i].phase != TouchPhase.Began) continue;
                    Vector3 pos = touchs[i].position;
                    float areaA = 1920 / Player;
                    float areaB = areaA * 2;
                   // Debug.Log("areaA"+ areaA);
                    //Debug.Log("areaB" + areaB);

                    if (pos.x < areaA && CoolA <= 0 && ArcheryTeamManager.instance.scores[0].state == PlayerState.Play)
                    {
                        CoolA = 3;
                        //Debug.Log("1p 터치");
                        // pos.x += pos.x / 2;
                        Shooting.EFMRandomPlay();
                        ShotArrow(CameraPlayerA, pos, PlayerATr, CameraMovingA);
                    }
                    else if (pos.x >= areaA && pos.x < areaB && CoolB <= 0 && ArcheryTeamManager.instance.scores[1].state == PlayerState.Play)
                    {
                        CoolB = 3;
                        //pos.x -= pos.x / 2;
                        //Debug.Log("2p 터치");
                        Shooting.EFMRandomPlay();
                        ShotArrow(CameraPlayerB, pos, PlayerBTr, CameraMovingB);
                    }

                }
            }
        }
        public void ShotArrow(Camera PlayerCamera, Vector3 TouchPos, Transform StartPos, ArcheryCameraMoving camMoving)
        {
            Ray ray = PlayerCamera.ScreenPointToRay(TouchPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                StartPos.LookAt(hit.point);
                if (hit.transform.name == "TouchAreaA")
                {
                    GameObject arrowobj = (GameObject)Instantiate(ArrowObj, StartPos.position, StartPos.rotation);
                    arrowobj.transform.parent = ArrowOP;

                    ArcheryArrow arrow = arrowobj.GetComponent<ArcheryArrow>();
                    arrow.SetTarget(hit.point, ArrowSpeed, TargetCenterA, 0, camMoving);
                    camMoving.SetTrackingCamera(arrow.CamPos);
                }
                else if (hit.transform.name == "TouchAreaB")
                {
                    GameObject arrowobj = (GameObject)Instantiate(ArrowObj, StartPos.position, StartPos.rotation);
                    arrowobj.transform.parent = ArrowOP;

                    ArcheryArrow arrow = arrowobj.GetComponent<ArcheryArrow>();
                    arrow.SetTarget(hit.point, ArrowSpeed, TargetCenterB, 1, camMoving);
                    camMoving.SetTrackingCamera(arrow.CamPos);
                }
                else if (hit.transform.name == "TouchArea_Out")
                {
                    GameObject arrowobj = (GameObject)Instantiate(ArrowObj, StartPos.position, StartPos.rotation);
                    arrowobj.transform.parent = ArrowOP;
                    ArcheryArrow arrow = arrowobj.GetComponent<ArcheryArrow>();
                    //arrow.SetTarget(hit.point, ArrowSpeed, TargetCenterA, 0);
                }
            }
        }
        public void ArrowDestroy()
        {
            if (ArrowOP.childCount == 0)
            {
                return;
            }
            for (int i = 0; i < ArrowOP.childCount; i++)
            {
                Destroy(ArrowOP.GetChild(i).gameObject);
            }
        }
    }
}

