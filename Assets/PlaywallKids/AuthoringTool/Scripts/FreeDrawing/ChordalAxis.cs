using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Poly2Tri;

public struct ChordalAxisInfo {
	public enum Type {
		SinglePoint,
		TwoPoint,
		Terminal,
		Sleeve,
		Junction
	}

	// type
	public Type type;

	// points
	public List<Point2D> points;

	// only used when type is junction
	public bool[] invaded;

	public ChordalAxisInfo(Type newType, List<Point2D> newPoints) {
		type = newType;
		points = new List<Point2D>();
		if(newPoints != null) {
			points.AddRange(newPoints);
		}
		invaded = new bool[3];
	}

	public static bool operator==(ChordalAxisInfo a, ChordalAxisInfo b) {
		return (a.type == b.type) && (a.points.Equals(b.points));
	}

	public static bool operator!=(ChordalAxisInfo a, ChordalAxisInfo b) {
		return !(a == b);
	}

	public override bool Equals(System.Object b) {
		if(b == null) return false;
		else {
			ChordalAxisInfo bb = (ChordalAxisInfo)b;
			if((System.Object)bb == null) return false;

			return this == bb;
		}
	}

	public bool Equals(ChordalAxisInfo b) {
		return this == b;
	}

	public override int GetHashCode() {
		return type.GetHashCode() ^ points.GetHashCode();
	}
}

public class ChordalAxis {
	public List<DelaunayTriangle> triangles = new List<DelaunayTriangle>();
	public Dictionary<DelaunayTriangle, ChordalAxisInfo> chordalAxisInfoDict = new Dictionary<DelaunayTriangle, ChordalAxisInfo>();
	public Dictionary<Point2D, Dictionary<Point2D, int>> pointConnectionsDict = new Dictionary<Point2D, Dictionary<Point2D, int>>();

	private List<Point2D> _cachedPoints = new List<Point2D>(16);

	public void AddTriangle(DelaunayTriangle triangle) {
		if(triangle == null) return;

		if(triangles.Contains(triangle) == false) {
			triangles.Add(triangle);
			AddPointConnectionsFromTriangle(triangle);
			ChordalAxisInfo info = GetChordalAxisInfoForTriangle(triangle);
			chordalAxisInfoDict[triangle] = info;
		}
	}

	public void AddTriangle(DelaunayTriangle triangle, ChordalAxisInfo info) {
		if(triangle == null) return;
		
		if(triangles.Contains(triangle) == false) {
			triangles.Add(triangle);
			AddPointConnectionsFromTriangle(triangle, info);
			chordalAxisInfoDict[triangle] = info;
		}
	}

	public void RemoveTriangle(DelaunayTriangle triangle) {
		if(triangle == null) return;

		if(triangles.Contains(triangle)) {
			ChordalAxisInfo info = chordalAxisInfoDict[triangle];
			triangles.Remove(triangle);
			chordalAxisInfoDict.Remove(triangle);
			RemovePointConnectionsFromTriangle(triangle, info);
		}
	}

	public void AddPointConnectionsFromTriangle(DelaunayTriangle triangle) {
		AddPointConnections(triangle.Points[0], triangle.Points[1], triangle.Points[2]);
		AddPointConnections(triangle.Points[1], triangle.Points[0], triangle.Points[2]);
		AddPointConnections(triangle.Points[2], triangle.Points[0], triangle.Points[1]);
	}
	
	public void AddPointConnectionsFromTriangle(DelaunayTriangle triangle, ChordalAxisInfo info) {
		if(info.type == ChordalAxisInfo.Type.TwoPoint) {
			int index1 = triangle.IndexOf((TriangulationPoint)info.points[0]);
			int index2 = triangle.IndexOf((TriangulationPoint)info.points[1]);
			int index3 = 0;
			for(int i = 0; i < 3; i++) {
				if(i != index1 && i != index2) {
					index3 = i;
					break;
				}
			}
			
			AddPointConnections(triangle.Points[index3], triangle.Points[index1], triangle.Points[index2]);
			AddPointConnections(triangle.Points[index1], triangle.Points[index3]);
			AddPointConnections(triangle.Points[index2], triangle.Points[index3]);
		}
		else {
			AddPointConnections(triangle.Points[0], triangle.Points[1], triangle.Points[2]);
			AddPointConnections(triangle.Points[1], triangle.Points[0], triangle.Points[2]);
			AddPointConnections(triangle.Points[2], triangle.Points[0], triangle.Points[1]);
		}
	}

	public void RemovePointConnectionsFromTriangle(DelaunayTriangle triangle) {
		RemovePointConnections(triangle.Points[0], triangle.Points[1], triangle.Points[2]);
		RemovePointConnections(triangle.Points[1], triangle.Points[0], triangle.Points[2]);
		RemovePointConnections(triangle.Points[2], triangle.Points[0], triangle.Points[1]);
	}

	public void RemovePointConnectionsFromTriangle(DelaunayTriangle triangle, ChordalAxisInfo info) {
		if(info.type == ChordalAxisInfo.Type.TwoPoint) {
			int index1 = triangle.IndexOf((TriangulationPoint)info.points[0]);
			int index2 = triangle.IndexOf((TriangulationPoint)info.points[1]);
			int index3 = 0;
			for(int i = 0; i < 3; i++) {
				if(i != index1 && i != index2) {
					index3 = i;
					break;
				}
			}
			
			RemovePointConnections(triangle.Points[index3], triangle.Points[index1], triangle.Points[index2]);
			RemovePointConnections(triangle.Points[index1], triangle.Points[index3]);
			RemovePointConnections(triangle.Points[index2], triangle.Points[index3]);
		}
		else {
			RemovePointConnections(triangle.Points[0], triangle.Points[1], triangle.Points[2]);
			RemovePointConnections(triangle.Points[1], triangle.Points[0], triangle.Points[2]);
			RemovePointConnections(triangle.Points[2], triangle.Points[0], triangle.Points[1]);
		}
	}

	public void AddPointConnections(Point2D from, params Point2D[] tos) {
		if(tos != null && tos.Length > 0) {
			Dictionary<Point2D, int> connections = null;
			if(pointConnectionsDict.ContainsKey(from)) {
				connections = pointConnectionsDict[from];
			}
			else {
				connections = new Dictionary<Point2D, int>();
				pointConnectionsDict[from] = connections;
			}

			for(int i = 0, cnt = tos.Length; i < cnt; i++) {
				Point2D to = tos[i];
				if(to != null) {
					if(connections.ContainsKey(to)) {
						connections[to]++;
					}
					else {
						connections[to] = 1;
					}
				}
			}
		}
	}

	public void RemovePointConnections(Point2D from, params Point2D[] tos) {
		if(tos != null && tos.Length > 0) {
			Dictionary<Point2D, int> connections = null;
			if(pointConnectionsDict.ContainsKey(from)) {
				connections = pointConnectionsDict[from];
			}
			else {
				connections = new Dictionary<Point2D, int>();
				pointConnectionsDict[from] = connections;
			}
			
			for(int i = 0, cnt = tos.Length; i < cnt; i++) {
				Point2D to = tos[i];
				if(to != null) {
					if(connections.ContainsKey(to)) {
						int count = connections[to] - 1;
						if(count <= 0) {
							connections.Remove(to);
						}
					}
				}
			}

			if(connections.Count == 0) {
				pointConnectionsDict.Remove(from);
			}
		}
	}

	public void Clear() {
		triangles.Clear();
		chordalAxisInfoDict.Clear();
		pointConnectionsDict.Clear();
	}

	public ChordalAxisInfo GetChordalAxisInfoForTriangle(DelaunayTriangle triangle) {
		int constrainedCount = Util.GetTriangleConstrainedCount(triangle);
		
		// sleeve
		if(constrainedCount == 1) {
			int CIndex = Util.GetTriangleCPointIndex(triangle);
			return GetChordalAxisInfoForSleeveTriangle(triangle, CIndex);
		}
		// terminal
		else if(constrainedCount == 2) {
			int NCIndex = Util.GetTriangleNCPointIndex(triangle);
			return GetChordalAxisInfoForTerminalTriangle(triangle, NCIndex);
		}
		// junction
		else {
			return GetChordalAxisInfoForJunctionTriangle(triangle);
		}
	}

	public ChordalAxisInfo GetChordalAxisInfoForSleeveTriangle(DelaunayTriangle triangle, int CIndex) {
		List<Point2D> points = _cachedPoints;
		points.Clear();
		points.Add((triangle.Points[CIndex] + triangle.Points[(CIndex+1)%3])*0.5f);
		points.Add((triangle.Points[CIndex] + triangle.Points[(CIndex+2)%3])*0.5f);

		ChordalAxisInfo info = new ChordalAxisInfo(ChordalAxisInfo.Type.Sleeve, points);
		return info;
	}

	public ChordalAxisInfo GetChordalAxisInfoForTerminalTriangle(DelaunayTriangle triangle, int NCIndex) {
		List<Point2D> points = _cachedPoints;
		points.Clear();
		points.Add(triangle.Points[NCIndex]);
		points.Add((triangle.Points[(NCIndex+1)%3] + triangle.Points[(NCIndex+2)%3])*0.5f);

		ChordalAxisInfo info = new ChordalAxisInfo(ChordalAxisInfo.Type.Terminal, points);
		return info;
	}

	public ChordalAxisInfo GetChordalAxisInfoForJunctionTriangle(DelaunayTriangle triangle) {
		List<Point2D> points = _cachedPoints;
		points.Clear();
		
		Point2D p01 = (triangle.Points[0] + triangle.Points[1]) * 0.5f;
		Point2D p12 = (triangle.Points[1] + triangle.Points[2]) * 0.5f;
		Point2D p20 = (triangle.Points[2] + triangle.Points[0]) * 0.5f;
		Point2D center = null;

		Point2D v01 = triangle.Points[1] - triangle.Points[0];
		Point2D v02 = triangle.Points[2] - triangle.Points[0];

		Vector3 vp01 = new Vector3(-v01.Yf, v01.Xf);
		Vector3 vp02 = new Vector3(-v02.Yf, v02.Xf);

		Edge e1 = new Edge(Util.PointToVector3(p01) - vp01 * 1000.0f, Util.PointToVector3(p01) + vp01 * 1000.0f, 0, 1);
		Edge e2 = new Edge(Util.PointToVector3(p20) - vp02 * 1000.0f, Util.PointToVector3(p20) + vp02 * 1000.0f, 2, 3);
		Vector3 outCenter;
		if(Edge.GetIntersection(e1, e2, out outCenter)) {
			center = Util.Vector3ToPoint(outCenter);
			Triangle tt = new Triangle(Util.PointToVector3(triangle.Points[0]), Util.PointToVector3(triangle.Points[1]), Util.PointToVector3(triangle.Points[2]));
			if(!tt.IsIn(outCenter)) {
				center = triangle.Centroid();
			}
		}
		else {
			center = triangle.Centroid();
		}

		points.Add(center);
		points.Add(p12);
		points.Add(p20);
		points.Add(p01);

		ChordalAxisInfo info = new ChordalAxisInfo(ChordalAxisInfo.Type.Junction, points);
		return info;
	}

	public ChordalAxisInfo GetChordalAxisInfoForSinglePointTriangle(DelaunayTriangle triangle, int index) {
		List<Point2D> points = _cachedPoints;
		points.Clear();
		points.Add(triangle.Points[index]);
		
		ChordalAxisInfo info = new ChordalAxisInfo(ChordalAxisInfo.Type.SinglePoint, points);
		return info;
	}

	public ChordalAxisInfo GetChordalAxisInfoForTwoPointTriangle(DelaunayTriangle triangle, int index1, int index2) {
		List<Point2D> points = _cachedPoints;
		points.Clear();
		points.Add(triangle.Points[index1]);
		points.Add(triangle.Points[index2]);
		
		ChordalAxisInfo info = new ChordalAxisInfo(ChordalAxisInfo.Type.TwoPoint, points);
		return info;
	}

	public float GetChordalAxisPointHeight(Point2D point) {
		float length = 0.0f;
		float min = float.MaxValue;
		
		if(pointConnectionsDict.ContainsKey(point)) {
			Dictionary<Point2D, int> connections = pointConnectionsDict[point];
			
			foreach(Point2D v in connections.Keys) {
				if(connections[v] > 0) {
					length += (float)(point - v).Magnitude();
					min = Mathf.Min(min, (float)(point - v).Magnitude());
				}			
			}
			
			length /= (float)connections.Count;
			if(min < length) {
				//length = (length + min) * 0.5f;
			}
		}

		//length = .25f;

		return length;
	}

	public float GetChordalAxisPointHeight(Vector3 point) {
		return GetChordalAxisPointHeight(Util.Vector3ToPoint(point));
	}

	public Vector3 GetAveragePointOfPointConnections(Point2D point) {
		Vector3 result = Util.PointToVector3(point);
		if(pointConnectionsDict.ContainsKey(point)) {
			Dictionary<Point2D, int> connections = pointConnectionsDict[point];
			Point2D sum = new Point2D(0.0, 0.0);

			foreach(Point2D v in connections.Keys) {
				sum += v;
			}

			sum /= (float)connections.Count;
			result = Util.PointToVector3(sum) * 0.5f + result * 0.5f;
		}
		else {
			Debug.Log ("found failed " + point);
		}

		return result;
	}

	public Vector3 GetAveragePointOfPointConnections(Vector3 point) {
		return GetAveragePointOfPointConnections(Util.Vector3ToPoint(point));
	}

	public float GetChordalAxisPointMaxHeight() {
		float maxHeight = 0.0f;
		foreach(Point2D point in pointConnectionsDict.Keys) {
			maxHeight = Mathf.Max(maxHeight, GetChordalAxisPointHeight(point));
		}
		return maxHeight;
	}
}
