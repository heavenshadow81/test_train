using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGM
{
    namespace KeepFish
    {
        public class FeedEvent : MonoBehaviour
        {
            public GameObject particle; // 소멸 효과

            private void OnTriggerStay2D(Collider2D collision)
            {
                if (collision.CompareTag("Player"))
                {
                    // Player와 충돌 시 파티클 생성
                    Instantiate(particle, transform.position, Quaternion.identity);
                    gameObject.SetActive(false);
                }
            }
        }
    }
}