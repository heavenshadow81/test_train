using UnityEngine;
using System.Collections.Generic;

public class Ocean : MonoBehaviour
{
    public int width = 32;
    public int height = 32;

    public float scale = 0.1f;

    public Vector3 size = new Vector3(150.0f, 1.0f, 150.0f);

    public int tiles_x = 2;
    public int tiles_y = 2;

    public float windx = 10.0f;

    public float normal_scale = 16;
    public float normalStrength = 1.0f;

    public float choppy_scale = 2.0f;
    public float uv_speed = 0.01f;

    public Material material;

    public bool followMainCamera = true;

    public bool showGUI = true;

    private int max_LOD = 4;
    private ComplexF[] h0;

    private ComplexF[] t_x;
    private ComplexF[] t_y;

    private ComplexF[] n0;
    private ComplexF[] n_x;
    private ComplexF[] n_y;

    private ComplexF[] data;
    private ComplexF[] data_x;

    private Color[] pixelData;
    private Texture2D textureA;
    private Texture2D textureB;

    private Vector3[] baseHeight;
    private Vector2[] baseUV;

    private Mesh baseMesh = null;

    private GameObject child;

    private List<Mesh>[] tiles_LOD;

    private int g_height;
    private int g_width;

    private int n_width;
    private int n_height;

    private bool drawFrame = true;

    private bool normalDone = false;

    private bool reflectionRefractionEnabled = false;
    private Camera depthCam = null;
    private Camera offscreenCam = null;
    private RenderTexture reflectionTexture = null;
    private RenderTexture refractionTexture = null;
    private RenderTexture waterHeightTexture = null;
    private RenderTexture underwaterTexture = null;

    private RenderTexture underwaterRefractionTexture = null;

    private Shader shader = null;
    private Shader depthShader = null;
    private Shader waterBelowShader = null;
    private Material waterCompositionMaterial = null;

    private float waterDirtyness = 0.016f;



    private Vector2[] uvs;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector4[] tangents;

    public bool forceOriginalShader = false;

    public Light SunLight = null;

    public Color surfaceColor = new Color(0.3f, 0.5f, 0.3f, 1.0f);
    public Color waterColor = new Color(0.3f, 0.4f, 0.3f);

    private Texture2D texFoam = null;
    private Texture2D texFresnel = null;
    private Texture2D texBump = null;

    private bool renderReflection = true;
    private bool renderRefraction = true;
    private bool renderWaterDepth = true;
    private bool renderUnderwater = true;
    private bool renderUnderwaterRefraction = true;

    private bool useCameraRenderTexture = false;

    private RenderTexture cameraRenderTexture = null;

    public float GetWaterHeightAtLocation(float x, float y)
    {
        x = x / size.x;
        x = (x - Mathf.Floor(x)) * width;
        y = y / size.z;
        y = (y - Mathf.Floor(y)) * height;

        return data[width * Mathf.FloorToInt(y) + Mathf.FloorToInt(x)].Re * scale / (width * height);
    }

    float GaussianRnd()
    {
        var x1 = Random.value;
        var x2 = Random.value;

        if (x1 == 0.0f)
            x1 = 0.01f;

        return Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Cos(2.0f * Mathf.PI * x2);
    }

    // Phillips spectrum
    float P_spectrum(Vector2 vec_k, Vector2 wind)
    {
        float A = vec_k.x > 0.0f ? 1.0f : 0.05f; // Set wind to blow only in one direction - otherwise we get turmoiling water

        float L = wind.sqrMagnitude / 9.81f;
        float k2 = vec_k.sqrMagnitude;
        // Avoid division by zero
        if (vec_k.magnitude == 0.0f)
        {
            return 0.0f;
        }
        return A * Mathf.Exp(-1.0f / (k2 * L * L) - Mathf.Pow(vec_k.magnitude * 0.1f, 2.0f)) / (k2 * k2) * Mathf.Pow(Vector2.Dot(vec_k / vec_k.magnitude, wind / wind.magnitude), 2.0f);// * wind_x * wind_y;
    }

    void Start()
    {
        cameraRenderTexture = new RenderTexture(2048, 2048, 24);

        // normal map size
        n_width = 128;
        n_height = 128;

        textureA = new Texture2D(n_width, n_height);
        textureB = new Texture2D(n_width, n_height);
        textureA.filterMode = FilterMode.Bilinear;
        textureB.filterMode = FilterMode.Bilinear;

        if (!SetupOffscreenRendering())
        {
            material.SetTexture("_BumpMap", textureA);
            material.SetTextureScale("_BumpMap", new Vector2(normal_scale, normal_scale));

            material.SetTexture("_BumpMap2", textureB);
            material.SetTextureScale("_BumpMap2", new Vector2(normal_scale, normal_scale));
        }

        pixelData = new Color[n_width * n_height];

        // Init the water height matrix
        data = new ComplexF[width * height];
        // lateral offset matrix to get the choppy waves
        data_x = new ComplexF[width * height];

        // tangent
        t_x = new ComplexF[width * height];
        t_y = new ComplexF[width * height];

        n_x = new ComplexF[n_width * n_height];
        n_y = new ComplexF[n_width * n_height];

        // Geometry size
        g_height = height + 1;
        g_width = width + 1;

        tiles_LOD = new List<Mesh>[max_LOD];
        for (var LOD = 0; LOD < max_LOD; LOD++)
        {
            tiles_LOD[LOD] = new List<Mesh>();
        }

        GameObject tile = null;
        int chDist = 0; // Chebychev distance
        for (int y = 0; y < tiles_y; y++)
        {
            for (int x = 0; x < tiles_x; x++)
            {
                chDist = Mathf.Max(Mathf.Abs(tiles_y / 2 - y), Mathf.Abs(tiles_x / 2 - x));
                chDist = chDist > 0 ? chDist - 1 : 0;
                int cy = y - tiles_y / 2;
                int cx = x - tiles_x / 2;
                tile = new GameObject("WaterTile" + chDist);
                tile.transform.position = new Vector3(cx * size.x, -2.0f * chDist, cy * size.z);
                tile.AddComponent<MeshFilter>();
                tile.AddComponent<MeshRenderer>();
                tile.GetComponent<Renderer>().material = material;
                tile.GetComponent<Renderer>().receiveShadows = false;
                tile.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                //Make child of this object, so we don't clutter up the
                //scene hierarchy more than necessary.
                tile.transform.parent = transform;

                //Also we don't want these to be drawn while doing refraction/reflection passes,
                //so we'll add the to the water layer for easy filtering.
                tile.layer = LayerMask.NameToLayer("Water");

                // Determine which LOD the tile belongs
                tiles_LOD[chDist].Add(tile.GetComponent<MeshFilter>().mesh);
            }
        }


        // Init wave spectra. One for vertex offset and another for normal map
        h0 = new ComplexF[width * height];
        n0 = new ComplexF[n_width * n_height];

        // Wind restricted to one direction, reduces calculations
        var wind = new Vector2(windx, 0.0f);

        // Initialize wave generator	
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int yc = y < height / 2 ? y : -height + y;
                int xc = x < width / 2 ? x : -width + x;
                Vector2 vec_k = new Vector2(2.0f * Mathf.PI * xc / size.x, 2.0f * Mathf.PI * yc / size.z);
                h0[width * y + x] = new ComplexF(GaussianRnd(), GaussianRnd()) * 0.707f * Mathf.Sqrt(P_spectrum(vec_k, wind));
            }
        }

        for (int y = 0; y < n_height; y++)
        {
            for (int x = 0; x < n_width; x++)
            {
                int yc = y < n_height / 2 ? y : -n_height + y;
                int xc = x < n_width / 2 ? x : -n_width + x;
                Vector2 vec_k = new Vector2(2.0f * Mathf.PI * xc / (size.x / normal_scale), 2.0f * Mathf.PI * yc / (size.z / normal_scale));
                n0[n_width * y + x] = new ComplexF(GaussianRnd(), GaussianRnd()) * 0.707f * Mathf.Sqrt(P_spectrum(vec_k, wind));
            }
        }
        GenerateHeightmap();
        GenerateBumpmaps();
    }
    void GenerateBumpmaps()
    {
        if (!normalDone)
        {
            for (int idx = 0; idx < 2; idx++)
            {
                for (int y = 0; y < n_height; y++)
                {
                    for (int x = 0; x < n_width; x++)
                    {
                        int yc = y < n_height / 2 ? y : -n_height + y;
                        int xc = x < n_width / 2 ? x : -n_width + x;
                        Vector2 vec_k = new Vector2(2.0f * Mathf.PI * xc / (size.x / normal_scale), 2.0f * Mathf.PI * yc / (size.z / normal_scale));

                        float iwkt = idx == 0 ? 0.0f : Mathf.PI / 2;
                        ComplexF coeffA = new ComplexF(Mathf.Cos(iwkt), Mathf.Sin(iwkt));
                        ComplexF coeffB = coeffA.GetConjugate();

                        int ny = y > 0 ? n_height - y : 0;
                        int nx = x > 0 ? n_width - x : 0;

                        n_x[n_width * y + x] = (n0[n_width * y + x] * coeffA + n0[n_width * ny + nx].GetConjugate() * coeffB) * new ComplexF(0.0f, -vec_k.x);
                        n_y[n_width * y + x] = (n0[n_width * y + x] * coeffA + n0[n_width * ny + nx].GetConjugate() * coeffB) * new ComplexF(0.0f, -vec_k.y);
                    }
                }
                Fourier.FFT2(n_x, n_width, n_height, FourierDirection.Backward);
                Fourier.FFT2(n_y, n_width, n_height, FourierDirection.Backward);

                for (int i = 0; i < n_width * n_height; i++)
                {
                    Vector3 bump = new Vector3(n_x[i].Re * Mathf.Abs(n_x[i].Re), n_y[i].Re * Mathf.Abs(n_y[i].Re), n_width * n_height / scale / normal_scale * normalStrength).normalized * 0.5f;
                    pixelData[i] = new Color(bump.x + 0.5f, bump.y + 0.5f, bump.z + 0.5f);
                    //			pixelData[i] = Color (0.5, 0.5, 1.0);			
                }
                if (idx == 0)
                {
                    textureA.SetPixels(pixelData, 0);
                    textureA.Apply();
                }
                else
                {
                    textureB.SetPixels(pixelData, 0);
                    textureB.Apply();
                }
            }
            normalDone = true;
        }

    }
    void GenerateHeightmap()
    {
        Mesh mesh = new Mesh();

        int y = 0;
        int x = 0;

        // Build vertices and UVs
        Vector3[] vertices = new Vector3[g_height * g_width];
        Vector4[] tangents = new Vector4[g_height * g_width];
        Vector2[] uv = new Vector2[g_height * g_width];

        Vector2 uvScale = new Vector2(1.0f / (g_width - 1), 1.0f / (g_height - 1));
        Vector3 sizeScale = new Vector3(size.x / (g_width - 1), size.y, size.z / (g_height - 1));

        for (y = 0; y < g_height; y++)
        {
            for (x = 0; x < g_width; x++)
            {
                Vector3 vertex = new Vector3(x, 0.0f, y);
                vertices[y * g_width + x] = Vector3.Scale(sizeScale, vertex);
                uv[y * g_width + x] = Vector2.Scale(new Vector2(x, y), uvScale);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;

        for (y = 0; y < g_height; y++)
        {
            for (x = 0; x < g_width; x++)
            {
                tangents[y * g_width + x] = new Vector4(1.0f, 0.0f, 0.0f, -1.0f);
            }
        }
        mesh.tangents = tangents;

        for (var LOD = 0; LOD < max_LOD; LOD++)
        {
            var verticesLOD = new Vector3[(height / Mathf.FloorToInt(Mathf.Pow(2, LOD)) + 1) * (width / Mathf.FloorToInt(Mathf.Pow(2, LOD)) + 1)];
            var uvLOD = new Vector2[(height / Mathf.FloorToInt(Mathf.Pow(2, LOD)) + 1) * (width / Mathf.FloorToInt(Mathf.Pow(2, LOD)) + 1)];
            int idx = 0;

            for (y = 0; y < g_height; y += Mathf.FloorToInt(Mathf.Pow(2, LOD)))
            {
                for (x = 0; x < g_width; x += Mathf.FloorToInt(Mathf.Pow(2, LOD)))
                {
                    verticesLOD[idx] = vertices[g_width * y + x];
                    uvLOD[idx++] = uv[g_width * y + x];
                }
            }
            for (int k = 0; k < tiles_LOD[LOD].Count; k++)
            {
                Mesh meshLOD = tiles_LOD[LOD][k];
                meshLOD.vertices = verticesLOD;
                meshLOD.uv = uvLOD;
            }
        }

        // Build triangle indices: 3 indices into vertex array for each triangle
        for (int LOD = 0; LOD < max_LOD; LOD++)
        {
            int index = 0;
            int width_LOD = width / Mathf.FloorToInt(Mathf.Pow(2, LOD)) + 1;
            int[] triangles = new int[(height / Mathf.FloorToInt(Mathf.Pow(2, LOD)) * width / Mathf.FloorToInt(Mathf.Pow(2, LOD))) * 6];
            for (y = 0; y < height / Mathf.Pow(2, LOD); y++)
            {
                for (x = 0; x < width / Mathf.Pow(2, LOD); x++)
                {
                    // For each grid cell output two triangles
                    triangles[index++] = (y * width_LOD) + x;
                    triangles[index++] = ((y + 1) * width_LOD) + x;
                    triangles[index++] = (y * width_LOD) + x + 1;

                    triangles[index++] = ((y + 1) * width_LOD) + x;
                    triangles[index++] = ((y + 1) * width_LOD) + x + 1;
                    triangles[index++] = (y * width_LOD) + x + 1;
                }
            }
            for (int k = 0; k < tiles_LOD[LOD].Count; k++)
            {
                Mesh meshLOD = tiles_LOD[LOD][k] as Mesh;
                meshLOD.triangles = triangles;
            }
        }

        baseMesh = mesh;
    }

    /*
    Prepares the scene for offscreen rendering; spawns a camera we'll use for for
    temporary renderbuffers as well as the offscreen renderbuffers (one for
    reflection and one for refraction).
    */
    bool SetupOffscreenRendering()
    {
        if (forceOriginalShader)
            return false;

        shader = Shader.Find("OceanReflectionRefraction");

        //Bail out if the shader could not be compiled
        if (shader == null)
            return false;

        if (!shader.isSupported)
            return false;


        depthShader = Shader.Find("WaterHeight");
        waterCompositionMaterial = new Material(Shader.Find("WaterComposition"));
        waterBelowShader = Shader.Find("DeepWaterBelow");

        //TODO: More fail-tests?
        underwaterTexture = new RenderTexture(512, 512, 16);
        underwaterTexture.wrapMode = TextureWrapMode.Clamp;
        underwaterTexture.isPowerOfTwo = true;

        reflectionTexture = new RenderTexture(512, 512, 16);
        refractionTexture = new RenderTexture(512, 512, 16);

        reflectionTexture.wrapMode = TextureWrapMode.Clamp;
        refractionTexture.wrapMode = TextureWrapMode.Clamp;

        reflectionTexture.isPowerOfTwo = true;
        refractionTexture.isPowerOfTwo = true;


        underwaterRefractionTexture = new RenderTexture(256, 256, 16);
        underwaterRefractionTexture.wrapMode = TextureWrapMode.Clamp;
        underwaterRefractionTexture.isPowerOfTwo = true;

        waterHeightTexture = new RenderTexture(128, 128, 16);
        waterHeightTexture.wrapMode = TextureWrapMode.Clamp;
        waterHeightTexture.isPowerOfTwo = true;
        //waterHeightTexture.format = RenderTextureFormat.Depth;

        //Spawn the camera we'll use for offscreen rendering (refraction/reflection)
        GameObject cam = new GameObject();
        cam.name = "DeepWaterOffscreenCam";
        cam.transform.parent = transform;
        offscreenCam = cam.AddComponent(typeof(Camera)) as Camera;
        offscreenCam.enabled = false;


        cam = new GameObject();
        cam.name = "DeepWaterFoamOffscreenCam";
        cam.transform.parent = transform;
        depthCam = cam.AddComponent(typeof(Camera)) as Camera;
        depthCam.enabled = false;


        //Hack to make this object considered by the renderer - first make a plane
        //covering the watertiles so we get a decent bounding box, then
        //scale all the vertices to 0 to make it invisible.
        gameObject.AddComponent<MeshRenderer>();

        GetComponent<Renderer>().material.renderQueue = 1001;
        GetComponent<Renderer>().receiveShadows = false;
        GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        Mesh m = new Mesh();

        var verts = new Vector3[4];
        var uv = new Vector2[4];
        var n = new Vector3[4];
        var tris = new int[6];

        float minSizeX = -1024;
        float maxSizeX = 1024;

        float minSizeY = -1024;
        float maxSizeY = 1024;

        verts[0] = new Vector3(minSizeX, 0.0f, maxSizeY);
        verts[1] = new Vector3(maxSizeX, 0.0f, maxSizeY);
        verts[2] = new Vector3(maxSizeX, 0.0f, minSizeY);
        verts[3] = new Vector3(minSizeX, 0.0f, minSizeY);

        tris[0] = 0;
        tris[1] = 1;
        tris[2] = 2;

        tris[3] = 2;
        tris[4] = 3;
        tris[5] = 0;

        m.vertices = verts;
        m.uv = uv;
        m.normals = n;
        m.triangles = tris;


        MeshFilter mfilter = gameObject.GetComponent<MeshFilter>();

        if (mfilter == null)
            mfilter = gameObject.AddComponent<MeshFilter>();

        mfilter.mesh = m;

        m.RecalculateBounds();

        //Hopefully the bounds will not be recalculated automatically
        verts[0] = Vector3.zero;
        verts[1] = Vector3.zero;
        verts[2] = Vector3.zero;
        verts[3] = Vector3.zero;

        m.vertices = verts;

        //Create the material and set up the texture references.
        material = new Material(shader);

        texBump = Resources.Load<Texture2D>("Ocean/Bump");
        texFresnel = Resources.Load<Texture2D>("Ocean/Fresnel");
        texFoam = Resources.Load<Texture2D>("Ocean/Foam");

        material.SetTexture("_Reflection", reflectionTexture);
        material.SetTexture("_Refraction", refractionTexture);
        material.SetTexture("_Bump", texBump);
        material.SetTexture("_Fresnel", texFresnel);
        material.SetTexture("_Foam", texFoam);
        material.SetVector("_Size", new Vector4(size.x, size.y, size.z, 0.0f));

        material.SetColor("_SurfaceColor", surfaceColor);
        material.SetColor("_WaterColor", waterColor);

        waterCompositionMaterial.SetColor("_WaterColor", waterColor);
        waterCompositionMaterial.SetTexture("_DepthTex", waterHeightTexture);
        waterCompositionMaterial.SetTexture("_UnderwaterTex", underwaterTexture);
        waterCompositionMaterial.SetTexture("_UnderwaterDistortionTex", texBump);


        if (SunLight != null)
            material.SetVector("_SunDir", transform.TransformDirection(SunLight.transform.forward));

        reflectionRefractionEnabled = true;

        return true;
    }

    /*
    Delete the offscreen rendertextures on script shutdown.
    */
    void OnDisable()
    {
        if (reflectionTexture != null)
            DestroyImmediate(reflectionTexture);

        if (refractionTexture != null)
            DestroyImmediate(refractionTexture);

        reflectionTexture = null;
        refractionTexture = null;

        if (waterHeightTexture != null)
            DestroyImmediate(waterHeightTexture);

        if (underwaterTexture != null)
            DestroyImmediate(underwaterTexture);

        if (underwaterRefractionTexture)
            DestroyImmediate(underwaterRefractionTexture);

        waterHeightTexture = null;
        underwaterTexture = null;
        underwaterRefractionTexture = null;
    }

    // Wave dispersion
    float disp(Vector2 vec_k)
    {
        return Mathf.Sqrt(9.81f * vec_k.magnitude);
    }

    void OnGUI()
    {
        if (!showGUI)
            return;

        if (useCameraRenderTexture)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), cameraRenderTexture, ScaleMode.StretchToFill, false);


        //For debugging shaders, it can be handy to display the output. Not so cool for actual demos,
        //though.


        /*	if (reflectionTexture != null)
                GUI.DrawTexture (new Rect(0, 0, 128, 128), reflectionTexture, ScaleMode.StretchToFill, false);

            if (refractionTexture != null)
                GUI.DrawTexture (new Rect(0, 128, 128, 128), refractionTexture, ScaleMode.StretchToFill, false);

            if (underwaterRefractionTexture != null)
                GUI.DrawTexture (new Rect(128, 0, 128, 128), underwaterRefractionTexture, ScaleMode.StretchToFill, false);
        */

        /*		
            GUI.DrawTexture (new Rect(128, 0, 128, 128), textureA, ScaleMode.StretchToFill, false);
            GUI.DrawTexture (new Rect(128, 128, 128, 128), textureB, ScaleMode.StretchToFill, false);
        */

        //	GUI.DrawTexture (new Rect(0, 0, 128, 128), waterHeightTexture, ScaleMode.StretchToFill, false);
        //	GUI.DrawTexture (new Rect(0, 128, 128, 128), underwaterTexture, ScaleMode.StretchToFill, false);

        /*
        GUI.Label(new Rect(10, 10, 100, 20), "Waveheight");
        scale = GUI.HorizontalSlider(new Rect(120, 10, 200, 20), scale, 0.05, 3.0);

        GUI.Label(new Rect(10, 30, 100, 20), "Wave sharpness");
        choppy_scale = GUI.HorizontalSlider(new Rect(120, 30, 200, 20), choppy_scale, 0.00, 10.0);

        GUI.Label(new Rect(10, 50, 100, 20), "Water dirtyness");
        waterDirtyness = GUI.HorizontalSlider(new Rect(120, 50, 200, 20), waterDirtyness, 0.0, 0.1);

        GUI.Label(new Rect(10, 70, 300, 20), "Campos: " + Camera.main.gameObject.transform.position);

        renderReflection = GUI.Toggle(new Rect(10, 90, 200, 20), renderReflection, "Render reflection");
        renderRefraction = GUI.Toggle(new Rect(10, 110, 200, 20), renderRefraction, "Render refraction");
        renderWaterDepth = GUI.Toggle(new Rect(10, 130, 200, 20), renderWaterDepth, "Render water depth");
        renderUnderwater = GUI.Toggle(new Rect(10, 150, 200, 20), renderUnderwater, "Render underwater");
        renderUnderwaterRefraction = GUI.Toggle(new Rect(10, 170, 200, 20), renderUnderwaterRefraction, "Render underwater refraction");

        useCameraRenderTexture = GUI.Toggle(new Rect(10, 190, 200, 20), useCameraRenderTexture, "Use camera render target");
        */
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //	Application.CaptureScreenshot("Screenshot.png");

        if (useCameraRenderTexture)
            Camera.main.targetTexture = cameraRenderTexture;
        else
            Camera.main.targetTexture = null;


        if (followMainCamera)
        {
            Vector3 campos = Camera.main.gameObject.transform.position;
            Vector3 centerOffset = campos;


            centerOffset.x = Mathf.Floor(campos.x / size.x) * size.x;
            centerOffset.z = Mathf.Floor(campos.z / size.z) * size.z;
            centerOffset.y = transform.position.y;

            transform.position = centerOffset;
        }


        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                int idx = width * y + x;
                int yc = y < height / 2 ? y : -height + y;
                int xc = x < width / 2 ? x : -width + x;
                Vector2 vec_k = new Vector2(2.0f * Mathf.PI * xc / size.x, 2.0f * Mathf.PI * yc / size.z);

                var iwkt = disp(vec_k) * Time.time;
                var coeffA = new ComplexF(Mathf.Cos(iwkt), Mathf.Sin(iwkt));
                var coeffB = coeffA.GetConjugate();

                int ny = y > 0 ? height - y : 0;
                int nx = x > 0 ? width - x : 0;

                data[idx] = h0[idx] * coeffA + h0[width * ny + nx].GetConjugate() * coeffB;
                t_x[idx] = data[idx] * new ComplexF(0.0f, vec_k.x) - data[idx] * vec_k.y;

                // Choppy wave calcuations
                if (x + y > 0)
                    data[idx] += data[idx] * vec_k.x / vec_k.magnitude;
            }
        }
        material.SetFloat("_BlendA", Mathf.Cos(Time.time));
        material.SetFloat("_BlendB", Mathf.Sin(Time.time));

        Fourier.FFT2(data, width, height, FourierDirection.Backward);
        Fourier.FFT2(t_x, width, height, FourierDirection.Backward);

        // Get base values for vertices and uv coordinates.
        if (baseHeight == null)
        {
            Mesh mesh = baseMesh;
            baseHeight = mesh.vertices;
            baseUV = mesh.uv;

            var itemCount = baseHeight.Length;
            uvs = new Vector2[itemCount];
            vertices = new Vector3[itemCount];
            normals = new Vector3[itemCount];
            tangents = new Vector4[itemCount];
        }

        var n_scale = size.x / width / scale;

        var scaleA = choppy_scale / (width * height);
        var scaleB = scale / (width * height);
        var scaleBinv = 1.0 / scaleB;

        for (int i = 0; i < width * height; i++)
        {
            int iw = i + i / width;
            vertices[iw] = baseHeight[iw];
            vertices[iw].x += data[i].Im * scaleA;
            vertices[iw].y = data[i].Re * scaleB;

            normals[iw] = new Vector3(t_x[i].Re, (float)scaleBinv, t_x[i].Im).normalized;

            //		uv = baseUV[iw];
            //		uv.x = uv.x + Time.time * uv_speed;
            //		uvs[iw] = uv;

            if (((i + 1) % width) == 0)
            {
                vertices[iw + 1] = baseHeight[iw + 1];
                vertices[iw + 1].x += data[i + 1 - width].Im * scaleA;
                vertices[iw + 1].y = data[i + 1 - width].Re * scaleB;

                normals[iw + 1] = new Vector3(t_x[i + 1 - width].Re, (float)scaleBinv, t_x[i + 1 - width].Im).normalized;

                //			uv = baseUV[iw+1];
                //			uv.x = uv.x + Time.time * uv_speed;
                //			uvs[iw+1] = uv;				
            }
        }

        var offset = g_width * (g_height - 1);

        for (int i = 0; i < g_width; i++)
        {
            vertices[i + offset] = baseHeight[i + offset];
            vertices[i + offset].x += data[i % width].Im * scaleA;
            vertices[i + offset].y = data[i % width].Re * scaleB;

            normals[i + offset] = new Vector3(t_x[i % width].Re, (float)scaleBinv, t_x[i % width].Im).normalized;

            //		uv = baseUV[i+offset];
            //		uv.x = uv.x - Time.time*uv_speed;
            //		uvs[i+offset] = uv;
        }

        if (!forceOriginalShader)
        {
            //Real-time updating of the water colors.
            material.SetColor("_SurfaceColor", surfaceColor);
            material.SetColor("_WaterColor", waterColor);

            waterCompositionMaterial.SetColor("_WaterColor", waterColor);

            for (int i = 0; i < g_width * g_height - 1; i++)
            {

                //Need to preserve w in refraction/reflection mode
                if (!reflectionRefractionEnabled)
                {
                    if (((i + 1) % g_width) == 0)
                    {
                        tangents[i] = (vertices[i - width + 1] + new Vector3(size.x, 0.0f, 0.0f) - vertices[i]).normalized;
                    }
                    else
                    {
                        tangents[i] = (vertices[i + 1] - vertices[i]).normalized;
                    }

                    tangents[i].w = 1.0f;
                }
                else
                {
                    Vector3 tmp = Vector3.zero;

                    if (((i + 1) % g_width) == 0)
                    {
                        tmp = (vertices[i - width + 1] + new Vector3(size.x, 0.0f, 0.0f) - vertices[i]).normalized;
                    }
                    else
                    {
                        tmp = (vertices[i + 1] - vertices[i]).normalized;
                    }

                    tangents[i] = new Vector4(tmp.x, tmp.y, tmp.z, tangents[i].w);
                }
            }

            //In reflection mode, use tangent w for foam strength
            if (reflectionRefractionEnabled)
            {
                for (int y = 0; y < g_height; y++)
                {
                    for (int x = 0; x < g_width; x++)
                    {
                        if (x + 1 >= g_width)
                        {
                            tangents[x + g_width * y].w = tangents[g_width * y].w;

                            continue;
                        }

                        if (y + 1 >= g_height)
                        {
                            tangents[x + g_width * y].w = tangents[x].w;

                            continue;
                        }

                        Vector3 right = vertices[(x + 1) + g_width * y] - vertices[x + g_width * y];
                        Vector3 back = vertices[x + g_width * y] - vertices[x + g_width * (y + 1)];

                        float foam = right.x / (size.x / g_width);


                        if (foam < 0.0f)
                            tangents[x + g_width * y].w = 1;
                        else if (foam < 0.5f)
                            tangents[x + g_width * y].w += 3.0f * Time.deltaTime;
                        else
                            tangents[x + g_width * y].w -= 0.4f * Time.deltaTime;

                        tangents[x + g_width * y].w = Mathf.Clamp(tangents[x + g_width * y].w, 0.0f, 2.0f);
                    }
                }
            }

            tangents[g_width * g_height - 1] = (vertices[g_width * g_height - 1] + new Vector3(size.x, 0.0f, 0.0f) - vertices[1]).normalized;
        }


        for (var LOD = 0; LOD < max_LOD; LOD++)
        {
            var den = Mathf.FloorToInt(Mathf.Pow(2, LOD));
            var itemcount = (height / den + 1) * (width / den + 1);

            var tangentsLOD = new Vector4[itemcount];
            var verticesLOD = new Vector3[itemcount];
            var normalsLOD = new Vector3[itemcount];
            //		uvLOD = new Vector2[(height/Mathf.Pow(2,LOD)+1) * (width/Mathf.Pow(2,LOD)+1)];
            int idx = 0;

            for (int y = 0; y < g_height; y += den)
            {
                for (int x = 0; x < g_width; x += den)
                {
                    int idx2 = g_width * y + x;
                    verticesLOD[idx] = vertices[idx2];
                    //				uvLOD[idx] = uvs[g_width * y + x];
                    tangentsLOD[idx] = tangents[idx2];
                    normalsLOD[idx++] = normals[idx2];
                }
            }
            for (int k = 0; k < tiles_LOD[LOD].Count; k++)
            {
                Mesh meshLOD = tiles_LOD[LOD][k];
                meshLOD.vertices = verticesLOD;
                meshLOD.normals = normalsLOD;
                //			meshLOD.uv = uvLOD;
                meshLOD.tangents = tangentsLOD;
            }
        }
    }

    /*
    Called when the object is about to be rendered. We render the refraction/reflection
    passes from here, since we only need to do it once per frame, not once per tile.
    */
    void OnWillRenderObject()
    {
        //Recursion guard, don't let the offscreen cam go into a never-ending loop.
        if (Camera.current == offscreenCam
        || Camera.current == depthCam)
            return;

        if (reflectionTexture == null
        || refractionTexture == null)
            return;

        RenderWaterDepth();
        RenderReflectionAndRefraction();
    }

    /*
    Renders the wave height from above for use as a depth comparison map
    when blending over/underwater renders.
    */
    void RenderWaterDepth()
    {
        if (!renderWaterDepth)
            return;

        depthCam.backgroundColor = Color.black;


        Vector3 pos = Camera.current.gameObject.transform.position;


        int waterMask = 1 << LayerMask.NameToLayer("Water");

        depthCam.orthographic = true;

        //TODO: Match this with the maximum possible viewplane for the current camera. 50 is WAY
        //      too large, but it works for testing purposes and it makes it possible to see the
        //      heightmap on the rendered output.
        depthCam.orthographicSize = 50;
        depthCam.aspect = 1.0f;

        //NOTE: Changes to the 20 unit view distance MUST BE REFLECTED 
        //      in WaterComposition and WaterHeight shaders! This is due
        //      to a hack because I had severe problems getting the clip-space
        //      values to work consistently on Windows and Mac. I figured I'd
        //      just do it this way, so there is no chance for it to break, although
        //      there will be a bit more work if the distance is to change.
        //TODO: Fix the hack, or set this as a parameter to WaterComposition
        //      and WaterHeight. This is left as an excercise for the reader.
        depthCam.nearClipPlane = 0.0f;
        depthCam.farClipPlane = 20.0f;

        depthCam.transform.position = new Vector3(pos.x, transform.position.y + 10.0f, pos.z);
        depthCam.transform.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

        depthCam.targetTexture = waterHeightTexture;
        depthCam.clearFlags = CameraClearFlags.SolidColor;
        depthCam.cullingMask = waterMask;
        depthCam.RenderWithShader(depthShader, "");
    }


    /*
    Renders the reflection and refraction buffers using a second camera copying the current
    camera settings.
    */
    void RenderReflectionAndRefraction()
    {
        int oldPixelLightCount = QualitySettings.pixelLightCount;
        QualitySettings.pixelLightCount = 0;


        Camera renderCamera = Camera.current;

        Matrix4x4 originalWorldToCam = renderCamera.worldToCameraMatrix;

        int cullingMask = renderCamera.cullingMask & ~(1 << LayerMask.NameToLayer("Water"));

        //Reflection pass
        Matrix4x4 reflection = Matrix4x4.zero;

        //TODO: Use local plane here, not global!

        float d = -transform.position.y;


        CameraHelper.CalculateReflectionMatrix(ref reflection, new Vector4(0, 1, 0, d));

        offscreenCam.transform.position = reflection.MultiplyPoint(renderCamera.transform.position);
        offscreenCam.transform.rotation = renderCamera.transform.rotation;
        offscreenCam.worldToCameraMatrix = originalWorldToCam * reflection;

        offscreenCam.cullingMask = cullingMask;
        offscreenCam.targetTexture = reflectionTexture;
        offscreenCam.clearFlags = renderCamera.clearFlags;

        //Need to reverse face culling for reflection pass, since the camera
        //is now flipped upside/down.
        GL.invertCulling = true;

        Vector4 cameraSpaceClipPlane = CameraHelper.CameraSpacePlane(offscreenCam, new Vector3(0.0f, transform.position.y, 0.0f), Vector3.up, 1.0f);

        Matrix4x4 projection = renderCamera.projectionMatrix;
        Matrix4x4 obliqueProjection = projection;

        offscreenCam.fieldOfView = renderCamera.fieldOfView;
        offscreenCam.aspect = renderCamera.aspect;

        CameraHelper.CalculateObliqueMatrix(ref obliqueProjection, cameraSpaceClipPlane);

        //Do the actual render, with the near plane set as the clipping plane. See the
        //pro water source for details.
        offscreenCam.projectionMatrix = obliqueProjection;

        if (!renderReflection)
            offscreenCam.cullingMask = 0;

        offscreenCam.Render();

        GL.invertCulling = false;

        //Refractionpass
        bool fog = RenderSettings.fog;
        Color fogColor = RenderSettings.fogColor;
        float fogDensity = RenderSettings.fogDensity;

        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.grey;
        RenderSettings.fogDensity = waterDirtyness;

        //TODO: If we want to use this as a refraction seen from under the seaplane,
        //      the cameraclear should be skybox.
        offscreenCam.clearFlags = CameraClearFlags.Color;
        offscreenCam.backgroundColor = Color.grey;

        offscreenCam.cullingMask = cullingMask;
        offscreenCam.targetTexture = refractionTexture;
        obliqueProjection = projection;

        offscreenCam.transform.position = renderCamera.transform.position;
        offscreenCam.transform.rotation = renderCamera.transform.rotation;
        offscreenCam.worldToCameraMatrix = originalWorldToCam;


        cameraSpaceClipPlane = CameraHelper.CameraSpacePlane(offscreenCam, Vector3.zero, Vector3.up, -1.0f);
        CameraHelper.CalculateObliqueMatrix(ref obliqueProjection, cameraSpaceClipPlane);
        offscreenCam.projectionMatrix = obliqueProjection;

        if (!renderRefraction)
            offscreenCam.cullingMask = 0;

        offscreenCam.Render();

        RenderSettings.fog = fog;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;

        offscreenCam.projectionMatrix = projection;


        offscreenCam.targetTexture = null;

        //Do the passes for the underwater "effect" if the WaterPostEffect script is present on the
        //current camera.
        WaterPostEffect wpe = renderCamera.gameObject.GetComponent<WaterPostEffect>();

        if (wpe != null)
        {
            int waterMask = 1 << LayerMask.NameToLayer("Water");




            offscreenCam.clearFlags = CameraClearFlags.Skybox;
            offscreenCam.backgroundColor = Color.grey;

            offscreenCam.cullingMask = cullingMask;
            offscreenCam.targetTexture = underwaterRefractionTexture;
            obliqueProjection = projection;

            offscreenCam.transform.position = renderCamera.transform.position;
            offscreenCam.transform.rotation = renderCamera.transform.rotation;
            offscreenCam.worldToCameraMatrix = originalWorldToCam;


            cameraSpaceClipPlane = CameraHelper.CameraSpacePlane(offscreenCam, new Vector3(0.0f, transform.position.y, 0.0f), Vector3.up, 1.0f);
            CameraHelper.CalculateObliqueMatrix(ref obliqueProjection, cameraSpaceClipPlane);
            offscreenCam.projectionMatrix = obliqueProjection;

            if (!renderUnderwaterRefraction)
                offscreenCam.cullingMask = 0;

            offscreenCam.Render();



            offscreenCam.projectionMatrix = projection;


            offscreenCam.targetTexture = null;



            Shader.SetGlobalTexture("_UnderWaterRefraction", underwaterRefractionTexture);
            Shader.SetGlobalTexture("_UnderWaterBump", texBump);
            Shader.SetGlobalTexture("_Fresnel", texFresnel);
            Shader.SetGlobalVector("_Size", new Vector4(size.x, size.y, size.z, 0.0f));



            //Draw underwater
            RenderSettings.fog = true;
            RenderSettings.fogColor = Color.grey;
            RenderSettings.fogDensity = waterDirtyness;

            offscreenCam.orthographic = false;
            offscreenCam.backgroundColor = Color.grey;
            offscreenCam.clearFlags = CameraClearFlags.Color;
            offscreenCam.transform.position = renderCamera.transform.position;
            offscreenCam.transform.rotation = renderCamera.transform.rotation;
            offscreenCam.fieldOfView = renderCamera.fieldOfView;
            offscreenCam.nearClipPlane = 0.3f;
            offscreenCam.farClipPlane = 200.0f;

            offscreenCam.targetTexture = underwaterTexture;

            if (renderUnderwater)
            {
                //First, draw only the water tiles with inverted normals and a custom, simplified
                //shader, so we can see the surface from below as well.
                offscreenCam.cullingMask = waterMask;
                GL.invertCulling = true;
                offscreenCam.RenderWithShader(waterBelowShader, "");
                GL.invertCulling = false;

                offscreenCam.clearFlags = CameraClearFlags.Nothing;
                offscreenCam.cullingMask = cullingMask & ~(waterMask);
                offscreenCam.Render();
            }

            RenderSettings.fog = fog;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;


            Matrix4x4 depthMV = depthCam.worldToCameraMatrix;

            Matrix4x4 MVP = renderCamera.projectionMatrix * renderCamera.worldToCameraMatrix;

            waterCompositionMaterial.SetMatrix("_DepthCamMV", depthMV);
            waterCompositionMaterial.SetMatrix("_DepthCamProj", depthCam.projectionMatrix);

            wpe.waterCompositionMaterial = waterCompositionMaterial;
        }

        QualitySettings.pixelLightCount = oldPixelLightCount;
    }
}