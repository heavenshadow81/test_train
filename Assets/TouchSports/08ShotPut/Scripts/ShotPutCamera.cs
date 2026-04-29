using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.T_Sports.ShotPut
{
    public class ShotPutCamera : MonoBehaviour
    {
        public Vector3 CameraDefaultPosition;
        public Quaternion CameraDefaultRotation;

        public Transform MyTr;
        public Transform LookTarget;

        public GameObject MyUICanvas;
        public bool CameraTracking;
        public float TrackingSpeed;

        public bool CameraReturnning;
        public float ReturnningSpeed;
        public float CamMovingTime;
        
        public int MyPlayerIdx;
        private void Awake()
        {
            MyTr = this.transform;
            CameraDefaultPosition = MyTr.position;
            CameraDefaultRotation = MyTr.rotation;
        }
        void Update()
        {
            if (CamMovingTime > 0)
                CamMovingTime -= Time.deltaTime;
            else
            {
                CameraTracking = false;
                CameraReturnning = true;
                StartCoroutine(ReturnCamera());
            }
            if (CameraTracking)
            {
                // MyTr과 LookTarget이 null인지 확인
                if (MyTr != null && LookTarget != null)
                {
                    float dist = Vector3.Distance(MyTr.position, LookTarget.position);
                    if (dist > 3)
                    {
                        MyTr.position = Vector3.Lerp(MyTr.position, LookTarget.position, Time.deltaTime * TrackingSpeed);
                    }
                    MyTr.LookAt(LookTarget);
                }
                //if (CameraTracking)
                //{
                //    float dist = Vector3.Distance(MyTr.position, LookTarget.position);
                //    if (dist > 3)
                //    {
                //        MyTr.position = Vector3.Lerp(MyTr.position, LookTarget.position, Time.deltaTime * TrackingSpeed);                    
                //    }
                //    MyTr.LookAt(LookTarget);
                //}
                // 미싱 오류가 떠서 이다인 수정
            }
        }

        public void CameraGo(Transform target, float retuntime)
        {
            MyUICanvas.SetActive(false);
            LookTarget = target;
            CameraTracking = true;
            CamMovingTime = retuntime;
        }

        IEnumerator ReturnCamera()
        {
            while (CameraReturnning)
            {
                float dist = Vector3.Distance(MyTr.position, CameraDefaultPosition);
                if (dist > 0.5f)
                {
                    MyTr.position = Vector3.Lerp(MyTr.position, CameraDefaultPosition, Time.deltaTime * ReturnningSpeed);
                    MyTr.rotation = Quaternion.Lerp(MyTr.rotation, CameraDefaultRotation, Time.deltaTime * ReturnningSpeed);
                }
                else
                {
                    CameraReturnning = false;
                    break;
                }
                yield return new WaitForSeconds(0.01f);
            }
            MyTr.position = CameraDefaultPosition;
            MyTr.rotation = CameraDefaultRotation;
            ShotPutManager.instance.SetPlayerAngle(MyPlayerIdx, true);
            MyUICanvas.SetActive(true);
        }
    }
}
