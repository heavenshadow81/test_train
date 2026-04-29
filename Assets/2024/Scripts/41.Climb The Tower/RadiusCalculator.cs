using UnityEngine;

namespace ClimbTheTower
{
    public class RadiusCalculator : MonoBehaviour
    {
        [SerializeField] private Transform[] towerFloors;  // 각 층의 Transform 배열

        private void Start()
        {
            foreach (Transform floor in towerFloors)
            {
                float radius = CalculateRadius(floor);
                Debug.Log($"Floor: {floor.name}, Radius: {radius}");
            }
        }

        // 층의 반지름을 계산하는 함수
        private float CalculateRadius(Transform floor)
        {
            // 해당 층의 MeshRenderer 또는 Collider로부터 Bounds 가져오기
            Renderer renderer = floor.GetComponent<Renderer>();
            if (renderer == null)
            {
                Debug.LogError("Renderer가 존재하지 않습니다.");
                return 0f;
            }

            // Bounds의 크기를 가져와서 반지름 계산
            Bounds bounds = renderer.bounds;
            float radius = bounds.extents.x; // X축을 기준으로 반지름 계산

            return radius;
        }
    }
}
