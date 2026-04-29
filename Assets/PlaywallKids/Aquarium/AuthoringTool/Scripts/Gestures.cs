using UnityEngine;
using System;

namespace ML.PlaywallKids.Aquarium
{
    using AuthoringTool;
    public class Gestures : MonoBehaviour
    {
        public UIPanel target;
        protected UserData userData;
        protected int nInstanceId = 0;
        int nIndex = 1;

        void Start()
        {
            userData = UserData.Instance();
            nInstanceId = target.GetInstanceID();
        }

        void OnClick()
        {
            nIndex = ((nIndex % 3) + 1);

            Doing(nIndex);
        }
        
        void Doing(int nNumber)
        {
            string sPath = "data\\Templates\\";

            string strTemplate = userData.GetTemplateName(nInstanceId);

            string[] strtemp;

            UITexture[] tex = this.transform.parent.parent.GetComponentsInChildren<UITexture>();

            if (this.name.Equals("Area Texture1"))
            {
                if (strTemplate.Equals("Bluetang"))
                {
                    string[] s = userData.GetTemplate(nInstanceId, 0).Split('_');

                    userData.SetTemplate(nInstanceId, string.Concat(s[0], "_", nNumber.ToString()), 0);
                    userData.SetTemplate(nInstanceId, nNumber.ToString(), 1);
                    userData.SetTemplate(nInstanceId, nNumber.ToString(), 2);
                }
                else if (strTemplate.Equals("Dolphin"))
                {
                    string[] s = userData.GetTemplate(nInstanceId, 0).Split('_');

                    userData.SetTemplate(nInstanceId, string.Concat(s[0], "_", nNumber.ToString()), 0);
                    userData.SetTemplate(nInstanceId, nNumber.ToString(), 1);
                }
                else if (strTemplate.Equals("Nemo"))
                {
                    string[] s = userData.GetTemplate(nInstanceId, 0).Split('_');

                    userData.SetTemplate(nInstanceId, string.Concat(nNumber.ToString(), "_", s[1]), 0);
                    userData.SetTemplate(nInstanceId, nNumber.ToString(), 1);
                }
                else if (strTemplate.Equals("Jellyfish"))
                {
                    userData.SetTemplate(nInstanceId, nNumber.ToString(), 0);
                }
                else if (strTemplate.Equals("Puffer"))
                {
                    string[] s = userData.GetTemplate(nInstanceId, 0).Split('_');

                    userData.SetTemplate(nInstanceId, string.Concat(s[0], "_", nNumber.ToString()), 0);

                    userData.SetTemplate(nInstanceId, nNumber.ToString(), 2);
                }
            }
            else if (this.name.Equals("Area Texture2"))
            {
                if (strTemplate.Equals("Bluetang"))
                {
                    string[] s = userData.GetTemplate(nInstanceId, 0).Split('_');

                    userData.SetTemplate(nInstanceId, string.Concat(nNumber.ToString(), "_", s[1]), 0);
                }
                else if (strTemplate.Equals("Dolphin"))
                {
                    string[] s = userData.GetTemplate(nInstanceId, 0).Split('_');

                    userData.SetTemplate(nInstanceId, string.Concat(nNumber.ToString(), "_", s[1]), 0);
                }
                else if (strTemplate.Equals("Nemo"))
                {
                    string[] s = userData.GetTemplate(nInstanceId, 0).Split('_');

                    userData.SetTemplate(nInstanceId, string.Concat(s[0], "_", nNumber.ToString()), 0);
                }
                else if (strTemplate.Equals("Puffer"))
                {
                    string[] s = userData.GetTemplate(nInstanceId, 0).Split('_');

                    userData.SetTemplate(nInstanceId, string.Concat(nNumber.ToString(), "_", s[1]), 0);
                    userData.SetTemplate(nInstanceId, nNumber.ToString(), 1);
                }
            }

            strtemp = userData.GetTemplates(nInstanceId);

            for (int i = 0; i < tex.Length; i++)
            {
                if (tex[i].name.IndexOf("Template") != -1)
                {
                    int nCount = Convert.ToInt32(tex[i].name.Substring(tex[i].name.Length - 1, 1));
                    tex[i].mainTexture = Resources.Load(string.Concat(sPath.Replace("\\", "/"), "Template/", strTemplate, "/" + nCount + "/", strtemp[nCount - 1])) as Texture2D;
                }
                else if (tex[i].name.IndexOf("Pattern") != -1)
                {
                    int nCount = Convert.ToInt32(tex[i].name.Substring(tex[i].name.Length - 1, 1));
                    tex[i].mainTexture = Resources.Load(string.Concat(sPath.Replace("\\", "/"), "Pattern/", strTemplate, "/" + nCount + "/", strtemp[nCount - 1])) as Texture2D;
                }
            }
        }
    }
}