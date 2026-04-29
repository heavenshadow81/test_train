using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.Aquarium
{
    public class SeaStoryServerTest : MonoBehaviour
    {
        public Texture2D[] inputs;
        public Texture2D[] outputs;

        public int userId = 0;
        public string templateName = "";

        string userIdStr = "";
        SeaStoryTemplatePacket packet = null;
        List<byte> bytes = new List<byte>();

        public void OnGUI()
        {
            GUI.Label(new Rect(20, 20, 100, 20), "User ID");
            userIdStr = GUI.TextField(new Rect(140, 20, 80, 20), userIdStr);
            int.TryParse(userIdStr, out userId);

            GUI.Label(new Rect(20, 50, 120, 20), "Template Name");
            templateName = GUI.TextField(new Rect(160, 50, 120, 20), templateName, 16);

            if (GUI.Button(new Rect(20, 80, 100, 30), "Byte Array!"))
            {
                List<SeaStoryTemplatePacket.ImageInfo> templateImages = new List<SeaStoryTemplatePacket.ImageInfo>();
                foreach (Texture2D input in inputs)
                {
                    SeaStoryTemplatePacket.ImageInfo image = new SeaStoryTemplatePacket.ImageInfo();
                    image.width = input.width;
                    image.height = input.height;
                    image.colors = input.GetPixels32();
                    templateImages.Add(image);
                }

                packet = new SeaStoryTemplatePacket(userId, templateName, templateImages);
                bytes.Clear();
                bytes.AddRange(packet.ToByteArray());
                packet = null;
                outputs = null;
            }

            if (bytes.Count > 0)
            {
                if (GUI.Button(new Rect(140, 80, 100, 30), "Parse!"))
                {
                    packet = SeaStoryTemplatePacket.Parse(bytes);
                    if (packet != null)
                    {
                        outputs = new Texture2D[packet.templateImages.Count];
                        for (int i = 0; i < outputs.Length; i++)
                        {
                            SeaStoryTemplatePacket.ImageInfo image = packet.templateImages[i];
                            Texture2D texture = new Texture2D(image.width, image.height, TextureFormat.ARGB32, false);
                            texture.SetPixels32(image.colors);
                            texture.Apply();
                            outputs[i] = texture;
                        }
                    }
                }
            }

            if (packet != null)
            {
                if (GUI.Button(new Rect(260, 80, 100, 30), "Save!"))
                {
                    SeaStoryTemplatePacketSave.Save(packet);
                }
            }

            // draw images
            if (inputs != null)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    GUI.DrawTexture(new Rect(20 + i * 140, 120, 128, 128), (Texture)inputs[i]);
                }
            }

            if (packet != null)
            {
                GUI.Label(new Rect(20, 280, 512, 20),
                          string.Format("Packet -> userId : {0}, templateName : {1}, image count : {2}", packet.userId, packet.templateName,
                                        packet.templateImages.Count));
            }

            if (outputs != null)
            {
                for (int i = 0; i < outputs.Length; i++)
                {
                    GUI.DrawTexture(new Rect(20 + i * 140, 320, 128, 128), (Texture)outputs[i]);
                }
            }
        }
    }
}