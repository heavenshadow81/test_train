using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    public class EASAToolClientPetDrawingPanel : EASAnimatablePanel
    {
        #region Public variables
        public Transform characterPos;
        public Transform palettes;
        public UISlider brushSizeSlider;
        public UISprite brushSizeSmallSprite, brushSizeBigSprite;
        #endregion

        #region Properties
        private System.WeakReference _mainPanel;
        public EASAToolClientPetPanel mainPanel
        {
            get
            {
                if (_mainPanel != null)
                {
                    return _mainPanel.Target as EASAToolClientPetPanel;
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

        private System.WeakReference _modelControl = null;
        public EASClientModelControl modelControl
        {
            get
            {
                if (_modelControl == null)
                {
                    return null;
                }
                return _modelControl.Target as EASClientModelControl;
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

        #region Unity Methods
        public void Update()
        {
            if (connected)
            {
                EASPacket packet = socket.Receive(EASPacket.kTypePet, false);
                if (packet != null)
                {
                    if (packet.GetBool("data/command/next_step"))
                    {
                        _PerformNextStep();

                        socket.Receive(EASPacket.kTypePet);
                    }
                }
            }
        }
        #endregion

        #region Super Methods
        public override void BeginShow()
        {
            base.BeginShow();

            _InitPalettes();
            _ShowBrushColor();

            if (modelControl != null && modelControl.model != null)
            {
                modelControl.model.transform.parent = characterPos;
                TweenRotation.Begin(modelControl.model, 0.25f, Quaternion.identity);
                var tween = TweenPosition.Begin(modelControl.model, 0.25f, Vector3.zero);
                tween.onFinished.Clear();
                tween.onFinished.Add(new EventDelegate(() =>
                {
                    TCCamera.sharedInstance.RequestRefreshTCRT();
                }));
            }
        }
        #endregion

        #region Rotation
        public void Rotate()
        {
            modelControl.rotate = true;
        }

        public void RotateStop()
        {
            modelControl.rotate = false;
        }
        #endregion

        #region Step
        public void NextStep()
        {
            if (connected)
            {
                EASClientManager.ShowLoading();

                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kTypePet;
                packet.Set("data/command/next_step", true);
                socket.Send(packet);
            }
            else
            {
                _PerformNextStep();
            }
        }

        private void _PerformNextStep()
        {
            EASClientManager.HideLoading();

            mainPanel.NextStep();
        }
        #endregion

        #region Setting up Palettes
        private void _InitPalettes()
        {
            if (modelControl != null)
            {
                if (palettes != null)
                {
                    for (int i = 0; i < palettes.childCount; i++)
                    {
                        Transform child = palettes.GetChild(i);
                        UIEventTrigger trigger = child.GetComponent<UIEventTrigger>();
                        if (trigger == null)
                        {
                            trigger = child.gameObject.AddComponent<UIEventTrigger>();
                        }
                        if (trigger.onClick.Count == 0)
                        {
                            trigger.onClick.Add(new EventDelegate(modelControl, child.name));
                            trigger.onClick.Add(new EventDelegate(_ShowBrushColor));
                        }
                    }
                }

                if (brushSizeSlider != null)
                    brushSizeSlider.value = (modelControl.brushSize - 8.0f) / 24.0f;
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
        #endregion
    }
}