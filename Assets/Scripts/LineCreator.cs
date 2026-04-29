using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class LineCreator : MonoBehaviour
    {
        public GameObject lineObj;
        public Transform[] linePoints;

        // Use this for initialization
        void Start()
        {
            int idx = 1;            
            for (int i = 0; i < linePoints.Length; i++)
            {
                idx = i + 1;
                if (idx == linePoints.Length)
                {
                    idx = 0;
                }
                 Quaternion qut = Quaternion.LookRotation(linePoints[i].position - linePoints[idx].position);
            Transform LineTr = ((GameObject)Instantiate(lineObj, (linePoints[idx].position + linePoints[i].position) / 2, qut)).transform;
            float dist = Vector3.Distance(linePoints[idx].position, linePoints[i].position);
            dist += 0.5f;
            LineTr.localScale = new Vector3(0.1f, 0.1f, dist);
            }
        }


    }
}

