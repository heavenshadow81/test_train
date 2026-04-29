using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.EAS
{
    public class EASAToolClient3DPanel : EASAnimatablePanel
    {
        #region Public variables
        public EASATool3DModelListPanel modelListPanel;
        public EASAToolClient3DStepPanel stepPanel;
        public EASAToolClient3DDrawingPanel drawingPanel;
        #endregion

        public override void BeginShow()
        {
            base.BeginShow();

            modelListPanel.Deactive();
            stepPanel.Deactive();
            drawingPanel.Deactive();

            modelListPanel.mainPanel = this;
            stepPanel.mainPanel = this;
            drawingPanel.mainPanel = this;

            modelListPanel.socket = socket;
            stepPanel.socket = socket;
            drawingPanel.socket = socket;

            if (connected)
            {
                socket.Clear(EASPacket.kType3D);
            }

            modelListPanel.onSelectedChanged = () =>
            {
                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kType3D;
                packet.Set("data/model", modelListPanel.selectedModelName);

                if (connected)
                {
                    socket.Send(packet);
                }
            };
        }

        public override void Active()
        {
            base.Active();

            NextStep();
        }

        public void NextStep()
        {
            if (modelListPanel.isShowing)
            {
                modelListPanel.Hide();
                stepPanel.modelName = modelListPanel.selectedModelName;
                stepPanel.Show();
            }
            else if (stepPanel.isShowing)
            {
                stepPanel.Hide();
                drawingPanel.model = stepPanel.generatedGameObject;
                drawingPanel.Show();
            }
            else if (drawingPanel.isShowing)
            {
                Hide();
            }
            else
            {
                modelListPanel.Show();
                modelListPanel.EnableWidgets();
            }
        }

        public void PrevStep()
        {
            if (drawingPanel.isShowing)
            {
                drawingPanel.Hide();
                stepPanel.Show();
            }
            else if (stepPanel.isShowing)
            {
                stepPanel.Hide();
                modelListPanel.Show();
            }
        }
    }
}