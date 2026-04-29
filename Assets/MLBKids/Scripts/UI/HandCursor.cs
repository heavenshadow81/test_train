using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ML.MLBKids
{
    public class HandCursor : MonoBehaviour
    {
        public Image cursor;
        public Sprite cursorSpriteDefault, cursorSpriteGrip;
        public Image loadingBack, loadingFront;

        public float holdTime = 2.0f;
        public static bool showsCursor = true;

        private bool _isClenched = false;
        private Button _lastButton = null;
        private float _holdTime = 0.0f;

        public void Awake()
        {
            HideCursor();
        }

        public void HideCursor()
        {
            cursor.gameObject.SetActive(false);
            loadingBack.gameObject.SetActive(false);
            _lastButton = null;
        }

        public void Update()
        {
            //if (!KinectHelper.instance.isOpen || !showsCursor)
            //{
            //    HideCursor();
            //    return;
            //}

            //if (KinectHelper.instance.trackingId != 0)
            //{
            //    cursor.gameObject.SetActive(true);
            //    _isClenched = KinectHelper.instance.rightHandState == Windows.Kinect.HandState.Closed;
            //    if (_isClenched)
            //    {
            //        cursor.sprite = cursorSpriteGrip;
            //    }
            //    else
            //    {
            //        cursor.sprite = cursorSpriteDefault;
            //        var cursorPos = KinectHelper.instance.uiSpaceRightHandPos;
            //        cursorPos.y = cursorPos.y * -1.0f;
            //        cursor.rectTransform.anchoredPosition = cursorPos;
            //    }
            //    _RaycastUI();

            //    loadingBack.gameObject.SetActive(_lastButton != null);
            //    loadingFront.fillAmount = 1.0f - _holdTime / Mathf.Clamp(holdTime, 0.25f, 10.0f);
            //}
            //else
            //{
            //    HideCursor();
            //}
        }

        private void _RaycastUI()
        {
            // convert from ui space to screen space
            var pointer = new PointerEventData(EventSystem.current);
            //Vector2 pos = KinectHelper.instance.uiSpaceRightHandPos;
            //float screenSpaceAspect = (float)Screen.width / (float)Screen.height;
            //pos.x = pos.x / (1920.0f * screenSpaceAspect) * (float)Screen.width;
            //pos.y = pos.y / 1920.0f * (float)Screen.height;
            //pos.y = Screen.height - pos.y;    // left-bottom side
            //pointer.position = pos;
            
            // Raycasts any button on hand
            Button btn = null;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            if (raycastResults.Count > 0)
            {
                GameObject ui = raycastResults[0].gameObject;
                RectTransform rtf = ui.GetComponent<RectTransform>();

                // ignores very-very big button
                if (rtf.rect.width < 1000 && rtf.rect.height < 1000)
                {
                    btn = ui.GetComponent<Button>();

                    // ignores hidden menu
                    if (ui.GetComponentInParent<HiddenMenu>() != null)
                        btn = null;
                }
            }
            
            // Checks button holding
            if (_lastButton != btn)
            {
                _holdTime = holdTime;
                _lastButton = btn;
                Debug.Log("detected button : " + (btn != null ? btn.name : null));
            }
            else
            {
                _holdTime -= Time.deltaTime;
                if (_isClenched)
                    _holdTime -= Time.deltaTime;
                if (_holdTime <= 0.0f)
                {
                    if (_lastButton != null && _lastButton.onClick != null)
                    {
                        _lastButton.onClick.Invoke();
                        _lastButton = null;
                    }
                }
            }
        }
    }
}