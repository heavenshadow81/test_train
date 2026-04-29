using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASAToolRemotePetMotionMakerPanel : EASAToolPetMotionMakerBase
    {
        #region Properties
        private System.WeakReference _mainPanel = null;
        public EASAToolRemotePetMotionPanel mainPanel
        {
            get
            {
                if (_mainPanel == null)
                {
                    return null;
                }
                return _mainPanel.Target as EASAToolRemotePetMotionPanel;
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
                if (packet.GetBool("data/command/next_step"))
                {
                    // check the anim list
                    var animListObj = packet.Get("data/anim_list");
                    if (animListObj != null)
                    {
                        List<string> animList = packet.GetList<string>("data/anim_list");
                        if (animList != null)
                        {
                            Play(animList);
                        }
                    }

                    // send response packet
                    EASPacket response = new EASPacket();
                    response.type = EASPacket.kTypePetMotion;
                    response.Set("data/command/next_step", true);
                    socket.Send(response);

                    // hide main panel
                    mainPanel.NextStep();
                }
                else if (packet.Get("data/anim_list") != null)
                {
                    List<string> animList = packet.GetList<string>("data/anim_list");

                    Play(animList);
                }
                else if (packet.GetBool("data/pause"))
                {
                    Pause();
                }
                else if (packet.GetBool("data/resume"))
                {
                    Resume();
                }
                else if (packet.GetBool("data/stop"))
                {
                    Stop();
                }
            }
        }
    }
}