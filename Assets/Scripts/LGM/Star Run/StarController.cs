using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LGM
{
    namespace StarRun
    {
        public class StarController : Singleton<StarController>
        {
            public GameObject diePrefab;    // 사망 시 효과
            public float speed; // y축 이동 속도
            [HideInInspector]
            public int revers = 1;  // 방향 반전(1이면 위, -1이면 아래로 이동)


            public void playerMoving()
            {
                Vector3 pos = transform.position;
                pos.y += Time.deltaTime * speed * revers;   // speed만큼 y축 이동
                transform.position = pos;
            }
        }
    }
}
