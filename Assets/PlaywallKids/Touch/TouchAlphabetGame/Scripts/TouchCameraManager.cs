using UnityEngine;
using System.Collections;

public class TouchCameraManager : MonoBehaviour {
    public Camera gameCamera;
    public Camera nguiCamera;
    public BarrelDistortionEffect lensCalibration;

    public Vector3 WorldToNGUIScreen(Vector3 pos)
    {
        pos = gameCamera.WorldToViewportPoint(pos);
        pos = gameCamera.ViewportToScreenPoint(pos);
        if (lensCalibration != null)
        { pos = lensCalibration.GetOriginalScreenPosFromDistorted(pos); }
        pos = gameCamera.ScreenToViewportPoint(pos);
        pos = nguiCamera.ViewportToWorldPoint(pos);
        pos.z = 0f;
        return pos;
    }

    
}
