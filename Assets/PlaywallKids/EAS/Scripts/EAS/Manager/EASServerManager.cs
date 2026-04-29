using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASServerClientInfo
    {
        #region Public variables
        #endregion

        #region Properties
        private EASSocket _client;
        public EASSocket client
        {
            get
            {
                return _client;
            }
        }

        public bool connected
        {
            get
            {
                return _client != null && _client.connected;
            }
        }

        private bool _root = false;
        public bool root
        {
            get
            {
                return _root;
            }
            set
            {
                _root = value;
                if (_client != null && _client.connected)
                {
                    EASPacket packet = new EASPacket();
                    packet.type = EASPacket.kTypeRoot;
                    packet.Set("data/flag", value);
                    _client.Send(packet);
                }
            }
        }

        private string _menu = EASPacket.kTypeNone;
        public string menu
        {
            get
            {
                return _menu;
            }
            set
            {
                if (!_menu.Equals(value))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        value = EASPacket.kTypeNone;
                    }

                    _menu = value;
                }
            }
        }

        private bool _changingMenu = false;
        public bool changingMenu
        {
            get
            {
                return _changingMenu;
            }
            set
            {
                _changingMenu = value;
            }
        }
        #endregion

        #region Constants
        public const float kConnectionCheckSendTime = 1.0f;
        public const float kConnectionCheckMaxWaitTime = 10.0f;
        #endregion

        #region Private variable
        private float _connectionCheckSendElapsedTime = 0.0f;
        private float _connectionCheckResponseElapsedTime = 0.0f;
        #endregion

        public EASServerClientInfo(EASSocket newClient)
        {
            _client = newClient;
        }

        public void Send(EASPacket packet)
        {
            if (connected && packet != null)
            {
                _client.Send(packet);
            }
        }

        public EASPacket Receive(string type)
        {
            EASPacket packet = null;

            if (_client != null)
            {
                packet = _client.Receive(type);
            }

            return packet;
        }

        public bool CheckConnection(float deltaTime)
        {
            if (connected)
            {
                EASPacket packet = null;

                // send connection check packet to client
                if (_connectionCheckSendElapsedTime >= kConnectionCheckSendTime)
                {
                    _connectionCheckSendElapsedTime -= kConnectionCheckSendTime;

                    packet = new EASPacket();
                    packet.type = EASPacket.kTypeCheckConnection;
                    _client.Send(packet);
                }
                else
                {
                    _connectionCheckSendElapsedTime += deltaTime;
                }

                // receive responds
                packet = _client.Receive(EASPacket.kTypeCheckConnection);
                if (packet != null)
                {
                    _connectionCheckResponseElapsedTime = 0.0f;
                }
                else
                {
                    _connectionCheckResponseElapsedTime += deltaTime;
                    if (_connectionCheckResponseElapsedTime >= kConnectionCheckMaxWaitTime)
                    {
                        return false;
                    }
                }

                // disconnect?
                packet = _client.Receive(EASPacket.kTypeDisconnect);
                if (packet != null)
                {
                    Disconnect();
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Disconnect()
        {
            if (connected)
            {
                _client.Disconnect();
            }
        }

        ~EASServerClientInfo()
        {
            Disconnect();
            _client = null;
        }
    }

    public class EASServerManager : MonoBehaviour
    {
        #region Public variables
        // socket informations
        public int port = 5999;

        // panel manager
        public EASServerPanelManager panelManager;

        // robot ui
        public EASRemoteKAvatarUI kavatarUI;

        // avatar ui
        public EASAnimatablePanel avatarUI;

        // two dimension admin
        public EASRemoteTwoDimensionAdminManagerPanel twoDimensionAdmin;

        // bgm
        public AudioSource BGM;
        #endregion

        #region Properties
        private static EASServerManager _sharedInstance = null;
        public static EASServerManager sharedInstance
        {
            get
            {
                if (_sharedInstance == null)
                {
                    _sharedInstance = FindObjectOfType<EASServerManager>();
                }
                return _sharedInstance;
            }
        }

        private EASSocket _socket = null;
        public EASSocket socket
        {
            get
            {
                return _socket;
            }
        }

        public bool connected
        {
            get
            {
                return (_socket != null && _socket.connected);
            }
        }

        private string _menu = EASPacket.kTypeNone;
        public string menu
        {
            get
            {
                return _menu;
            }
            private set
            {
                if (!_menu.Equals(value))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        value = EASPacket.kTypeNone;
                    }

                    _menu = value;
                    _changingMenuOfClients = true;

                    Messenger.Broadcast(kEventTypeMenuChanged, MessengerMode.DONT_REQUIRE_LISTENER);

                    // menu change request will be processed in _ProcessMenuTypePackets()
                }

                // prepare panels
                PrepareMenu();
            }
        }
        #endregion

        #region Constants
        public const string kEventTypeMenuChanged = "eas_server_manager_event_menu_changed";
        #endregion

        #region Private variables
        // Client sockets
        private List<EASServerClientInfo> _clients = new List<EASServerClientInfo>();

        // Handling menu
        private bool _changingMenuOfClients = false;

        // Is showing two dimension contents?
        private bool _showingTwoDimension = false;
        #endregion

        public void Start()
        {
            if (panelManager == null)
            {
                panelManager = FindObjectOfType<EASServerPanelManager>();
            }

            if (BGM == null)
            {
                GameObject go = GameObject.Find("BGM");
                if (go != null)
                {
                    BGM = go.GetComponent<AudioSource>();
                }
            }

            Loom.Initialize();

            AccessoryManager.Init();
            SettingsManager.Load();

            kavatarUI.Deactive();
            avatarUI.Deactive();
            twoDimensionAdmin.Deactive();

            Host();

            Messenger.Broadcast(kEventTypeMenuChanged, MessengerMode.DONT_REQUIRE_LISTENER);
        }

        public void OnDestroy()
        {
            Disconnect();
        }

        public void Host()
        {
            if (_socket == null)
            {
                _socket = new EASSocket();
            }

            if (!connected)
            {
                _socket.Host("", port, (flag) =>
                {
                    if (flag)
                    {
                        Debug.Log("EASServerManager - Hosting on port " + port);
                    }
                    else
                    {
                        Debug.Log("Host failed.");
                    }
                },
                (client) =>
                {
                // Log
                Debug.Log("Client (" + client.socket.RemoteEndPoint.ToString() + ") Connected!");

                // Add Client
                EASServerClientInfo clientInfo = new EASServerClientInfo(client);
                    _clients.Add(clientInfo);

                // Set as root if this client is the first of the list
                if (_clients.Count == 1)
                    {
                        _clients[0].root = true;
                    }

                // set panel manager client count
                if (panelManager != null)
                    {
                        panelManager.AddClient(clientInfo);
                    }

                // change two dimension admin socket state
                if (twoDimensionAdmin != null &&
                       _menu.Equals(EASPacket.kType2D) &&
                       twoDimensionAdmin.gameObject.activeSelf)
                    {
                        twoDimensionAdmin.AddClient(clientInfo);
                    }

                // change the client menu automatically
                _changingMenuOfClients = true;
                });
            }
        }

        public void Update()
        {
            // Process Packets
            for (int i = 0; i < _clients.Count; i++)
            {
                var client = _clients[i];
                if (client.CheckConnection(Time.deltaTime))
                {
                    ProcessReceivedPackets(client);
                }
                else
                {
                    DisconnectClient(client);
                }
            }

            // Check if all clients have changed the menu.
            if (_changingMenuOfClients)
            {
                bool allChanged = true;

                for (int i = 0; i < _clients.Count; i++)
                {
                    allChanged = allChanged && menu.Equals(_clients[i].menu);
                }

                _changingMenuOfClients = !allChanged;
            }
        }

        public void ProcessReceivedPackets(EASServerClientInfo client)
        {
            _ProcessMenuTypePackets(client);
            _ProcessRobotTypePackets(client);
        }

        private void _ProcessMenuTypePackets(EASServerClientInfo client)
        {
            EASPacket packet = client.Receive(EASPacket.kTypeMenu);
            if (packet != null)
            {
                bool result = packet.GetBool("data/result");

                if (result)
                {
                    client.changingMenu = false;
                    client.menu = menu;
                }
                else
                {
                    string newMenu = packet.GetString("data/menu");
                    if (client.root)
                    {
                        menu = newMenu;
                    }
                }
            }

            // actually menu change request processes here!
            if (_changingMenuOfClients && !client.changingMenu && !client.menu.Equals(menu))
            {
                packet = new EASPacket();
                packet.type = EASPacket.kTypeMenu;
                packet.Set("data/menu", menu);
                client.Send(packet);

                client.changingMenu = true;
            }

            if (menu.Equals(EASPacket.kTypeRobot))
            {
                EASServerMapManager.sharedInstance.ShowKAvatar();

                kavatarUI.Show();
                avatarUI.Hide();
                twoDimensionAdmin.Hide();
            }
            else
            {
                EASServerMapManager.sharedInstance.ShowDragonPark();
                kavatarUI.Hide();

                if (menu.Equals(EASPacket.kTypeKinect))
                {
                    panelManager.enabled = true;
                    avatarUI.Show();
                    twoDimensionAdmin.Hide();
                }
                else if (menu.Equals(EASPacket.kType2D))
                {
                    avatarUI.Hide();

                    for (int i = 0, cnt = _clients.Count; i < cnt; i++)
                    {
                        twoDimensionAdmin.AddClient(_clients[i]);
                    }

                    panelManager.enabled = false;
                    twoDimensionAdmin.Show();

                    EASServerMapManager.sharedInstance.HideDragonPark();
                    EASServerMapManager.sharedInstance.HideKAvatar();
                    StopBGM();
                    _showingTwoDimension = true;
                }
                else
                {
                    panelManager.enabled = true;
                    avatarUI.Hide();
                    twoDimensionAdmin.Hide();
                }

                if (!menu.Equals(EASPacket.kType2D) && _showingTwoDimension)
                {
                    _showingTwoDimension = false;

                    var mapMgr = EASServerMapManager.sharedInstance;
                    mapMgr.StartCoroutine(mapMgr.Tween(true));

                    if (menu.Equals(EASPacket.kTypeRobot))
                        EASServerMapManager.sharedInstance.ShowKAvatar();
                    else
                    {
                        PlayBGM();
                        EASServerMapManager.sharedInstance.ShowDragonPark();
                    }
                }
            }
        }

        private void _ProcessRobotTypePackets(EASServerClientInfo client)
        {
            EASPacket packet = client.Receive(EASPacket.kTypeRobot);
            if (packet != null && kavatarUI.isShowing)
            {
                if (packet.GetBool("data/play"))
                {
                    kavatarUI.Play();
                }
                else if (packet.GetBool("data/stop"))
                {
                    kavatarUI.Stop();
                }
            }
        }

        public void DisconnectClient(EASServerClientInfo client)
        {
            _clients.Remove(client);
            if (_clients.Count > 0)
            {
                _clients[0].root = true;
            }

            // prepare panels
            if (panelManager != null)
            {
                panelManager.RemoveClient(client);
            }

            // change two dimension socket state
            if (twoDimensionAdmin != null &&
               _menu.Equals(EASPacket.kType2D) &&
               twoDimensionAdmin.gameObject.activeSelf)
            {
                twoDimensionAdmin.RemoveClient(client);
            }
        }

        public void Disconnect()
        {
            foreach (EASServerClientInfo client in _clients)
            {
                if (client.connected)
                {
                    EASPacket packet = new EASPacket();
                    packet.type = EASPacket.kTypeDisconnect;
                    client.Send(packet);
                }
                client.Disconnect();
            }
            _clients.Clear();

            if (_socket != null)
            {
                _socket.Disconnect();
                _socket = null;
            }
        }

        public void PrepareMenu()
        {
            EASSocket[] sockets = new EASSocket[_clients.Count];
            for (int i = 0; i < sockets.Length; i++)
            {
                var client = _clients[i];
                sockets[i] = client.client;
            }

            if (panelManager != null)
            {
                panelManager.PrepareMenu(_menu);
            }
        }

        public void PlayBGM()
        {
            if (BGM != null)
            {
                BGM.Play();
            }
        }

        public void StopBGM()
        {
            if (BGM != null)
            {
                BGM.Pause();
            }
        }
    }
}