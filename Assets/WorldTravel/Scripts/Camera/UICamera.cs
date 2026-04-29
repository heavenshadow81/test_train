using System.Collections.Generic;
using UnityEngine;

namespace ML.MapoContents.WorldTravel
{
    using Kinect;

    [ExecuteInEditMode, RequireComponent(typeof(Camera))]
    public class UICamera : MonoBehaviour
    {
        public PanoramicCamera panoramicCamera;
        public RectTransform mainUICanvas;

        private Camera _camera;
        [SerializeField, HideInInspector]
        private List<Camera> _splittedCameras = new List<Camera>();

        public void Start()
        {
            _camera = GetComponent<Camera>();
        }

        public void Update()
        {
            _UpdateCamera();
        }

        private void _UpdateCamera()
        {
            if (panoramicCamera != null)
            {
                RenderTexture[] renderTextures = null;
                if (panoramicCamera.usesRenderTextures)
                {
                    renderTextures = panoramicCamera.renderTextures;
                }

                // Sets the area of UI canvas and camera.
                if (panoramicCamera.usesCustomAspectRatio)
                {
                    _camera.aspect = panoramicCamera.customAspectRatio;
                    if (mainUICanvas != null)
                    {
                        Vector2 size = mainUICanvas.sizeDelta;
                        size.y = 1080.0f;
                        if (renderTextures != null && renderTextures.Length > 0)
                            size.y = renderTextures[0].height;
                        size.x = size.y * panoramicCamera.customAspectRatio;
                        mainUICanvas.sizeDelta = size;
                        _camera.orthographicSize = size.y * 0.5f;

                        if (KinectHelper.instance != null)
                        {
                            KinectHelper.instance.usesCustomScreenSize = true;
                            KinectHelper.instance.customScreenSize = size;
                        }
                    }
                }
                else
                {
                    _camera.aspect = Screen.width / (float)Screen.height;
                    if (mainUICanvas != null)
                    {
                        Vector2 size = mainUICanvas.sizeDelta;
                        size.x = Screen.width;
                        size.y = Screen.height;
                        mainUICanvas.sizeDelta = size;
                        _camera.orthographicSize = size.y * 0.5f;

                        if (KinectHelper.instance != null)
                        {
                            KinectHelper.instance.usesCustomScreenSize = false;
                        }
                    }
                }

                // Creates internal cameras for each render textures.
                if (renderTextures != null)
                {
                    if (renderTextures.Length < _splittedCameras.Count)
                    {
                        for (int i = renderTextures.Length; i < _splittedCameras.Count; i++)
                        {
                            DestroyImmediate(_splittedCameras[i].gameObject);
                        }
                        _splittedCameras.RemoveRange(renderTextures.Length, (_splittedCameras.Count - renderTextures.Length));
                    }
                    else if (renderTextures.Length > _splittedCameras.Count)
                    {
                        for (int i = _splittedCameras.Count; i < renderTextures.Length; i++)
                        {
                            GameObject go = new GameObject(string.Format("camera_{0:00}", i + 1));
                            go.hideFlags = HideFlags.HideInHierarchy;
                            go.transform.parent = transform;
                            go.transform.localPosition = Vector3.zero;
                            go.transform.localRotation = Quaternion.identity;
                            go.transform.localScale = Vector3.one;
                            go.layer = gameObject.layer;
                            Camera cam = go.AddComponent<Camera>();
                            _splittedCameras.Add(cam);
                        }
                    }

                    float camAspect = _camera.aspect / _splittedCameras.Count;
                    Vector3 camPos = new Vector3(-_camera.orthographicSize * camAspect * (_splittedCameras.Count - 1), 0);
                    for (int i = 0; i < _splittedCameras.Count; i++)
                    {
                        Camera cam = _splittedCameras[i];
                        cam.CopyFrom(_camera);
                        cam.aspect = camAspect;
                        cam.targetTexture = renderTextures[i];
                        cam.transform.localPosition = camPos;
                        camPos.x += _camera.orthographicSize * camAspect * 2;
                    }
                }
                else
                {
                    for (int i = 0; i < _splittedCameras.Count; i++)
                    {
                        DestroyImmediate(_splittedCameras[i].gameObject);
                    }
                    _splittedCameras.Clear();
                }
            }
        }
    }
}