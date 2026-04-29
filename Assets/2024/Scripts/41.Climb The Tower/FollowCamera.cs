using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClimbTheTower
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Player player;  // 따라갈 캐릭터 (플레이어)
        [SerializeField] Vector3 offset;    // 카메라와 캐릭터 사이의 거리 (오프셋)
        private float smoothSpeed = 10f;    // 카메라 이동 속도 (부드러움)
        [SerializeField] private bool isCameraActive  = false; // 카메라가 플레이어의 회전을 따라갈지 여부

        void LateUpdate()
        {
            if (isCameraActive)
            {
                // 목표 위치 (캐릭터 위치 + 오프셋)
                Vector3 desiredPosition;

                // 플레이어의 로컬 회전 기준으로 카메라 위치를 설정
                desiredPosition = player.transform.position + player.transform.rotation * offset;
                // 현재 위치에서 목표 위치로 부드럽게 이동
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

                // 카메라 위치를 부드럽게 업데이트
                transform.position = smoothedPosition;

                // 카메라가 항상 플레이어를 바라보도록 설정
                transform.LookAt(player.transform.position + new Vector3(0, 3, 0));  // 플레이어 위치를 약간 위로 바라보도록 설정
            }
        }

        public void FollowCameraOnOff(bool isOn)
        {
            isCameraActive  = isOn;
        }
    }
}
