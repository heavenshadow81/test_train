using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LGM
{
    namespace SheepRevers
    {
        // 당근은 그네한테 양을 생성하게 하고 양은 사망 시 당근을 활성화
        // 당근->양->당근->양->....
        public class Carrot : MonoBehaviour
        {
            public Animator ani;
            protected void Awake()
            {
                ani = GetComponent<Animator>();
            }
            
            // 당근 이벤트
            public void CarrotEvent()
            {
                if (GameManager.Instance.stateClass.state == GameState.GamePlay)
                { 
                    // 오른쪽, 왼쪽 그네 중 랜덤하게 선택해서 "Start Trigger" 재생
                    int random = Random.Range(0, 2);
                    // 타겟 위치(자신 위치) 정보를 그네에게 전달
                    GameManager.Instance.trapezes[random].Trapeze_StartEvent(transform.parent);
                    GameManager.Instance.trapezes[random].ani.SetTrigger("Start Trigger");
                }
            }
        }
    }
}
