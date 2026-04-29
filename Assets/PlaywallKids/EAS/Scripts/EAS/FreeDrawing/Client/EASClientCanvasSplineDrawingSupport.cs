using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASClientCanvasSplineDrawingSupport : CanvasSplineDrawingSupport
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

        public bool connected
        {
            get
            {
                return socket != null && socket.connected;
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

        private Vector3 prevPosition = Vector3.zero;

        protected override bool _DrawForTouch(TouchInfo t, Vector3 canvasPos)
        {
            bool flag = base._DrawForTouch(t, canvasPos);

            if (connected)
            {
                Vector3 point = Vector3.zero;
                point.x = canvasPos.x / canvas.canvasSize.x;
                point.y = canvasPos.x / canvas.canvasSize.y;

                if ((prevPosition - point).sqrMagnitude >= 0.0002f || t.phase != TouchInfo.Phase.Move)
                {
                    prevPosition = point;

                    EASPacket packet = new EASPacket();
                    packet.type = type;

                    Dictionary<string, object> touchInfo = new Dictionary<string, object>();
                    touchInfo["touch_id"] = t.id;
                    touchInfo["x"] = canvasPos.x / canvas.canvasSize.x;
                    touchInfo["y"] = canvasPos.y / canvas.canvasSize.y;
                    touchInfo["phase"] = t.phase.ToString().ToLower();

                    List<object> splineDrawing = packet.GetList("data/spline_drawing");
                    if (splineDrawing == null)
                    {
                        splineDrawing = new List<object>();
                    }

                    splineDrawing.Add(touchInfo);
                    packet.Set("data/spline_drawing", splineDrawing);

                    socket.Send(packet);
                }

            }

            return flag;
        }
    }
}