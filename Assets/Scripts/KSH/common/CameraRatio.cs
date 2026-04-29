using UnityEngine;

public class CameraRatio : MonoBehaviour
{
    [SerializeField] private Camera cam; // Inspector에서 카메라를 직접 할당

    void Awake()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>(); // 카메라가 할당되지 않은 경우에만 GetComponent로 찾음
        }

        if (cam == null)
        {
            Debug.LogError($"{gameObject.name} : 카메라 컴포넌트 찾을 수 없음.");
            return;
        }

        AdjustCameraViewport();
    }

    void OnPreCull()
    {
        if (cam == null) return;
        GL.Clear(true, true, Color.black);
    }

    private void AdjustCameraViewport()
    {
        if (cam == null) return;

        Rect rect = cam.rect;
        float targetAspect = 16f / 9f;
        float screenAspect = (float)Screen.width / Screen.height;
        float scaleHeight = screenAspect / targetAspect;
        float scaleWidth = 1f / scaleHeight;

        if (scaleHeight < 1)
        {
            rect.height = scaleHeight;
            rect.y = (1f - scaleHeight) / 2f;
        }
        else
        {
            rect.width = scaleWidth;
            rect.x = (1f - scaleWidth) / 2f;
        }
        cam.rect = rect;
    }

    public void SetResolution(int width = 1920, int height = 1080)
    {
        if (cam == null) return;

        Screen.SetResolution(width, (int)(((float)Screen.height / Screen.width) * width), true);
        AdjustCameraViewport();
    }
}
