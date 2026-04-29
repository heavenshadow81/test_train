using UnityEngine;
using System.Collections.Generic;

namespace ML.MapoContents.WorldTravel
{
    [ExecuteInEditMode]
    public class PanoramicCamera : MonoBehaviour
    {
        [Range(1, 36)]
        public int cameraCount = 3;
        [Range(10, 178)]
        public float fieldOfView = 60.0f;
        [Range(0.1f, 2.0f)]
        public float nearClipPlane = 0.3f;
        [Range(10.0f, 100.0f)]
        public float farClipPlane = 20.0f;

        public bool usesCustomAspectRatio = false;
        public float customAspectRatio = 16.0f / 9.0f;

        public bool usesRenderTextures = false;
        public RenderTexture[] renderTextures;

        public int depth = 0;

        [SerializeField, HideInInspector]
        private List<Camera> _cams = new List<Camera>();

        [SerializeField, HideInInspector]
        private float[] _cachedAngles = new float[4];

        [SerializeField, HideInInspector]
        private Camera _sumCamera = null;

        // Use this for initialization
        void Start()
        {
            UpdateCameras();
        }

        void Update()
        {
            UpdateCameras();
        }

        void UpdateCameras()
        {
#if UNITY_EDITOR
            var gameViewSize = UnityEditor.Handles.GetMainGameViewSize();
            float aspect = gameViewSize.x / gameViewSize.y;
#else
		    float aspect = (float)Screen.width / (float)Screen.height;
#endif
            if (usesCustomAspectRatio)
                aspect = customAspectRatio;

            usesRenderTextures = usesRenderTextures && renderTextures != null && renderTextures.Length > 0;
            if (usesRenderTextures)
            {
                if (cameraCount % renderTextures.Length > 0)
                    cameraCount = cameraCount + (renderTextures.Length - cameraCount % renderTextures.Length);

                if (_sumCamera == null)
                {
                    GameObject go = new GameObject("_convolution_camera");
                    go.transform.parent = transform;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.hideFlags = HideFlags.HideInHierarchy;
                    go.AddComponent<SumCamera>().renderTextures = renderTextures;
                    _sumCamera = go.AddComponent<Camera>();
                    _sumCamera.depth = 20 + depth;
                    _sumCamera.nearClipPlane = nearClipPlane;
                    _sumCamera.farClipPlane = farClipPlane;
                    _sumCamera.clearFlags = CameraClearFlags.Color;
                    _sumCamera.allowHDR = false;
                    _sumCamera.allowMSAA = false;
                }
            }
            else
            {
                if (_sumCamera != null)
                {
                    DestroyImmediate(_sumCamera.gameObject);
                    _sumCamera = null;
                }
            }

            // Theta : Vertical Angle, Pi : Horizontal Angle
            float theta = 0.0f, pi = 0.0f;

            if (_cachedAngles.Length != cameraCount)
                _cachedAngles = new float[cameraCount];

            if (_cams.Count == 0)
            {
                Camera[] cams = GetComponentsInChildren<Camera>();
                for (int i = 0; i < cams.Length; i++)
                {
                    if (cams[i].gameObject != gameObject && cams[i] != _sumCamera)
                        _cams.Add(cams[i]);
                }
            }
            if (_cams.Count < cameraCount)
            {
                for (int i = _cams.Count; i < cameraCount; i++)
                {
                    GameObject go = new GameObject();
                    go.transform.parent = transform;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;

                    Camera cam = go.AddComponent<Camera>();
                    _cams.Add(cam);
                }
            }
            else if (_cams.Count > cameraCount)
            {
                for (int i = cameraCount; i < _cams.Count; i++)
                {
                    _cams[i].name = "__unused";
                    _cams[i].gameObject.SetActive(false);
                    _cams[i].gameObject.hideFlags = HideFlags.HideInHierarchy;
                }
            }

            float totalAngle = 0.0f;
            double camRectX = 0.0f;
            int cameraCountPerRT = cameraCount;
            if (usesRenderTextures)
                cameraCountPerRT = cameraCount / renderTextures.Length;

            for (int i = 0; i < cameraCount; i++)
            {
                Camera cam = _cams[i];

                if (cam != null)
                {
                    cam.gameObject.SetActive(true);
                    cam.gameObject.hideFlags = HideFlags.None;
                    cam.enabled = true;
                    cam.name = string.Format("camera_{0:00}", i + 1);
                    cam.targetTexture = usesRenderTextures ? renderTextures[i / cameraCountPerRT] : null;
                    cam.allowHDR = false;
                    cam.allowMSAA = false;

                    Rect camRect = cam.rect;
                    camRect.x = (float)camRectX;
                    camRect.y = 0;
                    camRect.width = (float)(1.0 / (double)cameraCountPerRT);
                    camRect.height = 1.0f;
                    cam.rect = camRect;

                    theta = fieldOfView * Mathf.Deg2Rad;
                    pi = 2 * Mathf.Atan(aspect * (1.0f / cameraCount) * Mathf.Tan(theta * 0.5f));

                    float angle = pi * Mathf.Rad2Deg;
                    _cachedAngles[i] = angle;
                    totalAngle += angle;
                    camRectX += camRect.width;
                    if (camRectX >= 0.99999)
                        camRectX = 0;
                }
                else
                    _cachedAngles[i] = 0;
            }

            float f = -totalAngle * 0.5f + _cachedAngles[0] * 0.5f;

            for (int i = 0; i < cameraCount; i++)
            {
                Camera cam = _cams[i];
                if (cam != null)
                {
                    if (Mathf.Abs(cam.fieldOfView - fieldOfView) > 0.0f)
                        cam.fieldOfView = fieldOfView;
                    if (Mathf.Abs(cam.nearClipPlane - nearClipPlane) > 0.0f)
                        cam.nearClipPlane = nearClipPlane;
                    if (Mathf.Abs(cam.farClipPlane - farClipPlane) > 0.0f)
                        cam.farClipPlane = farClipPlane;
                    if (cam.orthographic)
                        cam.orthographic = false;
                    if (Mathf.Abs(cam.aspect - aspect / (float)cameraCount) > 0.0f)
                        cam.aspect = aspect / (float)cameraCount;
                    cam.depth = depth;

                    Quaternion rot = Quaternion.Euler(0.0f, f, 0.0f);
                    if (Mathf.Abs(Quaternion.Angle(cam.transform.localRotation, rot)) > 0.0f)
                        cam.transform.localRotation = rot;

                    if (i + 1 < cameraCount)
                        f += _cachedAngles[i] * 0.5f + _cachedAngles[i + 1] * 0.5f;
                }
            }
        }
    }
}