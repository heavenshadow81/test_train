using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coding
{
    public class TouchEvent : MonoBehaviour
    {
        // 싱글톤
        static TouchEvent _instant;

        // 외부 수정 불가, 읽기만 가능 하도록
        public static TouchEvent Instance { get => _instant; }

        private int layerMask_Monster = 1 << 9; // Layer 3 == Monster
        private int layerMask_Player_1 = 1 << 6; // Layer 6 == player_1 Character
        private int layerMask_Player_2 = 1 << 7; // Layer 7 == player_2 Character

        // 레이 최대 거리
        private float maxDistance = 200000.0f;

        private void Awake()
        {
            if (!_instant)
            {
                _instant = this;
            }
        }
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // Monster 오브젝트 눌렀을때
                if (Physics.Raycast(ray, out hit, maxDistance, layerMask_Monster))
                {
                    hit.collider.GetComponent<MonsterAction>().MonsterTouch();

                    print("Monster Touch" + hit.transform.gameObject);
                }

                // Character_Player_1 눌렀을때
                if (Physics.Raycast(ray, out hit, maxDistance, layerMask_Player_1))
                {
                    hit.collider.GetComponent<CharacterMove>().CharacterTouch();

                    print("Player_1 Character Touch" + hit.transform.gameObject);
                }

                // Character_Player_2 눌렀을때
                if (Physics.Raycast(ray, out hit, maxDistance, layerMask_Player_2))
                {
                    hit.collider.GetComponent<CharacterMove>().CharacterTouch();

                    print("Player_2 Character Touch" + hit.transform.gameObject);
                }

            }
        }
    }
}

