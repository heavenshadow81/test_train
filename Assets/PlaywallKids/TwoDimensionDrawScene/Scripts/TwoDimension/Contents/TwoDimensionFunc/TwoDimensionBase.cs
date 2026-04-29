using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public abstract class TwoDimensionBase : MonoBehaviour, IProcessFlow
    {

        /// <summary>
        /// 콘텐츠 시작시 호출 함수, 초기화 또는 시작 이벤트를 이 함수에서 재정의 할 것
        /// </summary>
        /// <returns></returns>
        public abstract bool PlayStart();
        /// <summary>
        /// 콘텐츠 체험 중
        /// </summary>
        /// <returns></returns>
        public abstract bool Play();
        /// <summary>
        /// 콘텐츠 종료 시 호출 되는 함수, 종료 이벤트 등을 이함수에서 재정의 할 것
        /// </summary>
        /// <returns></returns>
        public abstract bool PlayEnd();
    }
}