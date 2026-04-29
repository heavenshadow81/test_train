using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
namespace ML.SportsMiniGame.KinectSkating
{
    public class LoadingRunner : MonoBehaviour
    {
        public Image LoadingBar;
        public RectTransform LoadingRunnerIcon;
        public RectTransform Target;
        public float PerDistance;
        // Use this for initialization
        void Start()
        {
            PerDistance = Vector2.Distance(LoadingRunnerIcon.anchoredPosition, Target.anchoredPosition);
            PerDistance = PerDistance / 100;
           // StartCoroutine(LoadingBarUI());
        }
        IEnumerator LoadingBarUI()
        {
            while (true)
            {
                LoadingBar.fillAmount += 0.01f;
                Vector2 tmp = LoadingRunnerIcon.anchoredPosition;
                tmp.x += PerDistance;
                LoadingRunnerIcon.anchoredPosition = tmp;
                if (tmp.x >= Target.anchoredPosition.x)
                {
                    break;
                }
                yield return new WaitForSeconds(Random.Range(0.01f, 0.005f));
            }
            KinectSkateManager.instance.playstate = PlayState.Intro;
        }
    }
}

