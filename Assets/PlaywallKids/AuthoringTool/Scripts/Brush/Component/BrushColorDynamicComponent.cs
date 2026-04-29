using UnityEngine;
using System.Collections;

public class BrushColorDynamicComponent : BrushComponent
{
	protected float mHueJitter;
	protected float mSaturationJitter;
	protected float mBrightnessJitter;
	
	public BrushColorDynamicComponent ()
	{
		componentName = "Color";
		componentType = ComponentType.ColorDynamic;
		hueJitter = 0.0f;
		saturationJitter = 0.0f;
		brightnessJitter = 0.0f;
	}
	
	// (0, 1)
	public float hueJitter {
		get {
			return mHueJitter;
		}
		set {
			mHueJitter = Mathf.Clamp01 (value);
		}
	}
	
	// (0, 1)
	public float saturationJitter {
		get {
			return mSaturationJitter;
		}
		set {
			mSaturationJitter = Mathf.Clamp01 (value);
		}
	}
	
	// (0, 1)
	public float brightnessJitter {
		get {
			return mBrightnessJitter;
		}
		set {
			mBrightnessJitter = Mathf.Clamp01 (value);
		}
	}
	
	public Color UpdateColor (Color color)
	{
		HSV hsv = BrushUtils.RGBToHSV (color);

		// Hue
		hsv.h += (Random.value * 2.0f - 1.0f) * hueJitter;
		hsv.h = Mathf.Clamp(hsv.h, 0.0f, 360.0f);

		// Saturation
		if (!float.IsNaN (hsv.h)) {
			hsv.s += (Random.value * 2.0f - 1.0f) * saturationJitter;
			hsv.s = Mathf.Clamp01 (hsv.s);
		}
		
		// Brightness
		hsv.v += (Random.value * 2.0f - 1.0f) * brightnessJitter;
		hsv.v = Mathf.Clamp01 (hsv.v);
		
		return BrushUtils.HSVToRGB (hsv);
	}
};