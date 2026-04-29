using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEditor;

namespace LGM
{
    namespace SheepRevers
    {
        public struct JumpInfo
        {
            public float jumpPower; // 점프할때 적용할 힘
            public int jumpCount;   // 점프 횟수
            public float duration;  // 체공 시간
            public JumpInfo(float _jumpPower, int _jumpCount, float _duration)
            {
                jumpPower = _jumpPower;
                jumpCount = _jumpCount;
                duration = _duration;
            }
        }
        public class Sheep : MonoBehaviour
        {
            public Transform targetPos;     // 날아갈 타겟 위치
            public AudioClip successClip;   // 성공 사운드

            private bool isExitOut = false; // 양이 퇴장 가능한지 체크
            private bool isStop = false;    // 사망 시 이벤트 실행 안되도록 체크
            private bool isSuccessSound = false;    // 성공 사운드 한번만 실행되도록 체크
            private float power;    // 양이 힘주는 지속 시간
            private JumpInfo jumpInfo = new JumpInfo(3, 1, 1);  // 점프 정보
            private Health hp;  // Hp 정보
            private AudioSource audioSource;    // AudioSource 정보
            private SpriteRenderer rend;    // SpriteRenderer 정보
            private GameManager manager;    // GameManager 정보
            private GameObject carrot;      // -----

            private Animator ani;
            public Animator Ani
            {
                get
                {
                    if (ani == null)
                    {
                        ani = GetComponentInChildren<Animator>();
                    }
                    return ani;
                }
            }

            private void Awake()
            {
                manager = GameManager.Instance;
                hp = GetComponentInChildren<Health>();
                audioSource = GetComponent<AudioSource>();
                rend = GetComponent<SpriteRenderer>();
            }
            
            private void Update()
            {

                if (isExitOut)  // 퇴장 시 이벤트
                {
                    ExitMoveOut(rend.flipX);    // 퇴장 시 flip에 따른 방향으로 퇴장
                }
                if (!isStop && hp.Hp >= hp.hp.Count)    // 정지 상태가 아니면서 혈압(HP)이 최대치면 사망
                {
                    DieEvent(); // 사망 이벤트
                }
                else if (!isStop && hp.Hp <= 0)     // 정지 상태가 아니면서 혈압이 0이하면 성공
                {
                    SuccessEvent(); // 성공 이벤트
                }
                Pull_Animation();   // 당근 뽑기 이벤트
            }
            
            // 클릭 이벤트
            public void ClickEvent()
            {
                if (!audioSource.isPlaying) // audio가 실행하지 않을 시 재생
                {
                    audioSource.Play();
                }
                hp.Hp -= 1; // Hp 변수는 프로퍼티로 생성되서 감소식만 사용해도 Hp Sprite가 자동으로 변경
                power = 1;  // 당근 뽑는 힘
            }

            // 당근 뽑기 이벤트
            private void Pull_Animation()
            {
                // power가 0일땐 혈압 상승 애니메이션 실행
                // power가 0보다 큰 동안 당근 뽑기 애니메이션 실행 
                power -= Time.deltaTime;
                power = Mathf.Clamp(power, 0, 1);
                Ani.SetFloat("Power", power);
            }
            // 혈압을 전부 낮췄을때 이벤트 (성공 이벤트)
            private void SuccessEvent()
            {
                isStop = true;  // 이벤트 실행 잠금
                isExitOut = true;   // 퇴장 이벤트
                manager.score++;    // 훔친 당근 갯수(점수) 증가
                Ani.SetTrigger("Success Trigger");  // 애니메이션 실행
                DestroyEvent(); // 삭제 이벤트
            }
            // flip에 따른 방향으로 이동
            private void ExitMoveOut(bool flip)
            {
                Vector3 temp = transform.position;
                // flip이 true면 오른쪽, false면 왼쪽으로 이동
                temp.x += Time.deltaTime * (5f * (flip ? 1 : -1));
                transform.position = temp;
            }
            // 양 사망 이벤트 (실패 이벤트)
            private void DieEvent()
            {
                isStop = true;  // 사망 및 성공이 한번만 실행되도록 isStop 변수를 true로 변경
                Ani.SetTrigger("Die Trigger");  // Die Trigger 애니메이션 재생
                rend.DOFade(0, 2.5f);   // 점차 투명해지도록 DOFade함수 사용
                if (manager.life.Count > 0) // 목숨이 남아 있으면 감소
                    manager.MinusLife();
                DestroyEvent();
            }
            private void DestroyEvent()
            {
                // UI 및 콜라이더 비활성화
                hp.hpBar.gameObject.SetActive(false);
                GetComponent<Collider2D>().enabled = false; // 시체는 클릭되지 않도록 설정
                manager.ActiveCarrot(); // 양이 사라질 시 게임이 계속 돌아갈 수 있도록 다음 당근 활성화
                // 3초 뒤에 자신 오브젝트 삭제
                Destroy(transform.parent.gameObject, 3f);
            }
            // 타겟 위치로 날아가는 이벤트 함수
            public void FlyEvent()
            {
                // targetPos.position 위치로 포물선 이동
                transform.parent.DOJump(targetPos.position, 
                    jumpInfo.jumpPower, jumpInfo.jumpCount, jumpInfo.duration);
            }

            // 당근 비활성화(성공, 실패 시 실행)
            public void ActiveFalseCarrot()
            {
                targetPos.gameObject.SetActive(false);
            }

            // 성공 사운드
            public void SeccessSound()
            {
                // 한번만 실행
                if (!isSuccessSound)
                {
                    isSuccessSound = true;
                    audioSource.PlayOneShot(successClip);   // 성공 사운드 출력
                }
            }
        }
    }
}

