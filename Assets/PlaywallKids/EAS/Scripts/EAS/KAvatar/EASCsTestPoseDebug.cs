using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    public class EASCsTestPoseDebug : MonoBehaviour
    {
        public EASCsTestPose testPose;

        public void Start()
        {
            if (testPose == null)
            {
                testPose = GetComponent<EASCsTestPose>();
            }
        }

        public void OnGUI()
        {
            if (GUI.Button(new Rect(20, 20, 100, 20), "Play"))
            {
                testPose.Play();
            }

            if (GUI.Button(new Rect(140, 20, 100, 20), "Stop"))
            {
                testPose.Stop();
            }
        }
    }
}