using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ML.PlaywallKids.Aquarium
{
    public class SeaStoryServerClientSocketTest : MonoBehaviour
    {
        public int userId = 0;
        public string templateName = "";
        public Texture2D[] templateTextures;

        Socket socket = null;

        public void OnGUI()
        {
            if (socket == null)
            {
                if (GUI.Button(new Rect(20, 20, 200, 100), "Send!!"))
                {
                    try
                    {
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5999);
                        socket.Connect((EndPoint)endPoint);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("Connect failed!");
                    }

                    if (socket != null)
                    {
                        List<SeaStoryTemplatePacket.ImageInfo> templateImages = new List<SeaStoryTemplatePacket.ImageInfo>(templateTextures.Length);
                        foreach (Texture2D t in templateTextures)
                        {
                            var image = new SeaStoryTemplatePacket.ImageInfo();
                            image.width = t.width;
                            image.height = t.height;
                            Color32[] colors = t.GetPixels32();
                            image.colors = colors;
                            templateImages.Add(image);
                        }

                        var packet = new SeaStoryTemplatePacket(userId, templateName, templateImages);
                        byte[] bytes = packet.ToByteArray();
                        socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new System.AsyncCallback(OnSendFinish), socket);
                    }
                }
            }
        }

        public void OnSendFinish(System.IAsyncResult ar)
        {
            Socket socket = ar.AsyncState as Socket;
            socket.EndSend(ar);

            socket.Disconnect(false);
            socket = null;
            this.socket = null;
        }
    }
}