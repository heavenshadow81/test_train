using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace ML.PlaywallKids.Interaction
{
    public delegate void ReceiveDataCallBack(int length, byte[] buffers);
    public class PlayWallKinectSockets
    {
        private const int WSAEINTR = 10004;
        private const int WSAEWOULDBLOCK = 10035;
        private const int WSAECONNABORTED = 10053;
        private const int MAX_PACKET_LENGTHS = 3832961;

        private string _ip;
        private int _port;

        private Socket _socket = null;
        private ReceiveDataCallBack _callback;
        private Thread _thread = null;
        private byte[] _buffers = null;

        private bool _isConnecting = false;
        private bool _isDisconnecting = false;
        private bool _isConnected = false;
        private bool _host = false;

        public bool Connecting
        {
            get { return _isConnecting; }
        }

        public bool Disconnecting
        {
            get { return _isDisconnecting; }
        }

        public bool Connected
        {
            get { return _isConnected; }
        }


        public PlayWallKinectSockets()
        {
            _buffers = new byte[MAX_PACKET_LENGTHS];
            FileInfo fileInfo = new FileInfo("ServerInfo.txt");
            if (fileInfo.Exists == true)
            {
                StreamReader sReader = new StreamReader("ServerInfo.txt");
                _ip = sReader.ReadLine();
                _port = int.Parse(sReader.ReadLine());

                sReader.Close();
            }
            else
            {
                FileStream fs = File.Create("ServerInfo.txt");
                StreamWriter sWriter = new StreamWriter(fs);
                sWriter.WriteLine(Constants.DEFAULT_SERVER_IP);
                sWriter.WriteLine(Constants.DEFAULT_SERVER_PORT.ToString());

                _ip = Constants.DEFAULT_SERVER_IP;
                _port = Constants.DEFAULT_SERVER_PORT;

                sWriter.Close();
                fs.Close();
            }
        }

        public void Connect(ReceiveDataCallBack receiveCallback, System.Action<bool> connectionHandler)
        {
            Debug.Log("Socket Connect Function");
            _callback = receiveCallback;
            if (_socket == null)
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.SendTimeout = 1000;
                _socket.NoDelay = true;
                _socket.ReceiveBufferSize = MAX_PACKET_LENGTHS;
                _socket.SendBufferSize = 65536;
            }

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["socket"] = _socket;
            dict["handler"] = connectionHandler;

            // begin connect
            System.IAsyncResult ar = _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(_ip), _port), new System.AsyncCallback(OnConnect), dict);
        }

        public void DisConnect()
        {
            if (_isConnected && !_isConnecting && !_isDisconnecting)
            {
                _isDisconnecting = true;

                if (_socket != null)
                {
                    if (!_host)
                    {
                        try
                        {
                            _socket.Shutdown(SocketShutdown.Both);
                        }
                        catch (SocketException e)
                        {
                            Debug.Log("EASSocket - exception occured doing shutdown.");
                            Debug.LogException(e);
                        }
                    }
                    Loom.QueueOnMainThread(() =>
                    {
                        Debug.Log("PlayWallKinectSocket - Socket Closing");
                    });
                    _socket.Close();
                    _socket = null;
                }

                if (_thread != null)
                {
                    _thread.Abort();
                    _thread = null;
                }

                _isDisconnecting = false;
                _isConnected = false;
                _host = false;

                Loom.QueueOnMainThread(() =>
                {
                    Debug.Log("PlayWallKinectSocket - Disconnected.");
                });
            }
        }

        public void SendMessage(byte[] packets)
        {
            Debug.Log("send message");
            _socket.Send(packets);
        }
        private void OnConnect(System.IAsyncResult ar)
        {
            Dictionary<string, object> dict = ar.AsyncState as Dictionary<string, object>;
            if (dict != null)
            {
                _isConnecting = false;

                Socket socket = null;
                System.Action<bool> connectionHandler = null;

                if (dict.ContainsKey("socket") && dict["socket"] != null)
                {
                    socket = dict["socket"] as Socket;
                }

                if (dict.ContainsKey("handler") && dict["handler"] != null)
                {
                    connectionHandler = dict["handler"] as System.Action<bool>;
                }

                if (socket != null)
                {
                    _isConnected = socket.Connected;

                    // Check if the thread is allocated
                    if (_thread != null)
                    {
                        _thread.Abort();
                        _thread = null;
                    }

                    _thread = new Thread(new ThreadStart(SocketReadWriteLoop));

                    // Run thread loop
                    try
                    {
                        _thread.Start();
                        Debug.Log("thread start complete");
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("thread start Exception");

                        Loom.QueueOnMainThread(() =>
                        {
                            Debug.LogException(e);
                        });
                    }
                    // Handle connection state, print the log
                    if (_isConnected)
                    {
                        Debug.Log("is connected");
                        // success log
                        Loom.QueueOnMainThread(() =>
                        {
                            Debug.Log(string.Format("EASSocket - Connection Success!"));
                        });

                        Debug.Log("is connected End");
                    }
                    else
                    {
                        Debug.Log("is connected else");
                        // failed log
                        Loom.QueueOnMainThread(() =>
                        {
                            Debug.Log("PlayWallKinectSocket - Connection failed.");
                        });

                        // close and release the socket
                        _socket.Close();
                        _socket = null;
                    }

                    // Send connection result to handler
                    if (connectionHandler != null)
                    {
                        Debug.Log("connection handler");
                        Loom.QueueOnMainThread(() =>
                        {
                            connectionHandler(_isConnected);
                        });
                    }

                    socket.EndConnect(ar);
                }
            }
        }

        private void SocketReadWriteLoop()
        {
            while (_isConnected)
            {
                try
                {
                    if (_buffers == null) return;
                    System.Array.Clear(_buffers, 0, _buffers.Length);
                    int length = _socket.Receive(_buffers, 0, MAX_PACKET_LENGTHS, SocketFlags.None);

                    if (length > 0)
                    {
                        _callback(length, _buffers);
                        //ParsePacket(_buffer);
                    }
                }
                catch (SocketException ex)
                {
                    bool closeSocket = false;
                    HandleSocketException(ex, out closeSocket);
                    if (closeSocket)
                    {
                        _isConnected = false;
                        break;
                    }
                }
            }
        }

        private void HandleSocketException(SocketException e, out bool closeSocket)
        {
            closeSocket = false;

            if (e != null)
            {
                switch (e.ErrorCode)
                {
                    case WSAEINTR:
                    case WSAECONNABORTED:
                        Debug.Log("Socket exception - error code : " + e.ErrorCode);
                        Debug.LogException(e);
                        closeSocket = true;
                        break;
                    case WSAEWOULDBLOCK:
                        break;
                }
            }
        }
    }
}