using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    public class EASClientCanvasControl : EASAnimatablePanel
    {
        #region Public variables
        public EASClientCanvas canvas;
        public Transform[] palettesList;
        #endregion

        #region Properties
        private Color32 _brushColor = Color.black;
        public Color32 brushColor
        {
            get
            {
                return _brushColor;
            }
            set
            {
                _brushColor = value;
                if (canvas.brush != null)
                {
                    Marker();
                    canvas.brush.color = brushColor;
                }
            }
        }

        public override EASSocket socket
        {
            get
            {
                return canvas.socket;
            }
            set
            {
                canvas.socket = value;
            }
        }
        #endregion

        public void Start()
        {
            UITexture uiTexture = canvas.uiTexture;

            _InitPalettes();
            brushColor = Color.black;
        }

        private void _InitPalettes()
        {
            if (palettesList != null && palettesList.Length > 0)
            {
                foreach (Transform palettes in palettesList)
                {
                    if (palettes == null) continue;

                    for (int i = 0; i < palettes.childCount; i++)
                    {
                        Transform child = palettes.GetChild(i);
                        UIEventTrigger trigger = child.GetComponent<UIEventTrigger>();
                        if (trigger == null)
                        {
                            trigger = child.gameObject.AddComponent<UIEventTrigger>();
                        }
                        if (trigger.onClick.Count == 0)
                        {
                            trigger.onClick.Add(new EventDelegate(this, child.name));
                            //trigger.onClick.Add(new EventDelegate(_ShowBrushColor));
                        }
                    }
                }
            }
        }

        public override void BeginShow()
        {
            base.BeginShow();

            canvas.ClearCanvas();
        }

        public void Marker()
        {
            canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameMarker);
            canvas.brush.color = brushColor;
            canvas.brush.diameter = 12.0f;
        }

        public void Crayon()
        {
            canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameCrayon);
            canvas.brush.color = brushColor;
        }

        public void Airbrush()
        {
            canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameAirbrush);
            canvas.brush.color = brushColor;
        }

        public void Pastel()
        {
            canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNamePastel);
            canvas.brush.color = brushColor;
        }

        public void Rainbow()
        {
            canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameRainbow);
            canvas.brush.color = brushColor;
        }

        public void Eraser()
        {
            Marker();
            canvas.brush.color = Color.white;
            canvas.brush.diameter = 60.0f;
            //		canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameEraser);
            //		canvas.brush.color = Color.white;
        }

        public void Black()
        {
            brushColor = new Color32(0, 0, 0, 255);
        }

        public void Blue()
        {
            brushColor = new Color32(0, 143, 255, 255);
        }

        public void BlueViolet()
        {
            brushColor = new Color32(0, 59, 255, 255);
        }

        public void DarkGreen()
        {
            brushColor = new Color32(70, 161, 85, 255);
        }

        public void Green()
        {
            brushColor = new Color32(0, 255, 14, 255);
        }

        public void Orange()
        {
            brushColor = new Color32(234, 143, 0, 255);
        }

        public void Pink()
        {
            brushColor = new Color32(249, 183, 186, 255);
        }

        public void Red()
        {
            brushColor = new Color32(255, 0, 0, 255);
        }

        public void Skyblue()
        {
            brushColor = new Color32(0, 203, 255, 255);
        }

        public void Violet()
        {
            brushColor = new Color32(126, 18, 223, 255);
        }

        public void VioletRed()
        {
            brushColor = new Color32(223, 16, 160, 255);
        }

        public void White()
        {
            brushColor = new Color32(255, 255, 255, 255);
        }

        public void Yellow()
        {
            brushColor = new Color32(255, 255, 0, 255);
        }

        public void YellowGreen()
        {
            brushColor = new Color32(213, 255, 0, 255);
        }
    }
}