using UnityEngine;
using System.IO;
using System.Net;
using System.Threading;

namespace ML.PlaywallKids.Common
{
    public static class FTPUploader
    {
        public static void Upload(string url, string id, string password, string name, Texture tex)
        {
            if (tex == null) return;

            Texture2D tex2d = null;
            if (tex is RenderTexture)
                tex2d = _RenderTextureToTexture2D((RenderTexture)tex);
            else if (tex is Texture2D)
                tex2d = (Texture2D)tex;

            if (tex2d != null)
            {
                // Texture -> byte[]
                byte[] data = tex2d.EncodeToPNG();

                // RenderTexture인 경우 tex2d는 복사본이므로 메모리 정리 필요함.
                if (tex is RenderTexture)
                    Object.Destroy(tex2d);

                new Thread(() =>
                {
                    try
                    {
                        _UploadFTP(url, id, password, name, data);
                    }
                    catch (System.Exception) { }
                }).Start();
            }
        }

        private static Texture2D _RenderTextureToTexture2D(RenderTexture rt)
        {
            if (rt == null) return null;

            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;

            Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            texture.Apply();

            RenderTexture.active = prev;
            return texture;
        }

        private static void _UploadFTP(string url, string id, string password, string name, byte[] data)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(string.Format("ftp://{0}/{1}.png", url, name));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(id, password);
            request.ContentLength = data.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Debug.Log(string.Format("Upload File Complete, status {0}", response.StatusDescription));

            response.Close();
        }
    }
}