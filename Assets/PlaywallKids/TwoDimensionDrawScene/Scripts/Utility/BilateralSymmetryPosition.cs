using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class BilateralSymmetryPosition : MonoBehaviour
    {
        public Transform[] objects;
        [Range(0f, 1f)]
        public float xRatio;

        [Range(0f, 1f)]
        public float yRatio;

        // public float gapBtween;
        void OnEnable()
        {
            for (int i = 0, len = objects.Length; i < len; ++i)
            {
                objects[i].localPosition = new Vector3(UtilityScript.width * xRatio * (i % 2 == 0 ? -0.5f : 0.5f), UtilityScript.height * yRatio, 0);
            }
        }
    }
}