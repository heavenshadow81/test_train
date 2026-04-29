using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    public class TemplateControl : MonoBehaviour
    {
        public GameObject target;
        public GameObject controlObject;
        bool nEffect = false;
        Vector3 startPos;
        UIPanel[] mWidgets;
        Vector3[] mstartPos;
        int frameCount = 0;
        float updateRate = 1.0f / 48.0f; // 24 updates per sec.	
        float startTime = 0.0f;
        //	int nClick = 0;
        float minPos;
        float maxPos;
        bool bIndex = false;

        // Use this for initialization
        void Start()
        {
            startPos = new Vector3(controlObject.transform.localPosition.x, controlObject.transform.localPosition.y, 0f);
            controlObject.transform.localPosition = startPos;

            mWidgets = target.GetComponentsInChildren<UIPanel>(true);

            mstartPos = new Vector3[mWidgets.Length];

            for (int i = 0, imax = mWidgets.Length; i < imax; ++i)
            {
                UIPanel w = mWidgets[i];
                if (w != null && !w.name.Equals("Template Panel"))
                {
                    minPos = w.transform.localPosition.y;
                    maxPos = w.transform.localPosition.y + 30;
                    break;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (bIndex)
            {
                startTime += Time.deltaTime;

                if (startTime >= updateRate)
                {
                    if (frameCount < 12)
                    {
                        foreach (UIPanel panel in mWidgets)
                        {
                            float posY = 0f;

                            if (!panel.name.Equals("Template Panel"))
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
            bIndex = true;

            if (!nEffect)
                nEffect = true;
            else
                nEffect = false;
        }
    }
}