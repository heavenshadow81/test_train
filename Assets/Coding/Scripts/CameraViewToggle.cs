using UnityEngine;

public class CameraViewToggle : MonoBehaviour
{
    [SerializeField]
    private Coding.UIController controller;

    public void OnValueChanged(bool value)
    {
        controller.SetActivePrivateCamera(value);
    }
}