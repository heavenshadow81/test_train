using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Poly2Tri;

public class MeshGenerationInfo {
	public HashSet<Triangle> triangles = new HashSet<Triangle>();
	public Dictionary<uint, Vector3> vertexDict = new Dictionary<uint, Vector3>();
	public Dictionary<uint, int> vertexIndexDict = new Dictionary<uint, int>();
	public Dictionary<int, HashSet<Triangle>> triangleRefDict = new Dictionary<int, HashSet<Triangle>>();

	public int vertexCount {
		get {
			return vertexIndexDict.Count;
		}
	}

	public int GetVertexIndex(Vector3 v) {
		uint vc = v.GetID();

		if(!vertexDict.ContainsKey(vc)) {
			/*
			foreach(uint i in vertexDict.Keys) {
				if((vertexDict[i] - v).magnitude < 0.001f) {
					Debug.Log ("??? - " + vertexDict[i].GetID() + " <> " + vc + ", why??");
					Debug.Log (string.Format("vertexDict[i] : ({0},{1},{2}) , v : ({3},{4},{5})", vertexDict[i].x, vertexDict[i].y, vertexDict[i].z, v.x, v.y, v.z));
				}
			}
			*/
			vertexDict[vc] = v;
		}
		if(!vertexIndexDict.ContainsKey(vc)) {
			vertexIndexDict[vc] = vertexCount;
		}

		int index = vertexIndexDict[vc];
		return index;
	}

	public void AddTriangle(Triangle t) {
		if(!triangles.Contains(t)) {
			triangles.Add(t);

			for(int i = 0; i < 3; i++) {
				AddTriangleReference(t.vertices[i], t);
			}
		}
	}

	private void AddTriangleReference(int index, Triangle triangle) {
		HashSet<Triangle> set = null;
		if(triangleRefDict.ContainsKey(index)) {
			set = triangleRefDict[index];
		}
		else {
			set = new HashSet<Triangle>();
			triangleRefDict[index] = set;
		}

		if(!set.Contains(triangle)) {
			set.Add(triangle);
		}
	}

	private void AddTriangleReference(Vector3 v, Triangle triangle) {
		int index = GetVertexIndex(v);
		AddTriangleReference(index, triangle);
	}

	public Vector3[] GetVertices() {
		Vector3[] vertices = new Vector3[vertexCount];

		foreach(var vertexCode in vertexDict.Keys) {
			Vector3 vertex = vertexDict[vertexCode];
			int index = vertexIndexDict[vertexCode];
            try
            {
                if (index < vertices.Length)
				    vertices[index] = vertex;
			}
			catch(System.Exception e) {

                Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                Debug.Log(vertexCode);
                Debug.Log(vertexCount);
                Debug.Log(index);
                Debug.Log(vertices.Length);
                Debug.Log(vertexIndexDict.Count);

                Debug.LogError("MeshGeneration.GetVertices() : Error occured during quering vertices.");
				Debug.LogException(e);

                return null;
			}
		}
		return vertices;
	}

	public Vector3[] GetNormals() {
		Vector3[] normals = new Vector3[vertexCount];

		foreach(var vertexCode in vertexDict.Keys) {
			int index = vertexIndexDict[vertexCode];
			HashSet<Triangle> triangleRef = triangleRefDict[index];
			Vector3 normal = Vector3.zero;
			foreach(Triangle t in triangleRef) {
				normal += t.GetNormal();
			}
			normal = normal.normalized;
			try {
				normals[index] = normal;
			}
            catch (System.Exception e)
            {
                Debug.LogError("MeshGeneration.GetNormals() : Error occured during quering normals.");
				Debug.LogException(e);

                return null;
			}
		}

		return normals;
	}

	public int[] GetIndices() {
		int[] indices = new int[triangles.Count * 3];
		int vertexIndex = 0;
		foreach(Triangle t in triangles) {
			indices[vertexIndex++] = GetVertexIndex(t.vertices[0]);
			indices[vertexIndex++] = GetVertexIndex(t.vertices[1]);
			indices[vertexIndex++] = GetVertexIndex(t.vertices[2]);
		}
		return indices;
	}

	public Vector2[] GetUVs() {
		Vector2[] uvs = new Vector2[vertexDict.Count];

		Vector3 center, size;
		GetBoundary(out center, out size);

		Vector3 min = center - size * 0.5f;

		foreach(uint vertexCode in vertexDict.Keys) {
			Vector3 vertex = vertexDict[vertexCode];
			int index = vertexIndexDict[vertexCode];

			Vector2 uv = new Vector2((vertex.x - min.x) / size.x, 0.5f * (vertex.y - min.y) / size.y);
			if(vertex.z > 0.0f) {
				uv.y += 0.5f;
			}
			uvs[index] = uv;
		}

		return uvs;
	}

	public void GetBoundary(out Vector3 center, out Vector3 size) {
		center = Vector3.zero;
		size = Vector3.zero;

		float minX = float.MaxValue, maxX = float.MinValue;
		float minY = float.MaxValue, maxY = float.MinValue;
		float minZ = float.MaxValue, maxZ = float.MinValue;

		foreach(uint vertexCode in vertexDict.Keys) {
			Vector3 vertex = vertexDict[vertexCode];
			minX = Mathf.Min(minX, vertex.x);
			minY = Mathf.Min(minY, vertex.y);
			minZ = Mathf.Min(minZ, vertex.z);
			maxX = Mathf.Max(maxX, vertex.x);
			maxY = Mathf.Max(maxY, vertex.y);
			maxZ = Mathf.Max(maxZ, vertex.z);
		}
		center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, (minZ + maxZ) * 0.5f);
		size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);
	}
}

public class MeshGeneration : MonoBehaviour {
	public static MeshGenerationInfo Generate(TriangulationContext context) {
		return Generate(context, 5, 0.5f, 5, 0.0f, 1.0f);
	}

	public static MeshGenerationInfo Generate(TriangulationContext context, int splitCount) {
		return Generate(context, splitCount, 0.5f, 5, 0.0f, 1.0f);
	}

	public static MeshGenerationInfo Generate(TriangulationContext context, int splitCount, float lambda, int iterationCount) {
		return Generate(context, splitCount, lambda, iterationCount, 0.0f, 1.0f);
	}

    public static MeshGenerationInfo Generate(TriangulationContext context, int splitCount, float lambda, int iterationCount, float extrude)
    {
        return Generate(context, splitCount, lambda, iterationCount, extrude, 1.0f);
    }
	
	public static MeshGenerationInfo Generate(TriangulationContext context, int splitCount, float lambda, int iterationCount, float extrude, float bevel) {
		MeshGenerationInfo info = new MeshGenerationInfo();

		if(context != null) {
			ChordalAxis chordalAxis = context.chordalAxis;
			
			if(splitCount < 0) {
				splitCount = 0;
			}
			
			float splitCountDiv1 = 1.0f / (float)(splitCount + 1);

			//var chordalAxisPointHeightDict = GetChordalAxisPointHeightDictionary(context);
			var chordalAxisSmoothVertexDict = GetChordalAxisSmoothVertexDictionary(context, lambda, iterationCount, extrude);
			
			foreach(DelaunayTriangle triangle in chordalAxis.triangles) {
				ChordalAxisInfo chordalAxisInfo = chordalAxis.chordalAxisInfoDict[triangle];
				Vector3 a = Vector3.zero, b = Vector3.zero, c = Vector3.zero;
				
				if(chordalAxisInfo.type == ChordalAxisInfo.Type.SinglePoint) {
					TriangulationPoint point = (TriangulationPoint)chordalAxisInfo.points[0];
					int index = triangle.IndexOf(point);
					
					//c = Util.PointToVector3(point);
					//c.z = -chordalAxis.GetChordalAxisPointHeight(point);
					//c.z = chordalAxisPointHeightDict[point];
					if(chordalAxisSmoothVertexDict.ContainsKey(point)) {
						c = chordalAxisSmoothVertexDict[point];
						a = Util.PointToVector3(triangle.Points[(index+1)%3]);
						b = Util.PointToVector3(triangle.Points[(index+2)%3]);
					}
					else {
						continue;
					}
				}
				else if(chordalAxisInfo.type == ChordalAxisInfo.Type.TwoPoint) {
					int index1 = triangle.IndexOf((TriangulationPoint)chordalAxisInfo.points[0]);
					int index2 = triangle.IndexOf((TriangulationPoint)chordalAxisInfo.points[1]);
					int index3 = 0;
					for(int i = 0; i < 3; i++) {
						if(i != index1 && i != index2) {
							index3 = i;
							break;
						}
					}

					TriangulationPoint point1, point2, point3;
					point1 = triangle.Points[index1];
					point2 = triangle.Points[index2];
					point3 = triangle.Points[index3];

					if(chordalAxisSmoothVertexDict.ContainsKey(point1) && chordalAxisSmoothVertexDict.ContainsKey(point2)) {
						//a = Util.PointToVector3(triangle.Points[index1]);
						//a.z = -chordalAxis.GetChordalAxisPointHeight(triangle.Points[index1]);
						//a.z = chordalAxisPointHeightDict[triangle.Points[index1]];
						a = chordalAxisSmoothVertexDict[triangle.Points[index1]];
						//b = Util.PointToVector3(triangle.Points[index2]);
						//b.z = -chordalAxis.GetChordalAxisPointHeight(triangle.Points[index2]);
						//b.z = chordalAxisPointHeightDict[triangle.Points[index2]];
						b = chordalAxisSmoothVertexDict[triangle.Points[index2]];
						c = Util.PointToVector3(triangle.Points[index3]);
					}
					else {
						continue;
					}
				}
				
				// T
				float t = 0.0f, nextT = 0.0f;
				// Height(Z axis, 0~1)
				float hT = 0.0f, hNT = 0.0f;
				
				// Loop!
				for(int i = 0; i <= splitCount; i++) {
					/*
					nextT = Mathf.Min(1.0f, t + splitCountDiv1);
					if(chordalAxisInfo.type == ChordalAxisInfo.Type.SinglePoint) {
						hNT = Mathf.Sqrt(nextT * (2.0f - nextT));
					}
					else {
						hNT = 1.0f - Mathf.Sqrt(1.0f - nextT * nextT);
					}
					*/
					/*
					hNT = Mathf.Min(1.0f, hT + splitCountDiv1);
					if(chordalAxisInfo.type == ChordalAxisInfo.Type.SinglePoint) {
						nextT = 1.0f - Mathf.Sqrt(1.0f - hNT * hNT);
					}
					else {
						nextT = Mathf.Sqrt(hNT * (2.0f - hNT));
					}
					*/

					if(chordalAxisInfo.type == ChordalAxisInfo.Type.SinglePoint) {
						nextT = 1.0f - Mathf.Max(0.0f, Mathf.Cos(Mathf.PI * 0.5f * (i+1) * splitCountDiv1));
						hNT = Mathf.Sqrt(nextT * (2.0f - nextT));
					}
					else {
						nextT = Mathf.Min(1.0f, Mathf.Sin(Mathf.PI * 0.5f * (i+1) * splitCountDiv1));
						hNT = 1.0f - Mathf.Sqrt(1.0f - nextT * nextT);
					}
					
					Vector3 ta = a * (1.0f - t) + c * t;
					Vector3 tb = b * (1.0f - t) + c * t;
					Vector3 nextA = a * (1.0f - nextT) + c * nextT;
					Vector3 nextB = b * (1.0f - nextT) + c * nextT;
					
					ta.z = a.z * (1.0f - hT) + c.z * hT;
					tb.z = b.z * (1.0f - hT) + c.z * hT;
					nextA.z = a.z * (1.0f - hNT) + c.z * hNT;
					nextB.z = b.z * (1.0f - hNT) + c.z * hNT;
					
					if(i == splitCount) {
						_AddTriangle(info, ta, tb, c, false);
						_AddTriangle(info, new Vector3(ta.x, ta.y, -ta.z), new Vector3(tb.x, tb.y, -tb.z), new Vector3(c.x, c.y, -c.z), true);
					}
					else {
						/*
						_AddTriangle(info, ta, tb, nextA, false);
						_AddTriangle(info, tb, nextA, nextB, false);
						_AddTriangle(info, new Vector3(ta.x, ta.y, -ta.z), new Vector3(tb.x, tb.y, -tb.z), new Vector3(nextA.x, nextA.y, -nextA.z), true);
						_AddTriangle(info, new Vector3(tb.x, tb.y, -tb.z), new Vector3(nextA.x, nextA.y, -nextA.z), new Vector3(nextB.x, nextB.y, -nextB.z), true);
						*/
						Vector3 centerPoint = (ta + tb + nextA + nextB) * 0.25f;
						_AddTriangle(info, ta, tb, centerPoint, false);
						_AddTriangle(info, tb, nextB, centerPoint, false);
						_AddTriangle(info, nextB, nextA, centerPoint, false);
						_AddTriangle(info, nextA, ta, centerPoint, false);
						
						ta.z = -ta.z; tb.z = -tb.z; nextA.z = -nextA.z; nextB.z = -nextB.z; centerPoint.z = -centerPoint.z;
						
						_AddTriangle(info, ta, tb, centerPoint, true);
						_AddTriangle(info, tb, nextB, centerPoint, true);
						_AddTriangle(info, nextB, nextA, centerPoint, true);
						_AddTriangle(info, nextA, ta, centerPoint, true);
					}
					
					t = nextT;
					hT = hNT;
				}
			}
		}

		return info;
	}

	public static Mesh GetMesh(MeshGenerationInfo info) {
		Mesh mesh = new Mesh();
        try
        {
            var vertices = info.GetVertices();
            var normals = info.GetNormals();
            var indices = info.GetIndices();
            var uv = info.GetUVs();

            if (vertices != null && normals != null && indices != null && uv != null &&
                vertices.Length == normals.Length &&
                vertices.Length == uv.Length)
            {
                mesh.vertices = info.GetVertices();
                mesh.normals = info.GetNormals();
                mesh.triangles = info.GetIndices();
                mesh.uv = info.GetUVs();
            }
            else
            {
                Debug.LogError("MeshGeneration.GetMesh() : Failed to generate polygons.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("MeshGeneration.GetMesh() : Unknown error.");
            Debug.LogException(e);
        }
		return mesh;
	}

	public static Mesh GetMesh(TriangulationContext context) {
		return GetMesh(Generate(context));
	}

	public static Mesh GetMesh(TriangulationContext context, int splitCount) {
		return GetMesh (Generate (context, splitCount));
	}
	
	public static Mesh GetMesh(TriangulationContext context, int splitCount, float lambda, int iterationCount) {
		return GetMesh (Generate (context, splitCount, lambda, iterationCount));
	}
	
	public static Mesh GetMesh(TriangulationContext context, int splitCount, float lambda, int iterationCount, float extrude) {
		return GetMesh (Generate (context, splitCount, lambda, iterationCount, extrude));
	}

	public static void Smooth(MeshGenerationInfo info) {
		Smooth (info, 0.5f, 5);
	}

	public static void Smooth(MeshGenerationInfo info, float lambda, int iterationCount) {
		if(iterationCount < 1) return;
		lambda = Mathf.Clamp(lambda, -1.0f, 1.0f);
		
		for(int iteration = 0; iteration < iterationCount; iteration++) {
			Dictionary<uint, Vector3> newVertexDict = new Dictionary<uint, Vector3>();
			Dictionary<uint, int> newIndexDict = new Dictionary<uint, int>();
			
			Dictionary<uint, Vector3> adjacentVertexDict = new Dictionary<uint, Vector3>();
			Dictionary<Triangle, int> correspondTriangleVertexIndexList = new Dictionary<Triangle, int>();
			List<uint> keyList = new List<uint>(info.vertexDict.Keys);
			for(int i = 0; i < info.vertexCount; i++) {
				adjacentVertexDict.Clear();
				correspondTriangleVertexIndexList.Clear();
				
				uint vertexCode = keyList[i];
				Vector3 vertex = info.vertexDict[vertexCode];
				int index = info.vertexIndexDict[vertexCode];
				
				var triangleRef = info.triangleRefDict[index];
				
				foreach(Triangle t in triangleRef) {
					for(int idx = 0; idx < 3; idx++) {
						Vector3 v = t.GetVertex(idx);
						uint id = v.GetID();
						if(id != vertexCode) {
							adjacentVertexDict[id] = v;
						}
						else {
							correspondTriangleVertexIndexList[t] = idx;
						}
					}
				}
				
				Vector3 sumOfVertices = Vector3.zero;
				float sumOfWeights = 0.00001f;
				foreach(var id in adjacentVertexDict.Keys) {
					Vector3 p = adjacentVertexDict[id];
					float weight = 1.0f / (p - vertex).magnitude;
					sumOfWeights += weight;
					sumOfVertices += weight * (adjacentVertexDict[id] - vertex);
				}
				vertex = lambda * sumOfVertices / sumOfWeights + vertex;
				vertexCode = vertex.GetID();
				newVertexDict[vertexCode] = vertex;
				newIndexDict[vertexCode] = index;
				foreach(Triangle t in correspondTriangleVertexIndexList.Keys) {
					t.vertices[correspondTriangleVertexIndexList[t]] = vertex;
				}
			}
			
			info.vertexDict.Clear();
			info.vertexIndexDict.Clear();

			foreach(uint vertexCode in newVertexDict.Keys) {
				info.vertexDict[vertexCode] = newVertexDict[vertexCode];
				info.vertexIndexDict[vertexCode] = newIndexDict[vertexCode];
			}
		}
	}

	public static Mesh GetSmoothMesh(Mesh mesh) {
		return GetSmoothMesh(mesh, 0.5f, 5);
	}

	public static Mesh GetSmoothMesh(Mesh mesh, float lambda, int iterationCount) {
		Vector3[] vertices = mesh.vertices;
		int[] indices = mesh.triangles;
		int triangleCount = indices.Length / 3;

		if(triangleCount < 2) return mesh;

		MeshGenerationInfo info = new MeshGenerationInfo();

		for(int i = 0; i < triangleCount; i++) {
			int aIdx = indices[i*3];
			int bIdx = indices[i*3+1];
			int cIdx = indices[i*3+2];
			Triangle triangle = new Triangle(vertices[aIdx], vertices[bIdx], vertices[cIdx]);
			info.AddTriangle(triangle);
		}

		Mesh output = GetSmoothMesh(info, lambda, iterationCount);

		if(output == null) {
			output = mesh;
		}

		return output;
	}

	public static Mesh GetSmoothMesh(MeshGenerationInfo info, float lambda, int iterationCount) {
		if(info == null) return null;

		Smooth (info, lambda, iterationCount);
		return GetMesh(info);
	}

	private static void _AddTriangle(MeshGenerationInfo info, Vector3 a, Vector3 b, Vector3 c, bool back) {
		Triangle t = new Triangle(a, c, b);
		if(!back && t.isCounterClockwise) {
			t = new Triangle(a, b, c);
		}
		else if(back && !t.isCounterClockwise) {
			t = new Triangle(a, b, c);
		}

		info.AddTriangle(t);
	}

	public static Dictionary<Point2D, float> GetChordalAxisPointHeightDictionary(TriangulationContext context) {
		List<DelaunayTriangle> triangleStrip = context.GetTriangleStripCCW();
		List<Point2D> chordalAxisPoints = new List<Point2D>();
		List<float> chordalAxisHeights = new List<float>();
		Dictionary<Point2D, float> chordalAxisPointHeightDict = new Dictionary<Point2D, float>();

		foreach(DelaunayTriangle triangle in triangleStrip) {
			ChordalAxisInfo chordalAxisInfo = context.chordalAxis.chordalAxisInfoDict[triangle];
			if(chordalAxisInfo.type == ChordalAxisInfo.Type.SinglePoint) {
				if(chordalAxisPoints.Count == 0 || !chordalAxisPoints[chordalAxisPoints.Count-1].Equals(chordalAxisInfo.points[0], 0.00001f)) {
					chordalAxisPoints.Add(chordalAxisInfo.points[0]);
				}
			}
			else if(chordalAxisInfo.type == ChordalAxisInfo.Type.TwoPoint) {
				if(chordalAxisPoints.Count == 0 || !chordalAxisPoints[chordalAxisPoints.Count-1].Equals(chordalAxisInfo.points[0], 0.00001f)) {
					chordalAxisPoints.Add(chordalAxisInfo.points[0]);
				}
				if(chordalAxisPoints.Count == 0 || !chordalAxisPoints[chordalAxisPoints.Count-1].Equals(chordalAxisInfo.points[1], 0.00001f)) {
					chordalAxisPoints.Add(chordalAxisInfo.points[1]);
				}
			}
		}

		foreach(Point2D p in chordalAxisPoints) {
			chordalAxisHeights.Add(-context.chordalAxis.GetChordalAxisPointHeight(p));
		}
		
		Util.LaplacianSmooth(chordalAxisHeights, 0.5f, 5, true);
		
		for(int i = 0; i < chordalAxisPoints.Count; i++) {
			Point2D p = chordalAxisPoints[i];
			int count = 1;
			for(int j = i+1; j < chordalAxisPoints.Count; j++) {
				Point2D q = chordalAxisPoints[j];
				if(p.Equals(q)) {
					chordalAxisHeights[i] += chordalAxisHeights[j];
					count++;
				}
			}
			chordalAxisHeights[i] /= (float)count;
		}
		
		for(int i = 0; i < chordalAxisPoints.Count; i++) {
			Point2D p = chordalAxisPoints[i];
			if(!chordalAxisPointHeightDict.ContainsKey(p)) {
				chordalAxisPointHeightDict[p] = chordalAxisHeights[i];
			}
		}

		return chordalAxisPointHeightDict;
	}
	
	public static Dictionary<Point2D, Vector3> GetChordalAxisSmoothVertexDictionary(TriangulationContext context, float lambda, int iterationCount, float extrude) {
		List<DelaunayTriangle> triangleStrip = context.GetTriangleStripCCW();
		List<Point2D> chordalAxisPoints = new List<Point2D>();
		List<Vector3> chordalAxisSmoothVertex = new List<Vector3>();
		Dictionary<Point2D, Vector3> chordalAxisSmoothVertexDict = new Dictionary<Point2D, Vector3>();
		
		float heightRatio = 1.0f;
		if(extrude > 0.0f) {
			heightRatio = extrude / context.chordalAxis.GetChordalAxisPointMaxHeight();
		}
		
		foreach(DelaunayTriangle triangle in triangleStrip) {
			ChordalAxisInfo chordalAxisInfo = context.chordalAxis.chordalAxisInfoDict[triangle];
			if(chordalAxisInfo.type == ChordalAxisInfo.Type.SinglePoint) {
				if(chordalAxisPoints.Count == 0 || !chordalAxisPoints[chordalAxisPoints.Count-1].Equals(chordalAxisInfo.points[0], 0.00001f)) {
					chordalAxisPoints.Add(chordalAxisInfo.points[0]);
				}
			}
			else if(chordalAxisInfo.type == ChordalAxisInfo.Type.TwoPoint) {
				if(chordalAxisPoints.Count == 0 || !chordalAxisPoints[chordalAxisPoints.Count-1].Equals(chordalAxisInfo.points[0], 0.00001f)) {
					chordalAxisPoints.Add(chordalAxisInfo.points[0]);
				}
				if(chordalAxisPoints.Count == 0 || !chordalAxisPoints[chordalAxisPoints.Count-1].Equals(chordalAxisInfo.points[1], 0.00001f)) {
					chordalAxisPoints.Add(chordalAxisInfo.points[1]);
				}
			}
		}
		
		foreach(Point2D p in chordalAxisPoints) {
			Vector3 v = Util.PointToVector3(p);
			v.z = -context.chordalAxis.GetChordalAxisPointHeight(p) * heightRatio;
			chordalAxisSmoothVertex.Add(v);
		}
		
		Util.LaplacianSmooth(chordalAxisSmoothVertex, lambda, iterationCount, true);
		
		for(int i = 0; i < chordalAxisPoints.Count; i++) {
			Point2D p = chordalAxisPoints[i];
			int count = 1;
			for(int j = i+1; j < chordalAxisPoints.Count; j++) {
				Point2D q = chordalAxisPoints[j];
				if(p.Equals(q)) {
					chordalAxisSmoothVertex[i] += chordalAxisSmoothVertex[j];
					count++;
				}
			}
			chordalAxisSmoothVertex[i] /= (float)count;
		}
		
		for(int i = 0; i < chordalAxisPoints.Count; i++) {
			Point2D p = chordalAxisPoints[i];
			if(!chordalAxisSmoothVertexDict.ContainsKey(p)) {
				chordalAxisSmoothVertexDict[p] = chordalAxisSmoothVertex[i];
			}
		}
		
		return chordalAxisSmoothVertexDict;
	}
}
