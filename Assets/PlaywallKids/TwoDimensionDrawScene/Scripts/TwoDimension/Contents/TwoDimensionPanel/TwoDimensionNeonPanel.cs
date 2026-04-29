using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TwoDimensionNeonPanel : TwoDimensionPanel
    {
        public AudioClip[] clips;

        ColorTable _cTable;
        ColorTable cTable
        {
            get
            {
                if (_cTable == null)
                {
                    _cTable = new ColorTable();
                }
                return _cTable;
            }
        }

        public Camera neonCamera;
        [Range(0, 1f)]
        public float fHardness;
        [Range(0, 1f)]
        public float fFlow;
        [Range(0, 1f)]
        public float fOpacity;
        [Range(0, 1f)]
        public float fSpacing;
        [Range(0, 1f)]
        public float fHueJitter;
        [Range(0, 1f)]
        public float fSaturationJitter;
        [Range(0, 1f)]
        public float fBrightnessJitter;

        AudioSource _audio;
        AudioSource mAudio
        {
            get
            {
                if (null == _audio)
                {
                    _audio = GetComponent<AudioSource>();
                    if (_audio == null)
                    {
                        gameObject.AddComponent<AudioSource>();
                        _audio = GetComponent<AudioSource>();
                    }
                }
                return _audio;
            }
        }

        public override void Awake()
        {
            base.Awake();
            cSketchCanvas.brush = GetBrush();
            cSketchCanvas.textureSize = new Vector2(4096, 2048);

            // add neon collision check
            cSketchCanvas.drawMode = Canvas_.DrawMode.COLLISION;

            UIPanel panel = GetComponent<UIPanel>();
            neonCamera.aspect = (float)Screen.width / Screen.height;
        }

        protected override Brush GetBrush()
        {
            Debug.Log("derived GetBrush");
            if (cStampBush == null)
            { cStampBush = CustomBrush.brush.Marker(); }
            cStampBush.color = cTable.GetRandomColor();

            fSpacing = (fSpacing == 0) ? 0.15f : fSpacing;
            cStampBush.spacing = fSpacing;

            fHardness = (fHardness == 0) ? 0.7f : fHardness;
            cStampBush.hardness = fHardness;

            fOpacity = (fOpacity == 0) ? 0.68f : fOpacity;
            cStampBush.opacity = fOpacity;

            fFlow = (fFlow == 0) ? 0.9f : fFlow;
            cStampBush.flow = fFlow;

            fHueJitter = (fHueJitter == 0) ? Random.Range(0, 1f) : fHueJitter;
            cStampBush.colorDynamicComponent.hueJitter = fHueJitter;

            fSaturationJitter = (fSaturationJitter == 0) ? Random.Range(0, 1f) : fSaturationJitter;
            cStampBush.colorDynamicComponent.saturationJitter = fSaturationJitter;

            fBrightnessJitter = (fBrightnessJitter == 0) ? Random.Range(0, 1f) : fBrightnessJitter;
            cStampBush.colorDynamicComponent.brightnessJitter = fBrightnessJitter;

            if (clips != null && clips.Length > 0)
                AudioSource.PlayClipAtPoint(clips[Random.Range(0, clips.Length)], Vector3.zero);
            return cStampBush;
        }
    }
}