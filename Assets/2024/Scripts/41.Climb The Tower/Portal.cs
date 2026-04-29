using ClimbTheTower;
using UnityEngine;

namespace ClimbTheTower
{
    public class Portal : MonoBehaviour
    {
        private PortalManager portalManager; // PortalManager를 참조
        private int targetFloorIndex; // 다음 층의 인덱스

        public void InitializePortal(PortalManager manager, int targetIndex)
        {
            portalManager = manager;
            targetFloorIndex = targetIndex; // 다음 층의 인덱스 설정
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Player player = other.GetComponent<Player>();

                // 포탈로 이동
                if (player != null)
                {
                    // 포탈에 진입한 후 애니메이션과 이동 처리
                    player.PerformPortalTransition(GetDestinationPosition());
                }
            }
        }

        // 캐릭터가 이동할 목적지 포지션을 반환하는 함수
        private Vector3 GetDestinationPosition()
        {
            // 5층(마지막 층)으로 이동할 때는 골 포인트로 이동
            if (targetFloorIndex == portalManager.GetFloorLength() - 1)
            {
                Transform goalPoint = portalManager.GetGoalPoint();
                return goalPoint.position; // 골 포인트의 위치 반환
            }
            else
            {
                // PortalManager에서 미리 설정된 텔레포트 포인트 가져오기
                Transform teleportPoint = portalManager.GetTeleportPoint(targetFloorIndex);

                if (teleportPoint != null)
                {
                    return teleportPoint.position; // 미리 설정된 텔레포트 포인트 반환
                }
                else
                {
                    Debug.LogError("이동 가능한 텔레포트 스폰 포인트가 없습니다.");
                    return transform.position; // 오류 시 현재 위치를 반환
                }
            }
        }
    }
}
