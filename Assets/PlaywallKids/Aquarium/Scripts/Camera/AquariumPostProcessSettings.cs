using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    using UnityEngine.PostProcessing;
    [RequireComponent(typeof(PostProcessingBehaviour))]
    [ExecuteInEditMode]
    public class AquariumPostProcessSettings : MonoBehaviour
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