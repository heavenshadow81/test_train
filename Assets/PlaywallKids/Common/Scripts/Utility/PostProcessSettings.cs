using UnityEngine;

namespace ML.PlaywallKids.Common
{
    using UnityEngine.PostProcessing;
    [RequireComponent(typeof(PostProcessingBehaviour))]
    [ExecuteInEditMode]
    public class PostProcessSettings : MonoBehaviour
    {
        public void Start()
        {
            PostProcessingBehaviour ppb = GetComponent<PostProcessingBehaviour>();
            ppb.enabled = QualitySettings.GetQualityLevel() > 3;
        }

#if UNITY_EDITOR
        public void Update()
        {
            PostProcessingBehaviour ppb = GetComponent<PostProcessingBehaviour>();
            ppb.enabled = QualitySettings.GetQualityLevel() > 3;
        }
#endif
    }
}