using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UITexture))]
public class Canvas_ : MonoBehaviour
{
    public enum DrawMode
    {
        NONE, COLLISION, IGNORE_COLLISION
    }


	#region Properties
    private DrawMode _drawMode = DrawMode.NONE;
    public DrawMode drawMode
    {
        get
        {
            if (_drawMode == DrawMode.NONE)
                _drawMode = DrawMode.IGNORE_COLLISION;
            return _drawMode;
        }
        set
        {
            _drawMode = value;
        }
    }

    // UI Texture
    private UITexture _uiTexture;
    public UITexture uiTexture
    {
        get
        {
            if (_uiTexture == null)
            {
                _uiTexture = GetComponent<UITexture>();
                if (_uiTexture != null)
                {
                    _uiTexture.color = Color.white;
                    _uiTexture.pivot = UIWidget.Pivot.Center;
                }
            }
            return _uiTexture;
        }
    }

	// Texture Size
    private Vector2 _textureSize = new Vector2(512, 512);
	public Vector2 textureSize {
		get {
			return _textureSize;
		}
        set
        {
            value.x = Mathf.Clamp(value.x, 256, 4096);
            value.y = Mathf.Clamp(value.y, 256, 4096);
            
            _textureSize = value;

            // Reset the canvas texture if need.
            _PrepareCanvasTexture();

            // Reset dual brush buffer
            _dualBrushBuffer = null;
            _dualBrushBufferColors = null;
            if (_dualBrushBufferTexture != null) Destroy(_dualBrushBufferTexture);
            _dualBrushBufferTexture = null;

            // Reset alpha buffer
            _alphaBuffer = null;
            _alphaBufferColors = null;
            if (_alphaBufferTexture != null) Destroy(_alphaBufferTexture);
            _alphaBufferTexture = null;

            // Reset temporary layer
            _temporaryLayer = null;
            if (_temporaryLayerTexture != null) Destroy(_temporaryLayerTexture);
            _temporaryLayerTexture = null;

            _UpdateBoxCollider();
        }
	}

	public int bufferSize {
		get {
            return Mathf.FloorToInt(textureSize.x) * Mathf.FloorToInt(textureSize.y);
		}
	}

	public Vector2 canvasSize {
		get {
			return uiTexture.localSize;
		}
	}

	private Texture _texture = null;
	public Texture texture {
		get {
            _PrepareCanvasTexture();

			return _texture;
		}
	}

	private Color[] _textureColors = null;
	public Color[] textureColors {
		get {
			if(_textureColors == null) {
				_textureColors = new Color[bufferSize];
			}
			return _textureColors;
		}
	}

    
    private BoxCollider _boxCollider = null;
	public BoxCollider boxCollider {
        get {
            if (_boxCollider == null)
            {
                _boxCollider = gameObject.GetComponent<BoxCollider>();
                if(_boxCollider == null)
                    _boxCollider = gameObject.AddComponent<BoxCollider>();
                _boxCollider.isTrigger = true;
            }

            return _boxCollider;
        }
    }
            

	private float[] _alphaBuffer = null;
	public float[] alphaBuffer {
		get {
			if(_alphaBuffer == null) {
				_alphaBuffer = new float[bufferSize];
			}
			return _alphaBuffer;
		}
	}

	private Color[] _temporaryLayer = null;
	public Color[] temporaryLayer {
		get {
			if(_temporaryLayer == null) {
				_temporaryLayer = new Color[bufferSize];
			}
			return _temporaryLayer;
		}
	}

	private float[] _dualBrushBuffer = null;
	public float[] dualBrushBuffer {
		get {
			if(_dualBrushBuffer == null) {
				_dualBrushBuffer = new float[bufferSize];
			}
			return _dualBrushBuffer;
		}
	}

	private Brush _brush = null;
	public Brush brush {
		get {
			if(_brush == null) {
				_brush = brushSet.Get (BrushSet.kBrushNameMarker);
				_brush.color = Color.red;
			}
			return _brush;
		}
		set {
			_brush = value;
		}
	}

	private BrushSet _brushSet;
	public BrushSet brushSet {
		get {
			if(_brushSet == null) {
				_brushSet = new BrushSet();
			}
			return _brushSet;
		}
		set {
			if(value == null) {
				value = new BrushSet();
			}
			_brushSet = value;
		}
	}

	private bool _wantsPaint = true;
	public bool wantsPaint {
		get {
			return _wantsPaint;
		}
		set {
			_wantsPaint = value;
		}
	}

	private bool _supportsMultiTouch = true;
	public bool supportsMultiTouch {
		get {
			return _supportsMultiTouch;
		}
		set {
			_supportsMultiTouch = value;
		}
	}
	
	// Screen->Texture Matrix
	private Matrix4x4 _screenToCanvasMatrix;
	public Matrix4x4 screenToCanvasMatrix {
		get {
			// size of the texture
			Vector2 size = canvasSize;

            // relative scale of widget
            Vector2 widgetLocalScale = uiTexture.cachedTransform.lossyScale;
            Vector2 uiRootScale = UIRoot.list[0].transform.lossyScale;
            widgetLocalScale.x /= uiRootScale.x;
            widgetLocalScale.y /= uiRootScale.y;

            // active height
            float aspectRatio = Screen.width / (float)Screen.height;
			float activeHeight = UIRoot.list[0].activeHeight;
			float activeWidth = aspectRatio * activeHeight;
			float widthRatio = activeWidth / Screen.width;
			float heightRatio = activeHeight / Screen.height;

			// pos
            Vector2 pos = Vector2.zero;
            if (camera != null)
                pos = camera.WorldToViewportPoint(_uiTexture.cachedTransform.position);

            // get!
            Vector2 T = new Vector2(
                size.x * 0.5f - activeWidth * pos.x / widgetLocalScale.x,
                size.y * 0.5f - activeHeight * pos.y / widgetLocalScale.y);
            Quaternion R = Quaternion.identity;
            Vector2 S = new Vector2(widthRatio / widgetLocalScale.x, heightRatio / widgetLocalScale.y);
            _screenToCanvasMatrix = Matrix4x4.TRS (T, R, S);

			return _screenToCanvasMatrix;
		}
	}

	private Matrix4x4 _canvasToTextureMatrix;
	public Matrix4x4 canvasToTextureMatrix {
		get {
			_canvasToTextureMatrix = Matrix4x4.TRS (
				Vector3.zero,
				Quaternion.identity,
				new Vector3 (textureSize.x / canvasSize.x, textureSize.y / canvasSize.y, 1.0f));
			
			return _canvasToTextureMatrix;
		}
	}

    private Texture2D _alphaBufferTexture = null;
    private Color[] _alphaBufferColors = null;
    public Texture2D alphaBufferTexture
    {
        get
        {
            if(_alphaBufferTexture == null) {
                _alphaBufferTexture = new Texture2D(Mathf.FloorToInt(textureSize.x),
                                        Mathf.FloorToInt(textureSize.y),
                                        TextureFormat.RGB24,
                                        false,
                                        true);
            }

            if(_alphaBufferColors == null) {
                _alphaBufferColors = new Color[bufferSize];
            }

            for (int i = 0, cnt = alphaBuffer.Length; i < cnt; i++)
            {
                Color c = Color.clear;

                int y = i / Mathf.FloorToInt(textureSize.y);
                int x = i % Mathf.FloorToInt(textureSize.x);

                c.b = _alphaBuffer[i];

                bool drawsOutline = false;
                int outlineWidth = 2;

                if (((x >= (int)_alphaBufferBounds.min.x - outlineWidth && x <= _alphaBufferBounds.min.x + outlineWidth) ||
                    (x >= (int)_alphaBufferBounds.max.x - outlineWidth && x <= _alphaBufferBounds.max.x + outlineWidth)) && 
                    y >= _alphaBufferBounds.min.y - outlineWidth && y <= _alphaBufferBounds.max.y + outlineWidth)
                {
                    drawsOutline = true;
                }
                else if (((y >= (int)_alphaBufferBounds.min.y - outlineWidth && y <= _alphaBufferBounds.min.y + outlineWidth) ||
                    (y >= (int)_alphaBufferBounds.max.y - outlineWidth && y <= _alphaBufferBounds.max.y + outlineWidth)) && 
                    x >= _alphaBufferBounds.min.x - outlineWidth && x <= _alphaBufferBounds.max.x + outlineWidth)
                {
                    drawsOutline = true;
                }

                if (drawsOutline)
                {
                    c = Color.yellow;
                }

                _alphaBufferColors[i] = c;
            }
            _alphaBufferTexture.SetPixels(_alphaBufferColors);
            _alphaBufferTexture.Apply();

            return _alphaBufferTexture;
        }
    }

    private Texture2D _dualBrushBufferTexture = null;
    private Color[] _dualBrushBufferColors = null;
    public Texture2D dualBrushBufferTexture
    {
        get
        {
            if (_dualBrushBufferTexture == null)
            {
                _dualBrushBufferTexture = new Texture2D(Mathf.FloorToInt(textureSize.x),
                                        Mathf.FloorToInt(textureSize.y),
                                        TextureFormat.Alpha8,
                                        false,
                                        true);
            }

            if (_dualBrushBufferColors == null)
            {
                _dualBrushBufferColors = new Color[bufferSize];
            }

            for (int i = 0, cnt = dualBrushBuffer.Length; i < cnt; i++)
            {
                _dualBrushBufferColors[i].a = _dualBrushBuffer[i];
            }
            _dualBrushBufferTexture.SetPixels(_dualBrushBufferColors);
            _dualBrushBufferTexture.Apply();

            return _dualBrushBufferTexture;
        }
    }

    private Texture2D _temporaryLayerTexture = null;
    public Texture2D temporaryLayerTexture
    {
        get
        {
            if (_temporaryLayerTexture == null)
            {
                _temporaryLayerTexture = new Texture2D(Mathf.FloorToInt(textureSize.x),
                                        Mathf.FloorToInt(textureSize.y),
                                        TextureFormat.ARGB32,
                                        false,
                                        true);
            }

            _temporaryLayerTexture.SetPixels(temporaryLayer);
            _temporaryLayerTexture.Apply();

            return _temporaryLayerTexture;
        }
    }

    private static bool _prefersComputeShaders = true;
    public static bool prefersComputeShaders
    {
        get
        {
            if (SystemInfo.supportsComputeShaders)
            {
                return _prefersComputeShaders;
            }
            else
            {
                Debug.Log("This device doesn't support GPGPU acceleration. (Needs any NVIDIA or AMD GPU device)");
            }

            return false;
        }
        set
        {
            _prefersComputeShaders = value;

            if (supportsComputeShaders)
            {
                Debug.Log("Canvas.prefersComputeShaders : GPGPU mode is enabled.");
            }
        }
    }

    public static bool supportsComputeShaders
    {
        get
        {
            return prefersComputeShaders && SystemInfo.supportsComputeShaders;
        }
    }

    private static ComputeShader _computeShader;
    public static ComputeShader computeShader
    {
        get
        {
            if (_computeShader == null)
            {
                _computeShader = (ComputeShader)Resources.Load("ComputeShaders/CanvasDrawing");
            }
            return _computeShader;
        }
    }
	#endregion

	#region Private Properties	
	// Camera
	private Camera _camera;
	private new Camera camera {
		get {
			if(_camera == null) {
				_camera = UICamera.currentCamera;
                if (_camera == null)
                    _camera = UICamera.list[0].cachedCamera;
			}
			return _camera;
		}
	}
	#endregion

	#region Private variables
	// Brushes for touches
	private Dictionary<int, Brush> _brushForTouchDict = new Dictionary<int, Brush>();
	public Dictionary<int, Brush> brushForTouchDict{
		get
		{
			if(_brushForTouchDict == null)
			{
				_brushForTouchDict = new Dictionary<int, Brush>();
			}
			return _brushForTouchDict;
		}
	}

	// Alpha buffer
	private Bounds _alphaBufferBounds = new Bounds();
	private bool _isAlphaBufferEmpty = true;

	// Dual Brush Buffer
	private Bounds _dualBrushBufferBounds = new Bounds();
	private bool _isDualBrushBufferEmpty = true;
	#endregion

	public virtual void Start() {
        if (uiTexture != null)
        {
            uiTexture.mainTexture = texture;
        }
	}

    private void _PrepareCanvasTexture()
    {
        Texture newTexture = null;
        bool createdFirstTexture = false;

        // Creates the new texture if the texture is null or not valid.
        if(_texture == null ||
            (supportsComputeShaders && _texture.GetType().Equals(typeof(Texture2D))) ||
            (!supportsComputeShaders && _texture.GetType().Equals(typeof(RenderTexture))) ||
            _texture.width != Mathf.RoundToInt(_textureSize.x) ||
            _texture.height != Mathf.RoundToInt(_textureSize.y))
        {
            int newWidth = Mathf.RoundToInt(textureSize.x);
            int newHeight = Mathf.RoundToInt(textureSize.y);
            createdFirstTexture = _texture == null;

            if (supportsComputeShaders)
            {
                // Creates the new RenderTexture.
                RenderTexture tex = new RenderTexture(newWidth, newHeight, 0, RenderTextureFormat.ARGB32);
                tex.enableRandomWrite = true;
                tex.Create();

                if (_texture != null)
                {
                    // Copy image from current texture
                    Graphics.Blit(_texture, tex);
                }

                newTexture = tex;
            }
            else
            {
                // Creates the new Texture2D.
                Texture2D tex = new Texture2D(newWidth, newHeight, TextureFormat.ARGB32, false, true);
                if (_texture != null)
                {
                    // Creates the dummy RenderTexture for copying from current texture.
                    RenderTexture dummy = RenderTexture.GetTemporary(newWidth, newHeight, 0, RenderTextureFormat.ARGB32);

                    Graphics.Blit(_texture, dummy);

                    RenderTexture prev = RenderTexture.active;
                    RenderTexture.active = dummy;
                    tex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
                    tex.Apply();
                    RenderTexture.active = prev;

                    // Release the temporary texture.
                    RenderTexture.ReleaseTemporary(dummy);
                }

                newTexture = tex;
            }
        }

        if (newTexture != null)
        {
            // Destroy current texture
            if (_texture != null)
            {
                if (_texture.GetType().Equals(typeof(RenderTexture)))
                    ((RenderTexture)_texture).Release();
                Destroy(_texture);
                _texture = null;
            }

            // Set the new one
            _texture = newTexture;
            uiTexture.mainTexture = _texture;

            // Updates the texture colors
            if (_texture.GetType().Equals(typeof(Texture2D)))
            {
                _textureColors = ((Texture2D)_texture).GetPixels();
            }
        }

        if (createdFirstTexture)
        {
            ClearCanvas();
        }
    }
    //콜라이더 크기 조절
    private void _UpdateBoxCollider()
    {
        Vector2 size = uiTexture.localSize;
        boxCollider.size = new Vector3(size.x, size.y, 0);
    }
    //
	public virtual void Update ()
	{
        // prepare the texture
        _PrepareCanvasTexture();

        // size of the texture
        Vector2 size = uiTexture.localSize;

        // canvas rect
        Rect rect = new Rect(0, 0, size.x, size.y);

        // Set boxCollider Size
        _UpdateBoxCollider();


        // draws only if wantsPaint flag is on.
		if(wantsPaint) {
            // iterate all touches
            TouchInfo[] touches = CustomInput.touches;
			for(int i = 0, touchCount = CustomInput.touchCount; i < touchCount; i++) {
				TouchInfo t = touches[i];

                bool checkDraw = false;
                switch (drawMode)
                {
                    case DrawMode.COLLISION:
                        Ray ray = camera.ScreenPointToRay(t.position);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit))
                            if (hit.collider.Equals(boxCollider))
                                checkDraw = true;
                        break;

                    case DrawMode.IGNORE_COLLISION:
                        Vector2 canvasPos = screenToCanvasMatrix * new Vector4(t.axisX, t.axisY, 0, 1.0f);

                        // check whether touch position is on the canvas region.
                        if (rect.Contains(canvasPos))
                            checkDraw = true;
                        break;

                    default:    Debug.LogWarning("None Select DrawMode");    break;
                }

                if (checkDraw)
                {
                    // draw
                    _DrawForTouch(t, ScreenToCanvas(t.position));

                    // if single touch mode, break loop.
                    if (!_supportsMultiTouch)
                    {
                        break;
                    }
                }
			}

            // upload to texture
            if (touches.Length > 0)
                Flush();
		}
	}

    public void Flush()
    {
        if (texture.GetType().Equals(typeof(Texture2D)))
            ((Texture2D)texture).Apply();
    }

	public void DrawForTouch(TouchInfo t)
	{
		_DrawForTouch(t, ScreenToCanvas(t.position));
    }

	public void DrawForTouch(TouchInfo t, Vector3 canvasPos)
	{
		_DrawForTouch(t, canvasPos);
	}

    protected virtual void _DrawForTouch(TouchInfo t, Vector3 pos)
    {
        // get touch id. if single touch mode, touch id will be set to 1.
        int touchId = t.id;
        if (!_supportsMultiTouch)
        {
            touchId = 1;
        }

        // get brush for touch
        Brush b = null;
        if (!_brushForTouchDict.ContainsKey(touchId))
        {
            b = (Brush)brush.Clone();

            _brushForTouchDict[touchId] = b;
        }
        else
        {
            b = _brushForTouchDict[touchId];
        }
        
        // check phase
        switch (t.phase)
        {
            case TouchInfo.Phase.Begin:
                
                b.StartPaint(this, pos);
                break;
            case TouchInfo.Phase.Move:
                b.Paint(this, pos);
                break;
            case TouchInfo.Phase.Cancel:
            case TouchInfo.Phase.End:
                b.EndPaint(this);

				// remove the brush object after the touch is finished
				_brushForTouchDict.Remove(touchId);

                // Debug.Log("Removed brush for touch " + touchId);
                break;
        }
    }

	public virtual void OnDestroy() {
        if (_texture != null)
        {
            if (_texture.GetType().Equals(typeof(RenderTexture)))
                ((RenderTexture)_texture).Release();
            DestroyImmediate(_texture);
            _texture = null;
        }

        if (_alphaBufferTexture != null)
        {
            Destroy(_alphaBufferTexture);
            _alphaBufferTexture = null;
        }

        if (_dualBrushBufferTexture != null)
        {
            Destroy(_dualBrushBufferTexture);
            _dualBrushBufferTexture = null;
        }

        if (_temporaryLayerTexture != null)
        {
            Destroy(_temporaryLayerTexture);
            _temporaryLayerTexture = null;
        }
	}

    public bool IsDrawing(int touchID)
    {
        if (_brushForTouchDict.ContainsKey(touchID))
            return _brushForTouchDict[touchID].isDrawing;
        
        //Debug.LogWarning("Failed Find Brush");
        return false;
    }

    public Color GetPixel(int x, int y)
    {
        x = Mathf.Clamp(x, 0, Mathf.FloorToInt(textureSize.x) - 1);
        y = Mathf.Clamp(y, 0, Mathf.FloorToInt(textureSize.y) - 1);

        return textureColors[y * Mathf.FloorToInt(textureSize.x) + x];
    }

	public virtual void ClearCanvas ()
    {
        _PerformFill(Color.clear);
	}

    public virtual void FillColor(Color color)
    {
        _PerformFill(color);
    }

    private void _PerformFill(Color color)
    {
        if (supportsComputeShaders)
        {
            if (computeShader != null)
            {
                int kernel = computeShader.FindKernel("CanvasDrawingFill");
                if (kernel != -1)
                {
                    int numThreads = 16;
                    computeShader.SetFloats("clearColor", color.r, color.g, color.b, color.a);
                    computeShader.SetTexture(kernel, "output", texture);
                    computeShader.Dispatch(kernel, Mathf.RoundToInt(textureSize.x / numThreads), Mathf.RoundToInt(textureSize.y / numThreads), 1);
                }
            }
        }
        else
        {
            int width = Mathf.FloorToInt(textureSize.x);
            int height = Mathf.FloorToInt(textureSize.y);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    textureColors[y * width + x] = color;
                }
            }

            ((Texture2D)texture).SetPixels(textureColors);
            ((Texture2D)texture).Apply();
        }
    }
    
    public void FillRect(Color color, float offsetX, float offsetY, float width, float height)
    {
        if (supportsComputeShaders)
        {
            if (computeShader != null)
            {
                int kernel = computeShader.FindKernel("CanvasDrawingOneLine");
                if (kernel != -1)
                {
                    int numThreads = 16;
                    computeShader.SetFloats("offset", offsetX, offsetY);
                    computeShader.SetFloats("clearColor", color.r, color.g, color.b, color.a);
                    computeShader.SetTexture(kernel, "output", texture);
                    computeShader.Dispatch(kernel, Mathf.RoundToInt(width / numThreads), Mathf.RoundToInt(height / numThreads), 1);
                }
            }
        }
        else
        {
            /*
            int width = Mathf.FloorToInt(textureSize.x);
            int height = Mathf.FloorToInt(textureSize.y);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    textureColors[y * width + x] = color;
                }
            }

            ((Texture2D)texture).SetPixels(textureColors);
            ((Texture2D)texture).Apply();
            */
        }
    }
	
	public Vector3 ScreenToCanvas (Vector3 pos)
	{
		return screenToCanvasMatrix.MultiplyPoint3x4 (pos);
	}
	
	public Vector3 CanvasToTexture (Vector3 pos)
	{
		return canvasToTextureMatrix.MultiplyPoint3x4 (pos);
	}
	
	public Matrix4x4 GetCanvasToTextureMatrix ()
	{
		return canvasToTextureMatrix;
	}
	
	public Vector3 TextureToCanvas (Vector3 pos)
	{
		return canvasToTextureMatrix.inverse.MultiplyPoint3x4 (pos);
	}
	
	public Matrix4x4 GetTextureToCanvasMatrix ()
	{
		return canvasToTextureMatrix.inverse;
	}

    public Vector3 CanvasToWorld(Vector3 pos)
    {
        // convert vector3 to vector2
        Vector2 offsetFromPivotPoint = canvasSize;

        // get offset of canvas pivot
        offsetFromPivotPoint.Scale(uiTexture.pivotOffset);

        // get adjusted position
        Vector3 adjustedPosition = pos - new Vector3(offsetFromPivotPoint.x, offsetFromPivotPoint.y);

        // make sure that z position is always zero
        adjustedPosition.z = 0.0f;

        // multiply by UITexture's local to world matrix
        adjustedPosition = transform.localToWorldMatrix.MultiplyPoint3x4(adjustedPosition);

        // return
        return adjustedPosition;
    }

	public virtual void ClearAlphaBuffer () {
		_alphaBufferBounds.SetMinMax (
			BrushUtils.Max (_alphaBufferBounds.min, Vector3.zero),
			BrushUtils.Min (_alphaBufferBounds.max, textureSize));

		// Clear
        int lineSize = Mathf.FloorToInt(textureSize.x);
        float[] currentAlphaBuffer = alphaBuffer;
        Color[] currentTemporaryLayer = temporaryLayer;
		for (int y = (int)_alphaBufferBounds.min.y, yMax = (int)_alphaBufferBounds.max.y; y < yMax; ++y) {
			for (int x = (int)_alphaBufferBounds.min.x, xMax = (int)_alphaBufferBounds.max.x; x < xMax; ++x) {
                currentAlphaBuffer[y * lineSize + x] = 0.0f;

				if( (y*lineSize + x) >= alphaBuffer.Length) 
				{
					Debug.LogError("index overflow");
				}

                currentTemporaryLayer[y * lineSize + x] = Color.clear;
			}
		}
    	
		_isAlphaBufferEmpty = true;
	}
	
	public float GetAlphaBuffer (int x, int y)
	{
		int width = Mathf.FloorToInt(textureSize.x);
		int idx = y * width + x;
		if(idx < alphaBuffer.Length) {
			return alphaBuffer[idx];
		}

		return 0.0f;
	}
	
	public void SetAlphaBuffer (int x, int y, float alpha)
	{
		int width = Mathf.FloorToInt(textureSize.x);
		int idx = y * width + x;
		if(idx < alphaBuffer.Length) {
			alphaBuffer[idx] = alpha;
		}
	}

	public void EncapsulateAlphaBufferBounds (Bounds bounds)
	{
		if (_isAlphaBufferEmpty) {
			_alphaBufferBounds = bounds;
			_isAlphaBufferEmpty = false;
		} else {
			_alphaBufferBounds.Encapsulate (bounds);
		}
	}

	public Color GetTemporaryLayer (int x, int y) {
		int width = Mathf.FloorToInt(textureSize.x);
		int idx = y * width + x;
		if(idx < temporaryLayer.Length) {
			return temporaryLayer[idx];
		}

		return Color.clear;
	}
	
	public void SetTemporaryLayer (int x, int y, Color color) {
		int width = Mathf.FloorToInt(textureSize.x);
		int idx = y * width + x;
		if(idx < temporaryLayer.Length) {
			temporaryLayer[idx] = color;
		}
	}
    
	public virtual void ClearDualBrushBuffer ()	{
		// Clamp
		_dualBrushBufferBounds.SetMinMax (
			BrushUtils.Max (_dualBrushBufferBounds.min, Vector3.zero),
			BrushUtils.Min (_dualBrushBufferBounds.max, textureSize)
		);

		// Clear
        int lineSize = Mathf.FloorToInt(textureSize.x);
		for (int y = (int)_dualBrushBufferBounds.min.y; y < (int)_dualBrushBufferBounds.max.y; ++y) {
			for (int x = (int)_dualBrushBufferBounds.min.x; x < (int)_dualBrushBufferBounds.max.x; ++x) {
				dualBrushBuffer[y * lineSize + x] = 0.0f;
			}
		}
    	
		_isDualBrushBufferEmpty = true;
	}

	public float GetDualBrushBuffer (int x, int y)
	{
		int width = Mathf.FloorToInt(textureSize.x);
		int idx = y * width + x;
		if(idx < dualBrushBuffer.Length) {
			return _dualBrushBuffer[idx];
		}

		return 0.0f;
	}

	public void SetDualBrushBuffer (int x, int y, float val)
	{
		int width = Mathf.FloorToInt(textureSize.x);
		int idx = y * width + x;
		if(idx < dualBrushBuffer.Length) {
			dualBrushBuffer[idx] = val;
		}
	}

	public void EncapsulateDualBrushBufferBounds (Bounds bounds)
	{
		if (_isDualBrushBufferEmpty) {
			_dualBrushBufferBounds = bounds;
			_isDualBrushBufferEmpty = false;
		} else {
			_dualBrushBufferBounds.Encapsulate (bounds);
		}
	}
}