using UnityEngine;
using UnityEngine.UI;

namespace Coding
{
    public class CustomToggle : MonoBehaviour
    {
        [Header("Switch Toggle Object References")]
        [Tooltip("The toggle element to use.")]
        public Toggle toggle;

        public GameObject Handle;
        public Vector3 HandleOnPosition;
        public Vector3 HandleOffPosition;

        public void SetToggleGraphic()
        {
            Handle.transform.localPosition = toggle.isOn ? HandleOnPosition : HandleOffPosition;
        }
    }
}
