using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.SportsMiniGame.KinectSkating
{
    public class CrowdRandomAnim : MonoBehaviour
    {
        public Animation myanim;
        public AnimationClip[] clips;
        private void Awake()
        {
            myanim = this.GetComponent<Animation>();
            StartCoroutine(RandomAnimationPlay());
        }
        IEnumerator RandomAnimationPlay()
        {
            int rand = Random.Range(0, clips.Length);
            myanim.clip = clips[rand];
            yield return new WaitForSeconds(0.15f);
            myanim.Play();
        }
    }
}

