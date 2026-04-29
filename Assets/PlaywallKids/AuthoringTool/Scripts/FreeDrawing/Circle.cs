using UnityEngine;
using System.Collections;

public struct Circle {
	public Vector3 center;
	public float radius;
	
	public Circle(Vector3 c, float r) {
		center = c;
		radius = r;
	}
	
	public static bool operator==(Circle a, Circle b) {
		return (a.center-b.center).magnitude < 0.00001f && Mathf.Abs(a.radius-b.radius) < 0.00001f;
	}
	
	public static bool operator!=(Circle a, Circle b) {
		return !(a == b);
	}
	
    public override bool Equals(System.Object obj)
    {
		Circle c = (Circle)obj;		
		return this == c;
    }

    public bool Equals(Circle p)
    {
        return this == p;
    }

    public override int GetHashCode()
    {
        return center.GetHashCode() * (int)radius;
    }
	
	public bool IsIn(Vector3 p) {
		return (center - p).magnitude <= radius;
	}
}
