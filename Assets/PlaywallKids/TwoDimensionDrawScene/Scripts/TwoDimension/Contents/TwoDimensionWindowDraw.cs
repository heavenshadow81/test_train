using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TwoDimensionWindowDraw : TwoDimensionBase
    {
        void OnEnable()
        {
        }
        
        public override bool PlayStart()
        {
            return true;
        }

        public override bool Play()
        {
            return false;
        }

        public override bool PlayEnd()
        {
            return true;
        }
    }
}