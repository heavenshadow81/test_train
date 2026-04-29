using UnityEngine;

namespace ML.MapoContents.WorldTravel
{
    public static class CameraRotation
    {
        public static void Pitch(Transform transform, float delta)
        {
            Vector3 right = transform.right;
            Vector3 up = transform.up;
            Vector3 forward = transform.forward;

            Quaternion q = Quaternion.AngleAxis(delta * 0.25f, right);
            Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
            up = m.MultiplyVector(up);
            forward = m.MultiplyVector(forward);
            if (Mathf.Abs(Vector3.Dot(Vector3.up, forward)) < 0.9f)
            {
                transform.up = up;
                transform.forward = forward;
            }
        }

        public static void RotateY(Transform transform, float delta)
        {
            Vector3 right = transform.right;
            Vector3 up = transform.up;
            Vector3 forward = transform.forward;

            Quaternion q = Quaternion.AngleAxis(-delta * 0.25f, Vector3.up);
            Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
            right = m.MultiplyVector(right);
            up = m.MultiplyVector(up);
            forward = m.MultiplyVector(forward);
            transform.right = right;
            transform.up = up;
            transform.forward = forward;
        }
    }
}