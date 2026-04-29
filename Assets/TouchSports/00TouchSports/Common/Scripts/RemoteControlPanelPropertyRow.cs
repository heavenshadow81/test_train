using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ML.T_Sports.Common
{
    public class RemoteControlPanelPropertyRow : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public enum ValueType
        {
            Float,
            Int,
            Percentage
        }

        public ContentsPropertyType propertyType;
        public ValueType valueType;
        public float increase;
        public bool isSharedProperty;
        public int acceleration;

        private CanvasGroup _canvasGroup;
        private Text _text;
        private Toggle _toggle;
        private Slider _slider;
        
        private bool _pressLeft, _pressRight;
        private float _pressTime;
        private int _numIncreasedDuringPressing = 0;

        private int _draggingPointerId = 0;

        public void OnEnable()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _text = GetComponentInChildren<Text>();
            _toggle = GetComponentInChildren<Toggle>();
            _slider = GetComponentInChildren<Slider>();
        }

        public void Update()
        {
            if (_pressLeft || _pressRight)
            {
                _pressTime += Time.deltaTime;
                if (_pressTime >= 0.25f)
                {
                    _pressTime -= 0.25f;
                    _numIncreasedDuringPressing += 1;

                    int multiplier = 1;
                    if (acceleration > 0)
                        multiplier = Mathf.RoundToInt(Mathf.Pow(2, _numIncreasedDuringPressing / acceleration));

                    for (int i = 0; i < multiplier; i += 1)
                    {
                        if (_pressLeft)
                            Decrease();
                        else
                            Increase();
                    }
                }
            }
        }

        public void Refresh()
        {
            if (ContentsManagerBase.Current != null)
            {
                bool enabled = !ContentsManagerBase.Current.IsPlaying;
                if (isSharedProperty)
                    enabled = enabled && ContentsManagerBase.Current.HasSharedProperty(propertyType);
                else
                    enabled = enabled && ContentsManagerBase.Current.HasProperty(propertyType);

                // UI 활성화 여부 설정
                if (_canvasGroup != null)
                {
                    _canvasGroup.interactable = enabled;
                    _canvasGroup.blocksRaycasts = enabled;
                }

                // 버튼을 누르는 중일 경우 비활성화일 때는 false로 설정
                if (!enabled)
                {
                    _pressLeft = _pressRight = false;
                }

                // 값을 불러와서 알맞게 표시
                float value = isSharedProperty ? 
                    ContentsManagerBase.Current.GetSharedPropertyValue(propertyType) : 
                    ContentsManagerBase.Current.GetPropertyValue(propertyType);

                if (_text != null)
                {
                    if (propertyType == ContentsPropertyType.Time)
                    {
                        int min = (int)(value / 60.0f);
                        int sec = (int)(value % 60.0f);
                        _text.text = string.Format("{0:00}분 {1:00}초", min, sec);
                    }
                    else
                    {
                        switch (valueType)
                        {
                            case ValueType.Float:
                                _text.text = value.ToString("0.0");
                                break;
                            case ValueType.Int:
                                _text.text = (value + 0.001f).ToString("0");
                                break;
                            case ValueType.Percentage:
                                _text.text = value.ToString("0%");
                                break;
                        }

                        if (propertyType == ContentsPropertyType.Chance)
                            _text.text += " 회";
                        else if (propertyType == ContentsPropertyType.Player)
                            _text.text += " 명";
                        else if (propertyType == ContentsPropertyType.Height)
                            _text.text += "cm";
                    }

                    // 활성화 상태에 따라 텍스트 알파 설정
                    if (_slider == null)
                    {
                        var c = _text.color;
                        c.a = enabled ? 1.0f : 0.5f;
                        _text.color = c;
                    }
                }
                if (_toggle != null)
                {
                    var handlers = _toggle.onValueChanged;
                    _toggle.onValueChanged = new Toggle.ToggleEvent();
                    _toggle.isOn = value >= 0.5f;
                    _toggle.onValueChanged = handlers;
                }
                if (_slider != null)
                {
                    var handlers = _slider.onValueChanged;
                    _slider.onValueChanged = new Slider.SliderEvent();
                    _slider.value = value;
                    _slider.onValueChanged = handlers;
                }
            }
        }

        public void Increase()
        {
            if (ContentsManagerBase.Current != null)
            {
                if (isSharedProperty)
                {
                    if (ContentsManagerBase.Current.HasSharedProperty(propertyType))
                    {
                        ContentsManagerBase.Current.SetSharedPropertyValue(propertyType, ContentsManagerBase.Current.GetSharedPropertyValue(propertyType) + increase);
                    }
                }
                else
                {
                    if (ContentsManagerBase.Current.HasProperty(propertyType))
                    {
                        ContentsManagerBase.Current.SetPropertyValue(propertyType, ContentsManagerBase.Current.GetPropertyValue(propertyType) + increase);
                    }
                }
            }

            Refresh();
        }

        public void Decrease()
        {
            if (ContentsManagerBase.Current != null)
            {
                if (isSharedProperty)
                {
                    if (ContentsManagerBase.Current.HasSharedProperty(propertyType))
                    {
                        ContentsManagerBase.Current.SetSharedPropertyValue(propertyType, ContentsManagerBase.Current.GetSharedPropertyValue(propertyType) - increase);
                    }
                }
                else
                {
                    if (ContentsManagerBase.Current.HasProperty(propertyType))
                    {
                        ContentsManagerBase.Current.SetPropertyValue(propertyType, ContentsManagerBase.Current.GetPropertyValue(propertyType) - increase);
                    }
                }
            }

            Refresh();
        }

        public void Toggle()
        {
            if (ContentsManagerBase.Current != null)
            {
                if (isSharedProperty)
                {
                    if (ContentsManagerBase.Current.HasSharedProperty(propertyType))
                    {
                        float props = ContentsManagerBase.Current.GetSharedPropertyValue(propertyType);
                        ContentsManagerBase.Current.SetSharedPropertyValue(propertyType, props >= 0.5f ? 0 : 1);
                    }
                }
                else
                {
                    if (ContentsManagerBase.Current.HasProperty(propertyType))
                    {
                        float props = ContentsManagerBase.Current.GetPropertyValue(propertyType);
                        ContentsManagerBase.Current.SetPropertyValue(propertyType, props >= 0.5f ? 0 : 1);
                    }
                }
            }

            Refresh();
        }

        public void Assign()
        {
            if (ContentsManagerBase.Current != null)
            {
                if (isSharedProperty)
                {
                    if (ContentsManagerBase.Current.HasSharedProperty(propertyType))
                    {
                        if (_slider != null)
                            ContentsManagerBase.Current.SetSharedPropertyValue(propertyType, _slider.value);
                    }
                }
                else
                {
                    if (ContentsManagerBase.Current.HasProperty(propertyType))
                    {
                        if (_slider != null)
                            ContentsManagerBase.Current.SetPropertyValue(propertyType, _slider.value);
                    }
                }
            }

            Refresh();
        }

        public void PressLeft()
        {
            if (!_pressRight)
            {
                _pressLeft = true;
                _pressTime = -0.25f;
                _numIncreasedDuringPressing = 1;
                Decrease();
            }
        }
        
        public void PressRight()
        {
            if (!_pressLeft)
            {
                _pressRight = true;
                _pressTime = -0.25f;
                _numIncreasedDuringPressing = 1;
                Increase();
            }
        }

        public void ReleaseLeft()
        {
            _pressLeft = false;
        }

        public void ReleaseRight()
        {
            _pressRight = false;
        }

        #region Dragging (slider)
        // Unity가 Multi-Display를 제대로 지원하지 못해서 생기는 문제
        // 5.4부터 발생한 버그인데 아직도 안고치는 듯. 언제 고쳐주는거지?
        // https://issuetracker.unity3d.com/issues/ui-dual-displays-ui-components-are-scaled-or-placed-wrongly-and-do-not-interact-in-dual-display-projects
        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId == _draggingPointerId)
            {
                Camera cam = GetComponentInParent<Camera>();
                Vector2 pos = eventData.position;
                _SetSliderValueInternal(pos, cam);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_slider != null && _draggingPointerId == 0)
            {
                _draggingPointerId = eventData.pointerId;

                Camera cam = GetComponentInParent<Camera>();
                Vector2 pos = eventData.position;
                _SetSliderValueInternal(pos, cam);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId == _draggingPointerId)
                _draggingPointerId = 0;
        }

        private void _SetSliderValueInternal(Vector2 pos, Camera cam)
        {
#if !UNITY_EDITOR
            // Unity 2017.x 에디터에서는 Multi-Display를 설정해도 현재 포커싱된 디스플레이만을 인식함. 빌드 후에는 Multi-Display 정상 작동.
            pos = Display.RelativeMouseAt(pos);
#endif
            if (_slider != null && _slider.fillRect != null)
            {
                RectTransform area = _slider.fillRect.parent as RectTransform;
                if (area != null)
                {
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(area, pos, cam, out pos))
                    {
                        _slider.value = (area.rect.width * area.pivot.x + pos.x) / area.rect.width;
                        Assign();
                    }
                }
            }
        }
#endregion
    }
}