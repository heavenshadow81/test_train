using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCameraToggle : MonoBehaviour
{
    public Camera targetCamera;

    private void OnBecameVisible()
    {
        targetCamera.enabled = true;
        //Debug.Log("screen camera enabled");
    }

    private void OnBecameInvisible()
    {
        if (targetCamera != null && targetCamera.gameObject != null)
            targetCamera.enabled = false;
        //Debug.Log("screen camera disabled");
    }
}
