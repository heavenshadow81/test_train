using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    using AuthoringTool;
    public class AuthoringToolControl : MonoBehaviour
    {
        public GameObject authoringToolObject;
        public UIPanel palettePanel;
        public UIPanel toolsPanel;
        public UIPanel templatesPanel;

        public UISlider brushSizeSlider;

        public UISprite saveBackground;
        public Canvas_ canvas;

        protected UserData userData;
        protected int nInstanceId = 0;
        int frameCount = 0;
        int frameCount1 = 0;
        float updateRate = 1.0f / 48.0f; // 24 updates per sec.
        Vector3 area = new Vector3(0.04f, 0.04f, 0.04f);
        float startTime = 0.0f;
        float startTime1 = 0.0f;
        public float alpha = 0.04f;
        Collider[] mColliders;
        UIWidget[] mWidgets;
        float[] mAlpha;
        float mLastAlpha = 1f;
        int mLevel = 2;
        protected Vector3 palettePos;
        protected Vector3 crayonPos;
        protected Vector3 eraserPos;
        protected Vector3 markerPos;

        protected Vector3 rainbowBrushPos;
        protected Vector3 sprayPos;
        protected Vector3 pastelPos;

        float oldFactor = 0f;
        float newFactor = 0f;
        bool bPosition = true;
        bool bRotation = false;
        /*
        TweenPosition templatesPosition;
        TweenRotation templatesRotation1;
        TweenRotation templatesRotation2;
        TweenRotation templatesRotation3;
        TweenRotation templatesRotation4;
        TweenRotation templatesRotation5;
        */

        bool bMoveUp = false;
        bool bMoveDown = false;
        float minPos;
        float maxPos;
        TweenRotation[] templatesRosition;

        //	TweenRotation[] templatesRotation;

        void Start()
        {
            nInstanceId = authoringToolObject.GetComponent<UIPanel>().GetInstanceID();

            userData = UserData.Instance();

            NGUITools.SetActive(saveBackground.gameObject, false);

            authoringToolObject.transform.localScale = area;

            userData.SetState(nInstanceId, UserData.State.START);

            toolsPanel.transform.localPosition = new Vector3(toolsPanel.transform.localPosition.x, toolsPanel.transform.localPosition.y, 10);

            foreach (UISprite sprite in toolsPanel.GetComponentsInChildren<UISprite>())
            {
                sprite.transform.parent.parent.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                sprite.transform.parent.parent.transform.localPosition = new Vector3(180f, 10, -4f);
            }

            palettePanel.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            palettePanel.transform.localPosition = new Vector3(-80f, -80f, 0f);
            NGUITools.SetActive(palettePanel.gameObject, false);
            NGUITools.SetActive(toolsPanel.gameObject, false);

            palettePos = new Vector3(-168.9332f, -328.101f, 0f);

            crayonPos = new Vector3(240f, -57.41046f, -4f);
            eraserPos = new Vector3(100f, -240f, -4f);
            markerPos = new Vector3(240f, 115.6986f, -4f);
            rainbowBrushPos = new Vector3(240f, -115.6259f, -4f);
            sprayPos = new Vector3(240f, 19.72098f, -4f);
            pastelPos = new Vector3(225f, -201f, -4f);

            templatesRosition = templatesPanel.GetComponentsInChildren<TweenRotation>();

            foreach (UIPanel panel in templatesPanel.GetComponentsInChildren<UIPanel>())
            {
                if (!panel.name.Equals("Template Panel"))
                {
                    minPos = panel.transform.localPosition.y;
                    maxPos = panel.transform.localPosition.y + 20f;
                    break;
                }
            }
            
            Canvas canvas = GetComponentInChildren<Canvas>();
            canvas.gameObject.SetActive(false);
        }

        void Update()
        {
            //Debug.Log ("nInstanceId : " + userData.GetState (nInstanceId) + " / " + Input.mousePosition);

            switch (userData.GetState(nInstanceId))
            {
                case UserData.State.START:
                    AuthoringToolEffect(0.04f);
                    break;
                case UserData.State.WAIT:
                    if (bMoveUp)
                    {
                        startTime1 += Time.deltaTime;

                        if (startTime1 >= updateRate)
                        {
                            if (frameCount1 < 24)
                            {
                                if (frameCount1 > 11)
                                {
                                    foreach (UIPanel panel in templatesPanel.GetComponentsInChildren<UIPanel>())
                                    {
                                        float posY = 0f;
                                        if (!panel.name.Equals("Template Panel"))
                                        {
                                            if (panel.transform.localPosition.y < panel.transform.localPosition.y + maxPos)
                                                posY = (panel.transform.localPosition.y - maxPos) / (24f - frameCount1);
                                            panel.transform.localPosition = new Vector3(panel.transform.localPosition.x, panel.transform.localPosition.y - posY, panel.transform.localPosition.z);
                                        }
                                    }
                                }
                                frameCount1++;
                                startTime1 = 0;
                            }
                            else
                            {
                                bMoveUp = false;
                                frameCount1 = 0;
                                startTime1 = 0;

                                if (!templatesRosition[0].enabled)
                                {
                                    foreach (TweenRotation t in templatesRosition)
                                    {
                                        t.enabled = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (bMoveDown)
                    {
                        startTime1 += Time.deltaTime;

                        if (startTime1 >= updateRate)
                        {
                            if (frameCount1 < 12)
                            {
                                foreach (UIPanel panel in templatesPanel.GetComponentsInChildren<UIPanel>())
                                {
                                    float posY = 0f;
                                    if (!panel.name.Equals("Template Panel"))
                                    {
                                        if (panel.transform.localPosition.y > minPos)
                                            posY = (panel.transform.localPosition.y - minPos) / (12f - frameCount1);
                                        panel.transform.localPosition = new Vector3(panel.transform.localPosition.x, panel.transform.localPosition.y - posY, panel.transform.localPosition.z);
                                    }
                                }
                                frameCount1++;
                                startTime1 = 0;
                            }
                            else
                            {
                                bMoveUp = true;
                                bMoveDown = false;
                                frameCount1 = 0;
                                startTime1 = 0;
                            }
                        }
                    }

                    if (templatesRosition[0].enabled)
                    {
                        if (templatesRosition[0].tweenFactor > oldFactor)
                            newFactor += templatesRosition[0].tweenFactor - oldFactor;
                        else
                            newFactor += oldFactor - templatesRosition[0].tweenFactor;

                        oldFactor = templatesRosition[0].tweenFactor;
                        if (newFactor > 5.7f)
                        {
                            foreach (TweenRotation t in templatesRosition)
                            {
                                t.ResetToBeginning();
                                t.enabled = false;
                            }
                            bMoveDown = true;
                            newFactor = 0f;
                            oldFactor = 0f;
                        }
                    }
                    break;
                case UserData.State.CUSTOMIZING:
                    foreach (TweenRotation t in templatesRosition)
                    {
                        t.ResetToBeginning();
                        t.enabled = false;
                    }
                    bMoveUp = false;
                    bMoveDown = false;

                    if (!NGUITools.GetActive(saveBackground.gameObject))
                        NGUITools.SetActive(saveBackground.gameObject, true);
                    break;
                case UserData.State.DRAW_START:
                    ToolsMove();
                    break;
                case UserData.State.DRAW:
                    break;
                case UserData.State.CUSTOMIZING_BACK:
                    ToolsBack();
                    break;
                case UserData.State.END:
                    AuthoringToolEffect(-0.04f);
                    break;
                default:
                    break;
            }
        }

        void UpdateAlpha()
        {
            // Update the widget alpha
            for (int i = 0, imax = mWidgets.Length; i < imax; ++i)
            {
                UIWidget w = mWidgets[i];
                if (w != null)
                {
                    w.alpha = mAlpha[i] * alpha;
                }
            }

            if (mLevel == 0)
            {
                // Fade in started -- enable all game objects
                Transform trans = transform;
                for (int i = 0, imax = trans.childCount; i < imax; ++i)
                    NGUITools.SetActive(trans.GetChild(i).gameObject, true);
                for (int i = 0, imax = mColliders.Length; i < imax; ++i)
                    mColliders[i].enabled = false;
                mLevel = 1;
            }
            else if (mLevel == 2 && alpha < 0.99f)
            {
                // Fade out started -- disable tweens and colliders
                TweenColor[] tweens = GetComponentsInChildren<TweenColor>();
                for (int i = 0, imax = tweens.Length; i < imax; ++i)
                    tweens[i].enabled = false;
                for (int i = 0, imax = mColliders.Length; i < imax; ++i)
                    mColliders[i].enabled = false;
                mLevel = 1;
            }

            if (mLevel == 1)
            {
                if (alpha < 0.01f)
                {
                    // Fade out finished -- disable all game objects
                    Transform trans = transform;
                    for (int i = 0, imax = trans.childCount; i < imax; ++i)
                        NGUITools.SetActive(trans.GetChild(i).gameObject, false);
                    mLevel = 0;
                }
                else if (alpha > 0.99f)
                {
                    // Fade in finished -- enable all colliders
                    for (int i = 0, imax = mColliders.Length; i < imax; ++i)
                        mColliders[i].enabled = true;
                    mLevel = 2;
                }
            }
        }

        // 코멘트
        // 이펙트 업데이트는 초당 24번 진행됨.
        // 
        void AuthoringToolEffect(float fCount)
        {
            startTime += Time.deltaTime;

            if (startTime >= updateRate)
            {
                if (frameCount < 24)
                {
                    if (frameCount == 0)
                    {
                        mColliders = authoringToolObject.GetComponentsInChildren<Collider>(true);
                        mWidgets = authoringToolObject.GetComponentsInChildren<UIWidget>(true);

                        // Remember the initial alpha
                        mAlpha = new float[mWidgets.Length];
                        for (int i = 0, imax = mWidgets.Length; i < imax; ++i)
                            mAlpha[i] = mWidgets[i].alpha;

                        // Set the initial fade level
                        mLastAlpha = Mathf.Clamp01(alpha);
                        mLevel = (mLastAlpha > 0.99f) ? 2 : (mLastAlpha < 0.01f ? 0 : 1);

                        // set initial state
                        if (fCount > 0)
                        {
                            alpha = 0.04f;
                            area = new Vector3(0.04f, 0.04f, 0.04f);
                        }

                        UpdateAlpha();
                    }
                    frameCount++;

                    area += new Vector3(fCount, fCount, fCount);
                    authoringToolObject.transform.localScale = area;

                    alpha += fCount;
                    if (mLastAlpha != alpha)
                    {
                        mLastAlpha = alpha;
                        UpdateAlpha();
                    }
                    startTime = 0;
                }
                else
                {
                    if (userData.GetState(nInstanceId) == UserData.State.START)
                    {
                        userData.SetState(nInstanceId, UserData.State.WAIT);
                        bMoveUp = true;
                    }
                    else
                    {
                        alpha = 0f;
                        authoringToolObject.transform.localScale = Vector3.zero;
                        Destroy(authoringToolObject);
                        userData.DeleteData(nInstanceId);
                    }

                    frameCount = 0;
                    startTime = 0;
                }
            }
        }

        void TemplatesMove()
        {
        }

        void ToolsMove()
        {
            if (!NGUITools.GetActive(palettePanel.gameObject))
                NGUITools.SetActive(palettePanel.gameObject, true);
            if (!NGUITools.GetActive(toolsPanel.gameObject))
                NGUITools.SetActive(toolsPanel.gameObject, true);

            startTime += Time.deltaTime;

            if (startTime >= updateRate)
            {
                if (frameCount < 12)
                {
                    if (frameCount == 0)
                    {

                    }

                    Vector3 pos = (palettePanel.transform.localPosition - palettePos) / (12f - frameCount);
                    Vector3 area = (palettePanel.transform.localScale - new Vector3(1.4f, 1.4f, 1f)) / (12f - frameCount);

                    palettePanel.transform.localPosition -= pos;
                    palettePanel.transform.localScale -= area;

                    Vector3 pos2 = (toolsPanel.transform.localPosition - new Vector3(toolsPanel.transform.localPosition.x, toolsPanel.transform.localPosition.y, -4f)) / (12f - frameCount);

                    toolsPanel.transform.localPosition -= pos2;

                    foreach (UISprite sprite in toolsPanel.GetComponentsInChildren<UISprite>())
                    {
                        if (sprite.transform.parent.parent.name.Equals("Crayon Panel"))
                        {
                            Vector3 pos1 = (sprite.transform.parent.parent.transform.localPosition - crayonPos) / (12f - frameCount);
                            Vector3 area1 = (sprite.transform.parent.parent.transform.localScale - new Vector3(1.4f, 1.4f, 1f)) / (12f - frameCount);
                            Color col = (sprite.color - new Color(1f, 1f, 1f, 1f)) / (12f - frameCount);

                            sprite.transform.parent.parent.transform.localPosition -= pos1;
                            sprite.transform.parent.parent.transform.localScale -= area1;
                            sprite.color -= col;
                        }
                        if (sprite.transform.parent.parent.name.Equals("Eraser Panel"))
                        {
                            Vector3 pos1 = (sprite.transform.parent.parent.transform.localPosition - eraserPos) / (12f - frameCount);
                            Vector3 area1 = (sprite.transform.parent.parent.transform.localScale - new Vector3(1.4f, 1.4f, 1f)) / (12f - frameCount);
                            Color col = (sprite.color - new Color(1f, 1f, 1f, 1f)) / (12f - frameCount);

                            sprite.transform.parent.parent.transform.localPosition -= pos1;
                            sprite.transform.parent.parent.transform.localScale -= area1;
                            sprite.color -= col;
                        }
                        if (sprite.transform.parent.parent.name.Equals("Marker Panel"))
                        {
                            Vector3 pos1 = (sprite.transform.parent.parent.transform.localPosition - markerPos) / (12f - frameCount);
                            Vector3 area1 = (sprite.transform.parent.parent.transform.localScale - new Vector3(1.4f, 1.4f, 1f)) / (12f - frameCount);
                            Color col = (sprite.color - new Color(1f, 1f, 1f, 1f)) / (12f - frameCount);

                            sprite.transform.parent.parent.transform.localPosition -= pos1;
                            sprite.transform.parent.parent.transform.localScale -= area1;
                            sprite.color -= col;
                        }
                        if (sprite.transform.parent.parent.name.Equals("RainbowBrush Panel"))
                        {
                            Vector3 pos1 = (sprite.transform.parent.parent.transform.localPosition - rainbowBrushPos) / (12f - frameCount);
                            Vector3 area1 = (sprite.transform.parent.parent.transform.localScale - new Vector3(1.4f, 1.4f, 1f)) / (12f - frameCount);
                            Color col = (sprite.color - new Color(1f, 1f, 1f, 1f)) / (12f - frameCount);

                            sprite.transform.parent.parent.transform.localPosition -= pos1;
                            sprite.transform.parent.parent.transform.localScale -= area1;
                            sprite.color -= col;
                        }
                        if (sprite.transform.parent.parent.name.Equals("Spray Panel"))
                        {
                            Vector3 pos1 = (sprite.transform.parent.parent.transform.localPosition - sprayPos) / (12f - frameCount);
                            Vector3 area1 = (sprite.transform.parent.parent.transform.localScale - new Vector3(1.4f, 1.4f, 1f)) / (12f - frameCount);
                            Color col = (sprite.color - new Color(1f, 1f, 1f, 1f)) / (12f - frameCount);

                            sprite.transform.parent.parent.transform.localPosition -= pos1;
                            sprite.transform.parent.parent.transform.localScale -= area1;
                            sprite.color -= col;
                        }
                        if (sprite.transform.parent.parent.name.Equals("Pastel Panel"))
                        {
                            Vector3 pos1 = (sprite.transform.parent.parent.transform.localPosition - pastelPos) / (12f - frameCount);
                            Vector3 area1 = (sprite.transform.parent.parent.transform.localScale - new Vector3(1.4f, 1.4f, 1f)) / (12f - frameCount);
                            Color col = (sprite.color - new Color(1f, 1f, 1f, 1f)) / (12f - frameCount);

                            sprite.transform.parent.parent.transform.localPosition -= pos1;
                            sprite.transform.parent.parent.transform.localScale -= area1;
                            sprite.color -= col;
                        }
                    }

                    frameCount++;
                    startTime = 0;
                }
                else
                {
                    string strTools = "";
                    if (userData.GetBrushTool(nInstanceId) == UserData.BrushTool.CRAYON)
                    {
                        strTools = "Crayon Panel";
                    }
                    else if (userData.GetBrushTool(nInstanceId) == UserData.BrushTool.ERASER)
                    {
                        strTools = "Eraser Panel";
                    }
                    else if (userData.GetBrushTool(nInstanceId) == UserData.BrushTool.MARKER)
                    {
                        strTools = "Marker Panel";
                    }
                    else if (userData.GetBrushTool(nInstanceId) == UserData.BrushTool.BRUSH)
                    {
                        strTools = "RainbowBrush Panel";
                    }
                    else if (userData.GetBrushTool(nInstanceId) == UserData.BrushTool.SPRAY)
                    {
                        strTools = "Spray Panel";
                    }
                    else if (userData.GetBrushTool(nInstanceId) == UserData.BrushTool.PASTEL)
                    {
                        strTools = "Pastel Panel";
                    }

                    foreach (UISprite sprite in toolsPanel.GetComponentsInChildren<UISprite>())
                    {
                        if (sprite.transform.parent.parent.name.Equals(strTools))
                        {
                            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
                            sprite.transform.parent.parent.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
                        }
                        else
                        {
                            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
                            sprite.transform.parent.parent.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                        }
                    }

                    userData.SetState(nInstanceId, UserData.State.DRAW);
                    frameCount = 0;
                    startTime = 0;
                }
            }
        }

        void ToolsBack()
        {
            startTime += Time.deltaTime;

            if (startTime >= updateRate)
            {
                if (frameCount < 12)
                {

                    Vector3 pos = (palettePanel.transform.localPosition - new Vector3(-80f, -80f, 0f)) / (12f - frameCount);
                    Vector3 area = (palettePanel.transform.localScale - new Vector3(0.8f, 0.8f, 0.8f)) / (12f - frameCount);

                    palettePanel.transform.localPosition -= pos;
                    palettePanel.transform.localScale -= area;

                    Vector3 pos2 = (toolsPanel.transform.localPosition - new Vector3(toolsPanel.transform.localPosition.x, toolsPanel.transform.localPosition.y, -4f)) / (12f - frameCount);

                    toolsPanel.transform.localPosition -= pos2;

                    foreach (UISprite sprite in toolsPanel.GetComponentsInChildren<UISprite>())
                    {
                        if (sprite.transform.parent.parent.name.Equals("Crayon Panel"))
                        {
                            Vector3 pos1 = (sprite.transform.parent.parent.transform.localPosition - new Vector3(180f, 10f, 0f)) / (12f - frameCount);
                            Vector3 area1 = (sprite.transform.parent.parent.transform.localScale - new Vector3(0.8f, 0.8f, 0.8f)) / (12f - frameCount);
                            Color col = (sprite.color - new Color(0f, 0f, 0f, 0f)) / (12f - frameCount);

                            sprite.transform.parent.parent.transform.localPosition -= pos1;
                            sprite.transform.parent.parent.transform.localScale -= area1;
                            sprite.color -= col;
                        }
                        if (sprite.transform.parent.parent.name.Equals("Eraser Panel"))
                        {
                            Vector3 pos1 = (sprite.transform.parent.parent.transform.localPosition - new Vector3(180f, 10f, 0f)) / (12f - frameCount);
                            Vector3 area1 = (sprite.transform.parent.parent.transform.localScale - new Vector3(0.8f, 0.8f, 0.8f)) / (12f - frameCount);
                            Color col = (sprite.color - new Color(0f, 0f, 0f, 0f)) / (12f - frameCount);

                            sprite.transform.parent.parent.transform.localPosition -= pos1;
                            sprite.transform.parent.parent.transform.localScale -= area1;
                            sprite.color -= col;
                        }
                        if (sprite.transform.parent.parent.name.Equals("Marker Panel"))
                        {
                            Vector3 pos1 = (sprite.transform.parent.parent.transform.localPosition - new Vector3(180f, 10f, 0f)) / (12f - frameCount);
                            Vector3 area1 = (sprite.transform.parent.parent.transform.localScale - new Vector3(0.8f, 0.8f, 0.8f)) / (12f - frameCount);
                            Color col = (sprite.color - new Color(0f, 0f, 0f, 0f)) / (12f - frameCount);

                            sprite.transform.parent.parent.transform.localPosition -= pos1;
                            sprite.transform.parent.parent.transform.localScale -= area1;
                            sprite.color -= col;
                        }
                        if (sprite.transform.parent.parent.name.Equals("RainbowBrush Panel"))
                        {
                            Vector3 pos1 = (sprite.transform.parent.parent.transform.localPosition - new Vector3(180f, 10f, 0f)) / (12f - frameCount);
                            Vector3 area1 = (sprite.transform.parent.parent.transform.localScale - new Vector3(1f, 1f, 1f)) / (12f - frameCount);
                            Color col = (sprite.color - new Color(0f, 0f, 0f, 0f)) / (12f - frameCount);

                            sprite.transform.parent.parent.transform.localPosition -= pos1;
                            sprite.transform.parent.parent.transform.localScale -= area1;
                            sprite.color -= col;
                        }
                        if (sprite.transform.parent.parent.name.Equals("Spray Panel"))
                        {
                            Vector3 pos1 = (sprite.transform.parent.parent.transform.localPosition - new Vector3(180f, 10f, 0f)) / (12f - frameCount);
                            Vector3 area1 = (sprite.transform.parent.parent.transform.localScale - new Vector3(0.8f, 0.8f, 0.8f)) / (12f - frameCount);
                            Color col = (sprite.color - new Color(0f, 0f, 0f, 0f)) / (12f - frameCount);

                            sprite.transform.parent.parent.transform.localPosition -= pos1;
                            sprite.transform.parent.parent.transform.localScale -= area1;
                            sprite.color -= col;
                        }
                    }

                    frameCount++;
                    startTime = 0;
                }
                else
                {
                    if (NGUITools.GetActive(palettePanel.gameObject))
                        NGUITools.SetActive(palettePanel.gameObject, false);
                    if (NGUITools.GetActive(toolsPanel.gameObject))
                        NGUITools.SetActive(toolsPanel.gameObject, false);

                    userData.SetState(nInstanceId, UserData.State.CUSTOMIZING);
                    frameCount = 0;
                    startTime = 0;
                }
            }
        }

        public void OnBrushSizeSliderChange()
        {
            float size = brushSizeSlider.value * 90f + 10f;

            canvas.brush.diameter = size;
            if (canvas.brush.dualComponent.enable)
            {
                canvas.brush.dualComponent.scatter = canvas.brush.diameter;
                canvas.brush.dualComponent.count = (int)canvas.brush.diameter;
            }

            userData.SetBrushSize(nInstanceId, (int)size);
        }

        ~AuthoringToolControl()
        {
            Debug.Log("~AuthoringToolControl()");
        }
    }
}