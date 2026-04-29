using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class SandParticle : MonoBehaviour
    {

        GameObject _obj;
        public GameObject obj
        {
            get
            {
                if (_obj == null)
                {
                    _obj = gameObject;
                }

                return _obj;
            }
        }

        private Transform _trans;
        public Transform cachedTransform
        {
            get
            {
                if (_trans == null)
                {
                    _trans = transform;
                }
                return _trans;
            }
        }

        private AudioSource _audio;
        public AudioSource mAudio
        {
            get
            {
                if (_audio == null)
                {
                    _audio = obj.GetComponent<AudioSource>();//  audio clip "sndSandEffect", 
                }
                return _audio;

            }
        }

        AudioClip clip = null;
        public void PlayEmit()
        {

            AudioSource.PlayClipAtPoint(mAudio.clip, cachedTransform.position);

            obj.SetActive(true);
        }

        public void StopEmit()
        {
            obj.SetActive(false);
        }
    }
}