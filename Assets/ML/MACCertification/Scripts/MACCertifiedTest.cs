using UnityEngine;
using UnityEngine.UI;

namespace ML.MACCertification
{
    /// <summary>
    /// 테스트 클래스.
    /// </summary>
    public class MACCertifiedTest : MonoBehaviour
    {
        public Text text;

        public void CheckLicense()
        {
            if (MACCertified.IsPlayable())
            {
                text.text = "Valid license.";
            }
            else
            {
                text.text = "Invalid license. Please verify \"License.mgl\" file or your PC's MAC address.";
            }
        }
    }
}