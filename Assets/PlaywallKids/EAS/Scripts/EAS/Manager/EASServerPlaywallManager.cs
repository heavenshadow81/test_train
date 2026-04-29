using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace ML.PlaywallKids.EAS
{
    public class EASServerPlaywallManager : MonoBehaviour
    {
        #region Public variables
        public string IP = "127.0.0.1";
        public int port = 5000;

        public AvatarSkeleton user1Skeleton = null;
        public AvatarSkeleton user2Skeleton = null;
        #endregion

        #region Properties
        private static EASServerPlaywallManager __sharedInstance = null;
        public static EASServerPlaywallManager sharedInstance
        {
            get
            {
                if (__sharedInstance == null)
                {
                    __sharedInstance = FindObjectOfType<EASServerPlaywallManager>();
                    if (__sharedInstance == null)
                    {
                        GameObject go = new GameObject("PlayWallServer");
                        __sharedInstance = go.AddComponent<EASServerPlaywallManager>();
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

        private bool _mirroringAvatar = false;
        #endregion

        #region Constants
        public const int kBufferSize = 256;
        #endregion

        #region Unity Methods
        void Awake()
        {
            Messenger.AddListener(EASServerManager.kEventTypeMenuChanged, _OnMenuChange);
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
            Messenger.RemoveListener(EASServerManager.kEventTypeMenuChanged, _OnMenuChange);

            if (_client != null)
            {
                _client.Close();
                _client = null;
            }

            if (_socket != null)
            {
                _socket.BeginDisconnect(false, new System.AsyncCallback(OnEndHostComplete), _socket);
            }
        }
        #endregion

        #region Menu Change
        private void _OnMenuChange()
        {
            EASServerManager manager = EASServerManager.sharedInstance;
            if (manager != null)
            {
                var menu = EASServerManager.sharedInstance.menu;
                byte data = 0x00;

                switch (menu)
                {
                    case EASPacket.kTypeNone:
                        data = SyncInterfacePacket.DATA_MENU_NONE;
                        break;
                    case EASPacket.kTypePet:
                        data = SyncInterfacePacket.DATA_MENU_DRAGON;
                        break;
                    case EASPacket.kTypeKinect:
                        data = SyncInterfacePacket.DATA_MENU_AVATAR;
                        break;
                    case EASPacket.kTypeSketch:
                        data = SyncInterfacePacket.DATA_MENU_DRAWING;
                        break;
                    case EASPacket.kType3D:
                        data = SyncInterfacePacket.DATA_MENU_FREEDRAWING;
                        break;
                    case EASPacket.kTypeRobot:
                        data = SyncInterfacePacket.DATA_MENU_ROBOT;
                        break;
                }

                SendSelectedMenu(data);
            }
        }
        #endregion

        #region Socket Connections
        private void _InitSocket()
        {
            port = SettingsManager.port;

            Debug.Log("EASServerPlaywallManager - Hosting on port " + port);

            try
            {
                IPEndPoint myEndPoint = new IPEndPoint(IPAddress.Any, port);
                _socket = new Socket(myEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(myEndPoint);
                _socket.Listen(5);
                _socket.BeginAccept(new System.AsyncCallback(OnClientConnect), _socket);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Exception when attempting to host (" + port + "): " + e);
                if (_socket != null)
                {
                    _socket.BeginDisconnect(false, new System.AsyncCallback(OnEndHostComplete), _socket);
                }
            }
        }

        void OnEndHostComplete(System.IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;

            if (socket != null)
            {
                socket.EndDisconnect(result);
                socket.Close();
                socket = null;
            }
        }

        void OnClientConnect(System.IAsyncResult result)
        {
            Debug.Log("Handling client connecting");
            if (_socket == null) return;
            try
            {
                Socket client = _socket.EndAccept(result);
                IPEndPoint endPoint = (IPEndPoint)client.RemoteEndPoint;
                string address = endPoint.Address.ToString();

                Debug.Log("Client connected : " + address);

                _client = client;

                _socket.BeginAccept(new System.AsyncCallback(OnClientConnect), _socket);

                client.BeginReceive(_buffer, 0, kBufferSize, SocketFlags.None, new System.AsyncCallback(OnReceive), client);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Exception when accepting incoming connection: " + e);
            }
        }

        void OnSend(System.IAsyncResult result)
        {
            Socket client = result.AsyncState as Socket;
            if (client != null)
            {
                client.EndSend(result);
            }
        }

        void OnReceive(System.IAsyncResult result)
        {
            Socket client = result.AsyncState as Socket;
            if (client != null)
            {
                int len = client.EndReceive(result);
                for (int i = 0; i < len; i++)
                {
                    _bufferList.Add(_buffer[i]);
                }

                client.BeginReceive(_buffer, 0, kBufferSize, SocketFlags.None, new System.AsyncCallback(OnReceive), client);
            }
        }

        private void _ParsePackets()
        {
            SyncInterfacePacket packet = SyncInterfacePacket.Parse(_bufferList);
            if (packet != null)
            {
                _ProcessPacket(packet);
            }
        }

        private void _ProcessPacket(SyncInterfacePacket packet)
        {
            switch (packet.CMD)
            {
                case SyncInterfacePacket.CMD_AVATAR:
                    _ProcessAvatarPacket(packet);
                    break;
            }
        }
        #endregion

        private void _ProcessAvatarPacket(SyncInterfacePacket packet)
        {
            int offset = 0;
            byte code = packet.data[offset++];
            if (code == SyncInterfacePacket.DATA_AVATAR_CODE_COORD && packet.data.Length > 1)
            {
                int user1 = System.BitConverter.ToInt32(packet.data, offset);
                offset += 4;

                int user2 = System.BitConverter.ToInt32(packet.data, offset);
                offset += 4;

                Vector3[] user1Positions = new Vector3[AvatarSkeleton.jointCount];
                Vector3[] user2Positions = new Vector3[AvatarSkeleton.jointCount];

                // user 1 positions
                for (int i = 0; i < AvatarSkeleton.jointCount; i++)
                {
                    // x
                    int x = 1920 - System.BitConverter.ToInt32(packet.data, offset);
                    offset += 4;

                    // y
                    int y = 1080 - System.BitConverter.ToInt32(packet.data, offset);
                    offset += 4;

                    // z
                    int z = 3000 - System.BitConverter.ToInt32(packet.data, offset);
                    offset += 4;

                    user1Positions[i] = new Vector3(x, y, z);
                }

                // user 2 positions
                for (int i = 0; i < AvatarSkeleton.jointCount; i++)
                {
                    // x
                    int x = 1920 - (System.BitConverter.ToInt32(packet.data, offset));
                    offset += 4;

                    // y
                    int y = 1080 - System.BitConverter.ToInt32(packet.data, offset);
                    offset += 4;

                    // z
                    int z = 3000 - System.BitConverter.ToInt32(packet.data, offset);
                    offset += 4;

                    user2Positions[i] = new Vector3(x, y, z);
                }

                if (user1 > 0 && user1Skeleton != null)
                {
                    user1Skeleton.MoveJoints(user1Positions);
                }
                if (user2 > 0 && user2Skeleton != null)
                {
                    user2Skeleton.MoveJoints(user2Positions);
                }
            }
        }

        public void SendSelectedMenu(byte code)
        {
            if (_client != null)
            {
                SyncInterfacePacket packet = new SyncInterfacePacket();
                packet.PID = SyncInterfacePacket.PID_PLAYWALL;
                packet.CMD = SyncInterfacePacket.CMD_MENU;

                byte data = code;

                packet.data = new byte[1] { data };

                byte[] bytes = packet.ToByteArray();
                _client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new System.AsyncCallback(OnSend), _client);

                Debug.Log(string.Format("Menu {0} --> SyncInterface", code));
            }
        }
    }
}