using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    public class Base : MonoBehaviour
    {
        [SerializeField] Line line = null;

        public void PlayTouchAnim()
        {
            line.LineAnim();
        }

        public Line GetLine()
        {
            return line;
        }
    }
}

