using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KartRider
{
    public class ParticleFollowParent : MonoBehaviour
    {
        public Transform parentTransform; // 부모의 Transform
        public ParticleSystem ParticleSystem; // 파티클 시스템

        void Start()
        {
            // 부모 Transform 할당
            parentTransform = transform.parent;

            // 파티클 시스템 할당
            ParticleSystem = GetComponent<ParticleSystem>();

            if (parentTransform == null)
            {
                Debug.LogError("부모 오브젝트가 설정되지 않았습니다.");
            }

            if (ParticleSystem == null)
            {
                Debug.LogError("ParticleSystem이 할당되지 않았습니다.");
            }
        }

        void LateUpdate()
        {
            if (parentTransform != null && ParticleSystem != null)
            {
                // 파티클 시스템의 위치를 부모 위치로 동기화
                transform.localPosition = parentTransform.localPosition;

                // 파티클 시스템의 회전을 부모 회전으로 동기화
                transform.localRotation = parentTransform.localRotation;
            }
        }
    }
}
