using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ML.PlaywallKids.EAS
{
    public class EASSocket
    {
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
        public bool disconnecting
        {
            get
            {
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

        private bool _host = false;
        #endregion

        #region Constants
        public const int kBufferMaxLength = 256;
        public const int kConnectionTimeout = 3000;

        private const int WSAEINTR = 10004;
        private const int WSAEWOULDBLOCK = 10035;
        private const int WSAECONNABORTED = 10053;
        #endregion

        #region Private classes
        private class EASSocketSendingPacketInfo
        {
            private EASPacket _packet;
            public EASPacket packet
            {
                get
                {
                    return _packet;
                }
            }

            private System.Action<bool> _handler = null;
            public System.Action<bool> handler
            {
                get
                {
                    return _handler;
                }
            }

            public EASSocketSendingPacketInfo(EASPacket packet, System.Action<bool> handler)
            {
                _packet = packet;
                _handler = handler;
            }
        }
        #endregion

        #region Private variables
        // Backing thread
        private Thread _thread = null;

        // Waiting sending bytes
        private Queue<EASSocketSendingPacketInfo> _waitingSendingPackets = new Queue<EASSocketSendingPacketInfo>();

        // Buffer
        private byte[] _buffer = new byte[kBufferMaxLength];

        // Waiting received bytes
        private List<byte> _waitingReceivedBytes = new List<byte>();

        // Received Packets
        private Dictionary<string, Queue<EASPacket>> _receivedPackets = new Dictionary<string, Queue<EASPacket>>();

        // Cached bytes
        private byte[] _cachedIntegerBytes = new byte[4];
        private byte[] _cachedStringBytes = new byte[128];
        #endregion

        private static EASSocket _CreateAcceptedClient(Socket client)
        {
            if (client != null)
            {
                client.NoDelay = true;
                client.Blocking = false;
                client.ReceiveBufferSize = 65536;
                client.SendBufferSize = 65536;

                EASSocket socket = new EASSocket();

                socket._socket = client;
                socket._connected = client.Connected;

                if (socket._thread == null)
                {
                    Thread thread = new Thread(new ThreadStart(socket._SocketReadWriteLoop));
                    thread.Start();

                    socket._thread = thread;
                }

                return socket;
            }

            return null;
        }

        public void Connect(string IP, int port, System.Action<bool> connectionHandler)
        {
            // if it is already connected or connecting, avoid.
            if (_connected || _connecting) return;

            // also if IP or port is invalid, doesn't connect.
            if (string.IsNullOrEmpty(IP) || port < 1) return;

            // set connecting flag to true
            _connecting = true;

            // makes new socket
            if (_socket == null)
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.SendTimeout = 1000;
                _socket.NoDelay = true;
                _socket.Blocking = false;
                _socket.ReceiveBufferSize = 65536;
                _socket.SendBufferSize = 65536;
            }
            // make state
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["socket"] = _socket;
            dict["handler"] = connectionHandler;

            // begin connect
            System.IAsyncResult ar = _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(IP), port), new System.AsyncCallback(_OnConnect), dict);

            // wait for seconds
            new Thread(new ThreadStart(() =>
            {
                bool success = ar.AsyncWaitHandle.WaitOne(kConnectionTimeout, true);
                if (!success)
                {
                    _connecting = false;
                    _connected = false;

                    if (_socket != null)
                    {
                        _socket.Close();
                        _socket = null;
                    }

                    if (connectionHandler != null)
                    {
                        Loom.QueueOnMainThread(() => { connectionHandler(false); });
                    }
                }
            })).Start();
        }

        private void _OnConnect(System.IAsyncResult ar)
        {
            Dictionary<string, object> dict = ar.AsyncState as Dictionary<string, object>;
            if (dict != null)
            {
                _connecting = false;

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
                    _connected = socket.Connected;

                    // Check if the thread is allocated
                    if (_thread != null)
                    {
                        _thread.Abort();
                        _thread = null;
                    }

                    _thread = new Thread(new ThreadStart(_SocketReadWriteLoop));

                    // Run thread loop
                    try
                    {
                        _thread.Start();
                    }
                    catch (System.Exception e)
                    {
                        Loom.QueueOnMainThread(() =>
                        {
                            Debug.LogException(e);
                        });
                    }
                    // Handle connection state, print the log
                    if (_connected)
                    {
                        // success log
                        Loom.QueueOnMainThread(() =>
                        {
                            Debug.Log(string.Format("EASSocket - Connection Success!"));
                        });
                    }
                    else
                    {
                        // failed log
                        Loom.QueueOnMainThread(() =>
                        {
                            Debug.Log("EASSocket - Connection failed.");
                        });

                        // close and release the socket
                        _socket.Close();
                        _socket = null;
                    }

                    // Send connection result to handler
                    if (connectionHandler != null)
                    {
                        Loom.QueueOnMainThread(() =>
                        {
                            connectionHandler(_connected);
                        });
                    }

                    socket.EndConnect(ar);
                }
            }
        }

        public void Host(string IP, int port, System.Action<bool> hostHandler, System.Action<EASSocket> acceptHandler)
        {
            // if it is already connected or connecting, avoid.
            if (_connected || _connecting) return;

            // also if port is invalid, doesn't connect.
            if (port < 1) return;

            // set the flag
            _connecting = true;

            // Set IP address
            IPAddress address = IPAddress.Any;
            if (!string.IsNullOrEmpty(IP))
            {
                IPAddress.TryParse(IP, out address);
            }

            if (_socket == null)
            {
                try
                {
                    // Make the new socket
                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    // Bind
                    _socket.Bind(new IPEndPoint(address, port));

                    // Listen
                    _socket.Listen(10);

                    Debug.Log("EASSocket - Hosting on port " + port);

                    // Make the object state
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    dict["socket"] = _socket;
                    dict["handler"] = acceptHandler;

                    _socket.BeginAccept(new System.AsyncCallback(_OnAccept), dict);

                    _connected = true;
                    _host = true;
                }
                catch (SocketException e)
                {
                    Debug.LogException(e);

                    _socket.Close();
                    _socket = null;

                    _connected = false;
                }

                if (hostHandler != null)
                {
                    hostHandler(_connected);
                }
            }

            _connecting = false;
        }

        private void _OnAccept(System.IAsyncResult ar)
        {
            Dictionary<string, object> dict = ar.AsyncState as Dictionary<string, object>;
            if (dict != null)
            {
                Socket socket = null;
                System.Action<EASSocket> acceptHandler = null;

                if (dict.ContainsKey("socket") && dict["socket"] != null)
                {
                    socket = dict["socket"] as Socket;
                }

                if (dict.ContainsKey("handler") && dict["handler"] != null)
                {
                    acceptHandler = dict["handler"] as System.Action<EASSocket>;
                }

                if (socket != null)
                {
                    Socket client = socket.EndAccept(ar);

                    EASSocket clientSocket = EASSocket._CreateAcceptedClient(client);

                    if (clientSocket != null)
                    {
                        if (acceptHandler != null)
                        {
                            Loom.QueueOnMainThread(() =>
                            {
                                acceptHandler(clientSocket);
                            });
                        }
                    }

                    _socket.BeginAccept(new System.AsyncCallback(_OnAccept), dict);
                }
            }
        }

        public void Disconnect()
        {
            if (_connected && !_connecting && !_disconnecting)
            {
                _disconnecting = true;

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
                    _socket.Close();
                    _socket = null;
                }

                if (_thread != null)
                {
                    _thread.Abort();
                    _thread = null;
                }

                _disconnecting = false;
                _connected = false;
                _host = false;

                Loom.QueueOnMainThread(() =>
                {
                    Debug.Log("EASSocket - Disconnected.");
                });
            }
        }

        public void Send(EASPacket packet)
        {
            if (packet != null)
            {
                EASSocketSendingPacketInfo info = new EASSocketSendingPacketInfo(packet, null);

                _waitingSendingPackets.Enqueue(info);
            }
        }

        public void Send(EASPacket packet, System.Action<bool> handler)
        {
            if (packet != null)
            {
                EASSocketSendingPacketInfo info = new EASSocketSendingPacketInfo(packet, handler);

                _waitingSendingPackets.Enqueue(info);
            }
            else if (handler != null)
            {
                handler(false);
            }
        }

        public EASPacket Receive(string type)
        {
            return Receive(type, true);
        }

        public EASPacket Receive(string type, bool pop)
        {
            EASPacket packet = null;

            Queue<EASPacket> queue = null;
            if (!string.IsNullOrEmpty(type))
            {
                _receivedPackets.TryGetValue(type, out queue);
                if (queue != null && queue.Count > 0)
                {
                    if (pop)
                    {
                        packet = queue.Dequeue();
                    }
                    else
                    {
                        packet = queue.Peek();
                    }
                }
            }

            return packet;
        }

        public void Clear(string type)
        {
            Queue<EASPacket> queue = null;
            if (!string.IsNullOrEmpty(type))
            {
                _receivedPackets.TryGetValue(type, out queue);

                if (queue != null)
                {
                    queue.Clear();
                }
            }
        }

        private void _ParsePackets()
        {
            while (_waitingReceivedBytes.Count > 4)
            {
                // copy count bytes to cached integer array
                _cachedIntegerBytes[0] = _waitingReceivedBytes[0];
                _cachedIntegerBytes[1] = _waitingReceivedBytes[1];
                _cachedIntegerBytes[2] = _waitingReceivedBytes[2];
                _cachedIntegerBytes[3] = _waitingReceivedBytes[3];

                // get length
                int len = System.BitConverter.ToInt32(_cachedIntegerBytes, 0);

                // if length is valid and buffer length is larger than value
                if (len > 0 && _waitingReceivedBytes.Count >= len + 4)
                {
                    // if cached string bytes array is smaller than buffer length, increase double.
                    while (len + 1 > _cachedStringBytes.Length)
                    {
                        _cachedStringBytes = new byte[_cachedStringBytes.Length * 2];
                    }

                    // copy buffer data to array
                    for (int i = 0; i < len; i++)
                    {
                        _cachedStringBytes[i] = _waitingReceivedBytes[i + 4];
                    }

                    // '\0'
                    _cachedStringBytes[len] = 0;

                    // Clear used buffer range
                    _waitingReceivedBytes.RemoveRange(0, len + 4);

                    // get json string
                    string jsonStr = System.Text.Encoding.UTF8.GetString(_cachedStringBytes);

                    // make packet
                    EASPacket packet = new EASPacket(jsonStr);

#if UNITY_EDITOR
                    Loom.QueueOnMainThread(() =>
                    {
                        if (!packet.type.Equals(EASPacket.kTypeCheckConnection)) Debug.Log("EASSocket - parsed string : " + jsonStr);
                    });
#endif

                    if (!_receivedPackets.ContainsKey(packet.type))
                    {
                        Queue<EASPacket> queue = new Queue<EASPacket>();
                        _receivedPackets[packet.type] = queue;
                    }

                    _receivedPackets[packet.type].Enqueue(packet);
                }
                else if (len <= 0)
                {
                    _waitingReceivedBytes.RemoveRange(0, 4);
                }
                else
                {
                    break;
                }
            }
        }

        private void _SocketReadWriteLoop()
        {
            while (_connected)
            {
                // Receive
                try
                {
                    System.Array.Clear(_buffer, 0, _buffer.Length);

                    int length = _socket.Receive(_buffer, 0, kBufferMaxLength, SocketFlags.None);
                    if (length > 0)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            _waitingReceivedBytes.Add(_buffer[i]);
                        }

                        _ParsePackets();
                    }
                }
                catch (SocketException e)
                {
                    bool closeSocket = false;
                    _HandleSocketException(e, out closeSocket);
                    if (closeSocket)
                    {
                        Disconnect();
                        break;
                    }
                }

                // Send
                if (_waitingSendingPackets.Count > 0)
                {
                    EASSocketSendingPacketInfo info = _waitingSendingPackets.Dequeue();
                    EASPacket packet = info.packet;
                    System.Action<bool> handler = info.handler;

                    if (packet != null)
                    {
                        byte[] bytes = packet.ToByteArray();
                        try
                        {
                            _socket.Send(bytes, 0, bytes.Length, SocketFlags.None);

                            Loom.QueueOnMainThread(() =>
                            {
                                if (handler != null)
                                {
                                    handler(true);
                                }

#if UNITY_EDITOR
                            if (!packet.type.Equals(EASPacket.kTypeCheckConnection)) Debug.Log("EASSocket - Sended packet " + packet.ToString());
#endif
                        });
                        }
                        catch (SocketException e)
                        {
                            bool closeSocket = false;
                            _HandleSocketException(e, out closeSocket);

                            if (handler != null)
                            {
                                Loom.QueueOnMainThread(() =>
                                {
                                    handler(false);
                                });
                            }

                            if (closeSocket)
                            {
                                Disconnect();
                                break;
                            }
                        }
                    }
                }

                // sleep slightly
                Thread.Sleep(8);
            }

            // if failed sending packet is available, call handler
            if (_waitingSendingPackets.Count > 0)
            {
                Loom.QueueOnMainThread(() =>
                {
                    foreach (var info in _waitingSendingPackets)
                    {
                        if (info.handler != null)
                        {
                            info.handler(false);
                        }
                    }
                });
            }
        }

        private void _HandleSocketException(SocketException e, out bool closeSocket)
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