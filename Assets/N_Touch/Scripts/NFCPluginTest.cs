using UnityEngine;
using UnityEngine.UI;

namespace ML.NFCPlugin
{
    public class NFCPluginTest : MonoBehaviour
    {

        public Text nfcUIDTxt;

        public void Start()
        {

            if (NFCPluginInterface.Open())
            {
                Debug.Log("Socket is open");
            }
            else
            {
                Debug.Log("Socket is closed");
            }
        }

        public void Update()
        {
            Packet packet = NFCPluginInterface.Receive();
            if (packet != null)
            {
                if (packet.Type == Packet.PacketType.CardAdded)
                {
                    Packet.CardAddedPacketData CardAdded = packet.Data as Packet.CardAddedPacketData;
                    Debug.Log("DeviceClass : " + CardAdded.DeviceClass);
                    Debug.Log("CardName : " + CardAdded.CardName);
                    Debug.Log("ATR : " + CardAdded.ATR);
                    Debug.Log("UID : " + CardAdded.UID);
                    Debug.Log("Reader ID : " + CardAdded.ReaderId);

                    //Text로 표시 -ljs
                    nfcUIDTxt.text = "UID : " + CardAdded.UID;
                }
            }
        }

        public void OnDestroy()
        {
            NFCPluginInterface.Close();
        }
    }
}