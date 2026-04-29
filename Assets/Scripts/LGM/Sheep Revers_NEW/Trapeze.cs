using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LGM
{
    namespace SheepRevers
    {

        public class Trapeze : MonoBehaviour
        {
            // 발사할 좌표 예약
            public List<Transform> targetPos = new List<Transform>();
            public GameObject sheepPrefab;  // 양 프리팹
            public Transform createPos; // 양 생성 위치
            public Animator ani;
            private SpriteRenderer rend;

            protected void Awake()
            {
                ani = GetComponent<Animator>();
                rend = GetComponent<SpriteRenderer>();
            }

            // 그네 애니메이션 시작 작동
            public void Trapeze_StartEvent(Transform _pos)   
            {
                targetPos.Add(_pos);
            }
            // 그네 애니메이션 끝날 시 실행할 이벤트
            public void Trapeze_EndEvent()
            {
                GameObject sheep =
                    Instantiate(sheepPrefab, createPos.position, Quaternion.identity);
                // 나중에 생긴 오브젝트가 위로 오도록 레이어 적용
                sheep.GetComponentInChildren<SpriteRenderer>().sortingOrder = GameManager.Instance.number++;
                // 그네와 같은 방향으로 뒤집힌 상태로 생성
                sheep.GetComponentInChildren<SpriteRenderer>().flipX = rend.flipX;
                // 타겟 위치 저장
                sheep.GetComponentInChildren<Sheep>().targetPos = targetPos[0];
                targetPos.RemoveAt(0);  // 저장해둔 위치 삭제
                if (targetPos.Count > 0)
                {
                    ani.SetTrigger("Start Trigger");
                }
            }
        }
    }
}

