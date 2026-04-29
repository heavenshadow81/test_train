using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RowBoat
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform player; // 플레이어를 참조
        private Vector3 offset;   // 카메라와 플레이어 사이의 거리 (오프셋)

        void Start()
        {
            // 카메라와 플레이어 간 현재 오프셋 계산
            offset = transform.position - player.position;
        }

        void LateUpdate()
        {
            // 플레이어의 위치에 오프셋을 더한 위치로 카메라 이동
            Vector3 targetPosition = player.position + offset;
            transform.position = targetPosition;

            // 카메라의 회전을 초기 회전값으로 고정
            //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        }
    }
}
