using UnityEngine;
using System;
using System.Collections.Generic;

public class BrushSet {
	// Const variables
	public const string kBrushNameMarker = "Marker";
	public const string kBrushNameAirbrush = "AirBrush";
	public const string kBrushNameCrayon = "Crayon";
	public const string kBrushNameRainbow = "Rainbow";
	public const string kBrushNamePastel = "Pastel";
	public const string kBrushNameEraser = "Eraser";

	// Private variables
	private Dictionary<string, Brush> _brushDict = new Dictionary<string, Brush>();
	private Dictionary<string, Brush> _modifiedBrushDict = new Dictionary<string, Brush>();

	// Constructor
	public BrushSet() {
		_GenerateDefaultBrushes();
	}
	
	public void Register(Brush brush) {
		if(brush == null) {
			Debug.LogError("BrushSet.Register() : brush is null.");
		}
		else if(brush.brushName == null) {
			Debug.LogError("BrushSet.Register() : brush name is null.");
		}
		else {
			_brushDict[brush.brushName] = brush;
			_brushDict[brush.brushName] = (Brush)brush.Clone();
		}
	}

	public Brush Get(string brushName) {
		Brush brush = null;
		if(!_modifiedBrushDict.ContainsKey(brushName)) {
			Brush originalBrush = null;
			_brushDict.TryGetValue(brushName, out originalBrush);
            if (originalBrush != null)
            {
                _modifiedBrushDict[brushName] = (Brush)originalBrush.Clone();
            }
		}
		_modifiedBrushDict.TryGetValue(brushName, out brush);
		return brush;
	}

	public void Revert(string brushName) {
		Brush brush = null;
		_brushDict.TryGetValue(brushName, out brush);
		if(brush != null) {
			_modifiedBrushDict[brushName] = (Brush)brush.Clone();
		}
	}

	private void _GenerateDefaultBrushes() {
		Brush brush = null;

		brush = _GetDefaultMarkerBrush();
		Register(brush);
		brush = _GetDefaultAirbrush();
		Register(brush);
		brush = _GetDefaultCrayonBrush();
		Register(brush);
		brush = _GetDefaultRainbowBrush();
		Register(brush);
		brush = _GetDefaultPastelBrush();
		Register(brush);
		brush = _GetDefaultEraserBrush();
		Register(brush);
	}

	private Brush _GetDefaultMarkerBrush() {
		// Marker
		Brush brush = new Brush ();
		brush.brushName = kBrushNameMarker;
		brush.maskType = MaskType.VectorMask;
		brush.diameter = 12.0f;
		brush.color = Color.black;
		brush.spacing = 0.15f;
		brush.hardness = 0.7f;
		brush.opacity = .78f;
		brush.flow = 0.5f;
		brush.airbrush = false;
		brush.paintStartPosition = true; 
		brush.useAlphaBuffer = true;
		return brush;
	}

	private Brush _GetDefaultAirbrush() {
		// AirBrush
		Brush brush = new Brush ();
		brush.brushName = kBrushNameAirbrush;
		brush.maskType = MaskType.VectorMask;
		brush.diameter = 30.0f;
		brush.color = Color.black;
		brush.spacing = 0.1f;
		brush.hardness = 0.0f;
		brush.opacity = .2f;
		brush.flow = 0.5f;
		brush.airbrush = true;
		brush.paintStartPosition = true; 
		brush.useAlphaBuffer = false;
		return brush;
	}

	private Brush _GetDefaultCrayonBrush() {
		// Crayon
		Brush brush = new Brush ();
		brush.brushName = kBrushNameCrayon;
		brush.maskType = MaskType.VectorMask;
		brush.diameter = 30.0f;
		brush.color = new Color (255.0f / 255.0f, 42.0f / 255.0f, 42.0f / 255.0f);
		brush.spacing = 0.15f;
		brush.hardness = 1.0f;
		brush.opacity = 1.0f;
		brush.flow = 1.0f;
		brush.airbrush = false;
		brush.paintStartPosition = true; 
		brush.useAlphaBuffer = false;

		// crayon requires additional components
		brush.shapeDynamicComponent.enable = true;
		brush.shapeDynamicComponent.sizeJitter = 1.0f;
		brush.shapeDynamicComponent.minimumDiameter = 0.0f;
		brush.shapeDynamicComponent.angleJitter = 0.45f;
		brush.shapeDynamicComponent.angleControl = AngleControl.Off;
		brush.shapeDynamicComponent.roundnessJitter = 0.1f;
		brush.shapeDynamicComponent.minimumRoundness = 0.25f;
		
		brush.transferComponent.enable = true;
		brush.transferComponent.opacityJitter = 0.3f;
		brush.transferComponent.flowJitter = 0.0f;
		
		brush.dualComponent.enable = true;
		brush.dualComponent.diameter = 3.0f;
		brush.dualComponent.spacing = 0.5f;
		brush.dualComponent.scatter = 1.45f;
		brush.dualComponent.count = 2;
        brush.dualComponent.blendMode = BlendMode.MultiplyRaw;
		brush.dualComponent.hardness = 0.0f;

		return brush;
	}

	private Brush _GetDefaultRainbowBrush() {
		Brush brush = new Brush ();
		brush.brushName = kBrushNameRainbow;
		brush.maskType = MaskType.PixelMask;
		brush.maskName = "rainbow_brush30";
		brush.useMaskColor = true;
		brush.diameter = 30.0f;
		brush.color = Color.black;
		brush.spacing = 0.15f;
		brush.opacity = 0.6f;
		brush.flow = 1.0f;
		brush.airbrush = false;
		brush.paintStartPosition = false; 
		brush.useAlphaBuffer = false;
		
		brush.shapeDynamicComponent.enable = true;
		brush.shapeDynamicComponent.angleControl = AngleControl.Direction;

		return brush;
	}

	private Brush _GetDefaultPastelBrush() {
		Brush brush = new Brush ();
		brush.brushName = kBrushNamePastel;
		brush.maskType = MaskType.PixelMask;
		//brush.blendMode = BlendMode.Multiply;
		brush.maskName = "pastel_icon";
		brush.diameter = 30.0f;
		brush.color = Color.black;
		brush.spacing = 0.06f;
		brush.hardness = 0.0f;
		brush.opacity = .5f;
		brush.flow = 0.5f;
		brush.angle = 24f;
		brush.airbrush = true;
		brush.paintStartPosition = true; 
		brush.useAlphaBuffer = false;

		brush.shapeDynamicComponent.enable = true;
		brush.shapeDynamicComponent.sizeJitter = 14.0f;
		brush.shapeDynamicComponent.minimumDiameter = 0.0f;
		brush.shapeDynamicComponent.angleJitter = 8.1f;
		brush.shapeDynamicComponent.angleControl = AngleControl.Off;
		brush.shapeDynamicComponent.roundnessJitter = 0.4f;
		brush.shapeDynamicComponent.minimumRoundness = 0.25f;

		return brush;
	}

	private Brush _GetDefaultEraserBrush() {

        Brush brush = new Brush();
        brush.brushName = kBrushNameEraser;
        brush.maskType = MaskType.EraserMask;
        brush.blendMode = BlendMode.MultiplyRaw;
        brush.diameter = 12.0f;
        brush.color = Color.white;
        brush.spacing = 0.15f;
        brush.hardness = 0.7f;
        brush.opacity = .78f;
        brush.flow = 0.5f;
        brush.airbrush = false;
        brush.paintStartPosition = true;
        brush.useAlphaBuffer = true;
        return brush;
	}
}