using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Poly2Tri;

/*
 * Triangulation Context
 * based on Teddy Algorithm (http://www-ui.is.s.u-tokyo.ac.jp/~takeo/teddy/teddy.htm)
 * used Poly2Tri(C#) library (https://code.google.com/p/poly2tri)
 */
public class TriangulationContext {
	private List<DelaunayTriangle> _originalTriangles = new List<DelaunayTriangle>();
	/// <summary>
	/// Input triangles (from Polygon.Triangles)
	/// </summary>
	public List<DelaunayTriangle> originalTriangles {
		get {
			return _originalTriangles;
		}
	}
	
	private ChordalAxis _chordalAxis = new ChordalAxis();
	/// <summary>
	/// It stores chordal axis per triangles.
	/// </summary>
	public ChordalAxis chordalAxis {
		get {
			return _chordalAxis;
		}
	}

	private List<DelaunayTriangle> _finalTriangles = new List<DelaunayTriangle>();
	/// <summary>
	/// Output triangles
	/// </summary>
	public List<DelaunayTriangle> finalTriangles {
		get {
			return _finalTriangles;
		}
	}

	#region Constructors
	public TriangulationContext(Spline spline) {
		List<PolygonPoint> points = new List<PolygonPoint>();
		for(int i = 0, cnt = spline.edges.Count; i < cnt; i++) {
			Vector3 aPos = spline.edges[i].aPos;
			points.Add(new PolygonPoint(aPos.x, aPos.y));
		}
		Polygon polygon = new Polygon(points);
		P2T.Triangulate(TriangulationAlgorithm.DTSweep, polygon);

		_originalTriangles.AddRange(polygon.Triangles);
	}

	public TriangulationContext(IList<DelaunayTriangle> triangles) {
		if(triangles != null) {
			_originalTriangles.AddRange(triangles);
		}
	}
	#endregion

	public List<DelaunayTriangle> GetTriangleStripCCW() {
		return GetTriangleStrip(null, true);
	}

	public List<DelaunayTriangle> GetTriangleStripCW() {
		return GetTriangleStrip(null, false);
	}

	public List<DelaunayTriangle> GetTriangleStrip(DelaunayTriangle start, bool counterClockwise) {
		List<DelaunayTriangle> list = new List<DelaunayTriangle>(finalTriangles.Count);

		if(finalTriangles.Count > 0) {
			DelaunayTriangle current = finalTriangles[0];
			if(start != null && finalTriangles.Contains(start)) {
				current = start;
			}
			TriangulationPoint cachedPoint = new TriangulationPoint(0, 0);	// cached point
			TriangulationPoint point = null;
			for(int i = 0, cnt = finalTriangles.Count; i < cnt; i++) {
				if(current == null) break;
				list.Add(current);

				// find next triangle
				ChordalAxisInfo info = chordalAxis.chordalAxisInfoDict[current];
				
				// triangle which has single chordal axis point
				if(info.type == ChordalAxisInfo.Type.SinglePoint) {
					cachedPoint.Set(info.points[0]);
					point = (counterClockwise ? current.PointCCWFrom(cachedPoint) : current.PointCWFrom(cachedPoint));
					current = current.NeighborAcrossFrom(point);
				}
				// triangle which has two chordal axis point
				else if(info.type == ChordalAxisInfo.Type.TwoPoint) {
					int idx1 = current.IndexOf(new TriangulationPoint(info.points[0].X, info.points[0].Y));
					int idx2 = current.IndexOf(new TriangulationPoint(info.points[1].X, info.points[1].Y));
					int idx3 = 0;
					for(;idx3 < 3;idx3++) {
						if(idx3 != idx1 && idx3 != idx2) break;
					}
					
					point = (counterClockwise ? current.PointCWFrom(current.Points[idx3]) : current.PointCCWFrom(current.Points[idx3]));
					current = current.NeighborAcrossFrom(point);
				}
				else {
					break;
				}
			}
		}

		return list;
	}

	#region Triangulate (Teddy)
	public void Triangulate() {
		Triangulate (true);
	}

	public void Triangulate(bool prune) {
		BeginTriangulate();

		if(prune) {
			PruneTriangles ();
		}

		FinalTriangulation();
	}

	public void BeginTriangulate() {
		_chordalAxis.Clear();
		_finalTriangles.Clear();

		_finalTriangles.AddRange(_originalTriangles);
		foreach(DelaunayTriangle triangle in _finalTriangles) {
			_chordalAxis.AddTriangle(triangle);
		}
	}

	public void PruneTriangles() {
		HashSet<DelaunayTriangle> list = new HashSet<DelaunayTriangle>(originalTriangles);
		List<DelaunayTriangle> terminalTriangles = new List<DelaunayTriangle>();
		
		// find endpoint triangles (constrained count = 2)
		foreach(DelaunayTriangle triangle in list) {
			int constrainedCount = Util.GetTriangleConstrainedCount(triangle);
			
			if(constrainedCount == 2) {
				terminalTriangles.Add(triangle);
			}
		}
		
		// Loop!
		foreach(DelaunayTriangle terminalTriangle in terminalTriangles) {
			PerformPruneTrianglesStartAt(list, terminalTriangle);
		}

		finalTriangles.Clear();
		finalTriangles.AddRange(list);
	}

	public void PerformPruneTrianglesStartAt(HashSet<DelaunayTriangle> list, DelaunayTriangle terminalTriangle) {
		if(!list.Contains(terminalTriangle)) return;
		
		// init
		List<DelaunayTriangle> pruneTargetTriangles = new List<DelaunayTriangle>();
		LinkedList<TriangulationPoint> pruneTargetVertices = new LinkedList<TriangulationPoint>();
		
		// find terminal vertex index
		int NCIndex = Util.GetTriangleNCPointIndex(terminalTriangle);
		DelaunayTriangle currentPruningTriangle = terminalTriangle;
		
		// add to target
		pruneTargetTriangles.Add(currentPruningTriangle);
		
		// init target vertices
		pruneTargetVertices.AddLast(currentPruningTriangle.PointCWFrom(currentPruningTriangle.Points[NCIndex]));
		pruneTargetVertices.AddLast(currentPruningTriangle.Points[NCIndex]);
		pruneTargetVertices.AddLast(currentPruningTriangle.PointCCWFrom(currentPruningTriangle.Points[NCIndex]));
		
		// go!
		TriangulationPoint leftPoint = pruneTargetVertices.Last.Value;
		TriangulationPoint rightPoint = pruneTargetVertices.First.Value;
		TriangulationPoint centerPoint = new TriangulationPoint(0.5 * (leftPoint.X + rightPoint.X), 0.5 * (leftPoint.Y + rightPoint.Y));
		TriangulationPoint offset = new TriangulationPoint(0.5 * (rightPoint.X - leftPoint.X), 0.5 * (rightPoint.Y - leftPoint.Y));
		
		currentPruningTriangle = currentPruningTriangle.Neighbors[NCIndex];
		if(!list.Contains(currentPruningTriangle)) currentPruningTriangle = null;
		DelaunayTriangle oppositeTriangle = null;
		while(currentPruningTriangle != null) {
			int constrainedCount = Util.GetTriangleConstrainedCount(currentPruningTriangle);
			// sleeve triangle
			if(constrainedCount == 1) {
				pruneTargetTriangles.Add(currentPruningTriangle);
				
				if(currentPruningTriangle.GetConstrainedEdgeAcross(leftPoint)) {
					oppositeTriangle = currentPruningTriangle.NeighborAcrossFrom(rightPoint);
					rightPoint = currentPruningTriangle.PointCCWFrom(leftPoint);
					pruneTargetVertices.AddFirst(rightPoint);
				}
				else {
					oppositeTriangle = currentPruningTriangle.NeighborAcrossFrom(leftPoint);
					leftPoint = currentPruningTriangle.PointCWFrom(rightPoint);
					pruneTargetVertices.AddLast(leftPoint);
				}
				// additional variables
				centerPoint = new TriangulationPoint(0.5 * (leftPoint.X + rightPoint.X), 0.5 * (leftPoint.Y + rightPoint.Y));
				offset = new TriangulationPoint(0.5 * (rightPoint.X - leftPoint.X), 0.5 * (rightPoint.Y - leftPoint.Y));
				
				// make the circle and check whether all target vertices is in circle.
				float radius = Mathf.Sqrt(Mathf.Pow((float)offset.X, 2.0f) + Mathf.Pow((float)offset.Y, 2.0f));
				Circle circle = new Circle(Util.PointToVector3(centerPoint), radius);
				
				bool allPointInCircle = true;
				foreach(TriangulationPoint point in pruneTargetVertices) {
					if(!circle.IsIn(Util.PointToVector3(point))) {
						allPointInCircle = false;
						break;
					}
				}
				
				if(!allPointInCircle) {
					break;
				}
				currentPruningTriangle = oppositeTriangle;
			}
			// junction triangle
			else if(constrainedCount == 0) {
				if(pruneTargetTriangles.Count > 0) {
					//centerPoint = currentPruningTriangle.Centroid();
					ChordalAxisInfo info = _chordalAxis.chordalAxisInfoDict[currentPruningTriangle];
					centerPoint = new TriangulationPoint(info.points[0].X, info.points[0].Y);

					int invadedAreaIndex = currentPruningTriangle.IndexCCWFrom(leftPoint);
					info.invaded[invadedAreaIndex] = true;
				}
				break;
			}
			else {
				break;
			}
		}
		
		if(pruneTargetTriangles.Count > 0) {
			// remove triangle
			foreach(DelaunayTriangle triangle in pruneTargetTriangles) {
				list.Remove(triangle);
				_chordalAxis.RemoveTriangle(triangle);
			}
			
			// we'll make fan triangles
			var node = pruneTargetVertices.First;
			TriangulationPoint prevPoint = node.Value;
			DelaunayTriangle prevTriangle = null;
			for(int i = 1, cnt = pruneTargetVertices.Count; i < cnt; i++) {
				TriangulationPoint currentPoint = node.Next.Value;
				
				// makes new triangle
				DelaunayTriangle newTriangle = new DelaunayTriangle(currentPoint, centerPoint, prevPoint);
				newTriangle.SetConstrainedEdgeAcross(centerPoint, true);

				// make link
				if(prevTriangle != null) {
					newTriangle.MarkNeighbor(prevTriangle);
				}
				prevTriangle = newTriangle;
				
				// add to list
				list.Add(newTriangle);
				
				// add to chordal axis
				ChordalAxisInfo chordalAxisInfo = _chordalAxis.GetChordalAxisInfoForSinglePointTriangle(newTriangle, 1);
				_chordalAxis.AddTriangle(newTriangle, chordalAxisInfo);
				
				// next loop
				prevPoint = currentPoint;
				node = node.Next;
			}
		}
	}

	public void FinalTriangulation() {
		// final triangles
		List<DelaunayTriangle> final = new List<DelaunayTriangle>();

		// for re-linking (neighbor)
		Dictionary<uint, List<DelaunayTriangle>> trianglesPerEdgeDict = new Dictionary<uint, List<DelaunayTriangle>>();

		// iterate!
		for(int i = 0; i < _finalTriangles.Count; i++) {
			DelaunayTriangle triangle = _finalTriangles[i];
			ChordalAxisInfo info = _chordalAxis.chordalAxisInfoDict[triangle];

			if(info.type == ChordalAxisInfo.Type.Terminal) {
				int NCIndex = Util.GetTriangleNCPointIndex(triangle);
				
				TriangulationPoint p1 = triangle.Points[NCIndex];
				TriangulationPoint p2 = triangle.PointCCWFrom(triangle.Points[NCIndex]);
				TriangulationPoint p3 = triangle.PointCWFrom(triangle.Points[NCIndex]);
				TriangulationPoint p23 = new TriangulationPoint(info.points[1].X, info.points[1].Y);

				DelaunayTriangle t1 = new DelaunayTriangle(p23, p1, p2);
				t1.SetConstrainedEdgeAcross(p23, true);
				DelaunayTriangle t2 = new DelaunayTriangle(p23, p3, p1);
				t2.SetConstrainedEdgeAcross(p23, true);

				ChordalAxisInfo newInfo1 = _chordalAxis.GetChordalAxisInfoForSinglePointTriangle(t1, 0);
				ChordalAxisInfo newInfo2 = _chordalAxis.GetChordalAxisInfoForSinglePointTriangle(t2, 0);

				_chordalAxis.AddTriangle(t1, newInfo1);
				_chordalAxis.AddTriangle(t2, newInfo2);
				final.Add(t1);
				final.Add(t2);
			}
			else if(info.type == ChordalAxisInfo.Type.Sleeve) {
				int CIndex = Util.GetTriangleCPointIndex(triangle);
				
				TriangulationPoint p1 = triangle.Points[CIndex];
				TriangulationPoint p2 = triangle.PointCCWFrom(triangle.Points[CIndex]);
				TriangulationPoint p3 = triangle.PointCWFrom(triangle.Points[CIndex]);

				TriangulationPoint p12 = new TriangulationPoint(info.points[0].X, info.points[0].Y);
				TriangulationPoint p13 = new TriangulationPoint(info.points[1].X, info.points[1].Y);

				DelaunayTriangle t1 = new DelaunayTriangle(p1, p12, p13);
				ChordalAxisInfo newInfo1 = _chordalAxis.GetChordalAxisInfoForTwoPointTriangle(t1, 1, 2);
				DelaunayTriangle t2 = null, t3 = null;
				if((p3-p12).Magnitude() > (p2-p13).Magnitude()) {
					t2 = new DelaunayTriangle(p2, p13, p12);
					t3 = new DelaunayTriangle(p13, p2, p3);
					t3.SetConstrainedEdgeAcross(p13, true);
				}
				else {
					t2 = new DelaunayTriangle(p3, p13, p12);
					t3 = new DelaunayTriangle(p12, p2, p3);
					t3.SetConstrainedEdgeAcross(p12, true);
				}
				
				ChordalAxisInfo newInfo2 = _chordalAxis.GetChordalAxisInfoForTwoPointTriangle(t2, 1, 2);
				ChordalAxisInfo newInfo3 = _chordalAxis.GetChordalAxisInfoForSinglePointTriangle(t3, 0);
				
				_chordalAxis.AddTriangle(t1, newInfo1);
				_chordalAxis.AddTriangle(t2, newInfo2);
				_chordalAxis.AddTriangle(t3, newInfo3);
				final.Add(t1);
				final.Add(t2);
				final.Add(t3);
			}
			else if(info.type == ChordalAxisInfo.Type.Junction) {
				TriangulationPoint p1 = triangle.Points[0];
				TriangulationPoint p2 = triangle.Points[1];
				TriangulationPoint p3 = triangle.Points[2];
				TriangulationPoint centroid = new TriangulationPoint(info.points[0].X, info.points[0].Y);
				TriangulationPoint[] pa = new TriangulationPoint[] { p1, p2, p3 };

				for(int j = 0; j < 3; j++) {
					if(info.invaded[j] == false) {
						TriangulationPoint pj1 = new TriangulationPoint(info.points[j+1].X, info.points[j+1].Y);

						DelaunayTriangle t1 = new DelaunayTriangle(pj1, centroid, pa[(j+1)%3]);
						ChordalAxisInfo newInfo1 = _chordalAxis.GetChordalAxisInfoForTwoPointTriangle(t1, 0, 1);
						DelaunayTriangle t2 = new DelaunayTriangle(centroid, pj1, pa[(j+2)%3]);
						ChordalAxisInfo newInfo2 = _chordalAxis.GetChordalAxisInfoForTwoPointTriangle(t2, 0, 1);

						if(triangle.EdgeIsConstrained[j]) {
							t1.SetConstrainedEdgeAcross(centroid, true);
							t2.SetConstrainedEdgeAcross(centroid, true);
						}
						
						_chordalAxis.AddTriangle(t1, newInfo1);
						_chordalAxis.AddTriangle(t2, newInfo2);
						final.Add(t1);
						final.Add(t2);
					}
				}
			}
			else {
				continue;
			}
			
			_finalTriangles.RemoveAt(i--);
			_chordalAxis.RemoveTriangle(triangle);
		}

		for(int i = 0, cnt = final.Count; i < cnt; i++) {
			_finalTriangles.Add(final[i]);
		}

		/*
		 * Re-link ALL triangles!
		 * - mark neighbors of ALL triangle
		 * - blah blah
		 * Easy :-)
		 */
		for(int i = 0, cnt = _finalTriangles.Count; i < cnt; i++) {
			DelaunayTriangle t = _finalTriangles[i];

			TriangulationConstraint c1, c2, c3;
			c1 = new TriangulationConstraint(t.Points[0], t.Points[1]);
			c2 = new TriangulationConstraint(t.Points[1], t.Points[2]);
			c3 = new TriangulationConstraint(t.Points[2], t.Points[0]);
			
			if(!trianglesPerEdgeDict.ContainsKey(c1.ConstraintCode)) trianglesPerEdgeDict[c1.ConstraintCode] = new List<DelaunayTriangle>(2);
			if(!trianglesPerEdgeDict.ContainsKey(c2.ConstraintCode)) trianglesPerEdgeDict[c2.ConstraintCode] = new List<DelaunayTriangle>(2);
			if(!trianglesPerEdgeDict.ContainsKey(c3.ConstraintCode)) trianglesPerEdgeDict[c3.ConstraintCode] = new List<DelaunayTriangle>(2);

			if(!trianglesPerEdgeDict[c1.ConstraintCode].Contains(t)) trianglesPerEdgeDict[c1.ConstraintCode].Add(t);
			if(!trianglesPerEdgeDict[c2.ConstraintCode].Contains(t)) trianglesPerEdgeDict[c2.ConstraintCode].Add(t);
			if(!trianglesPerEdgeDict[c3.ConstraintCode].Contains(t)) trianglesPerEdgeDict[c3.ConstraintCode].Add(t);
		}

		// link all!
		foreach(uint c in trianglesPerEdgeDict.Keys) {
			List<DelaunayTriangle> list = trianglesPerEdgeDict[c];
			if(list != null && list.Count > 1) {
				try {
					list[0].MarkNeighbor(list[1]);
				}
				catch(System.Exception e) {
					continue;
				}
			}
		}
	}
	#endregion

	#region Pruning Triangles (Slow Version, for TEST) 
	public void PruneTriangles_SlowVersionBegin(out List<DelaunayTriangle> list, out List<DelaunayTriangle> terminalTriangles) {
		list = new List<DelaunayTriangle>(originalTriangles);
		terminalTriangles = new List<DelaunayTriangle>();
		
		// find endpoint triangles (constrained count = 2)
		foreach(DelaunayTriangle triangle in list) {
			int constrainedCount = Util.GetTriangleConstrainedCount(triangle);
			
			if(constrainedCount == 2) {
				terminalTriangles.Add(triangle);
			}
		}
	}

	public class PruneTriangleSlowVersionProcess {
		public List<DelaunayTriangle> pruneTargetTriangles = new List<DelaunayTriangle>();
		public LinkedList<TriangulationPoint> pruneTargetVertices = new LinkedList<TriangulationPoint>();

		public DelaunayTriangle currentPruningTriangle;
		public DelaunayTriangle oppositeTriangle;

		public TriangulationPoint leftPoint, rightPoint, centerPoint, offset;

		public PruneTriangleSlowVersionProcess(DelaunayTriangle terminalTriangle) {
			// find terminal vertex index
			int NCIndex = Util.GetTriangleNCPointIndex(terminalTriangle);
			currentPruningTriangle = terminalTriangle;
			
			// add to target
			pruneTargetTriangles.Add(currentPruningTriangle);
			
			// init target vertices
			pruneTargetVertices.AddLast(currentPruningTriangle.PointCWFrom(currentPruningTriangle.Points[NCIndex]));
			pruneTargetVertices.AddLast(currentPruningTriangle.Points[NCIndex]);
			pruneTargetVertices.AddLast(currentPruningTriangle.PointCCWFrom(currentPruningTriangle.Points[NCIndex]));
			
			// go!
			leftPoint = pruneTargetVertices.Last.Value;
			rightPoint = pruneTargetVertices.First.Value;
			centerPoint = new TriangulationPoint(0.5 * (leftPoint.X + rightPoint.X), 0.5 * (leftPoint.Y + rightPoint.Y));
			offset = new TriangulationPoint(0.5 * (rightPoint.X - leftPoint.X), 0.5 * (rightPoint.Y - leftPoint.Y));
		}
	}
	
	public void PerformPruneTriangleStartAt_SlowVersion(PruneTriangleSlowVersionProcess process, List<DelaunayTriangle> list, DelaunayTriangle terminalTriangle) {
		if(!list.Contains(terminalTriangle)) return;

		List<DelaunayTriangle> pruneTargetTriangles = process.pruneTargetTriangles;
		LinkedList<TriangulationPoint> pruneTargetVertices = process.pruneTargetVertices;

		DelaunayTriangle currentPruningTriangle = process.currentPruningTriangle;
		DelaunayTriangle oppositeTriangle = process.oppositeTriangle;

		TriangulationPoint leftPoint = process.leftPoint, rightPoint = process.rightPoint, centerPoint = process.centerPoint, offset = process.offset;

		if(currentPruningTriangle == terminalTriangle) {
			int NCIndex = Util.GetTriangleNCPointIndex(terminalTriangle);
			currentPruningTriangle = currentPruningTriangle.Neighbors[NCIndex];
			if(!list.Contains(currentPruningTriangle)) currentPruningTriangle = null;
			oppositeTriangle = null; 
		}

		if(currentPruningTriangle != null) {
			int constrainedCount = Util.GetTriangleConstrainedCount(currentPruningTriangle);
			// sleeve triangle
			if(constrainedCount == 1) {
				pruneTargetTriangles.Add(currentPruningTriangle);
				
				if(currentPruningTriangle.GetConstrainedEdgeAcross(leftPoint)) {
					oppositeTriangle = currentPruningTriangle.NeighborAcrossFrom(rightPoint);
					rightPoint = currentPruningTriangle.PointCCWFrom(leftPoint);
					pruneTargetVertices.AddFirst(rightPoint);
				}
				else {
					oppositeTriangle = currentPruningTriangle.NeighborAcrossFrom(leftPoint);
					leftPoint = currentPruningTriangle.PointCWFrom(rightPoint);
					pruneTargetVertices.AddLast(leftPoint);
				}
				// additional variables
				centerPoint = new TriangulationPoint(0.5 * (leftPoint.X + rightPoint.X), 0.5 * (leftPoint.Y + rightPoint.Y));
				offset = new TriangulationPoint(0.5 * (rightPoint.X - leftPoint.X), 0.5 * (rightPoint.Y - leftPoint.Y));
				
				// make the circle and check whether all target vertices is in circle.
				float radius = Mathf.Sqrt(Mathf.Pow((float)offset.X, 2.0f) + Mathf.Pow((float)offset.Y, 2.0f));
				Circle circle = new Circle(Util.PointToVector3(centerPoint), radius);
				
				bool allPointInCircle = true;
				foreach(TriangulationPoint point in pruneTargetVertices) {
					if(!circle.IsIn(Util.PointToVector3(point))) {
						allPointInCircle = false;
						break;
					}
				}
				
				if(!allPointInCircle) {
					currentPruningTriangle = null;
				}
				else {
					currentPruningTriangle = oppositeTriangle;
				}
			}
			// junction triangle
			else if(constrainedCount == 0) {
				if(pruneTargetTriangles.Count > 0) {
					//centerPoint = currentPruningTriangle.Centroid();
					ChordalAxisInfo info = _chordalAxis.chordalAxisInfoDict[currentPruningTriangle];
					centerPoint = new TriangulationPoint(info.points[0].X, info.points[0].Y);

					int invadedAreaIndex = currentPruningTriangle.IndexCCWFrom(leftPoint);
					info.invaded[invadedAreaIndex] = true;
				}
				currentPruningTriangle = null;
			}
			else {
				currentPruningTriangle = null;
			}
		}
		else if(pruneTargetTriangles.Count > 0) {
			// remove triangle
			foreach(DelaunayTriangle triangle in pruneTargetTriangles) {
				list.Remove(triangle);
				_chordalAxis.RemoveTriangle(triangle);
			}
			
			// we'll make fan triangles
			var node = pruneTargetVertices.First;
			TriangulationPoint prevPoint = node.Value;
			for(int i = 1, cnt = pruneTargetVertices.Count; i < cnt; i++) {
				TriangulationPoint currentPoint = node.Next.Value;
				
				// makes new triangle
				DelaunayTriangle newTriangle = new DelaunayTriangle(currentPoint, centerPoint, prevPoint);
				newTriangle.SetConstrainedEdgeAcross(centerPoint, true);
				
				// add to list
				list.Add(newTriangle);
				
				// add to chordal axis
				ChordalAxisInfo chordalAxisInfo = _chordalAxis.GetChordalAxisInfoForSinglePointTriangle(newTriangle, 1);
				_chordalAxis.AddTriangle(newTriangle, chordalAxisInfo);
				
				// next loop
				prevPoint = currentPoint;
				node = node.Next;
			}
		}

		process.currentPruningTriangle = currentPruningTriangle;
		process.oppositeTriangle = oppositeTriangle;
		process.leftPoint = leftPoint; process.rightPoint = rightPoint; process.centerPoint = centerPoint; process.offset = offset;
	}
		
	public void PruneTriangle_SlowVersionEnd(List<DelaunayTriangle> list) {
		finalTriangles.Clear();
		finalTriangles.AddRange(list);
	}
	#endregion
}