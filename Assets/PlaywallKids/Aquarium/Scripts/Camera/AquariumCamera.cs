using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.PlaywallKids.SeaStory
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class AquariumCamera : MonoBehaviour
    {
        [Range(10.0f, 40.0f)]
        public float minFieldOfView = 18.0f;
        [Range(10.0f, 40.0f)]
        public float maxFieldOfView = 25.0f;
        [Range(1.0f, 5.3333333f)]
        public float minAspectRatio = 1.777777f;
        [Range(1.0f, 5.3333333f)]
        public float maxAspectRatio = 5.333333f;
        private Camera _camera;
        private int _screenWidth, _screenHeight;

        public void Start()
        {
            _camera = GetComponent<Camera>();
            _UpdateSettings(Screen.width, Screen.height);
        }

        public void Update()
        {
            int sw = Screen.width, sh = Screen.height;
            if (sw != _screenWidth || sh != _screenHeight)
            {
                _UpdateSettings(sw, sh);
            }
        }

        private void _UpdateSettings(int sw, int sh)
        {
            float aspect = sw / (float)sh;
            _camera.fieldOfView = Mathf.Lerp(maxFieldOfView, minFieldOfView, (aspect - minAspectRatio) / (maxAspectRatio - minAspectRatio));

            _screenWidth = sw;
            _screenHeight = sh;
        }
    }
}