using UnityEngine;
using UnityEngine.UI;

namespace ML.T_Sports.Common
{
    using TouchModule;

    /// <summary>
    /// 터치 센서 속도를 표시하는 UI (단순 표시용)
    /// </summary>
    public class TouchModuleSpeedUI : MonoBehaviour
    {
        public Text text;
        private Canvas _canvas;

        private float _speed = 0;
        private float _prevSpeed = 0;
        private float _speedLerp = 0;

        public void Awake()
        {
            _canvas = GetComponentInChildren<Canvas>();
            TouchModuleInput.Speed = 0;
            SetSpeedText(0);
        }

        public void Update()
        {
            if (_canvas != null && ContentsManagerBase.Current != null)
            {
                bool isPlaying = ContentsManagerBase.Current.IsPlaying;
                _canvas.enabled = isPlaying && TouchModuleInput.Instance.CurrentSensor != TouchSensor.None;
                if (_canvas.enabled)
                {
                    _speed = TouchModuleInput.Speed;
                    if (_prevSpeed != _speed)
                    {
                        _prevSpeed = _speed;
                        if (_speedLerp > 0)
                            _speedLerp = 0;
                    }

                    if (_speedLerp < 1)
                    {
                        _speedLerp += Time.deltaTime * 4.0f;
                        if (_speedLerp > 1)
                            _speedLerp = 1;
                        SetSpeedText(Mathf.Lerp(0, _prevSpeed, _speedLerp));
                    }
                }
                else
                {
                    _speed = _speedLerp = 0;
                    _prevSpeed = 0;
                    TouchModuleInput.Speed = 0;
                    SetSpeedText(0);
                }
            }
        }

        public void SetSpeedText(float speed)
        {
            text.text = speed.ToString("000") + " <size=60><i>km/h</i></size>";
        }
    }
}