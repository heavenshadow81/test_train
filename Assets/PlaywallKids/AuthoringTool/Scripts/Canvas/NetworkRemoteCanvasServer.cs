using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class NetworkRemoteCanvasServer : MonoBehaviour {
    #region Public variables
    public int port = 5999;
    #endregion

    #region Properties
    private Socket _socket = null;
    public Socket socket
    {
        get
        {
            return _socket;
        }
    }
    #endregion

    public NetworkRemoteCanvas canvas;

	// Use this for initialization
	void Start () {
        Host();
	}

    void OnDestroy()
    {
        Disconnect();
    }

    public void Host()
    {
        if (_socket == null)
        {
            try {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(new IPEndPoint(IPAddress.Any, port));
                _socket.Listen(10);

                Debug.Log("Hosting on port " + port);

                _socket.BeginAccept(new System.AsyncCallback(_OnAccept), _socket);
            }
            catch(SocketException e) {
                Debug.LogException(e);

                _socket.Close();
                _socket = null;
            }
        }
    }

    public void Disconnect()
    {
        if (_socket != null)
        {
            Debug.Log("Ending host...");
            _socket.BeginDisconnect(false, new System.AsyncCallback(_OnDisconnect), _socket);
        }
    }

    private void _OnDisconnect(System.IAsyncResult ar)
    {
        Socket socket = ar.AsyncState as Socket;
        if (socket != null)
        {
            Debug.Log("End host.");

            socket.EndDisconnect(ar);
            socket.Close();

            if (socket == _socket)
            {
                _socket = null;
            }
        }
    }

    private void _OnAccept(System.IAsyncResult ar)
    {
        Socket socket = ar.AsyncState as Socket;
        if (socket != null)
        {
			Debug.Log ("New client connection");
            Socket client = socket.EndAccept(ar);
			
			Debug.Log ("client : " + client);

            if (client != null && client.Connected)
            {
                canvas.clientSocket = client;
            }
            socket.BeginAccept(new System.AsyncCallback(_OnAccept), socket);
        }

    }
}
