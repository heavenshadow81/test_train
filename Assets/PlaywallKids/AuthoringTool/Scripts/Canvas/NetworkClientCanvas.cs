using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

[RequireComponent(typeof(UITexture))]
public class NetworkClientCanvas : Canvas_ {
    #region Properties
    private Socket _socket = null;
    public Socket socket
    {
        get
        {
            return _socket;
        }
    }

    private bool _connecting = false;
    public bool connecting
    {
        get
        {
            return _connecting;
        }
    }

	private bool _disconnecting = false;
	public bool disconnecting {
		get {
			return _disconnecting;
		}
	}

    private bool _connected = false;
    public bool connected
    {
        get
        {
            return _connected;
        }
    }
    #endregion

    #region Private variables
    private Thread _thread = null;

    private Dictionary<string, object> _drawingInfoDict = new Dictionary<string,object>();

    private List<string> _packedDrawingInfoStrList = new List<string>();
    #endregion

    #region Unity Methods
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        if (_connected)
        {
            _ClearDrawingInfoDict();

            base.Update();

            if (_drawingInfoDict.Count > 0)
            {
                lock (_packedDrawingInfoStrList)
                {
                    string jsonStr = MiniJSON.Json.Serialize(_drawingInfoDict);
                    if (!string.IsNullOrEmpty(jsonStr))
                    {
                        _packedDrawingInfoStrList.Add(jsonStr);
                    }

                    _ClearCommandInfoDict();
                }
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Disconnect();
    }
    #endregion

    #region Super methods
    protected override void _DrawForTouch(TouchInfo t, Vector3 pos)
    {
        base._DrawForTouch(t, pos);

        _WriteTouchInfoIntoDictionary(t);
    }

    public override void ClearCanvas()
    {
        base.ClearCanvas();

        _WriteClearCanvasInfoIntoDictionary();
    }

    public override void FillColor(Color color)
    {
        base.FillColor(color);

        _WriteClearCanvasInfoIntoDictionary();
    }

    public override void ClearAlphaBuffer()
    {
        base.ClearAlphaBuffer();

        _WriteClearAlphaBufferInfoIntoDictionary();
    }

    public override void ClearDualBrushBuffer()
    {
        base.ClearDualBrushBuffer();

        _WriteClearDualBrushBufferInfoIntoDictionary();
    }
    #endregion

    #region Socket Connection
    public void Connect(string IP, int port)
    {
        // if it is already connected or connecting, avoid.
        if (_connected || _connecting) return;

        // also if IP or port is invalid, doesn't connect.
        if(string.IsNullOrEmpty(IP) || port < 1) return;

        // set connecting flag to true
        _connecting = true;

        // makes new socket
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.SendTimeout = 1000;
        //_socket.NoDelay = true;
        _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(IP), port), new System.AsyncCallback(_OnConnect), _socket);
    }

    public void Disconnect()
    {
        if (_connected)
        {
            _connected = false;
			_disconnecting = true;

            // abort and release thread
            if (_thread != null)
            {
                _thread.Abort();
                _thread = null;
            }
        }
    }

	// This method can be run in thread only.
	private void _PerformDisconnect() {
		if (_socket != null)
		{
			// send disconnection message to server
			_socket.Send(System.BitConverter.GetBytes(-1));
			
			Debug.Log("NetworkClientCanvas - Disconected.");

			// shutdonw and close the socket
			_socket.Shutdown(SocketShutdown.Both);
			_socket.Close();

			// release the socket
			_socket = null;

			// set disconnecting flag to false
			_disconnecting = false;
		}
	}

    private void _OnConnect(System.IAsyncResult ar)
    {
        Socket socket = ar.AsyncState as Socket;
        if (socket != null)
        {
            _connecting = false;

            socket.EndConnect(ar);

            _connected = socket.Connected;

			// Run thread loop
			if (_thread == null)
			{
				_thread = new Thread(new ThreadStart(_SendDataToRemote));
				_thread.Start();
			}

			// Handle connection state, print the log
            if (_connected)
			{
				// success log
				Debug.Log (string.Format("NetworkClientCanvas({0})->{1} - Connection Success!", _socket.LocalEndPoint.ToString(), _socket.RemoteEndPoint.ToString()));
            }
            else
            {
				// failed log
                Debug.Log("NetworkClientCanvas(" + _socket.LocalEndPoint.ToString() + ") - Connection failed.");
                
				// close and release the socket
				_socket.Close();
				_socket = null;
            }
        }
    }
    #endregion

    #region Writing Drawing Informations

    private void _ClearDrawingInfoDict()
    {
        if (_drawingInfoDict.ContainsKey("drawing"))
        {
            _drawingInfoDict.Remove("drawing");
        }

        if (_drawingInfoDict.ContainsKey("brush"))
        {
            _drawingInfoDict.Remove("brush");
        }
    }

    private void _ClearCommandInfoDict()
    {
        if (_drawingInfoDict.ContainsKey("command"))
        {
            _drawingInfoDict.Remove("command");
        }
    }

    private void _WriteBrushInfoIntoDictionary()
    {
        Dictionary<string, object> brushInfoDict = new Dictionary<string, object>();

        // brush name
        brushInfoDict["brush_name"] = brush.brushName;

        // diameter
        brushInfoDict["diameter"] = brush.diameter;

        // r, g, b, a
        brushInfoDict["r"] = brush.color.r;
        brushInfoDict["g"] = brush.color.g;
        brushInfoDict["b"] = brush.color.b;
        brushInfoDict["a"] = brush.color.a;

        _drawingInfoDict["brush"] = brushInfoDict;
    }

    private void _WriteTouchInfoIntoDictionary(TouchInfo t)
    {
        Dictionary<string, object> touchInfoDict = new Dictionary<string, object>();

        // touch id
        touchInfoDict["touch_id"] = t.id;
        
        // convert screen space -> canvas space position
        Vector3 pos = ScreenToCanvas(t.position);

        // normalize position values (0 ~ 1)
        pos.x = pos.x / canvasSize.x;
        pos.y = pos.y / canvasSize.y;

        // write x, y
        touchInfoDict["x"] = pos.x;
        touchInfoDict["y"] = pos.y;

        // touch phase
        touchInfoDict["phase"] = t.phase.ToString().ToLower();

        // write to drawing info dictionary
        List<object> touchInfoDictList = null;
        if (_drawingInfoDict.ContainsKey("drawing"))
        {
            touchInfoDictList = _drawingInfoDict["drawing"] as List<object>;
        }
        else
        {
            touchInfoDictList = new List<object>();
            _drawingInfoDict["drawing"] = touchInfoDictList;
        }

        touchInfoDictList.Add(touchInfoDict);

        if (!_drawingInfoDict.ContainsKey("brush"))
        {
            _WriteBrushInfoIntoDictionary();
        }
    }

    private Dictionary<string, object> _GetCommandInfoDictFromDictionary()
    {
        Dictionary<string, object> commandInfoDict = null;

        if (_drawingInfoDict.ContainsKey("command"))
        {
            commandInfoDict = _drawingInfoDict["command"] as Dictionary<string, object>;
        }
        else
        {
            commandInfoDict = new Dictionary<string, object>();
            _drawingInfoDict["command"] = commandInfoDict;
        }

        return commandInfoDict;
    }

    private void _WriteClearCanvasInfoIntoDictionary()
    {
        var commandInfoDict = _GetCommandInfoDictFromDictionary();

        if (!commandInfoDict.ContainsKey("clear_canvas") || commandInfoDict["clear_canvas"] == null)
        {
            commandInfoDict["clear_canvas"] = true;
        }
    }

    private void _WriteFillColorInfoIntoDictionary(Color color)
    {
        var commandInfoDict = _GetCommandInfoDictFromDictionary();

        if (!commandInfoDict.ContainsKey("fill_color") || commandInfoDict["fill_color"] == null)
        {
            // convert float format to byte format
            Color32 color32 = color;

            // fill with string values like #ffffffff (color format
            commandInfoDict["fill_color"] = string.Format("{0:X}{1:X}{2:X}{4:X}", color32.r, color32.g, color32.b, color32.a);
        }
    }

    private void _WriteClearDualBrushBufferInfoIntoDictionary()
    {
        var commandInfoDict = _GetCommandInfoDictFromDictionary();

        if (!commandInfoDict.ContainsKey("clear_dual_brush_buffer") || commandInfoDict["clear_dual_brush_buffer"] == null)
        {
            commandInfoDict["clear_dual_brush_buffer"] = true;
        }
    }

    private void _WriteClearAlphaBufferInfoIntoDictionary()
    {
        var commandInfoDict = _GetCommandInfoDictFromDictionary();

        if (!commandInfoDict.ContainsKey("clear_alpha_buffer") || commandInfoDict["clear_alpha_buffer"] == null)
        {
            commandInfoDict["clear_alpha_buffer"] = true;
        }
    }
    #endregion

    #region Thread Loop
    private void _SendDataToRemote()
    {
        while (_connected)
        {
            if (_packedDrawingInfoStrList.Count > 0)
            {
                for (int i = 0; i < _packedDrawingInfoStrList.Count; i++)
                {
                    string jsonStr = _packedDrawingInfoStrList[i];

                    if (string.IsNullOrEmpty(jsonStr))
                    {
                        Debug.Log("Debug Me!!");
                    }

                    Debug.Log("Send data string : " + jsonStr);

                    if (!string.IsNullOrEmpty(jsonStr))
                    {
                        // buffer
                        List<byte> buffer = new List<byte>();

                        // convert string to bytes
                        byte[] jsonStrBytes = System.Text.Encoding.UTF8.GetBytes(jsonStr);

                        // length
                        int length = jsonStrBytes.Length;
                        byte[] lengthBytes = System.BitConverter.GetBytes(length);
                        buffer.AddRange(lengthBytes);

                        // data
                        buffer.AddRange(jsonStrBytes);

                        // send packet
                        if (_socket != null)
                        {
                            try
                            {
                                _socket.Send(buffer.ToArray(), 0, buffer.Count, SocketFlags.None);
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogException(e);
                            }
                        }
                    }
                }
                _packedDrawingInfoStrList.Clear();
            }
        }

		Debug.Log ("NetworkClientCanvas - Thread loop ended.");

		// Before thread is ended, disconnect the socket.
		_PerformDisconnect();
    }
    #endregion
}