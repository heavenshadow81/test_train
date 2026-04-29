using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuessNumber
{
    public class Statue : MonoBehaviour
    {
        public ObjectFade fade;
        [SerializeField] private Transform[] answerTransforms = null;

        public void StatueFadeOut(float fadeDuration, System.Action onComplete = null)
        {
            fade.FadeOut(fadeDuration, onComplete);
        }

        public Transform GetAnswerTransform(int idx)
        {
            return answerTransforms[idx];
        }
    }
}

