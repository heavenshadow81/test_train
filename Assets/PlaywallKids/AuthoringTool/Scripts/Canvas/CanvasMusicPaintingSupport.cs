using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CanvasMusicPaintingSupport : MonoBehaviour {
    #region Public variables
    public Canvas_ canvas;

    public GameObject musicPaintingParticlePrefab;

    public AudioClip[] sounds;

    public Color[] colorForSounds;
    #endregion

    #region Private variables
    private Dictionary<int, GameObject> _musicPaintingParticlePerTouchDict = new Dictionary<int,GameObject>();
    private Dictionary<int, int> _prevSoundPerTouchDict = new Dictionary<int, int>();
    #endregion

    // Use this for initialization
	void Start () {
        if (canvas == null)
        {
            canvas = GetComponent<Canvas_>();
        }
	}

    void OnDisable() {
        _DisablesAllParticleEmissions();
    }

    void _DisablesAllParticleEmissions() {
        foreach(GameObject go in _musicPaintingParticlePerTouchDict.Values) {
            ParticleSystem particle = go.GetComponent<ParticleSystem>();
            if(particle != null) {
                particle.enableEmission = false;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        _DisablesAllParticleEmissions();

		int touchCount = CustomInput.touchCount;
		TouchInfo[] touches = CustomInput.touches;

		for(int i = 0; i < touchCount; i++) {
			TouchInfo t = touches[i];

			Vector2 canvasPos = canvas.screenToCanvasMatrix * new Vector4(t.axisX, t.axisY, 0, 1.0f);

			Rect rect = new Rect(0, 0, canvas.canvasSize.x, canvas.canvasSize.y);

			if(rect.Contains(canvasPos)) {
                _PlayColorSoundForTouch(t);


                if (!canvas.supportsMultiTouch)
                {
                    break;
                }
			}
		}
	}

    private void _PlayColorSoundForTouch(TouchInfo t)
    {
        Vector3 pos = canvas.ScreenToCanvas(t.position);
        pos = canvas.CanvasToTexture(pos);

        Color c = canvas.GetPixel(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));

        int prevColor = -1;
        if (_prevSoundPerTouchDict.ContainsKey(t.id))
        {
            _prevSoundPerTouchDict.TryGetValue(t.id, out prevColor);
        }

        int currentColor = -1;
        float minVariance = float.MaxValue;
        
        if(c.a >= 0.5f) {
            for (int j = 0; j < sounds.Length; j++)
            {
                Color colorForSound = colorForSounds[j];
                float variance =
                    (Mathf.Pow((colorForSound.r - c.r), 2.0f) +
                    Mathf.Pow((colorForSound.g - c.g), 2.0f) +
                    Mathf.Pow((colorForSound.b - c.b), 2.0f)) * 0.333f;

                if (minVariance > variance)
                {
                    minVariance = variance;
                    currentColor = j;
                }
            }
        }

        bool emit = false;
        if (prevColor != currentColor && currentColor > -1)
        {
            AudioSource.PlayClipAtPoint(sounds[currentColor], UICamera.currentCamera.ScreenToWorldPoint(t.position));

            Debug.Log("Play Sound " + sounds[currentColor].name + ", color : " + colorForSounds[currentColor] + ", at pos : " + pos);

            emit = true;
        }

        _prevSoundPerTouchDict[t.id] = currentColor;

        if(colorForSounds.Length > currentColor)
            _ShowParticleForTouch(t, emit, colorForSounds[currentColor]);
    }

    private void _ShowParticleForTouch(TouchInfo t, bool emit, Color color)
    {
        GameObject go = null;
        if (!_musicPaintingParticlePerTouchDict.ContainsKey(t.id))
        {
            go = (GameObject)Instantiate(musicPaintingParticlePrefab);
            go.transform.parent = transform;
            go.transform.localScale = Vector3.one;

            _musicPaintingParticlePerTouchDict[t.id] = go;
        }

        go = _musicPaintingParticlePerTouchDict[t.id];
        if (go != null)
        {
            go.transform.position = UICamera.currentCamera.ScreenToWorldPoint(t.position) + new Vector3(0, 0, -5f);

            ParticleSystem particleSystem = go.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.enableEmission = true;
                particleSystem.emissionRate = 6.0f;

                if (emit)
                {
                    color.a = 1.0f;
                    particleSystem.startColor = color;
                    particleSystem.Emit(20);
                }
            }
        }
    }
}
