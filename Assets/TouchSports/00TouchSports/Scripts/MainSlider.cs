using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ML.T_Sports.Common;

public class MainSlider : MonoBehaviour
{
    private int _draggingPointerId = 0;
    private Slider _slider;
    private Vector3 _prevDragPos = Vector3.zero;
    public bool isSharedProperty;
    public ContentsPropertyType propertyType;

    public void Awake()
    {
        _slider = GetComponentInChildren<Slider>();
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
    }
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
        if (_slider != null)
        {
            _draggingPointerId = eventData.pointerId;
            _prevDragPos = eventData.position;

            Camera cam = GetComponentInParent<Camera>();
            Vector2 pos = eventData.position;
            _SetSliderValueInternal(pos, cam);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _draggingPointerId = 0;
    }

    private void _SetSliderValueInternal(Vector2 pos, Camera cam)
    {
#if !UNITY_EDITOR
            // Unity 2017.x 에디터에서는 Multi-Display를 설정해도 현재 포커싱된 디스플레이만을 인식함. 빌드 후에는 Multi-Display 정상 작동.
            pos = Display.RelativeMouseAt(pos);
#endif
        RectTransform area = _slider.fillRect.parent as RectTransform;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(area, pos, cam, out pos))
        {
            _slider.value = (area.rect.width * area.pivot.x + pos.x) / area.rect.width;
            //Assign();
        }
    }
}


