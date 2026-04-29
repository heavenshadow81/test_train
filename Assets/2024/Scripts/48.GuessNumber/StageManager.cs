using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuessNumber
{
    public class StageManager : MonoBehaviour
    {
        public QuestionManager questionManager;
        public OptionManager optionManager;
        [SerializeField] Statue statue;
        [SerializeField] Gate gate;
        private List<GameObject> answerObjects = null;

        public void GoToNextStage(System.Action onComplete = null)
        {
            statue.fade.enabled = true;
            gate.fade.enabled = true;

            statue.StatueFadeOut(1f, () => gate.GateFadeOut(1f, onComplete));

            GameManager.Instance.stage++;
        }

        public void SetAnswerObject(GameObject answerObject, int answerIdx)
        {
            // answerObject를 statue.GetAnswerTransform(answerIdx) 위치에 복사
            GameObject cloneAnswer = Instantiate(answerObject, statue.GetAnswerTransform(answerIdx));

            // cloneAnswer를 부모 내에서 (0, 0, 0) 위치로 설정 (부모의 로컬 좌표계에서)
            cloneAnswer.transform.localPosition = Vector3.zero;

            cloneAnswer.SetActive(false); // 생성된 오브젝트를 바로 비활성화

            if (answerObjects == null)
            {
                answerObjects = new List<GameObject>(); // 리스트 초기화
            }

            answerObjects.Add(cloneAnswer);
        }


        // answerObjects를 순차적으로 활성화하는 함수
        public void ShowAnswers(System.Action onComplete = null)
        {
            StartCoroutine(ActivateObjectsCoroutine(onComplete));
        }

        private IEnumerator ActivateObjectsCoroutine(System.Action onComplete = null)
        {
            foreach (GameObject answerObject in answerObjects)
            {
                SoundMGR.Instance.SoundPlay("ShowAnswer");
                answerObject.SetActive(true); // 오브젝트 활성화
                yield return new WaitForSeconds(1.5f); // 1초 대기
            }

            // 모든 오브젝트가 활성화된 후 콜백 호출
            onComplete?.Invoke(); // 콜백이 있으면 호출
        }

    }
}

