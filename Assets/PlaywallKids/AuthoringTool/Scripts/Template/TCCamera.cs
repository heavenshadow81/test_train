using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region Structs
public struct TCRTPixelInfo
{
    private Color _color;
    public Color color
    {
        get
        {
            return _color;
        }
        set
        {
            _color = value;

            _ParseColor();
        }
    }

    private byte _templateId;
    public byte templateId
    {
        get
        {
            return _templateId;
        }
    }

    private Vector2 _uv;
    public Vector2 uv
    {
        get
        {
            return _uv;
        }
    }

    public float u
    {
        get
        {
            return _uv.x;
        }
    }

    public float v
    {
        get
        {
            return _uv.y;
        }
    }

    private void _ParseColor()
    {
        Color32 texelColor = _color;
        _templateId = texelColor.r;

        int u = texelColor.g * 16 + (texelColor.b >> 4);
        int v = (texelColor.b % 16) * 256 + texelColor.a;

        // 1 / 2048 = 0.00048828125
        _uv = new Vector2(u * 0.00048828125f, v * 0.00048828125f);
    }

    public TCRTPixelInfo(Color newColor)
    {
        _color = newColor;
        _templateId = 0;
        _uv = Vector2.zero;

        _ParseColor();
    }
}
#endregion

/// <summary>
/// The special camera for rendering texture coordinate map.
/// </summary>
public class TCCamera : MonoBehaviour
{
    #region Public variables
    /// <summary>
    /// The target camera.
    /// </summary>
    public Camera targetCamera;

    /// <summary>
    /// Target render texture for rendering texture coordinate map.
    /// </summary>
    public RenderTexture tcRT;

    /// <summary>
    /// The texture coordinate map shader for rendering.
    /// </summary>
    public Shader tcShader;
    #endregion

    #region Properties
    private static TCCamera __sharedInstance = null;
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <value>The shared instance.</value>
    public static TCCamera sharedInstance
    {
        get
        {
            return __sharedInstance;
        }
    }

    /// <summary>
    /// Gets the aspect ratio of the render texture.
    /// </summary>
    /// <value>The aspect ratio.</value>
    public float aspectRatio
    {
        get
        {
            return (float)tcRT.width / (float)tcRT.height;
        }
    }

    private static bool _prefersComputeShaders = true;
    /// <summary>
    /// You can turn on or off the GPGPU mode. Only NVIDIA or AMD devices will support.
    /// </summary>
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
                Debug.Log("TCCamera.prefersComputeShaders : GPGPU mode is enabled.");
            }
        }
    }

    /// <summary>
    /// This property actually queries that this device supports GPGPU acceleration.
    /// </summary>
    public static bool supportsComputeShaders
    {
        get
        {
            return prefersComputeShaders && SystemInfo.supportsComputeShaders;
        }
    }

    private ComputeShader _computeShader;
    /// <summary>
    /// The compute shader used in TCCamera. Dynamically loaded in Resources.
    /// </summary>
    public ComputeShader computeShader
    {
        get
        {
            if (_computeShader == null)
            {
                _computeShader = (ComputeShader)Resources.Load("ComputeShaders/TCCamera");
            }
            return _computeShader;
        }
    }
    #endregion

    #region Private variables
    /// <summary>
    /// Registered templates dictionary. Each template is being refered using key(1~255). 
    /// </summary>
    private Dictionary<byte, Template3D> _templateDict = new Dictionary<byte, Template3D>();

    /// <summary>
    /// Copied texture from render texture in which other objects refer colors.
    /// </summary>
    private Texture2D _tcRTBackup = null;

    /// <summary>
    /// Cached color map.
    /// </summary>
    private Color[] _tcRTColor = null;

    /// <summary>
    /// Should camera refresh render texture?
    /// </summary>
    private bool _requiresRefreshTCRT = false;

    /// <summary>
    /// Cached template layer IDs. Used in PerformDrawTCCamera().
    /// </summary>
    private List<int> _templateLayerIDs = new List<int>();

    /// <summary>
    /// Cached template shader list. Used in PerformDrawTCCamera().
    /// </summary>
    private List<Shader> _templateShaders = new List<Shader>();

    private ComputeBuffer _computePixelsBuffer;

    private bool _capturedPixels = false;
    #endregion

    #region Unity methods
    void Awake()
    {
        __sharedInstance = this;
    }

    void OnDestroy()
    {
        __sharedInstance = null;
        if (_computePixelsBuffer != null)
            _computePixelsBuffer.Release();
    }

    public void Start()
    {
        if (tcRT == null && targetCamera != null)
        {
            tcRT = targetCamera.targetTexture;

            if (tcRT == null)
            {
                tcRT = new RenderTexture(1024, 512, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
                targetCamera.targetTexture = tcRT;
            }
        }

        _Initialize();
        PerformDrawTCCamera();
        RequestRefreshTCRT();
    }

    private void _Initialize()
    {
        if (supportsComputeShaders)
        {
            if (_tcRTBackup != null)
            {
                Destroy(_tcRTBackup);
                _tcRTBackup = null;
            }
        }
        else
        {
            if (_tcRTBackup == null)
            {
                _tcRTBackup = new Texture2D(tcRT.width, tcRT.height, TextureFormat.ARGB32, false, false);
            }
        }
    }

    public void Update()
    {
        if (_requiresRefreshTCRT)
        {
            _requiresRefreshTCRT = false;

            // Initialize
            _Initialize();

            // Draw
            PerformDrawTCCamera();

            // Caches texture color!
            if (supportsComputeShaders)
            {
                if (computeShader != null)
                {
                    if (_computePixelsBuffer == null || _computePixelsBuffer.count != tcRT.width * tcRT.height)
                    {
                        if (_computePixelsBuffer != null) _computePixelsBuffer.Release();
                        _computePixelsBuffer = new ComputeBuffer(tcRT.width * tcRT.height, sizeof(float) * 4);
                    }

                    int kernel = computeShader.FindKernel("TCCameraReadPixels");
                    int numThreads = 16;
                    computeShader.SetTexture(kernel, "TCMap", tcRT);
                    computeShader.SetBuffer(kernel, "Result", _computePixelsBuffer);
                    computeShader.SetInt("width", tcRT.width);
                    computeShader.Dispatch(kernel, tcRT.width / numThreads, tcRT.height / numThreads, 1);
                }
            }
            else
            {
                RenderTexture prev = RenderTexture.active;
                RenderTexture.active = tcRT;
                _tcRTBackup.ReadPixels(new Rect(0, 0, tcRT.width, tcRT.height), 0, 0);
                _tcRTBackup.Apply();
                RenderTexture.active = prev;
            }

            _capturedPixels = false;
        }
    }
    #endregion

    #region Template management
    /// <summary>
    /// Registers the template to the camera. This method will be called by the template automatically.
    /// </summary>
    /// <param name="template">Template object.</param>
    public void RegisterTemplate(Template3D template)
    {
        if (template != null && !_templateDict.ContainsValue(template))
        {
            // find the empty slot.
            for (byte i = 1; i <= byte.MaxValue; i++)
            {
                if (_templateDict.ContainsKey(i) == false)
                {
                    _templateDict[i] = template;
                    template.identifierForTCCamera = i;
                    break;
                }
            }

            // refresh
            RequestRefreshTCRT();
        }
    }

    /// <summary>
    /// Unregisters the template. This method will be called by the template automatically.
    /// </summary>
    /// <param name="template">Template.</param>
    public void UnregisterTemplate(Template3D template)
    {
        if (template != null)
        {
            // 과일 먹을때 identifierForTCCamera 아이디 값이 꼬이므로 포함하고 있는지 체크
            // 과일이 이미 딕셔너리에 빠져있는데 또 제거 요청이 들어오면서 과일이 사라지고 그다음 
            // 인덱스 값으로 잡은 오브젝트의 템플릿이 제거되는 버그
            if (_templateDict.ContainsValue(template))
                _templateDict.Remove(template.identifierForTCCamera);
            RequestRefreshTCRT();
        }
    }
    #endregion

    #region Rendering
    /// <summary>
    /// Renders texture coordinate map.
    /// </summary>
    public void PerformDrawTCCamera()
    {
        // In order to use the render texture's all area, We need to adjust the aspect ratio as width/height.
        targetCamera.aspect = (float)Screen.width / (float)Screen.height;

        // Clear cached lists
        _templateLayerIDs.Clear();
        _templateShaders.Clear();

        List<byte> keys = new List<byte>(_templateDict.Keys);

        // Renders the blank, black-colored screen.
        targetCamera.clearFlags = CameraClearFlags.SolidColor;
        targetCamera.backgroundColor = Color.black;
        targetCamera.Render();

        // Changes the clear flags
        targetCamera.clearFlags = CameraClearFlags.Depth;

        // Draws all templates
        for (int i = 0; i < keys.Count; i++)
        {
            byte key = keys[i];
            Template3D template = _templateDict[key];

            if (template == null || template.gameObject == null)
            {
                _templateDict.Remove(key);
                keys.RemoveAt(i--);
                continue;
            }

            // Saves current template's layer and set "TCCamera".
            Transform[] tfs = template.GetComponentsInChildren<Transform>();
            for (int ti = 0, tcnt = tfs.Length; ti < tcnt; ti++)
            {
                GameObject go = tfs[ti].gameObject;
                _templateLayerIDs.Add(tfs[ti].gameObject.layer);
                go.layer = LayerMask.NameToLayer("TCCamera");
            }

            Renderer[] renderers = template.GetComponentsInChildren<Renderer>();
            for (int ri = 0; ri < renderers.Length; ri++)
            {
                Renderer renderer = renderers[ri];
                if (renderer != null)
                {
                    for (int mi = 0; mi < renderer.materials.Length; mi++)
                    {
                        Material mat = renderer.materials[mi];
                        if (mat != null)
                        {
                            Shader shader = mat.shader;
                            _templateShaders.Add(shader);
                            mat.shader = tcShader;
                            if (mat.HasProperty("_Id")) mat.SetInt("_Id", template.identifierForTCCamera);
                        }
                    }
                }
            }
        }

        targetCamera.Render();

        // Set current template's ID.
        //Shader.SetGlobalFloat("_Id", template.identifierForTCCamera);

        // Render!
        //targetCamera.RenderWithShader(tcShader, null);
        //targetCamera.Render();

        int templateLayerIDsIndex = 0, templateShadersIndex = 0;
        for (int i = 0; i < keys.Count; i++)
        {
            byte key = keys[i];
            Template3D template = _templateDict[key];
            Transform[] tfs = template.GetComponentsInChildren<Transform>();

            // Reverts current template's layer.
            for (int ti = 0, tcnt = tfs.Length; ti < tcnt; ti++)
            {
                GameObject go = tfs[ti].gameObject;
                go.layer = _templateLayerIDs[templateLayerIDsIndex++];
            }

            Renderer[] renderers = template.GetComponentsInChildren<Renderer>();
            for (int ri = 0; ri < renderers.Length; ri++)
            {
                Renderer renderer = renderers[ri];
                if (renderer != null)
                {
                    for (int mi = 0; mi < renderer.materials.Length; mi++)
                    {
                        Material mat = renderer.materials[mi];
                        if (mat != null)
                        {
                            Shader shader = _templateShaders[templateShadersIndex++];
                            mat.shader = shader;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the pixel.
    /// </summary>
    /// <returns>The pixel color value.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public Color GetPixel(int x, int y)
    {
        if (!_capturedPixels)
        {
            _capturedPixels = true;

            if (supportsComputeShaders && _computePixelsBuffer != null)
            {
                if (_tcRTColor == null || _tcRTColor.Length != tcRT.width * tcRT.height)
                {
                    _tcRTColor = new Color[tcRT.width * tcRT.height];
                }
                _computePixelsBuffer.GetData(_tcRTColor);
            }
            else
            {
                _tcRTColor = _tcRTBackup.GetPixels();
            }
        }

        return _tcRTColor[Mathf.Min(_tcRTColor.Length - 1, Mathf.Max(0, y) * tcRT.width + Mathf.Max(x, 0))];
    }

    // Convert screen space point to TCCamera render texture point.
    public Vector3 ScreenToTCRT(Vector3 position)
    {
        Vector3 point = ScreenToTCRTNormalized(position);

        // Multiply by render texture width and height.
        point.x = Mathf.Floor(point.x * tcRT.width);
        point.y = Mathf.Floor(point.y * tcRT.height);

        return point;
    }

    public Vector3 ScreenToTCRTNormalized(Vector3 position)
    {
        Vector3 point = Vector3.zero;

        if (tcRT != null)
        {
            // Get normalized position in TCRT space.
            float widthNormalized = position.x / (float)Screen.width;
            float heightNormalized = position.y / (float)Screen.height;

            // -- legacy code
            // float widthNormalized = (position.x - 0.5f * (Screen.width - Screen.height * aspectRatio)) / (Screen.height * aspectRatio);
            // float heightNormalized = position.y / (float)Screen.height;

            point.x = widthNormalized;
            point.y = heightNormalized;
        }

        return point;
    }

    public TCRTPixelInfo GetTCRTPixelInfo(float x, float y)
    {
        return GetTCRTPixelInfo(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    }

    public TCRTPixelInfo GetTCRTPixelInfo(Vector2 point)
    {
        return GetTCRTPixelInfo(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y));
    }

    public TCRTPixelInfo GetTCRTPixelInfo(int x, int y)
    {
        Color color = GetPixel(x, y);

        TCRTPixelInfo info = new TCRTPixelInfo(color);

        return info;
    }

    /// <summary>
    /// Requests texture coordinate map render texture refreshing.
    /// </summary>
    public void RequestRefreshTCRT()
    {
        _requiresRefreshTCRT = true;
    }
    #endregion
}
