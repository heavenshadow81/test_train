using UnityEngine;
using System.Collections.Generic;

public enum ComponentType
{
	Unknown      = -1,
	ShapeDynamic = 0,
	Scattering,
	Texture,
	Dual,
	ColorDynamic,
	Transfer,
};

public class BrushComponent {
	protected string        mComponentName;
	protected ComponentType mComponentType;
	protected bool          mEnable;
	
	public BrushComponent () {
		componentName = "BrushComponent";
		componentType = ComponentType.Unknown;
		enable = false;
	}
	
	public string componentName {
		get {
			return mComponentName;
		}
		set {
			mComponentName = value;
		}
	}
	
	public ComponentType componentType {
		get { 
			return mComponentType;
		}
		set {
			mComponentType = value;
		}
	}
	
	public bool enable {
		get { 
			return mEnable;
		}
		set {
			mEnable = value;
		}
	}

    public object Clone()
    {
        return this.MemberwiseClone();
    }
};