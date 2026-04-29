using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookingGame
{
    public class FryingPan : MonoBehaviour
    {
        [Header("기울기 설정")]
        [SerializeField] private float tiltSpeed = 5f;
        [SerializeField] private float maxTiltAngle = 10f;
        [SerializeField] private float tiltChangeInterval = 1f;

        [Header("속도 계산")]
        private Vector3 previousRotation; // 이전 프레임의 로컬 회전
        public Vector3 AngularVelocity { get; private set; } // 각속도 계산
        [SerializeField] private float movementThreshold = 0.1f; // 움직임 판별 임계값
        public bool IsMoving { get; private set; } = false; // 프라이팬의 움직임 상태
        private bool isPlaying = true; 

        private Vector3 targetRotation;
        private Vector3 currentRotation;
        private float nextTiltChangeTime;
        private int lastDirection = -1;

        void Start()
        {
            currentRotation = transform.eulerAngles;
            targetRotation = currentRotation;
            nextTiltChangeTime = Time.time + tiltChangeInterval;
            previousRotation = transform.eulerAngles;
        }

        void Update()
        {
            if(isPlaying)
            {
                ApplyTilt();

                if (Time.time >= nextTiltChangeTime)
                {
                    ChangeTiltDirection();
                    nextTiltChangeTime = Time.time + tiltChangeInterval;
                }

                CalculateAngularVelocity();
                UpdateIsMoving();
            }
        }

        private void CalculateAngularVelocity()
        {
            // 각속도 계산 (이전 회전과 현재 회전의 차이)
            Vector3 currentRotation = transform.eulerAngles;
            Vector3 deltaRotation = currentRotation - previousRotation;

            // 각도 차이를 시간으로 나누어 초당 회전량으로 변환
            AngularVelocity = deltaRotation / Time.deltaTime;

            // 각도 누적값을 업데이트
            previousRotation = currentRotation;
        }

        private void UpdateIsMoving()
        {
            // AngularVelocity의 크기가 임계값 이상일 경우 움직임으로 간주
            IsMoving = AngularVelocity.magnitude > movementThreshold;
        }

        private void ChangeTiltDirection()
        {
            int direction;

            // 이전 방향과 다른 방향이 나올 때까지 반복
            do
            {
                direction = Random.Range(0, 4);
            } while (direction == lastDirection);

            lastDirection = direction;

            switch (direction)
            {
                case 0: TiltUp(); break;
                case 1: TiltDown(); break;
                case 2: TiltLeft(); break;
                case 3: TiltRight(); break;
            }
        }

        private void TiltUp()
        {
            targetRotation.x = Mathf.Clamp(targetRotation.x - maxTiltAngle, -maxTiltAngle, maxTiltAngle);
        }

        private void TiltDown()
        {
            targetRotation.x = Mathf.Clamp(targetRotation.x + maxTiltAngle, -maxTiltAngle, maxTiltAngle);
        }

        private void TiltLeft()
        {
            targetRotation.z = Mathf.Clamp(targetRotation.z - maxTiltAngle, -maxTiltAngle, maxTiltAngle);
        }

        private void TiltRight()
        {
            targetRotation.z = Mathf.Clamp(targetRotation.z + maxTiltAngle, -maxTiltAngle, maxTiltAngle);
        }

        private void ApplyTilt()
        {
            currentRotation = Vector3.Lerp(currentRotation, targetRotation, tiltSpeed * Time.deltaTime);
            transform.eulerAngles = currentRotation;
        }

        public void StopMovement()
        {
            // 움직임 상태를 멈추도록 설정
            isPlaying = false;
            IsMoving = false;
            AngularVelocity = Vector3.zero;

            // 회전 초기화
            targetRotation = Vector3.zero;
            currentRotation = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
        }
    }
}
