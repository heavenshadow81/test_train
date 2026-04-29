using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MultiWindowDrawing))]
public class MultiWindowDrawingGUI : MonoBehaviour
{
    #region Private variables
    // Component
    private MultiWindowDrawing _multiWindowDrawing;

    // Message
    string _message = "";
    float _messageShowTime = 0.0f;
    float _currentMessageShowTime = 0.0f;
    #endregion

    #region Unity Methods
    private void Start()
    {
        _multiWindowDrawing = GetComponent<MultiWindowDrawing>();
    }
    /*
    private void OnGUI()
    {
        IntPtr hDC = _multiWindowDrawing.hDC;
        IntPtr hRC = _multiWindowDrawing.hRC;
        IntPtr hWnd = _multiWindowDrawing.hWnd;

        GUI.contentColor = Color.white;

        var graphicsDeviceType = SystemInfo.graphicsDeviceType;
        string graphicsDeviceTypeStr = "Unknown";
        switch (graphicsDeviceType)
        {
            case GraphicsDeviceType.OpenGLCore: graphicsDeviceTypeStr = "OpenGL Core"; break;
#if !UNITY_5_5_OR_NEWER
            case GraphicsDeviceType.OpenGL2: graphicsDeviceTypeStr = "OpenGL Legacy"; break;
#endif
            case GraphicsDeviceType.Direct3D11: graphicsDeviceTypeStr = "DirectX 11"; break;
            case GraphicsDeviceType.Direct3D12: graphicsDeviceTypeStr = "DirectX 9"; break;
            case GraphicsDeviceType.Metal: graphicsDeviceTypeStr = "Metal"; break;
        }
        GUI.Box(new Rect(20, 60, 120, 20), graphicsDeviceTypeStr);
        GUI.Box(new Rect(150, 60, 400, 20), string.Format("hDC : {0}, hRC : {1}, hWnd : {2}", hDC, hRC, hWnd));

        GUI.Box(new Rect(20, 90, 100, 20), "Views");
        int height = 120;
        foreach (Texture t in _multiWindowDrawing.textures)
        {
            string texName = t.name;

            GUI.Label(new Rect(20, height, 100, 20), texName);
            if (_multiWindowDrawing.IsWindowOpened(t))
            {
                GUI.Label(new Rect(120, height, 100, 20), "Opened");
            }
            else
            {
                if (GUI.Button(new Rect(120, height, 100, 20), "Open"))
                {
                    _multiWindowDrawing.OpenWindow(t);
                }
            }
            height += 25;
        }

        if (GUI.Button(new Rect(20, 15 + height, 150, 20), "Restore Windows (W)"))
        {
            _multiWindowDrawing.RestoreWindows();
        }
        if (GUI.Button(new Rect(180, 15 + height, 140, 20), "Close Windows (Q)"))
        {
            _multiWindowDrawing.Clear();
        }

        height += 30;

        if (GUI.Button(new Rect(20, 15 + height, 180, 20), "Save Windows (Shift+S)"))
        {
            _multiWindowDrawing.SaveWindowRects();
        }

        height += 40;

        if (!string.IsNullOrEmpty(_message) && _currentMessageShowTime < _messageShowTime)
        {
            // simple text fade in/out animation
            float alpha = 1.0f;
            float fadeTime = Mathf.Min(_messageShowTime * 0.1666f, 0.25f);
            if (_currentMessageShowTime < fadeTime)
                alpha = Mathf.Lerp(0.0f, 1.0f, 1.0f - Mathf.Pow(_currentMessageShowTime / fadeTime - 1.0f, 2.0f));
            else if (_currentMessageShowTime >= _messageShowTime - fadeTime)
                alpha = Mathf.Lerp(1.0f, 0.0f, Mathf.Pow((_messageShowTime - fadeTime - _currentMessageShowTime) / fadeTime, 2.0f));

            Color messageColor = new Color(1.0f, 1.0f, 1.0f, alpha);
            GUI.contentColor = messageColor;

            GUI.Label(new Rect(20, height, 400, 20), _message);
            _currentMessageShowTime += 0.0166f;
        }
    }*/
    
    private void OnDestroy()
    {
        _multiWindowDrawing = null;
    }
#endregion

#region Messaging
    public void ShowMessage(string message)
    {
        _message = message;
        _messageShowTime = 3.0f;
        _currentMessageShowTime = 0.0f;
    }
#endregion
}