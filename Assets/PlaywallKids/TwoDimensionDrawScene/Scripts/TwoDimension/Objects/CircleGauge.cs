using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class CircleGauge : MonoBehaviour
    {
        public UISprite backGround;
        public UISprite foreGround;
        //public int size;

        Transform _transform;
        public Transform cachedTransform
        {
            get
            {
                if (_transform == null) _transform = this.transform;
                return _transform;
            }
        }

        public float foreValue
        {
            get { return foreGround.fillAmount; }
            set { foreGround.fillAmount = Mathf.Clamp01(value); }
        }

        public float backValue
        {
            get { return backGround.fillAmount; }
            set { backGround.fillAmount = Mathf.Clamp01(value); }
        }

        void OnEnable()
        {
            backValue = 1f;
            foreValue = 0f;
        }
    }
}