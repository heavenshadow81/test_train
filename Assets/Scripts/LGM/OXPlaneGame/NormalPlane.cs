using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LGM
{
    namespace OXPlaneGame
    {
        public class NormalPlane : PlaneEvent
        {
            public AudioClip missClip;  // 오답 사운드
            public AudioClip rightClip; // 정답 사운드
            private Animator ani;
            public bool isTrap = false; // 함정 설치 유무
            public bool isKey = false;

            protected override void Awake()
            {
                base.Awake();
                ani = GetComponent<Animator>();
            }
            protected override void PlaneDownEvent()
            {
                scaleClick.Restart();
                if (!GameManager.Instance.clickRock)
                {
                    base.PlaneDownEvent();
                    if (isKey)
                    {
                        OpenDoor();
                        PlaneEvent();
                    }
                    else if (isTrap) // 함정일 경우
                    {
                        TrapEvent();
                    }
                    else // 일반(정답) 발판 이벤트
                    {
                        PlaneEvent();
                    }
                }
            }
            public void PlaneEvent()    // 발판(정답) 이벤트
            {
                PlaneShotAudio(rightClip);  // 정답 사운드 출력
                // 자기 라인의 함정 제거
                NormalPlane trapPlane = manager.trapPlane[id.y];
                trapPlane.ani.SetTrigger("Sink_Trigger");
                if (manager.map.Count < id.y + 1)
                {
                    return;
                }
                manager.map[id.y + 1].LineActive(true);
            }
            public void TrapEvent() // 함정 이벤트
            {
                // 목숨 감소
                LifeManager.Instance.LifeDelete();
                ani.SetTrigger("Sink_Trigger");
                PlaneShotAudio(missClip);
            }

            // 문 이벤트
            public void OpenDoor()
            {
                // 클릭 가능 여부(게임 종료 체크)
                if (!GameManager.Instance.clickRock)
                {
                    GameManager.Instance.getKey = true; // 키 획득 체크
                    GameManager.Instance.doorObj.SetActive(false);  // 문 삭제
                    //GameManager.Instance.keyPrefab.SetActive(true); // 키 활성화
                }
            }

            public void TrapClear()
            {
                if (isTrap == true)
                {
                    List<Line> map = GameManager.Instance.map;
                    map[id.y].Clear(id.x);
                }
            }
        }
    }
}

