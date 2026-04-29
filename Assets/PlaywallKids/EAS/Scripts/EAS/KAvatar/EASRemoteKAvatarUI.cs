using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.EAS
{
    public class EASRemoteKAvatarUI : EASAnimatablePanel
    {
        #region Enums
        public enum ConnectionState
        {
            None,
            Failed,
            Connecting,
            Connected
        }
        #endregion

        #region Public variables
        public UISprite connectionSprite;
        public UILabel connectionLabel;

        public EASCsTestPose testPose;

        public AudioClip audioClip;
        #endregion

        #region Properties
        public string IP
        {
            get
            {
                return PlayerPrefs.GetString("kavatar_ip", "127.0.0.1");
            }
        }

        public int port
        {
            get
            {
                return PlayerPrefs.GetInt("kavatar_port", 8000);
            }
        }

        private ConnectionState _connectionState = ConnectionState.None;
        public ConnectionState connectionState
        {
            get
            {
                return _connectionState;
            }
            private set
            {
                _connectionState = value;

                _RefreshUI();
            }
        }

        public bool initialized
        {
            get
            {
                return _connectionState == ConnectionState.Connected;
            }
        }

        public override bool connected
        {
            get
            {
                return _socket != null && _socket.connected;
            }
        }
        #endregion

        #region Private variables
        private EASSocket _socket;
        #endregion

        private void _RefreshUI()
        {
            switch (_connectionState)
            {
                case ConnectionState.None:
                    connectionSprite.color = Color.gray;
                    connectionLabel.text = "";
                    break;
                case ConnectionState.Failed:
                    connectionSprite.color = Color.red;
                    connectionLabel.text = "Connection Failed.";
                    break;
                case ConnectionState.Connecting:
                    connectionSprite.color = Color.yellow;
                    connectionLabel.text = "Connecting...";
                    break;
                case ConnectionState.Connected:
                    connectionSprite.color = Color.green;
                    connectionLabel.text = "Connection Success.";
                    break;
            }
        }

        public override void BeginShow()
        {
            connectionState = ConnectionState.None;

            Init();
        }

        public override void Deactive()
        {
            testPose.Stop();
            EASServerManager.sharedInstance.PlayBGM();

            base.Deactive();
        }

        public void Update()
        {
            if (connected)
            {
                EASPacket packet = _socket.Receive(EASPacket.kTypeRobot);
                if (packet == null) packet = _socket.Receive(EASPacket.kTypeNone);

                if (packet != null)
                {
                    if (packet.GetBool("data/init"))
                    {
                        connectionState = ConnectionState.Connected;
                    }
                    else if (packet.GetBool("data/play"))
                    {
                        connectionState = ConnectionState.Connected;
                    }
                    else if (packet.GetBool("data/stop"))
                    {
                        connectionState = ConnectionState.Connected;

                        testPose.Stop();
                        EASServerManager.sharedInstance.PlayBGM();
                    }

                    _socket.Disconnect();
                    _socket = null;
                }
            }
        }

        #region Commands
        public void Init()
        {
            EASPacket packet = new EASPacket();
            packet.type = EASPacket.kTypeRobot;
            packet.Set("data/init", true);

            _SendPacket(packet);
        }

        public void Play()
        {
            if (initialized)
            {
                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kTypeRobot;
                packet.Set("data/play", true);

                _SendPacket(packet, 1.5f);

                testPose.audioClip = audioClip;
                EASServerManager.sharedInstance.StopBGM();
                testPose.Play();
            }
        }

        public void Stop()
        {
            if (initialized)
            {
                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kTypeRobot;
                packet.Set("data/stop", true);

                _SendPacket(packet);
            }
        }
        #endregion

        #region Packet Management
        private void _SendPacket(EASPacket packet)
        {
            StartCoroutine(_SendPacketDelayed(packet, 0.0f));
        }

        private void _SendPacket(EASPacket packet, float delayTime)
        {
            StartCoroutine(_SendPacketDelayed(packet, delayTime));
        }

        private IEnumerator _SendPacketDelayed(EASPacket packet, float delayTime)
        {
            if (packet == null) yield return null;
            else
            {
                if (delayTime > 0.0f)
                {
                    yield return new WaitForSeconds(delayTime);
                }

                yield return null;

                connectionState = ConnectionState.Connecting;

                if (_socket != null)
                {
                    _socket.Send(packet);
                }
                else
                {
                    _socket = new EASSocket();
                    _socket.Connect(IP, port, (flag) =>
                    {
                        if (flag)
                        {
                            _socket.Send(packet);
                        }
                        else
                        {
                            if (_socket != null)
                            {
                                _socket.Disconnect();
                                _socket = null;
                            }

                            connectionState = ConnectionState.Failed;
                        }
                    });
                }
            }
        }
        #endregion
    }
}