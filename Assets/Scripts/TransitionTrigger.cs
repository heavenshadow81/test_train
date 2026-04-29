using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class TransitionTrigger : MonoBehaviour
    {
        public ParticleSystem snow;
        public AudioSource TransitionSound;
        public GameObject Podiumcam;
        public GameObject MyObj;
        //애니메이션에 들어있음..
        public void CamChange()
        {
            MyObj.SetActive(false);
            Podiumcam.SetActive(true);
        }
        //애니메이션 트리거로 발동됨
        public void ParticlePlay()
        {
            TransitionSound.Play();
            //snow.Play();
        }
    }
}
