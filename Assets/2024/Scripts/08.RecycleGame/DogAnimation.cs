using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using DG.Tweening;

namespace RecycleGame
{
    public class DogAnimation : MonoBehaviour
    {
        Vector3 startPosition;
        Vector3 endPosition;
        public GameObject endObj;

        // Update is called once per frame
        void OnEnable()
        {
            startPosition = gameObject.transform.position;
            endPosition = endObj.transform.position;

            GoEnd();
        }

        void GoEnd()
        {
            gameObject.transform.DOMove(endPosition, 10).SetEase(Ease.Linear).OnComplete(GoStart);
            gameObject.transform.DOScaleX(1, 0.1f);
        }

        void GoStart()
        {
            gameObject.transform.DOMove(startPosition, 10).SetEase(Ease.Linear).OnComplete(GoEnd);
            gameObject.transform.DOScaleX(-1, 0.1f);
        }
    }
}