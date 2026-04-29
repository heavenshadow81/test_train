using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Triangulation : MonoBehaviour {
	public static Mesh Triangulate3D(Spline spline, int split) {
		return Triangulate3D(spline, split, true);
	}

	public static Mesh Triangulate3D(Spline spline, int splitCount, bool cullBack) {
		return Triangulate3D(spline, splitCount, cullBack, 0.0f, 1.0f);
	}
	
	/// <summary>
	/// Triangulates 3D object based on spline.
	/// Spline will be outline of the output.
	/// </summary>
	/// <returns>triangulated 3D object.</returns>
	/// <param name="spline">Spline.</param>
	/// <param name="splitCount">Split count.</param>
	/// <param name="cullBack">If set to <c>true</c> cull back.</param>
	public static Mesh Triangulate3D(Spline spline, int splitCount, bool cullBack, float extrude, float bevel) {
		// Error checking
		if(spline == null) {
			Debug.LogError("Triangulation.Triangulate3D() : Failed to triangulate. spline is null.");
			return null;
		}
		else if(spline.edges.Count < 3) {
			Debug.LogError("Triangulation.Triangulate3D() : Failed to triangulate. edge count is not enough.");
			return null;
		}

		// Variable checking
		if(splitCount < 0)
			splitCount = 0;

		// bevel : 0.0~1.0, 1.0 becomes sphere.
		bevel = Mathf.Min(1.0f, bevel);
		if(bevel < 1.0f) {
			splitCount++;
		}

		// new mesh
		Mesh mesh = new Mesh();

		// edge count
		int cnt = spline.edges.Count;

		// variables
		float left = float.MaxValue, right = float.MinValue, top = float.MinValue, bottom = float.MaxValue;

		// get gravity (center) point
		Vector3 gravity = _GetGravity(spline, out left, out right, out top, out bottom);

		// smallest length will be height.
		float height = Mathf.Min(Mathf.Abs(right-left) * 0.5f, Mathf.Abs(top-bottom) * 0.5f);

		// if extrude is larger than 0, use extrude instead.
		if(extrude > 0.0f) {
			height = extrude;
		}

		/**
		 * We'll generate mesh. 
		 * Front face, back face will be generated.
		 * Front and back use different UV coordinates. So that we need double spline vertices.
		 * 
		 * Vertex Count = Edge Count * (2 + Split Count * 2) + 2
		 * [+2] means Front/Back Gravity Point
		 * 
		 * Triangle Count = Edge Count * (1 + Split Count * 2) * 2
		 * If you need render front and back polygons, Make double.
		 * 
		 * */
		int vertexCount = cnt * (2 + splitCount * 2) + 2;
		int triangleCount = cnt * (1 + splitCount * 2) * 2;
		int normalCount = vertexCount;
		if(cullBack == false) {
			triangleCount *= 2;
			normalCount *= 2;
		}

		Vector3[] vertices = new Vector3[vertexCount];
		Vector3[] normals = new Vector3[vertexCount];
		Vector2[] uvs = new Vector2[vertexCount];
		int[] indices = new int[triangleCount * 3];

		/**
		 * Triangle Reference Ditionary
		 * Caches list of the triangles per vertex.
		 * */
		Dictionary<int, List<Triangle>> triangleRefDict = new Dictionary<int, List<Triangle>>();
		for(int i = 0; i < vertexCount; i++) {
			triangleRefDict[i] = new List<Triangle>();
		}
		
		// for split
		float t = 0.0f;
		float tD1 = 1.0f / (float)(splitCount + (bevel < 1.0f ? 0 : 1));
		int idx = 0;
		
		// 1. Generates Vertices
		for(int i = 0; i < cnt; i++) {
			Vector3 aPos = spline.edges[i].aPos;
			Vector3 bPos = spline.edges[i].bPos;
			int aIdx = spline.edges[i].aIndex;
			int bIdx = spline.edges[i].bIndex;

			// front face
			idx = i;
			vertices[idx] = aPos;
			uvs[idx] = new Vector2(0.5f * (aPos.x - left) / (right - left), (aPos.y - bottom) / (top - bottom));

			// back face
			idx = cnt * (1 + splitCount) + i;
			vertices[idx] = aPos;
			uvs[idx] = new Vector2(1.0f - 0.5f * (aPos.x - left) / (right - left), (aPos.y - bottom) / (top - bottom));

			// generates splitted vertices
			t = tD1;
			for(int j = 1; j <= splitCount; j++, t += tD1) {
				Vector3 splitVertex = aPos * (1.0f - t * bevel) + t * bevel * gravity;
				float sinT = Mathf.Sqrt(2.0f * t - t * t);

				// front
				splitVertex.z = Mathf.Lerp(aPos.z, gravity.z + height, sinT);
				idx = j * cnt + i;
				vertices[idx] = splitVertex;			
				uvs[idx] = new Vector2(0.5f * (splitVertex.x - left) / (right - left), (splitVertex.y - bottom) / (top - bottom));

				// back
				splitVertex.z = Mathf.Lerp(aPos.z, gravity.z - height, sinT);
				idx = (splitCount + 1 + j) * cnt + i;
				vertices[idx] = splitVertex;
				uvs[idx] = new Vector2(1.0f - 0.5f * (splitVertex.x - left) / (right - left), (splitVertex.y - bottom) / (top - bottom));
			}
		}

		// front gravity point
		vertices[vertexCount - 2] = gravity + Vector3.forward * height;
		uvs[vertexCount - 2] = new Vector2(
			0.5f * (vertices[vertexCount - 2].x - left) / (right - left), 
			(vertices[vertexCount - 2].y - bottom) / (top - bottom));

		// back gravity point
		vertices[vertexCount - 1] = gravity - Vector3.forward * height;
		uvs[vertexCount - 1] = new Vector2(
			1.0f - 0.5f * (vertices[vertexCount - 1].x - left) / (right - left),
			(vertices[vertexCount - 1].y - bottom) / (top - bottom));
			
		// 2. Generates Triangles
		int indicesIndex = 0;
		for(int i = 0; i < cnt; i++) {
			// generates splitted triangles (front side, back side)
			for(int j = 0; j < 2; j++) {
				int aIdx = i;
				int bIdx = (i+1)%cnt;
				bool back = j == 1;
				if(back) {
					aIdx += (splitCount + 1) * cnt;
					bIdx += (splitCount + 1) * cnt;
				}

				for(int k = 1; k <= splitCount; k++) {
					// next A/B index
					int nextAIdx = (k + splitCount * j + (back ? 1 : 0)) * cnt + i;
					int nextBIdx = (k + splitCount * j + (back ? 1 : 0)) * cnt + (i + 1) % cnt;
					
					// first triangle
					_PerformSplitTriangle(cnt, splitCount, vertices, indices, triangleRefDict, aIdx, bIdx, nextAIdx, false, ref indicesIndex);
					if(cullBack == false) {
						_PerformSplitTriangle(cnt, splitCount, vertices, indices, triangleRefDict, aIdx, bIdx, nextAIdx, true, ref indicesIndex);
					}

					aIdx = nextAIdx;
					
					// second triangle
					_PerformSplitTriangle(cnt, splitCount, vertices, indices, triangleRefDict, aIdx, bIdx, nextBIdx, false, ref indicesIndex);
					if(cullBack == false) {
						_PerformSplitTriangle(cnt, splitCount, vertices, indices, triangleRefDict, aIdx, bIdx, nextBIdx, true, ref indicesIndex);
					}
					
					bIdx = nextBIdx;
				}
				
				// last triangle (forward/back)
				_PerformSplitTriangle(cnt, splitCount, vertices, indices, triangleRefDict, aIdx, bIdx, vertexCount - 2 + j, false, ref indicesIndex);
				if(cullBack == false) {
					_PerformSplitTriangle(cnt, splitCount, vertices, indices, triangleRefDict, aIdx, bIdx, vertexCount - 2 + j, true, ref indicesIndex);
				}
			}
		}
		
		// get normals
		for(int i = 0; i < vertexCount; i++) {
			Vector3 dot = Vector3.zero;
			for(int j = 0; j < triangleRefDict[i].Count; j++) {
				dot += triangleRefDict[i][j].GetNormal();
			}
			dot /= triangleRefDict[i].Count;
			normals[i] = dot;
		}
				
		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.normals = normals;
		mesh.uv = uvs;
		
		return mesh;
	}
	
	private static Vector3 _GetGravity(Spline spline, out float left, out float right, out float top, out float bottom) {
		Vector3 gravity = Vector3.zero;
		
		// Get A
		float A = 0.0f;
		left = float.MaxValue; right = float.MinValue; top = float.MinValue; bottom = float.MaxValue;
		int cnt = spline.edges.Count;
		
		for(int i = 0; i < cnt; i++) {
			Vector3 aPos = spline.edges[i].aPos;
			Vector3 bPos = spline.edges[i].bPos;
			
			A += aPos.x * bPos.y - bPos.x * aPos.y;
			
			left = Mathf.Min(aPos.x, bPos.x, left);
			right = Mathf.Max(aPos.x, bPos.x, right);
			top = Mathf.Max(aPos.y, bPos.y, top);
			bottom = Mathf.Min(aPos.y, bPos.y, bottom);
		}
		
		A *= 0.5f;
		
		// Get Gravity
		for(int i = 0; i < cnt; i++) {
			Vector3 aPos = spline.edges[i].aPos;
			Vector3 bPos = spline.edges[i].bPos;
			
			gravity.x += (aPos.x * bPos.x) * (aPos.x * bPos.y - bPos.x * aPos.y);
			gravity.y += (aPos.y * bPos.y) * (aPos.x * bPos.y - bPos.x * aPos.y);
		}
		
		gravity /= 6.0f * A;
		
		gravity = Vector3.zero;
		for(int i = 0; i < cnt; i++) {
			gravity += spline.edges[i].aPos;
		}
		gravity /= (float)cnt;
		
		return gravity;
	}
	
	private static void _PerformSplitTriangle(int edgeCount, int splitCount, Vector3[] vertices, int[] indices, Dictionary<int, List<Triangle>> triangleRefDict, int aIdx, int bIdx, int cIdx, bool back, ref int indicesIndex) {		
		Triangle triangle = new Triangle(vertices[aIdx], vertices[bIdx], vertices[cIdx]);

		// unity uses left-handed winding...
		bool flag = !triangle.isCounterClockwise3D;
		if(back) {
			flag = !flag;
		}
		if(flag) {
			indices[indicesIndex++] = aIdx;
			indices[indicesIndex++] = bIdx;
		}
		else {
			triangle = new Triangle(vertices[bIdx], vertices[aIdx], vertices[cIdx]);
			indices[indicesIndex++] = bIdx;
			indices[indicesIndex++] = aIdx;
		}
		indices[indicesIndex++] = cIdx;
		
		// ref
		_RefTriangle(edgeCount, splitCount, triangleRefDict, triangle, aIdx);
		_RefTriangle(edgeCount, splitCount, triangleRefDict, triangle, bIdx);
		_RefTriangle(edgeCount, splitCount, triangleRefDict, triangle, cIdx);
	}

	private static void _RefTriangle(int edgeCount, int splitCount, Dictionary<int, List<Triangle>> triangleRefDict, Triangle triangle, int index) {
		if(!triangleRefDict.ContainsKey(index)) {
			triangleRefDict[index] = new List<Triangle>();
		}

		if(!triangleRefDict[index].Contains(triangle)) {
			triangleRefDict[index].Add(triangle);

			if(index < edgeCount) {
				_RefTriangle(edgeCount, splitCount, triangleRefDict, triangle, index + (1 + splitCount) * edgeCount);
			}
			else if(index >= (1 + splitCount) * edgeCount && index < (2 + splitCount) * edgeCount) {
				_RefTriangle(edgeCount, splitCount, triangleRefDict, triangle, index - (1 + splitCount) * edgeCount);
			}
		}
	}
}
