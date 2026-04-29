using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FindShapePuzzle
{
    public class CandySpawner : MonoBehaviour
    {
        [SerializeField] GameObject candy;

        private void OnEnable()
        {
            SoundMGR.Instance.SoundPlay("Candy");
            candy.transform.DOLocalMoveY(candy.transform.localPosition.y + 10f, 3f);
        }
    }
}
