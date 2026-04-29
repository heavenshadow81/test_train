using UnityEngine;
using UnityEngine.UI;

namespace ML.MLBKids
{
    public class Popup : MonoBehaviour
    {
        public Text messageText;
        private System.Action _handler;
        
        public void Set(string message, System.Action handler = null)
        {
            messageText.text = message;
            _handler = handler;

            //DelayedCall.Begin("popup_auto_hide", GlobalConstants.instance.anyActionWaitTime, (flag) =>
            //{
            //    if (flag)
            //    {
            //        gameObject.SetActive(false);
            //    }
            //});
        }

        public void OK()
        {
            if (_handler != null)
                _handler();
            gameObject.SetActive(false);

            DelayedCall.Stop("popup_auto_hide");
        }
    }
}