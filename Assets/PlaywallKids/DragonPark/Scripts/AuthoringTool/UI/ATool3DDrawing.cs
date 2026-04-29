using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Dragon Park Free-Drawing Panel
    /// Step 3: Painting
    /// </summary>
    public class ATool3DDrawing : AnimatablePanel
    {
        #region Public variables
        public SimpleModelControl modelControl;
        public Transform characterPos;
        public Transform palettes;
        public UISlider brushSizeSlider;
        public UISprite brushSizeSmallSprite, brushSizeBigSprite;
        #endregion

        #region Properties
        private System.WeakReference _mainPanel;
        public ATool3DPanel mainPanel
        {
            get
            {
                if (_mainPanel != null)
                {
                    return _mainPanel.Target as ATool3DPanel;
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
                return (modelControl.model != null ? modelControl.model.gameObject : null);
            }
            set
            {
                modelControl.model = value;

                if (modelControl.model != null)
                {
                    // default transform settings
                    modelControl.model.transform.parent = characterPos;
                    modelControl.model.transform.localPosition = Vector3.zero;
                    modelControl.model.transform.localRotation = Quaternion.identity;
                    modelControl.model.transform.localScale = Vector3.one;

                    // per-model transform settings
                    switch (modelControl.model.GetComponent<FreeDrawingObjectBone>().objectType)
                    {
                        case FreeDrawingObjectType.Car:
                            modelControl.model.transform.localRotation = Quaternion.Euler(0, 90, 0);
                            break;
                        case FreeDrawingObjectType.Airplane:
                            modelControl.model.transform.localPosition += new Vector3(0.0f, 0.6f, 0.0f);
                            modelControl.model.transform.localRotation = Quaternion.Euler(0, 90, 0);
                            modelControl.model.transform.localScale *= 1.33f;
                            break;
                    }

                    Animator animator = modelControl.model.GetComponentInChildren<Animator>();
                    if (animator != null)
                        animator.enabled = false;
                }
            }
        }
        #endregion

        #region Super Methods
        public override void BeginShow()
        {
            _InitPalettes();
        }

        public override void EnableWidgets()
        {
            base.EnableWidgets();
            modelControl.wantsPaint = true;
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