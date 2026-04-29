using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PanoramicCamera : MonoBehaviour
{
    [Range(1,36)]
    public int cameraCount = 3;
    public float fov = 60.0f;
    public int depth = 0;

    private List<Camera> _cams = new List<Camera>();
    private float[] _cachedAngles = new float[4];

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

        // Theta : Vertical Angle, Pi : Horizontal Angle
        float theta = 0.0f, pi = 0.0f;

        while (_cachedAngles.Length < cameraCount)
            _cachedAngles = new float[_cachedAngles.Length * 2];
        if (_cams.Count == 0)
        {
            Camera[] cams = GetComponentsInChildren<Camera>();
            for (int i = 0; i < cams.Length; i++)
            {
                if (cams[i].gameObject != gameObject) _cams.Add(cams[i]);
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
            }
        }

        float totalAngle = 0.0f;
        for (int i = 0; i < cameraCount; i++)
        {
            Camera cam = _cams[i];

            if (cam != null)
            {
                cam.gameObject.SetActive(true);
                cam.enabled = true;
                cam.name = string.Format("camera_{0:00}", i + 1);

                Rect camRect = cam.rect;
                camRect.x = (float)i / (float)cameraCount;
                camRect.y = 0;
                camRect.width = 1.0f / (float)cameraCount;
                camRect.height = 1.0f;
                cam.rect = camRect;

                theta = fov * Mathf.Deg2Rad;
                pi = 2 * Mathf.Atan(aspect * cam.rect.width * Mathf.Tan(theta * 0.5f));

                float angle = pi * Mathf.Rad2Deg;
                _cachedAngles[i] = angle;
                totalAngle += angle;
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
                if (Mathf.Abs(cam.fieldOfView - fov) > 0.0f)
                    cam.fieldOfView = fov;
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
