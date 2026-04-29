using UnityEngine;
using System.Collections;

public class BrushTextureComponent : BrushComponent
{
	protected string    mTextureName;
	protected float     mScale;
	protected float     mBrightness;
	protected float     mContrast;
	protected BlendMode mBlendMode;
	protected float     mDepth;
	
	public BrushTextureComponent ()
	{
		componentName = "Texture";
		componentType = ComponentType.Texture;
		textureName = "";
		scale = 1.0f;
		brightness = 0.0f;
		contrast = 0.0f;
		blendMode = BlendMode.Multiply;
		depth = 1.0f;
	}
	
	public string textureName {
		get {
			return mTextureName;
		}
		set {
			mTextureName = value;
		}
	}
	
	// (0.01, 10)
	public float scale {
		get {
			return mScale;
		}
		set {
			mScale = Mathf.Clamp (value, 0.01f, 10.0f);
		}
	}
	
	// (-150, 150)
	public float brightness {
		get {
			return mBrightness;
		}
		set {
			mBrightness = Mathf.Clamp (value, -150.0f, 150.0f);
		}
	}
	
	// (-50, 100)
	public float contrast {
		get {
			return mContrast;
		}
		set {
			mContrast = Mathf.Clamp (value, -50.0f, 100.0f);
		}
	}
	
	public BlendMode blendMode {
		get {
			return mBlendMode;
		}
		set {
			mBlendMode = value;
		}
	}
	
	// (0, 1)
	public float depth {
		get {
			return mDepth;
		}
		set {
			mDepth = Mathf.Clamp01 (value);
		}
	}
};