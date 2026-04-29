using UnityEngine;

namespace Coding
{
    public class Spin : MonoBehaviour
    {
        public float turnDegPerSec = 360;
        public Vector3 localAxis;
        protected float turnDegPerFrame;

        public void Start()
        {
            turnDegPerFrame = turnDegPerSec * Time.deltaTime;
        }

        public void Update()
        {
            transform.Rotate(localAxis, turnDegPerFrame);
        }
    }
}
