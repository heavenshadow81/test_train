using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.SportsMiniGame.KinectSkating
{
    public class Booster : MonoBehaviour
    {
        public GameObject[] BoosterImage;
        public ParticleSystem[] BoosterParticles;
        public int BoostIdx;
        private void Awake()
        {
            BoostIdx = 0;
            for (int i = 0; i < BoosterImage.Length; i++)
                BoosterImage[i].SetActive(false);
        }
        private void Update()
        {
           /* if (Input.GetKeyDown(KeyCode.S))
                BoostUp();
            if (Input.GetKeyDown(KeyCode.D))
                BoostDown();*/
        }
        public void BoostUp()
        {
            if (BoostIdx < BoosterImage.Length)
            {
                BoosterImage[BoostIdx].SetActive(true);
                BoosterParticles[BoostIdx].Play();
                BoostIdx++;
            }
        }
        public void BoostDown()
        {
            if (BoostIdx > 0)
            {
                BoostIdx--;
                BoosterParticles[BoostIdx].Stop();
                BoosterImage[BoostIdx].SetActive(false);
            }
        }
    }
}
