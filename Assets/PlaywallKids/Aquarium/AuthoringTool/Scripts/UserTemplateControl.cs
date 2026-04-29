using System.IO;
using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.Aquarium
{
    using AuthoringTool;
    public class UserTemplateControl : MonoBehaviour
    {
        public GameObject target;
        public GameObject controlObject;
        bool nEffect = false;
        Vector3 startPos;
        UIPanel[] mWidgets;
        Vector3[] mstartPos;
        public GameObject _UserTemplate;
        private ArrayList templateList = new ArrayList();
        bool nBoolClick = false;
        bool nBoolMessage = false;
        string strTemplateName;
        protected UserData userData;
        protected int nInstanceId = 0;

        // Use this for initialization
        void Start()
        {
            nInstanceId = target.transform.parent.gameObject.GetComponent<UIPanel>().GetInstanceID();

            userData = UserData.Instance();

            //string sPath = "data\\UserTemplate";
            string sPath = string.Concat("data\\UserTemplate\\", userData.GetUserId(nInstanceId));

            string sFileName = "*.png";

            int nIndex = 0;

            DirectoryInfo dir = new DirectoryInfo(sPath);

            if (dir.Exists == true)
            {
                foreach (string s in Directory.GetFiles(sPath, sFileName, SearchOption.AllDirectories))
                {
                    nIndex++;
                    templateList.Add(s);
                    /*
                    GameObject go = NGUITools.AddChild (target, _UserTemplate);
                    go.name = s.Replace (string.Concat (sPath, "\\"), "").Replace (".png", " Panel");
                    go.transform.localPosition = new Vector3 (0f, 0f - (65f * nIndex), 0f);

                    UIButton btn = go.GetComponentInChildren<UIButton> ();
                    btn.name = s.Replace (string.Concat (sPath, "\\"), "").Replace (".png", " Button");
                    UISlicedSprite back = go.GetComponentInChildren<UISlicedSprite> ();
                    back.spriteName = s.Replace (string.Concat (sPath, "\\"), "").Replace (".png", "");
                    */
                }
            }

            startPos = new Vector3(controlObject.transform.localPosition.x, controlObject.transform.localPosition.y, 0f);
            controlObject.transform.localPosition = startPos;

            mWidgets = target.GetComponentsInChildren<UIPanel>(true);

            mstartPos = new Vector3[mWidgets.Length];

            for (int i = 1, imax = mWidgets.Length; i < imax; ++i)
            {
                UIPanel w = mWidgets[i];
                if (w != null)
                    mstartPos[i] = new Vector3(w.transform.localPosition.x, w.transform.localPosition.y, w.transform.localPosition.z);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (nBoolClick)
            {
                for (int i = 1, imax = mWidgets.Length; i < imax; ++i)
                {
                    UIPanel w = mWidgets[i];
                    if (w != null)
                        w.transform.localPosition = new Vector3(mstartPos[i].x - 24f, w.transform.localPosition.y, w.transform.localPosition.z);
                }
                controlObject.transform.localPosition = new Vector3(controlObject.transform.localPosition.x, controlObject.transform.localPosition.y, 1f);


                if (!nEffect)
                {
                    for (int i = 1, imax = mWidgets.Length; i < imax; ++i)
                    {
                        UIPanel w = mWidgets[i];
                        if (w != null)
                            w.transform.localPosition = new Vector3(mstartPos[i].x - 24f, w.transform.localPosition.y, w.transform.localPosition.z);
                    }
                    controlObject.transform.localPosition = new Vector3(controlObject.transform.localPosition.x, controlObject.transform.localPosition.y, 1f);

                    nEffect = true;
                }
                else
                {
                    for (int i = 1, imax = mWidgets.Length; i < imax; ++i)
                    {
                        UIPanel w = mWidgets[i];
                        if (w != null)
                            w.transform.localPosition = new Vector3(mstartPos[i].x, w.transform.localPosition.y, w.transform.localPosition.z);
                    }
                    controlObject.transform.localPosition = startPos;

                    nEffect = false;
                }
                nBoolClick = false;
            }

            if (nBoolMessage)
            {
                if (!nEffect)
                {
                    for (int i = 1, imax = mWidgets.Length; i < imax; ++i)
                    {
                        UIPanel w = mWidgets[i];
                        if (w != null)
                        {
                            w.transform.localPosition = new Vector3(mstartPos[i].x - 24f, w.transform.localPosition.y, w.transform.localPosition.z);

                            if (!w.name.Replace(" Panel", "").Equals(strTemplateName))
                                w.transform.localPosition = new Vector3(w.transform.localPosition.x, mstartPos[i].y, w.transform.localPosition.z);
                        }
                    }
                    controlObject.transform.localPosition = new Vector3(controlObject.transform.localPosition.x, controlObject.transform.localPosition.y, 1f);

                    nEffect = true;
                }
                else
                {
                    for (int i = 1, imax = mWidgets.Length; i < imax; ++i)
                    {

                        UIPanel w = mWidgets[i];
                        if (w != null)
                        {
                            w.transform.localPosition = new Vector3(mstartPos[i].x, w.transform.localPosition.y, w.transform.localPosition.z);

                            if (w.name.Replace(" Panel", "").Equals(strTemplateName))
                            {
                                w.transform.localPosition = new Vector3(mstartPos[i].x - 24f, w.transform.localPosition.y, w.transform.localPosition.z);
                            }
                        }
                    }
                    controlObject.transform.localPosition = startPos;

                    nEffect = false;
                }
                nBoolMessage = false;
            }
        }

        void OnClick()
        {
            nBoolClick = true;
        }

        void selectUserTemplate(string strName)
        {
            strTemplateName = strName;
            nBoolMessage = true;
        }
    }
}