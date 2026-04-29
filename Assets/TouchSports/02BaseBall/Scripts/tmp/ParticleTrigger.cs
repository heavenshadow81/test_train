using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.T_Sports.BaseBall
{
    public class ParticleTrigger : MonoBehaviour
    {
        public ParticleSystem star;
        public UI_Particle[] uipaticles;
        public void ParticlePlay()
        {
            star.Play();
            for (int i = 0; i < uipaticles.Length; i++)
                uipaticles[i].TwinklePlay();
        }
    }
}