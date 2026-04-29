using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASAToolClient3DStepPanel : EASATool3DStepPanelBase
    {
        #region Public variables
        public UIButton nextButton, okButton;
        #endregion

        #region Properties
        public override EASSocket socket
        {
            get
            {
                return base.socket;
            }
            set
            {
                base.socket = value;

                EASClientCanvasSplineDrawingSupport clientCanvasDrawingSupport = (EASClientCanvasSplineDrawingSupport)canvasDrawingSupport;
                if (clientCanvasDrawingSupport != null)
                {
                    clientCanvasDrawingSupport.socket = value;
                    clientCanvasDrawingSupport.type = EASPacket.kType3D;
                }
            }
        }

        private System.WeakReference _mainPanel;
        public EASAToolClient3DPanel mainPanel
        {
            get
            {
                if (_mainPanel != null)
                {
                    return _mainPanel.Target as EASAToolClient3DPanel;
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
        #endregion

        public void Update()
        {
            if (connected)
            {
                // get packet without pop
                EASPacket packet = socket.Receive(EASPacket.kType3D, false);
                if (packet != null)
                {
                    // check if there's command "next_step"
                    if (packet.Get("data/command/next_step") != null)
                    {
                        if (packet.GetBool("data/command/next_step"))
                        {
                            _PerformNextStep();
                        }
                        else
                        {
                            // hmm...
                        }
                    }

                    // pop from queue
                    socket.Receive(EASPacket.kType3D, true);
                }
            }

            if (_currentStep + 1 == stepCount)
            {
                okButton.isEnabled = canvasDrawingSupport.canMakeSpline;
            }
            else
            {
                nextButton.isEnabled = canvasDrawingSupport.canMakeSpline;
            }
        }

        public override void BeginShow()
        {
            base.BeginShow();

            if (nextButton != null)
            {
                nextButton.gameObject.SetActive(true);
            }
            if (okButton != null)
            {
                okButton.gameObject.SetActive(false);
            }
        }

        public void NextStep()
        {
            if (canvasDrawingSupport.canMakeSpline)
            {
                canvasDrawingSupport.enabled = false;

                if (connected)
                {
                    // show loading screen
                    EASClientManager.ShowLoading();

                    // send packet to server
                    EASPacket packet = new EASPacket();
                    packet.type = EASPacket.kType3D;
                    packet.Set("data/command/next_step", true);
                    socket.Send(packet);
                }
                else
                {
                    _PerformNextStep();
                }
            }
        }

        private void _PerformNextStep()
        {
            // hide the loading screen
            EASClientManager.HideLoading();

            // makes real mesh
            MakePartsMesh(canvasDrawingSupport.spline);

            // appends the step
            _currentStep++;

            // refresh labels
            Refresh();

            // clear canvas and spline
            canvasDrawingSupport.Clear();

            // make canvas drawable
            canvasDrawingSupport.enabled = true;

            // if all step have been ended, hide the panel and send to main panel.
            if (_currentStep == stepCount)
            {
                mainPanel.NextStep();
            }
            // if current step is final step, shows ok button instead of next button.
            else if (_currentStep + 1 == stepCount)
            {
                nextButton.gameObject.SetActive(false);
                okButton.gameObject.SetActive(true);
            }
        }
    }
}