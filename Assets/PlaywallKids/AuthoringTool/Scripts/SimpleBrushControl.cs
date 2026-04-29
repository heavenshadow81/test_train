using UnityEngine;
using System.Collections;

public class SimpleBrushControl : MonoBehaviour {
	public UIPopupList blendModePopup;
	public Canvas_     canvas;
    public UITexture   background;
	public Brush       brush;

    public UIToggle GPGPUModeToggle;
    public UILabel GPGPUModeLabel;
    public UIPopupList resolutionPopup;

	public ColorField colorField;
	public UISlider   brushSizeSlider;

    public UITexture colorTexture;
    public UITexture alphaTexture;
    public UITexture dualBrushTexture;
    public UITexture tempLayerTexture;

    private float _time = 0.0f;

    public const string kBrushNameSandArt = "sand_art";

	public void Start() {
        canvas.supportsMultiTouch = true;
        CustomInput.receivesMouseInput = true;
        if (canvas.uiTexture.material != null && canvas.uiTexture.material.HasProperty("_BumpMap"))
        {
            Vector2 textureScale = canvas.uiTexture.material.GetTextureScale("_BumpMap");
            textureScale.x = (float)Screen.width / (float)Screen.height;
            textureScale.y = 1.0f;
            canvas.uiTexture.material.SetTextureScale("_BumpMap", textureScale);
        }

        if (background != null)
        {
            Rect uvRect = background.uvRect;
		
            uvRect.width = (float)Screen.width / (float)Screen.height;
            uvRect.height = 1;
            background.uvRect = uvRect;
        }

        Marker();
		BrushSettings();

		colorField.onColorSelected = (color)=>{
			brush.color = color;
		};

        GPGPUModeToggle.value = Canvas_.prefersComputeShaders;
        GPGPUModeLabel.text = Canvas_.supportsComputeShaders ? "GPGPU Mode" : "Software Mode";
        GPGPUModeToggle.onChange.Add(new EventDelegate(() =>
        {
            Canvas_.prefersComputeShaders = GPGPUModeToggle.value;
            GPGPUModeLabel.text = Canvas_.supportsComputeShaders ? "GPGPU Mode" : "Software Mode";
        }));
	}

    
    public void Update()
    {
        _time += Time.deltaTime;
        if (_time >= 0.25f)
        {
            _time = 0.0f;
            if(colorTexture != null)
                colorTexture.mainTexture = canvas.texture;
            
            // Too slow!
            //if(alphaTexture != null)
            //    alphaTexture.mainTexture = canvas.alphaBufferTexture;
            //if(dualBrushTexture != null)
            //    dualBrushTexture.mainTexture = canvas.dualBrushBufferTexture;
            //if(tempLayerTexture != null)
            //    tempLayerTexture.mainTexture = canvas.temporaryLayerTexture;
        }
    }

	public void SetBlendMode() {
		string selection = blendModePopup.value;

		if(brush != null) {
			switch(selection) {
			case "AlphaBlend":
				brush.blendMode = BlendMode.AlphaBlend;
				break;
			case "Multiply":
				brush.blendMode = BlendMode.Multiply;
				break;
			case "ColorBurn":
				brush.blendMode = BlendMode.ColorBurn;
				break;
			case "Screen":
				brush.blendMode = BlendMode.Screen;
				break;
			case "Overlay":
				brush.blendMode = BlendMode.Overlay;
				break;
			}
			canvas.brush = brush;
		}
	}

    public void SetTextureSize()
    {
        string selection = resolutionPopup.value;
        Vector2 textureSize = canvas.textureSize;

        switch (selection)
        {
            case "256":
                textureSize = new Vector2(256, 256);
                break;
            case "512":
                textureSize = new Vector2(512, 512);
                break;
            case "1024":
                textureSize = new Vector2(1024, 1024);
                break;
            case "2048":
                textureSize = new Vector2(2048, 2048);
                break;
            case "4096":
                textureSize = new Vector2(4096, 4096);
                break;
        }

        canvas.textureSize = textureSize;
    }

	public void ChangeBrushSize() {

		brush.diameter = Mathf.Max(4.0f, brushSizeSlider.value * 100.0f);


		if(brush.dualComponent.enable) {
			brush.dualComponent.scatter = brush.diameter;
			brush.dualComponent.count = (int)brush.diameter;
		}
		canvas.brush = brush;
	}

	public void BrushSettings() {
		brush.color = colorField.currentColor;
		ChangeBrushSize();
	}

	public void Marker() {
		brush = canvas.brushSet.Get(BrushSet.kBrushNameMarker);
		canvas.brush = brush;
		BrushSettings();
	}
	
	public void Eraser() {
        brush = canvas.brushSet.Get(BrushSet.kBrushNameEraser);
		canvas.brush = brush;
		BrushSettings();
	}
	
	public void Crayon() {
		brush = canvas.brushSet.Get(BrushSet.kBrushNameCrayon);
		canvas.brush = brush;
		BrushSettings();
	}
	
	public void Pastel() {
        brush = canvas.brushSet.Get(BrushSet.kBrushNamePastel);
        brush.maskName = "pastel_icon";
        brush.blendMode = BlendMode.AlphaBlend;
		canvas.brush = brush;
		BrushSettings();
	}
	
	public void Rainbow() {
		brush = canvas.brushSet.Get(BrushSet.kBrushNameRainbow);
		canvas.brush = brush;
		BrushSettings();
	}
	
	public void Spray() {
		brush = canvas.brushSet.Get(BrushSet.kBrushNameAirbrush);
		canvas.brush = brush;
		BrushSettings();
	}

    public void SandArt()
    {
        brush = canvas.brushSet.Get(kBrushNameSandArt);
        if (brush == null)
        {
            brush = new Brush();
            brush.brushName = kBrushNameSandArt;
            brush.maskType = MaskType.PixelMask;
            brush.maskName = "sand64";
            brush.useMaskColor = true;
            brush.diameter = 30.0f;
            brush.color = Color.white;
            brush.spacing = 0.25f;
            brush.opacity = 1.0f;
            brush.flow = 0.8f;
            brush.airbrush = false;
            brush.paintStartPosition = false;
            brush.useAlphaBuffer = false;

            brush.shapeDynamicComponent.enable = true;
            brush.shapeDynamicComponent.angleControl = AngleControl.Direction;

            canvas.brushSet.Register(brush);
        }
        canvas.brush = brush;
        BrushSettings();
    }
}
