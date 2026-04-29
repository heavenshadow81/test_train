using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FindShapePuzzle
{
    public class CameraMove : MonoBehaviour
    {
        [SerializeField] Transform[] camTransform = null;

        public void MoveCam(int transformIndex = 0, float duration = 2f, System.Action onComplete = null)
        {
            // 카메라의 위치를 타겟 위치로 부드럽게 이동
            transform.DOMove(camTransform[transformIndex].position, duration).SetEase(Ease.InOutQuad);

            // 카메라의 회전도 타겟의 회전으로 부드럽게 변경
            transform.DORotateQuaternion(camTransform[transformIndex].rotation, duration).SetEase(Ease.InOutQuad)
                // 회전이 끝난 후 onComplete 콜백 호출
                .OnComplete(() =>
                {
                    // 이동과 회전이 모두 완료되었을 때 onComplete 콜백 실행
                    onComplete?.Invoke();
                });
        }

        public void MoveCam(float duration, System.Action onComplete)
        {
            MoveCam(0, duration, onComplete);
        }

        public void MoveCam(System.Action onComplete)
        {
            MoveCam(0, 2f, onComplete);
        }
    }
}
