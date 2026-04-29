using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.MotionJump
{
    public class UIResultController : MonoBehaviour
    {
        const string prefixFileName = "imgStar_";
        const string prefixEmptyFileName = "imgEmptyStar_";

        public UISprite[] stars;
        public Transform origin;
        public Transform target;
        public GameObject panel;
        public GameObject celebtrationObj;
        [Range(0.1f, 3f)]
        public float duration;
        public AnimationCurve aniCurve;

        public NumericsDisplayer goldPointDisplayer;
        public NumericsDisplayer scorePointDisplayer;
        [HideInInspector]
        UITimeDisplayer timeDisplayer;
        public bool bShowUp { get; private set; }

        private Vector3 originPos;

        void Awake()
        {
            originPos = panel.transform.localPosition;
            if (duration == 0) duration = 1.5f;
        }

        public void ChangeTime(float seconds)
        {
            if (timeDisplayer == null) timeDisplayer = GetComponentInChildren<UITimeDisplayer>();
            if (timeDisplayer != null)
            { timeDisplayer.ChangeTime(seconds); }
        }

        public void ShowUp()
        {
            if (!bShowUp)
            {
                panel.SetActive(true);
                TweenTransform tween = TweenTransform.Begin(panel, duration, origin, target); ;
                tween.animationCurve = aniCurve;
                celebtrationObj.SetActive(true);
            }
        }

        public void Init()
        {
            panel.transform.localPosition = originPos;
            panel.SetActive(false);
            celebtrationObj.SetActive(false);
            bShowUp = false;
            NumericsDisplayer.ChangePoint(goldPointDisplayer, 0);
            NumericsDisplayer.ChangePoint(scorePointDisplayer, 0);
            ChangeTime(0);

            for (int i = 0; i < stars.Length; ++i)
            {
                stars[i].spriteName = prefixEmptyFileName + i.ToString();
            }
        }

        public void ChangeStarCount(int _point)
        {
            if (stars == null) return;

            int len = stars.Length;
            if (_point < 200)
            {
                len = 1;
            }
            else if (200 <= _point && _point < 1000)
            { len = 2; }
            StartCoroutine(ShowStarProcess(len));
        }

        IEnumerator ShowStarProcess(int length)
        {
            for (int i = 0; i < length; ++i)
            {
                if (i < stars.Length)
                {
                    stars[i].cachedTransform.localScale = Vector3.zero;
                    stars[i].spriteName = prefixFileName + i.ToString();
                    TweenScale tween = TweenScale.Begin(stars[i].cachedGameObject, 0.2f, Vector3.one);
                }
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForEndOfFrame();
        }
    }
}