using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LGM
{
    namespace KeepFish
    {
        public class Fish : MonoBehaviour
        {
            [HideInInspector]
            public Animator ani;
            public List<SpriteRenderer> parts = new List<SpriteRenderer>(); // 물고기 각 부위 이미지 정보

            private void Awake()
            {
                ani = GetComponent<Animator>();
                parts = GetComponentsInChildren<SpriteRenderer>().ToList();
            }
            // 물고기의 모든 파츠 뒤집기(방향 전환)
            public void FishFlip(bool flip)
            {
                foreach(SpriteRenderer part in parts)
                {
                    part.flipX = flip;
                }
            }
        }
    }
}

