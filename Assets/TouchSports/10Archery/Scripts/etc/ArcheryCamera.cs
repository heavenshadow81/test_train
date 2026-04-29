using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.T_Sports.Archery
{
    public class ArcheryCamera : MonoBehaviour
    {
        public Transform MyTr;
        public Transform ReadyTr;
        public Vector3 StartPos;
        public Quaternion StartRot;
        public Transform LookTargets;
        public float TrackingSpeed;
        public float ReturningSpeed;

        public bool Go;
        public bool Returning;
        void Start()
        {
            MyTr = this.transform;
            StartPos = MyTr.position;
            StartRot = MyTr.rotation;
            Go = false;
            Returning = false;
        }
        // Update is called once per frame
        /* void Update()
         {
             if (Go)
             {
                 float dist = Vector3.Distance(MyTr.position, LookTargets.position);
                 MyTr.LookAt(LookTargets);
                 MyTr.position = Vector3.Lerp(MyTr.position, LookTargets.position, Time.deltaTime * TrackingSpeed);

                 if (dist < 3 && !Returning)
                 {
                     Go = false;
                     Returning = true;
                     StartCoroutine(ReturnCamera());                    
                 }
             }
         }*/
       /* IEnumerator go()
        {
            yield return new WaitForSeconds(0.15f);
            Go = true;
        }*/
        IEnumerator goCam()
        {
            yield return new WaitForSeconds(2.15f);
            Go = false;
            Returning = true;
            StartCoroutine(ReturnCamera());
        }
        public void SetCamera(Transform CamPos, Transform LookTarget)
        {
            LookTargets = LookTarget;
            MyTr.position = CamPos.position;
            MyTr.parent = LookTarget;
            MyTr.LookAt(LookTarget);

            if (Returning)
                Returning = false;
            StartCoroutine(goCam());
        }
        
        IEnumerator ReturnCamera()
        {
            yield return new WaitForSeconds(1);
            while (Returning)
            {
                yield return new WaitForSeconds(0.01f);
                float dist = Vector3.Distance(MyTr.position, StartPos);
                MyTr.position = Vector3.Lerp(MyTr.position, StartPos, Time.deltaTime * ReturningSpeed);
                MyTr.rotation = Quaternion.Lerp(MyTr.rotation, StartRot, Time.deltaTime * 10);
                if (dist <= 1f)
                {
                    Returning = false;
                }
            }
            MyTr.position = StartPos;
            MyTr.rotation = StartRot;
        }
    }
}
