using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spline {
	private List<Edge> _edges = new List<Edge>();
	public List<Edge> edges {
		get {
			return _edges;
		}
	}

	public enum Method {
		None,
		ConvexHull,
		ConcaveHull
	}
	
	public static Spline Make(Vector3[] vertices, Method method) {
		switch(method) {
		case Method.None:
			return _MakeSpline(vertices);
		case Method.ConvexHull:
			return _MakeConvexHullSpline(vertices);
		case Method.ConcaveHull:
			return _MakeConcaveHullSpline(vertices);
		default:
			return null;
		}
	}
	
	private Spline() {
	}
	
	private static Spline _MakeSpline(List<Vector3> vertices) {
		return _MakeSpline(vertices, false);
	}
	
	private static Spline _MakeSpline(List<Vector3> vertices, bool connectFirstAndLast) {
		Spline spline = new Spline();
		
		for(int i = 1, cnt = vertices.Count; i < cnt; i++) {
			Edge edge = new Edge(vertices[i-1], vertices[i], i-1, i);
			spline.edges.Add(edge);
		}
		
		if(connectFirstAndLast) {
			Edge edge = new Edge(vertices[vertices.Count-1], vertices[0], vertices.Count-1, 0);
			spline.edges.Add(edge);
		}
		
		return spline;
	}
	
	private static Spline _MakeSpline(Vector3[] vertices) {
		return _MakeSpline(vertices, false);
	}
	
	private static Spline _MakeSpline(Vector3[] vertices, bool connectFirstAndLast) {
		Spline spline = new Spline();
		spline.edges.Capacity = vertices.Length;
		
		for(int i = 1, cnt = vertices.Length; i < cnt; i++) {
			Edge edge = new Edge(vertices[i-1], vertices[i], i-1, i);
			spline.edges.Add(edge);
		}
		
		if(connectFirstAndLast) {
			Edge edge = new Edge(vertices[vertices.Length-1], vertices[0], vertices.Length-1, 0);
			spline.edges.Add(edge);
		}
		
		return spline;
	}
	
	// We use Graham's algorithm to get convex hull at O(n log n)
	private static Spline _MakeConvexHullSpline(Vector3[] vertices) {
		List<Vector3> vList = new List<Vector3>(vertices);
		List<Vector3> stack = new List<Vector3>(vList.Count);
		
		// 1. Find the lowest (smallest y position) vertex (guaranteed to be on the hull)
		int lowestIdx = -1;
		float maxX = float.MinValue;
		
		lowestIdx = _FindBottomRightVertex(vList, out maxX);
		
		// 2. Sort vertices based on counter-clockwise angle
		Vector3 lowestVertex = vList[lowestIdx];
		
		vList.RemoveAt(lowestIdx);
		
		Vector3 horizontalVector = Vector3.right;
		vList.Sort((x, y) => {
			Vector3 xVector = x - lowestVertex;
			Vector3 yVector = y - lowestVertex;
			
			float xVal = Vector3.Dot (horizontalVector, xVector) / xVector.magnitude;
			float yVal = Vector3.Dot (horizontalVector, yVector) / yVector.magnitude;
			
			return yVal.CompareTo(xVal);
		});
		
		stack.Add(vList[vList.Count-1]);
		stack.Add(lowestVertex);
		
		// Get!
		for(int i = 0, cnt = vList.Count, top = stack.Count-1; i < cnt; i++) {
			while(top > 0) {
				Vector3 v1 = stack[top-1];
				Vector3 v2 = stack[top];
				Vector3 v3 = vList[i];
				if(Triangle.IsCounterClockwise(v1, v2, v3)) {
					stack.Add(v3);
					top++;
					break;
				}
				else {
					stack.RemoveAt(top--);
				}
			}
		}
		
		return _MakeSpline(stack);
	}
	
	public struct SplitInfo {
		public float s;
		public int index;
	}
	
	public struct IndexInfo {
		public int actualIndex;
		public int indicesIndex;
		public int direction;
	}
	
	private class ConcaveHullSplineExtraInfo {
		public List<Vector3> vertices = new List<Vector3>();
		public List<int> indices = new List<int>();
		public List<bool> isIntersections = new List<bool>();

		public Dictionary<int, List<IndexInfo>> intersecionsNextVertexIndexInfosDict = new Dictionary<int, List<IndexInfo>>();
	}
	
	private static Spline _MakeConcaveHullSpline(Vector3[] vertices) {
		bool connectFirstAndLast = false;
		if(vertices != null && vertices.Length > 0 && (vertices[vertices.Length - 1] != vertices[0]))
			connectFirstAndLast = true;
		
		// Input edges
		Spline spline = _MakeSpline(vertices, connectFirstAndLast);
		
		// 1. Get ALL Intersection Points
		ConcaveHullSplineExtraInfo info = new ConcaveHullSplineExtraInfo();
		info.vertices.AddRange(vertices);
		for(int i = 0, cnt = info.vertices.Count; i < cnt; i++) {
			info.isIntersections.Add(false);
		}
		
		List<List<SplitInfo>> splitInfoList = new List<List<SplitInfo>>(spline.edges.Count);
		for(int i = 0, cnt = spline.edges.Count; i < cnt; i++) {
			splitInfoList.Add(new List<SplitInfo>(3));
		}
		
		// error value 
		float epsilon = 0.000001f;
		
		// add first index
		if(spline.edges.Count > 0) {
			info.indices.Add(spline.edges[0].aIndex);
		}
		
		// loop!
		for(int i = 0, cnt = spline.edges.Count; i < cnt; i++) {
			List<SplitInfo> splitInfos = splitInfoList[i];
			
			// find intersection points and add to vertices
			for(int j = i + 1; j < cnt; j++) {
				float outS = 0.0f, outT = 0.0f;
				Vector3 intersectionPoint = Vector3.zero;
				if(spline.edges[i].GetIntersection(spline.edges[j], out outS, out outT, out intersectionPoint)) {
					if((Mathf.Abs(outS) > epsilon && Mathf.Abs(outT - 1.0f) > epsilon) && (Mathf.Abs(outS - 1.0f) > epsilon && Mathf.Abs(outT) > epsilon)) {
						int index = info.vertices.FindIndex((v) => {
							return (intersectionPoint - v).sqrMagnitude < epsilon;
						});
						
						if(index < 0) {
							info.vertices.Add(intersectionPoint);
							info.isIntersections.Add(true);
							index = info.vertices.Count-1;
						}
						
						SplitInfo iSplitInfo = new SplitInfo();
						iSplitInfo.index = index;
						iSplitInfo.s = outS;
						
						splitInfos.Add(iSplitInfo);
						
						SplitInfo jSplitInfo = new SplitInfo();
						jSplitInfo.index = index;
						jSplitInfo.s = outT;
						
						splitInfoList[j].Add(jSplitInfo);
						info.isIntersections[index] = true;
					}
				}
			}
			
			// sort split infos
			splitInfos.Sort((x, y) => {
				return x.s.CompareTo(y.s);
			});
			
			// add indices
			for(int k = 0, splitCount = splitInfos.Count; k < splitCount; k++) {
				SplitInfo splitInfo = splitInfos[k];
				int index = splitInfo.index;
				info.indices.Add(index);
				
				List<IndexInfo> list = null;
				if(info.intersecionsNextVertexIndexInfosDict.ContainsKey(index) == false) {
					list = new List<IndexInfo>();
					info.intersecionsNextVertexIndexInfosDict[index] = list;
				}
				else {
					list = info.intersecionsNextVertexIndexInfosDict[index];
				}
				
				// next index info
				IndexInfo indexInfo = new IndexInfo();
				indexInfo.indicesIndex = info.indices.Count;
				indexInfo.direction = 1;
				if(k + 1 < splitCount) {
					indexInfo.actualIndex = splitInfos[k+1].index;
				}
				else {
					indexInfo.actualIndex = spline.edges[i].bIndex;
					if(indexInfo.actualIndex == 0) {
						indexInfo.indicesIndex = 0;
					}
				}
				list.Add(indexInfo);
				
				// prev index info
				/*
				indexInfo = new IndexInfo();
				indexInfo.indicesIndex = info.indices.Count - 2;
				indexInfo.actualIndex = info.indices[indexInfo.indicesIndex];
				indexInfo.direction = -1;
				list.Add(indexInfo);
				*/
			}
			
			if(i + 1 != cnt) {
				info.indices.Add(spline.edges[i].bIndex);
			}
		}
		
		// output points
		List<Vector3> output = new List<Vector3>();
		
		/*
		for(int i = 0, cnt = info.indices.Count; i < cnt; i++) {
			output.Add(info.vertices[info.indices[i]]);
		}
		*/
		
		// 2. Find low-right points
		int startIndex = -1;
		int prevIndex = -1;
		int currentIndex = -1;
		int nextIndex = -1;
		int direction = 1;
		
		float maxX = float.MinValue;
		
		int lowestVertexIndex = _FindBottomRightVertex(info.vertices, out maxX);
		
		Vector3 lowestVertex = info.vertices[lowestVertexIndex];
		Vector3 positiveVector = Vector3.right;
		
		// 3. Find the start index to loop
		// (Find the outest vertex next to the lowest vertex)
		float cosValue = 0.0f;
		for(int i = 0, cnt = info.indices.Count; i < cnt; i++) {
			if(info.indices[i] == lowestVertexIndex) {
				int nextVertexIdx = info.indices[(i+1) % cnt];
				Vector3 v = info.vertices[nextVertexIdx] - lowestVertex;
				float newCosValue = Mathf.Abs(Vector3.Dot(v, positiveVector) / v.magnitude);
				if(cosValue < newCosValue) {
					cosValue = newCosValue;
					startIndex = i;
					currentIndex = (i+1)%cnt;
				}
			}
		}
		
		prevIndex = startIndex;
		
		// 4. Find!
		int indicesCnt = info.indices.Count;
		output.Add(lowestVertex);
		
		Vector3 prevVertex = Vector3.zero;
		Vector3 currentVertex = Vector3.zero;
		
		int loopCnt = 0;

		try {
			do {
				if(loopCnt++ > vertices.Length * 10)
				{
					Debug.Log ("Spline._MakeConcaveHulSpline() : Couldn't find concave hull. Used Convex hull spline instead.");
					return _MakeConvexHullSpline(vertices);
				}
				
				prevIndex = currentIndex - 1 * direction;
				if(prevIndex < 0) prevIndex = indicesCnt - 1;
				else if(prevIndex >= indicesCnt) prevIndex = 0;
				
				prevVertex = info.vertices[info.indices[prevIndex]];
				currentVertex = info.vertices[info.indices[currentIndex]];
				
				// is intersection point?
				if(info.isIntersections[info.indices[currentIndex]]) {
					int prev2Index = (currentIndex - 2 < 0 ? indicesCnt - 2 : currentIndex - 2);
					Vector3 prev2Vertex = info.vertices[info.indices[prev2Index]];
					
					Vector3 v1 = currentVertex - prevVertex;
					bool isCCW = Triangle.IsCounterClockwise(prev2Vertex, prevVertex, currentVertex);
					
					// smallest cosine value
					float smallestPositiveCosVal = float.MaxValue;
					float smallestNegativeCosVal = float.MaxValue;
					
					int nextPositiveIndex = 0;
					int nextNegativeIndex = 0;
					
					int positiveDirection = direction;
					int negativeDirection = direction;
					
					// has found negative clockwise?
					//bool foundNegative = false;
					
					foreach(IndexInfo key in info.intersecionsNextVertexIndexInfosDict[info.indices[currentIndex]]) {
						Vector3 v2 = info.vertices[key.actualIndex] - currentVertex;
						
						float newCosVal = Mathf.Abs(Vector3.Dot(v1, v2)/(v1.magnitude*v2.magnitude));
						bool isCCW2 = Triangle.IsCounterClockwise(prevVertex, currentVertex, info.vertices[key.actualIndex]);
						//isCCW2 = isCCW2 && (key.direction > 0);
						
						if(isCCW != isCCW2) {
							//foundNegative = true;
							
							if(smallestPositiveCosVal > newCosVal) {
								smallestPositiveCosVal = newCosVal;
								nextPositiveIndex = key.indicesIndex;
								positiveDirection = key.direction;
							}
						}
						else/* if(!foundNegative)*/ {
							if(smallestNegativeCosVal > newCosVal) {
								smallestNegativeCosVal = newCosVal;
								nextNegativeIndex = key.indicesIndex;
								negativeDirection = key.direction;
							}
						}
						
						if(smallestPositiveCosVal < smallestNegativeCosVal) {
						//if(foundNegative) {
							nextIndex = nextPositiveIndex;
							direction = positiveDirection;
						}
						else {
							nextIndex = nextNegativeIndex;
							direction = negativeDirection;
						}
					}
				}
				else {
					nextIndex = currentIndex + 1 * direction;
					if(nextIndex < 0) nextIndex = indicesCnt - 1;
					else if(nextIndex >= indicesCnt) nextIndex = 0;
				}
				
				output.Add(currentVertex);
				
				currentIndex = nextIndex;
			}
			while(currentIndex != startIndex || currentIndex != nextIndex);
			
			return _MakeSpline(output, true);
		}
		catch(System.Exception e) {
			Debug.Log ("Spline._MakeConcaveHulSpline() : Couldn't find concave hull. Used Convex hull spline instead.");
			return _MakeConvexHullSpline(vertices);
		}
	}
	
	private static int _FindBottomRightVertex(List<Vector3> vList, out float maxX) {
		// 1. Find the lowest (smallest y position) vertex (guaranteed to be on the hull)
		int lowestIdx = -1;
		float xPos = float.MinValue;
		float yPos = float.MaxValue;
		maxX = float.MinValue;
		
		for(int i = 0, cnt = vList.Count; i < cnt; i++) {
			bool flag = false;
			if(yPos > vList[i].y) {
				flag = true;
				xPos = float.MinValue;
				yPos = vList[i].y;
			}
			else if(yPos == vList[i].y && xPos < vList[i].x) {
				flag = true;
				xPos = vList[i].x;
			}
			
			maxX = Mathf.Max(maxX, vList[i].x);
			
			if(flag) {
				lowestIdx = i;
				yPos = vList[i].y;
			}
		}
		return lowestIdx;
	}

	public void GetBoundary(out float left, out float right, out float top, out float bottom) {
		left = float.MaxValue; right = float.MinValue; top = float.MinValue; bottom = float.MaxValue;
		for(int i = 0, cnt = edges.Count; i < cnt; i++) {
			Edge edge = edges[i];
			Vector3 aPos = edge.aPos;
			Vector3 bPos = edge.bPos;

			left = Mathf.Min(aPos.x, bPos.x, left);
			right = Mathf.Max(aPos.x, bPos.x, right);
			top = Mathf.Max(aPos.y, bPos.y, top);
			bottom = Mathf.Min(aPos.y, bPos.y, bottom);
		}
	}

	public void ChangeBoundary(float left, float right, float top, float bottom) {
		float prevLeft, prevRight, prevTop, prevBottom;
		GetBoundary(out prevLeft, out prevRight, out prevTop, out prevBottom);

		float[] c = new float[4];
		c[0] = (right-left)/(prevRight-prevLeft);
		c[1] = (right+left)*0.5f-(prevRight+prevLeft)*0.5f;
		c[2] = (top-bottom)/(prevTop-prevBottom);
		c[3] = (top+bottom)*0.5f-(prevTop+prevBottom)*0.5f;

		for(int i = 0, cnt = edges.Count; i < cnt; i++) {
			Edge edge = edges[i];
			Vector3 aPos = edge.aPos, bPos = edge.bPos;

			aPos.x = c[0] * aPos.x + c[1];
			bPos.x = c[0] * bPos.x + c[1];
			aPos.y = c[2] * aPos.y + c[3];
			bPos.y = c[2] * bPos.y + c[3];

			edge.aPos = aPos;
			edge.bPos = bPos;
			edges[i] = edge;
		}
	}

	public void Draw() {
		for(int i = 0, cnt = _edges.Count; i < cnt; i++) {
			_edges[i].Draw();
		}
	}
	
	public void Draw(Color lineColor, float width) {
		for(int i = 0, cnt = _edges.Count; i < cnt; i++) {
			_edges[i].Draw(lineColor, width);
		}
	}
}
