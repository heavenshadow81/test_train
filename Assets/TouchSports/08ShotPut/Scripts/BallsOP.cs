using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.T_Sports.ShotPut
{
    public class BallsOP : MonoBehaviour
    {
        public Transform Mytr;
        private void Awake()
        {
            Mytr = this.transform; 
        }
        public void BallReset()
        {
            if (Mytr.childCount <= 0)
                return;
            for (int i = 0; i < Mytr.childCount; i++)
            {
                Destroy(Mytr.GetChild(i).gameObject);
            }
        }
    }
}
