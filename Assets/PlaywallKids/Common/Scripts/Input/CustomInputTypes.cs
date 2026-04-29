using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Touch info.
/// </summary>
public struct TouchInfo
{
	/// <summary>
	/// identifier of the touch info.
	/// </summary>
	public int id;

	/// <summary>
	/// user ID of this touch info
	/// </summary>
	public int userId;
	
	public enum Type {
		Custom,
		Mouse,
		Touch
	}

	/// <summary>
	/// type will be initialized by CustomInput. You don't need to modify.
	/// </summary>
	public Type type;
	
	public int channel;

	/// <summary>
	/// position
	/// </summary>
	public Vector2 position;

	/// <summary>
	/// position X
	/// </summary>
	public int axisX {
		get {
			return (int)position.x;
		}
	}

	/// <summary>
	/// position Y
	/// </summary>
	public int axisY {
		get {
			return (int)position.y;
		}
	}
	
	public enum Phase {
		Begin,
		Move,
		Stay,
		End,
		Cancel
	}

	/// <summary>
	/// Touch phase.
	/// </summary>
	public Phase phase;

	/// <summary>
	/// Tap Count.
	/// </summary>
	public int tapCount;

	public int diameter;
}

/// <summary>
/// Motion info.
/// </summary>
public struct MotionInfo
{
	public bool  isActive;
	public int	 id;
	public float axisX;
	public float axisY;
	public int   motionType;
}