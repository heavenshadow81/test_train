using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Authoring Tool <br>
    /// Step 2, Sizing
    /// </summary>
    public class AToolCharacterSizing : AToolCharacterStep
    {
        #region Public variables
        // character position
        public Transform characterPos;
        #endregion

        #region Properties
        private ModelSizeControl[] _modelSizeControls;
        public ModelSizeControl[] modelSizeControls
        {
            get
            {
                if (_modelSizeControls == null ||
                   _modelSizeControls.Length == 0)
                {
                    _modelSizeControls = GetComponentsInChildren<ModelSizeControl>();
                }
                return _modelSizeControls;
            }
        }
        #endregion

        public void OnEnable()
        {
            _InitModelSizeControls();
        }

        // Update is called once per frame
        public override void Reset()
        {
            Template3D template = mainPanel.currentTemplate;
            if (template != null)
            {
                if (template.cachedTransform.parent != characterPos)
                {
                    template.cachedTransform.parent = characterPos;

                    TweenPosition.Begin(template.gameObject, 0.25f, Vector3.zero);
                    TweenRotation.Begin(template.gameObject, 0.25f, Quaternion.identity);
                }
            }

            _InitModelSizeControls();
        }


        public override void Rotate()
        {
            SimpleModelControl modelControl = mainPanel.modelControl;
            modelControl.RotateAndType(true, AutoRotate.RotateType.Right);
        }

        public override void RotateLeft()
        {
            SimpleModelControl modelControl = mainPanel.modelControl;
            modelControl.RotateAndType(true, AutoRotate.RotateType.Left);
        }

        public override void RotateStop()
        {
            SimpleModelControl modelControl = mainPanel.modelControl;
            modelControl.rotate = false;
        }

        private void _InitModelSizeControls()
        {
            if (mainPanel != null)
            {
                SimpleModelControl modelControl = mainPanel.modelControl;

                foreach (ModelSizeControl m in modelSizeControls)
                {
                    m.modelControl = mainPanel.modelControl;
                }
            }
        }
    }
}