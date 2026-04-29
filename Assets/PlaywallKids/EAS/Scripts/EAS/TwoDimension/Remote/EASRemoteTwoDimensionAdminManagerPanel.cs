using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASRemoteTwoDimensionAdminManagerPanel : AnimatablePanel
    {
        #region Properties
        private EASRemoteTwoDimensionAdmin _twoDimensionAdmin;
        public EASRemoteTwoDimensionAdmin twoDimensionAdmin
        {
            get
            {
                if (_twoDimensionAdmin == null)
                {
                    _twoDimensionAdmin = GetComponent<EASRemoteTwoDimensionAdmin>();
                    if (_twoDimensionAdmin == null)
                    {
                        _twoDimensionAdmin = GetComponentInChildren<EASRemoteTwoDimensionAdmin>();
                    }
                }
                return _twoDimensionAdmin;
            }
        }

        public List<EASSocket> sockets
        {
            get
            {
                if (twoDimensionAdmin != null)
                {
                    return twoDimensionAdmin.sockets;
                }

                return null;
            }
        }

        public bool connected
        {
            get
            {
                if (twoDimensionAdmin != null)
                {
                    return twoDimensionAdmin.connected;
                }

                return false;
            }
        }
        #endregion

        #region Methods
        public override void Deactive()
        {
            base.Deactive();

            RemoveAllClients();
        }

        public void AddClient(EASServerClientInfo client)
        {
            if (client != null && client.client != null && twoDimensionAdmin != null)
                twoDimensionAdmin.AddSocket(client.client);
        }

        public void RemoveClient(EASServerClientInfo client)
        {
            if (client != null && client.client != null && twoDimensionAdmin != null)
                twoDimensionAdmin.RemoveSocket(client.client);
        }

        public void RemoveAllClients()
        {
            if (twoDimensionAdmin != null)
                twoDimensionAdmin.RemoveAllSockets();
        }
        #endregion
    }
}