using UnityEngine;

namespace ML.PlaywallKids.Common
{
    /// <summary>
    /// 스크린 종류. 16x9 패널 기준 [세로]x[가로] 개수를 의미한다.
    /// </summary>
    public enum ScreenType
    {
        /// <summary>
        /// 알 수 없음.
        /// </summary>
        None,
        /// <summary>
        /// 2x2 패널 (16:9)
        /// </summary>
        Bigboard2x2,
        /// <summary>
        /// 2x3 패널 (24:9)
        /// </summary>
        Bigboard2x3,
        /// <summary>
        /// 2x4 패널 (32:9)
        /// </summary>
        Bigboard2x4,
        /// <summary>
        /// 2x6 패널 (48:9)
        /// </summary>
        Bigboard2x6
    }

    public static partial class ScreenUtil
    {
        /// <summary>
        /// 첫번째 디스플레이의 스크린 종류.
        /// </summary>
        public static ScreenType screenType
        {
            get { return GetScreenType(0); }
        }

        /// <summary>
        /// 첫번째 디스플레이의 스크린 너비 (pixel).
        /// </summary>
        public static float screenWidth
        {
            get { return GetScreenWidth(0); }
        }

        /// <summary>
        /// 첫번째 디스플레이의 스크린 높이 (pixel).
        /// </summary>
        public static float screenHeight
        {
            get { return GetScreenHeight(0); }
        }
        
        /// <summary>
        /// 첫번째 디스플레이의 스크린 종횡비.
        /// </summary>
        public static float aspectRatio
        {
            get { return GetScreenAspectRatio(0); }
        }

        public static ScreenType GetScreenType(int displayIndex)
        {
            if (Display.displays.Length <= displayIndex)
                return ScreenType.None;
            else
            {
                float width = Display.displays[displayIndex].renderingWidth;
                float height = Display.displays[displayIndex].renderingHeight;
                float aspectRatio = width / height;

                ScreenType screenType = ScreenType.Bigboard2x2;
                if (aspectRatio >= 1.7f)
                    screenType = ScreenType.Bigboard2x2;
                if (aspectRatio >= 2.5f)
                    screenType = ScreenType.Bigboard2x3;
                if (aspectRatio >= 3.5f)
                    screenType = ScreenType.Bigboard2x4;
                if (aspectRatio >= 5f)
                    screenType = ScreenType.Bigboard2x6;

                return screenType;
            }
        }

        public static float GetScreenWidth(int displayIndex)
        {
            if (Display.displays.Length <= displayIndex)
                return Screen.width;
            else
                return Display.displays[displayIndex].renderingWidth;
        }

        public static float GetScreenHeight(int displayIndex)
        {
            if (Display.displays.Length <= displayIndex)
                return Screen.height;
            else
                return Display.displays[displayIndex].renderingHeight;
        }

        public static float GetScreenAspectRatio(int displayIndex)
        {
            float width = GetScreenWidth(displayIndex);
            float height = GetScreenHeight(displayIndex);
            float aspectRatio = width / height;
            return aspectRatio;
        }
    }
}