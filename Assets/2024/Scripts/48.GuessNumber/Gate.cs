using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuessNumber
{
    public class Gate : MonoBehaviour
    {
        public ObjectFade fade;

        public void GateFadeOut(float fadeDuration, System.Action onComplete = null)
        {
            fade.FadeOut(fadeDuration, onComplete);
        }
    }
}

