using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.EAS
{
    public class EASAnimatablePanel : AnimatablePanel
    {
        public virtual EASSocket socket
        {
            get;
            set;
        }

        public virtual bool connected
        {
            get
            {
                return socket != null && socket.connected;
            }
        }
    }
}