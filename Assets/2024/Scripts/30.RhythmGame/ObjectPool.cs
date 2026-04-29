using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RhythmGame
{
    // 직렬화 가능한 클래스. 오브젝트 풀에 사용할 정보(프리팹, 개수, 부모 트랜스폼)를 담고 있음.
    [System.Serializable]
    public class ObjectInfo
    {
        public GameObject prefab;  // 생성할 오브젝트의 프리팹
        public int count;          // 풀에 넣을 오브젝트의 수
        public Transform poolParent; // 풀에 생성된 오브젝트를 배치할 부모 트랜스폼
    }

    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] ObjectInfo[] objectInfos = null; // 풀에 사용할 오브젝트 정보 배열

        public Queue<GameObject> noteQueue = new Queue<GameObject>(); // 큐를 사용하여 오브젝트 풀을 관리

        private void Start()
        {
            noteQueue = InsertQueue(objectInfos[0]); // 첫 번째 ObjectInfo 데이터를 이용해 큐에 오브젝트를 삽입
        }  

        // ObjectInfo 데이터를 받아 해당 개수만큼 오브젝트를 생성하고 큐에 삽입하는 함수
        Queue<GameObject> InsertQueue(ObjectInfo objectInfo)
        {
            Queue<GameObject> t_Queue = new Queue<GameObject>(); // 새로운 큐 생성
            for (int i = 0; i < objectInfo.count; i++) // 지정된 개수만큼 오브젝트 생성
            {
                // 프리팹을 인스턴스화하여 위치와 회전 설정 후 비활성화
                GameObject note = Instantiate(objectInfo.prefab, transform.position, Quaternion.identity);
                note.SetActive(false); // 풀에서 사용되기 전까지는 비활성화

                // poolParent가 있으면 해당 부모로 설정, 없으면 현재 오브젝트의 트랜스폼에 설정
                if (objectInfo.poolParent != null)
                {
                    note.transform.SetParent(objectInfo.poolParent, false);
                }
                else
                {
                    note.transform.SetParent(this.transform, false);
                }

                t_Queue.Enqueue(note); // 생성된 오브젝트를 큐에 추가
            }

            return t_Queue; // 큐 반환
        }
    }
}
