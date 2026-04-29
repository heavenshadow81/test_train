using UnityEngine;

namespace ML.PlaywallKids.MotionJump
{
    /// <summary>
    /// 현재 사용안함
    /// </summary>
    [RequireComponent(typeof(UISlider))]
    public class UIInputZoneController : MonoBehaviour
    {
        [Range(0.1f, 1f)]
        public float[] inputZones;
        [Range(0.1f, 50f)]
        public float inputZoneWidth;
        [Range(0.1f, 10f)]
        public float scrollSpeed;

        bool bIncrease;
        bool bInput;
        float fVolume;

        UISprite mThumb;
        public UISprite thumb
        {
            get
            {
                if (mThumb == null)
                { mThumb = slider.thumb.gameObject.GetComponent<UISprite>(); }
                return mThumb;
            }
        }

        private UISlider mSlider;
        public UISlider slider
        {
            get
            {
                if (mSlider == null)
                {
                    mSlider = this.GetComponent<UISlider>();
                }
                return mSlider;
            }
        }

        void OnEnable()
        {
            if (scrollSpeed == 0) scrollSpeed = 0.1f;
            bIncrease = (slider.value == 0);
            bInput = false;
        }

        void FixedUpdate()
        {
            fVolume += Time.deltaTime * scrollSpeed * (bIncrease ? 1 : -1);
            if (fVolume >= 1f)
            {
                fVolume = 1f;
                bIncrease = false;
            }
            else if (fVolume <= 0)
            {
                fVolume = 0;
                bIncrease = true;
            }

            for (int i = 0; i < inputZones.Length; ++i)
            {
                if (inputZones[i] < fVolume && fVolume < inputZones[i] + 0.15f)
                {
                    thumb.color = Color.red;
                    bInput = true;
                    break;
                }
                else
                {
                    thumb.color = Color.white;
                    bInput = false;
                }
            }
            slider.value = fVolume;

        }

        public bool AcceptInput()
        {
            return bInput;
        }
    }
}