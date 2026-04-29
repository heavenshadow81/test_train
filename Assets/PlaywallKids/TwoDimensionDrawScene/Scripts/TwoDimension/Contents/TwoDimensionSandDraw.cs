using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 모래 그림 그리기 메인 클래스
    /// </summary>
    public class TwoDimensionSandDraw : TwoDimensionBase
    {
        /// <summary>
        /// true : 이벤트 진행 중 , false : 현재 이벤트 없음
        /// </summary>
        protected bool bEvent = false;
        private IEvent[] _arr;
        /// <summary>
        /// interface IEvent 참조
        /// </summary>
        public IEvent[] localEvents
        {
            get
            {
                if (_arr == null)
                {
                    _arr = new IEvent[3];
                    _arr[0] = (IEvent)GetComponentInChildren<TwoDimensionPanel>();
                    _arr[1] = (IEvent)GetComponentInChildren<TwoDimensionInteractionPanel>();
                    _arr[2] = (IEvent)GetComponentInChildren<TwoDimensionWavePanel>();
                }
                return _arr;
            }
        }

        /// <summary>
        /// 비트 플래그 변수
        /// </summary>
        int iEventState;

        //  1 << 0 | 1 << 1 | 1 << 2 == localEvens 의 index 총수
        const int CHECK_EVENT = 0x07;

        /// <summary>
        /// 초기화
        /// </summary>
        void Awake()
        {
            bEvent = false;
            iEventState = 0;
        }

        /// <summary>
        /// 콘텐츠 체험 시작
        /// </summary>
        /// <returns>return true : 시작 완료, return false : 시작 준비 하는 중</returns>
        public override bool PlayStart()
        {
            bEvent = false;
            return true;

        }

        /// <summary>
        /// 콘텐츠 체험 중
        /// </summary>
        /// <returns>return false 콘텐츠 체험 중</returns>
        public override bool Play()
        {
            if (!bEvent)
            {
                for (int i = 0; i < localEvents.Length; ++i)
                {
                    if (localEvents[i] != null)
                    {
                        localEvents[i].StateInPlay();
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 콘텐츠 종료시 호출 할 함수들 재정의
        /// </summary>
        /// <returns> return true : 종료 완료, return false : 종료 하는 중</returns>
        public override bool PlayEnd()
        {
            return true;
        }

        // 개별 콘텐츠로 테스트를 원할 경우 Update함수 주석 해제
        // TwoDimensionAdmin를 통해 동적 생성 될 경우는 주석 해제 하면 안됨
        void Update()
        {
            Play();
        }

        /// <summary>
        /// 파도 이벤트 코루틴
        /// </summary>
        /// <returns></returns>
        IEnumerator EventProcess()
        {
            if (localEvents == null || localEvents.Length != 3)
            { Debug.LogError("null"); }

            iEventState = 0;
            do
            {
                for (int i = 0; i < localEvents.Length; ++i)
                {
                    if (localEvents[i] != null)
                    {
                        // 등록 된 이벤트 제어 클래스 들이 이벤트 준비(각종 변수, 이벤트 애니메이션 등 초기화 완료 체크)
                        if (localEvents[i].StateEventReady())
                        { iEventState |= 0x01 << i; } // 해당 비트 1로 dirty bit 적용
                    }
                    else
                    {
                        // null이면 무조건 체크, 무한 루프 방지용
                        iEventState |= 0x01 << i;
                    }
                }
                yield return new WaitForEndOfFrame();
            } while ((iEventState & CHECK_EVENT) != CHECK_EVENT);

            iEventState = 0;
            do
            {
                for (int i = 0; i < localEvents.Length; ++i)
                {
                    if (localEvents[i] != null)
                    {
                        // 반환 값이 true 일경우 해당 index class 이벤트 완료
                        if (localEvents[i].StateEventActivates())
                        { iEventState |= 0x01 << i; }// 이벤트 진행중 bit flag 체크
                    }
                    else//무한 루프 방지용 코드
                    { iEventState |= 0x01 << i; }
                }
                yield return new WaitForEndOfFrame();
            } while ((iEventState & CHECK_EVENT) != CHECK_EVENT);

            iEventState = 0;
            bEvent = false;
        }

        public void GenerateEvent()
        {
            if (!bEvent)
            {
                bEvent = true;
                StartCoroutine(EventProcess());
            }
        }
    }
}