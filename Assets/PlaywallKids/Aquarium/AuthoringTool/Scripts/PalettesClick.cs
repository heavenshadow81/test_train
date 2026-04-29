using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    using AuthoringTool;
    public class PalettesClick : MonoBehaviour
    {
        public UIPanel target;
        public Canvas_ canvas;
        public UserData.PaletteColor paletteColor = UserData.PaletteColor.BLACK;
        public Color col = Color.white;
        public Color pastelCol = Color.white;

        protected UISprite crayonSprite;
        protected UISprite markerSprite;
        protected UISprite spraySprite;

        protected UserData userData;
        protected int nInstanceId = 0;

        void Start()
        {
            nInstanceId = target.GetInstanceID();
            userData = UserData.Instance();

            foreach (UIPanel panel in target.GetComponentsInChildren<UIPanel>())
            {
                if (panel.name.Equals("Crayon Panel"))
                {
                    crayonSprite = panel.GetComponentInChildren<UISprite>();
                }
                if (panel.name.Equals("Marker Panel"))
                {
                    markerSprite = panel.GetComponentInChildren<UISprite>();
                }
                if (panel.name.Equals("Spray Panel"))
                {
                    spraySprite = panel.GetComponentInChildren<UISprite>();
                }
            }
        }

        void OnClick()
        {
            userData.SetPaletteColor(nInstanceId, paletteColor);
            Color c = col;
            if (userData.GetBrushTool(nInstanceId) == UserData.BrushTool.PASTEL)
                c = pastelCol;
            userData.SetColor(nInstanceId, col);

            if (crayonSprite != null)
                crayonSprite.spriteName = string.Concat("SB_T_T_03_", this.name.Replace(" Button", ""));
            if (markerSprite != null)
                markerSprite.spriteName = string.Concat("SB_T_T_07_", this.name.Replace(" Button", ""));
            if (spraySprite != null)
                spraySprite.spriteName = string.Concat("SB_T_T_04_", this.name.Replace(" Button", ""));

            if (canvas != null)
            {
                if(canvas.brush.brushName.Equals(BrushSet.kBrushNameEraser))
                    canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameMarker);
                canvas.brush.color = col;
            }
        }
    }
}