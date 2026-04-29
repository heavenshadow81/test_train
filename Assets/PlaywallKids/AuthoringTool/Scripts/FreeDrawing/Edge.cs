using UnityEngine;
using System.Collections;

public struct Edge {
	private Vector3 _aPos, _bPos;
	public Vector3 aPos {
		get {
			return _aPos;
		}
		set {
			_aPos = value;
		}
	}
	
	public Vector3 bPos {
		get {
			return _bPos;
		}
		set {
			_bPos = value;
		}
	}
	
	private int _aIndex, _bIndex;
	public int aIndex {
		get {
			return _aIndex;
		}
	}
	
	public int bIndex {
		get {
			return _bIndex;
		}
	}
	
	private bool _isExternal;
	public bool isExternal {
		get {
			return _isExternal;
		}
		set {
			_isExternal = value;
		}
	}
	
	public float length {
		get {
			return (_bPos-_aPos).magnitude;
		}
	}
	
	public float sqrLength {
		get {
			return (_bPos-_aPos).sqrMagnitude;
		}
	}
	
	public static bool GetIntersection(Edge e1, Edge e2, out Vector3 intersectionPoint) {
		float outS = 0.0f, outT = 0.0f;
		return e1.GetIntersection(e2, out outS, out outT, out intersectionPoint);
	}
	
	public Edge(Vector3 a, Vector3 b, int aIdx, int bIdx) {
		_aIndex = aIdx;
		_bIndex = bIdx;
		_aPos = a;
		_bPos = b;
		_isExternal = false;
	}
	
	public Edge(Vector3 a, Vector3 b, int aIdx, int bIdx, bool external) {
		_aIndex = aIdx;
		_bIndex = bIdx;
		_aPos = a;
		_bPos = b;
		_isExternal = external;
	}
	
	public bool GetIntersection(Edge another) {
		float outS = 0.0f, outT = 0.0f;
		Vector3 intersectionPoint = Vector3.zero;
		
		return GetIntersection(another, out outS, out outT, out intersectionPoint);
	}
	
	public bool GetIntersection(Edge another, out float outS) {
		float outT = 0.0f;
		Vector3 intersectionPoint = Vector3.zero;
		
		return GetIntersection(another, out outS, out outT, out intersectionPoint);
	}
	
	public bool GetIntersection(Edge another, out float outS, out float outT, out Vector3 intersectionPoint) {
		Vector3 v1 = aPos, v2 = bPos, v3 = another.aPos, v4 = another.bPos;
		intersectionPoint = Vector3.zero;
		outS = -1.0f; outT = -1.0f;
		
		float under = (v4.y-v3.y) * (v2.x-v1.x) - (v4.x-v3.x) * (v2.y-v1.y);
		if(under == 0.0f) return false;
		
		float s = (v4.x-v3.x) * (v1.y-v3.y) - (v4.y-v3.y) * (v1.x - v3.x);
		float t = (v2.x-v1.x) * (v1.y-v3.y) - (v2.y-v1.y) * (v1.x - v3.x);
		
		s /= under;
		t /= under;
		
		outS = s;
		outT = t;
		
		if(s < 0.0f || s > 1.0f || t < 0.0f || t > 1.0f) return false;
		
		intersectionPoint = (1.0f - s) * v1 + s * v2;
		return true;
	}
	
	public Edge GetReversed() {
		Edge edge = new Edge(bPos, aPos, bIndex, aIndex, !isExternal);
		return edge;
	}
	
	public static bool operator==(Edge a, Edge b) {
		return (a.aIndex == b.aIndex) && (a.bIndex == b.bIndex);
	}
	
	public static bool operator!=(Edge a, Edge b) {
		return !(a == b);
	}
	
    public override bool Equals(System.Object obj)
    {
		Edge c = (Edge)obj;		
		return this == c;
    }

    public bool Equals(Edge p)
    {
        return this == p;
    }

    public override int GetHashCode()
    {
		int hashCode = 0;
		hashCode = aIndex + bIndex;
		hashCode *= aIndex - bIndex;
		return hashCode;
    }
	
	public void Draw(Color lineColor, float width) {
		GL.Begin(GL.QUADS);
		
		Vector3 perpendicular = (new Vector3(_bPos.y, _aPos.x, 0) - new Vector3(_aPos.y, _bPos.x, 0)).normalized * width;
		GL.Color(lineColor);
		GL.Vertex(_aPos - perpendicular);
		GL.Vertex(_aPos + perpendicular);
		GL.Vertex(_bPos + perpendicular);
		GL.Vertex(_bPos - perpendicular);
		GL.End();
	}
	
	public void Draw() {
		float width = (_isExternal) ? 0.05f : 0.01f;
		Color lineColor = _isExternal ? Color.red : Color.magenta;
		
		Draw(lineColor, width);
	}
	
	public override string ToString() {
		
		return string.Format("{0}->{1}", aIndex, bIndex);
	}
}
