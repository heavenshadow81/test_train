using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class ParticleRateController : MonoBehaviour
    {
        public ParticleSystem[] particles;
        private void Awake()
        {
            SetParticleRate(0);
        }

        public void SetParticleRate(float rate)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                var emit = particles[i].emission;
                    emit.rateOverDistance = +rate;
            }
        }
    }
}

