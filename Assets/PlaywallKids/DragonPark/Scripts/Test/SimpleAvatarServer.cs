using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ML.PlaywallKids.DragonPark.Test
{
    public class SimpleAvatarServer : MonoBehaviour
    {
        #region Public variables
        public string IP = "127.0.0.1";
        public int port = 5000;

        public AvatarSkeleton user1Skeleton = null;
        public AvatarSkeleton user2Skeleton = null;
        #endregion

        #region Properties
        private static SimpleAvatarServer __sharedInstance = null;
        public static SimpleAvatarServer sharedInstance
        {
            get
            {
                if (__sharedInstance == null)
                {
                    __sharedInstance = FindObjectOfType<SimpleAvatarServer>();
                    if (__sharedInstance == null)
                    {
                        GameObject go = new GameObject("SimpleAvatarServer");
                        __sharedInstance = go.AddComponent<SimpleAvatarServer>();
                        DontDestroyOnLoad(go);
                    }
                }
                return __sharedInstance;
            }
        }
        #endregion

        #region Private variables
        private Socket _socket;
        private Socket _client;

        private byte[] _buffer = new byte[kBufferSize];
        private List<byte> _bufferList = new List<byte>(kBufferSize * 4);
        #endregion

        #region Constants
        public const int kBufferSize = 256;
        #endregion

        #region Unity Methods
        void Start()
        {
            _InitSocket();
        }

        void Update()
        {
            _ParsePackets();
        }

        void OnDestroy()
        {
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }

            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }
        }
        #endregion

        private void _InitSocket()
        {
            Debug.Log("Hosting on port " + port);

            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(new IPEndPoint(IPAddress.Any, port));
                _socket.Listen(5);
                _socket.BeginAccept(new System.AsyncCallback(OnClientConnect), _socket);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Exception when attempting to host (" + port + "): " + e);
                if (_socket != null)
                {
                    _socket.BeginDisconnect(false, new System.AsyncCallback(OnEndHostComplete), _socket);
                    _socket = null;
                }
            }
        }

        void OnEndHostComplete(System.IAsyncResult result)
        {
            _socket = null;
        }

        void OnClientConnect(System.IAsyncResult result)
        {
            Debug.Log("Handling client connecting");
            if (_socket == null) return;
            try
            {
                Socket client = _socket.EndAccept(result);
                IPEndPoint endPoint = (IPEndPoint)client.RemoteEndPoint;
                Debug.Log("Client connected : " + endPoint.Address.ToString());

                _client = client;

                _socket.BeginAccept(new System.AsyncCallback(OnClientConnect), _socket);

                _client.BeginReceive(_buffer, 0, kBufferSize, SocketFlags.None, new System.AsyncCallback(OnReceive), null);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Exception when accepting incoming connection: " + e);
            }
        }

        void OnReceive(System.IAsyncResult result)
        {
            if (_client != null)
            {
                int len = _client.EndReceive(result);
                for (int i = 0; i < len; i++)
                {
                    _bufferList.Add(_buffer[i]);
                }

                _client.BeginReceive(_buffer, 0, 7, SocketFlags.None, new System.AsyncCallback(OnReceive), null);
            }
        }

        private void _ParsePackets()
        {
            int len = _bufferList.Count;
            if (len >= AvatarPacket.kPacketSize)
            {
                AvatarPacket packet = AvatarPacket.Parse(_bufferList);
                _bufferList.RemoveRange(0, AvatarPacket.kPacketSize);

                if (packet.userCount > 0)
                {
                    user1Skeleton.MoveJoints(packet.user1Positions);
                }
                if (packet.userCount > 1)
                {
                    user2Skeleton.MoveJoints(packet.user2Positions);
                }
            }
        }

        void OnSend(System.IAsyncResult result)
        {
            if (_client != null)
            {
                _client.EndSend(result);
            }
        }
    }
}