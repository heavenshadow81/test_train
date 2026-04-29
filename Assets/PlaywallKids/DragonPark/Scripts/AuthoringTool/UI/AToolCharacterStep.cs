using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.DragonPark
{
    public class AToolCharacterStep : AnimatablePanel
    {
        #region Properties
        System.WeakReference _mainPanel = new System.WeakReference(null);
        public AToolMainPanel mainPanel
        {
            get
            {
                return _mainPanel.Target as AToolMainPanel;
            }
            set
            {
                _mainPanel.Target = value;
            }
        }
        #endregion

        public virtual void Reset()
        {
        }

        public virtual void Rotate()
        {
        }

        public virtual void RotateLeft()
        {
        }

        public virtual void RotateStop()
        {
        }

        public virtual void GoToNextStep()
        {
            if (mainPanel != null)
            {
                mainPanel.Next();
            }
        }
    }
}