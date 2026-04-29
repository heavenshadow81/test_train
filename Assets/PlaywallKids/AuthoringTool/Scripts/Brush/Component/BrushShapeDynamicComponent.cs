using UnityEngine;
using System.Collections;

public class BrushShapeDynamicComponent : BrushComponent {
	protected float        mSizeJitter;
	protected float        mMinimumDiameter;
	protected float        mAngleJitter;
	protected AngleControl mAngleControl;
	protected float        mRoundnessJitter;
	protected float        mMinimumRoundness;
	
	public BrushShapeDynamicComponent ()
	{
		componentName = "ShapeDynamic";
		componentType = ComponentType.ShapeDynamic;
		sizeJitter = 0.0f;
		minimumDiameter = 0.0f;
		angleJitter = 0.0f;
		angleControl = AngleControl.Off;
		roundnessJitter = 0.0f;
		minimumRoundness = 0.01f;
	}
	
	// (0, 1)
	public float sizeJitter {
		get {
			return mSizeJitter;
		}
		set {
			mSizeJitter = Mathf.Clamp01 (value);
		}
	}
	
	// (0, 1)
	public float minimumDiameter {
		get {
			return mMinimumDiameter;
		}
		set {
			mMinimumDiameter = Mathf.Clamp01 (value);
		}
	}
	
	// (0, 1)
	public float angleJitter {
		get {
			return mAngleJitter;
		}
		set {
			mAngleJitter = Mathf.Clamp01 (value);
		}
	}
	
	public AngleControl angleControl {
		get {
			return mAngleControl;
		}
		set {
			mAngleControl = value;
		}
	}
	
	// (0, 1)
	public float roundnessJitter {
		get {
			return mRoundnessJitter;
		}
		set {
			mRoundnessJitter = Mathf.Clamp01 (value);
		}
	}
	
	// (0.01, 1)
	public float minimumRoundness {
		get {
			return mMinimumRoundness;
		}
		set {
			mMinimumRoundness = Mathf.Clamp (value, 0.01f, 1.0f);
		}
	}
	
	public Vector3 UpdateSize (Vector3 size)
	{
		size.y = Mathf.Max (size.y * (1.0f - Random.value * sizeJitter), size.y * minimumDiameter);
		size.x = size.y * (1.0f - Random.value * roundnessJitter);
		return size;
	}
	
	public float UpdateAngle (float angle, Vector3 delta)
	{
		if (angleControl == AngleControl.Off) {
			return angle + (Random.value - 0.5f) * 360.0f * angleJitter;
		} else if (angleControl == AngleControl.Direction) {
			angle = Vector3.Angle (delta, Vector3.up);
			if (Vector3.Dot (delta, Vector3.right) < 0.0f) {
				angle *= -1.0f;
			}
			return angle;
        }
        else if (angleControl == AngleControl.Angle)
        {
            return angle;
        }else
        {
			return 0.0f;
		}
	}
};