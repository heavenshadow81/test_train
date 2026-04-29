using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.T_Sports.Archery
{
    public class ArcheryCameraMoving : MonoBehaviour
    {
        
        public Vector3 StartPos;
        public Quaternion StartRot;

        public Transform TargetCameraTr;
        public Transform TargetLookTr;
        public float speed;
        public bool TrackingGo;
        private void Awake()
        {
            TrackingGo = false;
            
            StartPos = transform.position;
            StartRot = transform.rotation;
        }
        private void Update()
        {
            if (TrackingGo)
            {
                float dist = Vector3.Distance(transform.position, TargetCameraTr.position);
                transform.position = Vector3.Lerp(transform.position, TargetCameraTr.position, Time.deltaTime * speed);                
            }
        }

        public void ResetCamera()
        {
            TrackingGo = false;
            StartCoroutine(ReturnCameraPosition());
        }
        IEnumerator Going()
        {
            yield return new WaitForSeconds(0.15f);
            TrackingGo = true;
        }
        IEnumerator ReturnCameraPosition()
        {
            print($"현재 위치: {transform.position.x}, {transform.position.y},{transform.position.z}");
            yield return new WaitForSeconds(1);
            transform.SetPositionAndRotation(StartPos, StartRot);
            print($"되돌아갈 위치: {StartPos.x}, {StartPos.y},{StartPos.z}");
            print($"되돌아간 위치: {transform.position.x}, {transform.position.y},{transform.position.z}");
        }
        public void SetTrackingCamera(Transform campos)
        {
            TargetCameraTr = campos;
            transform.SetPositionAndRotation(campos.position, campos.rotation);
            
            StartCoroutine(Going());
        }
    }
}
