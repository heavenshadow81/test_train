using UnityEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// 로보빌더 로봇과의 통신을 위한 클래스. 모션 관련 정보를 송수신한다.
    /// </summary>
    [Obsolete("This class is no longer used.")]
    public class SimpleMotionServer : MonoBehaviour
    {
        public string IP = "127.0.0.1";
        public int port = 5000;

        private Socket _socket;
        private Socket _client;

        private byte[] _buffer = new byte[13];
        private List<byte> _bufferList = new List<byte>(255);

        public const int User1 = 0x31;
        public const int User2 = 0x32;

        public UserMotionInfo user1 = new UserMotionInfo(User1);
        public UserMotionInfo user2 = new UserMotionInfo(User2);

        private static SimpleMotionServer __sharedInstance = null;
        public static SimpleMotionServer sharedInstance
        {
            get
            {
                if (__sharedInstance == null)
                {
                    __sharedInstance = FindObjectOfType<SimpleMotionServer>();
                    if (__sharedInstance == null)
                    {
                        GameObject go = new GameObject("SimpleMotionServer");
                        __sharedInstance = go.AddComponent<SimpleMotionServer>();
                        DontDestroyOnLoad(go);
                    }
                }
                return __sharedInstance;
            }
        }
        
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

        private void _InitSocket()
        {
            Debug.Log("Hosting on port " + port);

            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(new IPEndPoint(IPAddress.Any, port));
                _socket.Listen(5);
                _socket.BeginAccept(new AsyncCallback(OnClientConnect), _socket);
            }
            catch (Exception e)
            {
                Debug.LogError("Exception when attempting to host (" + port + "): " + e);
                if (_socket != null)
                {
                    _socket.BeginDisconnect(false, new AsyncCallback(OnEndHostComplete), _socket);
                    _socket = null;
                }
            }
        }

        void OnEndHostComplete(IAsyncResult result)
        {
            _socket = null;
        }

        void OnClientConnect(IAsyncResult result)
        {
            Debug.Log("Handling client connecting");
            if (_socket == null) return;
            try
            {
                Socket client = _socket.EndAccept(result);
                IPEndPoint endPoint = (IPEndPoint)client.RemoteEndPoint;
                Debug.Log("Client connected : " + endPoint.Address.ToString());

                _client = client;

                _socket.BeginAccept(new AsyncCallback(OnClientConnect), _socket);

                _client.BeginReceive(_buffer, 0, 13, SocketFlags.None, new System.AsyncCallback(OnReceive), null);
            }
            catch (Exception e)
            {
                Debug.LogError("Exception when accepting incoming connection: " + e);
            }
        }

        void OnReceive(IAsyncResult result)
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
            if (len >= 4)
            {
                int plen = 0;

                byte stx = _bufferList[0];
                byte pid = _bufferList[1];
                byte cmd = _bufferList[2];
                byte user = _bufferList[3];

                UserMotionInfo mi = null;
                if (user == 0x31) mi = user1;
                else if (user == 0x32) mi = user2;

                // MOTION
                if (cmd == 0x23 || cmd == 0x24)
                {
                    // read only 6 bytes!
                    if (len >= 6)
                    {
                        byte data = _bufferList[4];
                        byte etx = _bufferList[5];
                        UserMotionInfo.Type type = (UserMotionInfo.Type)data;

                        if (mi != null)
                        {
                            byte flag = 0x01;
                            if (mi.type == UserMotionInfo.Type.None &&
                               type != UserMotionInfo.Type.None)
                            {
                                MotionPacketServer resp = new MotionPacketServer();
                                resp.command = 0x23;
                                resp.user = mi.id;
                                resp.flag = flag;
                                resp.data = data;

                                byte[] arr = resp.ToByteArray();
                                _client.BeginSend(arr, 0, arr.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
                            }

                            mi.type = type;


                            plen = 6;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                // FIND
                else
                {
                }

                if (plen > 0)
                {
                    _bufferList.RemoveRange(0, plen);
                }
            }
        }

        public void StopMotion(int user)
        {
            UserMotionInfo mi = null;
            if (user == User1)
            {
                mi = user1;
            }
            else if (user == User2)
            {
                mi = user2;
            }

            if (mi != null)
            {
                MotionPacketServer packet = new MotionPacketServer();
                packet.command = 0x23;
                packet.user = mi.id;
                packet.data = (byte)mi.type;
                packet.flag = 0;

                if (_client != null)
                {
                    byte[] arr = packet.ToByteArray();
                    _client.BeginSend(arr, 0, arr.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
                }
                mi.type = UserMotionInfo.Type.None;
            }
        }

        void OnSend(IAsyncResult result)
        {
            if (_client != null)
            {
                _client.EndSend(result);
            }
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();

            PrintUserState(user1);
            PrintUserState(user2);

            GUILayout.EndVertical();
        }

        public void PrintUserState(UserMotionInfo user)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("User (" + user.id + ") State : ");
            GUILayout.Label(user.type.ToString());
            if (user.type != UserMotionInfo.Type.None)
            {
                if (GUILayout.Button("Stop!"))
                {
                    StopMotion(user.id);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}