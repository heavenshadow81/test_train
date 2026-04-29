using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    public class EASAToolClientPetFreeDrawingPanel : EASAToolPetFreeDrawingPanelBase
    {
        #region Public variables
        public Transform palettes;
        public UIButton okButton;
        public UIButton rotateButton;
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

        public override void BeginShow()
        {
            EASClientCanvasSplineDrawingSupport c = canvasSplineDrawingSupport as EASClientCanvasSplineDrawingSupport;
            c.socket = socket;
            c.type = EASPacket.kTypePet;

            modelControl.socket = socket;
            modelControl.wantsPaint = false;

            if (onSelectItem == null)
            {
                onSelectItem = (idx) =>
                {
                    HidePalettes(true);

                    if (connected)
                    {
                        EASPacket packet = new EASPacket();
                        packet.type = EASPacket.kTypePet;
                        packet.Set("data/fruit", idx);
                        socket.Send(packet);
                    }
                };
            }

            _InitPalettes();

            ClearModel();

            base.BeginShow();
        }

        public void Update()
        {
            if (connected)
            {
                EASPacket packet = socket.Receive(EASPacket.kTypePet, false);
                if (packet != null)
                {
                    if (packet.Get("data/command/next_step") != null)
                    {
                        if (packet.GetBool("data/command/next_step"))
                        {
                            _PerformNextStep();
                        }
                        else
                        {
                            ClearModel();
                            HidePalettes(false);
                        }
                    }

                    socket.Receive(EASPacket.kTypePet);
                }
            }

            if (template == null)
            {
                rotateButton.isEnabled = false;
                okButton.isEnabled = canvasSplineDrawingSupport.canMakeSpline;
            }
            else
            {
                rotateButton.isEnabled = true;
                okButton.isEnabled = true;
            }
        }

        private void _ShowBrushColor()
        {
            brushSizeSmallSprite.color = brushSizeBigSprite.color = modelControl.brushColor;
        }

        public void ChangeBrushSize()
        {
            if (modelControl != null)
            {
                modelControl.brushSize = Mathf.RoundToInt(8.0f + 24.0f * brushSizeSlider.value);
            }
        }

        public void Rotate()
        {
            modelControl.rotate = true;
        }

        public void RotateStop()
        {
            modelControl.rotate = false;
        }

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

            if (template == null)
            {
                Triangulate();
                if (template != null)
                {
                    ShowPalettes(true);
                    modelControl.model = template.gameObject;
                    modelControl.wantsPaint = true;
                }
            }
            else
            {
                mainPanel.NextStep();
            }
        }
        
        #region Palettes
        private void _InitPalettes()
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

            if (modelControl != null)
            {
                brushSizeSlider.value = (modelControl.brushSize - 8.0f) / 24.0f;
            }

            HidePalettes(false);
        }

        public void ShowPalettes(bool showsAniamtion)
        {
            palettes.gameObject.SetActive(true);
            brushSizeSlider.gameObject.SetActive(true);
            brushSizeSmallSprite.gameObject.SetActive(true);
            brushSizeBigSprite.gameObject.SetActive(true);

            if (showsAniamtion)
            {
                UITweener[] tweens = palettes.GetComponents<UITweener>();
                foreach (UITweener tween in tweens)
                {
                    tween.PlayForward();
                }
                tweens = brushSizeContainer.GetComponentsInChildren<UITweener>();
                foreach (UITweener tween in tweens)
                {
                    tween.PlayForward();
                }
            }
        }

        public void HidePalettes(bool showsAnimation)
        {
            if (showsAnimation)
            {
                UITweener[] tweens = palettes.GetComponents<UITweener>();
                foreach (UITweener tween in tweens)
                {
                    tween.PlayReverse();
                }
                tweens = brushSizeContainer.GetComponentsInChildren<UITweener>();
                foreach (UITweener tween in tweens)
                {
                    tween.PlayReverse();
                }
            }
            else
            {
                palettes.gameObject.SetActive(false);
                brushSizeSlider.gameObject.SetActive(false);
                brushSizeSmallSprite.gameObject.SetActive(false);
                brushSizeBigSprite.gameObject.SetActive(false);
            }
        }
        #endregion
    }
}