using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuessNumber
{
    public class Number : MonoBehaviour
    {
        private ObjectFade fade;
        private int number; // 숫자를 저장할 변수

        private void Awake()
        {
            fade = GetComponent<ObjectFade>();
            if (fade == null)
            {
                Debug.LogError($"{gameObject.name} : ObjectFade 컴포넌트 없음");
            }
        }

        private void OnEnable()
        {
            fade.FadeIn(1f);
        }

        public void NumberFadeOut(float fadeDuration, System.Action onComplete = null)
        {
            fade.FadeOut(fadeDuration, onComplete);
        }

        public void SetNumber(int num) // 숫자를 설정하는 메서드
        {
            number = num; // 넘겨받은 숫자를 저장
        }

        public int GetNumber()
        {
            return number; // 저장된 숫자 반환
        }
    }
}

