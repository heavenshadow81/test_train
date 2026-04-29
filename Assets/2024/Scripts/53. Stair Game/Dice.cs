using DG.Tweening;
using System;
using UnityEngine;

namespace StairGame
{
    public class Dice : MonoBehaviour
    {
        private float rollDuration = 0.5f;
        private Vector3 initialOffset;
        public void RollUp(Action onComplete = null)
        {
            transform.DORotate(new Vector3(0, 0, 90), rollDuration, RotateMode.WorldAxisAdd).OnComplete(() => onComplete?.Invoke());
            SoundMGR.Instance.SoundPlay("Roll");
        }

        public void RollDown(Action onComplete = null)
        {
            transform.DORotate(new Vector3(0, 0, -90), rollDuration, RotateMode.WorldAxisAdd).OnComplete(() => onComplete?.Invoke());
            SoundMGR.Instance.SoundPlay("Roll");
        }

        public void RollLeft(Action onComplete = null)
        {
            transform.DORotate(new Vector3(0, 90, 0), rollDuration, RotateMode.WorldAxisAdd).OnComplete(() => onComplete?.Invoke());
            SoundMGR.Instance.SoundPlay("Roll");
        }

        public void RollRight(Action onComplete = null)
        {
            transform.DORotate(new Vector3(0, -90, 0), rollDuration, RotateMode.WorldAxisAdd).OnComplete(() => onComplete?.Invoke());
            SoundMGR.Instance.SoundPlay("Roll");
        }

        public void InitializeOffset(Vector3 playerPosition)
        {
            initialOffset = transform.parent.position - playerPosition;
        }

        public void MoveDice(Vector3 targetPos, Action onComplete = null)
        {
            transform.parent.DOMove(initialOffset + targetPos, 1.5f).OnComplete(() => onComplete?.Invoke());
        }
    }
}
