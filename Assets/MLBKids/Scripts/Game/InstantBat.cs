using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace ML.MLBKids
{
    public class InstantBat : MonoBehaviour
    {
        public Vector3 dir;

        public void Start()
        {
            Stadium stadium = Stadium.instance;

            if(stadium == null)
            {
                Destroy(gameObject);
                return;
            }

            Camera cam = stadium.cams[(int)Stadium.GameMode.Hit];

            float verticalAngle = Vector3.Angle(dir, cam.transform.forward);
            transform.rotation = Quaternion.Euler(verticalAngle, 50, 0);
            Vector3 axis = Vector3.Cross(transform.forward, dir);
            Quaternion q = Quaternion.AngleAxis(110.0f, axis);
            transform.DORotateQuaternion(transform.rotation * q, 0.15f);

            Destroy(gameObject, 0.5f);
        }
    }
}
