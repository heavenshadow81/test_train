using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASClientCanvas : Canvas_
    {
        #region Properties
        private EASSocket _socket;
        public EASSocket socket
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

        public bool connected
        {
            get
            {
                return (_socket != null && _socket.connected);
            }
        }
        #endregion

        #region Private variables
        private EASPacket _currentPacket;
        #endregion

        #region Constructor
        public EASClientCanvas()
        {
            _MakeNewPacket();
        }
        #endregion

        #region Super methods
        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            if (connected)
            {
                base.Update();

                if (_currentPacket.data.Count > 0)
                {
                    _socket.Send(_currentPacket);
                    _MakeNewPacket();
                }
            }
        }

        protected override void _DrawForTouch(TouchInfo t, Vector3 pos)
        {
            base._DrawForTouch(t, pos);

            _WriteTouchInfoIntoPacket(t);
        }

        public override void ClearCanvas()
        {
            base.ClearCanvas();

            if (gameObject.activeInHierarchy)
                _WriteClearCanvasInfoIntoPacket();
        }

        public override void FillColor(Color color)
        {
            base.FillColor(color);

            if (gameObject.activeInHierarchy)
                _WriteClearCanvasInfoIntoPacket();
        }

        public override void ClearAlphaBuffer()
        {
            base.ClearAlphaBuffer();

            _WriteClearAlphaBufferInfoIntoPacket();
        }

        public override void ClearDualBrushBuffer()
        {
            base.ClearDualBrushBuffer();

            _WriteClearDualBrushBufferInfoIntoPacket();
        }
        #endregion

        private void _MakeNewPacket()
        {
            _currentPacket = new EASPacket();
            _currentPacket.type = "sketch";
        }

        private void _WriteTouchInfoIntoPacket(TouchInfo t)
        {
            Dictionary<string, object> touchInfoDict = new Dictionary<string, object>();

            // touch id
            touchInfoDict["touch_id"] = t.id;

            // convert screen space -> canvas space position
            Vector3 pos = ScreenToCanvas(t.position);

            // normalize position values (0 ~ 1)
            pos.x = pos.x / canvasSize.x;
            pos.y = pos.y / canvasSize.y;

            // write x, y
            touchInfoDict["x"] = pos.x;
            touchInfoDict["y"] = pos.y;

            // touch phase
            touchInfoDict["phase"] = t.phase.ToString().ToLower();

            // write to drawing info dictionary
            List<object> touchInfoDictList = _currentPacket.Get("data/drawing") as List<object>;
            if (touchInfoDictList == null)
            {
                touchInfoDictList = new List<object>();
                _currentPacket.Set("data/drawing", touchInfoDictList);
            }

            touchInfoDictList.Add(touchInfoDict);

            if (_currentPacket.Get("data/brush") == null)
                _WriteBrushInfoIntoPacket();
        }

        private void _WriteBrushInfoIntoPacket()
        {
            Dictionary<string, object> brushInfoDict = new Dictionary<string, object>();

            // brush name
            brushInfoDict["brush_name"] = brush.brushName;

            // diameter
            brushInfoDict["diameter"] = brush.diameter;

            // r, g, b, a
            brushInfoDict["r"] = brush.color.r;
            brushInfoDict["g"] = brush.color.g;
            brushInfoDict["b"] = brush.color.b;
            brushInfoDict["a"] = brush.color.a;

            // spacing
            brushInfoDict["spacing"] = brush.spacing;

            // hardness
            brushInfoDict["hardness"] = brush.hardness;

            // opacity
            brushInfoDict["opacity"] = brush.opacity;

            // flow
            brushInfoDict["flow"] = brush.flow;

            // angle
            brushInfoDict["angle"] = brush.angle;

            // blend mode
            brushInfoDict["blend_mode"] = (int)brush.blendMode;

            // mask type
            brushInfoDict["mask_type"] = (int)brush.maskType;

            // mask name
            brushInfoDict["mask_name"] = brush.maskName;

            // use mask color
            brushInfoDict["use_mask_color"] = brush.useMaskColor;

            // paint start position
            brushInfoDict["paint_start_pos"] = brush.paintStartPosition;

            // paint on drag
            brushInfoDict["paint_on_drag"] = brush.paintOnDrag;

            // use alpha buffer
            brushInfoDict["use_alpha_buffer"] = brush.useAlphaBuffer;

            // airbrush
            brushInfoDict["airbrush"] = brush.airbrush;

            // random seed
            brushInfoDict["random_seed"] = Random.seed;

            _currentPacket.Set("data/brush", brushInfoDict);

            _WriteBrushComponentsInfoIntoPacket();
        }

        private void _WriteBrushComponentsInfoIntoPacket()
        {
            BrushComponent[] components = new BrushComponent[] {
            brush.shapeDynamicComponent, brush.colorDynamicComponent, brush.scatteringComponent, brush.textureComponent, brush.transferComponent
        };

            List<object> list = new List<object>();

            for (int i = 0, cnt = components.Length; i < cnt; i++)
            {
                Dictionary<string, object> dict = _GetBrushComponentDefaultInfoDict(components[i]);
                switch (components[i].componentType)
                {
                    case ComponentType.ShapeDynamic:
                        {
                            BrushShapeDynamicComponent comp = (BrushShapeDynamicComponent)components[i];
                            if (comp.enable)
                            {
                                dict["size_jitter"] = comp.sizeJitter;
                                dict["minimum_diameter"] = comp.minimumDiameter;
                                dict["angle_jitter"] = comp.angleJitter;
                                dict["angle_control"] = (int)comp.angleControl;
                                dict["roundness_jitter"] = comp.roundnessJitter;
                                dict["minimum_roundness"] = comp.minimumRoundness;
                            }
                        }
                        break;
                    case ComponentType.ColorDynamic:
                        {
                            BrushColorDynamicComponent comp = (BrushColorDynamicComponent)components[i];
                            if (comp.enable)
                            {
                                dict["h_jitter"] = comp.hueJitter;
                                dict["s_jitter"] = comp.saturationJitter;
                                dict["b_jitter"] = comp.brightnessJitter;
                            }
                        }
                        break;
                    case ComponentType.Scattering:
                        {
                            BrushScatteringComponent comp = (BrushScatteringComponent)components[i];
                            if (comp.enable)
                            {
                                dict["scatter"] = comp.scatter;
                                dict["both_axes"] = comp.bothAxes;
                                dict["count"] = comp.count;
                                dict["count_jitter"] = comp.countJitter;
                            }
                        }
                        break;
                    case ComponentType.Texture:
                        {
                            BrushTextureComponent comp = (BrushTextureComponent)components[i];
                            if (comp.enable)
                            {
                                dict["texture_name"] = comp.textureName;
                                dict["scale"] = comp.scale;
                                dict["brightness"] = comp.brightness;
                                dict["contrast"] = comp.contrast;
                                dict["blend_mode"] = (int)comp.blendMode;
                                dict["depth"] = comp.depth;
                            }
                        }
                        break;
                    case ComponentType.Transfer:
                        {
                            BrushTransferComponent comp = (BrushTransferComponent)components[i];
                            if (comp.enable)
                            {
                                dict["opacity_jitter"] = comp.opacityJitter;
                                dict["flow_jitter"] = comp.flowJitter;
                            }
                        }
                        break;
                }

                list.Add(dict);
            }

            _currentPacket.Set("data/brush/components", list);
        }

        private Dictionary<string, object> _GetBrushComponentDefaultInfoDict(BrushComponent comp)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict["component_type"] = (int)comp.componentType;
            dict["enable"] = comp.enable;

            return dict;
        }

        private void _WriteClearCanvasInfoIntoPacket()
        {
            _currentPacket.Set("data/command/clear_canvas", true);
        }

        private void _WriteFillColorInfoIntoPacket(Color color)
        {
            // convert float format to byte format
            Color32 color32 = color;

            // fill
            _currentPacket.Set("data/command/fill_color", string.Format("{0:X}{1:X}{2:X}{4:X}", color32.r, color32.g, color32.b, color32.a));
        }

        private void _WriteClearDualBrushBufferInfoIntoPacket()
        {
            _currentPacket.Set("data/command/clear_dual_brush_buffer", true);
        }

        private void _WriteClearAlphaBufferInfoIntoPacket()
        {
            _currentPacket.Set("data/command/clear_alpha_buffer", true);
        }
    }
}