using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    public class EASClientConnectionPopup : AnimatablePanel
    {
        #region Public variables
        public UIInput ipInput;
        public UIInput portInput;

        public UIButton connectButton;

        public UILabel messageLabel;
        #endregion

        public override void BeginShow()
        {
            base.BeginShow();

            EASClientManager manager = EASClientManager.currentManager;

            ipInput.value = PlayerPrefs.GetString("eas_server_ip", manager.IP);
            portInput.value = string.Format("{0}", PlayerPrefs.GetInt("eas_server_port", manager.port));

            messageLabel.text = "";
        }

        public override void EnableWidgets()
        {
            base.EnableWidgets();

            ipInput.GetComponent<Collider>().enabled = true;
            portInput.GetComponent<Collider>().enabled = true;
        }

        public override void DisableWidgets()
        {
            base.DisableWidgets();

            ipInput.GetComponent<Collider>().enabled = false;
            portInput.GetComponent<Collider>().enabled = false;
        }

        public void Connect()
        {
            DisableWidgets();

            // set message as blank
            messageLabel.text = "";

            // get current manager
            var manager = EASClientManager.currentManager;

            // set ip and port
            manager.IP = ipInput.value;
            int port = manager.port;
            int.TryParse(portInput.value, out port);
            manager.port = port;

            // request connect
            manager.Connect((flag) =>
            {
                EnableWidgets();

                if (flag)
                {
                // save settings
                PlayerPrefs.SetString("eas_server_ip", manager.IP);
                    PlayerPrefs.SetInt("eas_server_port", manager.port);
                    PlayerPrefs.Save();

                // hide
                Hide();
                }
                else
                {
                    messageLabel.text = "Connection failed!";
                }
            });
        }
    }
}