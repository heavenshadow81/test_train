using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Settings;
using VirtualTouch;
using static TouchInjection.TouchInjector;

namespace LGM
{
    namespace CraneGame
    {


        public class CraneManager : Singleton<CraneManager>
        {
            public Transform crane; // 크레인 
            public Transform claw;  // 크레인 집게
            public Transform comeBack;  // 크레인이 돌아갈 위치
            public CraneScreen screen;  // 게임 스크린 화면
            public float craneSpeed;    // 크레인 이동 속도

            public AudioSource audioSource; // AudioSource 정보
            public GameObject gameClear;// 게임 클리어 UI
            public GameObject gameOver; // 게임 오버 UI

            // 게임에 사용할 소리
            public Dictionary<AudioType, AudioClip> dicAudio = new Dictionary<AudioType, AudioClip>();
            [HideInInspector]
            public GameObject colliderDoll;   // 잡을 인형(집게와 충돌한 인형)
            [HideInInspector]
            public GameObject gripDoll; // 잡은 인형
            [HideInInspector]
            public bool touchRefuse = false;    // 터치/클릭 잠금 체크(크레인 복귀 및 패배 시 버튼 잠금)

            public Vector3 Crane
            {
                get { return crane.localPosition; } // 그레인 위치 반환
                set { crane.localPosition = value.Clamp(screen.bL, screen.tR); }    // 크레인 위치 이동 범위 제한
            }

            private void Awake()
            {
                InitializeTouchInjection();
                // 게임에 사용될 모든 사운드 저장
                dicAudio.Add(AudioType.BackGround,
                    Resources.Load<AudioClip>("Crane Game/Audio/Back Ground Sound"));
                dicAudio.Add(AudioType.Move,
                    Resources.Load<AudioClip>("Crane Game/Audio/Move Sound"));
                dicAudio.Add(AudioType.Select,
                    Resources.Load<AudioClip>("Crane Game/Audio/Select Sound"));
                dicAudio.Add(AudioType.Complete,
                    Resources.Load<AudioClip>("Crane Game/Audio/Complete Sound"));
                dicAudio.Add(AudioType.GameClear,
                    Resources.Load<AudioClip>("Crane Game/Audio/Game Clear Sound"));
                dicAudio.Add(AudioType.GameOver,
                    Resources.Load<AudioClip>("Crane Game/Audio/Game Over Sound"));
            }

            private void Start()
            {
                screen.ScreenInit();    // 스크린 초기화
            }

            

            public void UpdateLogic()
            {
                // 인형을 전부 뽑을 시 게임 클리어 이벤트 실행
                if (DollManager.Instance.doll.Count <= 0)
                {
                    DollManager.Instance.stateClass.resultState = GameResult.Success;
                    DollManager.Instance.zozo.Change(GameState.GameResult);
                    //TimeManager.Instance.GamEndEvent(gameClear);  // 클리어 UI 활성화
                }
            }

            /// <summary>
            /// 크레인과 겹쳐 있는 인형 정보를 받아옴
            /// </summary>
            /// <param name="collision"></param> 충돌한 오브젝트의 콜라이더 정보
            private void OnTriggerStay2D(Collider2D collision)
            {
                if (collision.CompareTag("Target Point"))
                {
                    // 잡았던 인형(colliderDoll)이 있을 경우 인형의 orderLayer값 저장 / 없을 시 -1(없음)
                    int beforeDoll = colliderDoll != null ? colliderDoll.GetSpriteOrderLayer() : -1;
                    int newDoll = collision.gameObject.Parent().GetSpriteOrderLayer();
                    // 인형이 겹쳐있을 시 Order Layer값 이 큰 오브젝트를 우선 저장
                    if (beforeDoll <= newDoll)
                    {
                        colliderDoll = collision.gameObject.Parent();
                    }
                }
                else
                {
                    colliderDoll = null;
                }
            }
        }
    }
}