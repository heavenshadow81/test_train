using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  // DOTween을 사용하기 위한 네임스페이스

namespace GuessNumber
{
    public class ShowQuestion : MonoBehaviour
    {
        [SerializeField] private float duration = 5f; // 한 번의 회전 시간
        [SerializeField] private float disableDelay = 2f; // 비활성화 지연 시간
        [SerializeField] private OptionManager optionManager;

        private DOTween rotateTween;

        private void OnEnable()
        {
            SoundMGR.Instance.SoundPlay("Hint");

            // Z축을 기준으로 계속 회전 (360도씩 반복)
            transform.DORotate(new Vector3(0, 0, 360), duration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)  // 선형 회전
                .SetLoops(-1);  // 무한 반복

            // 3초 후에 게임 오브젝트 비활성화
            StartCoroutine(FadeOutNumbersAfterDelay(disableDelay, DisableNumber));
        }

        private IEnumerator FadeOutNumbersAfterDelay(float delay, System.Action onComplete = null)
        {
            yield return new WaitForSeconds(delay);

            int completedCount = 0; // 완료된 숫자 페이드 아웃 개수를 추적
            int totalCount = NumberManager.Instance.answerNumberObjects.Length;

            for (int i = 0; i < totalCount; i++)
            {
                // 각 NumberFadeOut의 완료 후 콜백에서 카운트를 증가시킴
                NumberManager.Instance.answerNumberObjects[i].NumberFadeOut(1f, () =>
                {
                    completedCount++;
                    if (completedCount == totalCount) // 모든 숫자가 완료되면
                    {
                        onComplete?.Invoke(); // onComplete 호출 (DisableNumber)
                    }
                });
            }
        }


        private void DisableNumber()
        {       
            // 회전 애니메이션 중지
            transform.DOKill();

            // 회전값을 0으로 설정
            transform.rotation = Quaternion.identity;

            // 오브젝트 파괴
            NumberManager.Instance.DestroyNumberObjects();

            // 선택지 보여주기
            optionManager.Setttingoption();

            // 컴포넌트 비활성화
            this.enabled = false;
        }

        private void Update()
        {
            // 자식 오브젝트들의 회전을 조정
            foreach (Transform child in transform)
            {
                // 월드 기준으로 자식 오브젝트들의 Up 방향을 고정
                child.up = Vector3.up;
            }
        }
    }
}
