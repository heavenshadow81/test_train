using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Text;
using System.IO;

public class MultiWindowDrawing : MonoBehaviour
{
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    public class WindowInfo
    {
        public string name { get { return texture != null ? texture.name : ""; } }
        public IntPtr hWnd;
        public Texture texture;
    }

#if UNITY_STANDALONE
    [DllImport("user32.dll")]
    public static extern void GetWindowRect(IntPtr hWnd, ref RECT rect);

    [DllImport("user32.dll")]
    public static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern long SetWindowLong(IntPtr hWnd, int nIndex, long style);

    [DllImport("MultiWindowGLDLL2")]
    public static extern IntPtr MultiWindowGetDC();

    [DllImport("MultiWindowGLDLL2")]
    public static extern IntPtr MultiWindowGetRC();

    [DllImport("MultiWindowGLDLL2")]
    public static extern IntPtr MultiWindowGetHWnd();

    [DllImport("MultiWindowGLDLL2")]
    public static extern void InitMainGLWindow(IntPtr hWnd, IntPtr hDC, IntPtr hRC);

    [DllImport("MultiWindowGLDLL2")]
    public static extern int GetAllMonitorInfos();
    
    [DllImport("MultiWindowGLDLL2")]
    public static extern IntPtr CreateGLWindow(int monitor, [MarshalAs(UnmanagedType.LPWStr)]string title, int x, int y, int width, int height, bool fullscreen);

    [DllImport("MultiWindowGLDLL2")]
    public static extern void DrawTextureInGLWindow(IntPtr hWnd, IntPtr textureID);

    [DllImport("MultiWindowGLDLL2")]
    public static extern void CloseGLWindow(IntPtr hWnd);

    [DllImport("MultiWindowGLDLL2")]
    public static extern int ShareGLContext(IntPtr hWnd);

    [DllImport("MultiWindowGLDLL2")]
    public static extern void SetGLTextureRect(IntPtr textureID, float x, float y, float width, float height);

    [DllImport("MultiWindowGLDLL2")]
    public static extern IntPtr GetRenderEventFunc();
#endif

    #region Public variables
    public Texture[] textures;
    public bool restoreWindowsOnStart = false;
    #endregion

    #region Properties
    public static Dictionary<string, RECT> cachedWindowRects
    {
        get
        {
            if (_cachedWindowRects.Count == 0)
            {
                try
                {
                    StreamReader file = new StreamReader("window.txt");
                    string line = null;
                    string[] comma = { "," };
                    while ((line = file.ReadLine()) != null)
                    {
                        string[] tokens = line.Split(comma, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens != null && tokens.Length >= 5)
                        {
                            string windowName = tokens[0];

                            int left, top, right, bottom;
                            int.TryParse(tokens[1], out left);
                            int.TryParse(tokens[2], out top);
                            int.TryParse(tokens[3], out right);
                            int.TryParse(tokens[4], out bottom);

                            RECT rect = new RECT();
                            rect.left = left;
                            rect.right = right;
                            rect.top = top;
                            rect.bottom = bottom;

                            _cachedWindowRects[windowName] = rect;
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            return _cachedWindowRects;
        }
    }

    // OpenGL (Windows)
    public IntPtr hDC { get; private set; }
    public IntPtr hRC { get; private set; }
    public IntPtr hWnd { get; private set; }
    #endregion

    private static List<IntPtr> _windowHandles = new List<IntPtr>();
    private static Dictionary<string, IntPtr> _windowTextureDict = new Dictionary<string, IntPtr>();
    private static Dictionary<string, RECT> _cachedWindowRects = new Dictionary<string, RECT>();

    // coroutine
    private static Coroutine _RefreshStatesCoroutine;
    private static Coroutine _DrawWindowsCoroutine;

    private MultiWindowDrawingGUI _gui;

    bool borderless = false;

    #region Unity Methods
    void Start()
    {
#if UNITY_STANDALONE
        hDC = MultiWindowGetDC();
        hRC = MultiWindowGetRC();
        hWnd = MultiWindowGetHWnd();
#elif UNITY_ANDROID
        Debug.LogWarning("MultiWindow 기능은 Android 플랫폼에서 지원되지 않습니다.");
#endif

        _gui = GetComponent<MultiWindowDrawingGUI>();

        if (restoreWindowsOnStart)
            RestoreWindows();

        if (_RefreshStatesCoroutine == null)
            _RefreshStatesCoroutine = StartCoroutine(_RefreshStates());
        if (_DrawWindowsCoroutine == null)
            _DrawWindowsCoroutine = StartCoroutine(_DrawWindows());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            var t = textures[_windowHandles.Count];

            OpenWindow(t);
        }
        else if (Input.GetKeyDown(KeyCode.S) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            SaveWindowRects();
        }
        else if (Input.GetKeyDown(KeyCode.F) && !Screen.fullScreen)
        {
            ToggleBorderless(!borderless);

            Debug.Log("Borderless : " + borderless.ToString());
            _ShowMessage("Borderless : " + borderless.ToString());
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            RestoreWindows();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Clear();
        }
    }

    void OnDestroy()
    {
        if (_RefreshStatesCoroutine != null)
        {
            StopCoroutine(_RefreshStatesCoroutine);
            _RefreshStatesCoroutine = null;
        }
        if (_DrawWindowsCoroutine != null)
        {
            StopCoroutine(_DrawWindowsCoroutine);
            _DrawWindowsCoroutine = null;
        }

        ClearTexturesInWindow();
    }

    void OnApplicationQuit()
    {
        Clear();
    }
    #endregion

    #region States
    IEnumerator _RefreshStates()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            // Checks if the windows are opened or closed.
            _RefreshWindowStates();
        }
    }

    void _RefreshWindowStates()
    {
        var keys = new List<string>(_windowTextureDict.Keys);

        foreach (string key in keys)
        {
            IntPtr hWnd = _windowTextureDict[key];

#if UNITY_STANDALONE
            if (!IsWindow(hWnd))
            {
                _windowTextureDict.Remove(key);
                _windowHandles.Remove(hWnd);

                Debug.Log("Closed Window (" + key + ")");
            }
#endif
        }
    }

    void ToggleBorderless(bool flag)
    {
#if UNITY_STANDALONE
        borderless = flag;

        long dwStyle = 0, dwExStyle = 0;

        if (borderless)
        {
            dwStyle = 0x92000000L;
            dwExStyle = 0x00040000L;
        }
        else
        {
            dwStyle = 0x00C00000L | 0x00080000L | 0x00040000L | 0x00020000L | 0x00010000L | 0x12000000L;
            dwExStyle = 0x000001000L;
        }

        SetWindowLong(hWnd, -16, dwStyle);
        SetWindowLong(hWnd, -20, dwExStyle);
#endif
    }
    #endregion

    #region Draw
    private IEnumerator _DrawWindows()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

#if UNITY_STANDALONE
            if (_windowTextureDict.Count > 0)
            {
                GL.IssuePluginEvent(GetRenderEventFunc(), 0);
            }
#endif
        }
    }
    #endregion

    #region Window Managements
    public void OpenWindow(Texture t, int x, int y, int width, int height, bool fullscreen)
    {
        if (t == null) return;

        string texName = t.name;

#if UNITY_STANDALONE
        IntPtr hWnd = CreateGLWindow(0, texName, x, y, width, height, fullscreen);
        if (hWnd != IntPtr.Zero)
        {
            _windowHandles.Add(hWnd);

            if (ShareGLContext(hWnd) > 0)
            {
                Debug.Log("Share context(" + hWnd + ") success!");
            }
            else
            {
                Debug.Log("Share context(" + hWnd + ") failed!");
            }

            if (t.GetType().Equals(typeof(RenderTexture)))
            {
                RenderTexture rt = (RenderTexture)t;
                if (!rt.IsCreated())
                    rt.Create();
            }
            
            DrawTextureInGLWindow(hWnd, t.GetNativeTexturePtr());

            _windowTextureDict[texName] = hWnd;
        }
#endif
    }

    public void OpenWindow(Texture t)
    {
        OpenWindow(t, 40 + _windowHandles.Count * 20, 40 + _windowHandles.Count * 20, 800, 600, false);
    }

    public bool IsWindowOpened(Texture t)
    {
        if (t == null) return false;
        return _windowTextureDict.ContainsKey(t.name);
    }

    void CloseWindow(IntPtr hWnd)
    {
        string texName = "";

#if UNITY_STANDALONE
        foreach (string t in _windowTextureDict.Keys)
        {
            if (_windowTextureDict[t] == hWnd)
            {
                texName = t;
                break;
            }
        }

        if (!string.IsNullOrEmpty(texName))
        {
            _windowTextureDict.Remove(texName);
        }

        if (_windowHandles.Contains(hWnd))
        {
            _windowHandles.Remove(hWnd);
        }

        CloseGLWindow(hWnd);
#endif
    }

    public void CloseWindow(Texture t)
    {
        IntPtr hWnd = new IntPtr();

#if UNITY_STANDALONE
        foreach (string texName in _windowTextureDict.Keys)
        {
            if (texName.Equals(t.name))
            {
                hWnd = _windowTextureDict[texName];
                break;
            }
        }

        if (hWnd != IntPtr.Zero)
        {
            if (!string.IsNullOrEmpty(t.name))
            {
                _windowTextureDict.Remove(t.name);
            }

            if (_windowHandles.Contains(hWnd))
            {
                _windowHandles.Remove(hWnd);
            }

            CloseGLWindow(hWnd);
        }
#endif
    }

    public void RestoreWindows()
    {
        _ShowMessage("Restored windows.");

#if UNITY_STANDALONE
        if (_windowTextureDict.Count > 0)
        {
            foreach (string textureName in _windowTextureDict.Keys)
            {
                IntPtr hWnd = _windowTextureDict[textureName];
                Texture texture = null;

                foreach (Texture t in textures)
                {
                    string texName = t.name;
                    if (texName.Equals(textureName))
                    {
                        texture = t;
                        break;
                    }
                }

                if (texture != null)
                    DrawTextureInGLWindow(hWnd, texture.GetNativeTexturePtr());
                else
                {
                    CloseGLWindow(hWnd);
                }
            }
        }
        else if (cachedWindowRects.Count > 0)
        {
            foreach (string texName in cachedWindowRects.Keys)
            {
                Texture texture = null;
                foreach (Texture t in textures)
                {
                    if (texName.Equals(t.name))
                    {
                        texture = t;
                        break;
                    }
                }

                if (texture != null)
                {
                    RECT rect = _cachedWindowRects[texName];

                    if (rect.right - rect.left > 0 && rect.bottom - rect.top > 0)
                        OpenWindow(texture, rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top, true);
                }
            }
        }
#endif
    }

    public void ClearTexturesInWindow()
    {
#if UNITY_STANDALONE
        foreach (string textureName in _windowTextureDict.Keys)
        {
            IntPtr hWnd = _windowTextureDict[textureName];

            DrawTextureInGLWindow(hWnd, IntPtr.Zero);
        }
#endif
    }

    public void SetTextureUVRect(Texture t, Rect rect)
    {
#if UNITY_STANDALONE
        SetGLTextureRect(t.GetNativeTexturePtr(), rect.x, 1.0f - rect.y - rect.height, rect.width, rect.height);
#endif
    }

    public void SaveWindowRects()
    {
        StringBuilder sb = new StringBuilder();

#if UNITY_STANDALONE
        foreach (string texName in _windowTextureDict.Keys)
        {
            IntPtr hWnd = _windowTextureDict[texName];
            RECT rect = new RECT();

            GetWindowRect(hWnd, ref rect);

            // append string
            sb.AppendFormat("{0},{1},{2},{3},{4}\n", texName, rect.left, rect.top, rect.right, rect.bottom);

            // refresh cached window rect
            _cachedWindowRects[texName] = rect;
        }

        try
        {
            File.WriteAllText("window.txt", sb.ToString());
            Debug.Log("MultiWindowDrawing.SaveWindowRects() : Completed writing \"window.txt\"!");

            _ShowMessage("Saved \"window.txt\"");
        }
        catch (IOException e)
        {
            Debug.Log("MultiWindowDrawing.SaveWindowRects() : Failed writing \"window.txt\"!");
            Debug.LogException(e);

            _ShowMessage(string.Format("Failed to save \"window.txt\". reason ({0}).", e.Message));
        }
#endif
    }

    public void Clear()
    {
        _ShowMessage("Cleared all windows.");

#if UNITY_STANDALONE
        foreach (IntPtr hWnd in _windowHandles)
        {
            CloseGLWindow(hWnd);
        }
#endif

        _windowHandles.Clear();
        _windowTextureDict.Clear();
    }
    #endregion

    private void _ShowMessage(string message)
    {
        if (_gui != null)
            _gui.ShowMessage(message);
    }
}

