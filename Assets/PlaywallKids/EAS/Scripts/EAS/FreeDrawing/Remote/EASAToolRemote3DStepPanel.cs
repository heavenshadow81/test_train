using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.EAS
{
    public class EASAToolRemote3DStepPanel : EASATool3DStepPanelBase
    {
        #region Properties
        private System.WeakReference _mainPanel;
        public EASAToolRemote3DPanel mainPanel
        {
            get
            {
                if (_mainPanel != null)
                {
                    return _mainPanel.Target as EASAToolRemote3DPanel;
                }

                return null;
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

        public override EASSocket socket
        {
            get
            {
                return base.socket;
            }
            set
            {
                base.socket = value;

                EASRemoteCanvasSplineDrawingSupport remoteCanvasDrawingSupport = (EASRemoteCanvasSplineDrawingSupport)canvasDrawingSupport;
                if (remoteCanvasDrawingSupport != null)
                {
                    remoteCanvasDrawingSupport.socket = value;
                    remoteCanvasDrawingSupport.type = EASPacket.kType3D;
                }
            }
        }
        #endregion

        public override void BeginShow()
        {
            base.BeginShow();

            canvasDrawingSupport.Clear();
        }

        public void ProcessPacket(EASPacket packet)
        {
            if (packet != null)
            {
                if (packet.GetBool("data/command/next_step"))
                {
                    NextStep();
                }
                else
                {
                    EASRemoteCanvasSplineDrawingSupport remoteCanvasDrawingSupport = (EASRemoteCanvasSplineDrawingSupport)canvasDrawingSupport;
                    if (remoteCanvasDrawingSupport != null)
                    {
                        remoteCanvasDrawingSupport.ProcessPacket(packet);
                    }
                }
            }
        }

        public void NextStep()
        {
            // sned packet
            if (connected)
            {
                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kType3D;
                packet.Set("data/command/next_step", true);
                socket.Send(packet);
            }

            // makes real mesh
            MakePartsMesh(canvasDrawingSupport.spline);

            // appends the step
            _currentStep++;

            // refresh ui
            Refresh();

            // clear canvas and spline
            canvasDrawingSupport.Clear();

            // if all step have been ended, hide the panel and send to main panel.
            if (_currentStep == stepCount)
            {
                mainPanel.NextStep();
            }
        }
    }
}