using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.SportsMiniGame.KinectSkating
{
    public class CurvChange : MonoBehaviour
    {
        public bool CurvStart;
        public bool CurvEnd;
        public Animator PlayerAnim;
        public Animator AIAnim;
        public void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                if (CurvStart)
                {
                    PlayerAnim.SetTrigger("ConerTrigger");
                    PlayerAnim.SetBool("Conering", true);

                }
                if (CurvEnd)
                {
                    PlayerAnim.SetBool("Conering", false);
                }
            }
            else if (other.tag == "AI")
            {
                if (CurvStart)
                {
                    AIAnim.SetTrigger("ConerTrigger");
                    AIAnim.SetBool("Conering", true);

                }
                if (CurvEnd)
                {
                    AIAnim.SetBool("Conering", false);
                }
            }            
        }
    }
}
