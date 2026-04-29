using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuessNumber
{
    public class CameraMove : MonoBehaviour
    {
        [SerializeField] Transform[] camTransform = null;
        private int camStack = 0;

        public void CamMove(System.Action onComplete = null)
        {
            // 배열 범위 내에서만 이동
            if (camStack < camTransform.Length)
            {
                // 카메라의 위치를 타겟 위치로 부드럽게 이동
                transform.DOMove(camTransform[camStack].position, 2f).SetEase(Ease.InOutQuad);

                // 카메라의 회전도 타겟의 회전으로 부드럽게 변경
                transform.DORotateQuaternion(camTransform[camStack].rotation, 2f).SetEase(Ease.InOutQuad)
                    // 회전이 끝난 후 onComplete 콜백 호출
                    .OnComplete(() =>
                    {
                        // 이동과 회전이 모두 완료되었을 때 onComplete 콜백 실행
                        onComplete?.Invoke();
                    });

                camStack++;
            }
            else
            {
                Debug.LogError("더 이상 이동할 위치가 없음");
            }
        }



        // 동굴이 흔들리는 느낌을 주는 카메라 흔들림 효과
        public void CamShake(float duration = 2f, float positionStrength = 0.1f, float rotationStrength = 0.1f, int vibrato = 10, float randomness = 20f)
        {
            // 카메라 위치 흔들림 (미세한 진동)
            transform.DOShakePosition(duration, positionStrength, vibrato, randomness, false, true).SetEase(Ease.InOutQuad);

            // 카메라 회전 흔들림 (회전도 미세하게 흔들리게 설정)
            transform.DOShakeRotation(duration, rotationStrength, vibrato, randomness).SetEase(Ease.InOutQuad);
        }


    }
}
