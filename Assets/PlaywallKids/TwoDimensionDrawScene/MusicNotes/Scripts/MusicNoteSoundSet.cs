using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class MusicNoteSoundSet : MonoBehaviour
    {
        public AudioClip[] clips;

        public AudioClip RandomClip()
        {
            AudioClip clip = null;
            if (clips != null && clips.Length > 0)
            {
                clip = clips[Random.Range(0, clips.Length)];
            }
            return clip;
        }
    }
}