using UnityEngine;
using System.Collections;

public class BrushDualComponent : BrushComponent
{
	
	protected MaskType  mMaskType;
	protected string    mMaskName;
	protected float     mDiameter;
	protected float     mHalfDiameter;
	protected float     mSpacing;
	protected float     mScatter;
	protected bool      mBothAxes;
	protected int       mCount;
	protected BlendMode mBlendMode;
	protected float     mHardness;
	private   Vector3   mPreviousPosition;
	private   Vector3   mStartPosition;
	private   Vector3   mEndPosition;
	
	
	public BrushDualComponent ()
	{
		componentName = "Dual";
		componentType = ComponentType.Dual;
		maskType = MaskType.VectorMask;
		maskName = "";
		diameter = 1.0f;
		spacing = 0.25f;
		scatter = 0.0f;
		count = 1;
		blendMode = BlendMode.Multiply;
		
		mHardness = 1.0f;
	}
	
	public MaskType maskType {
		get {
			return mMaskType;
		}
		set {
			mMaskType = value;
		}
	}
	
	public string maskName {
		get {
			return mMaskName;
		}
		set {
			mMaskName = value;
		}
	}
	
	// (1, 5000)
	public float diameter {
		get {
			return mDiameter;
		}
		set {
			mDiameter = Mathf.Clamp (value, 1.0f, 5000.0f);
			mHalfDiameter = mDiameter * 0.5f;
		}
	}
	
	// (0.01, 10)
	public float spacing {
		get {
			return mSpacing;
		}
		set {
			mSpacing = Mathf.Clamp (value, 0.01f, 10.0f);
		}
	}
	
	// (0, 10)
	public float scatter {
		get {
			return mScatter;
		}
		set {
			mScatter = Mathf.Clamp (value, 0.0f, 10.0f);
		}
	}
	
	public bool bothAxes {
		get {
			return mBothAxes;
		}
		set {
			mBothAxes = value;
		}
	}
	
	// (1, 16)
	public int count {
		get {
			return mCount;
		}
		set {
			mCount = (int)Mathf.Clamp (value, 1.0f, 16.0f);
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
	
	//**
	public float hardness {
		get {
			return mHardness;
		}
		set {
			mHardness = value;
		}
	}
	
	/*
    public int UpdateCount() {
        return (int)Mathf.Min(Random.value * count, count - 1);
    }
*/
	public Vector3 UpdatePosition (Vector3 current, Vector3 up, Vector3 right, Vector3 size)
	{
		Vector3 newPosition = current + (up * (Random.value - 0.5f) * size.y * scatter);
		if (bothAxes) {
			newPosition = newPosition + (right * (Random.value - 0.5f) * size.y * scatter);
		}
		return newPosition;
	}
	
	public void StartPaint (Vector3 previous, Vector3 start, Vector3 end)
	{
		mPreviousPosition = previous;
		mStartPosition = start;
		mEndPosition = end;
	}
	
	public void Paint (Canvas_ canvas, Vector3 end, float duration)
	{
		if(Mathf.Abs(duration) < 4.0f) return;

		mEndPosition = end;
		
		Vector3 delta = mEndPosition - mStartPosition;
		float dist = diameter * spacing;
		
		if (delta.magnitude > dist) {
			Vector3 prev = mPreviousPosition;
			Vector3 curr = BrushUtils.CatmullRom (mPreviousPosition, mStartPosition, mEndPosition, mEndPosition, dist, delta.magnitude);
			for (float i = dist; i < delta.magnitude; i += dist) {
				Vector3 next = BrushUtils.CatmullRom (mPreviousPosition, mStartPosition, mEndPosition, mEndPosition, i + dist, duration);
				Paint (canvas, prev, curr, next);
				
				prev = curr;
				curr = next;
			}
			
			mPreviousPosition = mStartPosition;
			mStartPosition = mEndPosition;
		}
	}
	
	public void Paint (Canvas_ canvas, Vector3 previous, Vector3 current, Vector3 next)
	{
		Vector3 delta = next - previous;
		Vector3 up = Vector3.Cross (delta, -Vector3.forward).normalized;
		//int count     = UpdateCount();
		
		for (int i = 0; i < count; ++i) {
			Vector3 newPosition = current;
			Vector3 newSize = new Vector3 (diameter, diameter);
			Vector3 halfSize = newSize * 0.5f;
			
			// Scattering
			newPosition = UpdatePosition (current, up, delta.normalized, newSize);
			
			// Make Mask Transform
			Matrix4x4 textureToCanvasM = canvas.GetTextureToCanvasMatrix ();// Texture to canvas
			Matrix4x4 zeroBaseDiffM = Matrix4x4.TRS (-newPosition + halfSize, Quaternion.identity, Vector3.one);// Canvas to zero base
			Matrix4x4 maskTransform = zeroBaseDiffM * textureToCanvasM;
			
			// Draw
			Vector3 stampPosition = canvas.CanvasToTexture (newPosition);
			Vector3 stampSize = canvas.CanvasToTexture (newSize);
            Bounds stampBounds = new Bounds(stampPosition, stampSize);
            stampBounds.SetMinMax(// Clamp out ot texture
                                   BrushUtils.Max(stampBounds.min, Vector3.zero),
                                   BrushUtils.Min(stampBounds.max, canvas.textureSize)
                                   );
			
			Vector3 maskCenter = new Vector3 ((newSize.x - 1.0f) * 0.5f, (newSize.y - 1.0f) * 0.5f);// (0, size - 1)
			
			for (int y = (int)stampBounds.min.y; y < stampBounds.max.y; ++y) {
				for (int x = (int)stampBounds.min.x; x < stampBounds.max.x; ++x) {
					
					// Mask Position
					Vector3 maskPosition = maskTransform.MultiplyPoint3x4 (new Vector3 (x, y));
					
					// Alpha 
					float distance = (maskPosition - maskCenter).magnitude;
					float halfInnerSize = mHalfDiameter * hardness;
					float weight = 0.0f;
					
					if (distance > mHalfDiameter) {
						weight = 0.0f;
					} else if (distance > halfInnerSize) {
						weight = Mathf.Min (1.0f, (distance - halfInnerSize) / (mHalfDiameter - halfInnerSize));
						if (weight <= 0.3f) {
							weight = 1.0f - 0.1f / 0.3f * weight;
						} else if (weight <= 0.7f) {
							weight = 1.0f - (0.8f / 0.4f * weight - 0.5f);
						} else {
							weight = 1.0f - (0.1f / 0.3f * weight + (1.0f - 0.1f / 0.3f));
						}
					} else {
						weight = 1.0f;
					}
					
					// Blend
					canvas.SetDualBrushBuffer (
						x, y, 
						Mathf.Max (weight, canvas.GetDualBrushBuffer (x, y))
						);
				}
			}
			
			canvas.EncapsulateDualBrushBufferBounds (stampBounds);
		}
	}
};