using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KartRider
{
    public class SequentialButton : MonoBehaviour
    {
        TextMeshProUGUI displayText; // 현재 문자를 표시할 텍스트(UI)
        private char currentChar = 'a'; // 현재 문자 초기값

        private void Start()
        {
            displayText = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        public void OnButtonClicked()
        {
            // 현재 문자 변경
            if (currentChar < 'Z')
            {
                currentChar++;
            }
            else
            {
                currentChar = 'A'; // 'Z'를 넘으면 다시 'A'로
            }

            // 텍스트 업데이트
            displayText.text = currentChar.ToString();
        }
    }
}
