using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.SportsMiniGame.KinectSkating
{
    public class AIPosition : MonoBehaviour
    {
        public Transform mytr;
        private void Awake()
        {
            mytr = this.transform;
            Vector3 tmp = mytr.localPosition;
            tmp.x = Random.Range(-3, 3);
            mytr.localPosition = tmp;
        }
    }
}

