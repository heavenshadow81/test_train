using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triangle {
	public Vector3[] vertices = new Vector3[3];

	public bool isCounterClockwise {
		get {
			Vector3 v1 = vertices[0];
			Vector3 v2 = vertices[1];
			Vector3 v3 = vertices[2];
			return IsCounterClockwise(v1, v2, v3);
		}
	}
	
	public bool isCounterClockwise3D {
		get {
			Vector3 v1 = vertices[0];
			Vector3 v2 = vertices[1];
			Vector3 v3 = vertices[2];
			return IsCounterClockwise3D(v1, v2, v3);
		}
	}

	public virtual Triangle oppositeTriangle {
		get {
			Vector3 v1 = vertices[0];
			Vector3 v2 = vertices[1];
			Vector3 v3 = vertices[2];
			return new Triangle(v1, v3, v2);
		}
	}

	public static bool IsCounterClockwise(Vector3 v1, Vector3 v2, Vector3 v3) {
		return v1.x * (v2.y - v3.y) + v2.x * (v3.y - v1.y) + v3.x * (v1.y - v2.y) > 0;
	}
	
	public static bool IsCounterClockwise3D(Vector3 v1, Vector3 v2, Vector3 v3) {
		// unity uses left-handed winding...
		return v1.x * (v2.y * v3.z - v2.z * v3.y) - v2.x * (v1.y * v3.z - v1.z * v3.y) + v3.x * (v1.y * v2.z - v1.z * v2.y) < 0;
	}

	public static bool IsIn(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v) {
		bool counterClockwise = false;
		bool f1 = false, f2 = false, f3 = false;

		// attempts 2D
		if(v1.z == 0.0f && v2.z == 0.0f && v3.z == 0.0f) {
			counterClockwise = Triangle.IsCounterClockwise(v1, v2, v3);
			f1 = Triangle.IsCounterClockwise(v1, v2, v);
			f2 = Triangle.IsCounterClockwise(v2, v3, v);
			f3 = Triangle.IsCounterClockwise(v3, v1, v);
		}
		// 3D
		else {
			counterClockwise = Triangle.IsCounterClockwise3D(v1, v2, v3);
			f1 = Triangle.IsCounterClockwise3D(v1, v2, v);
			f2 = Triangle.IsCounterClockwise3D(v2, v3, v);
			f3 = Triangle.IsCounterClockwise3D(v3, v1, v);
		}

		return (counterClockwise == f1) && (counterClockwise == f2) && (counterClockwise == f3);
	}
	
	public Triangle(Vector3 a, Vector3 b, Vector3 c) {
		vertices[0] = a;
		vertices[1] = b;
		vertices[2] = c;
	}
	
	public static bool operator==(Triangle a, Triangle b) {
		bool flag = false;

		if((System.Object)a != null && (System.Object)b != null) {
			flag = true;
			for(int i = 0; i < 3; i++) {
				if(a.IndexOf(b.GetVertex(i)) < 0) {
					flag = false;
					break;
				}
			}
		}
		else if((System.Object)a == null && (System.Object)b == null) {
			flag = true;
		}
		
		return flag;
	}
	
	public static bool operator!=(Triangle a, Triangle b) {
		return !(a == b);
	}
	
	public Circle GetCircumcircle() {
		Vector3 u = vertices[1]-vertices[0];
		Vector3 v = vertices[2]-vertices[0];
		Vector3 x = new Vector3(u.y, -u.x, 0);
		//Vector3 y = new Vector3(v.y, -v.x, 0);
		
		float s = 0.5f * Vector3.Dot(v - u, v) / Vector3.Dot (x, v);
		//float t = 0.5f * Vector3.Dot(v - u, u) / Vector3.Dot(y, u);
		
		Vector3 center = u * 0.5f + s * x + vertices[0];
		float radius = (vertices[0]-center).magnitude;
		
		Circle circle = new Circle(center, radius);
		
		return circle;
	}
	
    public override bool Equals(System.Object obj)
    {
		Triangle c = (Triangle)obj;		
		return this == c;
    }

    public bool Equals(Triangle p)
    {
        return this == p;
    }

    public override int GetHashCode()
    {
		int hashCode = 0;
		for(int i = 0, cnt = vertices.Length; i < cnt; i++) {
			hashCode += vertices[i].GetHashCode() + i;
		}
		return hashCode;
    }
	
	public bool IsIn(Vector3 v) {
		return IsIn(vertices[0], vertices[1], vertices[2], v);
	}

	public bool Contains(Vector3 v) {
		return (IndexOf(v) > -1);
	}

	public int IndexOf(Vector3 v) {
		for(int i = 0; i < 3; i++) {
			Vector3 vertex = vertices[i];
			if((vertex - v).sqrMagnitude <= 0.00001f) {
				return i;
			}
		}
		return -1;
	}

	public Vector3 GetVertex(int index) {
		return vertices[index%3];
	}
	
	public void Draw(Color lineColor) {
		GL.Begin(GL.LINES);
		GL.Color(lineColor);
		GL.Vertex(vertices[0]);
		GL.Vertex(vertices[1]);
		GL.Vertex(vertices[1]);
		GL.Vertex(vertices[2]);
		GL.Vertex(vertices[2]);
		GL.Vertex(vertices[0]);
		GL.End();
	}
	
	public Vector3 GetNormal() {
		Vector3 u = vertices[1] - vertices[0];
		Vector3 v = vertices[2] - vertices[0];
		return Vector3.Cross(u, v).normalized;
	}
}
