using UnityEngine;

namespace ML.MapoContents.WorldTravel
{
    [RequireComponent(typeof(PanoramicCamera))]
    public class CursorCamera : MonoBehaviour
    {
        private PanoramicCamera _panoramicCamera;

        Vector3 _mousePos;
        Vector3 _mouseDeltaLerp;
        
        void Start()
        {
            _panoramicCamera = GetComponent<PanoramicCamera>();
            _mousePos = Input.mousePosition;
        }
        
        void Update()
        {
            Vector3 mousePos = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                _mousePos = mousePos;
            }
            if (Input.GetMouseButton(0))
            {
                Vector3 mouseDelta = mousePos - _mousePos;
                
                CameraRotation.Pitch(transform, mouseDelta.y);
                CameraRotation.RotateY(transform, mouseDelta.x);

                _mouseDeltaLerp = mouseDelta;
                _mousePos = mousePos;
            }
            else
            {
                _mouseDeltaLerp = Vector3.Lerp(_mouseDeltaLerp, Vector3.zero, Time.deltaTime * 6.5f);

                CameraRotation.Pitch(transform, _mouseDeltaLerp.y);
                CameraRotation.RotateY(transform, _mouseDeltaLerp.x);
            }

            _panoramicCamera.fieldOfView -= Input.mouseScrollDelta.y;
        }
    }
}