using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.EAS
{
    public class EASRemoteCanvasControl : EASAnimatablePanel
    {
        #region Public variables
        public EASRemoteCanvas canvas;
        #endregion

        #region Properties
        public override EASSocket socket
        {
            get
            {
                return canvas.socket;
            }
            set
            {
                canvas.socket = value;
            }
        }
        #endregion

        public void Update()
        {
            if (socket == null || !socket.connected)
            {
                Hide();
            }
        }

        public override void Deactive()
        {
            base.Deactive();

            canvas.socket = null;
        }
    }
}