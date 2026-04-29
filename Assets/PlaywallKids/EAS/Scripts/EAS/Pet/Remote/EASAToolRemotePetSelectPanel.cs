using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.EAS
{
    public class EASAToolRemotePetSelectPanel : EASAToolPetSelectPanelBase
    {
        #region Properties
        private System.WeakReference _mainPanel = null;
        public EASAToolRemotePetPanel mainPanel
        {
            get
            {
                if (_mainPanel == null)
                {
                    return null;
                }
                return _mainPanel.Target as EASAToolRemotePetPanel;
            }
            set
            {
                if (_mainPanel == null)
                {
                    _mainPanel = new System.WeakReference(value);
                }
                else
                {
                    _mainPanel.Target = value;
                }
            }
        }
        #endregion

        public void ProcessPacket(EASPacket packet)
        {
            if (packet != null)
            {
                if (packet.Get("data/model") != null)
                {
                    string modelName = packet.GetString("data/model");
                    SelectCharacter(modelName);
                }
                if (packet.GetBool("data/command/next_step"))
                {
                    EASPacket response = new EASPacket();
                    response.type = EASPacket.kTypePet;
                    response.Set("data/command/next_step", true);
                    socket.Send(response);

                    mainPanel.NextStep();
                }
            }
        }
    }
}