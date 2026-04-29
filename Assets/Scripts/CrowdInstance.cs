using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.SportsMiniGame.KinectSkating
{
    public class CrowdInstance : MonoBehaviour
    {
        public GameObject[] Crows;
        public Transform myTr;
        // Use this for initialization
        void Start()
        {
            myTr = this.transform;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}