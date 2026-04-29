using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASClientModelControl : SimpleModelControl
    {
        #region Properties
        public string type { get; set; }

        public EASSocket socket
        {
            get;
            set;
        }

        public bool connected
        {
            get
            {
                return socket != null && socket.connected;
            }
        }
        #endregion

        #region Private variables
        private EASPacket _currentPacket;

        private List<object> _cachedUVBufferForPacket = new List<object>(64);

        private Dictionary<string, bool> _templatePaintingStateDict = new Dictionary<string, bool>();
        #endregion

        #region Constructor
        public EASClientModelControl()
        {
            _MakeNewPacket();
        }
        #endregion

        #region Unity Methods
        public void Update()
        {
            if (model == null) return;

            if (wantsPaint && rotate)
            {
                TCCamera.sharedInstance.RequestRefreshTCRT();
            }

            if (connected && model != null)
            {
                _MakeNewPacket();

                _WriteRotationInfoIntoPacket();

                if (wantsPaint)
                {
                    if (CustomInput.touchCount > 0)
                    {
                        for (int i = 0, cnt = CustomInput.touchCount; i < cnt; i++)
                        {
                            TouchInfo touch = CustomInput.GetTouch(i);

                            _DrawForTouch(touch);
                        }
                    }
                    else
                    {
                        foreach (Template3D template in templates)
                        {
                            if (template.painting) template.EndPaint();
                        }
                    }
                }

                if (_currentPacket.data.Count > 0)
                {
                    socket.Send(_currentPacket);
                }
            }
        }

        #endregion

        #region Making and Writing Packets
        private void _MakeNewPacket()
        {
            _currentPacket = new EASPacket();
            _currentPacket.type = type;
        }

        private void _WriteRotationInfoIntoPacket()
        {
            if (rotate)
            {
                _currentPacket.Set("data/command/model_rotation", model.transform.localRotation.eulerAngles.y);
            }
        }

        private void _WriteTouchInfoIntoPacket(TouchInfo touchInfo, Template3D template)
        {
            TCCamera tcCam = TCCamera.sharedInstance;

            Vector3 uv = tcCam.GetTCRTPixelInfo(tcCam.ScreenToTCRT(touchInfo.position)).uv;

            // check all parameters is valid
            if (template == null) return;

            // new touch info dict
            Dictionary<string, object> touchInfoDict = new Dictionary<string, object>();

            // touch id
            touchInfoDict["touch_id"] = touchInfo.id;

            // template id
            touchInfoDict["template_name"] = template.name;

            // UV
            touchInfoDict["u"] = System.Math.Round(uv.x, 4);
            touchInfoDict["v"] = System.Math.Round(uv.y, 4);

            // UV List
            if (_cachedUVBufferForPacket.Capacity < _paintedUVs.Capacity)
                _cachedUVBufferForPacket.Capacity *= 2;

            _cachedUVBufferForPacket.Clear();
            for (int i = 0, cnt = _paintedUVs.Count; i < cnt; i++)
            {
                double u = System.Math.Round(_paintedUVs[i].x, 5);
                double v = System.Math.Round(_paintedUVs[i].y, 5);

                if (_cachedUVBufferForPacket.Count < 2 ||
                    System.Math.Abs((double)_cachedUVBufferForPacket[_cachedUVBufferForPacket.Count - 2] - u) > 0.0f ||
                    System.Math.Abs((double)_cachedUVBufferForPacket[_cachedUVBufferForPacket.Count - 1] - v) > 0.0f)
                {
                    _cachedUVBufferForPacket.Add(u);
                    _cachedUVBufferForPacket.Add(v);
                }
            }
            touchInfoDict["uvs"] = new List<object>(_cachedUVBufferForPacket);

            // touch phase
            touchInfoDict["phase"] = touchInfo.phase.ToString().ToLower();

            // write to drawing info dictionary
            List<object> touchInfoDictList = _currentPacket.Get("data/painting") as List<object>;
            if (touchInfoDictList == null)
            {
                touchInfoDictList = new List<object>();
                _currentPacket.Set("data/painting", touchInfoDictList);
            }

            if (_cachedUVBufferForPacket.Count > 0)
            {
                touchInfoDictList.Add(touchInfoDict);

                _templatePaintingStateDict[template.name] = true;
            }
            else if (touchInfo.phase == TouchInfo.Phase.End || touchInfo.phase == TouchInfo.Phase.Cancel)
            {
                if (_templatePaintingStateDict.ContainsKey(template.name) && _templatePaintingStateDict[template.name])
                    touchInfoDictList.Add(touchInfoDict);

                _templatePaintingStateDict[template.name] = false;
            }

            _WriteBrushInfoIntoPacket();
        }

        private void _WriteBrushInfoIntoPacket()
        {
            Dictionary<string, object> brushInfoDict = new Dictionary<string, object>();

            // brush name
            brushInfoDict["brush_name"] = BrushSet.kBrushNameMarker;

            // diameter
            brushInfoDict["diameter"] = brushSize;

            // r, g, b, a
            brushInfoDict["r"] = brushColor.r;
            brushInfoDict["g"] = brushColor.g;
            brushInfoDict["b"] = brushColor.b;
            brushInfoDict["a"] = brushColor.a;

            _currentPacket.Set("data/brush", brushInfoDict);
        }
        #endregion

        protected override void _Draw(TouchInfo touchInfo, Template3D template)
        {
            base._Draw(touchInfo, template);

            _WriteTouchInfoIntoPacket(touchInfo, template);
        }
    }
}