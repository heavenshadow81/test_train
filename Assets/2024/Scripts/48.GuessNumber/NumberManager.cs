using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuessNumber
{
    public class NumberManager : MonoBehaviour
    {
        public static NumberManager Instance;

        public GameObject[] numbers = null;
        public Number[] answerNumberObjects = new Number[4];
        
        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }    
        }

        private void OnDestroy()
        {
            if(Instance != null)
            {
                Instance = null;
            }
        }

        public Number SpawnNumber(int num, Transform spawnTransform)
        {
            GameObject newNum = Instantiate(numbers[num], spawnTransform);
            Number numberComponent = newNum.GetComponent<Number>();

            if (numberComponent != null)
            {
                numberComponent.SetNumber(num); // 숫자를 설정
            }

            return numberComponent;
        }

        public void DestroyNumberObjects()
        {
            for (int i = 0; i < answerNumberObjects.Length; i++)
            {
                Destroy(answerNumberObjects[i]);
            }
        }
    }
}

