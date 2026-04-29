using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ML.SportsMiniGame.Common
{
    public class SpriteAnimation : MonoBehaviour
    {
        public Image AnimationImage;
        public Sprite[] Sequence;
        private int SequenceIdx;
        public float AnimationSpeed;
        private void Awake()
        {
            SequenceIdx = 0;
          //  StartCoroutine(SequenceAnimationPlay());
        }
        IEnumerator SequenceAnimationPlay()
        {
            while (true)
            {
                AnimationImage.sprite = Sequence[SequenceIdx];
                SequenceIdx++;
                if (SequenceIdx >= Sequence.Length)
                    SequenceIdx = 0;
                yield return new WaitForSeconds(AnimationSpeed);
            }
        }
    }
}

