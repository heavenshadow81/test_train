using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.DragonPark
{
    public class AToolMenuSelector : AToolCharacterStep
    {
        public UIButton[] menus;

        private AToolMainPanel.Step _prevStep;

        public void Start()
        {
            for (int i = 0; i < menus.Length; i++)
            {
                menus[i].onClick.Add(new EventDelegate(this, string.Format("Menu{0}", i + 1)));
            }
            HideOthers();
        }

        public void Update()
        {
            if (_prevStep != mainPanel.currentStep)
            {
                HideOthers();
            }
            _prevStep = mainPanel.currentStep;
        }

        public void Menu1()
        {
            if (mainPanel != null)
            {
                mainPanel.currentStep = AToolMainPanel.Step.SelectCharacter;
            }
            HideOthers();
        }

        public void Menu2()
        {
            if (mainPanel != null)
            {
                mainPanel.currentStep = AToolMainPanel.Step.Sizing;
            }
            HideOthers();
        }

        public void Menu3()
        {
            if (mainPanel != null)
            {
                mainPanel.currentStep = AToolMainPanel.Step.Drawing;
            }
            HideOthers();
        }

        public void Menu4()
        {
            if (mainPanel != null)
            {
                mainPanel.currentStep = AToolMainPanel.Step.FreeDrawing;
            }
            HideOthers();
        }

        public void HideOthers()
        {
            for (int i = 0; i < menus.Length; i++)
            {
                if (i + 1 == (int)mainPanel.currentStep)
                {
                    menus[i].gameObject.SetActive(true);
                }
                else
                {
                    menus[i].gameObject.SetActive(false);
                }
            }
        }
    }
}