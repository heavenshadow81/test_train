using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleModelControl : MonoBehaviour
{
	#region Properties
	private GameObject _model = null;
	public GameObject model
	{
		get
		{
			return _model;
		}
		set
		{
			_model = value;
			
			if(_model != null) {
				templates = new List<Template3D>(_model.GetComponentsInChildren<Template3D>());
			}
			else {
				templates = new List<Template3D>();
			}
		}
	}
	
	private List<Template3D> _templates = new List<Template3D>();
	public List<Template3D> templates
	{
		get
		{
			return _templates;
		}
		protected set
		{
			_templates.Clear();
			if (value != null)
			{
				_templates.AddRange(value);
			}
			rotate = _rotate;
			wantsPaint = _wantsPaint;
		}
	}
	
	public Template3D template {
		get {
			return templates.Count > 0 ? templates[0] : null;
		}
	}

	private bool _rotate = false;
	public bool rotate
	{
		get
		{
			return _rotate;
		}
		set
        {
            RotateAndType(value);
		}
	}

	
	private bool _wantsPaint = false;
	public bool wantsPaint
	{
		get
		{
			return _wantsPaint;
		}
		set
		{
			_wantsPaint = value;
			foreach (Template3D template in templates)
			{
				template.wantsPaint = _wantsPaint;
			}
		}
	}
	
	private int _brushSize = 32;
	public int brushSize
	{
		get
		{
			return _brushSize;
		}
		set
		{
			_brushSize = Mathf.Clamp(value, 4, 32);
		}
	}
	
	private Color _brushColor = Color.red;
	public Color brushColor
	{
		get
		{
			return _brushColor;
		}
		set
		{
			_brushColor = value;
		}
	}
	#endregion

	#region Private variables
    protected Dictionary<int, Vector2> _prevTouchPosDict = new Dictionary<int, Vector2>();
    protected List<Vector2> _paintedUVs = new List<Vector2>(64);

    private TweenScale curScale = null;
	#endregion

	public void Update () {
		if(model == null) return;

		if(wantsPaint)
		{
			if(rotate)
				TCCamera.sharedInstance.RequestRefreshTCRT();

			if (CustomInput.touchCount > 0)
			{
				for (int i = 0, cnt = CustomInput.touchCount; i < cnt; i++)
				{
					TouchInfo touch = CustomInput.GetTouch(i);
					
					_DrawForTouch(touch);
				}
			}
			else
			{
				foreach (Template3D template in templates)
				{
					if (template.painting) template.EndPaint();
				}
			}
		}
	}

	protected virtual void _DrawForTouch(TouchInfo touchInfo)
	{
		if (templates.Count == 0)
		{
			templates = new List<Template3D>(model.GetComponentsInChildren<Template3D>());
		}

        Vector2 prevPos = Vector2.zero;

        if (_prevTouchPosDict.ContainsKey(touchInfo.id))
            prevPos = _prevTouchPosDict[touchInfo.id];
		
		if (touchInfo.phase == TouchInfo.Phase.End ||
            touchInfo.phase == TouchInfo.Phase.Cancel ||
            (prevPos - touchInfo.position).sqrMagnitude >= 0.25f)
        {
            _prevTouchPosDict[touchInfo.id] = touchInfo.position;

			foreach(Template3D template in templates)
            {
                _Draw(touchInfo, template);
			}
		}
	}

    protected virtual void _Draw(TouchInfo touchInfo, Template3D template)
    {
        if (!template.painting &&
            touchInfo.phase != TouchInfo.Phase.End &&
            touchInfo.phase != TouchInfo.Phase.Cancel)
        {
            template.BeginPaint();
        }

        _paintedUVs.Clear();

        if (template.painting)
        {
            template.Paint(touchInfo, _brushSize, _brushColor, _paintedUVs);

            if(_paintedUVs.Count > 0) template.FlushPaint();

            if (touchInfo.phase == TouchInfo.Phase.End || touchInfo.phase == TouchInfo.Phase.Cancel)
            {
                _prevTouchPosDict.Remove(touchInfo.id);
            }
        }
    }

	public void Rotate ()
	{
		rotate = true;
	}

	public void RotateStop ()
	{
		rotate = false;
	}


    public void RotateAndType(bool value, AutoRotate.RotateType type = AutoRotate.RotateType.Right)
    {
        if (model != null)
        {
            AutoRotate ar = model.GetComponent<AutoRotate>();
            if (ar == null)
            {
                ar = model.gameObject.AddComponent<AutoRotate>();
                ar.axis = Vector3.up;

                ar.anglePerSecond = 180.0f;

                ar.isLocal = true;
            }
            ar.rotateType = type;

            ar.initialAngle = model.transform.localRotation.eulerAngles.y;
            ar.enabled = _rotate = value;
        }
        else
            Debug.LogWarning("Rotate model is null");
    }
	
	public void BigBrush ()
	{
		brushSize = 24;
	}

	public void MiddleBrush ()
	{
		brushSize = 12;
	}

	public void LowBrush ()
	{
		brushSize = 4;
	}

	public void Black (GameObject pancel)
	{
		SetColor(pancel, new Color32 (0, 0, 0, 255));
	}

	public void Blue (GameObject pancel)
	{
		SetColor(pancel, new Color32 (0, 143, 255, 255));
	}

	public void BlueViolet (GameObject pancel)
	{
		SetColor(pancel, new Color32 (0, 59, 255, 255));
	}

	public void DarkGreen (GameObject pancel)
	{
		SetColor(pancel, new Color32 (70, 161, 85, 255));
	}

	public void Green (GameObject pancel)
	{
		SetColor(pancel, new Color32 (0, 255, 14, 255));
	}

	public void Orange (GameObject pancel)
	{
		SetColor(pancel, new Color32 (234, 143, 0, 255));
	}

	public void Pink (GameObject pancel)
	{
		SetColor(pancel, new Color32 (249, 183, 186, 255));
	}

	public void Red (GameObject pancel)
	{
		SetColor(pancel, new Color32 (255, 0, 0, 255));
	}

	public void Skyblue (GameObject pancel)
	{
		SetColor(pancel, new Color32 (0, 203, 255, 255));
	}

	public void Violet (GameObject pancel)
	{
		SetColor(pancel, new Color32 (126, 18, 223, 255));
	}

	public void VioletRed (GameObject pancel)
	{
		SetColor(pancel, new Color32 (223, 16, 160, 255));
	}

	public void White (GameObject pancel)
	{
		SetColor(pancel, new Color32 (255, 255, 255, 255));
	}

	public void Yellow (GameObject pancel)
	{
		SetColor(pancel, new Color32 (255, 255, 0, 255));
	}

	public void YellowGreen (GameObject pancel)
	{
		SetColor(pancel, new Color32 (213, 255, 0, 255));
	}

    int depthCnt = 0;
    private void SetColor(GameObject pancel, Color color)
    {
        if (pancel != null)
        {
            UISprite pancelSprite = pancel.GetComponentInChildren<UISprite>();
            if (curScale != null)
                curScale = TweenScale.Begin(curScale.gameObject, 0.1f, Vector3.one);
            curScale = TweenScale.Begin(pancelSprite.gameObject, 0.25f, Vector3.one * 1.25f);
            if (depthCnt == 0)
                depthCnt = pancelSprite.depth;
            pancelSprite.depth = ++depthCnt;
        }

        _brushColor = color;
    }
	
	public void SetBrushSizeFromSlider()
	{
		float value = UISlider.current.value;
		brushSize = Mathf.RoundToInt(4.0f + 28.0f * value);
	}
}
