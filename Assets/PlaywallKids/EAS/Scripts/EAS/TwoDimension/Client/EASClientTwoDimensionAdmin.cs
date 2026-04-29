using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.EAS
{
    using TwoDimensionDrawScene;

    public class EASClientTwoDimensionAdmin : TwoDimensionAdmin
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
                return _socket != null && _socket.connected;
            }
        }

        protected override string prefabPath
        {
            get
            {
                return System.IO.Path.Combine(base.prefabPath, "EAS").Replace("\\", "/");
            }
        }


        protected override TwoDimensionTable cModeFile
        {
            get
            {
                if (_cModeFile == null)
                {
                    _cModeFile = new TwoDimensionTable_EASClient();
                }
                return _cModeFile;
            }
        }
        #endregion

        #region Private variables
        private int _uiRootPrevManualWidth;
        private int _uiRootPrevManualHeight;
        private bool _uiRootPrevFitWidth, _uiRootPrevFitHeight;
        #endregion

        #region Methods
        protected override void OnEnable()
        {
            base.OnEnable();

            // 2d admin is optimized at resolution 3840x1440
            // so that we need to modify the ui root settings
            UIRoot root = UIRoot.list[0];
            _uiRootPrevManualWidth = root.manualWidth;
            _uiRootPrevManualHeight = root.manualHeight;
            _uiRootPrevFitWidth = root.fitWidth;
            _uiRootPrevFitHeight = root.fitHeight;

            root.manualWidth = 3840; root.manualHeight = 1440;
            root.fitWidth = false; root.fitHeight = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // Revert ui root settings
            UIRoot root = UIRoot.list[0];

            root.manualWidth = _uiRootPrevManualWidth; root.manualHeight = _uiRootPrevManualHeight;
            root.fitWidth = _uiRootPrevFitWidth; root.fitHeight = _uiRootPrevFitHeight;
        }

        public void Update()
        {
            if (connected)
            {
                EASPacket packet = socket.Receive(EASPacket.kType2D);
                if (packet != null)
                {
                    if (packet.Get("data/next") != null)
                    {
                        // Hide loading screen
                        EASClientManager.HideLoading();

                        if (packet.GetBool("data/next"))
                        {
                            // check the current content index
                            if (packet.data.ContainsKey("current_index") && packet.GetInt("data/current_index") != iCurrentIndex)
                            {
                                iCurrentIndex = packet.GetInt("data/current_index");
                            }

                            // let's play!
                            Next();
                        }
                    }
                    else if (packet.GetBool("data/play"))
                    {
                        // check the current content index
                        if (packet.data.ContainsKey("current_index") && packet.GetInt("data/current_index") != iCurrentIndex)
                        {
                            iCurrentIndex = packet.GetInt("data/current_index");
                        }

                        // let's play!
                        StartCoroutine(LetsPlay(_fPlayTime));
                    }
                }
            }
        }

        public void RequestNext()
        {
            if (connected)
            {
                // Show loading screen
                EASClientManager.ShowLoading();

                // Send packet to server
                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kType2D;
                packet.Set("data/next", true);
                socket.Send(packet);
            }
            else
            {
                Next();
            }
        }

        public override void OnContentStart(TwoDimensionBase currentContent)
        {
            if (connected)
            {
                TwoDimensionPanel canvasPanel = currentContent.GetComponentInChildren<TwoDimensionPanel>();
                if (canvasPanel != null && canvasPanel.cSketchCanvas != null)
                {
                    EASClientCanvas canvas = (EASClientCanvas)canvasPanel.cSketchCanvas;
                    if (canvas != null)
                    {
                        canvas.socket = socket;
                    }
                }
            }

            base.OnContentStart(currentContent);
        }

        public override void OnContentPlay(TwoDimensionBase currentContent)
        {
            if (connected)
            {
                _SendReadyPacket();
            }

            base.OnContentPlay(currentContent);
        }

        private void _SendReadyPacket()
        {
            EASPacket packet = new EASPacket();
            packet.type = EASPacket.kType2D;
            packet.Set("data/ready", true);
            socket.Send(packet, (flag) =>
            {
                if (!flag)
                {
                    _SendReadyPacket();
                }
            });
        }

        public override void OnContentEnd(TwoDimensionBase currentContent)
        {
            TwoDimensionPanel canvasPanel = currentContent.GetComponentInChildren<TwoDimensionPanel>();
            if (canvasPanel != null && canvasPanel.cSketchCanvas != null)
            {
                EASClientCanvas canvas = (EASClientCanvas)canvasPanel.cSketchCanvas;
                if (canvas != null)
                    canvas.socket = null;
            }

            base.OnContentEnd(currentContent);
        }
        #endregion
    }
}