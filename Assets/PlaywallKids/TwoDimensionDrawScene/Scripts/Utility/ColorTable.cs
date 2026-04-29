using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class ColorTable
    {
        Color[] c = new Color[]{
            new Color(0f, 0.329f, 1f ),
            new Color(0f, 1f,     0.965f),
            new Color(1f, 0.706f, 0f),
            new Color(1f, 0f,     0.4f),
            new Color(1f, 0f,     0.965f),
            new Color(1f, 0f,     0f),
            new Color(0.047f, 1f, 0f),
            new Color(0.635f, 0f, 1f),
            new Color(1f,     1f, 1f),
            new Color(1f, 0.918f, 0f),
        };

        public Color32 GetRandomColor()
        {
            int idx = Random.Range(0, c.Length);
            if (idx >= c.Length)
            {
                idx = c.Length - 1;
            }
            return c[idx];
        }

        static ColorTable instance;
        /// <summary>
        /// 랜덤 색상
        /// </summary>
        /// <returns></returns>
        public static Color32 GetColor()
        {
            if (instance == null) instance = new ColorTable();
            Color c = instance.GetRandomColor();
            if ((c.r + c.g + c.b) == 3f)
            {
                c.g -= 0.8f;
                c.b -= 0.8f;
            }
            return c;
        }
    }
}