using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    /// <summary>
    /// This class extends remote canvas for accepting many client sockets.
    /// </summary>
    public class EASRemoteMultiCanvas : EASRemoteCanvas
    {
        #region Properties
        private List<EASSocket> _sockets = new List<EASSocket>();
        public List<EASSocket> sockets
        {
            get
            {
                return _sockets;
            }
            set
            {
                _sockets.Clear();

                if (value != null)
                {
                    _sockets.AddRange(value);
                }
            }
        }

        public override EASSocket socket
        {
            get
            {
                if (_sockets.Count > 0)
                    return _sockets[0];

                return null;
            }
            set
            {
                if (value != null)
                {
                    if (_sockets.Count > 0)
                        _sockets[0] = value;
                    else
                        _sockets.Add(value);
                }
            }
        }

        public override bool connected
        {
            get
            {
                bool val = false;
                for (int i = 0; i < _sockets.Count; i++)
                    val = val || _sockets[i].connected;
                return val;
            }
        }
        #endregion

        #region Unity Methods
        public override void Update()
        {
            if (wantsPaint && connected)
            {
                for (int i = 0; i < _sockets.Count; i++)
                {
                    EASSocket socket = _sockets[i];

                    while (true)
                    {
                        EASPacket packet = socket.Receive(EASPacket.kTypeSketch);
                        if (packet != null)
                        {
                            if (packet.Get("data/drawing") != null)
                            {
                                int touchCount = packet.GetInt("data/drawing/count");

                                for (int j = 0; j < touchCount; j++)
                                {
                                    string path = string.Format("data/drawing/{0}/touch_id", j);
                                    int touchId = packet.GetInt(path);
                                    int clientId = i;

                                    touchId = (clientId << 16) | touchId;
                                    packet.Set(path, touchId);
                                }
                            }

                            Dictionary<string, object> dict = packet.data;

                            _ParseDataAndExecute(dict);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            _sockets.Clear();
            _sockets = null;

            base.OnDestroy();
        }
        #endregion
    }
}