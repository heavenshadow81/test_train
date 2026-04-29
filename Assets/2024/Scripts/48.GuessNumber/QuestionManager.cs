using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GuessNumber
{
    public class QuestionManager : MonoBehaviour
    {
        [SerializeField] Transform[] spawnTransforms = null;
        ShowQuestion question;

        private void Awake()
        {
            question = GetComponent<ShowQuestion>();
            if (question == null)
            {
                Debug.LogError($"{gameObject.name} : QuestionRotate 컴포넌트 없음");
            }
        }
         
        public void SetQuestion()
        {          
            for(int i = 0; i < NumberManager.Instance.answerNumberObjects.Length; i++)
            {
                int randNum = Random.Range(0, NumberManager.Instance.numbers.Length);

                Number spawnNum = NumberManager.Instance.SpawnNumber(randNum, spawnTransforms[i]);

                NumberManager.Instance.answerNumberObjects[i] = spawnNum;
            }

            GameManager.Instance.SetAnswer();

            question.enabled = true;
        }

    }
}
