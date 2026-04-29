using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmGame
{
    public class EffectManager : MonoBehaviour
    {
        [SerializeField] Animator noteHitAnimator = null;
        string hit = "Hit";

        [SerializeField] GameObject[] judgementEffet = null;

        public void NoteHitEffect()
        {
            noteHitAnimator.SetTrigger(hit);
        }

        public void PlayJudgemnetEffect(int judgement)
        {
            judgementEffet[judgement].SetActive(true);
        }
    }
}

