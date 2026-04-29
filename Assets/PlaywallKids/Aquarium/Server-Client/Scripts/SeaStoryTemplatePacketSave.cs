using UnityEngine;
using System.IO;

namespace ML.PlaywallKids.Aquarium
{
    public class SeaStoryTemplatePacketSave : MonoBehaviour
    {
        public static void Save(SeaStoryTemplatePacket packet)
        {
            if (packet == null) return;

            int userId = packet.userId;
            string templateName = packet.templateName;

            string sUserPath = string.Concat(Application.dataPath, "/Resources/data/UserTemplate/");

            string path = string.Concat(sUserPath, userId, "/", templateName);
            path = path.Replace("/", "\\");
            path = path.Replace("\"", "");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            for (int i = 0; i < packet.templateImages.Count; i++)
            {
                var image = packet.templateImages[i];

                Texture2D saveTex = new Texture2D(image.width, image.height, TextureFormat.ARGB32, false);
                saveTex.SetPixels32(image.colors);
                saveTex.Apply();

                string filePath = string.Format("{0}{1}/{2}/{3}.png", sUserPath, userId, templateName, i + 1);
                filePath = filePath.Replace("/", "\\");
                filePath = filePath.Replace("\"", "");

                try
                {
                    BinaryWriter bw = new BinaryWriter(new FileStream(filePath, FileMode.Create));
                    bw.Write(saveTex.EncodeToPNG());
                    bw.Close();
                }
                catch (IOException e)
                {
                    Debug.Log(e);
                }
            }
        }
    }
}