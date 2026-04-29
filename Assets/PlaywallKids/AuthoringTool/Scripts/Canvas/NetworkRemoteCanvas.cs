using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

[RequireComponent(typeof(UITexture))]
public class NetworkRemoteCanvas : Canvas_ {
    #region Properties
    private Socket _clientSocket = null;
    public Socket clientSocket
    {
        get
        {
            return _clientSocket;
        }
        set
        {
			if(_clientSocket != value) {
				if(_clientSocket != null) {
					Disconnect();
				}
				
				_clientSocket = value;
				
				if (_clientSocket != null && _clientSocket.Connected)
				{
					_clientSocket.ReceiveTimeout = 1000;
					
					if (_thread == null)
					{
						_thread = new Thread(new ThreadStart(_ReceiveDataFromClient));
						_thread.Start();
					}
				}
			}
        }
    }

	public bool connected {
		get {
			return _clientSocket != null;
		}
	}
    #endregion

    #region Private variables
    private Thread _thread;

	private bool _disconnecting = false;

    private List<Dictionary<string, object>> _receivedDrawingInfoDictList = new List<Dictionary<string, object>>();

    private List<byte> _buffer = new List<byte>();

    private byte[] _packetBuffer = new byte[128];

    private byte[] _cachedIntegerBytes = new byte[4];
    private byte[] _cachedStringBytes = new byte[256];
    #endregion

    #region Unity Methods
    public override void Update()
    {
	    if (wantsPaint)
	    {
	        _ParseBuffer();

	        for (int i = 0; i < _receivedDrawingInfoDictList.Count; i++)
	        {
	            Dictionary<string, object> dict = _receivedDrawingInfoDictList[i];

	            _ParseDataAndExecute(dict);
	        }

	        _receivedDrawingInfoDictList.Clear();
	    }
	}

    public override void OnDestroy()
    {
        base.OnDestroy();
        Disconnect();
    }
    #endregion

    #region Parsing datas
    private void _ParseDataAndExecute(Dictionary<string, object> dict)
    {
        if (dict != null)
        {
            _ParseCommandsAndExecute(dict);
            _ParseBrushInfoAndExecute(dict);
            _ParseDrawingInfoAndExecute(dict);
        }
    }

    private void _ParseCommandsAndExecute(Dictionary<string, object> dict)
    {
        if (dict.ContainsKey("command"))
        {
            Dictionary<string, object> commandDict = dict["command"] as Dictionary<string, object>;
            if (commandDict != null)
            {
                // Clear Canvas
                if (commandDict.ContainsKey("clear_canvas") && commandDict["clear_canvas"] != null)
                {
                    bool clearCanvas = false;
                    bool.TryParse(commandDict["clear_canvas"].ToString(), out clearCanvas);

                    if (clearCanvas)
                    {
                        ClearCanvas();
                    }
                }

                // Fill Color
                if (commandDict.ContainsKey("fill_color") && commandDict["fill_color"] != null)
                {
                    string colorStr = commandDict["fill_color"].ToString();

                    // remove #
                    if (colorStr[0] == '#')
                    {
                        colorStr = colorStr.Remove(0, 1);
                    }

                    // parse components (r, g, b, a)
                    Color color = Color.clear;

                    // fill with one component (grayscale)
                    if (colorStr.Length < 3)
                    {
                        byte value = 0;
                        byte.TryParse(colorStr, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out value);
                        color.r = color.g = color.a = (float)value / 255.0f;
                        color.a = 1.0f;
                    }
                    // multi components
                    else
                    {
                        byte r = 0, g = 0, b = 0, a = 255;
                        byte.TryParse(colorStr.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out r);
                        if (colorStr.Length >= 4)
                        {
                            byte.TryParse(colorStr.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out g);
                        }
                        if (colorStr.Length >= 6)
                        {
                            byte.TryParse(colorStr.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out b);
                        }
                        if (colorStr.Length >= 8)
                        {
                            byte.TryParse(colorStr.Substring(6, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out a);
                        }
                        color.r = (float)r / 255.0f;
                        color.g = (float)g / 255.0f;
                        color.b = (float)b / 255.0f;
                        color.a = (float)a / 255.0f;
                    }

                    FillColor(color);
                }

                // Clear Alpha Buffer
                if (commandDict.ContainsKey("clear_alpha_buffer") && commandDict["clear_alpha_buffer"] != null)
                {
                    bool clearAlphaBuffer = false;
                    bool.TryParse(commandDict["clear_alpha_buffer"].ToString(), out clearAlphaBuffer);

                    if (clearAlphaBuffer)
                    {
                        ClearAlphaBuffer();
                    }
                }

                // Clear Dual Brush Buffer
                if (commandDict.ContainsKey("clear_dual_brush_buffer") && commandDict["clear_dual_brush_buffer"] != null)
                {
                    bool clearDualBrushBuffer = false;
                    bool.TryParse(commandDict["clear_dual_brush_buffer"].ToString(), out clearDualBrushBuffer);

                    if (clearDualBrushBuffer)
                    {
                        ClearDualBrushBuffer();
                    }
                }
            }
        }
    }

    private void _ParseDrawingInfoAndExecute(Dictionary<string, object> dict)
    {
        if (dict.ContainsKey("drawing"))
        {
            List<object> list = dict["drawing"] as List<object>;
            if (list != null)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    Dictionary<string, object> touchInfoDict = list[j] as Dictionary<string, object>;

                    if (touchInfoDict == null) continue;

                    int touchId = 0;
                    float x = 0, y = 0;
                    TouchInfo.Phase phase = TouchInfo.Phase.Begin;

                    TouchInfo touchInfo = new TouchInfo();
                    if (touchInfoDict.ContainsKey("touch_id") && touchInfoDict["touch_id"] != null)
                    {
                        int.TryParse(touchInfoDict["touch_id"].ToString(), out touchId);
                    }
                    if (touchInfoDict.ContainsKey("x") && touchInfoDict["x"] != null)
                    {
                        float.TryParse(touchInfoDict["x"].ToString(), out x);
                    }
                    if (touchInfoDict.ContainsKey("y") && touchInfoDict["y"] != null)
                    {
                        float.TryParse(touchInfoDict["y"].ToString(), out y);
                    }
                    if (touchInfoDict.ContainsKey("phase") && touchInfoDict["phase"] != null)
                    {
                        string phaseStr = touchInfoDict["phase"].ToString();
                        switch (phaseStr)
                        {
                            case "begin":
                                phase = TouchInfo.Phase.Begin;
                                break;
                            case "move":
                                phase = TouchInfo.Phase.Move;
                                break;
                            case "stay":
                                phase = TouchInfo.Phase.Stay;
                                break;
                            case "cancel":
                                phase = TouchInfo.Phase.Cancel;
                                break;
                            case "end":
                                phase = TouchInfo.Phase.End;
                                break;
                        }
                    }

                    touchInfo.id = touchId;
                    touchInfo.phase = phase;

                    Vector3 pos = new Vector3(x * canvasSize.x, y * canvasSize.y);

                    _DrawForTouch(touchInfo, pos);
                }

                Flush();
            }
        }
    }

    private void _ParseBrushInfoAndExecute(Dictionary<string, object> dict)
    {
        if (dict.ContainsKey("brush"))
        {
            Dictionary<string, object> brushInfoDict = dict["brush"] as Dictionary<string, object>;

            Brush b = brush;

            string brushName = b.brushName;
            float diameter = 4.0f;
            Color color = Color.white;

            if (brushInfoDict.ContainsKey("brush_name") && brushInfoDict["brush_name"] != null)
            {
                brushName = brushInfoDict["brush_name"].ToString();
            }
            if (brushInfoDict.ContainsKey("diameter") && brushInfoDict["diameter"] != null)
            {
                float.TryParse(brushInfoDict["diameter"].ToString(), out diameter);
            }
            if (brushInfoDict.ContainsKey("r") && brushInfoDict["r"] != null)
            {
                float.TryParse(brushInfoDict["r"].ToString(), out color.r);
            }
            if (brushInfoDict.ContainsKey("g") && brushInfoDict["g"] != null)
            {
                float.TryParse(brushInfoDict["g"].ToString(), out color.g);
            }
            if (brushInfoDict.ContainsKey("b") && brushInfoDict["b"] != null)
            {
                float.TryParse(brushInfoDict["b"].ToString(), out color.b);
            }
            if (brushInfoDict.ContainsKey("a") && brushInfoDict["a"] != null)
            {
                float.TryParse(brushInfoDict["a"].ToString(), out color.a);
            }

            bool changeBrush = false;
            if (!b.brushName.Equals(brushName))
            {
                changeBrush = true;
                Brush newBrush = brushSet.Get(brushName);
                if (newBrush != null)
                {
                    b = newBrush;
                }

                b.diameter = diameter;
                b.color = color;
            }
            else if (!diameter.Equals(b.diameter))
            {
                changeBrush = true;
                b.diameter = diameter;
                b.color = color;
            }
            else
            {
                b.color = color;
            }

            if (changeBrush)
            {
                brush = b;
            }
        }
    }

    #endregion

    #region Socket Connections
    public void Disconnect()
    {
        if (_clientSocket != null)
        {
			_clientSocket.Shutdown(SocketShutdown.Both);
			_clientSocket.Close();
			_clientSocket = null;
        }

		if(_thread != null) 
		{
			_thread.Abort();
			_thread = null;
		}
    }

    private void _ParseBuffer()
    {
        if (_buffer.Count > 4)
        {
            // copy count bytes to cached integer array
            _cachedIntegerBytes[0] = _buffer[0];
            _cachedIntegerBytes[1] = _buffer[1];
            _cachedIntegerBytes[2] = _buffer[2];
            _cachedIntegerBytes[3] = _buffer[3];

            // get length
            int len = System.BitConverter.ToInt32(_cachedIntegerBytes, 0);

            // if length is valid and buffer length is larger than value
            if (len > 0 && _buffer.Count >= len + 4)
            {
                // if cached string bytes array is smaller than buffer length, increase double.
                if (len + 1 > _cachedStringBytes.Length)
                {
                    _cachedStringBytes = new byte[_cachedStringBytes.Length * 2];
                }

                // copy buffer data to array
                for (int i = 0; i < len; i++)
                {
                    _cachedStringBytes[i] = _buffer[4 + i];
                }

                // '\0'
                _cachedStringBytes[len] = 0;

                // get json string
                string jsonStr = System.Text.Encoding.UTF8.GetString(_cachedStringBytes);

                Debug.Log("parsed string : " + jsonStr);

                // parse json buffer to dictionary and add into list
                try
                {
                    Dictionary<string, object> dict = MiniJSON.Json.Deserialize(jsonStr) as Dictionary<string, object>;
                    if (dict != null)
                    {
                        _receivedDrawingInfoDictList.Add(dict);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.Log("NetworkRemoteCanvas.ParseBuffer() : Invalid json format. This buffer will be ignored.");
                    Debug.LogException(e);
                }

				_buffer.RemoveRange(0, len + 4);
            }
			// disconnection
			else if(len == -1) 
			{
				Disconnect();
			}
        }
    }
    #endregion

    #region Thread Loop
    private void _ReceiveDataFromClient()
    {
        while (connected)
        {
            try
            {
                int len = _clientSocket.Receive(_packetBuffer);
                if (len > 0)
                {
                    for (int i = 0; i < len; i++)
                    {
                        _buffer.Add(_packetBuffer[i]);
                    }
                }
            }
            catch (SocketException e)
            {
                Debug.LogException(e);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

		Debug.Log("NetworkRemoteCanvas - Ending thread loop...");
    }
    #endregion
}
