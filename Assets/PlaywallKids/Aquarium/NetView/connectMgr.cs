using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    public class connectMgr : MonoBehaviour
    {

        private static connectMgr s_data = null;

        int nSvrPort = 3721;

        void Start()
        {
            Object[] onlyOne = FindObjectsOfType(this.GetType());

            if (onlyOne.Length > 1)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }

            //if (Network.peerType == NetworkPeerType.Disconnected)
            //{
            //    Network.InitializeServer(32, nSvrPort, false);
            //}
        }

        //void OnGUI()
        //{
        //    if (Network.peerType == NetworkPeerType.Disconnected)
        //    {
        //        nSvrPort = int.Parse(GUI.TextField(new Rect(10, Screen.height - 80, 100, 30), nSvrPort.ToString()));

        //        if (GUI.Button(new Rect(10, Screen.height - 40, 100, 30), "Connect"))
        //        {
        //            Network.InitializeServer(32, nSvrPort, false);
        //        }
        //    }
        //}

        public void sendTexPath(int nID, string tmpName)
        {
            string buf = string.Empty;

            buf = nID.ToString() + "," + tmpName;

            //NetworkView nv = GetComponent<NetworkView>();
            //if(nv != null)
            //{
            //    nv.RPC("SendToClientData", RPCMode.Others, buf);
            //}
        }

        //[RPC] void SendToClientData(string buf) { }

        public static connectMgr Instance()
        {
            if (s_data == null)
            {
                s_data = FindObjectOfType(typeof(connectMgr)) as connectMgr;
            }
            if (s_data == null)
            {
                GameObject obj = new GameObject("connectMgr");
                s_data = obj.AddComponent(typeof(connectMgr)) as connectMgr;
            }
            return s_data;
        }
    }
}