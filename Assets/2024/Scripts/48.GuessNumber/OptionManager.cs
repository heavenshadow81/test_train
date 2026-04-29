using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuessNumber
{
    [System.Serializable]
    public class OptionTransform
    {
        public Transform[] numberTransforms = new Transform[4];
    }

    public class OptionManager : MonoBehaviour
    {
        [SerializeField] OptionTransform[] optionTransforms = new OptionTransform[4];
        [SerializeField] Option[] options = new Option[4];
        [SerializeField] List<Number> spawnNumbers = null;

        private ObjectFade fade;

        private void Awake()
        {
            fade = GetComponent<ObjectFade>();
            if (fade == null)
            {
                Debug.LogError($"{gameObject.name} : ObjectFade 컴포넌트 없음");
            }
        }

        public void Setttingoption()
        {
            //SoundMGR.Instance.SoundPlay("Hint");

            // 정답 객체를 랜덤하게 배치할 인덱스를 설정
            int answerIndex = Random.Range(0, optionTransforms.Length);

            // answerNums 배열을 랜덤하게 섞기
            int[] shuffledAnswerNums = (int[])GameManager.Instance.answerNums.Clone();
            ShuffleArray(shuffledAnswerNums);  // 셔플 함수 사용

            int answerCounter = 0; // 배열에서 값을 꺼내 쓸 인덱스

            // 각 optionTransforms에 대해 순회
            for (int i = 0; i < optionTransforms.Length; i++)
            {
                // 정답 객체를 랜덤하게 하나의 optionTransforms에 스폰
                if (i == answerIndex)
                {
                    for (int j = 0; j < optionTransforms[i].numberTransforms.Length; j++)
                    {
                        int randAnswer = shuffledAnswerNums[answerCounter]; // 셔플된 값 사용
                        answerCounter++; // 다음 값으로 넘어감

                        // answerObjects 중 랜덤으로 선택하여 스폰
                        spawnNumbers.Add(NumberManager.Instance.SpawnNumber(randAnswer, optionTransforms[i].numberTransforms[j]));
                    }
                }
                else
                {
                    // 각 numberTransform에 대해 순회하여 랜덤 숫자를 스폰
                    for (int j = 0; j < optionTransforms[i].numberTransforms.Length; j++)
                    {
                        int randNum = Random.Range(0, NumberManager.Instance.numbers.Length);

                        // 랜덤 숫자 프리팹을 스폰
                        spawnNumbers.Add(NumberManager.Instance.SpawnNumber(randNum, optionTransforms[i].numberTransforms[j]));
                    }
                }
            }

            ToggleOptionObject();
        }

        // 배열을 랜덤하게 섞는 함수 (Fisher-Yates 셔플 알고리즘)
        private void ShuffleArray(int[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                int temp = array[i];
                array[i] = array[randomIndex];
                array[randomIndex] = temp;
            }
        }

        private void ToggleOptionObject()
        {
            for (int i = 0; i < options.Length; i++)
            {
                options[i].HideFire();

                // 각 오브젝트의 활성 상태를 반전
                bool isActive = !options[i].gameObject.activeSelf;
                options[i].gameObject.SetActive(isActive);

                // 활성 상태가 true일 때 태그를 "Option"으로 변경
                if (isActive)
                {
                    options[i].gameObject.tag = "Option";
                }
            }
        }

        public void ClearNumber(System.Action onComplete = null)
        {
            int completedCount = 0; // 완료된 숫자 페이드 아웃 개수를 추적
            int totalCount = spawnNumbers.Count;

            for ( int i = 0;i < totalCount; i++)
            {
                // 각 NumberFadeOut의 완료 후 콜백에서 카운트를 증가시킴
                spawnNumbers[i].NumberFadeOut(1f, () =>
                {              
                    completedCount++;
                    if (completedCount == totalCount) // 모든 숫자가 완료되면
                    {
                        ToggleOptionObject();
                        SpawnNumbersReset();
                        onComplete?.Invoke();
                    }
                });
            }
        }

        public void SpawnNumbersReset()
        {
            for( int i = 0; i <  spawnNumbers.Count; i++)
            {
                Destroy(spawnNumbers[i].gameObject);
            }

            spawnNumbers.Clear();
        }

        public void SetOptionUntagged()
        {
            for (int i = 0; i < options.Length; i++)
            {
                options[i].gameObject.tag = "Untagged";
            }
        }

    }
}


