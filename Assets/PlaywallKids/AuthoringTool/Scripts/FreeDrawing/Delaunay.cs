using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Delaunay {
	public struct DTriangle {
		public Triangle triangle;
		public Edge[] edges;
		
		public DTriangle(Triangle t, Edge[] e) {
			triangle = t;
			edges = e;
		}
		
		public int externalEdgeCount {
			get {
				int count = 0;
				for(int i = 0, cnt = edges.Length; i < cnt; i++) {
					if(edges[i].isExternal) count++;
				}
				return count;
			}
		}
		
		public void Draw() {
			triangle.Draw(Color.green);
		}
	}
	
	public DTriangle[] DoDelaunay(List<Vector3> vertices) {
		return DoDelaunay(vertices.ToArray());
	}
	
	public DTriangle[] DoDelaunayN3(List<Vector3> vertices) {
		return DoDelaunayN3(vertices.ToArray());
	}
	
	public DTriangle[] DoDelaunay(Vector3[] vertices) {
		List<DTriangle> result = new List<DTriangle>();
		
		List<Edge> CE = new List<Edge>();
		List<Edge> DE = new List<Edge>();
		
		// find first edge (smallest)
		Vector3 v0 = vertices[0];
		float len = float.MaxValue;
		int minIdx = 0;
		for(int i = 1; i < vertices.Length; i++) {
			float newLen = (vertices[i]-v0).sqrMagnitude;
			if(len > newLen) {
				minIdx = i;
				len = newLen;
			}
		}
		
		Edge edge = new Edge(v0, vertices[minIdx], 0, minIdx);
		CE.Add(edge);
		edge = new Edge(vertices[minIdx], v0, minIdx, 0);
		CE.Add(edge);
		
		// Time Check
		System.DateTime prev = System.DateTime.Now;
		int loopCount = 0;
		
		// Looping
		while(CE.Count > 0) {
			loopCount += 1;
			if(loopCount > 1000000) {
				Debug.Log ("Oh, NO!!!!");
				break;
			}
			edge = CE[0];
			CE.RemoveAt(0);
			
			Triangle resTriangle = null;
			Circle c = new Circle();
			int idx = -1;
			bool findCircle = false;
			
			for(int i = 0, cnt = vertices.Length; i < cnt; i++) {
				if(i == edge.aIndex || i == edge.bIndex) continue;
				
				// Make triangle
				Triangle t = new Triangle(edge.aPos, edge.bPos, vertices[i]);
				
				// is CCW?
				if(t.isCounterClockwise) {
					if(findCircle == false) {
						findCircle = true;
						c = t.GetCircumcircle();
						resTriangle = t;
						idx = i;
					}
					else if(c.IsIn(vertices[i])) {
						c = t.GetCircumcircle();
						resTriangle = t;
						idx = i;
					}
				}
			}
			
			if(idx > -1) {
				int i = idx;
				
				// Counter-Clockwise edges
				Edge e2 = new Edge(edge.bPos, vertices[i], edge.bIndex, i);
				Edge e3 = new Edge(vertices[i], edge.aPos, i, edge.aIndex);
				
				// Clockwise edges
				Edge e2C = e2.GetReversed();
				Edge e3C = e3.GetReversed();
				
				bool c1 = !DE.Contains(edge), c2 = !DE.Contains(e2), c3 = !DE.Contains(e3);
				
				if(c1 || c2 || c3) {
					DTriangle resultTriangle = new DTriangle(resTriangle, new Edge[] { edge, e2, e3 });
					result.Add (resultTriangle);
				
					if(c1) DE.Add(edge);
					if(c2) DE.Add(e2);
					if(c3) DE.Add(e3);
				
					if(!DE.Contains(e2C) && !CE.Contains(e2C))
						CE.Add(e2C);
					if(!DE.Contains (e3C) && !CE.Contains(e3C))
						CE.Add(e3C);
				}
			}
		}
		
		System.DateTime now = System.DateTime.Now;
		double sec = now.Subtract(prev).TotalSeconds;
		
		Debug.Log (string.Format("Loop count : {0}, time : {1:0.0000}s", loopCount, sec));
		
		return result.ToArray();
	}
	
	// O(n^3) version
	public DTriangle[] DoDelaunayN3(Vector3[] vertices) {
		List<DTriangle> result = new List<DTriangle>();
		
		List<Edge> CE = new List<Edge>();
		List<Edge> DE = new List<Edge>();
		
		// find first edge (smallest)
		Vector3 v0 = vertices[0];
		float len = float.MaxValue;
		int minIdx = 0;
		for(int i = 1; i < vertices.Length; i++) {
			float newLen = (vertices[i]-v0).sqrMagnitude;
			if(len > newLen) {
				minIdx = i;
				len = newLen;
			}
		}
		
		Edge edge = new Edge(v0, vertices[minIdx], 0, minIdx);
		CE.Add(edge);
		edge = new Edge(vertices[minIdx], v0, minIdx, 0);
		CE.Add(edge);
		
		// Time Check
		System.DateTime prev = System.DateTime.Now;
		int loopCount = 0;
		
		// Looping
		while(CE.Count > 0) {
			loopCount += 1;
			if(loopCount > 1000000) {
				Debug.Log ("Oh, NO!!!!");
				break;
			}
			edge = CE[0];
			CE.RemoveAt(0);
			
			for(int i = 0, cnt = vertices.Length; i < cnt; i++) {
				if(i == edge.aIndex || i == edge.bIndex) continue;
				
				// Make triangle
				Triangle t = new Triangle(edge.aPos, edge.bPos, vertices[i]);
				
				// is CCW?
				if(t.isCounterClockwise) {
					// Get circumcircle
					Circle c = t.GetCircumcircle();
					bool isValid = true;
					for(int j = 0; j < cnt; j++) {
						if(j == edge.aIndex || j == edge.bIndex || j == i) continue;
						// Check if any other vertices is contained in circle
						if(c.IsIn(vertices[j])) {
							isValid = false;
							break;
						}
					}
					
					if(isValid) {
						// Counter-Clockwise edges
						Edge e2 = new Edge(edge.bPos, vertices[i], edge.bIndex, i);
						Edge e3 = new Edge(vertices[i], edge.aPos, i, edge.aIndex);
						
						// Clockwise edges
						Edge e2C = e2.GetReversed();
						Edge e3C = e3.GetReversed();
						
						DTriangle resultTriangle = new DTriangle(t, new Edge[] { edge, e2, e3 });
						result.Add (resultTriangle);
						
						if(!DE.Contains(edge)) DE.Add(edge);
						DE.Add(e2);
						DE.Add(e3);
						
						if(!DE.Contains(e2C))
							CE.Add(e2C);
						if(!DE.Contains (e3C))
							CE.Add(e3C);
						
						
					}
				}
			}
		}
		
		System.DateTime now = System.DateTime.Now;
		double sec = now.Subtract(prev).TotalSeconds;
		
		Debug.Log (string.Format("Loop count : {0}, time : {1:0.0000}s", loopCount, sec));
		
		return result.ToArray();
	}
}
