using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuessNumber
{
    public class Option : MonoBehaviour
    {
        [SerializeField] private int[] optionNumber = new int[4];
        [SerializeField] private GameObject fireEffect;

        private void OnEnable()
        {
            // 자식에서 Number 컴포넌트 배열 가져오기
            Number[] numberComponents = GetComponentsInChildren<Number>();

            // Number 컴포넌트가 있다면 값을 배열에 복사
            if (numberComponents != null && numberComponents.Length > 0)
            {
                for (int i = 0; i < numberComponents.Length && i < optionNumber.Length; i++)
                {
                    optionNumber[i] = numberComponents[i].GetNumber();
                }
            }
        }

        public int[] GetOptionNumber()
        {
            return optionNumber; // 복사한 배열 반환
        }

        public void ShowFire()
        {
            fireEffect.SetActive(true);
        }

        public void HideFire()
        {
            fireEffect.SetActive(false);
        }
    }
}
