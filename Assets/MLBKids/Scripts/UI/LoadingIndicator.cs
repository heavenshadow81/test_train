using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.MLBKids
{
    public class LoadingIndicator : MonoBehaviour
    {
        private float time = 0.0f;

        public void Update()
        {
            time += Time.deltaTime;
            if (time >= 0.166666f)
            {
                transform.Rotate(0, 0, -30.0f);
                time -= 0.166666f;
            }
        }
    }
}