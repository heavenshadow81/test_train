using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.EAS
{
    public class EASClientLoading : AnimatablePanel
    {
        #region Properties
        private static EASClientLoading _sharedInstance = null;
        public static EASClientLoading sharedInstance
        {
            get
            {
                return _sharedInstance;
            }
        }
        #endregion

        void Start()
        {
            _sharedInstance = this;
        }
    }
}