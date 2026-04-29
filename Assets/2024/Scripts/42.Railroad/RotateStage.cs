using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Railroad
{
    public class RotateStage : MonoBehaviour
    {
        [Header("회전 애니메이션 설정")]
        private float rotationAngle = -360f;  // 회전 각도 (360도)
        private float duration = 72f;         // 한 바퀴 도는데 걸리는 시간 (기본값: 72초)
        private Ease easeType = Ease.Linear; // 회전 애니메이션의 Ease 설정 (기본값: Linear)

        private Tween rotationTween; // 현재 회전 애니메이션을 추적하기 위한 Tween 변수

        private void Start()
        {
            // 시작할 때 회전 애니메이션을 실행
            RotateClockwise();
        }

        // 시계 방향으로 회전시키는 함수
        public void RotateClockwise()
        {
            // 현재 객체를 지정된 각도만큼 시계 방향으로 무한히 회전 (Y축 기준)
            rotationTween = transform.DORotate(new Vector3(0, rotationAngle, 0), duration, RotateMode.FastBeyond360)
                                      .SetEase(easeType)          // Ease 타입 설정
                                      .SetLoops(-1, LoopType.Incremental); // 무한 반복
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

        // 충격받은 느낌의 애니메이션 함수
        public void SpawnRailAnimation(float scaleAmount = -0.2f, float duration = 0.1f, int vibrato = 1, float elasticity = 1f)
        {
            // 현재 스케일 값을 기준으로 커졌다가 다시 작아지는 효과
            transform.DOPunchScale(new Vector3(scaleAmount, scaleAmount, scaleAmount), duration, vibrato, elasticity)
                     .SetEase(Ease.OutBounce); // 바운스 효과를 적용하여 충격을 받은 느낌으로 설정
        }
    }
}
