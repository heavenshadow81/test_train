using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    using Common;

    /// <summary>
    /// Dragon Park - Free-drawing object action menu after creation
    /// </summary>
    public class ATool3DMenuPanel : AnimatablePanel
    {
        public UIButton printButton;

        public int userId { get; private set; }
        public FreeDrawingObjectBone bone { get; private set; }
        public FreeDrawingAnimationControl animationControl { get; private set; }

        public override void BeginShow()
        {
            base.BeginShow();
            _CheckPrinterStatus();
        }

        public override void EnableWidgets()
        {
            base.EnableWidgets();
            _CheckPrinterStatus();
        }

        private void _CheckPrinterStatus()
        {
            GameObject template = SimpleInstantiatedTemplateControl.GetCurrentTemplate(userId);
            bool isPrinted = template == null || DragonPrinter.IsPrinted(userId, template.name);
            printButton.isEnabled = CommonSettings.printerEnabled && !isPrinted;
        }

        public override void Deactive()
        {
            base.Deactive();

            bone = null;
            animationControl = null;
        }

        public void Set(int newUserId)
        {
            userId = newUserId;

            GameObject go = SimpleInstantiatedTemplateControl.GetCurrentTemplate(userId);
            if (go != null)
            {
                bone = go.GetComponent<FreeDrawingObjectBone>();
                animationControl = go.GetComponent<FreeDrawingAnimationControl>();
            }
        }

        public void Print()
        {
            Hide();
            MenuControl.sharedInstance.ShowPetPrint(userId);
        }

        public void FreePlay()
        {
            if (bone != null)
            {
                switch (bone.objectType)
                {
                    case FreeDrawingObjectType.Car:
                        ((FreeDrawingCarBone)bone).SetDragon(userId);
                        break;
                    default:
                        if (animationControl != null)
                            animationControl.movesAlongPath = true;
                        break;
                }
            }

            Hide();
            MenuControl.sharedInstance.Activate(userId);
        }
    }
}