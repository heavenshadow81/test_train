using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace LGM
{
    namespace AnimalMatch
    {
        public class InputManager : Singleton<InputManager>
        {
            [HideInInspector]
            public bool isAllClick = false; // 전체 클릭 잠금
            public float shakePower = 10f;
            public AudioClip successClip;   // 정답 사운드
            public AudioClip faildClip;     // 오답 사운드
            public GameObject smokePrefab;   // 소멸 효과
            public CardBG cardBGA;   // 카드 백그라운드
            public CardBG cardBGB;   // 카드 백그라운드

            private Sequence sequence;  // 비교 닷트윈 이벤트
            private Sequence successSequence;   // 정답 시 닷트윈 이벤트
            private Sequence failSequence;  // 오답 시 닷트윈 이벤트
            [HideInInspector]
            public AudioSource source;

            private ACard clickA;  // 클릭한 카드 (위에 배치된 카드)
            public ACard ClickA
            {
                get
                {
                    return clickA;
                }
                set
                {
                    clickA = value;
                    cardBGA.AtiveTrue(clickA.transform);    // 카드 뒤에 배경 활성화 
                    // 아직 B가 눌리지 않았으면 리턴
                    if (clickB == null) 
                        return;
                    CompareEvent(); // ACard, BCard 비교 이벤트
                    sequence.Restart();
                }
            }

            private BCard clickB;  // 클릭한 카드 (아래에 배치된 카드)
            public BCard ClickB
            {
                get
                {
                    return clickB;
                }
                set
                {
                    clickB = value;
                    cardBGB.AtiveTrue(clickB.transform);    // 카드 뒤에 배경 활성화 
                    // 아직 A가 눌리지 않았으면 리턴
                    if (clickA == null)
                        return;
                    CompareEvent();
                    sequence.Restart();
                }
            }

            private void Awake()
            {
                source = GetComponent<AudioSource>();
            }

            // 비교 이벤트
            private void CompareEvent()
            {
                if (clickA != null && clickB != null)
                {
                    cardBGA.transform.SetAsLastSibling();   // 카드 배경A를 맨앞에 그리기
                    cardBGB.transform.SetAsLastSibling();   // 카드 배경B를 맨앞에 그리기
                    clickA.transform.parent.SetAsLastSibling(); // 카드A를 맨앞에 그리기
                    clickB.transform.parent.SetAsLastSibling(); // 카드B를 맨앞에 그리기

                    isAllClick = true;
                    // sequence 실행 시 clickA, clickB 오브젝트를 지정된 위치로 이동 후 비교 함수(CompareEvent) 실행
                    sequence = DOTween.Sequence().SetAutoKill(true).Pause();
                    // clickA, B를 1초 동안 compares위치(월드 좌표 (0,0,0))로 이동
                    sequence.Append(clickA.transform.DOMove(new Vector3(0, 0, 0), 1f));
                    sequence.Join(clickB.transform.DOMove(new Vector3(0, 0, 0), 1f));
                    // 0.5초 후 A,B 카드를 비교. 비교에 따른 이벤트 실행
                    sequence.AppendInterval(0.5f).
                        OnComplete(() =>
                        {
                            if (clickA.type == clickB.type)
                            {
                                SuccessEvent(); // 성공 이벤트 (성공 Sequence등록)
                                successSequence.Restart();  //
                            }
                            else
                            {
                                FailEvent();    // 실패 이벤트
                                failSequence.Restart(); // (실패 Sequence등록)
                            }
                        });
                }
            }

            // 성공 이벤트
            private void SuccessEvent()
            {
                GameManager.Instance.gameClear++;   // 맞춘 갯수 증가
                AudioPlay(successClip);     // 성공 사운드 재생
                Instantiate(smokePrefab);   // 정답 파티클 생성
                Destroy(clickA.gameObject); // 카드 A 오브젝트 삭제
                Destroy(clickB.gameObject); // 카드 B 오브젝트 삭제
                EventInit();    // 초기 상태로 초기화
            }
            // 실패 이벤트
            private void FailEvent()
            {
                AudioPlay(faildClip);
                // Sequence 사용 시 자동 삭제되도록 설정
                failSequence = DOTween.Sequence().SetAutoKill(true).Pause();
                // 1초동안 카드 A, B를 오른쪽 방향으로 shakePower만큼 동시에 흔들기.
                failSequence.Append(clickA.transform.DOShakePosition(1f, Vector3.right * shakePower));
                failSequence.Join(clickB.transform.DOShakePosition(1f, Vector3.right * shakePower));
                failSequence.AppendInterval(0.5f);    // 1초 대기
                // clickA, B를 1초 동안 compares위치로 이동
                failSequence.Append(clickA.transform.DOLocalMove(new Vector3(0, 0, 0), 1f));
                failSequence.Join(clickB.transform.DOLocalMove(new Vector3(0, 0, 0), 1f)).OnComplete(() =>
                {
                    EventInit();    // 이벤트 종료 후 공통 초기화
                });
            }
            // 이벤트 종료 후 공통 초기화
            private void EventInit()
            {
                // 모든 정답을 맞추면 n초 뒤 클리어
                if(GameManager.Instance.gameClear >= GameManager.Instance.successCount)
                {
                    StartCoroutine(DelayUIActive(true, 1f));    // 1초 뒤 UI를 활성화
                }

                // 사용한 카드 배경 A,B는 다시 비활성화
                if (cardBGA.gameObject.activeSelf)
                {
                    cardBGA.ActiveFalse();
                }
                if (cardBGB.gameObject.activeSelf)
                {
                    cardBGB.ActiveFalse();
                }
                // 기본 상태로 초기화
                isAllClick = false;
                clickA.isClick = false;
                clickB.isClick = false;
                clickA = null;
                clickB = null;
            }

            // 사운드 재생
            private void AudioPlay(AudioClip clip)
            {
                // 오답 사운드 재생
                source.PlayOneShot(clip);
            }

            private IEnumerator DelayUIActive(bool active = true, float timer = 1f)
            {
                yield return new WaitForSeconds(timer);
                GameManager.Instance.stateClass.resultState = GameResult.Success;
                GameManager.Instance.zozo.Change(GameState.GameResult);
                
            }
        }
    }
}

