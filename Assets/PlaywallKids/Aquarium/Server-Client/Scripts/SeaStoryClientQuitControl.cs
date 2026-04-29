using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace ML.PlaywallKids.Aquarium
{
    using AuthoringTool;
    /*
     * 시연용 QuitControl
     */
    public class SeaStoryClientQuitControl : MonoBehaviour
    {
        public UIPanel target;
        public UITexture drawingTexture;
        public UITexture fillTexture;
        public UIPanel patternPanel;
        public UIPanel templatesPanel;
        public UIPanel areaPanel;
        public bool isSaveButton = true;

        protected UserData userData;
        protected int nInstanceId = 0;
        string strTemplate;

        void Start()
        {
            userData = UserData.Instance();
            nInstanceId = target.GetInstanceID();
        }

        void OnClick()
        {

            if (isSaveButton)
            {
                if (userData.GetState(nInstanceId) == UserData.State.CUSTOMIZING)
                {
                    foreach (UISprite sprite in target.GetComponentsInChildren<UISprite>())
                    {
                        if (sprite.name.Equals("Save Background"))
                            sprite.spriteName = "SB_T_C_01";
                    }

                    UITexture[] areaTextures = areaPanel.GetComponentsInChildren<UITexture>();
                    foreach (var uiTex in areaTextures)
                        NGUITools.Destroy(uiTex.gameObject);

                    NGUITools.SetActive(drawingTexture.gameObject, true);
                    NGUITools.SetActive(fillTexture.gameObject, true);

                    // Activate canvas
                    Canvas_ canvas = drawingTexture.GetComponent<Canvas_>();
                    if (canvas != null)
                    {
                        canvas.textureSize = drawingTexture.localSize;
                        canvas.ClearCanvas();
                        canvas.wantsPaint = true;
                    }

                    string strTexName = userData.GetTemplateName(nInstanceId);
                    fillTexture.mainTexture = Resources.Load(string.Concat("data/Templates/Fill/", strTexName, "/", userData.GetTemplate(nInstanceId, 0))) as Texture2D;
                    fillTexture.cachedTransform.localPosition = drawingTexture.cachedTransform.localPosition;
                    userData.SetState(nInstanceId, UserData.State.DRAW_START);
                }
                else
                {
                    MakeTexture();
                }
            }
            else
            {
                userData.SetState(nInstanceId, UserData.State.END);
                Destroy(drawingTexture.gameObject);
                Destroy(fillTexture.gameObject);
            }
        }

        void MakeTexture()
        {
            strTemplate = userData.GetTemplateName(nInstanceId);

            Texture sourceTex = drawingTexture.mainTexture;
            Texture2D tex = null;
            if (sourceTex is RenderTexture)
                tex = ScaleTextureFromRenderTexture((RenderTexture)sourceTex, drawingTexture.mainTexture.width, drawingTexture.mainTexture.height);
            else
                tex = ScaleTexture((Texture2D)Instantiate(drawingTexture.mainTexture), drawingTexture.mainTexture.width, drawingTexture.mainTexture.height);

            UITexture[] patternTexture = patternPanel.GetComponentsInChildren<UITexture>();
            UITexture[] templateTexture = templatesPanel.GetComponentsInChildren<UITexture>();

            Color[] drawingCol = tex.GetPixels();

            List<SeaStoryTemplatePacket.ImageInfo> templateImages = new List<SeaStoryTemplatePacket.ImageInfo>();


            for (int i = 1; i < templateTexture.Length + 1; i++)
            {
                bool bPattern = false;
                Texture2D templateTex = new Texture2D(4, 4);
                Texture2D patternTex = new Texture2D(4, 4);
                Texture2D effectTex = new Texture2D(4, 4);

                Color[] templateCol = new Color[drawingCol.Length];
                Color[] patternCol = new Color[drawingCol.Length];
                Color32[] saveCol = new Color32[drawingCol.Length];
                Color[] effectCol = new Color[drawingCol.Length];

                foreach (UITexture texture in templateTexture)
                {
                    if (texture.name.Equals(string.Concat("Template Texture", i)))
                    {
                        if (templateTex != null)
                        {
                            Destroy(templateTex);
                        }
                        templateTex = (Texture2D)Instantiate(texture.mainTexture);
                        templateTex = ScaleTexture(templateTex, tex.width, tex.height);

                        templateCol = templateTex.GetPixels();
                    }
                }

                foreach (UITexture texture in patternTexture)
                {
                    if (texture.name.Equals(string.Concat("Pattern Texture", i)))
                    {
                        if (patternTex != null)
                        {
                            Destroy(patternTex);
                        }
                        bPattern = true;

                        patternTex = (Texture2D)Instantiate(texture.mainTexture);
                        patternTex = ScaleTexture(patternTex, tex.width, tex.height);
                        patternCol = patternTex.GetPixels();
                    }
                }

                if ((strTemplate.Equals("Dolphin") || strTemplate.Equals("Jellyfish")) && i == 1)
                {
                    string[] s = userData.GetTemplate(nInstanceId, 0).Split('_');

                    if (effectTex != null)
                    {
                        Destroy(effectTex);
                    }

                    effectTex = Resources.Load(string.Concat("data/Templates/Effect/", strTemplate, "/1/", s[0])) as Texture2D;
                    effectCol = effectTex.GetPixels();
                }

                for (int px = 0; px < templateCol.Length; px++)
                {
                    saveCol[px] = templateCol[px];

                    if (templateCol[px].a > 0f && (templateCol[px] != Color.black && templateCol[px] != Color.white))
                    {
                        if (templateCol[px].a > 0f)
                        {
                            saveCol[px] = Color.Lerp(templateCol[px], drawingCol[px], drawingCol[px].a);
                        }

                        if (bPattern && patternCol[px].a > 0f)
                        {
                            //saveCol [px] = Color.Lerp(saveCol [px], patternCol [px], patternCol [px].a);
                            saveCol[px] = new Color(patternCol[px].r, patternCol[px].g, patternCol[px].b, 1f);
                        }
                    }

                    if ((strTemplate.Equals("Dolphin") || strTemplate.Equals("Jellyfish")) && i == 1)
                    {
                        if (effectCol[px].a > 0f)
                        {
                            saveCol[px] = Color.Lerp(saveCol[px], effectCol[px], effectCol[px].a);
                        }
                    }
                }

                SeaStoryTemplatePacket.ImageInfo image = new SeaStoryTemplatePacket.ImageInfo();
                image.width = tex.width;
                image.height = tex.height;
                image.colors = saveCol;
                templateImages.Add(image);

                Destroy(templateTex);
                Destroy(patternTex);

                templateCol = null;
                patternCol = null;
                saveCol = null;
                effectCol = null;
            }

            SeaStoryTemplatePacket packet = new SeaStoryTemplatePacket(nInstanceId, strTemplate, templateImages);

            SendPacket(packet);

            userData.SetState(nInstanceId, UserData.State.END);

            userData.SetBrushTool(nInstanceId, UserData.BrushTool.NONE);
            userData.SetColor(nInstanceId, Color.white);

            Destroy(tex);

            Destroy(drawingTexture.gameObject);
            Destroy(fillTexture.gameObject);
        }

        void SendPacket(SeaStoryTemplatePacket packet)
        {
            Socket socket = null;

            try
            {
                string IP = "127.0.0.1";
                try
                {
                    string jsonText = File.ReadAllText("./settings.txt");
                    Dictionary<string, object> dict = MiniJSON.Json.Deserialize(jsonText) as Dictionary<string, object>;
                    if (dict.ContainsKey("ip") && dict["ip"] != null)
                    {
                        IP = dict["ip"].ToString();
                    }
                }
                catch (IOException e2)
                {
                    Debug.Log(e2);
                }

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(IP), 5999);
                socket.Connect((EndPoint)endPoint);
                byte[] bytes = packet.ToByteArray();

                socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new System.AsyncCallback(OnSendFinish), socket);

                bytes = null;
                packet = null;
                /*
                socket.Send(bytes);
                socket.Disconnect(false);
                socket.Close();
                socket = null;
                packet = null;
                System.GC.Collect();
                */
            }
            catch (System.Exception e)
            {
                Debug.Log("Connect failed!");
                Debug.Log(e);

                try
                {
                    if (socket != null)
                    {
                        socket.Disconnect(false);
                    }
                }
                catch (System.Exception e2)
                {
                    Debug.Log(e2);
                }
            }
        }

        public void OnSendFinish(System.IAsyncResult ar)
        {
            Socket socket = ar.AsyncState as Socket;
            if (socket != null)
            {
                socket.EndSend(ar);

                socket.Disconnect(false);
                socket.Close();
                socket = null;
            }

            System.GC.Collect();
        }

        private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);

            Color[] rpixels = result.GetPixels(0);

            float incX = ((float)1 / source.width) * ((float)source.width / targetWidth);

            float incY = ((float)1 / source.height) * ((float)source.height / targetHeight);

            for (int px = 0; px < rpixels.Length; px++)
            {
                rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
            }
            result.SetPixels(rpixels, 0);

            result.Apply();

            return result;

        }

        private Texture2D ScaleTextureFromRenderTexture(RenderTexture source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(source.width, source.height, TextureFormat.RGBA32, true);

            RenderTexture prevActive = RenderTexture.active;
            RenderTexture.active = source;
            result.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
            RenderTexture.active = prevActive;

            if (result.width != targetWidth || result.height != targetHeight)
                result.Reinitialize(targetWidth, targetHeight);
            result.Apply();

            return result;
        }
    }
}