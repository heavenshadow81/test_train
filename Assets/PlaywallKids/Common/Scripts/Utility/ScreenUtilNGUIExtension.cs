using UnityEngine;

namespace ML.PlaywallKids.Common
{
    public static partial class ScreenUtil
    {
        public static float NGUIWidth
        {
            get
            {
                return NGUIHeight * aspectRatio;
            }
        }

        public static float NGUIHeight
        {
            get
            {
                float screenHeight = defaultNGUIScreenHeight;
                if (UIRoot.list.Count > 0)
                    screenHeight = UIRoot.list[0].activeHeight;
                else
                {
                    UIRoot root = Object.FindObjectOfType<UIRoot>();
                    if (root != null) screenHeight = root.activeHeight;
                }
                return screenHeight;
            }
        }

        public static float NGUIScaleRatio
        {
            get
            {
                return NGUIHeight / Screen.height;
            }
        }

        // PlaywallKids의 기본 UI screen height는 1440.
        public const float defaultNGUIScreenHeight = 1440.0f;

        public static Vector2 ViewportToNGUIScreen(float x, float y)
        {
            return ViewportToNGUIScreen(new Vector2(x, y));
        }

        public static Vector2 ViewportToNGUIScreen(Vector2 viewPort)
        {
            Vector2 size = new Vector2(NGUIWidth, NGUIHeight);
            return new Vector2(
                Mathf.Lerp(-size.x * 0.5f, size.x * 0.5f, viewPort.x),
                Mathf.Lerp(-size.y * 0.5f, size.y * 0.5f, viewPort.y)
                );
        }
    }
}