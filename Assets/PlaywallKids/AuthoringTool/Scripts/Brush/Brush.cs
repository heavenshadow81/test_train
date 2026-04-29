using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Brush : ICloneable
{ 
	protected string    mBrushName;
	protected MaskType  mMaskType;
	protected string    mMaskName;
	protected Texture2D mMaskTexture;
	protected bool      mUseMaskColor;
	protected float     mDiameter;
	protected float     mHalfDiameter;
	protected float     mLastDiameter;
	protected Color     mColor;
	protected float     mSpacing;
	protected float     mHardness;
	protected float     mOpacity;
	protected float     mFlow;
	protected float     mAngle;
	protected float     mRoundness;
	protected bool      mAirbrush;
	protected BlendMode mBlendMode;
	protected bool      mUseAlphaBuffer;
	private   bool      mPaintStartPosition;
    private   bool      mPaintOnDrag;
	protected float     mAirbrushScale = 1.0f; //
//	protected Vector2 prePos;
//	protected Vector2 startPos;
	protected BrushComponent[] mBrushComponents = new BrushComponent[6];
	private   bool      mNeedPaint;
	private   Vector3   mPreviousPosition;
	private   Vector3   mStartPosition;
	private   Vector3   mEndPosition;
	private	  int 		mTouchId;
	
	// Cached variables
	private static Color[] _colorList = null;
	private static int _colorListSize = 0;

    // for GPGPU Mode
    private static RenderTexture _computeTextureBackup;
    private class _CanvasGPGPUDrawingBatchInfo : IDisposable 
    {
        // Variables
        public int maskType = 0;
        public Vector2[] maskCenterArray;
        public float[] halfDiameterArray;
        public float[] opacityArray;
        public float[] flowArray;
        public float[] angleArray;
        public float[] hardnessArray;
        public bool useMaskColor = false;
        public bool airbrush = false;
        public Color[] colorArray;
        public Vector2[] halfSizeArray;
        public int blendMode = 0;
        public Matrix4x4[] maskTransformArray;
        public Vector4[] stampBoundsArray;
        public float[] elapsedTimeArray;
        public Texture maskTexture;

        // Range
        public int minX, minY, maxX, maxY;
        public const int numThreads = 16;
       
        // Buffers
        private ComputeBuffer maskCenterBuffer;
        private ComputeBuffer halfDiameterBuffer;
        private ComputeBuffer opacityBuffer;
        private ComputeBuffer flowBuffer;
        private ComputeBuffer angleBuffer;
        private ComputeBuffer hardnessBuffer;
        private ComputeBuffer colorBuffer;
        private ComputeBuffer halfSizeBuffer;
        private ComputeBuffer maskTransformBuffer;
        private ComputeBuffer stampBoundsBuffer;
        private ComputeBuffer elapsedTimeBuffer;
        // buffer size
        private int _bufferSize = 0;
        public int bufferSize
        {
            get
            {
                return _bufferSize;
            }
        }

        // count
        public int count = 0;
        public static int maxCount = 200;

        /*
        ~_CanvasGPGPUDrawingBatchInfo()
        {
            Dispose();
        }*/

        public void Dispose()
        {
            Debug.Log("ComputeBuffer Dispose");
            maskCenterBuffer.Dispose();
            halfDiameterBuffer.Dispose();
            opacityBuffer.Dispose();
            flowBuffer.Dispose();
            angleBuffer.Dispose();
            hardnessBuffer.Dispose();
            colorBuffer.Dispose();
            halfSizeBuffer.Dispose();
            maskTransformBuffer.Dispose();
            stampBoundsBuffer.Dispose();
            elapsedTimeBuffer.Dispose();

            maskCenterBuffer = null;
            halfDiameterBuffer = null;
            opacityBuffer = null;
            flowBuffer = null;
            angleBuffer = null;
            hardnessBuffer = null;
            colorBuffer = null;
            halfSizeBuffer = null;
            maskTransformBuffer = null;
            stampBoundsBuffer = null;
            elapsedTimeBuffer = null;
        }

        public _CanvasGPGPUDrawingBatchInfo(int initialBufferSize)
        {
            ResizeBuffers(initialBufferSize);
            Reset();
        }

        public void ResizeBuffers(int bufferSize)
        {
            if (bufferSize < 4) bufferSize = 4;

            if (_bufferSize != bufferSize)
            {
                _bufferSize = bufferSize;

                var newMaskCenterArray = new Vector2[bufferSize];
                var newHalfDiameterArray = new float[bufferSize];
                var newOpacityArray = new float[bufferSize];
                var newFlowArray = new float[bufferSize];
                var newAngleArray = new float[bufferSize];
                var newHardnessArray = new float[bufferSize];
                var newColorArray = new Color[bufferSize];
                var newHalfSizeArray = new Vector2[bufferSize];
                var newMaskTransformArray = new Matrix4x4[bufferSize];
                var newStampBoundsArray = new Vector4[bufferSize];
                var newElapsedTimeArray = new float[bufferSize];

                if(maskCenterArray != null) System.Array.Copy(maskCenterArray, newMaskCenterArray, maskCenterArray.Length);
                if (halfDiameterArray != null) System.Array.Copy(halfDiameterArray, newHalfDiameterArray, halfDiameterArray.Length);
                if (opacityArray != null) System.Array.Copy(opacityArray, newOpacityArray, opacityArray.Length);
                if (flowArray != null) System.Array.Copy(flowArray, newFlowArray, flowArray.Length);
                if (angleArray != null) System.Array.Copy(angleArray, newAngleArray, angleArray.Length);
                if (hardnessArray != null) System.Array.Copy(hardnessArray, newHardnessArray, hardnessArray.Length);
                if (colorArray != null) System.Array.Copy(colorArray, newColorArray, colorArray.Length);
                if (halfSizeArray != null) System.Array.Copy(halfSizeArray, newHalfSizeArray, halfSizeArray.Length);
                if (maskTransformArray != null) System.Array.Copy(maskTransformArray, newMaskTransformArray, maskTransformArray.Length);
                if (stampBoundsArray != null) System.Array.Copy(stampBoundsArray, newStampBoundsArray, stampBoundsArray.Length);
                if (elapsedTimeArray != null) System.Array.Copy(elapsedTimeArray, newElapsedTimeArray, elapsedTimeArray.Length);

                maskCenterArray = newMaskCenterArray;
                halfDiameterArray = newHalfDiameterArray;
                opacityArray = newOpacityArray;
                flowArray = newFlowArray;
                angleArray = newAngleArray;
                hardnessArray = newHardnessArray;
                colorArray = newColorArray;
                halfSizeArray = newHalfSizeArray;
                maskTransformArray = newMaskTransformArray;
                stampBoundsArray = newStampBoundsArray;
                elapsedTimeArray = newElapsedTimeArray;

                ReleaseBuffers();
            }
        }

        public void ReleaseBuffers()
        {
            if (maskCenterBuffer != null) maskCenterBuffer.Release();
            if (halfDiameterBuffer != null) halfDiameterBuffer.Release();
            if (opacityBuffer != null) opacityBuffer.Release();
            if (flowBuffer != null) flowBuffer.Release();
            if (angleBuffer != null) angleBuffer.Release();
            if (hardnessBuffer != null) hardnessBuffer.Release();
            if (colorBuffer != null) colorBuffer.Release();
            if (halfSizeBuffer != null) halfSizeBuffer.Release();
            if (maskTransformBuffer != null) maskTransformBuffer.Release();
            if (stampBoundsBuffer != null) stampBoundsBuffer.Release();
            if (elapsedTimeBuffer != null) elapsedTimeBuffer.Release();

            maskCenterBuffer = null;
            halfDiameterBuffer = null;
            opacityBuffer = null;
            flowBuffer = null;
            angleBuffer = null;
            hardnessBuffer = null;
            colorBuffer = null;
            halfSizeBuffer = null;
            maskTransformBuffer = null;
            stampBoundsBuffer = null;
            elapsedTimeBuffer = null;
        }

        public void AllocateBuffers()
        {
            if (maskCenterBuffer == null)       maskCenterBuffer = new ComputeBuffer(count, sizeof(float) * 2);          
            if (halfDiameterBuffer == null)     halfDiameterBuffer = new ComputeBuffer(count, sizeof(float) * 1); 
            if (opacityBuffer == null)          opacityBuffer = new ComputeBuffer(count, sizeof(float) * 1);         
            if (flowBuffer == null)             flowBuffer = new ComputeBuffer(count, sizeof(float) * 1);  
            if (angleBuffer == null)            angleBuffer = new ComputeBuffer(count, sizeof(float) * 1);  
            if (hardnessBuffer == null)         hardnessBuffer = new ComputeBuffer(count, sizeof(float) * 1);      
            if (colorBuffer == null)            colorBuffer = new ComputeBuffer(count, sizeof(float) * 4);
            if (halfSizeBuffer == null)         halfSizeBuffer = new ComputeBuffer(count, sizeof(float) * 2);
            if (maskTransformBuffer == null)    maskTransformBuffer = new ComputeBuffer(count, sizeof(float) * 16);
            if (stampBoundsBuffer == null)      stampBoundsBuffer = new ComputeBuffer(count, sizeof(float) * 4);
            if (elapsedTimeBuffer == null)      elapsedTimeBuffer = new ComputeBuffer(count, sizeof(float));
        }

        public void Reset()
        {
            count = 0;
            minX = int.MaxValue;
            minY = int.MaxValue;
            maxX = int.MinValue;
            maxY = int.MinValue;
        }

        public void Dispatch(Canvas_ canvas)
        {
            if (count < 1) return;

            if (maskCenterBuffer != null && count > maskCenterBuffer.count)
            {
                ReleaseBuffers();
            }
            AllocateBuffers();

            ComputeShader cs = Canvas_.computeShader;
            if (cs != null)
            {
                RenderTexture texture = (RenderTexture)canvas.texture;

                if (_computeTextureBackup == null ||
                    _computeTextureBackup.width != texture.width ||
                    _computeTextureBackup.height != texture.height)
                {
                    if (_computeTextureBackup != null)
                    {
                        _computeTextureBackup.Release();
                        GameObject.Destroy(_computeTextureBackup);
                    }

                    _computeTextureBackup = new RenderTexture(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
                    _computeTextureBackup.Create();
                }

                RenderTexture prev = RenderTexture.active;
                RenderTexture.active = texture;
                Graphics.Blit(texture, _computeTextureBackup);
                RenderTexture.active = prev;

                int kernel = cs.FindKernel("CanvasDrawingPaint");
                if (kernel != -1)
                {
                    cs.SetInt("count", count);
                    cs.SetFloats("offset", (float)minX, (float)minY);
                    if (maskTexture != null)
                    {
                        cs.SetInt("hasMaskTexture", 1);
                        cs.SetTexture(kernel, "maskTexture", maskTexture);
                    }
                    else
                    {
                        cs.SetInt("hasMaskTexture", 0);
                        cs.SetTexture(kernel, "maskTexture", Texture2D.whiteTexture);
                    }
                    
                    maskTransformBuffer.SetData(maskTransformArray, 0, 0, maskTransformBuffer.count);
                    //maskTransformBuffer.SetData(maskTransformArray);
                    maskCenterBuffer.SetData(maskCenterArray, 0, 0, maskCenterBuffer.count);
                    //maskCenterBuffer.SetData(maskCenterArray);
                    halfDiameterBuffer.SetData(halfDiameterArray, 0, 0, halfDiameterBuffer.count);
                    //halfDiameterBuffer.SetData(halfDiameterArray);
                    opacityBuffer.SetData(opacityArray, 0, 0, opacityBuffer.count);
                    //opacityBuffer.SetData(opacityArray);
                    flowBuffer.SetData(flowArray, 0,0,flowBuffer.count);
                    //flowBuffer.SetData(flowArray);
                    angleBuffer.SetData(angleArray, 0,0, angleBuffer.count);
                    //angleBuffer.SetData(angleArray);
                    hardnessBuffer.SetData(hardnessArray,0,0,hardnessBuffer.count);
                    //hardnessBuffer.SetData(hardnessArray);
                    colorBuffer.SetData(colorArray,0,0,colorBuffer.count);
                    //colorBuffer.SetData(colorArray);
                    halfSizeBuffer.SetData(halfSizeArray, 0,0, halfSizeBuffer.count);
                    //halfSizeBuffer.SetData(halfSizeArray);
                    stampBoundsBuffer.SetData(stampBoundsArray, 0,0,stampBoundsBuffer.count);
                    //stampBoundsBuffer.SetData(stampBoundsArray);
                    elapsedTimeBuffer.SetData(elapsedTimeArray, 0,0,elapsedTimeBuffer.count);
                    //elapsedTimeBuffer.SetData(elapsedTimeArray);


                    cs.SetBuffer(kernel, "maskTransformBuffer", maskTransformBuffer);
                    cs.SetInt("maskType", (int)maskType);
                    cs.SetBuffer(kernel, "maskCenterBuffer", maskCenterBuffer);
                    cs.SetBuffer(kernel, "halfDiameterBuffer", halfDiameterBuffer);
                    cs.SetBuffer(kernel, "opacityBuffer", opacityBuffer);
                    cs.SetBuffer(kernel, "flowBuffer", flowBuffer);
                    cs.SetBuffer(kernel, "angleBuffer", angleBuffer);
                    cs.SetBuffer(kernel, "hardnessBuffer", hardnessBuffer);
                    cs.SetInt("useMaskColor", useMaskColor ? 1 : 0);
                    cs.SetInt("airbrush", airbrush ? 1 : 0);
                    cs.SetBuffer(kernel, "colorBuffer", colorBuffer);
                    cs.SetBuffer(kernel, "halfSizeBuffer", halfSizeBuffer);
                    cs.SetInt("blendMode", (int)blendMode);
                    cs.SetBuffer(kernel, "stampBoundsBuffer", stampBoundsBuffer);
                    cs.SetBuffer(kernel, "elapsedTimeBuffer", elapsedTimeBuffer);
                    cs.SetTexture(kernel, "input", _computeTextureBackup);
                    cs.SetTexture(kernel, "output", texture);
                    
                    cs.SetInt("width", texture.width);
                    cs.SetInt("height", texture.height);

                    int sizeX = maxX - minX;
                    int sizeY = maxY - minY;
                    sizeX = sizeX + (numThreads - sizeX % numThreads);
                    sizeY = sizeY + (numThreads - sizeY % numThreads);

                    if(sizeX / numThreads > 0 && sizeY / numThreads > 0)
                    {
                        cs.SetBuffer(kernel, "colorBuffer", colorBuffer);
                        
                        //바로 윗줄은 내가 임의로 추가한 버퍼
                        cs.Dispatch(kernel, sizeX / numThreads, sizeY / numThreads, 1);
                    }
                        

                    //Debug.Log(string.Format("Brush : Dispatched at range ({0},{1},{2},{3})", minX, minY, sizeX, sizeY));
                }
            }
        }   
    }
    private static _CanvasGPGPUDrawingBatchInfo _computeDrawingBatchInfo = null;

	public Brush ()
	{
		brushName = "Brush";
		maskType = MaskType.VectorMask;
		maskName = "";
		useMaskColor = false;
		diameter = 1.0f;
		color = Color.clear;
		spacing = 0.25f;
		hardness = 1.0f;
		opacity = 1.0f;
		flow = 1.0f;
		angle = 0.0f;
		roundness = 1.0f;
		airbrush = false;
		blendMode = BlendMode.AlphaBlend;
		paintStartPosition = true;
		mUseAlphaBuffer = true;
        mPaintOnDrag = true;

		shapeDynamicComponent = new BrushShapeDynamicComponent ();
		scatteringComponent = new BrushScatteringComponent ();
		textureComponent = new BrushTextureComponent ();
		dualComponent = new BrushDualComponent ();
		colorDynamicComponent = new BrushColorDynamicComponent ();
		transferComponent = new BrushTransferComponent ();

		mNeedPaint = false;
	}

	public object Clone() {
        Brush newBrush = (Brush)this.MemberwiseClone();

        if (shapeDynamicComponent != null)
            newBrush.shapeDynamicComponent = (BrushShapeDynamicComponent)shapeDynamicComponent.Clone();
        if (scatteringComponent != null)
            newBrush.scatteringComponent = (BrushScatteringComponent)scatteringComponent.Clone();
        if (textureComponent != null)
            newBrush.textureComponent = (BrushTextureComponent)textureComponent.Clone();
        if (dualComponent != null)
            newBrush.dualComponent = (BrushDualComponent)dualComponent.Clone();
        if (colorDynamicComponent != null)
            newBrush.colorDynamicComponent = (BrushColorDynamicComponent)colorDynamicComponent.Clone();
        if (transferComponent != null)
            newBrush.transferComponent = (BrushTransferComponent)transferComponent.Clone();

        return newBrush;
	}
    
	public string brushName {
		get {
			return mBrushName;
		}
		set {
			mBrushName = value;
		}
	}
    
	public MaskType maskType {
		get {
			return mMaskType;
		}
		set {
			mMaskType = value;
		}
	}
    
	public string maskName {
		get {
			return mMaskName;
		}
		set {
			if(mMaskName == null || !mMaskName.Equals(value) || mMaskTexture == null)
			{
				if(mMaskTexture != null)
				{
					mMaskTexture = null;
				}

                if (value != null && value.Length > 0)
                {
                    mMaskTexture = Resources.Load("Canvas/Palettes/" + value) as Texture2D;

                    if (mMaskTexture != null)
                        mMaskName = value;
                }
			}
		}
	}

	public bool useMaskColor {
		get {
			return mUseMaskColor;
		}
		set {
			mUseMaskColor = value;
		}
	}
    
	// (1, 5000)
	public float diameter {
		get {
			return mDiameter;
		}
		set {
			mDiameter = Mathf.Clamp (value, 1.0f, 5000.0f);
			mHalfDiameter = mDiameter * 0.5f;
		}
	}
    
	public Color color {
		get {
			return mColor;
		}
		set {
			mColor = value;
		}
	}
    
	// (0.01, 10)
	public float spacing {
		get {
			return mSpacing;
		}
		set {
			mSpacing = Mathf.Clamp (value, 0.01f, 10.0f);
		}
	}
    
	// (0, 1)
	public float hardness {
		get {
			return mHardness;
		}
		set {
			mHardness = Mathf.Clamp (value, 0.0f, 1.0f);
		}
	}
    
	// (0.01, 1)
	public float opacity {
		get {
			return mOpacity;
		}
		set {
			mOpacity = Mathf.Clamp (value, 0.01f, 1.0f);
		}
	}
    
	// (0.01, 1)
	public float flow {
		get {
			return mFlow;
		}
		set {
			mFlow = Mathf.Clamp (value, 0.01f, 1.0f);
		}
	}

	public float angle { 
		get {
			return mAngle;
		}
		set {
			mAngle = Mathf.Clamp (value, 0f, 360f);
		}
	}

	public float roundness {
		get {
			return mRoundness;
		}
		set {
			mRoundness = Mathf.Clamp01 (value);
		}
	}
    
	public bool airbrush {
		get {
			return mAirbrush;
		}
		set {
			mAirbrush = value;
		}
	}
    
	public BlendMode blendMode {
		get {
			return mBlendMode;
		}
		set {
			mBlendMode = value;
		}
	}

	public bool paintStartPosition {
		get {
			return mPaintStartPosition;
		}
		set {
			mPaintStartPosition = value;
		}
	}
	
	public bool useAlphaBuffer {
		get {
			return mUseAlphaBuffer;
		}
		set {
			mUseAlphaBuffer = value;
		}
	}

    public bool paintOnDrag //150122
    {
        get
        {
            return mPaintOnDrag;
        }
        set
        {
			if(value ==false)
			{ mPaintStartPosition = true; }
			else
			{
				mPaintStartPosition = false;
			}
            mPaintOnDrag = value;
        }
    }
    /// <summary>
    /// now drawing check
    /// only get canvas!!
    /// </summary>
    public bool isDrawing  {
        get  {
            return mNeedPaint;
        }
    }

	public void StartPaint (Canvas_ canvas, Vector3 pos)
	{
		mNeedPaint = true;

        mPreviousPosition = mStartPosition = mEndPosition = pos;
		
		if (paintStartPosition) {
			// Dual Brush
			if (dualComponent.enable) {
				dualComponent.StartPaint (mPreviousPosition, mStartPosition, mEndPosition); 
				dualComponent.Paint (canvas, mPreviousPosition, mStartPosition, mEndPosition);
			}

			Paint (canvas, mPreviousPosition, mStartPosition);
		}
	}

	public void EndPaint (Canvas_ canvas)
	{
		mNeedPaint = false;
		mAirbrushScale = 1.0f;//
        
        if (Canvas_.supportsComputeShaders)
        {
            if (_computeDrawingBatchInfo != null)
                _computeDrawingBatchInfo.ReleaseBuffers();
        }
        else
        {
            // Currently brush doesn't use auxiliary buffers in GPGPU mode.

            // Dual Brush
            if (dualComponent.enable)
                canvas.ClearDualBrushBuffer();

            // Alpha Buffer
            if (useAlphaBuffer)
                canvas.ClearAlphaBuffer();
        }
	}

    /// <summary>
    /// Paints on the canvas at position.
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="pos"></param>
    public void Paint(Canvas_ canvas, Vector3 pos)
    {
        if (!mNeedPaint)
            return;

        mEndPosition = pos;

        Vector3 delta = mEndPosition - mStartPosition;
        float dist = mLastDiameter * spacing;

        // Dual Brush
        if (dualComponent.enable)
        {
            dualComponent.Paint(canvas, mEndPosition, delta.magnitude);
        }

        if (delta.magnitude > dist)
        {
            Vector3 prev = mPreviousPosition;
            Vector3 curr = BrushUtils.CatmullRom(mPreviousPosition, mStartPosition, mEndPosition, mEndPosition, dist, delta.magnitude);
            Vector3 next = new Vector3();

            for (float i = dist; i < delta.magnitude; i += dist)
            {
                next = Paint(canvas, prev, curr, i, delta.magnitude);

                dist = mLastDiameter * spacing;
                prev = curr;
                curr = next;
            }

            mPreviousPosition = mStartPosition;
            mStartPosition = mEndPosition;

            FlushPaint(canvas);
        }
        else if (airbrush)
        {
            // Dual Brush
            if (dualComponent.enable)
            {
                dualComponent.Paint(canvas, mPreviousPosition, mStartPosition, mEndPosition);
            }

            Paint(canvas, mPreviousPosition, mStartPosition);
        }

    }

	public Vector3 Paint(Canvas_ canvas, Vector3 previous, Vector3 current) {
		Vector2 next = Paint (canvas, previous, current, 0.0f, 0.0f);

        FlushPaint(canvas);

        return next;
	}

    public void FlushPaint(Canvas_ canvas)
    {
        if (Canvas_.supportsComputeShaders)
        {
            _computeDrawingBatchInfo.maskTexture = mMaskTexture;
            _computeDrawingBatchInfo.useMaskColor = useMaskColor;
            _computeDrawingBatchInfo.maskType = (int)maskType;
            _computeDrawingBatchInfo.blendMode = (int)blendMode;
            _computeDrawingBatchInfo.airbrush = airbrush;

            if (_computeDrawingBatchInfo.count > 0)
            {
                _computeDrawingBatchInfo.Dispatch(canvas);
                _computeDrawingBatchInfo.Reset();
            }
        }
    }

	public Vector3 Paint (Canvas_ canvas, Vector3 previous, Vector3 current, float elapsedTime, float duration)
	{
		Vector3 delta = new Vector3 ();
		Vector3 up = new Vector3 ();
		Vector3 next = new Vector3 ();

		// Scattering Component
		int count = 1;
		if (elapsedTime > 0.0f && scatteringComponent.enable) {
			count = scatteringComponent.UpdateCount ();
		}

        if(Canvas_.supportsComputeShaders)
        {
            if (_computeDrawingBatchInfo == null)
            {   _computeDrawingBatchInfo = new _CanvasGPGPUDrawingBatchInfo(count);  }

            if (_computeDrawingBatchInfo.count + count > _computeDrawingBatchInfo.bufferSize)
            {   _computeDrawingBatchInfo.ResizeBuffers(_computeDrawingBatchInfo.count + count);  }
        }

		for (int i = 0; i < count; ++i) {
			Vector3 newPosition = current;
			float newAngle = angle;
			Color newColor = color;
			float newOpacity = opacity;
			float newFlow = flow;
			float newDiameter = diameter;
			float newHalfDiameter = mHalfDiameter;
			
			if (airbrush && elapsedTime == 0.0f) {
				mAirbrushScale += 0.01f;
				if (mAirbrushScale > 2.0f) { 
					mAirbrushScale = 2.0f;
				}
				newDiameter *= mAirbrushScale;
				newHalfDiameter *= mAirbrushScale;
			}
			
			Vector3 newSize = new Vector3 (newDiameter * roundness, newDiameter);

			// Shape Dynamic Component
			if (shapeDynamicComponent.enable) {
				newSize = shapeDynamicComponent.UpdateSize (newSize);
			}
			Vector3 halfSize = newSize * 0.5f;

			// Calculate prerequisites parameters
			if (i == 0) {
				mLastDiameter = newSize.y;
				next = BrushUtils.CatmullRom (mPreviousPosition, mStartPosition, mEndPosition, mEndPosition, elapsedTime + mLastDiameter * spacing, duration);
				delta = next - previous;
				up = Vector3.Cross (delta, -Vector3.forward).normalized;
			}

			// Shape Dynamic Component
			if (shapeDynamicComponent.enable) {
				newAngle = shapeDynamicComponent.UpdateAngle (newAngle, delta);
			}
            
			// Scattering Component
			if (scatteringComponent.enable) {
				newPosition = scatteringComponent.UpdatePosition (current, up, delta.normalized, newSize);
			}

			// Texture Component
			if (textureComponent.enable) {
				// TODO
			}

			// Dual Component
			if (dualComponent.enable) {
				// TODO
			}

			// Color Dynamic Component
			if (colorDynamicComponent.enable) {
				newColor = colorDynamicComponent.UpdateColor (newColor);
			}

			// Transfer Component
			if (transferComponent.enable) {
				newOpacity = transferComponent.UpdateOpacity (newOpacity);
				newFlow = transferComponent.UpdateFlow (newFlow);
			}

			// Make Mask Transform
			Matrix4x4 textureToCanvasM = canvas.GetTextureToCanvasMatrix();// Texture to canvas
			Matrix4x4 zeroBaseDiffM = Matrix4x4.TRS (-newPosition + halfSize, Quaternion.identity, Vector3.one);// Canvas to zero base
			Matrix4x4 t = Matrix4x4.TRS (-halfSize, Quaternion.identity, Vector3.one);
			Matrix4x4 r = Matrix4x4.TRS (Vector3.zero, Quaternion.Euler (0.0f, 0.0f, newAngle), Vector3.one);
			Matrix4x4 rotateM = t.inverse * r * t;// Rotate
			Vector3 scale = new Vector3 (newDiameter / newSize.x, newDiameter / newSize.y);
			if (maskType == MaskType.PixelMask && mMaskTexture != null) {
				scale.Scale (new Vector3 (mMaskTexture.width / newDiameter, mMaskTexture.height / newDiameter));
			}
			Matrix4x4 scaleM = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, scale);// Scale
			Matrix4x4 maskTransform = scaleM * rotateM * zeroBaseDiffM * textureToCanvasM;

			// Draw
			Vector3 stampPosition = canvas.CanvasToTexture (newPosition);
			Vector3 stampSize = canvas.CanvasToTexture (newSize);
			Bounds stampBounds = new Bounds (stampPosition, stampSize);
			stampBounds.SetMinMax (// Clamp out ot texture
                BrushUtils.Max (stampBounds.min, Vector3.zero), 
                BrushUtils.Min (stampBounds.max, canvas.textureSize)
            );
			
			Vector3 maskCenter = new Vector3 ((newDiameter - 1.0f) * 0.5f, (newDiameter - 1.0f) * 0.5f);// (0, size - 1)
			
			int width = (int)stampBounds.max.x - (int)stampBounds.min.x + 1;
			int height = (int)stampBounds.max.y - (int)stampBounds.min.y + 1;

            Vector3 textureSize = canvas.textureSize;
			
			if((int)stampBounds.min.x + width > Mathf.FloorToInt(textureSize.x)) {
				width = Mathf.FloorToInt(textureSize.x) - (int)stampBounds.min.x; 
			}
			if((int)stampBounds.min.y + height > Mathf.FloorToInt(textureSize.y)) {
				height = Mathf.FloorToInt(textureSize.y) - (int)stampBounds.min.y;
			}

			if(width <= 0 || height <= 0) {
				return next;
			}

            if (!mPaintOnDrag)
            {
                if (previous != current)
                {
                    count = i + 1;
                    break;
                }
            }

			if(width * height > _colorListSize) {
				_colorList = new Color[width * height];
				_colorListSize = width * height;
			}

			int idx = 0;
			int minX = (int)stampBounds.min.x, maxX = minX + width;
			int minY = (int)stampBounds.min.y, maxY = minY + height;

            if (Canvas_.supportsComputeShaders)
            {
                int indexOffset = _computeDrawingBatchInfo.count;

                _computeDrawingBatchInfo.maskTransformArray[indexOffset + i] = maskTransform;
                _computeDrawingBatchInfo.maskCenterArray[indexOffset + i] = maskCenter;
                _computeDrawingBatchInfo.halfDiameterArray[indexOffset + i] = newHalfDiameter;
                _computeDrawingBatchInfo.opacityArray[indexOffset + i] = newOpacity;
                _computeDrawingBatchInfo.flowArray[indexOffset + i] = newFlow;
                _computeDrawingBatchInfo.angleArray[indexOffset + i] = newAngle;
                _computeDrawingBatchInfo.hardnessArray[indexOffset + i] = hardness;
                _computeDrawingBatchInfo.colorArray[indexOffset + i] = newColor;
                _computeDrawingBatchInfo.halfSizeArray[indexOffset + i] = halfSize;
                _computeDrawingBatchInfo.stampBoundsArray[indexOffset + i] = new Vector4(minX, minY, maxX, maxY);
                _computeDrawingBatchInfo.elapsedTimeArray[indexOffset + i] = elapsedTime;

                _computeDrawingBatchInfo.minX = Mathf.Min(_computeDrawingBatchInfo.minX, Mathf.Max(minX, 0));
                _computeDrawingBatchInfo.minY = Mathf.Min(_computeDrawingBatchInfo.minY, Mathf.Max(minY, 0));
                _computeDrawingBatchInfo.maxX = Mathf.Max(_computeDrawingBatchInfo.maxX, maxX);
                _computeDrawingBatchInfo.maxY = Mathf.Max(_computeDrawingBatchInfo.maxY, maxY);

                _computeDrawingBatchInfo.count++;

                if (_computeDrawingBatchInfo.count >= _CanvasGPGPUDrawingBatchInfo.maxCount)
                {
                    FlushPaint(canvas);
                }

                //canvas.EncapsulateAlphaBufferBounds(stampBounds);
            }
            else
            {
                Color[] textureColors = canvas.textureColors;

                for (int y = minY; y < maxY; ++y)
                {
                    for (int x = minX; x < maxX; ++x)
                    {
                        // Mask Position
                        Vector3 maskPosition = maskTransform.MultiplyPoint3x4(new Vector3(x, y));

                        // Alpha 
                        if (maskType == MaskType.VectorMask || maskType == MaskType.EraserMask)
                        {// Calculate alpha
                            float distance = (maskPosition - maskCenter).magnitude;
                            float halfInnerSize = newHalfDiameter * hardness;

                            if (distance > newHalfDiameter)
                            {
                                newColor.a = 0.0f;
                            }
                            else if (distance > halfInnerSize)
                            {
                                float weight = Mathf.Min(1.0f, (distance - halfInnerSize) / (newHalfDiameter - halfInnerSize));
                                //newColor.a = 1.0f - 4.0f * (weight * weight);
                                if (weight <= 0.3f)
                                {
                                    weight = 1.0f - 0.1f / 0.3f * weight;
                                }
                                else if (weight <= 0.7f)
                                {
                                    weight = 1.0f - (0.8f / 0.4f * weight - 0.5f);
                                }
                                else
                                {
                                    weight = 1.0f - (0.1f / 0.3f * weight + (1.0f - 0.1f / 0.3f));
                                }
                                newColor.a = weight;
                            }
                            else
                            {
                                newColor.a = 1.0f;
                            }

                        }
                        else if (maskType == MaskType.PixelMask && mMaskTexture != null)
                        {// Get alpha from mask texture
                            Color maskColor = mMaskTexture.GetPixel((int)maskPosition.x, (int)maskPosition.y);

                            if (useMaskColor)
                            {
                                newColor = maskColor;
                            }
                            else
                            {
                                newColor.a = maskColor.a;
                            }
                        }

                        newColor.a *= newOpacity * newFlow;
                        if (maskType == MaskType.EraserMask)
                        {
                            newColor = new Color(1.0f, 1.0f, 1.0f, 1.0f - newColor.a);
                        }

                        if (airbrush && elapsedTime == 0.0f)
                        {
                            newColor.a *= Time.deltaTime;
                        }

                        // Dual Brush
                        if (dualComponent.enable)
                        {
                            float dualWeight = canvas.GetDualBrushBuffer(x, y);
                            newColor = BrushUtils.Blend(dualComponent.blendMode, newColor, new Color(1.0f, 1.0f, 1.0f, dualWeight));
                        }

                        // Blend
                        Color baseColor = textureColors[y * (int)textureSize.x + x];

                        if (useAlphaBuffer && !canvas.supportsMultiTouch)
                        {// Temporary layer
                            float oldAlpha = canvas.GetAlphaBuffer(x, y);

                            if (oldAlpha == 0.0f)
                            {
                                canvas.SetTemporaryLayer(x, y, canvas.GetPixel(x, y));
                            }
                            else
                            {
                                baseColor = canvas.GetTemporaryLayer(x, y);
                            }

                            newColor.a = Mathf.Min(newColor.a + oldAlpha, opacity);
                            canvas.SetAlphaBuffer(x, y, newColor.a);
                        }
                        Color blendedColor = BrushUtils.Blend(blendMode, baseColor, newColor);

                        textureColors[y * (int)textureSize.x + x] = _colorList[idx++] = blendedColor;
                    }
                }

                Texture2D texture = (Texture2D)canvas.texture;
                texture.SetPixels((int)stampBounds.min.x, (int)stampBounds.min.y,
                    width, height, _colorList);
            }

			canvas.EncapsulateAlphaBufferBounds (stampBounds);
		}

		return next;
	}

	public BrushShapeDynamicComponent shapeDynamicComponent {
		get {
			return (BrushShapeDynamicComponent)mBrushComponents [(int)ComponentType.ShapeDynamic];
		}
		set {
			mBrushComponents [(int)ComponentType.ShapeDynamic] = value;
		}
	}

	public BrushScatteringComponent scatteringComponent {
		get {
			return (BrushScatteringComponent)mBrushComponents [(int)ComponentType.Scattering];
		}
		set {
			mBrushComponents [(int)ComponentType.Scattering] = value;
		}
	}

	public BrushTextureComponent textureComponent {
		get {
			return (BrushTextureComponent)mBrushComponents [(int)ComponentType.Texture];
		}
		set {
			mBrushComponents [(int)ComponentType.Texture] = value;
		}
	}

	public BrushDualComponent dualComponent {
		get {
			return (BrushDualComponent)mBrushComponents [(int)ComponentType.Dual];
		}
		set {
			mBrushComponents [(int)ComponentType.Dual] = value;
		}
	}

	public BrushColorDynamicComponent colorDynamicComponent {
		get {
			return (BrushColorDynamicComponent)mBrushComponents [(int)ComponentType.ColorDynamic];
		}
		set {
			mBrushComponents [(int)ComponentType.ColorDynamic] = value;
		}
	}

	public BrushTransferComponent transferComponent {
		get {
			return (BrushTransferComponent)mBrushComponents [(int)ComponentType.Transfer];
		}
		set {
			mBrushComponents [(int)ComponentType.Transfer] = value;
		}
	}
};