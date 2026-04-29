using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class SelectedAnserCheck : MonoBehaviour
    {
        public int index;
        public bool touchAble;
        public ParticleSystem Correct;
        public ParticleSystem Playerboom;
        public ParticleSystem Wrong;
        public SelectedAnserCheck anotherSelect;

        public ExplosionIce exp;
        public void OnTriggerEnter(Collider other)
        {
            if (!touchAble)
                return;
            if (other.tag == "Player")
            {
                anotherSelect.touchAble = false;
                touchAble = false;
                if (KinectSkateManager.instance.playstate == PlayState.Play)
                {
                    exp.Explosion();
                    if (KinectSkateManager.instance.UserSelected(index))
                    {
                        Playerboom.Play();
                        Correct.Play();
                    }
                    else
                    {
                        Wrong.Play();
                    }
                }                
            }
        }
    }
}

