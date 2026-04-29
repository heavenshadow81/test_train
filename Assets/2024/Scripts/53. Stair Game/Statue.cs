using GuessNumber;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StairGame
{
    public class Statue : MonoBehaviour
    {
        public ObjectFade Fade { get; private set; }

        private void Awake()
        {
            Fade = GetComponent<ObjectFade>();

            Fade.FadeOut(0f);
        }
    }
}
