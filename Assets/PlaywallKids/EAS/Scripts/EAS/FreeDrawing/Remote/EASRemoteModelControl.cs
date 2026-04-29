using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASRemoteModelControl : EASClientModelControl
    {
        #region Private variables
        private List<Vector2> _parsedPaintedUVs = new List<Vector2>(64);
        #endregion

        new void Update()
        {

        }

        public void ProcessPacket(EASPacket packet)
        {
            if (packet != null)
            {
                if (packet.Get("data/brush") != null)
                {
                    Color newColor = brushColor;
                    newColor.r = packet.GetFloat("data/brush/r");
                    newColor.g = packet.GetFloat("data/brush/g");
                    newColor.b = packet.GetFloat("data/brush/b");
                    newColor.a = packet.GetFloat("data/brush/a");
                    brushColor = newColor;

                    if (packet.Get("data/brush/diameter") != null)
                    {
                        brushSize = packet.GetInt("data/brush/diameter");
                    }
                }

                if (packet.Get("data/command/model_rotation") != null)
                {
                    float modelRotation = packet.GetFloat("data/command/model_rotation");
                    Quaternion rotation = model.transform.localRotation;
                    Vector3 eulerAngles = rotation.eulerAngles;
                    eulerAngles.y = modelRotation;
                    rotation.eulerAngles = eulerAngles;
                    model.transform.localRotation = rotation;
                }

                if (packet.GetList("data/painting") != null)
                {
                    List<object> painting = packet.GetList("data/painting");
                    for (int i = 0; i < painting.Count; i++)
                    {
                        Dictionary<string, object> dict = painting[i] as Dictionary<string, object>;
                        if (dict != null)
                        {
                            TouchInfo touchInfo = new TouchInfo();
                            Template3D template = null;
                            string templateName = "";
                            Vector3 uv = Vector3.zero;

                            if (dict.ContainsKey("touch_id") && dict["touch_id"] != null)
                            {
                                touchInfo.id = int.Parse(dict["touch_id"].ToString());
                            }
                            if (dict.ContainsKey("template_name") && dict["template_name"] != null)
                            {
                                templateName = dict["template_name"].ToString();
                                if (!string.IsNullOrEmpty(templateName))
                                {
                                    if (templates.Count == 0 && model != null)
                                    {
                                        templates = new List<Template3D>(model.GetComponentsInChildren<Template3D>());
                                    }

                                    template = templates.Find((obj) =>
                                    {
                                        return obj.name.Equals(templateName);
                                    });
                                }
                            }
                            if (dict.ContainsKey("u") && dict["u"] != null)
                            {
                                uv.x = float.Parse(dict["u"].ToString());
                            }
                            if (dict.ContainsKey("v") && dict["v"] != null)
                            {
                                uv.y = float.Parse(dict["v"].ToString());
                            }
                            if (dict.ContainsKey("phase") && dict["phase"] != null)
                            {
                                switch (dict["phase"].ToString())
                                {
                                    case "begin":
                                        touchInfo.phase = TouchInfo.Phase.Begin;
                                        break;
                                    case "move":
                                        touchInfo.phase = TouchInfo.Phase.Move;
                                        break;
                                    case "stay":
                                        touchInfo.phase = TouchInfo.Phase.Stay;
                                        break;
                                    case "end":
                                        touchInfo.phase = TouchInfo.Phase.End;
                                        break;
                                    case "cancel":
                                        touchInfo.phase = TouchInfo.Phase.Cancel;
                                        break;
                                }
                            }

                            _parsedPaintedUVs.Clear();

                            if (dict.ContainsKey("uvs") && dict["uvs"] != null)
                            {
                                List<object> uvs = dict["uvs"] as List<object>;
                                if (uvs != null)
                                {
                                    for (int j = 0, cnt = uvs.Count; j < cnt; j += 2)
                                    {
                                        float u = 0.0f, v = 0.0f;

                                        float.TryParse(uvs[j].ToString(), out u);
                                        float.TryParse(uvs[j + 1].ToString(), out v);

                                        _parsedPaintedUVs.Add(new Vector2(u, v));
                                    }
                                }
                            }

                            _Draw(touchInfo, _parsedPaintedUVs, template);
                        }
                    }
                }
            }
        }

        protected virtual void _Draw(TouchInfo touchInfo, List<Vector2> uvs, Template3D template)
        {
            if (template != null)
            {
                if (!template.painting)
                {
                    template.BeginPaint();
                }

                template.PaintUV(uvs, 0, brushSize, brushColor);

                template.FlushPaint();

                if (touchInfo.phase == TouchInfo.Phase.End ||
                    touchInfo.phase == TouchInfo.Phase.Cancel)
                {
                    template.EndPaint();
                }
            }
        }
    }
}