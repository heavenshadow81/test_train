using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class QuizZone : MonoBehaviour
    {
        public bool QuizOn;
        public QuizZone ontherQuizZone;
        
        public void OnTriggerEnter(Collider other)
        {
            if (!QuizOn && KinectSkateManager.instance.playstate == PlayState.Play)
            {
                if (other.tag == "Player")
                {                    
                    QuizOn = true;
                    ontherQuizZone.QuizOn = false;
                    KinectSkateManager.instance.QuizOn();
                    //퀴즈 호출
                }

            }
        }
    }
}

