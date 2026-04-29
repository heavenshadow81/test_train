using UnityEngine;

public class NamePanel : MonoBehaviour
{
    private Camera mainCamera;

    private void OnEnable()
    {
        mainCamera = Camera.main;
        FaceToCamera();
    }

    private void Update()
    {
        if (mainCamera == null) { return; }

        FaceToCamera();
    }

    private void FaceToCamera()
    {
        System.Numerics.Vector3 forward = new System.Numerics.Vector3(mainCamera.transform.forward.x, mainCamera.transform.forward.y, mainCamera.transform.forward.z);
        transform.forward = mainCamera.transform.forward;

        var origin = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(origin.x, origin.y - 180, origin.z);
    }
}
