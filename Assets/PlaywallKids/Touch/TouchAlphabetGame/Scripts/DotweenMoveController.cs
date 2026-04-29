using UnityEngine;
using System.Collections;
//using DG.Tweening;

public class DotweenMoveController : MonoBehaviour {

    public Transform target;
    public Transform cam;

    [Range(0.01f,2f)]
    public float rotateSpeed;
    Transform cachedTransform;
    Vector3 targetRotate;

    void Start()
    {
        cachedTransform = this.transform;
      //  this.transform.DOPath(iTweenPath.GetPath("AlphabetPath"), 10f, PathType.CatmullRom, PathMode.Full3D, 10).SetSpeedBased(true);//.OnComplete(); //reference DragonPath.cs
      //  iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("AlphabetPath"), "axis", "y", "time", 500, "orienttopath", true));
    }

    void OnEnable()
    {
        if (rotateSpeed == 0) rotateSpeed = 1f;
        targetRotate = Vector3.zero;
    }

   void OnTriggerEnter(Collider other)
   {
        targetRotate = other.transform.localEulerAngles;
        targetRotate.x = 0f;
        targetRotate.y = 0f;
   }

    void LateUpdate()
    {
        if (target != null)
        {
            cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.Euler(targetRotate), Time.deltaTime * rotateSpeed);
            cachedTransform.LookAt(target);
        
            //Quaternion q = Quaternion.LookRotation(target.position - cachedTransform.position); 
            //cachedTransform.localRotation = Quaternion.Lerp(cachedTransform.localRotation, q, Time.deltaTime);
           
        }/*
        else
        {
            Vector3 dir = cachedTransform.position - prePos;
            cachedTransform.localRotation = Quaternion.Euler(dir);
            prePos = cachedTransform.position;
        }*/
    }
}


/* void FixedUpdate()
    {
        
        if(nodeIndex < pathNode.Length)
        {
            Vector3 vector = pathNode[nodeIndex] - cachedTransform.position;
            float sqrDistance = vector.sqrMagnitude;
            if (sqrDistance < 1f)
            {
                if( (bitChecker & 0x01L << nodeIndex )== 0)
                {


                    bitChecker |= 0x01L << nodeIndex;
                    nodeIndex++;
                }
            }else
            {
                cachedTransform.localEulerAngles = Vector3.Lerp(cachedTransform.localEulerAngles, rotateNode[nodeIndex], Time.deltaTime);
            }
        }else
        {
            Debug.Log("Arrive");
        }
    }*/