using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.Common
{
    public class TickTockBehaviour : MonoBehaviour
    {
        public float angle = 13.0f;
        public float frequency = 1.0f;
        private float _time;

        public void Update()
        {
            _time += Time.deltaTime * frequency;
            transform.localEulerAngles = new Vector3(0, 0, angle * Mathf.Sin(2 * Mathf.PI * _time));
        }

        public void OnEnable()
        {
            _time = 0.0f;
        }
    }
}