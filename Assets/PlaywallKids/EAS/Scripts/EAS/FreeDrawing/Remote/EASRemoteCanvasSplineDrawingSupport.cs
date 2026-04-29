using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASRemoteCanvasSplineDrawingSupport : CanvasSplineDrawingSupport
    {
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

        private string _type = EASPacket.kType3D;
        public string type
        {
            get
            {
                return _type;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _type = value;
                }
            }
        }

        public bool connected
        {
            get
            {
                return _socket != null && _socket.connected;
            }
        }

        new void Update()
        {
        }

        public void ProcessPacket(EASPacket packet)
        {
            if (packet != null)
            {
                if (packet.Get("data/spline_drawing") != null)
                {
                    List<object> drawing = packet.GetList("data/spline_drawing");

                    foreach (object obj in drawing)
                    {
                        Dictionary<string, object> dict = obj as Dictionary<string, object>;
                        if (dict != null)
                        {
                            TouchInfo t = new TouchInfo();
                            Vector3 pos = Vector3.zero;

                            if (dict.ContainsKey("touch_id") && dict["touch_id"] != null)
                            {
                                t.id = int.Parse(dict["touch_id"].ToString());
                            }
                            if (dict.ContainsKey("x") && dict["x"] != null)
                            {
                                pos.x = float.Parse(dict["x"].ToString());
                            }
                            if (dict.ContainsKey("y") && dict["y"] != null)
                            {
                                pos.y = float.Parse(dict["y"].ToString());
                            }
                            if (dict.ContainsKey("phase") && dict["phase"] != null)
                            {
                                switch (dict["phase"].ToString())
                                {
                                    case "begin":
                                        t.phase = TouchInfo.Phase.Begin;
                                        break;
                                    case "move":
                                        t.phase = TouchInfo.Phase.Move;
                                        break;
                                    case "stay":
                                        t.phase = TouchInfo.Phase.Stay;
                                        break;
                                    case "end":
                                        t.phase = TouchInfo.Phase.End;
                                        break;
                                    case "cancel":
                                        t.phase = TouchInfo.Phase.Cancel;
                                        break;
                                }
                            }

                            // convert normalized point to canvas space point.
                            pos.x = pos.x * canvas.canvasSize.x;
                            pos.y = pos.y * canvas.canvasSize.y;

                            //pos.x = pos.x * (Screen.height * camera.aspectRatio) + 0.5f * (Screen.width - Screen.height * camera.aspectRatio);
                            //pos.y = pos.y * (float)Screen.height;

                            t.position = pos;

                            // Draw spline for touch
                            if (_DrawForTouch(t, pos))
                            {

                                // draws canvas too
                                canvas.DrawForTouch(t, pos);
                                canvas.Flush();

                                // if drawing has been ended, generates the spline.
                                if (t.phase == TouchInfo.Phase.End)
                                {
                                    GenerateSpline();
                                }
                            }
                            else
                            {
                                GenerateSpline();
                                canvas.ClearCanvas();
                            }
                        }
                    }
                }
            }
        }
    }
}