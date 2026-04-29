using UnityEngine;
using System.Collections.Generic;

namespace ClimbTheTower
{
    [System.Serializable]
    public class FloorPortalPoints
    {
        public Transform[] spawnPoints;  // 각 층의 포탈 스폰 포인트 배열
    }

    public class PortalManager : MonoBehaviour
    {
        [SerializeField] private GameObject portalPrefab;   // 올라가는 포탈 프리팹
        [SerializeField] private Transform[] towerFloors;   // 각 층의 Transform 배열 (1층 ~ 5층)
        [SerializeField] private FloorPortalPoints[] portalSpawnPoints;  // 각 층의 포탈 스폰 포인트 배열
        [SerializeField] private Transform goalPoint;  // 최종 골 포인트 (5층)

        private Dictionary<int, Transform> portalPointsPerFloor;    // 각 층의 포탈 위치
        private Dictionary<int, Transform> teleportPointsPerFloor;  // 각 층의 텔레포트 위치

        private void OnEnable()
        {
            // 각 층마다 포탈과 텔레포트 위치를 저장할 Dictionary 초기화
            portalPointsPerFloor = new Dictionary<int, Transform>();
            teleportPointsPerFloor = new Dictionary<int, Transform>();

            // 각 층마다 포탈과 텔레포트 위치 설정 (마지막 층 제외)
            for (int i = 0; i < towerFloors.Length - 1; i++)
            {
                SetupPortalAndTeleportPoints(i); // 각 층의 포탈과 텔레포트 위치를 설정
            }
        }

        /// <summary>
        /// 각 층에 포탈 생성 위치와 텔레포트 위치를 설정합니다.
        /// </summary>
        /// <param name="currentFloorIndex">현재 층의 인덱스</param>
        private void SetupPortalAndTeleportPoints(int currentFloorIndex)
        {
            Transform[] spawnPoints = portalSpawnPoints[currentFloorIndex].spawnPoints;

            // 1. 랜덤으로 포탈 생성 위치를 선택
            int randomPortalIndex = Random.Range(0, spawnPoints.Length);
            Transform portalPoint = spawnPoints[randomPortalIndex];

            // 2. 포탈 생성 위치를 제외한 나머지 중 랜덤으로 텔레포트 위치 선택
            List<Transform> remainingPoints = new List<Transform>(spawnPoints);
            remainingPoints.RemoveAt(randomPortalIndex); // 포탈 위치를 제외
            int randomTeleportIndex = Random.Range(0, remainingPoints.Count);
            Transform teleportPoint = remainingPoints[randomTeleportIndex];

            // 3. 각 층의 포탈 위치와 텔레포트 위치를 Dictionary에 저장
            portalPointsPerFloor[currentFloorIndex] = portalPoint;
            teleportPointsPerFloor[currentFloorIndex] = teleportPoint;

            // towerFloors[currentFloorIndex]의 Y축 값과 portalPoint의 Y축 값을 비교하여 높이 차이를 계산
            float heightDifference = portalPoint.position.y - towerFloors[currentFloorIndex].position.y;

            // teleportPoint의 Y축을 높이 차이만큼 조정
            teleportPoint.position = new Vector3(
                teleportPoint.position.x,
                teleportPoint.position.y - heightDifference,  // 높이 차이만큼 조정
                teleportPoint.position.z
            );

            // 4. 포탈을 생성하여 해당 위치에 배치
            GameObject portal = Instantiate(portalPrefab, portalPoint);

            //// 부모 오브젝트의 이름이 "Tower3"이고, 2층 이상일 때 포탈의 스케일을 0.1배로 조정
            //if (currentFloorIndex >= 1 && towerFloors[currentFloorIndex].parent != null && towerFloors[currentFloorIndex].parent.name == "Tower_03")
            //{
            //    portal.transform.localScale *= 0.1f;  // 스케일을 0.1배로 줄임
            //}

            portal.GetComponent<Portal>().InitializePortal(this, currentFloorIndex + 1);  // 다음 층의 인덱스를 넘김
        }

        /// <summary>
        /// 특정 층에서 미리 설정된 텔레포트 위치를 반환합니다.
        /// </summary>
        /// <param name="floorIndex">층 인덱스</param>
        public Transform GetTeleportPoint(int floorIndex)
        {
            if (teleportPointsPerFloor.ContainsKey(floorIndex))
            {
                return teleportPointsPerFloor[floorIndex];
            }
            else
            {
                Debug.LogError($"층 {floorIndex}에 텔레포트 포인트가 설정되지 않았습니다.");
                return null;
            }
        }

        public int GetFloorLength()
        {
            return towerFloors.Length;
        }

        public Transform GetGoalPoint()
        {
            return goalPoint; // 최종 골 포인트 반환
        }
    }
}
