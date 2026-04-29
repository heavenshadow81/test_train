using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Util : MonoBehaviour {
	public static Vector3 PointToVector3(Poly2Tri.Point2D p) {
		return new Vector3((float)p.X, (float)p.Y);
	}

	public static Poly2Tri.Point2D Vector3ToPoint(Vector3 p) {
		return new Poly2Tri.Point2D(p.x, p.y);
	}

	public static int GetTriangleCPointIndex(Poly2Tri.DelaunayTriangle t) {
		int CIndex = -1;

		if(t != null) {
			for(int i = 0; i < 3; i++) {
				if(t.EdgeIsConstrained[i]) {
					CIndex = i;
					break;
				}
			}
		}

		return CIndex;
	}
	
	public static int GetTriangleNCPointIndex(Poly2Tri.DelaunayTriangle t) {
		int NCIndex = -1;
		
		if(t != null) {
			for(int i = 0; i < 3; i++) {
				if(!t.EdgeIsConstrained[i]) {
					NCIndex = i;
					break;
				}
			}
		}
		
		return NCIndex;
	}

	public static int GetTriangleConstrainedCount(Poly2Tri.DelaunayTriangle t) {
		int count = 0;

		if(t != null) {
			for(int i = 0; i < 3; i++) {
				if(t.EdgeIsConstrained[i]) {
					count++;
				}
			}
		}

		return count;
	}

	public static void LaplacianSmooth(List<Poly2Tri.Point2D> list, float lambda, int iterationCount, bool connected) {
		iterationCount = Mathf.Clamp(iterationCount, 0, 100);

		while(iterationCount-- > 0) {
			List<Poly2Tri.Point2D> output = new List<Poly2Tri.Point2D>(list.Count);
			
			for(int i = 0, cnt = list.Count; i < cnt; i++) {
				var p = list[i];
				
				if(!(connected && (i == 0 || i + 1 == cnt))) {
					var prev = list[(i+cnt-1)%cnt];
					var next = list[(i+1)%cnt];
					p = p + ((prev - p) * 0.5f + (next - p) * 0.5f) * lambda;
				}
				output.Add(p);
			}
			
			for(int i = 0, cnt = list.Count; i < cnt; i++) {
				list[i] = output[i];
			}
		}
	}
	
	public static void LaplacianSmooth(List<Poly2Tri.TriangulationPoint> list, float lambda, int iterationCount, bool connected) {
		iterationCount = Mathf.Clamp(iterationCount, 0, 100);
		
		while(iterationCount-- > 0) {
			List<Poly2Tri.TriangulationPoint> output = new List<Poly2Tri.TriangulationPoint>(list.Count);
			
			for(int i = 0, cnt = list.Count; i < cnt; i++) {
				var p = list[i];

				if(!(connected && (i == 0 || i + 1 == cnt))) {
					var prev = list[(i+cnt-1)%cnt];
					var next = list[(i+1)%cnt];
					var newP = new Poly2Tri.TriangulationPoint(0.0, 0.0);
					newP.Set(p + ((prev - p) * 0.5f + (next - p) * 0.5f) * lambda);
					p = newP;
				}
				output.Add(p);
			}
			
			for(int i = 0, cnt = list.Count; i < cnt; i++) {
				list[i] = output[i];
			}
		}
	}
	
	public static void LaplacianSmooth(List<Vector3> list, float lambda, int iterationCount, bool connected) {
		iterationCount = Mathf.Clamp(iterationCount, 0, 100);
		
		while(iterationCount-- > 0) {
			List<Vector3> output = new List<Vector3>(list.Count);
			
			for(int i = 0, cnt = list.Count; i < cnt; i++) {
				var p = list[i];
				
				if(!(connected && (i == 0 || i + 1 == cnt))) {
					var prev = list[(i+cnt-1)%cnt];
					var next = list[(i+1)%cnt];
					p = p + ((prev - p) * 0.5f + (next - p) * 0.5f) * lambda;
				}
				output.Add(p);
			}
			
			for(int i = 0, cnt = list.Count; i < cnt; i++) {
				list[i] = output[i];
			}
		}
	}
	
	public static void LaplacianSmooth(List<float> list, float lambda, int iterationCount, bool connected) {
		iterationCount = Mathf.Clamp(iterationCount, 0, 100);
		
		while(iterationCount-- > 0) {
			List<float> output = new List<float>(list.Count);
			
			for(int i = 0, cnt = list.Count; i < cnt; i++) {
				var p = list[i];
				
				if(!(!connected && (i == 0 || i + 1 == cnt))) {
					var prev = list[(i+cnt-1)%cnt];
					var next = list[(i+1)%cnt];
					p = p + ((prev - p) * 0.5f + (next - p) * 0.5f) * lambda;
				}
				output.Add(p);
			}
			
			for(int i = 0, cnt = list.Count; i < cnt; i++) {
				list[i] = output[i];
			}
		}
	}
}
