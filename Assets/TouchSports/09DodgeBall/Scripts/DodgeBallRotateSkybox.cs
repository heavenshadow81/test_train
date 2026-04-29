using UnityEngine;

namespace ML.T_Sports.DodgeBall
{
    /// <summary>
    /// 배경 skybox 360도 회전 스크립트
    /// </summary>
    public class DodgeBallRotateSkybox : MonoBehaviour
    {
        /// <summary>
        /// 회전 속도 (1.0 = 1초에 360º회전)
        /// </summary>
        [Range(0.0f, 2.0f)]
        public float speed = 1.0f;

        Material _skybox;
        float _time;

        void Start()
        {
            Material skybox = RenderSettings.skybox;
            if (skybox != null)
            {
                _skybox = new Material(skybox);
                RenderSettings.skybox = _skybox;
            }
        }
        
        void Update()
        {
            if (_skybox != null)
            {
                _skybox.SetFloat("_Rotation", _time);
                _time += Time.deltaTime * speed;
                if (_time >= 360.0f)
                    _time -= 360.0f;
            }
        }

        void OnDestroy()
        {
            if (_skybox != null)
            {
                Destroy(_skybox);
            }
        }
    }
}
