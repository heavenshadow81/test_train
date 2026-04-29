using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class UIArrow : UIObject
    {
        UISprite _sprite;
        public UISprite img
        {
            get
            {
                if (_sprite == null) _sprite = this.GetComponent<UISprite>();
                return _sprite;
            }
        }

        public override float Value
        {
            get { return CachedTransform.localEulerAngles.z; }
            set { CachedTransform.localEulerAngles = new Vector3(0, 0, -value); }
        }
    }
}