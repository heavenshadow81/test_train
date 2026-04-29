using UnityEngine;
using System.Collections;

/// <summary>
/// A debugging script of Canvas spline drawing support.
/// It represents points or simplified points, and generated spline.
/// </summary>
public class CanvasSplineDrawingSupportDebug : MonoBehaviour {
	public CanvasSplineDrawingSupport canvasDrawing;
	public SimpleDraw2D draw2D;

	// Use this for initialization
	void Start () {
		if(canvasDrawing == null) {
			canvasDrawing = GetComponent<CanvasSplineDrawingSupport>();
		}
	}

	void OnGUI() {
		if(canvasDrawing != null) {
			Spline spline = canvasDrawing.spline;
			if(spline != null) 
			{
				foreach(Edge e in spline.edges) {
					Drawing.DrawLine(e.aPos, e.bPos, Color.blue, 1.0f, true);
				}
			}
		}
	}
}
