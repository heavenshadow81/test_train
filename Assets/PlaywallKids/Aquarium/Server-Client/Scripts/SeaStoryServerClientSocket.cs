using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ML.PlaywallKids.Aquarium
{
    public class SeaStoryServerClientSocket
    {
        #region Constants
        private const int kBufferSize = 262144;
        #endregion

        #region Properties
        public bool isConnected
        {
            get
            {
                return (_socket != null);
            }
        }

        private List<SeaStoryTemplatePacket> _packets = new List<SeaStoryTemplatePacket>();
        public List<SeaStoryTemplatePacket> packets
        {
            get
            {
                return _packets;
            }
        }
        #endregion

        #region Private variables
        private Socket _socket = null;
        private byte[] _buffer = new byte[kBufferSize];
        private List<byte> _accumBuffer = new List<byte>();
        #endregion

        public SeaStoryServerClientSocket(Socket socket)
        {
            _socket = socket;

            if (_socket != null)
            {
                _socket.BeginReceive(_buffer, 0, kBufferSize, SocketFlags.None, new System.AsyncCallback(OnReceive), null);
            }
        }

        public void Cleanup()
        {
            _CleanSocket();
        }

        ~SeaStoryServerClientSocket()
        {
            Cleanup();

            Debug.Log("~SeaStoryServerClientSocket()");
        }

        private void _CleanSocket()
        {
            if (_socket != null)
            {
                _socket.BeginDisconnect(false, new System.AsyncCallback(OnSocketDisconnect), _socket);
            }
            _socket = null;
        }

        public void OnReceive(System.IAsyncResult ar)
        {
            if (_socket != null)
            {
                int size = _socket.EndReceive(ar);
                for (int i = 0; i < size; i++)
                {
                    _accumBuffer.Add(_buffer[i]);
                }

                if (_packets.Count == 0)
                {
                    _Process();

                    if (_socket != null)
                    {
                        _socket.BeginReceive(_buffer, 0, kBufferSize, SocketFlags.None, new System.AsyncCallback(OnReceive), null);
                    }
                }
            }
        }

        private void _Process()
        {
            var packet = SeaStoryTemplatePacket.Parse(_accumBuffer);
            if (packet != null)
            {
                _packets.Add(packet);
            }
        }

        public void OnSocketDisconnect(System.IAsyncResult ar)
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