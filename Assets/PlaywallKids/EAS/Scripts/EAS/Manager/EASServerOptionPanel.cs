using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.EAS
{
    public class EASServerOptionPanel : MonoBehaviour
    {
        public UIInput kavatarIP, kavatarPort;

        // Use this for initialization
        void Start()
        {
            string IP = PlayerPrefs.GetString("kavatar_ip", kavatarIP.value);
            int port = PlayerPrefs.GetInt("kavatar_port", int.Parse(kavatarPort.value));

            PlayerPrefs.SetString("kavatar_ip", IP);
            PlayerPrefs.SetInt("kavatar_port", port);
        }

        public void SetIP()
        {
            PlayerPrefs.SetString("kavatar_ip", kavatarIP.value);
        }

        public void SetPort()
        {
            PlayerPrefs.SetInt("kavatar_port", int.Parse(kavatarPort.value));
        }
    }
}