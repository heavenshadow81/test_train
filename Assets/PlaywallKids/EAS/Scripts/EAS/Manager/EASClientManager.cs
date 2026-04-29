using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.EAS
{
    public class EASClientManager : MonoBehaviour
    {
        #region Public variables
        public string IP = "192.0.0.1";
        public int port = 5999;

        public EASClientConnectionPopup connectionPopup;
        public EASClientLoading loading;

        public UIButton menuButton;
        public UIButton disconnectButton;
        public UIButton quitButton;

        public EASClientCanvasControl canvasPanel;
        public EASAnimatablePanel aToolPetPanel;
        public EASAToolClient3DPanel aTool3DPanel;
        public EASClientKAvatarUI kavatarPanel;
        public EASAnimatablePanel aToolPetMotionPanel;
        public EASAnimatablePanel twoDimensionPanel;

        public GameObject background;
        #endregion

        #region Properties
        private static EASClientManager _currentManager;
        public static EASClientManager currentManager
        {
            get
            {
                return _currentManager;
            }
        }

        public bool connected
        {
            get
            {
                return _socket != null && _socket.connected;
            }
        }

        private bool _root = false;
        public bool root
        {
            get
            {
                return _root;
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
                        value = EASPacket.kTypeNone;

                    _menu = value;

                    if (_currentPanel != null)
                    {
                        _currentPanel.Hide();
                        _currentPanel = null;
                    }

                    _PrepareMenu();
                }
            }
        }
        #endregion

        #region Constants
        public const float kConnectionCheckMaxWaitTime = 10.0f;
        #endregion

        #region Private variables
        // Socket
        private EASSocket _socket;

        // Current Panel
        private EASAnimatablePanel _currentPanel;

        // Flags
        private bool _destroying = false;
        private bool _sentDisconnectPacket = false;

        // Time Check
        private float _connectionCheckWaitTime = 0.0f;
        #endregion

        #region Unity Methods
        public IEnumerator Start()
        {
            // initialize singleton variable
            _currentManager = this;

            // initialize Loom
            Loom.Initialize();

            // set target frame as 30
            Application.targetFrameRate = 30;

            // enable/disables mouse input
            CustomInput.receivesMouseInput = ML.PlaywallKids.Common.CommonSettings.receivesMouseInput;

            // don't do touch error correction. EAS apps doesn't need to do that.
            CustomInput.enablesErrorCorrection = false;

            // initialize accessory manager (for speed)
            AccessoryManager.Init();

            // Deactives all panels
            loading.Deactive();
            connectionPopup.Deactive();
            canvasPanel.Deactive();
            aToolPetPanel.Deactive();
            aTool3DPanel.Deactive();
            kavatarPanel.Deactive();
            aToolPetMotionPanel.Deactive();
            twoDimensionPanel.Deactive();

            NGUITools.SetActive(menuButton.gameObject, false);
            NGUITools.SetActive(disconnectButton.gameObject, false);
            NGUITools.SetActive(quitButton.gameObject, false);

            // wait for seconds
            yield return new WaitForSeconds(0.5f);

            // shows connection popup
            ShowConnectionPopup();
        }

        public void OnDestroy()
        {
            _destroying = true;

            Disconnect();
        }

        public void Update()
        {
            if (connected)
            {
                if (CheckConnection())
                {
                    ProcessReceivedPackets();
                }
            }
        }
        #endregion

        #region Connection
        public void ShowConnectionPopup()
        {
            NGUITools.SetActive(menuButton.gameObject, false);
            NGUITools.SetActive(disconnectButton.gameObject, false);
            NGUITools.SetActive(quitButton.gameObject, false);

            ShowLoading();
            connectionPopup.Show();
        }

        public void Connect(System.Action<bool> handler = null)
        {
            if (_socket == null)
            {
                _socket = new EASSocket();
            }

            _sentDisconnectPacket = false;

            _socket.Connect(IP, port, (flag) =>
            {
                if (handler != null)
                {
                    handler(flag);

                    if (flag)
                    {
                        HideLoading();
                        NGUITools.SetActive(disconnectButton.gameObject, true);
                        NGUITools.SetActive(quitButton.gameObject, true);
                    }
                }
            });
        }

        public bool CheckConnection()
        {
            bool result = true;

            // Get check connection packet
            EASPacket packet = _socket.Receive(EASPacket.kTypeCheckConnection);

            if (packet != null)
            {
                _connectionCheckWaitTime = 0.0f;

                _socket.Send(packet);
            }
            else
            {
                if (_connectionCheckWaitTime >= kConnectionCheckMaxWaitTime)
                {
                    _connectionCheckWaitTime = 0.0f;
                    Disconnect();
                    result = false;
                }
                else
                {
                    _connectionCheckWaitTime += Time.deltaTime;
                }
            }

            return result;
        }

        public void Disconnect()
        {
            if (connected)
            {
                if (!_sentDisconnectPacket)
                {
                    _sentDisconnectPacket = true;

                    EASPacket packet = new EASPacket();
                    packet.type = EASPacket.kTypeDisconnect;

                    if (!_destroying)
                    {
                        ShowLoading();
                    }

                    _socket.Send(packet, (flag) =>
                    {
                        _socket.Disconnect();
                        _socket = null;

                        if (!_destroying)
                        {
                            menu = EASPacket.kTypeNone;
                            ShowConnectionPopup();
                        }
                    });
                }
            }
        }
        #endregion

        #region Packet process
        public void ProcessReceivedPackets()
        {
            // root type
            _ProcessRootTypePackets();

            // menu type
            _ProcessMenuTypePackets();
        }

        private void _ProcessRootTypePackets()
        {
            EASPacket packet = _socket.Receive(EASPacket.kTypeRoot);
            if (packet != null)
            {
                // get flag value (default is false)
                _root = packet.GetBool("data/flag");

                // set menu button activate state
                NGUITools.SetActive(menuButton.gameObject, _root);

                // Log
                Debug.Log(string.Format("EASClientManager - Changed menu button active state as {0}.", _root));
            }
        }

        private void _ProcessMenuTypePackets()
        {
            EASPacket packet = _socket.Receive(EASPacket.kTypeMenu);
            if (packet != null)
            {
                // get new menu
                string newMenu = packet.GetString("data/menu");

                // actually change menu
                menu = newMenu;

                // hide loading screen
                HideLoading();

                // send response packet
                packet = new EASPacket();
                packet.type = EASPacket.kTypeMenu;
                packet.Set("data/result", true);
                _socket.Send(packet);
            }
        }
        #endregion

        #region Menu
        private void _PrepareMenu()
        {
            switch (_menu)
            {
                case EASPacket.kTypeSketch:
                    _currentPanel = canvasPanel;
                    Debug.Log("EASClientManager - Show Sketch Panel.");
                    break;
                case EASPacket.kTypePet:
                    _currentPanel = aToolPetPanel;
                    Debug.Log("EASClientManager - Show Pet Panel.");
                    break;
                case EASPacket.kType3D:
                    _currentPanel = aTool3DPanel;
                    Debug.Log("EASClientManager - Show 3D Panel.");
                    break;
                case EASPacket.kTypeKinect:
                    _currentPanel = null;
                    Debug.Log("EASClientManager - Show Kinect Panel.");
                    break;
                case EASPacket.kTypeRobot:
                    _currentPanel = kavatarPanel;
                    Debug.Log("EASClientManager - Show KAvatar.");
                    break;
                case EASPacket.kTypePetMotion:
                    _currentPanel = aToolPetMotionPanel;
                    Debug.Log("EASClientManager - Show Pet Motion.");
                    break;
                case EASPacket.kType2D:
                    _currentPanel = twoDimensionPanel;
                    Debug.Log("EASClientManager - Show 2D Panel.");
                    break;
                default:
                    _menu = EASPacket.kTypeNone;
                    _currentPanel = null;

                    Debug.Log("EASClientManager - Hided all panels.");
                    break;
            }

            // hide background only if current menu is 2D.
            // this code prevents showing unexpected rendering when playing sand drawing.
            if (_menu.Equals(EASPacket.kType2D))
            {
                if (background != null) background.SetActive(false);
            }
            else
            {
                if (background != null) background.SetActive(true);
            }

            if (_currentPanel != null)
            {
                _currentPanel.socket = _socket;
                _currentPanel.Show();
            }
        }

        public void RootCanvas()
        {
            RootMenuSelect(EASPacket.kTypeSketch);
        }

        public void Root3D()
        {
            RootMenuSelect(EASPacket.kType3D);
        }

        public void Root2D()
        {
            RootMenuSelect(EASPacket.kType2D);
        }

        public void RootPet()
        {
            RootMenuSelect(EASPacket.kTypePet);
        }

        public void RootKinect()
        {
            RootMenuSelect(EASPacket.kTypeKinect);
        }

        public void RootRobot()
        {
            RootMenuSelect(EASPacket.kTypeRobot);
        }

        public void RootPetMotion()
        {
            RootMenuSelect(EASPacket.kTypePetMotion);
        }

        public void RootNone()
        {
            RootMenuSelect(EASPacket.kTypeNone);
        }

        public void RootMenuSelect(string menu)
        {
            if (!_menu.Equals(menu))
            {
                if (connected)
                {
                    // show loading screen
                    ShowLoading();

                    // send menu packet to server
                    EASPacket packet = new EASPacket();
                    packet.type = EASPacket.kTypeMenu;
                    packet.Set("data/menu", menu);
                    _socket.Send(packet, (flag) =>
                    {
                        if (!flag)
                        {
                            // disconnect
                            Disconnect();
                        }
                    });
                }
            }
        }
        #endregion

        #region Static Methods
        public static void ShowLoading()
        {
            if (currentManager != null && currentManager.loading != null)
            {
                currentManager.loading.Show();
            }

            Debug.Log("EASClientManager.ShowLoading()");
        }

        public static void HideLoading()
        {
            if (currentManager != null && currentManager.loading != null)
            {
                currentManager.loading.Hide();
            }

            Debug.Log("EASClientManager.HideLoading()");
        }
        #endregion

        #region Quit
        public void Quit()
        {
#if UNITY_EDITOR
            Debug.Log("EASClientManager.Quit()");
#endif

            Application.Quit();
        }
        #endregion
    }
}