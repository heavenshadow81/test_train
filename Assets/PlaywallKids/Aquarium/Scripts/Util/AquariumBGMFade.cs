using UnityEngine;
using DG.Tweening;

namespace ML.PlaywallKids.Aquarium
{
    public class AquariumBGMFade : MonoBehaviour
    {
        public AudioSource BGM;

        void Awake()
        {
            if (BGM != null)
            {
                BGM.volume = 0;
                BGM.DOFade(0.4f, 4.0f);
            }
        }
    }
}