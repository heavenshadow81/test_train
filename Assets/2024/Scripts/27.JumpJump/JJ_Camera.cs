using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JJ_Camera : MonoBehaviour
{
    public Transform player;  // 플레이어의 Transform을 받아옵니다.
    public float offsetY = 5f; // 카메라가 플레이어와의 높이 차이 (필요시 조정 가능)

    void LateUpdate()
    {
        // 현재 카메라의 위치를 가져옵니다.
        Vector3 cameraPosition = transform.position;

        // 플레이어의 Y 좌표에 오프셋을 더해 카메라의 Y 좌표를 설정합니다.
        cameraPosition.y = player.position.y + offsetY;

        // 카메라의 위치를 업데이트합니다.
        transform.position = cameraPosition;
    }
}
