using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TwoDimensionWindowPanel : TwoDimensionPanel
    {

        bool bInit;

        public override void Awake()
        {
            base.Awake();
            cSketchCanvas.textureSize = new Vector2(2048, 1024);
            //cSketchCanvas.textureSize = new Vector2(4096, 2048);
        }

        void Start()
        {
            //Debug.Log("Window Start begin");
            //cSketchCanvas.FillColor( new Color(100, 100, 100, 20) );
            //Debug.Log("Window Start end");
            FillWindow();
            cSketchCanvas.brush = GetBrush();
        }

        void Update()
        {
            if (!bInit)
            {
                bInit = true;
                FillWindow();
                //color init
            }
        }

        public void FillWindow()
        {
            cSketchCanvas.FillColor(new Color(1.0f, 1.0f, 1.0f, 0.5f));
            cSketchCanvas.FillColor(new Color(1.0f, 1.0f, 1.0f, 0.5f));
        }

        public override void OnDisable()
        {
            base.OnDisable();
            bInit = false;
        }


        protected override Brush GetBrush()
        {
            Debug.Log("derived GetBrush");
            if (cStampBush == null)
            {
                cStampBush = CustomBrush.brush.Eraser();
                cStampBush.diameter = 40.0f;
            }
            return cStampBush;
        }
    }

    /*
     * Color c = new Color(100, 100, 100, 100);
            Debug.Log("Window Start begin");
            for(int h = 0 ; h < cSketchCanvas.texture.height ; ++h)
            {
                for(int w = 0 ; w < cSketchCanvas.texture.width ; ++w)
                {
                    cSketchCanvas.texture.SetPixel(w,h,c);
                }
            }
            Debug.Log("Window Start end");
            for(int h = 0 ; h < cSketchCanvas.texture.height ; ++h)
            {
                for(int w = 0 ; w < cSketchCanvas.texture.width ; ++w)
                {
                    cSketchCanvas.texture.SetPixel(w,h,c);
                }
            }
     */
}