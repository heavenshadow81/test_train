using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TwoDimensionGoalPost : TwoDimensionColliderManager
    {
        [Range(0f, 1f)]
        public float widthRatio;
        [Range(0f, 1f)]
        public float heightRatio;
        void Start()
        {
            float _w = UtilityScript.width * widthRatio;
            float _h = UtilityScript.height * heightRatio;
            float _restH = (UtilityScript.height - _h) * 0.5f;

            if (top != null)
            {
                top.localScale = new Vector3(_w, _restH, 1f);
                top.localPosition = new Vector3(0, (_h * 0.5f) + (_restH * 0.5f), 0);
            }
            if (bottom != null)
            {
                bottom.localScale = new Vector3(_w, _restH, 1f);
                bottom.localPosition = new Vector3(0, -1 * ((_h * 0.5f) + (_restH * 0.5f)), 0);
            }
            if (front != null) front.localScale = new Vector3(_w, _h, 1f);
        }
    }
}