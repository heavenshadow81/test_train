using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;
    public class EASAToolClient3DDrawingPanel : EASAnimatablePanel
    {
        #region Public variables
        public EASClientModelControl modelControl;
        public Transform characterPos;
        public Transform palettes;
        public UISlider brushSizeSlider;
        public UISprite brushSizeSmallSprite, brushSizeBigSprite;
        #endregion

        #region Properties
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

        public GameObject model
        {
            get
            {
                return modelControl.model;
            }
            set
            {
                if (modelControl.model != null)
                {
                    GameObject go = modelControl.model;
                    modelControl.model = null;

                    Destroy(go);
                }

                if (value != null)
                {
                    GameObject newModel = (GameObject)Instantiate(value);
                    newModel.transform.parent = characterPos;
                    newModel.transform.localPosition = Vector3.zero;
                    if (newModel.GetComponent<FreeDrawingCarBone>() != null)
                    {
                        newModel.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    }
                    else
                    {
                        newModel.transform.localRotation = Quaternion.identity;
                    }
                    newModel.transform.localScale = Vector3.one;

                    Animator animator = newModel.GetComponent<Animator>();
                    if (animator == null)
                    {
                        animator = newModel.GetComponentInChildren<Animator>();
                    }
                    if (animator != null)
                    {
                        animator.enabled = false;
                    }
                    modelControl.model = newModel;
                }
                else
                {
                    modelControl.model = null;
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
                modelControl.socket = value;
            }
        }
        #endregion

        #region Unity Methods
        public void Update()
        {
            if (connected)
            {
                EASPacket packet = socket.Receive(EASPacket.kType3D, false);
                if (packet != null)
                {
                    if (packet.GetBool("data/command/next_step"))
                    {
                        _PerformDone();

                        socket.Receive(EASPacket.kType3D);
                    }
                }
            }
        }
        #endregion

        #region Super Methods
        public override void BeginShow()
        {
            _InitPalettes();
            modelControl.type = EASPacket.kType3D;
            modelControl.wantsPaint = connected;
        }

        public override void EnableWidgets()
        {
            base.EnableWidgets();
            modelControl.wantsPaint = connected;
        }

        public override void DisableWidgets()
        {
            base.DisableWidgets();
            modelControl.wantsPaint = false;
        }
        #endregion

        #region Step
        public void Done()
        {
            if (connected)
            {
                EASClientManager.ShowLoading();

                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kType3D;
                packet.Set("data/command/next_step", true);
                socket.Send(packet);
            }
            else
            {
                _PerformDone();
            }
        }

        private void _PerformDone()
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