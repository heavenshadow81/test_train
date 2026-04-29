using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TwoDimensionCanvas : Canvas_
    {

        float fHeightRaion
        {
            get
            {
                return heightRatio;
            }
            set
            {
                heightRatio = Mathf.Clamp(value, 0.1f, 1f);
            }
        }

        [Range(0.1f, 1f)]
        public float heightRatio;

        int center;

        float _height;
        float height
        {
            get
            {
                if (_height <= 0 || _height > 1f)
                {
                    _height = center * fHeightRaion;
                }
                return _height;
            }
        }

        Canvas_ _canvas;
        Canvas_ canvas
        {
            get
            {
                if (_canvas == null)
                {
                    _canvas = this.gameObject.GetComponent<Canvas_>();
                }
                return _canvas;
            }
        }

        public override void Start()
        {
            base.Start();
            center = (int)(canvasSize.y / 2);
            fHeightRaion = 0.73f;

        }
        // Update is called once per frame
        public override void Update()
        {
            // draws only if wantsPaint flag is on.
            if (wantsPaint)
            {
                TouchInfo[] touches = CustomInput.touches;
                // size of the texture
                Vector2 size = uiTexture.localSize;

                // canvas rect
                Rect rect = new Rect(0, 0, size.x, size.y);

                // iterate all touches


                for (int i = 0, touchCount = CustomInput.touchCount; i < touchCount; i++)
                {
                    TouchInfo t = touches[i];

                    Vector2 canvasPos = screenToCanvasMatrix * new Vector4(t.axisX, t.axisY, 0, 1.0f);

                    if (canvasPos.y < center - height || center + height < canvasPos.y) continue; //check input area

                    // check whether touch position is on the canvas region.
                    if (rect.Contains(canvasPos))
                    {
                        // draw
                        _DrawForTouch(t, ScreenToCanvas(t.position));

                        // if single touch mode, break loop.
                        if (!supportsMultiTouch)
                        {
                            break;
                        }
                    }
                }

                // upload to texture
                if (touches.Length > 0)
                    Flush();
            }
        }

        void OnEnable()
        {
            canvas.uiTexture.depth = 2;
        }
        void OnDisable()
        {
            canvas.ClearCanvas();
        }
    }
}