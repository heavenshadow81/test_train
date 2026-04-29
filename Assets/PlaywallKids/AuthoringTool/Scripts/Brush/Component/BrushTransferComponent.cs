using UnityEngine;
using System.Collections;

public class BrushTransferComponent : BrushComponent
{
	protected float mOpacityJitter;
	protected float mFlowJitter;
	
	public BrushTransferComponent ()
	{
		componentName = "Transfer";
		componentType = ComponentType.Transfer;
		opacityJitter = 0.0f;
		flowJitter = 0.0f;
	}
	
	// (0, 1)
	public float opacityJitter {
		get {
			return mOpacityJitter;
		}
		set {
			mOpacityJitter = Mathf.Clamp01 (value);
		}
	}
	
	// (0, 1)
	public float flowJitter {
		get {
			return mFlowJitter;
		}
		set {
			mFlowJitter = Mathf.Clamp01 (value);
		}
	}
	
	public float UpdateOpacity (float opacity)
	{
		return opacity * (1.0f - Random.value * opacityJitter);
	}
	
	public float UpdateFlow (float flow)
	{
		return flow * (1.0f - Random.value * flowJitter);
	}
}