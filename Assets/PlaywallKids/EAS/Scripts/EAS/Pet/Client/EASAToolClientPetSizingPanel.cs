using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;
    public class EASAToolClientPetSizingPanel : EASAToolPetSizingPanelBase
    {
        #region Public variables
        public UISlider headSlider;
        public UISlider wingSlider;
        public UISlider armSlider;
        public UISlider tailSlider;
        #endregion

        #region Properties
        private System.WeakReference _mainPanel = null;
        public EASAToolClientPetPanel mainPanel
        {
            get
            {
                if (_mainPanel == null)
                {
                    return null;
                }
                return _mainPanel.Target as EASAToolClientPetPanel;
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

        #region Private variables
        private EASPacket _currentPacket = null;
        #endregion

        public EASAToolClientPetSizingPanel()
        {
            _MakeNewPacket();
        }

        private void _MakeNewPacket()
        {
            _currentPacket = new EASPacket();
            _currentPacket.type = EASPacket.kTypePet;
            onAccessorySet = (newAcc) =>
            {
                string accName = AccessoryManager.GetAccessoryName(newAcc);
                string boneName = null;

                Transform dummy = AccessoryManager.GetDummy(newAcc);
                if (dummy != null)
                {
                // Head
                if (dummy.name.Contains("Head") || dummy.name.Contains("Hair"))
                    {
                        boneName = BoneObject.kHeadBone;
                    }
                // Body
                else if (dummy.name.Contains("Body"))
                    {
                        boneName = BoneObject.kBodyBone;
                    }
                }

                _WriteAccInfoIntoPacket(boneName, accName);
            };
        }

        public void Update()
        {
            if (connected)
            {
                EASPacket packet = socket.Receive(EASPacket.kTypePet, false);
                if (packet != null)
                {
                    if (packet.GetBool("data/command/next_step"))
                    {
                        _PerformDone();

                        socket.Receive(EASPacket.kTypePet);
                    }
                }

                if (_currentPacket.data.Count > 0)
                {
                    socket.Send(_currentPacket);
                    _MakeNewPacket();
                }
            }
        }

        public override void BeginShow()
        {
            base.BeginShow();
            headSlider.value = wingSlider.value = armSlider.value = tailSlider.value = 0.0f;
        }

        public void Rotate()
        {
            if (modelControl != null)
            {
                modelControl.rotate = true;
            }
        }

        public void RotateStop()
        {
            if (modelControl != null)
            {
                modelControl.rotate = false;
            }
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
                mainPanel.NextStep();
            }
        }

        private void _PerformDone()
        {
            EASClientManager.HideLoading();

            mainPanel.NextStep();
        }

        public void HeadSize()
        {
            if (dragon != null)
            {
                float sliderValue = headSlider.value + 1.0f;
                dragon.SetBoneScale(Dragon.kHeadBone, sliderValue);
                _WriteBoneInfoIntoPacket(Dragon.kHeadBone, sliderValue);
            }
        }

        public void WingSize()
        {
            if (dragon != null)
            {
                float sliderValue = wingSlider.value + 1.0f;

                float wingLScale = dragon.GetBoneScale(Dragon.kWingLBone);
                float wingRScale = dragon.GetBoneScale(Dragon.kWingRBone);

                wingLScale = sliderValue * (wingLScale >= 0.0f ? 1.0f : -1.0f);
                wingRScale = sliderValue * (wingRScale >= 0.0f ? 1.0f : -1.0f);

                dragon.SetBoneScale(Dragon.kWingLBone, wingLScale);
                dragon.SetBoneScale(Dragon.kWingRBone, wingRScale);

                _WriteBoneInfoIntoPacket(Dragon.kWingLBone, wingLScale);
                _WriteBoneInfoIntoPacket(Dragon.kWingRBone, wingRScale);
            }
        }

        public void ArmSize()
        {
            if (dragon != null)
            {
                float sliderValue = armSlider.value + 1.0f;

                dragon.SetBoneScale(Dragon.kArmLBone, sliderValue);
                dragon.SetBoneScale(Dragon.kArmRBone, sliderValue);
                _WriteBoneInfoIntoPacket(Dragon.kArmLBone, sliderValue);
                _WriteBoneInfoIntoPacket(Dragon.kArmRBone, sliderValue);
            }
        }

        public void TailSize()
        {
            if (dragon != null)
            {
                float sliderValue = tailSlider.value + 1.0f;

                dragon.SetBoneScale(Dragon.kTailBone, sliderValue);
                _WriteBoneInfoIntoPacket(Dragon.kTailBone, sliderValue);
            }
        }

        private void _WriteBoneInfoIntoPacket(string boneName, float value)
        {
            if (!string.IsNullOrEmpty(boneName))
            {
                List<object> list = _currentPacket.GetList("data/bone");
                if (list == null)
                {
                    list = new List<object>();
                }

                Dictionary<string, object> info = new Dictionary<string, object>();
                info["name"] = boneName;
                info["scale"] = value;
                list.Add(info);

                _currentPacket.Set("data/bone", list);
            }
        }

        private void _WriteAccInfoIntoPacket(string boneName, string accName)
        {
            if (!string.IsNullOrEmpty(boneName) && !string.IsNullOrEmpty(accName))
            {
                List<object> list = _currentPacket.GetList("data/acc");
                if (list == null)
                {
                    list = new List<object>();
                }

                Dictionary<string, object> info = new Dictionary<string, object>();
                info["name"] = accName;
                info["bone"] = boneName;
                list.Add(info);

                _currentPacket.Set("data/acc", list);
            }
        }
    }
}