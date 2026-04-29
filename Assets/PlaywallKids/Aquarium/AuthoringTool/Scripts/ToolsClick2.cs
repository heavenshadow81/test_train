using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.PlaywallKids.Aquarium
{
    using AuthoringTool;
    /// <summary>
    /// Simplified version of ToolsClick which excludes UI animations. Used in ATool_Fish.
    /// </summary>
    public class ToolsClick2 : MonoBehaviour
    {
        public UIPanel target;
        public Canvas_ canvas;
        public UserData.BrushTool tool = UserData.BrushTool.NONE;

        protected UserData userData;
        protected int nInstanceId = 0;
        
        void Start()
        {
            nInstanceId = target.GetInstanceID();
            userData = UserData.Instance();
            
        }

        private void OnEnable()
        {
            canvas.brush = canvas.brushSet.Get(BrushSet.kBrushNameMarker);
        }
        void OnClick()
        {
            if (userData.GetState(nInstanceId) == UserData.State.DRAW)
            {
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
    }
}