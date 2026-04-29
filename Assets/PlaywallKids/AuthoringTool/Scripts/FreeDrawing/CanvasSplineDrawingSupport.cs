using UnityEngine;
using System.Collections;
using System.Collections.Generic;




/// <summary>
/// Supporter class for drawing spline.
/// Canvas will shows spline.
/// </summary>
/// 
public class CanvasSplineDrawingSupport : MonoBehaviour {
	
	#region Public variables
	public Canvas_ canvas;
	public UISprite backgroundLineSprite;

	#endregion

	#region Properties
	private Spline _spline;
	public Spline spline {
		get {
			return _spline;
		}
	}

	private Camera _camera;
	public new Camera camera {
		get {
			if(_camera == null) {
				_camera = UICamera.mainCamera;
			}
			return _camera;
		}
	}

	private bool _wantsPaint = true;
	public bool wantsPaint {
		get {
			return _wantsPaint;
		}
		set {
			_wantsPaint = value;
			canvas.wantsPaint = value;
			backgroundLineSprite.cachedGameObject.SetActive(_wantsPaint);
			if(!_wantsPaint) {
				Clear();
			}
		}
	}

	public bool canMakeSpline {
		get {
			return _simplifiedPoints.Count > 3;
		}
	}
	#endregion

	#region Private variables
	private bool _drawing = false;
	private List<Vector3> _points = new List<Vector3>();
	private List<Vector3> _simplifiedPoints = new List<Vector3>();
	#endregion

	#region Constants
	private const float epsilon = 0.01f;
	#endregion

	public void OnEnable() {
        // if canvas variable is not initialized, find the component.
		if(canvas == null) {
			canvas = GetComponent<Canvas_>();
		}
		
		// canvas will be drawn by single touch
		canvas.supportsMultiTouch = false;
		canvas.brushSet.Get(BrushSet.kBrushNameMarker);
		// clear
		Clear ();
	}

	public void Clear() {
		_spline = null;
		_points.Clear();
		_simplifiedPoints.Clear ();
		canvas.ClearCanvas();
	}

	public void Update() {
        if (_wantsPaint) {
            // set drawing flag to false
            _drawing = false;

            // enumerate all touches and find valid touch
			for(int i = 0, touchCount = CustomInput.touchCount; i < touchCount; i++) {
                // get touch
                TouchInfo t = CustomInput.GetTouch(i);

                // touch position in canvas
                Vector2 canvasPos = canvas.screenToCanvasMatrix.MultiplyPoint3x4(t.position);

                // draw spline for touch
                if (_DrawForTouch(t, canvasPos))
                {
                    // set drawing flag to true
                    _drawing = true;

                    // draw spline only for one touch
                    break;
                }
			}

            // if drawing is finished and point list is available
			if(_drawing == false && _points.Count > 0) {
                GenerateSpline();
			}
		}
	}

    public void GenerateSpline()
    {
        // simplify spline
        SimplifySpline();

        // generate spline
        if (canMakeSpline)
        {
            _spline = Spline.Make(_simplifiedPoints.ToArray(), Spline.Method.ConcaveHull);
        }

        // clear point list
        _points.Clear();
    }

    // Draw spline for touch input.
    // Return true when succeed, return false when given position is outside of canvas region.
    protected virtual bool _DrawForTouch(TouchInfo t, Vector3 canvasPos)
    {
        // check the canvas region
        Rect rect = new Rect(0, 0, canvas.canvasSize.x, canvas.canvasSize.y);

        if (rect.Contains(canvasPos))
        {
            // when first drawing, clear canvas
            if (_points.Count == 0)
            {
                canvas.ClearCanvas();
            }

            // get world space position
            Vector3 point = canvas.CanvasToWorld(canvasPos);

            // make sure that center point of the spline should be zero
            point = point - transform.position;

            // check if the point list count is zero or whether previous and current position is near
            if (_points.Count == 0 || (_points[_points.Count - 1] - point).sqrMagnitude >= 0.0002f)
            {
                // add to list
                _points.Add(point);

                // when point list count is too large, remove the first point.
                if (_points.Count >= 1000)
                {
                    _points.RemoveAt(0);
                }
            }

            return true;
        }

        return false;
    }

    // Legacy Implementation of DrawForTouch.
    // Draws spline using Raycasting.
    [System.Obsolete("Legacy implementation. Use _DrawForTouch() instead.")]
    protected virtual void _DrawForTouch_Legacy(TouchInfo t, Vector3 canvasPos)
    {
        RaycastHit hitInfo;
        Vector3 pos = t.position;
        pos.z = 100.0f;
        Ray ray = camera.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider.name.Equals("background") || hitInfo.collider.gameObject == gameObject)
            {
                Vector3 point = hitInfo.point;
                point -= this.transform.position;
                if (_points.Count == 0 || (_points[_points.Count - 1] - point).magnitude >= 0.01f)
                {
                    _points.Add(point);
                    if (_points.Count >= 1000)
                    {
                        _points.RemoveAt(0);
                    }
                }
            }
        }
    }

	
	public void SimplifySpline() {
		List<int> indices = new List<int>();
		indices.Add(0);
		indices.Add(_points.Count-1);
		DPAlgorithm(_points, indices, 0, _points.Count-1, epsilon);
		
		indices.Sort();
		_simplifiedPoints.Clear();
		for(int i = 0, count = _points.Count; i < count; i++) {
			if(indices.BinarySearch(i) > -1) {
				_simplifiedPoints.Add(_points[i]);
			}
		}
	}
	
	/// <summary>
	/// Dugles-Peucker Algorithm
	/// </summary>
	public void DPAlgorithm(List<Vector3> _points, List<int> indices, int start, int end, float epsilon) {
		if(start + 1 >= end) return;
		
		float dx = _points[end].x - _points[start].x;
		float dy = _points[end].y - _points[start].y;
		float a = dy;
		float b = -dx;
		float c = - dy * _points[start].x + dx * _points[start].y;

		float maxLength = 0.0f; int maxLenIdx = -1;
		for(int i = start + 1; i < end - 1; i++) {
			Vector3 p = _points[i];
			float len = Mathf.Abs(a * p.x + b * p.y + c) / Mathf.Sqrt(a * a + b * b);
			if(maxLength < len) {
				maxLength = len;
				maxLenIdx = i;
			}
		}
		
		if(maxLength < epsilon || maxLenIdx < 0) {
			return;
		}
		
		indices.Add(maxLenIdx);
		
		DPAlgorithm(_points, indices, start, maxLenIdx, epsilon);
		DPAlgorithm(_points, indices, maxLenIdx, end, epsilon);
	}
}
