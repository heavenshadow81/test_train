using UnityEngine;

namespace ML.MLBKids
{
    public class GlobalConstants : MonoBehaviour
    {
        public float bodyDetectionTime = 2.0f;
        public float anyActionWaitTime = 10.0f;
        public float closeToScreenAnyActionWaitTime = 20.0f;
        public float tutorialStandWaitTime = 20.0f;
        public float tutorialReadyTime = 5.0f;
        public float tutorialStartButtonWaitTime = 10.0f;
        public float resultPageAnyActionWaitTime = 20.0f;

        // for gesture detection
        public float gestureReadyThreshold = 0.1f;
        public float gestureDepthThreshold = 0.3f;
        public int gestureEffectFrameCount = 20;
        public float gestureEffectDistance = 0.3f;
        
        public static GlobalConstants instance
        {
            get; private set;
        }

        public void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }

        public void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}