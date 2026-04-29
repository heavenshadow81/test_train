using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ML.PlaywallKids.Common;

public class ModelDrawTestControl : MonoBehaviour
{
    // Template3D, ModelControl
    public Template3D model;
    public SimpleModelControl modelControl;

    // Bone
    public Transform[] bones;

    // Renderer Preview
    public RawImage textureCoordinateMapImage, brushMapImage, colorMapImage;

    // TC Camera
    Vector3 _prev = Vector3.zero;
    TCCamera _tcCam;

    private Dictionary<Transform, float> _initialBoneScales = new Dictionary<Transform, float>();
    private Dictionary<Transform, float> _boneScales = new Dictionary<Transform, float>();

    private int _selectedColor = 0;

    private string[] _colorList = {
        "Red", "Orange", "Yellow", "Green", "Cyan", "Blue", "Pink", "White", "Black"
    };

    private Color randomColor = Color.red;

    private bool _showsMainTexture = true;

    private bool _prefersComptueShaders = false;

    public void Awake()
    {
        SettingsManager.Load();
        _prefersComptueShaders = CommonSettings.prefersComputeShaders;
    }

    public void Start()
    {
        modelControl.model = model.gameObject;
        modelControl.wantsPaint = true;

        _tcCam = TCCamera.sharedInstance;

        textureCoordinateMapImage.texture = _tcCam.tcRT;
        brushMapImage.texture = model.brushTexture;
        colorMapImage.texture = model.GetComponentInChildren<Renderer>().material.mainTexture;

        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i] != null)
            {
                _initialBoneScales[bones[i]] = bones[i].localScale.magnitude / Mathf.Sqrt(3.0f);
                if (bones[i].localScale.x < 0.0f)
                    _initialBoneScales[bones[i]] *= -1;
            }
        }
        _boneScales = new Dictionary<Transform, float>(_initialBoneScales);

        if (Template3D.supportsComputeShaders)
        {
            Debug.Log("ModelDrawTestControl : Template drawing will use GPGPU implementation!");
        }
    }

    public void Update()
    {
        brushMapImage.texture = model.brushTexture;
    }

    public void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(600));
        GUILayout.BeginHorizontal(GUILayout.Height(80));
        GUILayout.Box(Template3D.supportsComputeShaders ? "Painting Mode : GPGPU (DirectCompute)" : "Painting Mode : Software (C#)", GUILayout.Width(350));
        bool prefersComputeShaders = GUILayout.Toggle(_prefersComptueShaders, "GPGPU Mode");
        if (_prefersComptueShaders != prefersComputeShaders)
        {
            _prefersComptueShaders = prefersComputeShaders;
            Template3D.prefersComputeShaders = _prefersComptueShaders;
            TCCamera.prefersComputeShaders = _prefersComptueShaders;
        }
        GUILayout.EndHorizontal();

        foreach (Transform key in _initialBoneScales.Keys)
        {
            float value = _boneScales[key];

            _ShowSlider(key, ref value);
            _boneScales[key] = value;

            key.localScale = Vector3.one * value;
        }

        modelControl.rotate = GUILayout.Toggle(modelControl.rotate, "Rotate");

        if (GUILayout.Button("Reset"))
        {
            foreach (Transform key in _initialBoneScales.Keys)
            {
                _boneScales[key] = _initialBoneScales[key];
                key.localScale = Vector3.one * _boneScales[key];
            }
        }

        int selectedNewColor = GUILayout.SelectionGrid(_selectedColor, _colorList, 9);
        GUILayout.BeginHorizontal();

        GUILayout.Label("Brush Size", GUILayout.Width(80f));

        float newBrushSize = GUILayout.HorizontalSlider(modelControl.brushSize, 4, 32, GUILayout.Width(150f));
        modelControl.brushSize = (int)newBrushSize;
        GUILayout.Label(string.Format("{0:#0}", modelControl.brushSize), GUILayout.Width(50f));
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        if (selectedNewColor != _selectedColor)
        {
            _selectedColor = selectedNewColor;
            switch (_selectedColor)
            {
                case 0:
                    randomColor = Color.red;
                    break;
                case 1:
                    randomColor = new Color(1.0f, 1.0f, 0.5f);
                    break;
                case 2:
                    randomColor = Color.yellow;
                    break;
                case 3:
                    randomColor = Color.green;
                    break;
                case 4:
                    randomColor = Color.cyan;
                    break;
                case 5:
                    randomColor = Color.blue;
                    break;
                case 6:
                    randomColor = Color.magenta;
                    break;
                case 7:
                    randomColor = Color.white;
                    break;
                case 8:
                    randomColor = Color.black;
                    break;
            }
        }

        modelControl.brushColor = randomColor;

        if (GUILayout.Button(_showsMainTexture ? "Hide Main Texture" : "Show Main Texture"))
        {
            _showsMainTexture = !_showsMainTexture;
            if (_showsMainTexture)
            {
                modelControl.template.ShowMainTexture();
            }
            else
            {
                modelControl.template.CleanMainTexture();
            }
        }

        if (GUILayout.Button("Clean Brush Texture"))
        {
            modelControl.template.CleanBrushTexture();
        }
    }

    private void _ShowSlider(Transform target, ref float valIn)
    {
        GUILayout.BeginHorizontal();

        string title = target.name;
        GUILayout.Label(title, GUILayout.Width(120));
        float valOut = GUILayout.HorizontalSlider(valIn, (valIn < 0) ? -.5f : .5f, (valIn < 0) ? -3f : 3f, GUILayout.Width(150f));
        GUILayout.Label(string.Format("{0:#.0#}", valOut), GUILayout.Width(50f));

        if (valIn != valOut)
        {
            _tcCam.RequestRefreshTCRT();
        }

        valIn = valOut;

        GUILayout.EndHorizontal();
    }
}