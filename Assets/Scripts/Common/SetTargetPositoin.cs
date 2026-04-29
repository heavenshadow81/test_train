using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.Common
{
    public class SetTargetPositoin : MonoBehaviour
    {
        public Transform mytr;
        public Transform targetTr;
        // Use this for initialization
        void Start()
        {
            mytr = this.transform;
        }

        // Update is called once per frame
        void Update()
        {
            if (targetTr != null)
                mytr.position = targetTr.position;
        }
    }

}
