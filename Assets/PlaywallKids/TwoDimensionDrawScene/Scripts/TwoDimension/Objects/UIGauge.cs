using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class UIGauge : UIObject
    {
        public UISprite backGround;
        public UISprite foreGround;
        //public int size;

        public override float Value
        {
            get { return foreGround.fillAmount; }
            set
            {
                if (value > 0)
                    foreGround.fillAmount += value;
                else
                    foreGround.fillAmount = 0f;


            }
        }

        public float backValue
        {
            get { return backGround.fillAmount; }
            set { backGround.fillAmount += Mathf.Clamp01(value); }
        }

        void OnEnable()
        {
            backValue = 1f;
            Value = 0f;
        }
    }
}
