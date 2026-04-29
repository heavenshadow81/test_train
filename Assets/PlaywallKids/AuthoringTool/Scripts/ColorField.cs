using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ColorField : MonoBehaviour {
	public UIWidget selection;

	private float _brightness = 1.0f;
	public float brightness {
		get {
			return _brightness;
		}
        set {
            _brightness = Mathf.Clamp01(value);
        }
	}

	private UITexture _uiTexture;

	private Texture2D _texture;
	public Texture2D texture {
		get {
			if(_texture == null) {
				_texture = new Texture2D(kSize, kSize, TextureFormat.RGBA32, false, true);
				_texture.wrapMode = TextureWrapMode.Clamp;
			}
			return _texture;
		}
	}

	private Color _currentColor = Color.white;
	public Color currentColor {
		get {
			return _currentColor;
		}
	}

	public System.Action<Color> onColorSelected;

	private Color[] _colors;

	private float _x = 0.5f, _y = 0.5f;

	private const int kSize = 256;

	private void _Fill() {
		if(_colors == null) {
			_colors = new Color[kSize * kSize];
		}

		int radius = kSize / 2;
		float r = Mathf.Round((float)radius);

		for(int i = 0; i < kSize; i++) {
			for(int j = 0; j < kSize; j++) {
				float x = (float)(i - radius);
				float y = (float)(j - radius);
				if(r * r >= x * x + y * y) {
					float s = Mathf.Sqrt(x * x + y * y) / r;
					float theta = Mathf.PI * 0.5f;
					if(Mathf.Abs(x) > 0) {
						theta = Mathf.Atan(y / x);
					}
					else {
						if(y >= 0) {
							theta = Mathf.PI * 0.5f;
						}
						else {
							theta = -Mathf.PI * 0.5f;
						}
					}

					if(x < 0) {
						theta += Mathf.PI;
					}
					else if(y < 0) {
						theta += Mathf.PI * 2;
					}

					float h = theta / Mathf.PI * 180.0f;
					float v = brightness;

					// hsv to RGB
					_colors[j * kSize + i] = GetColorFromHSV(h, s, v);
					//_colors[j * kSize + i] = new Color(h / 360.0f, s, v);
				}
				else {
					_colors[j * kSize + i] = new Color(1.0f, 1.0f, 1.0f, 0.0f);
				}
			}
		}		
		_texture.SetPixels(_colors);
		_texture.Apply();
	}

	// Use this for initialization
	void Start () {
		_uiTexture = GetComponent<UITexture>();
		if(_uiTexture != null) {
			_uiTexture.mainTexture = texture;
		}
		_Fill ();
		ColorSelect(0.5f, 0.5f);
	}

	void OnDestroy() {
		if(_texture != null) {
			DestroyImmediate (_texture);
			_texture = null;
		}
		if(_colors != null) {
			_colors = null;
		}
	}

	public Color GetColorFromHSV(float h, float s, float v) {
		float c = v * s;
		float x = c * (1.0f - Mathf.Abs((h / 60.0f) % 2.0f - 1.0f));
		float m = v - c;

		float r1 = 0, g1 = 0, b1 = 0;
		if(h >= 0.0f && h < 60.0f) {
			r1 = c;
			g1 = x;
		}
		else if(h >= 60.0f && h < 120.0f) {
			r1 = x;
			g1 = c;
		}
		else if(h >= 120.0f && h < 180.0f) {
			g1 = c;
			b1 = x;
		}
		else if(h >= 180.0f && h < 240.0f) {
			g1 = x;
			b1 = c;
		}
		else if(h >= 240.0f && h < 300.0f) {
			r1 = x;
			b1 = c;
		}
		else if(h >= 300.0f && h < 360.0f) {
			r1 = c;
			b1 = x;
		}

		return new Color(r1 + m, g1 + m, b1 + m);
	}

	public void Update() {
		if(CustomInput.touchCount > 0) {
			var firstTouch = CustomInput.GetTouch(0);

			Ray ray = UICamera.currentCamera.ScreenPointToRay(firstTouch.position);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)) {
				if(hit.collider.gameObject == gameObject) {
					BoxCollider c = hit.collider as BoxCollider;
					Vector3 center = Vector3.Scale(c.center, transform.lossyScale) + transform.position;
					Vector3 size = Vector3.Scale(c.size, transform.lossyScale);//c.size * transform.lossyScale;
					Vector3 point = hit.point;

					point = new Vector3((point.x - center.x) / size.x, (point.y - center.y) / size.y);
					ColorSelect(point.x + 0.5f, point.y + 0.5f);
				}
			}
		}
	}

	public void ColorSelect(float x, float y) {
		int i = Mathf.RoundToInt(x * kSize);
		int j = Mathf.RoundToInt(y * kSize);

		if(((x - 0.5f) * (x - 0.5f) + (y - 0.5f) * (y - 0.5f)) <= 0.25f) {
			_x = x;
			_y = y;
			_currentColor = _colors[j * kSize + i];

			if(selection != null) {
				selection.color = currentColor;
				BoxCollider c = GetComponent<BoxCollider>();
				Vector3 pos = new Vector3((x - 0.5f) * c.size.y - c.center.x, (y - 0.5f) * c.size.y - c.center.y);
				selection.transform.localPosition = pos;
			}

			if(onColorSelected != null) {
				onColorSelected(currentColor);
			}
		}
	}

	public void SetBrightness() {
		brightness = UISlider.current.value;
		_Fill();
		ColorSelect(_x, _y);
	}
}
