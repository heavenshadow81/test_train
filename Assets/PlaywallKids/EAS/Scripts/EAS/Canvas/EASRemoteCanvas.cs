using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASRemoteCanvas : Canvas_
    {
        #region Properties
        private EASSocket _socket;
        public virtual EASSocket socket
        {
            get
            {
                return _socket;
            }
            set
            {
                _socket = value;
            }
        }

        public virtual bool connected
        {
            get
            {
                return _socket != null && _socket.connected;
            }
        }
        #endregion

        public override void Update()
        {
            if (wantsPaint && connected)
            {
                while (true)
                {
                    EASPacket packet = socket.Receive(EASPacket.kTypeSketch);
                    if (packet != null)
                    {
                        Dictionary<string, object> dict = packet.data;

                        _ParseDataAndExecute(dict);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _socket = null;
        }

        #region Parsing datas
        protected void _ParseDataAndExecute(Dictionary<string, object> dict)
        {
            if (dict != null)
            {
                _ParseCommandsAndExecute(dict);
                _ParseBrushInfoAndExecute(dict);
                _ParseDrawingInfoAndExecute(dict);
            }
        }

        protected void _ParseCommandsAndExecute(Dictionary<string, object> dict)
        {
            if (dict.ContainsKey("command"))
            {
                Dictionary<string, object> commandDict = dict["command"] as Dictionary<string, object>;
                if (commandDict != null)
                {
                    // Clear Canvas
                    if (commandDict.ContainsKey("clear_canvas") && commandDict["clear_canvas"] != null)
                    {
                        bool clearCanvas = false;
                        bool.TryParse(commandDict["clear_canvas"].ToString(), out clearCanvas);

                        if (clearCanvas)
                        {
                            ClearCanvas();
                        }
                    }

                    // Fill Color
                    if (commandDict.ContainsKey("fill_color") && commandDict["fill_color"] != null)
                    {
                        string colorStr = commandDict["fill_color"].ToString();

                        // remove #
                        if (colorStr[0] == '#')
                        {
                            colorStr = colorStr.Remove(0, 1);
                        }

                        // parse components (r, g, b, a)
                        Color color = Color.clear;

                        // fill with one component (grayscale)
                        if (colorStr.Length < 3)
                        {
                            byte value = 0;
                            byte.TryParse(colorStr, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out value);
                            color.r = color.g = color.a = (float)value / 255.0f;
                            color.a = 1.0f;
                        }
                        // multi components
                        else
                        {
                            byte r = 0, g = 0, b = 0, a = 255;
                            byte.TryParse(colorStr.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out r);
                            if (colorStr.Length >= 4)
                            {
                                byte.TryParse(colorStr.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out g);
                            }
                            if (colorStr.Length >= 6)
                            {
                                byte.TryParse(colorStr.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out b);
                            }
                            if (colorStr.Length >= 8)
                            {
                                byte.TryParse(colorStr.Substring(6, 2), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out a);
                            }
                            color.r = (float)r / 255.0f;
                            color.g = (float)g / 255.0f;
                            color.b = (float)b / 255.0f;
                            color.a = (float)a / 255.0f;
                        }

                        FillColor(color);
                    }

                    // Clear Alpha Buffer
                    if (commandDict.ContainsKey("clear_alpha_buffer") && commandDict["clear_alpha_buffer"] != null)
                    {
                        bool clearAlphaBuffer = false;
                        bool.TryParse(commandDict["clear_alpha_buffer"].ToString(), out clearAlphaBuffer);

                        if (clearAlphaBuffer)
                        {
                            ClearAlphaBuffer();
                        }
                    }

                    // Clear Dual Brush Buffer
                    if (commandDict.ContainsKey("clear_dual_brush_buffer") && commandDict["clear_dual_brush_buffer"] != null)
                    {
                        bool clearDualBrushBuffer = false;
                        bool.TryParse(commandDict["clear_dual_brush_buffer"].ToString(), out clearDualBrushBuffer);

                        if (clearDualBrushBuffer)
                        {
                            ClearDualBrushBuffer();
                        }
                    }
                }
            }
        }

        protected void _ParseDrawingInfoAndExecute(Dictionary<string, object> dict)
        {
            if (dict.ContainsKey("drawing"))
            {
                List<object> list = dict["drawing"] as List<object>;
                if (list != null)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        Dictionary<string, object> touchInfoDict = list[j] as Dictionary<string, object>;

                        if (touchInfoDict == null) continue;

                        int touchId = 0;
                        float x = 0, y = 0;
                        TouchInfo.Phase phase = TouchInfo.Phase.Begin;

                        TouchInfo touchInfo = new TouchInfo();
                        if (touchInfoDict.ContainsKey("touch_id") && touchInfoDict["touch_id"] != null)
                        {
                            int.TryParse(touchInfoDict["touch_id"].ToString(), out touchId);
                        }
                        if (touchInfoDict.ContainsKey("x") && touchInfoDict["x"] != null)
                        {
                            float.TryParse(touchInfoDict["x"].ToString(), out x);
                        }
                        if (touchInfoDict.ContainsKey("y") && touchInfoDict["y"] != null)
                        {
                            float.TryParse(touchInfoDict["y"].ToString(), out y);
                        }
                        if (touchInfoDict.ContainsKey("phase") && touchInfoDict["phase"] != null)
                        {
                            string phaseStr = touchInfoDict["phase"].ToString();
                            switch (phaseStr)
                            {
                                case "begin":
                                    phase = TouchInfo.Phase.Begin;
                                    break;
                                case "move":
                                    phase = TouchInfo.Phase.Move;
                                    break;
                                case "stay":
                                    phase = TouchInfo.Phase.Stay;
                                    break;
                                case "cancel":
                                    phase = TouchInfo.Phase.Cancel;
                                    break;
                                case "end":
                                    phase = TouchInfo.Phase.End;
                                    break;
                            }
                        }

                        touchInfo.id = touchId;
                        touchInfo.phase = phase;

                        Vector3 pos = new Vector3(x * canvasSize.x, y * canvasSize.y);

                        _DrawForTouch(touchInfo, pos);
                    }

                    Flush();
                }
            }
        }

        protected void _ParseBrushInfoAndExecute(Dictionary<string, object> dict)
        {
            if (dict.ContainsKey("brush"))
            {
                Dictionary<string, object> brushInfoDict = dict["brush"] as Dictionary<string, object>;

                Brush b = brush;

                string brushName = b.brushName;
                float diameter = 4.0f;
                Color color = Color.white;
                float spacing = brush.spacing;
                float hardness = brush.hardness;
                float opacity = brush.opacity;
                float flow = brush.flow;
                float angle = brush.angle;
                BlendMode blendMode = brush.blendMode;
                MaskType maskType = b.maskType;
                string maskName = b.maskName;
                bool useMaskColor = b.useMaskColor;
                bool paintStartPos = b.paintStartPosition;
                bool paintOnDrag = b.paintOnDrag;
                bool useAlphaBuffer = b.useAlphaBuffer;
                bool airbrush = b.airbrush;

                if (brushInfoDict.ContainsKey("brush_name") && brushInfoDict["brush_name"] != null)
                {
                    brushName = brushInfoDict["brush_name"].ToString();
                }
                if (brushInfoDict.ContainsKey("diameter") && brushInfoDict["diameter"] != null)
                {
                    float.TryParse(brushInfoDict["diameter"].ToString(), out diameter);
                }
                if (brushInfoDict.ContainsKey("r") && brushInfoDict["r"] != null)
                {
                    float.TryParse(brushInfoDict["r"].ToString(), out color.r);
                }
                if (brushInfoDict.ContainsKey("g") && brushInfoDict["g"] != null)
                {
                    float.TryParse(brushInfoDict["g"].ToString(), out color.g);
                }
                if (brushInfoDict.ContainsKey("b") && brushInfoDict["b"] != null)
                {
                    float.TryParse(brushInfoDict["b"].ToString(), out color.b);
                }
                if (brushInfoDict.ContainsKey("a") && brushInfoDict["a"] != null)
                {
                    float.TryParse(brushInfoDict["a"].ToString(), out color.a);
                }

                if (brushInfoDict.ContainsKey("spacing") && brushInfoDict["spacing"] != null)
                {
                    float.TryParse(brushInfoDict["spacing"].ToString(), out spacing);
                }
                if (brushInfoDict.ContainsKey("hardness") && brushInfoDict["hardness"] != null)
                {
                    float.TryParse(brushInfoDict["hardness"].ToString(), out hardness);
                }
                if (brushInfoDict.ContainsKey("opacity") && brushInfoDict["opacity"] != null)
                {
                    float.TryParse(brushInfoDict["opacity"].ToString(), out opacity);
                }
                if (brushInfoDict.ContainsKey("flow") && brushInfoDict["flow"] != null)
                {
                    float.TryParse(brushInfoDict["flow"].ToString(), out flow);
                }
                if (brushInfoDict.ContainsKey("angle") && brushInfoDict["angle"] != null)
                {
                    float.TryParse(brushInfoDict["angle"].ToString(), out angle);
                }
                if (brushInfoDict.ContainsKey("blend_mode") && brushInfoDict["blend_mode"] != null)
                {
                    int val = (int)blendMode;
                    int.TryParse(brushInfoDict["blend_mode"].ToString(), out val);
                    blendMode = (BlendMode)val;
                }

                if (brushInfoDict.ContainsKey("mask_type") && brushInfoDict["mask_type"] != null)
                {
                    int val = (int)maskType;
                    int.TryParse(brushInfoDict["mask_type"].ToString(), out val);
                    maskType = (MaskType)val;
                }
                if (brushInfoDict.ContainsKey("mask_name") && brushInfoDict["mask_name"] != null)
                {
                    maskName = brushInfoDict["mask_name"].ToString();
                }
                if (brushInfoDict.ContainsKey("use_mask_color") && brushInfoDict["use_mask_color"] != null)
                {
                    bool.TryParse(brushInfoDict["use_mask_color"].ToString(), out useMaskColor);
                }
                if (brushInfoDict.ContainsKey("paint_start_pos") && brushInfoDict["paint_start_pos"] != null)
                {
                    bool.TryParse(brushInfoDict["paint_start_pos"].ToString(), out paintStartPos);
                }
                if (brushInfoDict.ContainsKey("paint_on_drag") && brushInfoDict["paint_on_drag"] != null)
                {
                    bool.TryParse(brushInfoDict["paint_on_drag"].ToString(), out paintOnDrag);
                }
                if (brushInfoDict.ContainsKey("use_alpha_buffer") && brushInfoDict["use_alpha_buffer"] != null)
                {
                    bool.TryParse(brushInfoDict["use_alpha_buffer"].ToString(), out useAlphaBuffer);
                }
                if (brushInfoDict.ContainsKey("airbrush") && brushInfoDict["airbrush"] != null)
                {
                    bool.TryParse(brushInfoDict["airbrush"].ToString(), out airbrush);
                }

                bool changeBrush = false;
                if (!b.brushName.Equals(brushName))
                {
                    changeBrush = true;
                    Brush newBrush = brushSet.Get(brushName);
                    if (newBrush != null)
                    {
                        b = newBrush;
                    }

                    b.diameter = diameter;
                    b.color = color;
                }
                else if (!diameter.Equals(b.diameter))
                {
                    changeBrush = true;
                    b.diameter = diameter;
                    b.color = color;
                }
                else
                {
                    b.color = color;
                }

                b.spacing = spacing;
                b.hardness = hardness;
                b.opacity = opacity;
                b.flow = flow;
                b.angle = angle;
                b.blendMode = blendMode;
                b.maskType = maskType;
                b.maskName = maskName;
                b.useMaskColor = useMaskColor;
                b.paintStartPosition = paintStartPos;
                b.paintOnDrag = paintOnDrag;
                b.useAlphaBuffer = useAlphaBuffer;
                b.airbrush = airbrush;

                // parse components
                _ParseBrushComponentsInfoAndExecute(dict, b);

                // change brush if needed
                if (changeBrush)
                {
                    brush = b;
                }

                // check random seed
                if (brushInfoDict.ContainsKey("random_seed") && brushInfoDict["random_seed"] != null)
                {
                    int seed = Random.seed;
                    int.TryParse(brushInfoDict["random_seed"].ToString(), out seed);
                    Random.seed = seed;
                }
            }
        }

        protected void _ParseBrushComponentsInfoAndExecute(Dictionary<string, object> dict, Brush b)
        {
            if (dict.ContainsKey("brush"))
            {
                Dictionary<string, object> brushInfoDict = dict["brush"] as Dictionary<string, object>;
                if (brushInfoDict.ContainsKey("components"))
                {
                    List<object> components = brushInfoDict["components"] as List<object>;
                    if (components != null)
                    {
                        for (int i = 0; i < components.Count; i++)
                        {
                            Dictionary<string, object> compDict = components[i] as Dictionary<string, object>;
                            if (compDict != null)
                            {
                                ComponentType componentType = (ComponentType)int.Parse(compDict["component_type"].ToString());
                                bool enable = bool.Parse(compDict["enable"].ToString());

                                switch (componentType)
                                {
                                    case ComponentType.ShapeDynamic:
                                        {
                                            b.shapeDynamicComponent.enable = enable;

                                            if (enable)
                                            {
                                                if (compDict.ContainsKey("size_jitter") && compDict["size_jitter"] != null)
                                                {
                                                    float val = b.shapeDynamicComponent.sizeJitter;
                                                    float.TryParse(compDict["size_jitter"].ToString(), out val);
                                                    b.shapeDynamicComponent.sizeJitter = val;
                                                }
                                                if (compDict.ContainsKey("minimum_diameter") && compDict["minimum_diameter"] != null)
                                                {
                                                    float val = b.shapeDynamicComponent.minimumDiameter;
                                                    float.TryParse(compDict["minimum_diameter"].ToString(), out val);
                                                    b.shapeDynamicComponent.minimumDiameter = val;
                                                }
                                                if (compDict.ContainsKey("angle_jitter") && compDict["angle_jitter"] != null)
                                                {
                                                    float val = b.shapeDynamicComponent.angleJitter;
                                                    float.TryParse(compDict["angle_jitter"].ToString(), out val);
                                                    b.shapeDynamicComponent.angleJitter = val;
                                                }
                                                if (compDict.ContainsKey("angle_control") && compDict["angle_control"] != null)
                                                {
                                                    int val = (int)b.shapeDynamicComponent.angleControl;
                                                    int.TryParse(compDict["angle_control"].ToString(), out val);
                                                    b.shapeDynamicComponent.angleControl = (AngleControl)val;
                                                }
                                                if (compDict.ContainsKey("roundness_jitter") && compDict["roundness_jitter"] != null)
                                                {
                                                    float val = b.shapeDynamicComponent.roundnessJitter;
                                                    float.TryParse(compDict["roundness_jitter"].ToString(), out val);
                                                    b.shapeDynamicComponent.roundnessJitter = val;
                                                }
                                                if (compDict.ContainsKey("minimum_roundness") && compDict["minimum_roundness"] != null)
                                                {
                                                    float val = b.shapeDynamicComponent.minimumRoundness;
                                                    float.TryParse(compDict["minimum_roundness"].ToString(), out val);
                                                    b.shapeDynamicComponent.minimumRoundness = val;
                                                }
                                            }
                                        }
                                        break;
                                    case ComponentType.ColorDynamic:
                                        {
                                            b.colorDynamicComponent.enable = enable;

                                            if (enable)
                                            {
                                                if (compDict.ContainsKey("h_jitter") && compDict["h_jitter"] != null)
                                                {
                                                    float val = b.colorDynamicComponent.hueJitter;
                                                    float.TryParse(compDict["h_jitter"].ToString(), out val);
                                                    b.colorDynamicComponent.hueJitter = val;
                                                }
                                                if (compDict.ContainsKey("s_jitter") && compDict["s_jitter"] != null)
                                                {
                                                    float val = b.colorDynamicComponent.saturationJitter;
                                                    float.TryParse(compDict["s_jitter"].ToString(), out val);
                                                    b.colorDynamicComponent.saturationJitter = val;
                                                }
                                                if (compDict.ContainsKey("b_jitter") && compDict["b_jitter"] != null)
                                                {
                                                    float val = b.colorDynamicComponent.brightnessJitter;
                                                    float.TryParse(compDict["b_jitter"].ToString(), out val);
                                                    b.colorDynamicComponent.brightnessJitter = val;
                                                }
                                            }
                                        }
                                        break;
                                    case ComponentType.Scattering:
                                        {
                                            b.scatteringComponent.enable = enable;

                                            if (enable)
                                            {
                                                if (compDict.ContainsKey("scatter") && compDict["scatter"] != null)
                                                {
                                                    float val = b.scatteringComponent.scatter;
                                                    float.TryParse(compDict["scatter"].ToString(), out val);
                                                    b.scatteringComponent.scatter = val;
                                                }
                                                if (compDict.ContainsKey("both_axes") && compDict["both_axes"] != null)
                                                {
                                                    bool val = b.scatteringComponent.bothAxes;
                                                    bool.TryParse(compDict["both_axes"].ToString(), out val);
                                                    b.scatteringComponent.bothAxes = val;
                                                }
                                                if (compDict.ContainsKey("count") && compDict["count"] != null)
                                                {
                                                    int val = b.scatteringComponent.count;
                                                    int.TryParse(compDict["count"].ToString(), out val);
                                                    b.scatteringComponent.count = val;
                                                }
                                                if (compDict.ContainsKey("count_jitter") && compDict["count_jitter"] != null)
                                                {
                                                    float val = b.scatteringComponent.countJitter;
                                                    float.TryParse(compDict["count_jitter"].ToString(), out val);
                                                    b.scatteringComponent.countJitter = val;
                                                }
                                            }
                                        }
                                        break;
                                    case ComponentType.Texture:
                                        {
                                            b.textureComponent.enable = enable;

                                            if (enable)
                                            {
                                                if (compDict.ContainsKey("texture_name") && compDict["texture_name"] != null)
                                                    b.textureComponent.textureName = compDict["texture_name"].ToString();
                                                if (compDict.ContainsKey("scale") && compDict["scale"] != null)
                                                {
                                                    float val = b.textureComponent.scale;
                                                    float.TryParse(compDict["scale"].ToString(), out val);
                                                    b.textureComponent.scale = val;
                                                }
                                                if (compDict.ContainsKey("brightness") && compDict["brightness"] != null)
                                                {
                                                    float val = b.textureComponent.brightness;
                                                    float.TryParse(compDict["brightness"].ToString(), out val);
                                                    b.textureComponent.brightness = val;
                                                }
                                                if (compDict.ContainsKey("contrast") && compDict["contrast"] != null)
                                                {
                                                    float val = b.textureComponent.contrast;
                                                    float.TryParse(compDict["contrast"].ToString(), out val);
                                                    b.textureComponent.contrast = val;
                                                }
                                                if (compDict.ContainsKey("blend_mode") && compDict["blend_mode"] != null)
                                                {
                                                    int val = (int)b.textureComponent.blendMode;
                                                    int.TryParse(compDict["blend_mode"].ToString(), out val);
                                                    b.textureComponent.blendMode = (BlendMode)val;
                                                }
                                                if (compDict.ContainsKey("depth") && compDict["depth"] != null)
                                                {
                                                    float val = b.textureComponent.depth;
                                                    float.TryParse(compDict["depth"].ToString(), out val);
                                                    b.textureComponent.depth = val;
                                                }
                                            }
                                        }
                                        break;
                                    case ComponentType.Transfer:
                                        {
                                            b.transferComponent.enable = enable;

                                            if (enable)
                                            {
                                                if (compDict.ContainsKey("opacity_jitter") && compDict["opacity_jitter"] != null)
                                                {
                                                    float val = b.transferComponent.opacityJitter;
                                                    float.TryParse(compDict["opacity_jitter"].ToString(), out val);
                                                    b.transferComponent.opacityJitter = val;
                                                }
                                                if (compDict.ContainsKey("flow_jitter") && compDict["flow_jitter"] != null)
                                                {
                                                    float val = b.transferComponent.flowJitter;
                                                    float.TryParse(compDict["flow_jitter"].ToString(), out val);
                                                    b.transferComponent.flowJitter = val;
                                                }
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}