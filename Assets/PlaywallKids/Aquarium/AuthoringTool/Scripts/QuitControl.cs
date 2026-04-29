using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    using AuthoringTool;
    using Common;

    /// <summary>
    /// 아쿠아리움 자유롭게 그리기 창 저장/취소 버튼
    /// </summary>
    public class QuitControl : MonoBehaviour
    {
        public UIPanel target;
        public UITexture drawingTexture;
        public UITexture fillTexture;
        public UIPanel patternPanel;
        public UIPanel templatesPanel;
        public UIPanel areaPanel;
        public Shader outputShader;
        public bool isSaveButton = true;
        public bool letFishWait = false;        // 물고기 생성 후 카메라 앞에서 대기 여부
        
        protected UserData userData;
        protected int nInstanceId = 0;
        protected string strUuid, strUuidShort;
        protected string strTemplate;
        Material outputMat;

        void Start()
        {
            userData = UserData.Instance();
            nInstanceId = target.GetInstanceID();
            outputMat = new Material(outputShader);
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
                    //fillTexture.SetDimensions(drawingTexture.width, drawingTexture.height);
                    userData.SetState(nInstanceId, UserData.State.DRAW_START);
                }
                else
                {
                    AToolFishInfo info = DoSaveing();
                    createFishesObj.Instance().SetFishesPath(info, target.transform.localPosition, exAtoolFath.InitMode.None);
                    userData.SetState(nInstanceId, UserData.State.END);
                }
            }
            else
            {
                userData.SetState(nInstanceId, UserData.State.END);
                Destroy(drawingTexture.gameObject);
                Destroy(fillTexture.gameObject);
            }
        }

        AToolFishInfo DoSaveing()
        {
            AToolFishInfo info = new AToolFishInfo();
            info.identifier = System.Guid.NewGuid().ToString();
            info.templateName = userData.GetTemplateName(nInstanceId);
            info.userId = userData.GetUserId(nInstanceId);
            strTemplate = info.templateName;
            strUuid = info.identifier;
            strUuidShort = strUuid.Substring(0, Mathf.Min(8, strUuid.Length));

            Texture sourceTex = drawingTexture.mainTexture;
            UITexture[] patternTexture = patternPanel.GetComponentsInChildren<UITexture>();
            UITexture[] templateTexture = templatesPanel.GetComponentsInChildren<UITexture>();
            RenderTexture printRenderTexture = new RenderTexture(sourceTex.width, sourceTex.height, 0, RenderTextureFormat.ARGB32);
            printRenderTexture.name = string.Format("{0}_print", strUuidShort);

            for (int i = 1; i <= templateTexture.Length; i++)
            {
                RenderTexture renderTexture = new RenderTexture(sourceTex.width, sourceTex.height, 0, RenderTextureFormat.ARGB32);
                renderTexture.name = string.Format("{0}_{1}", strUuidShort, i);
                Texture templateTex = null;
                Texture patternTex = null;
                Texture effectTex = null;
                
                foreach (UITexture texture in templateTexture)
                {
                    if (texture.name.Equals(string.Concat("Template Texture", i)))
                    {
                        templateTex = texture.mainTexture;
                        break;
                    }
                }

                foreach (UITexture texture in patternTexture)
                {
                    if (texture.name.Equals(string.Concat("Pattern Texture", i)))
                    {
                        patternTex = texture.mainTexture;
                        break;
                    }
                }

                if ((info.templateName.Equals("Dolphin") || info.templateName.Equals("Jellyfish")) && i == 1)
                {
                    string[] s = userData.GetTemplate(nInstanceId, 0).Split('_');

                    effectTex = Resources.Load(string.Concat("data/Templates/Effect/", info.templateName, "/1/", s[0])) as Texture2D;
                }

                outputMat.SetTexture("_TemplateTex", templateTex);
                outputMat.SetTexture("_PatternTex", patternTex);
                outputMat.SetTexture("_EffectTex", effectTex);

                Graphics.Blit(sourceTex, renderTexture, outputMat, 0);
                Graphics.Blit(renderTexture, printRenderTexture, outputMat, 1);

                info.textures.Add(renderTexture);
            }

            info.printTexture = printRenderTexture;
            ResourceManager.SaveAToolFish(info);

            FTPUploader.Upload(SettingsManager.ftpAddress, SettingsManager.ftpUsername, SettingsManager.ftpPassword, strUuid, printRenderTexture);
            
            Destroy(drawingTexture.gameObject);
            Destroy(fillTexture.gameObject);

            return info;
        }

        private Texture2D ToTexture2D(RenderTexture rt)
        {
            Texture2D outTex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, true, true);
            RenderTexture prevActive = RenderTexture.active;
            RenderTexture.active = rt;
            outTex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            RenderTexture.active = prevActive;
            outTex.Apply();
            return outTex;
        }
    }
}