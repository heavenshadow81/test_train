using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    using TwoDimensionDrawScene;

    public class EASRemoteTwoDimensionSandDraw : TwoDimensionSandDraw
    {
        #region Properties
        public List<EASSocket> sockets
        {
            get
            {
                if (_admin != null)
                    return _admin.sockets;

                return null;
            }
        }

        public bool connected
        {
            get
            {
                if (_admin != null)
                    return _admin.connected;

                return false;
            }
        }
        #endregion

        #region Private variables
        private EASRemoteTwoDimensionAdmin _admin;

        private Dictionary<EASSocket, bool> _clientEventReadyDict = new Dictionary<EASSocket, bool>();
        #endregion

        #region Constants
        public const string kType2DSandDraw = "2d.sand_draw";
        #endregion

        #region Methods
        public override bool PlayStart()
        {
            if (_admin == null)
                _admin = GetComponentInParent<EASRemoteTwoDimensionAdmin>();

            return base.PlayStart();
        }

        public override bool PlayEnd()
        {
            _admin = null;
            _clientEventReadyDict.Clear();

            return base.PlayEnd();
        }

        public virtual void Update()
        {
            if (connected)
            {
                bool eventReady = true;
                if (_clientEventReadyDict.Count > 0)
                {
                    for (int i = 0; i < sockets.Count; i++)
                    {
                        bool ready = false;
                        _clientEventReadyDict.TryGetValue(sockets[i], out ready);
                        eventReady = eventReady && ready;
                    }

                    eventReady = eventReady && !bEvent;
                }
                else
                    eventReady = false;

                for (int i = 0; i < sockets.Count; i++)
                {
                    EASSocket socket = sockets[i];

                    if (socket != null && socket.connected)
                    {
                        EASPacket packet = socket.Receive(kType2DSandDraw);

                        if (packet != null)
                        {
                            if (packet.GetBool("data/event_ready"))
                            {
                                _clientEventReadyDict[socket] = true;
                                continue;
                            }
                            if (packet.GetBool("data/event"))
                            {
                                EASPacket response = new EASPacket();
                                response.type = kType2DSandDraw;
                                response.Set("data/event/result", eventReady);

                                if (eventReady)
                                {
                                    GenerateEvent();

                                    for (int j = 0; j < sockets.Count; j++)
                                    {
                                        if (sockets[j] != null && sockets[j].connected)
                                        {
                                            sockets[j].Send(response);
                                        }
                                    }

                                    break;
                                }
                                else
                                {
                                    socket.Send(response);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}