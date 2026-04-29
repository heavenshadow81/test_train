using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 콘텐츠 내 UI 객체들의 base 클래스
    /// </summary>
    public abstract class UIObject : MonoBehaviour
    {
        Transform _transform;

        public Transform CachedTransform
        {
            get
            {
                if (_transform == null) _transform = transform;
                return _transform;
            }
        }

        /// <summary>
        /// 상속 받은 자식 클래스에서 재정의
        /// 클래스 마다 값의 성질이 다름
        /// </summary>
        public abstract float Value
        {
            get;
            set;
        }
    }
}