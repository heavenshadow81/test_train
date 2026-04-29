using UnityEngine;
using System.Collections;

public class BrushScatteringComponent : BrushComponent
{
	protected float mScatter;
	protected bool  mBothAxes;
	protected int   mCount;
	protected float mCountJitter;
	
	public BrushScatteringComponent ()
	{
		componentName = "Scattering";
		componentType = ComponentType.Scattering;
		scatter = 0.0f;
		bothAxes = false;
		count = 1;
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
	
	// (0, 1)
	public float countJitter {
		get {
			return mCountJitter;
		}
		set {
			mCountJitter = value;
		}
	}
	
	public int UpdateCount ()
	{
		return (int)Mathf.Min (Random.value * count * (1.0f - countJitter), count - 1);
	}
	
	public Vector3 UpdatePosition (Vector3 current, Vector3 up, Vector3 right, Vector3 size)
	{
		Vector3 newPosition = current + (up * (Random.value - 0.5f) * size.y * scatter);
		if (bothAxes) {
			newPosition = newPosition + (right * (Random.value - 0.5f) * size.y * scatter);
		}
		return newPosition;
	}
};