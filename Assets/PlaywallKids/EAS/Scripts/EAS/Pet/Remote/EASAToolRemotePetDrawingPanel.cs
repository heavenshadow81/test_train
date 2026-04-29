using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    public class EASAToolRemotePetDrawingPanel : EASAnimatablePanel
    {
        #region Public variables
        public Transform characterPos;
        public UISlider brushSizeSlider;
        public UISprite brushSizeSmallSprite, brushSizeBigSprite;
        #endregion

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
                if (packet.GetBool("data/command/next_step"))
                {
                    EASPacket response = new EASPacket();
                    response.type = EASPacket.kTypePet;
                    response.Set("data/command/next_step", true);
                    socket.Send(response);

                    mainPanel.NextStep();
                }
                else if (modelControl != null)
                {
                    modelControl.ProcessPacket(packet);
                    _ShowBrushColor();
                    brushSizeSlider.value = (modelControl.brushSize - 4.0f) / 28.0f;
                }
            }
        }

        public override void BeginShow()
        {
            base.BeginShow();

            if (modelControl != null && modelControl.model != null)
            {
                modelControl.model.transform.parent = characterPos;
                TweenPosition.Begin(modelControl.model, 0.25f, Vector3.zero);
                TweenRotation.Begin(modelControl.model, 0.25f, Quaternion.identity);
                var tween = modelControl.model.GetComponent<TweenRotation>();
                if (tween != null)
                {
                    tween.SetOnFinished(new EventDelegate(() =>
                    {
                        TCCamera.sharedInstance.RequestRefreshTCRT();
                    }));
                }
            }
        }

        private void _ShowBrushColor()
        {
            if (modelControl != null)
            {
                if (brushSizeSmallSprite != null)
                {
                    brushSizeSmallSprite.color = modelControl.brushColor;
                }

                if (brushSizeBigSprite != null)
                {
                    brushSizeBigSprite.color = modelControl.brushColor;
                }
            }
        }
    }
}