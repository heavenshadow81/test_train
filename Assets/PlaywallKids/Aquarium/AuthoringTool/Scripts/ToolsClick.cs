using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    using AuthoringTool;
    public class ToolsClick : MonoBehaviour
    {
        public UIPanel target;
        public Canvas_ canvas;

        protected UISprite brushSprite;
        protected bool bDrag;
        protected Vector2 dragStartPos = Vector2.zero;
        protected Vector2 dragEndPos = Vector2.zero;
        protected UIPanel[] toolsPanel;
        protected Rect rect;
        protected float areaX;
        protected float areaY;
        protected Vector2 startPos;
        protected Vector2 endPos;
        protected UserData userData;
        protected int nInstanceId = 0;
        Camera guiCam;
        Rect cameraRect;
        UIDragObject[] toolsDrag;

        public UserData.BrushTool tool = UserData.BrushTool.NONE;

        void Start()
        {
            nInstanceId = target.GetInstanceID();
            userData = UserData.Instance();

            // 그리기 도구 객체 참조
            toolsPanel = gameObject.transform.parent.parent.gameObject.GetComponentsInChildren<UIPanel>();

            //GUI객체의 카메라 객체입니다.
            guiCam = NGUITools.FindCameraForLayer(gameObject.layer);

            cameraRect = new Rect(0, 0, Screen.width, Screen.height);

            toolsDrag = this.transform.parent.parent.GetComponentsInChildren<UIDragObject>();
        }

        void OnClick()
        {
            if (userData.GetState(nInstanceId) == UserData.State.DRAW)
            {
                string toolName = this.name.Replace(" Button", "");

                foreach (UISprite sprite in this.transform.parent.parent.GetComponentsInChildren<UISprite>())
                {
                    if (sprite.transform.parent.name.Equals(this.name))
                    {
                        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
                        sprite.transform.parent.parent.transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                    else
                    {
                        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
                        sprite.transform.parent.parent.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    }
                }

                if (tool != UserData.BrushTool.NONE)
                {
                    userData.SetBrushTool(nInstanceId, tool);

                    if (canvas != null)
                    {
                        switch (tool)
                        {
                            case UserData.BrushTool.MARKER:
                                canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameMarker);
                                break;
                            case UserData.BrushTool.CRAYON:
                                canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameCrayon);
                                break;
                            case UserData.BrushTool.SPRAY:
                                canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameAirbrush);
                                break;
                            case UserData.BrushTool.BRUSH:
                                canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameRainbow);
                                break;
                            case UserData.BrushTool.PASTEL:
                                canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNamePastel);
                                break;
                            case UserData.BrushTool.ERASER:
                                canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameEraser);
                                break;
                        }

                        if (userData.GetBrushTool(nInstanceId) == UserData.BrushTool.ERASER)
                            canvas.brush.color = new Color32(228, 244, 249, 255);
                        else
                            canvas.brush.color = userData.GetColor(nInstanceId);
                        canvas.brush.diameter = userData.GetBrushSize(nInstanceId);
                        if (canvas.brush.dualComponent.enable)
                        {
                            canvas.brush.dualComponent.scatter = canvas.brush.diameter;
                            canvas.brush.dualComponent.count = (int)canvas.brush.diameter;
                        }
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (userData.GetState(nInstanceId) == UserData.State.DRAW)
            {
                getArea();
            }
        }

        void OnPress(bool pressed)
        {
            if (pressed)
            {
                foreach (UIDragObject d in toolsDrag)
                {
                    d.enabled = true;
                }
            }
        }

        void OnDrag(Vector2 delta)
        {
            //	getArea ();
        }

        void getArea()
        {
            startPos = (Vector2)this.transform.parent.transform.localPosition;

            // 선택된 도구 크기 구하기		
            areaX = this.GetComponentInChildren<UISprite>().transform.localScale.x / 2f;
            areaY = this.GetComponentInChildren<UISprite>().transform.localScale.y / 2f;

            Vector2 pos = (Vector2)guiCam.WorldToScreenPoint(new Vector3(this.transform.parent.transform.position.x, this.transform.parent.transform.position.y, 0f));

            Vector2[] posArea = new Vector2[2];
            posArea[0] = new Vector2(pos.x + areaX, pos.y + areaY);
            posArea[1] = new Vector2(pos.x - areaX, pos.y - areaY);

            if (cameraRect.Contains(posArea[0]) && cameraRect.Contains(posArea[1]))
            {
                rect = new Rect((this.transform.parent.transform.localPosition.x - areaX), (this.transform.parent.transform.localPosition.y + areaY), (this.transform.parent.transform.localPosition.x + areaX), (this.transform.parent.transform.localPosition.y - areaY));

                Rect[] toolsRect = new Rect[toolsPanel.Length];

                // 선택된 도구를 제외한 도구 크기 구하기

                for (int i = 0; i < toolsPanel.Length; i++)
                {
                    if (!toolsPanel[i].name.Equals(this.transform.parent.name) && !toolsPanel[i].name.Equals(this.transform.parent.parent.name))
                    {
                        float tempX = toolsPanel[i].GetComponentInChildren<UISprite>().transform.localScale.x / 2f;
                        float tempY = toolsPanel[i].GetComponentInChildren<UISprite>().transform.localScale.y / 2f;

                        toolsRect[i] = new Rect((toolsPanel[i].transform.localPosition.x - tempX), (toolsPanel[i].transform.localPosition.y + tempY)
                    , (toolsPanel[i].transform.localPosition.x + tempX), (toolsPanel[i].transform.localPosition.y - tempY));

                        // 도구 충돌 체크							

                        if (toolsRect[i].x <= rect.width && toolsRect[i].width >= rect.x && toolsRect[i].y >= rect.height && toolsRect[i].height <= rect.y)
                        {
                            toolsPanel[i].transform.localPosition = new Vector3((toolsPanel[i].transform.localPosition.x + (startPos.x - endPos.x)), (toolsPanel[i].transform.localPosition.y + (startPos.y - endPos.y)), toolsPanel[i].transform.localPosition.z);
                        }
                    }
                }
            }
            else
            {
                foreach (UIDragObject d in toolsDrag)
                {
                    //d.restrictWithinPanel = true;
                    d.enabled = false;
                    this.transform.parent.transform.localPosition = new Vector3(endPos.x, endPos.y, this.transform.parent.transform.localPosition.z);
                }
            }
            endPos = (Vector2)this.transform.parent.transform.localPosition;
        }
    }
}