using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class AIGUIAnim : MonoBehaviour
    {
        public Animator myanim;

        private void Awake()
        {
            myanim = this.GetComponent<Animator>();
            StartCoroutine(AutoPlayAnim());
        }
        IEnumerator AutoPlayAnim()
        {
            while (true)
            {
                yield return new WaitForSeconds(6f);
                myanim.SetTrigger("ConerTrigger");
                myanim.SetBool("Conering", true);
                yield return new WaitForSeconds(4f);
                myanim.SetBool("Conering", false);
            }
        }
       
    }
}

