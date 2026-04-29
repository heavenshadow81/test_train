using GuessNumber;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StairGame
{
    public class Stair : MonoBehaviour
    {
        [SerializeField] GameObject smokeEffect;

        private void OnEnable()
        {
            smokeEffect.SetActive(true);
        }
    }
}
