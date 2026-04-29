using UnityEngine;
using System.Collections;

// Referenced from
// http://www.decarpentier.nl/lens-distortion


public enum PositionType
{
    Screen,
    ViewPort,
    WorldPoint
}


[ExecuteInEditMode]
public class BarrelDistortionEffect : MonoBehaviour {
    public Shader shader;

    [Range(0, 1)]
    public float strength = 1.0f;

    [Range(0, 4)]
    public float cylindricalRatio = 0.5f;

    public bool debug = false;

    private Camera _cam;
    private Material _mat;

    public void Start()
    {
        _cam = GetComponent<Camera>();

        _mat = new Material(shader);

        Update();
    }

    public void Update()
    {
#if UNITY_EDITOR
        var gameViewSize = UnityEditor.Handles.GetMainGameViewSize();
        float aspect = gameViewSize.x / gameViewSize.y;
#else
		float aspect = (float)Screen.width / (float)Screen.height;
#endif

        if(_mat != null)
        {
            if (_mat.HasProperty("_AspectRatio"))
                _mat.SetFloat("_AspectRatio", aspect);
            if (_mat.HasProperty("_FOV") && _cam != null)
                _mat.SetFloat("_FOV", _cam.fieldOfView);
            if (_mat.HasProperty("_Strength"))
                _mat.SetFloat("_Strength", strength);
            if (_mat.HasProperty("_CylindricalRatio"))
                _mat.SetFloat("_CylindricalRatio", cylindricalRatio);
        }
    }

    public Vector3 GetDistoredScreenPosFromOriginal(Vector3 pos)
    {
        Vector2 result = pos;

        // FOV, Aspect Ratio, Height
#if UNITY_EDITOR
        var gameViewSize = UnityEditor.Handles.GetMainGameViewSize();
        float screenWidth = gameViewSize.x;
        float screenHeight = gameViewSize.y;
#else
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
#endif
        float aspectRatio = screenWidth / screenHeight;

        float fov = 0.0f;
        if (_cam != null) fov = _cam.fieldOfView;
        float height = Mathf.Tan(fov * Mathf.Deg2Rad * 0.5f);

        // Calculate required variables
        float scaledHeight = strength * height;
        float cylAspectRatio = aspectRatio * cylindricalRatio;
        float aspectDiagSq = aspectRatio * aspectRatio + 1.0f;
        float diagSq = scaledHeight * scaledHeight * aspectDiagSq;

        float z = 0.5f * Mathf.Sqrt(diagSq + 1.0f) + 0.5f;
        float ny = (z - 1.0f) / (cylAspectRatio * cylAspectRatio + 1.0f);

        // Convert screen space to viewport space [-1, 1]
        Vector2 p = new Vector2(2.0f * pos.x / screenWidth - 1.0f, 2.0f * pos.y / screenHeight - 1.0f);

        // Calculate!
        float div = z - (ny * cylAspectRatio * cylAspectRatio * p.x * p.x + ny * p.y * p.y);

        result = p / div;

        result = result * 0.5f + Vector2.one * 0.5f;

        result.x *= screenWidth;
        result.y *= screenHeight;

        return new Vector3(result.x, result.y, pos.z);
    }

    /// <summary>
    /// Gets the original screen coordinate from distorted screen coordinate.
    /// </summary>
    /// <param name="pos">Distorted screen position</param>
    /// <returns>The reverted original screen position</returns>
    public Vector3 GetOriginalScreenPosFromDistorted(Vector3 pos)
    {
        Vector2 result = pos;
        
        // FOV, Aspect Ratio, Height
#if UNITY_EDITOR
        var gameViewSize = UnityEditor.Handles.GetMainGameViewSize();
        float screenWidth = gameViewSize.x;
        float screenHeight = gameViewSize.y;
#else
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
#endif
        float aspectRatio = screenWidth / screenHeight;
        float fov = 0.0f;
        if (_cam != null) fov = _cam.fieldOfView;
        float height = Mathf.Tan(fov * Mathf.Deg2Rad * 0.5f);

        // Calculate required variables
        float scaledHeight = strength * height;
        float cylAspectRatio = aspectRatio * cylindricalRatio;
        float aspectDiagSq = aspectRatio * aspectRatio + 1.0f;
        float diagSq = scaledHeight * scaledHeight * aspectDiagSq;

        float z = 0.5f * Mathf.Sqrt(diagSq + 1.0f) + 0.5f;
        float ny = (z - 1.0f) / (cylAspectRatio * cylAspectRatio + 1.0f);

        // Convert screen space to viewport space [-1, 1]
        Vector2 p = new Vector2(2.0f * pos.x / screenWidth - 1.0f, 2.0f * pos.y / screenHeight - 1.0f);

        // Calculate!
        float div = 0.5f + Mathf.Sqrt(0.25f + z * (ny * cylAspectRatio * cylAspectRatio * p.x * p.x + ny * p.y * p.y));

        result = z * p / div;

        result = result * 0.5f + Vector2.one * 0.5f;

        result.x *= screenWidth;
        result.y *= screenHeight;

        return new Vector3(result.x, result.y, pos.z);
    }

    public void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_mat != null)
            Graphics.Blit(src, dest, _mat);
        else
            Graphics.Blit(src, dest);
    }

    public void OnDrawGizmos()
    {
        if (!debug) return;

        if (_cam == null)
            _cam = GetComponent<Camera>();

        Gizmos.color = Color.green;

#if UNITY_EDITOR
        var gameViewSize = UnityEditor.Handles.GetMainGameViewSize();
        float screenWidth = gameViewSize.x;
        float screenHeight = gameViewSize.y;
#else
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
#endif

        for (int i = 0; i < 11; i++)
        {
            float h = i * 0.1f;

            for (int j = 0; j < 11; j++)
            {
                float w = j * 0.1f;

                Vector3 from = GetDistoredScreenPosFromOriginal(new Vector2(screenWidth * w, screenHeight * h));
                Vector3 right = GetDistoredScreenPosFromOriginal(new Vector2(screenWidth * (w + 0.1f), screenHeight * h));
                Vector3 up = GetDistoredScreenPosFromOriginal(new Vector2(screenWidth * w, screenHeight * (h + 0.1f)));

                from.z = right.z = up.z = (_cam.nearClipPlane + _cam.farClipPlane) * 0.25f;
                from = _cam.ScreenToWorldPoint(from);
                right = _cam.ScreenToWorldPoint(right);
                up = _cam.ScreenToWorldPoint(up);

                if (j < 10) Gizmos.DrawLine(from, right);
                if (i < 10) Gizmos.DrawLine(from, up);
            }
        }


        Vector3 pos = Input.mousePosition;
        pos = GetDistoredScreenPosFromOriginal(pos);
        pos.z = (_cam.nearClipPlane + _cam.farClipPlane) * 0.25f;

        Vector3 to = _cam.ScreenToWorldPoint(pos);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, to);
    }

    public static Vector3 ConvertToDistorted(Camera cam, PositionType inoutType, Vector3 position)
    {
        return ConvertToDistorted(cam, inoutType, inoutType, position);
    }

    public static Vector3 ConvertToOriginal(Camera cam, PositionType inoutType, Vector3 position)
    {
        return ConvertToOriginal(cam, inoutType, inoutType, position);
    }

    public static Vector3 ConvertToDistorted(Camera cam, PositionType inputType, PositionType returnType, Vector3 inputPosition)
    {
        Vector3 pos = inputPosition;
        if (cam != null)
        {
            BarrelDistortionEffect effect = cam.GetComponent<BarrelDistortionEffect>();
            if (effect != null)
            {
                switch(inputType)
                {
                    case PositionType.ViewPort:
                        pos = cam.ViewportToScreenPoint(pos);
                        break;
                    case PositionType.WorldPoint:
                        pos = cam.WorldToScreenPoint(pos);
                        break;
                }

                pos = effect.GetDistoredScreenPosFromOriginal(pos);

                switch(returnType)
                {
                    case PositionType.ViewPort:
                        pos = cam.ScreenToViewportPoint(pos);
                        break;
                    case PositionType.WorldPoint:
                        pos = cam.ScreenToWorldPoint(pos);
                        break;
                }
            }
        }
        return pos;
    }

    public static Vector3 ConvertToOriginal(Camera cam, PositionType inputType, PositionType returnType, Vector3 inputPosition)
    {
        Vector3 pos = inputPosition;
        if (cam != null)
        {
            BarrelDistortionEffect effect = cam.GetComponent<BarrelDistortionEffect>();
            if (effect != null)
            {
                switch (inputType)
                {
                    case PositionType.ViewPort:
                        pos = cam.ViewportToScreenPoint(pos);
                        break;
                    case PositionType.WorldPoint:
                        pos = cam.WorldToScreenPoint(pos);
                        break;
                }

                pos = effect.GetOriginalScreenPosFromDistorted(pos);

                switch (returnType)
                {
                    case PositionType.ViewPort:
                        pos = cam.ScreenToViewportPoint(pos);
                        break;
                    case PositionType.WorldPoint:
                        pos = cam.ScreenToWorldPoint(pos);
                        break;
                }
            }
        }
        return pos;
    }
}
