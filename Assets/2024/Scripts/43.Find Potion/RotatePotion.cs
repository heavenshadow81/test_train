using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FindPotion
{
    public class RotatePotion : MonoBehaviour
    {
        [Header("회전 애니메이션 설정")]
        private float rotationAngle = -360f;  // 회전 각도 (360도)
        private float duration = 36f;         // 한 바퀴 도는데 걸리는 시간 
        private Ease easeType = Ease.Linear;  // 회전 애니메이션의 Ease 설정

        private Tween rotationTween;          // 현재 회전 애니메이션을 추적하기 위한 Tween 변수

        private void OnEnable()
        {
            // 시작할 때 회전 애니메이션을 실행
            RotateClockwise();
        }

        private void OnDisable()
        {
            StopRotation();
        }

        public void RotateClockwise()
        {
            rotationTween = transform.DOLocalRotate(new Vector3(transform.localRotation.x, rotationAngle, transform.localRotation.z), duration, RotateMode.FastBeyond360)
                                      .SetEase(easeType)          // Ease 타입 설정
                                      .SetLoops(-1); // 무한 반복         
        }

        // 회전 애니메이션을 멈추는 함수
        public void StopRotation()
        {
            // 현재 진행 중인 트윈이 있는 경우 해당 트윈을 중지
            if (rotationTween != null && rotationTween.IsActive())
            {
                rotationTween.Kill(); // 트윈을 중지하고 제거
                rotationTween = null; // 변수 초기화
            }
        }
    }
}
