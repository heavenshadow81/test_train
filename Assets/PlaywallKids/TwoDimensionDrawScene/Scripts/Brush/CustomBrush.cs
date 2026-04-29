using UnityEngine;
using com.Loxwell.File;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    [RequireComponent(typeof(FileName))]
    public class CustomBrush
    {

        private static CustomBrush _brush;
        public static CustomBrush brush
        {
            get
            {
                if (_brush == null)
                {
                    _brush = new CustomBrush();
                }
                return _brush;
            }
        }

        Color mColor;
        Brush mSandBrush;
        Brush mEraserBrush;
        Brush mMarkerBrush;

        public CustomBrush()
        {
            mSandBrush = new Brush();
            mEraserBrush = new Brush();
            mMarkerBrush = new Brush();
        }

        public Brush SandBrush()
        {
            if (mSandBrush == null) mSandBrush = new Brush();
            mSandBrush.brushName = "SandBrush";
            mSandBrush.maskType = MaskType.PixelMask;
            mSandBrush.maskName = FileName.SandBrush;
            mSandBrush.blendMode = BlendMode.AlphaBlend;
            mSandBrush.useMaskColor = true;
            mSandBrush.diameter = 25.0f;
            mSandBrush.color = Color.white;
            mSandBrush.spacing = 0.25f;
            mSandBrush.opacity = 0.9f;
            mSandBrush.flow = 0.8f;
            mSandBrush.airbrush = true;

            mSandBrush.useAlphaBuffer = true;
            mSandBrush.paintOnDrag = true;
            mSandBrush.paintStartPosition = false;

            mSandBrush.shapeDynamicComponent.enable = true;
            mSandBrush.shapeDynamicComponent.angleControl = AngleControl.Direction;

            return mSandBrush;
        }

        public Brush RandomColorStamp(string _name)
        {
            Brush mStampBrush = new Brush();
            mStampBrush.brushName = _name;
            mStampBrush.maskType = MaskType.PixelMask;
            mStampBrush.maskName = _name;//;


            mStampBrush.useMaskColor = false;
            mStampBrush.diameter = 100f;
            mStampBrush.blendMode = BlendMode.Multiply;
            mColor = new Color(1f, 1f, 1f);
            mStampBrush.color = mColor;
            mStampBrush.spacing = 0.15f;
            mStampBrush.opacity = 0.65f;
            mStampBrush.flow = 1.0f;
            mStampBrush.airbrush = false;
            mStampBrush.useAlphaBuffer = false;
            mStampBrush.paintOnDrag = false;
            //mBrush.paintStartPosition = true; 
            mStampBrush.colorDynamicComponent.enable = true;
            mStampBrush.colorDynamicComponent.hueJitter = Random.Range(0, 360f);
            mStampBrush.colorDynamicComponent.saturationJitter = Random.Range(0, 1f);
            mStampBrush.colorDynamicComponent.brightnessJitter = Random.Range(0, 1f);
            mStampBrush.shapeDynamicComponent.enable = true;
            mStampBrush.shapeDynamicComponent.angleControl = AngleControl.Direction;

            return mStampBrush;
        }


        public Brush Eraser()
        {
            if (mEraserBrush == null) mEraserBrush = new Brush();
            mEraserBrush.diameter = 20.0f;
            mEraserBrush.color = Color.black;
            mEraserBrush.spacing = 0.06f;
            mEraserBrush.hardness = 0.5f;
            mEraserBrush.brushName = "Eraser";

            mEraserBrush.angle = 0;

            mEraserBrush.paintStartPosition = true;
            mEraserBrush.useAlphaBuffer = false;

            mEraserBrush.shapeDynamicComponent.enable = true;
            mEraserBrush.shapeDynamicComponent.sizeJitter = 1.0f;
            mEraserBrush.shapeDynamicComponent.minimumDiameter = 0.0f;
            mEraserBrush.shapeDynamicComponent.angleJitter = 6.1f;
            mEraserBrush.shapeDynamicComponent.angleControl = AngleControl.Off;
            mEraserBrush.shapeDynamicComponent.roundnessJitter = 1f;
            mEraserBrush.shapeDynamicComponent.minimumRoundness = 0.5f;
            mEraserBrush.maskType = MaskType.PixelMask;

            mEraserBrush.maskName = "round_blur";
            mEraserBrush.opacity = 0.7f;
            mEraserBrush.flow = 0.85f;
            mEraserBrush.useMaskColor = true;
            mEraserBrush.airbrush = false;
            mEraserBrush.blendMode = BlendMode.MultiplyRaw;

            mEraserBrush.paintStartPosition = false;
            mEraserBrush.paintOnDrag = true;


            return mEraserBrush;
        }


        public Brush Marker()
        {
            // Marker
            if (mMarkerBrush == null) mMarkerBrush = new Brush();
            mMarkerBrush.brushName = "Marker";

            mMarkerBrush.maskType = MaskType.VectorMask;
            mMarkerBrush.diameter = 25.0f;
            mMarkerBrush.color = Color.black;
            mMarkerBrush.spacing = 0.15f;
            mMarkerBrush.hardness = 0.7f;
            mMarkerBrush.opacity = .68f;
            mMarkerBrush.flow = 1f;
            mMarkerBrush.airbrush = false;
            mMarkerBrush.paintStartPosition = true;
            mMarkerBrush.useAlphaBuffer = true;

            mMarkerBrush.paintOnDrag = true;
            mMarkerBrush.paintStartPosition = false;

            return mMarkerBrush;
        }
    }
}