using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TwoDimensionForeground : MonoBehaviour
    {

        [Range(2f, 4f)]
        public float fHScale;
        bool bArray;

        public Texture2D[] _imgs;
        public Texture2D[] imgs
        {
            get
            {
                if (_imgs == null)
                {
                    return null;
                }

                return _imgs;
            }
        }

        private UITexture[] _decos;
        public UITexture[] decos
        {
            get
            {
                if (_decos == null)
                {
                    _decos = new UITexture[2];
                }

                return _decos;
            }
        }

        private UITexture _foreground;
        public UITexture foreground
        {
            get
            {
                if (_foreground == null)
                {
                    _foreground = NGUITools.AddChild<UITexture>(cPanel.cachedGameObject);
                }
                return _foreground;
            }
        }

        private UIPanel _cPanel;
        public UIPanel cPanel
        {
            get
            {
                if (_cPanel == null)
                {
                    _cPanel = this.GetComponent<UIPanel>();
                }
                return _cPanel;
            }
        }

        void Awake()
        {
            if (fHScale == 0) fHScale = 2.5f;


            if (imgs != null)
            {

                if (imgs.Length == 0) return;

                bArray = imgs.Length > 1f;
                float w = imgs[0].width;
                float h = imgs[0].height;
                h += (h * 0.2f);
                float realHeight = UIRoot.list[0].activeHeight;
                float ratioHeight = ((float)realHeight / Screen.height);

                float ratioWidth = Screen.width * ratioHeight;
                float posY = ((realHeight / 2) - (h * 0.5f));// *(1 / ratioHeight) * ratioHeight; 

                //float imageHeight = (realHeight /2 ) -  

                for (int i = 0; i < decos.Length; ++i)
                {
                    bArray = (decos[i] != null);
                    decos[i] = NGUITools.AddChild<UITexture>(cPanel.cachedGameObject);
                    decos[i].name = "Decoration_" + i;

                    if (bArray)
                    {
                        decos[i].mainTexture = imgs[i];
                    }
                    else
                    {
                        decos[i].mainTexture = imgs[0];
                    }

                    decos[i].transform.localScale = new Vector3(UtilityScript.width * 1.1f, UtilityScript.height * 0.22f  /* / fHScale */, 1f);

                    int n = (i == 0) ? 1 : -1;
                    decos[i].transform.localPosition = new Vector3(0, n * posY, 0);

                    if (!bArray && (i == 1)) { decos[i].cachedTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180f)); }
                }
            }
        }
    }
}