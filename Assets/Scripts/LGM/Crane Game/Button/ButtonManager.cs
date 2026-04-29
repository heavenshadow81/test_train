using DG.Tweening;
using LGM.SheepRevers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LGM
{
    namespace CraneGame
    {


        public class ButtonManager : Singleton<ButtonManager>
        {
            public ButtonType type; // 현재 버튼
            private CraneManager cManager;  // 크레인 매니저
            private Vector3 mySclae;    // 크레인 기존 크기 저장
            [HideInInspector]
            public bool isComeback = false; // 크레인 돌아오기 이벤트 중인지 체크

            private void Awake()
            {
                cManager = CraneManager.Instance;
            }

            private void Update()
            {
                // 터치/클릭이 가능한 상황에 type에 따른 이벤트 처리
                if (!cManager.touchRefuse)
                {
                    switch (type)
                    {
                        case ButtonType.Top:
                            Move_Button(Vector3.up);
                            break;
                        case ButtonType.Bottom:
                            Move_Button(Vector3.down);
                            break;
                        case ButtonType.Left:
                            Move_Button(Vector3.left);
                            break;
                        case ButtonType.Right:
                            Move_Button(Vector3.right);
                            break;
                        case ButtonType.Select:
                            Select_Button_Start();
                            break;
                    }
                }
                if (isComeback)
                {
                    Audio_Play(cManager.dicAudio[AudioType.Move]);
                }
            }
            public void Select_Button_Start()
            {
                cManager.touchRefuse = true;    // 키 입력 잠금
                mySclae = cManager.claw.localScale; // 기존 크기 저장
                Audio_Play(cManager.dicAudio[AudioType.Select]);
                Sequence dotEvent = DOTween.Sequence().SetAutoKill(true).Pause();
                dotEvent.Append(cManager.claw.DOScale(0.5f, 2f).SetEase(Ease.Linear));
                // 1초 대기 후 select_Button_Up() 실행
                dotEvent.AppendInterval(1f).OnComplete(() => { select_Button_Up(); });
                dotEvent.Restart();
            }
            public void select_Button_Up()
            {
                GripDollEvent();    // 인형 잡기 이벤트
                Audio_Play(cManager.dicAudio[AudioType.Select]);
                Sequence dotEvent = DOTween.Sequence().SetAutoKill(true).Pause();
                dotEvent.Append(cManager.claw.DOScale(mySclae, 2f).SetEase(Ease.Linear)).OnComplete(() =>
                {
                    Select_Button_ComeBackY();
                });
                dotEvent.Restart();
            }
            public void Select_Button_ComeBackY()
            {
                if (cManager.gripDoll != null && cManager.gripDoll.TryGetComponent(out GripPoint _obj))
                {
                    if (_obj.PossibilityGripDoll()) // 랜덤 확률로 인형 잡기
                    {
                        MissDollEvent();    // 놓쳤을때 이벤트
                    }
                }
                isComeback = true;
                Sequence dotEvent = DOTween.Sequence().SetAutoKill(true).Pause();
                dotEvent.Append(cManager.crane.DOLocalMoveY(cManager.comeBack.position.y, 1.5f).SetEase(Ease.Linear));
                dotEvent.AppendInterval(0.1f).OnComplete(() =>
                {
                    Select_Button_ComeBackX();
                });
                dotEvent.Restart();
            }
            public void Select_Button_ComeBackX()
            {
                Audio_Play(cManager.dicAudio[AudioType.Move]);
                Sequence dotEvent = DOTween.Sequence().SetAutoKill(true).Pause();
                dotEvent.Append(cManager.crane.DOLocalMoveX(cManager.comeBack.position.x, 1.5f).SetEase(Ease.Linear)).OnComplete(() =>
                {
                    Select_Button_Check();
                });
                dotEvent.Restart();
            }
            public void Select_Button_Check()
            {
                isComeback = false;
                cManager.touchRefuse = false;    // 키 입력 잠금 해제
                cManager.gripDoll.transform.SetParent(null);
                cManager.gripDoll.transform.DOScale(0, 0.5f).SetEase(Ease.InCubic).OnComplete(() =>
                {
                    DollManager.Instance.DestroyDoll(cManager.gripDoll);
                    // 타겟이랑 같은걸 뽑았는지 체크
                    if (DollManager.Instance.target.sprite == cManager.gripDoll.GetSpriteRenderer().sprite)
                    {
                        ScoreManager.Instance.score += ScoreManager.Instance.oneScore;
                        SuccessEvent(); // 성공 이벤트
                    }
                });
            }
            // dir에 따른 이동 및 사운드 재생
            public void Move_Button(Vector3 dir)
            {
                // 크레인 이동
                cManager.Crane +=
                    dir * Time.deltaTime * cManager.craneSpeed;
                Audio_Play(cManager.dicAudio[AudioType.Move]);  // 오디오 재생
            }
            public void Audio_Play(AudioClip clip)
            {
                cManager.audioSource.clip = clip;   // 사운드 클립 변경
                if (!cManager.audioSource.isPlaying)// 기존 사운드가 전부 재생된후 재생되도록 체크
                {
                    cManager.audioSource.Play();
                }
            }

            // 인형 집기 이벤트
            public void GripDollEvent()
            {
                // 인형 잡기는 집게랑 인형이 겹쳤을 때만 실행
                if (cManager.colliderDoll != null)
                {
                    cManager.colliderDoll.transform.parent = cManager.crane; // 잡을 인형을 집게 오브젝트의 자식으로 등록
                    cManager.gripDoll = cManager.colliderDoll;   // 잡을 인형에서 -> 잡은 인형으로 변경
                    float beforeSize = cManager.gripDoll.transform.localScale.x;    // 돌아올떄를 대비해 기존 크기 저장
                    cManager.gripDoll.transform.DOScale(beforeSize + 0.3f, 2f).SetEase(Ease.Linear);    // 기존 크기보다 0.3 커지도록 설정
                    cManager.gripDoll.SetSpriteOrderLayer(19);  // 잡은 인형의 레이어를 앞으로 이동
                }
            }
            // 성공 이벤트
            private void SuccessEvent()
            {
                DollManager.Instance.SetRandomTarget(); // 성공 시 다음에 뽑을 인형 선택
            }
            // 인형 놓쳤을 시 이벤트
            private void MissDollEvent()
            {
                cManager.gripDoll.transform.DOScale(1, 0.5f).SetEase(Ease.OutBounce);   // 인형 크기 복구
                cManager.gripDoll.transform.SetParent(null);    // 집게의 자식객체에서 벗어남
                cManager.gripDoll = null;   // 잡은 오브젝트 null로 변경
            }
        }
    }
}
