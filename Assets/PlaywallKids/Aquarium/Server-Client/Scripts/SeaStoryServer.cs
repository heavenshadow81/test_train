using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ML.PlaywallKids.Aquarium
{
    /*
     * 5월 시연용 바다여행 서버 클래스.
     */
    public class SeaStoryServer : MonoBehaviour
    {
        #region Properties
        private static SeaStoryServer __sharedInstance = null;
        public static SeaStoryServer sharedInstance
        {
            get
            {
                if (__sharedInstance == null)
                {
                    __sharedInstance = FindObjectOfType(typeof(SeaStoryServer)) as SeaStoryServer;
                    if (__sharedInstance == null)
                    {
                        GameObject go = new GameObject("SeaStoryServer");
                        __sharedInstance = go.AddComponent<SeaStoryServer>();
                        DontDestroyOnLoad(go);
                    }
                }
                return __sharedInstance;
            }
        }
        #endregion

        #region Private variables
        private Socket _socket;
        private List<SeaStoryServerClientSocket> _clients = new List<SeaStoryServerClientSocket>();
        #endregion

        #region Constants
        public const int Port = 5999;
        #endregion

        public void Start()
        {
            _InitSocket();
        }

        public void Update()
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                var client = _clients[i];
                if (client.packets.Count > 0)
                {
                    var packet = client.packets[0];
                    client.packets.RemoveAt(0);

                    SeaStoryTemplatePacketSave.Save(packet);
                    createFishesObj.Instance().SetFishesPath(packet.userId, "", packet.templateName, new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0), exAtoolFath.InitMode.PresentAndDontWait);

                    client.Cleanup();
                    _clients.Remove(client);
                    i--;
                }
            }
        }

        public void SaveTemplate(SeaStoryTemplatePacket packet)
        {
        }

        public void OnDestroy()
        {
            Debug.Log("SeaStoryServer.OnDestroy()");
            _CleanSocket();
        }

        private void _InitSocket()
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, Port);
                _socket.Bind((EndPoint)endPoint);
                _socket.Listen(5);
                _socket.BeginAccept(new System.AsyncCallback(OnClientConnect), null);

                Debug.Log("Hosting on port " + Port);
            }
            catch (System.Exception e)
            {
                Debug.Log("Socket initialization failed...");
            }
        }

        private void _CleanSocket()
        {
            _socket.BeginDisconnect(false, new System.AsyncCallback(OnEndHostComplete), _socket);
            _socket = null;
        }

        public void OnClientConnect(System.IAsyncResult ar)
        {
            if (_socket == null) return;

            // accept client
            Socket socket = _socket.EndAccept(ar);

            if (socket != null)
            {
                SeaStoryServerClientSocket client = new SeaStoryServerClientSocket(socket);
                _clients.Add(client);
            }

            _socket.BeginAccept(new System.AsyncCallback(OnClientConnect), null);
        }

        public void OnEndHostComplete(System.IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            if (socket != null)
            {
                socket.EndDisconnect(ar);
            }
            socket = null;
        }
    }
}