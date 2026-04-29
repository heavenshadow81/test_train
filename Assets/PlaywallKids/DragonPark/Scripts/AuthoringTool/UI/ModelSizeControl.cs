using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    public class ModelSizeControl : MonoBehaviour
    {
        private SimpleModelControl _modelControl = null;
        public SimpleModelControl modelControl
        {
            get
            {
                return _modelControl;
            }
            set
            {
                _modelControl = value;
            }
        }

        private Dragon _dragon = null;
        public Dragon dragon
        {
            get
            {
                if (_dragon == null)
                {
                    if (_modelControl != null)
                    {
                        Template3D model = _modelControl.template;

                        if (model != null)
                        {
                            _dragon = model.GetComponent<Dragon>();
                        }
                    }
                }
                return _dragon;
            }
        }

        public float sliderValue
        {
            get
            {
                float value = 0.0f;
                if (_modelSizeSlider != null)
                    value += _modelSizeSlider.value;
                return value;
            }
        }

        public float sliderValueForBone
        {
            get
            {
                return sliderValue + 1.0f;
            }
        }

        private UISlider _modelSizeSlider;

        // Use this for initialization
        void Start()
        {
            _modelSizeSlider = GetComponent<UISlider>();
        }

        public void HeadSize()
        {
            if (modelControl == null) return;

            dragon.SetBoneScale(Dragon.kHeadBone, sliderValueForBone);
        }

        public void WingSize()
        {
            if (modelControl == null) return;

            // Check the wing bones exist
            if (dragon.GetBone(Dragon.kWingLBone) || dragon.GetBone(Dragon.kWingRBone))
            {
                float wingLScale = dragon.GetBoneScale(Dragon.kWingLBone);
                float wingRScale = dragon.GetBoneScale(Dragon.kWingRBone);

                dragon.SetBoneScale(Dragon.kWingLBone, sliderValueForBone * (wingLScale >= 0.0f ? 1.0f : -1.0f));
                dragon.SetBoneScale(Dragon.kWingRBone, sliderValueForBone * (wingRScale >= 0.0f ? 1.0f : -1.0f));
            }
            // Or manteaus
            else if (dragon.GetBone(Dragon.kManteauLBone) || dragon.GetBone(Dragon.kManteauRBone))
            {
                float wingLScale = dragon.GetBoneScale(Dragon.kManteauLBone);
                float wingRScale = dragon.GetBoneScale(Dragon.kManteauRBone);

                dragon.SetBoneScale(Dragon.kManteauLBone, sliderValueForBone * (wingLScale >= 0.0f ? 1.0f : -1.0f));
                dragon.SetBoneScale(Dragon.kManteauRBone, sliderValueForBone * (wingRScale >= 0.0f ? 1.0f : -1.0f));
            }
        }

        public void HandSize()
        {
            if (modelControl == null) return;

            dragon.SetBoneScale(Dragon.kArmLBone, sliderValueForBone);
            dragon.SetBoneScale(Dragon.kArmRBone, sliderValueForBone);
        }

        public void TailSize()
        {
            if (modelControl == null) return;

            dragon.SetBoneScale(Dragon.kTailBone, sliderValueForBone);
        }

        public void BrushSize()
        {
            if (modelControl == null) return;

            modelControl.brushSize = Mathf.RoundToInt(sliderValue * 20.0f + 4.0f);
        }
    }
}