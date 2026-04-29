using System.Collections;
using UnityEngine;

namespace KartRider
{
    public class DirectionSetter : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform targetPoint; // 도착 지점
        [SerializeField] private float rotationDuration = 3f; // 회전 지속 시간 (초)
        [SerializeField] private float angleOffset = 15f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("KartBody")) // "KartBody" 태그를 가진 객체만 처리
            {
                Transform kartTransform = other.transform.parent.transform;

                // 트리거된 순간, 이동 및 회전 코루틴 실행
                StartCoroutine(MoveAndRotateKart(kartTransform));
            }
        }

        private IEnumerator MoveAndRotateKart(Transform kartTransform)
        {
            // 1. 목표 방향 계산
            Vector3 direction = (targetPoint.position - kartTransform.position).normalized;
            Quaternion initialRotation = kartTransform.rotation; // 현재 회전값
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up); // 목표 회전값

            // 2. 목표 회전 값의 x축에 -20도 추가
            Vector3 targetEulerAngles = targetRotation.eulerAngles; // 목표 회전값의 EulerAngles 추출
            targetEulerAngles.x -= angleOffset;
            targetRotation = Quaternion.Euler(targetEulerAngles); // 수정된 EulerAngles로 목표 회전 재생성

            float elapsedTime = 0f;

            // 3. 회전(기울기) 부드럽게 조정
            while (elapsedTime < rotationDuration)
            {
                // Slerp로 부드럽게 회전
                kartTransform.rotation = Quaternion.Slerp(
                    initialRotation,
                    targetRotation,
                    elapsedTime / rotationDuration // 경과 시간 비율에 따라 회전
                );

                elapsedTime += Time.deltaTime;
                yield return null; // 한 프레임 대기
            }

            // 최종적으로 목표 회전값에 정확히 맞춤
            kartTransform.rotation = targetRotation;
        }
    }
}
