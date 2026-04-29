using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.T_Sports.Common
{
    public class BallCounter : MonoBehaviour
    {
        Transform mytr;
        public GameObject strike;
        public GameObject ball;
        // Use this for initialization
        void Start()
        {
            mytr = this.transform;
            strike = mytr.GetChild(0).gameObject;
            ball = mytr.GetChild(1).gameObject;
            ResetCounter();
        }

        public void SetStrike()
        {
            //Debug.Log("SetStrike");
            strike.SetActive(true);
            ball.SetActive(false);
        }
        public void SetBall()
        {
            //Debug.Log("SetBall");
            strike.SetActive(false);
            ball.SetActive(true);
        }
        public void ResetCounter()
        {
            //Debug.Log("ResetCounter");
            if (strike != null)
                strike.SetActive(false);
            if (ball != null)
                ball.SetActive(false);
        }
    }
}

