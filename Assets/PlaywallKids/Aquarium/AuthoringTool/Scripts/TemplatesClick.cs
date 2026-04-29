using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.Aquarium
{
    using AuthoringTool;
    public class TemplatesClick : MonoBehaviour
    {
        public string templateName;
        public UIPanel target;
        public UISprite templateShadow;
        public UITexture fillTexture;
        public UITexture drawingTexture;
        public UIPanel drawingPanel;
        public UIPanel patternPanel;
        public UIPanel templatesPanel;
        public UIPanel areaPanel;
        public GameObject gesturePrefab;
        public bool animateUI = false;

        protected UserData userData;
        protected int nInstanceId = 0;
        protected Vector3 basePos;
        int frameCount = 0;
        float updateRate = 1.0f / 48.0f; // 24 updates per sec.	
        float startTime = 0.0f;
        bool bClick = false;
        UIPanel[] panelTemplates;
        float minPos;
        float maxPos;
        int nIndex = 0;
        bool bIndex = false;

        float relativeScale = 1.0f;
        string[] strtemp;

        void Start()
        {
            nInstanceId = target.GetInstanceID();
            userData = UserData.Instance();

            panelTemplates = this.transform.parent.parent.GetComponentsInChildren<UIPanel>();
            minPos = this.transform.parent.transform.localPosition.y;
            maxPos = this.transform.parent.transform.localPosition.y + 30;
            basePos = drawingTexture.localSize;
        }

        void Update()
        {
            if (bIndex)
            {
                startTime += Time.deltaTime;

                if (startTime >= updateRate)
                {
                    if (frameCount < 12)
                    {
                        foreach (UIPanel panel in panelTemplates)
                        {
                            float posY = 0f;

                            if (!panel.name.Equals(this.transform.parent.name) && !panel.name.Equals("Template Panel"))
                            {
                                if (panel.transform.localPosition.y > minPos)
                                    posY = (panel.transform.localPosition.y - minPos) / (12f - frameCount);
                            }
                            else if (panel.name.Equals(this.transform.parent.name) && !panel.name.Equals("Template Panel"))
                            {
                                if (panel.transform.localPosition.y < maxPos)
                                    posY = (panel.transform.localPosition.y - maxPos) / (12f - frameCount);
                            }
                            panel.transform.localPosition = new Vector3(panel.transform.localPosition.x, panel.transform.localPosition.y - posY, panel.transform.localPosition.z);
                        }
                        frameCount++;
                        startTime = 0;
                    }
                    else
                    {
                        bIndex = false;
                        frameCount = 0;
                        startTime = 0;
                    }
                }
            }
        }

        void OnClick()
        {
            // Turn on legacy ui animation. (for AuthoringTool prefab)
            if (animateUI)
                bIndex = true;

            // Loads template information.
            TemplateDataInfo templateInfo = Resources.Load<TemplateDataInfo>(string.Format("Aquarium/Templates/Database/{0}", templateName));
            if(templateInfo == null)
            {
                Debug.LogWarning(string.Format("Couldn't find template named ({0}).", templateName));
                return;
            }

            if (userData.GetState(nInstanceId) == UserData.State.WAIT)
            {
                userData.SetState(nInstanceId, UserData.State.CUSTOMIZING);
            }
            else
            {
                foreach (UISprite sprite in target.GetComponentsInChildren<UISprite>())
                {
                    if (sprite.name.Equals("Save Background"))
                        sprite.spriteName = "SB_T_C_03";
                }
                foreach (UITexture texture in target.GetComponentsInChildren<UITexture>())
                {
                    if (texture.name.Equals("Fill Texture") || texture.name.Equals("Drawing Texture"))
                        NGUITools.SetActive(texture.gameObject, false);
                }
                userData.SetState(nInstanceId, UserData.State.CUSTOMIZING_BACK);
            }
            
            string[] strTemplates = new string[templateInfo.templates.Length];
            for (int i = 0; i < strTemplates.Length; i++)
            {
                if (!templateName.Equals("Jellyfish") && i == 0)
                {
                    strTemplates[i] = "1_1";
                }
                else
                    strTemplates[i] = "1";
            }
            strtemp = userData.SetTemplates(nInstanceId, strTemplates.Length, strTemplates);

            // Sets fill texture
            fillTexture.mainTexture = Resources.Load<Texture2D>(string.Concat("data/Templates/Fill/", templateName, "/", strtemp[0]));

            // Deletes previous ui textures
            {
                UITexture[] uiTexs = templatesPanel.GetComponentsInChildren<UITexture>();
                UITexture[] uiTexs2 = patternPanel.GetComponentsInChildren<UITexture>();
                UITexture[] uiTexs3 = areaPanel.GetComponentsInChildren<UITexture>();
                foreach (UITexture uiTex in uiTexs)
                    NGUITools.Destroy(uiTex.gameObject);
                foreach (UITexture uiTex in uiTexs2)
                    NGUITools.Destroy(uiTex.gameObject);
                foreach (UITexture uiTex in uiTexs3)
                    NGUITools.Destroy(uiTex.gameObject);
            }

            // Creates new ui textures
            {
                // Template
                for (int i = 0; i < templateInfo.templates.Length; i++)
                {
                    UITexture ut = NGUITools.AddWidget<UITexture>(templatesPanel.gameObject);
                    ut.name = string.Concat("Template Texture", templateInfo.templates[i]);
                    ut.cachedTransform.localPosition = drawingTexture.cachedTransform.localPosition;
                    ut.mainTexture = Resources.Load<Texture2D>(string.Concat("data/Templates/Template/", templateName, "/", templateInfo.templates[i], "/", userData.GetTemplate(nInstanceId, i)));
                    ut.depth = i;

                    if (i == 0)
                    {
                        // Gets relative scale of the template texture and scales proportionally.
                        float scaleX = basePos.x / ut.mainTexture.width;
                        float scaleY = basePos.y / ut.mainTexture.height;
                        relativeScale = Mathf.Min(scaleX, scaleY);

                        if (!templateName.Equals(userData.GetTemplateName(nInstanceId)))
                            userData.SetTemplateName(nInstanceId, UserData.TemplateType.TEMPLATE, templateName);

                        ut.SetDimensions((int)(ut.mainTexture.width * relativeScale), (int)(ut.mainTexture.height * relativeScale));
                        fillTexture.SetDimensions((int)(fillTexture.mainTexture.width * relativeScale * (fillTexture.mainTexture.width / (float)ut.mainTexture.width)), 
                            (int)(fillTexture.mainTexture.height * relativeScale * (fillTexture.mainTexture.height / (float)ut.mainTexture.height)));
                        drawingTexture.SetDimensions(ut.width, ut.height);
                    }
                    else
                    {
                        ut.SetDimensions(drawingTexture.width, drawingTexture.height);
                    }
                }

                // Pattern
                for (int i = 0; i < templateInfo.patterns.Length; i++)
                {
                    UITexture ut = NGUITools.AddWidget<UITexture>(patternPanel.gameObject);
                    ut.name = string.Concat("Pattern Texture", templateInfo.patterns[i]);
                    ut.cachedTransform.localPosition = drawingTexture.cachedTransform.localPosition;
                    ut.mainTexture = Resources.Load<Texture2D>(string.Concat("data/Templates/Pattern/", templateName, "/", templateInfo.patterns[i], "/", userData.GetTemplate(nInstanceId, i)));
                    ut.SetDimensions(drawingTexture.width, drawingTexture.height);
                    ut.depth = i;
                }

                // Area
                if (userData.GetState(nInstanceId) == UserData.State.START || userData.GetState(nInstanceId) == UserData.State.CUSTOMIZING_BACK || userData.GetState(nInstanceId) == UserData.State.CUSTOMIZING)
                {
                    Vector2[] areaPos = new Vector2[templateInfo.areas.Length];
                    System.Array.Copy(templateInfo.areas, areaPos, areaPos.Length);

                    for (int i = 0; i < areaPos.Length; i++)
                    {
                        GameObject prefab = NGUITools.AddChild(areaPanel.gameObject, gesturePrefab);
                        Gestures gestures = prefab.GetComponent<Gestures>();
                        gestures.target = target;
                        UITexture tex = prefab.GetComponent<UITexture>();
                        tex.name = string.Concat("Area Texture", (i + 1));
                        tex.mainTexture = Resources.Load<Texture2D>(string.Concat("data/Templates/Area/", templateName, "/", (i + 1)));
                        tex.depth = i;

                        tex.cachedTransform.localPosition = drawingTexture.cachedTransform.localPosition + Vector3.Scale(areaPos[i], new Vector3(relativeScale, relativeScale));
                        tex.SetDimensions((int)(tex.mainTexture.width * relativeScale), (int)(tex.mainTexture.height * relativeScale));
                    }
                }
            }

            // Hides unnecessary uis.
            NGUITools.SetActive(fillTexture.gameObject, false);
            NGUITools.SetActive(drawingTexture.gameObject, false);
        }
    }
}