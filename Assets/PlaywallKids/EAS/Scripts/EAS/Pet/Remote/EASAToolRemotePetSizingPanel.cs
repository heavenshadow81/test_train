using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASAToolRemotePetSizingPanel : EASAToolPetSizingPanelBase
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

        private System.WeakReference _modelControl = null;
        public EASRemoteModelControl modelControl
        {
            get
            {
                if (_modelControl == null)
                {
                    return null;
                }
                return _modelControl.Target as EASRemoteModelControl;
            }
            set
            {
                if (_modelControl == null)
                {
                    _modelControl = new System.WeakReference(value);
                }
                else
                {
                    _modelControl.Target = value;
                }
            }
        }
        #endregion

        public void ProcessPacket(EASPacket packet)
        {
            if (packet != null)
            {
                if (packet.Get("data/bone") != null)
                {
                    List<object> list = packet.GetList("data/bone");
                    foreach (object obj in list)
                    {
                        Dictionary<string, object> boneInfo = obj as Dictionary<string, object>;

                        string boneName = null;
                        float boneScale = 1.0f;

                        if (boneInfo.ContainsKey("name") && boneInfo["name"] != null)
                        {
                            boneName = boneInfo["name"].ToString();
                        }
                        if (boneInfo.ContainsKey("scale") && boneInfo["scale"] != null)
                        {
                            boneScale = float.Parse(boneInfo["scale"].ToString());
                        }

                        if (dragon != null && !string.IsNullOrEmpty(boneName))
                        {
                            dragon.SetBoneScale(boneName, boneScale);
                        }
                    }
                }
                if (packet.Get("data/acc") != null)
                {
                    List<object> list = packet.GetList("data/acc");
                    foreach (object obj in list)
                    {
                        Dictionary<string, object> accInfo = obj as Dictionary<string, object>;

                        string accName = null;
                        string boneName = null;

                        if (accInfo.ContainsKey("name") && accInfo["name"] != null)
                        {
                            accName = accInfo["name"].ToString();
                        }
                        if (accInfo.ContainsKey("bone") && accInfo["bone"] != null)
                        {
                            boneName = accInfo["bone"].ToString();
                        }

                        if (dragon != null && !string.IsNullOrEmpty(boneName) && !string.IsNullOrEmpty(accName))
                        {
                            SetAccessory(accName);
                        }
                    }
                }
                if (packet.GetBool("data/command/next_step"))
                {
                    EASPacket response = new EASPacket();
                    response.type = EASPacket.kTypePet;
                    response.Set("data/command/next_step", true);
                    socket.Send(response);
                    mainPanel.NextStep();
                }

                if (modelControl != null)
                {
                    modelControl.ProcessPacket(packet);
                }
            }
        }
    }
}