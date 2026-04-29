using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ML.PlaywallKids.Launcher
{
    /// <summary>
    /// 런처 로딩 문구
    /// </summary>
    public class LauncherLoadingText : MonoBehaviour
    {
        IEnumerator Start()
        {
            Text text = GetComponent<Text>();
            UILabel uiLabel = GetComponent<UILabel>();

            int dotCount = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            while (true)
            {
                if (sb.Length > 0) sb.Remove(0, sb.Length);
                sb.Append("Loading");
                for (int i = 0; i < dotCount; i++) sb.Append(".");

                if (text != null) text.text = sb.ToString();
                else if (uiLabel != null) uiLabel.text = sb.ToString();

                yield return new WaitForSeconds(0.5f);
                dotCount = (dotCount + 1) % 4;
            }
        }
    }
}