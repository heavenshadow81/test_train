using UnityEngine;
using UnityEngine.SceneManagement;

namespace ML.PlaywallKids.Aquarium
{
    public class SeaIntro : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }

        void DestroyFade()
        {
            iTween.CameraFadeDestroy();
        }

        void Awake()
        {
            iTween.CameraFadeAdd();
            iTween.CameraFadeFrom(iTween.Hash("amount", 1.0f, "time", 2, "oncomplete", "DestroyFade", "oncompletetarget", gameObject));
            iTween.AudioTo(gameObject, iTween.Hash("volume", 1, "time", 4));
        }

        public void FadeOut()
        {
            iTween.CameraFadeAdd();
            iTween.CameraFadeTo(iTween.Hash("amount", 1.0f, "time", 3.5, "delay", 1, "onComplete", "LoadNextLevel", "onCompleteTarget", gameObject));
            iTween.AudioTo(gameObject, iTween.Hash("volume", 0, "time", 6));
        }

        void LoadNextLevel()
        {
            SceneManager.LoadScene(1);
        }
    }
}