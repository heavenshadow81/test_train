using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    /// <summary>
    /// 이전에 사용자가 생성한 물고기 오브젝트를 미리 생성시키는 컴포넌트.
    /// </summary>
    public class SpawnRecentAToolFishesOnLoad : MonoBehaviour
    {
        public void Start()
        {
            ResourceManager.IdentifierInfo[] identifiers = ResourceManager.GetRecentAToolFishIdentifiers(-1, createFishesObj.MAX_FISHES);
            print($"물고기 불러온 갯수{identifiers.Length}");
            for(int i = 0; i < identifiers.Length; i++)
            {
                var info = identifiers[i];
                GameObject fish = createFishesObj.Instance().SetFishesPath(info.userId, info.identifier, "", Vector3.zero, exAtoolFath.InitMode.Swim);
                if(fish != null)
                {
                    // 랜덤 위치 지정
                    fish.transform.position = transform.position + Random.insideUnitSphere * 140.0f;

                    // PathExample 붙이기
                    fish.AddComponent<PathExample>();
                }
            }
        }
    }
}