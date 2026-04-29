using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class AutoDisable : MonoBehaviour
    {
        public new Light light;
        public float intensive;
        public float sensivity;

        void OnEnable()
        {
            light.intensity = intensive;
        }

        void Update()
        {
            light.intensity -= Time.deltaTime * sensivity;
        }
    }
}