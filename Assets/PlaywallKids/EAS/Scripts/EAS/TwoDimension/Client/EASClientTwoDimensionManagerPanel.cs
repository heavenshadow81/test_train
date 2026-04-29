using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.EAS
{
    public class EASClientTwoDimensionManagerPanel : EASAnimatablePanel
    {
        #region Public variables
        public UIButton nextButton;
        #endregion

        #region Properties
        private EASClientTwoDimensionAdmin _twoDimensionAdmin;
        public EASClientTwoDimensionAdmin twoDimensionAdmin
        {
            get
            {
                if (_twoDimensionAdmin == null)
                {
                    _twoDimensionAdmin = GetComponent<EASClientTwoDimensionAdmin>();
                    if (_twoDimensionAdmin == null)
                    {
                        _twoDimensionAdmin = GetComponentInChildren<EASClientTwoDimensionAdmin>();
                    }
                }
                return _twoDimensionAdmin;
            }
        }

        public override EASSocket socket
        {
            set
            {
                base.socket = value;
                if (twoDimensionAdmin != null)
                    twoDimensionAdmin.socket = socket;
            }
        }
        #endregion

        #region Methods
        public void Update()
        {
            bool isRoot = EASClientManager.currentManager.root;

            if (nextButton != null) nextButton.gameObject.SetActive(isRoot);
        }
        #endregion
    }
}