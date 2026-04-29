using UnityEngine;

namespace ML.T_Sports.Common
{
    public class AutoRotator : MonoBehaviour
    {
        public float anglePerSecond = 90.0f;
        public Vector3 axis = Vector3.up;
        public bool isLocal = true;

        public void Update()
        {
            transform.Rotate(axis, anglePerSecond * Time.deltaTime, isLocal ? Space.Self : Space.World);
        }
    }
}