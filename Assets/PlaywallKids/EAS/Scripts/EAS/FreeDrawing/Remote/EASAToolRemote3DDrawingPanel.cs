using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;
    public class EASAToolRemote3DDrawingPanel : EASAnimatablePanel
    {
        #region Public variables
        public EASRemoteModelControl modelControl;
        public Transform characterPos;
        public UISlider brushSizeSlider;
        public UISprite brushSizeSmallSprite, brushSizeBigSprite;
        #endregion

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

        public GameObject model
        {
            get
            {
                return modelControl.model;
            }
            set
            {
                modelControl.model = value;

                if (modelControl.model != null)
                {
                    modelControl.model.transform.parent = characterPos;
                    modelControl.model.transform.localPosition = Vector3.zero;
                    if (modelControl.model.GetComponent<FreeDrawingCarBone>() != null)
                    {
                        modelControl.model.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    }
                    else
                    {
                        modelControl.model.transform.localRotation = Quaternion.identity;
                    }
                    modelControl.model.transform.localScale = Vector3.one;

                    Animator animator = modelControl.model.GetComponent<Animator>();
                    if (animator == null)
                    {
                        animator = modelControl.model.GetComponentInChildren<Animator>();
                    }
                    if (animator != null)
                    {
                        animator.enabled = false;
                    }
                }
            }
        }
        #endregion

        #region Unity Methods
        public void Update()
        {
        }

        public void ProcessPacket(EASPacket packet)
        {
            if (packet != null)
            {
                if (packet.GetBool("data/command/next_step"))
                {
                    if (connected)
                    {
                        socket.Send(packet);
                    }
                    Done();
                }
                else
                {
                    modelControl.ProcessPacket(packet);
                    brushSizeBigSprite.color = brushSizeSmallSprite.color = modelControl.brushColor;
                    brushSizeSlider.value = (modelControl.brushSize - 4.0f) / 28.0f;
                }
            }
        }
        #endregion

        #region Step
        public void Done()
        {
            mainPanel.NextStep();
        }
        #endregion
    }
}