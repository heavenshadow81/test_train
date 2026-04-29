using UnityEngine;
using System.Collections;

public class SimpleCanvasControl : MonoBehaviour {
	// Public variables
    public Canvas_ canvas;
    public Transform palettes;
	
    // Properties
	private Color32 _brushColor = Color.black;
	public Color32 brushColor {
		get {
			return _brushColor;
		}
		set {
			_brushColor = value;
			if(canvas.brush != null) {
				//Marker ();
				canvas.brush.color = brushColor;
			}
		}
	}

	public void Start() {

		UITexture uiTexture = canvas.GetComponent<UITexture>();
		if(uiTexture != null) {
//			canvas.canvasSize = uiTexture.localSize;
		}

		_InitPalettes();
		brushColor = Color.black;
	}

    private void _InitPalettes()
    {
        for (int i = 0; i < palettes.childCount; i++)
        {
            Transform child = palettes.GetChild(i);
            UIEventTrigger trigger = child.GetComponent<UIEventTrigger>();
            if (trigger == null)
            {
                trigger = child.gameObject.AddComponent<UIEventTrigger>();
            }
            if (trigger.onClick.Count == 0)
            {
                EventDelegate.Set(trigger.onClick, delegate() { this.SendMessage(child.name, child.gameObject); });

                //trigger.onClick.Add(new EventDelegate(this, child.name));
            }
        }
	}
	
	public void Marker() {
		canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameMarker);
		canvas.brush.color = brushColor;
		canvas.brush.diameter = 12.0f;
	}

	public void Crayon() {
		canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameCrayon);
		canvas.brush.color = brushColor;
	}

	public void Airbrush() {
		canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameAirbrush);
		canvas.brush.color = brushColor;
	}

	public void Pastel() {
		canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNamePastel);
		canvas.brush.color = brushColor;
	}

	public void Rainbow() {
		canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameRainbow);
		canvas.brush.color = brushColor;
	}

	public void Eraser() {
		//Marker ();
		//canvas.brush.color = Color.white;
        //canvas.brush.diameter = 60.0f;
		canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameEraser);
		canvas.brush.color = Color.white;
        canvas.brush.diameter = 60.0f;
	}

    public void Black(GameObject pancel)
    {
        SetColor(pancel, new Color32(0, 0, 0, 255));
    }

    public void Blue(GameObject pancel)
    {
        SetColor(pancel, new Color32(0, 143, 255, 255));
    }

    public void BlueViolet(GameObject pancel)
    {
        SetColor(pancel, new Color32(0, 59, 255, 255));
    }

    public void DarkGreen(GameObject pancel)
    {
        SetColor(pancel, new Color32(70, 161, 85, 255));
    }

    public void Green(GameObject pancel)
    {
        SetColor(pancel, new Color32(0, 255, 14, 255));
    }

    public void Orange(GameObject pancel)
    {
        SetColor(pancel, new Color32(234, 143, 0, 255));
    }

    public void Pink(GameObject pancel)
    {
        SetColor(pancel, new Color32(249, 183, 186, 255));
    }

    public void Red(GameObject pancel)
    {
        SetColor(pancel, new Color32(255, 0, 0, 255));
    }

    public void Skyblue(GameObject pancel)
    {
        SetColor(pancel, new Color32(0, 203, 255, 255));
    }

    public void Violet(GameObject pancel)
    {
        SetColor(pancel, new Color32(126, 18, 223, 255));
    }

    public void VioletRed(GameObject pancel)
    {
        SetColor(pancel, new Color32(223, 16, 160, 255));
    }

    public void White(GameObject pancel)
    {
        SetColor(pancel, new Color32(255, 255, 255, 255));
    }

    public void Yellow(GameObject pancel)
    {
        SetColor(pancel, new Color32(255, 255, 0, 255));
    }

    public void YellowGreen(GameObject pancel)
    {
        SetColor(pancel, new Color32(213, 255, 0, 255));
    }

    private TweenScale curScale = null;
    private int depthCnt = 0;
    private void SetColor(GameObject pancel, Color color)
    {
        Debug.Log(pancel);
        if (canvas.brush.brushName == BrushSet.kBrushNameEraser)
            Marker();

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

        brushColor = color;
    }
}
