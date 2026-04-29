using UnityEngine;

namespace ML.PlaywallKids.MotionJump
{
    /// <summary>
    /// 사용자 높이를 디스플레이 하는 클래스
    /// </summary>
    [RequireComponent(typeof(UISlider))]
    public class UIJumpHeightController : MonoBehaviour
    {

        UISlider mSlider;
        UISlider slider
        {
            get
            {
                if (mSlider == null)
                { mSlider = GetComponent<UISlider>(); }
                return mSlider;
            }
        }

        public void UIDisplayJumpHeight(float _fPercentage)
        {
            _fPercentage = Mathf.Clamp01(_fPercentage);
            slider.value = _fPercentage;
        }
    }
}