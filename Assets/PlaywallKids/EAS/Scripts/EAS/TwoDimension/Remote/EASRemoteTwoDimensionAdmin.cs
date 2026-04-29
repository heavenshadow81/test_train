using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    using TwoDimensionDrawScene;

    public class EASRemoteTwoDimensionAdmin : TwoDimensionAdmin
    {
        #region Properties
        private List<EASSocket> _sockets = new List<EASSocket>();
        public List<EASSocket> sockets
        {
            get
            {
                return new List<EASSocket>(_sockets);
            }
        }

        public bool connected
        {
            get
            {
                bool val = false;

                for (int i = 0; i < _sockets.Count; i++)
                {
                    val = val || (_sockets[i] != null && _sockets[i].connected);
                }

                return val;
            }
        }

        protected override TwoDimensionTable cModeFile
        {
            get
            {
                if (_cModeFile == null)
                {
                    _cModeFile = new TwoDimensionTable_EASRemote();
                }
                return _cModeFile;
            }
        }

        protected override string prefabPath
        {
            get
            {
                return System.IO.Path.Combine(base.prefabPath, "EAS").Replace("\\", "/");
            }
        }

        private EASRemoteCanvas _currentRemoteCanvas = null;
        public EASRemoteCanvas currentRemoteCanvas
        {
            get
            {
                return _currentRemoteCanvas;
            }
        }
        #endregion

        #region Private variables
        private int _uiRootPrevManualWidth;
        private int _uiRootPrevManualHeight;
        private bool _uiRootPrevFitWidth, _uiRootPrevFitHeight;

        private Dictionary<EASSocket, bool> _clientReadyDict = new Dictionary<EASSocket, bool>();
        #endregion

        #region Methods
        protected override void OnEnable()
        {
            // 2d admin is optimized at resolution 3840x1440
            // so that we need to modify the ui root settings
            UIRoot root = UIRoot.list[0];
            _uiRootPrevManualWidth = root.manualWidth;
            _uiRootPrevManualHeight = root.manualHeight;
            _uiRootPrevFitWidth = root.fitWidth;
            _uiRootPrevFitHeight = root.fitHeight;

            root.manualWidth = 3840; root.manualHeight = 1440;
            root.fitWidth = false; root.fitHeight = true;

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            iCurrentIndex = 0;

            // Revert ui root settings
            UIRoot root = UIRoot.list[0];

            root.manualWidth = _uiRootPrevManualWidth; root.manualHeight = _uiRootPrevManualHeight;
            root.fitWidth = _uiRootPrevFitWidth; root.fitHeight = _uiRootPrevFitHeight;

            // Release remote canvas
            if (_currentRemoteCanvas != null)
            {
                if (_currentRemoteCanvas.GetType().Equals(typeof(EASRemoteMultiCanvas)))
                    ((EASRemoteMultiCanvas)_currentRemoteCanvas).sockets.Clear();
                else
                    _currentRemoteCanvas.socket = null;

                _currentRemoteCanvas = null;
            }

            base.OnDisable();
        }

        public void Update()
        {
            if (connected)
            {
                for (int i = 0; i < _sockets.Count; i++)
                {
                    EASSocket socket = _sockets[i];

                    if (socket != null)
                    {
                        EASPacket packet = socket.Receive(EASPacket.kType2D);

                        if (packet != null)
                        {
                            if (packet.Get("data/next") != null)
                            {
                                bool goNext = true;

                                if (packet.GetBool("data/next"))
                                {
                                    // check all client state
                                    List<EASSocket> keys = new List<EASSocket>(_clientReadyDict.Keys);
                                    foreach (var key in keys)
                                        goNext = goNext && _clientReadyDict[key];

                                    if (goNext)
                                    {
                                        EASPacket response = new EASPacket();
                                        response.type = EASPacket.kType2D;
                                        response.Set("data/next", true);
                                        response.Set("data/current_index", iCurrentIndex);

                                        _SendPacket(response);

                                        foreach (var key in keys)
                                            _clientReadyDict[key] = false;

                                        Next();
                                        break;
                                    }
                                }

                                if (!goNext)
                                {
                                    EASPacket response = new EASPacket();
                                    response.type = EASPacket.kType2D;
                                    response.Set("data/next", false);
                                    socket.Send(response);
                                }
                            }
                            else if (packet.GetBool("data/ready"))
                            {
                                _clientReadyDict[socket] = true;
                            }
                        }
                    }
                }
            }
        }

        private void _SendPacket(EASPacket packet)
        {
            if (packet != null)
            {
                for (int i = 0; i < _sockets.Count; i++)
                {
                    EASSocket socket = _sockets[i];

                    if (socket != null) socket.Send(packet);
                }
            }
        }

        public void AddSocket(EASSocket socket)
        {
            if (socket != null && !_sockets.Contains(socket))
            {
                _sockets.Add(socket);
                _clientReadyDict[socket] = false;

                Debug.Log("EASRemote2DAdmin - Added socket " + socket.ToString());

                // send play packet
                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kType2D;
                packet.Set("data/current_index", iCurrentIndex);
                packet.Set("data/play", true);
                socket.Send(packet);

                // check current remote canvas is available and check sockets
                if (_currentRemoteCanvas != null)
                {
                    if (_currentRemoteCanvas.GetType().Equals(typeof(EASRemoteMultiCanvas)))
                        ((EASRemoteMultiCanvas)_currentRemoteCanvas).sockets = _sockets;
                    else
                        _currentRemoteCanvas.socket = socket;

                    Debug.Log("EASRemoteCanvas - Added socket " + socket.ToString());
                }
            }
        }

        public void RemoveSocket(EASSocket socket)
        {
            if (socket != null && _sockets.Contains(socket))
            {
                _sockets.Remove(socket);
                _clientReadyDict.Remove(socket);

                Debug.Log("EASRemote2DAdmin - Removed socket " + socket.ToString());

                if (_currentRemoteCanvas != null)
                {
                    if (_currentRemoteCanvas.GetType().Equals(typeof(EASRemoteMultiCanvas)))
                        ((EASRemoteMultiCanvas)_currentRemoteCanvas).sockets.Clear();
                    else
                        _currentRemoteCanvas.socket = null;

                    Debug.Log("EASRemoteCanvas - Removed socket " + socket.ToString());
                }
            }
        }

        public void RemoveAllSockets()
        {
            _sockets.Clear();

            Debug.Log("EASRemote2DAdmin - Removed all sockets");

            if (_currentRemoteCanvas != null)
            {
                if (_currentRemoteCanvas.GetType().Equals(typeof(EASRemoteMultiCanvas)))
                    ((EASRemoteMultiCanvas)_currentRemoteCanvas).sockets.Clear();
                else
                    _currentRemoteCanvas.socket = null;

                Debug.Log("EASRemoteCanvas - Removed all sockets");
            }
        }

        public override void OnContentStart(TwoDimensionBase currentContent)
        {
            if (connected)
            {
                TwoDimensionPanel canvasPanel = currentContent.GetComponentInChildren<TwoDimensionPanel>();
                if (canvasPanel != null && canvasPanel.cSketchCanvas != null)
                {

                    EASRemoteCanvas canvas = canvasPanel.cSketchCanvas as EASRemoteCanvas;
                    if (canvas != null)
                    {
                        if (canvas.GetType().Equals(typeof(EASRemoteMultiCanvas)))
                            ((EASRemoteMultiCanvas)canvas).sockets = sockets;
                        else
                            canvas.socket = sockets[0];

                        _currentRemoteCanvas = canvas;
                    }
                }
            }

            base.OnContentStart(currentContent);
        }

        public override void OnContentEnd(TwoDimensionBase currentContent)
        {
            if (_currentRemoteCanvas != null)
            {
                if (_currentRemoteCanvas.GetType().Equals(typeof(EASRemoteMultiCanvas)))
                    ((EASRemoteMultiCanvas)_currentRemoteCanvas).sockets = null;
                else
                    _currentRemoteCanvas.socket = null;

                _currentRemoteCanvas = null;
            }

            base.OnContentEnd(currentContent);
        }
        #endregion
    }
}