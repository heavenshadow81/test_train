using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;
    public class EASAToolRemotePetFreeDrawingPanel : EASAToolPetFreeDrawingPanelBase
    {
        #region Properties
        private System.WeakReference _mainPanel = null;
        public EASAToolRemotePetPanel mainPanel
        {
            get
            {
                if (_mainPanel == null)
                {
                    return null;
                }
                return _mainPanel.Target as EASAToolRemotePetPanel;
            }
            set
            {
                if (_mainPanel == null)
                {
                    _mainPanel = new System.WeakReference(value);
                }
                else
                {
                    _mainPanel.Target = value;
                }
            }
        }

        private System.WeakReference _modelControl = null;
        public EASRemoteModelControl modelControl
        {
            get
            {
                if (_modelControl == null)
                {
                    return null;
                }
                return _modelControl.Target as EASRemoteModelControl;
            }
            set
            {
                if (_modelControl == null)
                {
                    _modelControl = new System.WeakReference(value);
                }
                else
                {
                    _modelControl.Target = value;
                }
            }
        }
        #endregion

        public override void BeginShow()
        {
            base.BeginShow();

            EASRemoteCanvasSplineDrawingSupport c = canvasSplineDrawingSupport as EASRemoteCanvasSplineDrawingSupport;
            c.socket = socket;
            c.type = EASPacket.kTypePet;

            modelControl.socket = socket;

            ClearModel();
        }

        public void ProcessPacket(EASPacket packet)
        {
            if (packet != null)
            {
                if (packet.GetBool("data/command/next_step"))
                {
                    if (template == null)
                    {
                        Triangulate();
                        if (template != null)
                        {
                            modelControl.model = template.gameObject;
                        }

                        EASPacket response = new EASPacket();
                        response.type = EASPacket.kTypePet;
                        response.Set("data/command/next_step", template != null);
                        socket.Send(response);
                    }
                    else
                    {
                        // release the reference.
                        GameObject go = modelControl.model;
                        modelControl.model = null;
                        go.transform.parent = EASServerMapManager.sharedInstance.dragonPark.transform;

                        // set layer as MainScene
                        go.SetLayerRecursively("MainScene");

                        // initialize fruit
                        DragonFruit fruit = template.gameObject.AddComponent<DragonFruit>();
                        fruit.userId = mainPanel.userId;
                        fruit.dragon = SimpleInstantiatedTemplateControl.GetCurrentTemplate(mainPanel.userId).GetComponent<Dragon>();
                        fruit.name = string.Format("Fruit({0}->{1})", selectedItemName, fruit.dragon.name);
                        fruit.poopSound = poopSound;

                        // send response packet
                        EASPacket response = new EASPacket();
                        response.type = EASPacket.kTypePet;
                        response.Set("data/command/next_step", true);
                        socket.Send(response);

                        // call main panel's next step
                        mainPanel.NextStep();
                    }
                }
                else if (packet.Get("data/fruit") != null)
                {
                    int idx = packet.GetInt("data/fruit");
                    SelectItem(idx);
                }
                else if (template == null)
                {
                    ((EASRemoteCanvasSplineDrawingSupport)canvasSplineDrawingSupport).ProcessPacket(packet);
                }
                else
                {
                    modelControl.ProcessPacket(packet);
                    _ShowBrushColor();
                    brushSizeSlider.value = (modelControl.brushSize - 4.0f) / 28.0f;
                }
            }
        }

        private void _ShowBrushColor()
        {
            if (modelControl != null)
            {
                if (brushSizeSmallSprite != null)
                {
                    brushSizeSmallSprite.color = modelControl.brushColor;
                }

                if (brushSizeBigSprite != null)
                {
                    brushSizeBigSprite.color = modelControl.brushColor;
                }
            }
        }
    }
}