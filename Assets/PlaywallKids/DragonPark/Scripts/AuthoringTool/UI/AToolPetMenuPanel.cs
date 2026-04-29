using UnityEngine;
using System.Collections.Generic;
using ML.PlaywallKids.Common;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Dragon Park - Dragon action menu after creation
    /// </summary>
    public class AToolPetMenuPanel : AnimatablePanel
    {
        public UIButton printButton;

        public int userId { get; set; }

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

        public void Print()
        {
            MenuControl.sharedInstance.ActivatePetMenu(userId, false);
            MenuControl.sharedInstance.ShowPetPrint(userId);
        }

        public void Dance()
        {
            var template = SimpleInstantiatedTemplateControl.GetCurrentTemplate(userId);
            if (template != null)
            {
                DragonMotionList motionList = template.GetComponent<DragonMotionList>();
                if (motionList == null)
                    motionList = template.gameObject.AddComponent<DragonMotionList>();
                motionList.Set(new List<int>() { Random.Range(1, 4) });
                motionList.Play();
            }

            // If you want to show panel, comment above and uncomment below.
            /*
            MenuControl.sharedInstance.ActivatePetMenu(userId, false);
            var touchArea = MenuControl.sharedInstance.GetTouchArea(userId);
            GameObject petMotionPanel = null;

            if (touchArea != null)
            {
                petMotionPanel = touchArea.PetMotion();
            }
            else
            {
                petMotionPanel = MenuControl.sharedInstance.ShowPetMotion(userId);
            }

            AToolPetMotionPanel panel = petMotionPanel.GetComponent<AToolPetMotionPanel>();
            panel.UseCurrentTemplate();
            */
        }

        public void FreePlay()
        {
            MenuControl.sharedInstance.HidePetMenu(userId);
            MenuControl.sharedInstance.Activate(userId);

            DragonFruit fruit = DragonFruit.GetFruit(userId);
            if (fruit != null)
            {
                fruit.showMenuAfterEat = true;
            }
            else
            {
                var template = SimpleInstantiatedTemplateControl.GetCurrentTemplate(userId);
                if (template != null)
                {
                    // 현재 춤 동작을 그만하도록 함
                    DragonMotionList motionList = template.GetComponent<DragonMotionList>();
                    if (motionList != null)
                        motionList.Stop();

                    // 드래곤이 뛰어놀게 하기
                    DragonComeToFront comeToFront = template.GetComponent<DragonComeToFront>();
                    if (comeToFront != null)
                        comeToFront.Back();
                    else
                    {
                        DragonAnimationControl dragonAnimation = template.GetComponent<DragonAnimationControl>();
                        if (dragonAnimation != null)
                        {
                            dragonAnimation.movesAlongPath = true;
                            CharacterManager.AddMakeObject(userId, dragonAnimation);
                        }
                    }
                }
            }
        }

        public void Feed()
        {
            MenuControl.sharedInstance.ActivatePetMenu(userId, false);
            MenuControl.sharedInstance.ShowPetFeed(userId);
        }
    }
}