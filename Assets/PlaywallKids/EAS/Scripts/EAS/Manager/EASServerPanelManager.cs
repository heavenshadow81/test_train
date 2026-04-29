using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASServerPanelManager : MonoBehaviour
    {
        #region Public variables
        public UIRoot root;

        public UIPanel playgroundPanel;

        public EASRemoteCanvasControl canvasRemotePanelPrefab;
        public EASAToolRemote3DPanel aToolRemote3DPanelPrefab;
        public EASAToolRemotePetPanel aToolRemotePetPanelPrefab;
        public EASAToolRemotePetMotionPanel aToolRemotePetMotionPanelPrefab;
        #endregion

        #region Properties
        public int clientCount
        {
            get
            {
                return _clients.Count;
            }
        }

        private Dictionary<EASServerClientInfo, AnimatablePanel> _activePanels = new Dictionary<EASServerClientInfo, AnimatablePanel>();
        public AnimatablePanel[] activePanels
        {
            get
            {
                return new List<AnimatablePanel>(_activePanels.Values).ToArray();
            }
        }

        private string _menu = EASPacket.kTypeNone;
        public string menu
        {
            get
            {
                return _menu;
            }
        }
        #endregion

        #region Constants
        public const int kWidthForClient = 640;
        #endregion

        #region Private variables
        private bool _destroying = false;

        private List<EASServerClientInfo> _clients = new List<EASServerClientInfo>();
        #endregion

        public void OnEnable()
        {
            if (root == null)
            {
                root = UIRoot.list[0];
            }

            ResetContentRegion();
        }

        public void OnDestroy()
        {
            _destroying = true;
        }

        public void ResetContentRegion()
        {
            if (root != null)
            {

                if (clientCount == 1)
                {
                    root.manualWidth = kWidthForClient * 5 / 2;
                }
                else if (clientCount == 2)
                {
                    root.manualWidth = kWidthForClient * 3;
                }
                else
                {
                    root.manualWidth = kWidthForClient * clientCount;
                }
            }
        }

        public void AddClient(EASServerClientInfo client)
        {
            if (!_clients.Contains(client))
            {
                _clients.Add(client);

                if (!_destroying)
                {
                    _AddPanel(client);
                    _LayoutAllPanels();
                    ResetContentRegion();
                }
            }
        }

        public void RemoveClient(EASServerClientInfo client)
        {
            if (_clients.Contains(client))
            {
                _clients.Remove(client);

                if (!_destroying)
                {
                    _RemovePanel(client);
                    _LayoutAllPanels();
                    ResetContentRegion();
                }
            }
        }

        public void PrepareMenu(string newMenu)
        {
            if (!menu.Equals(newMenu))
            {
                // clear all active panels
                foreach (EASServerClientInfo client in _clients)
                {
                    _RemovePanel(client);
                }

                // set _menu as new value
                _menu = newMenu;

                // re-create active panels for all clients
                foreach (EASServerClientInfo client in _clients)
                {
                    _AddPanel(client);
                }

                _LayoutAllPanels();
            }
        }

        private void _AddPanel(EASServerClientInfo client)
        {
            switch (menu)
            {
                case EASPacket.kTypeSketch:
                    _AddCanvasRemotePanel(client);
                    break;
                case EASPacket.kType3D:
                    _AddAToolRemote3DPanel(client);
                    break;
                case EASPacket.kTypePet:
                    _AddAToolRemotePetPanel(client);
                    break;
                case EASPacket.kTypePetMotion:
                    _AddAToolRemotePetMotionPanel(client);
                    break;
                default:
                    break;
            }
        }

        private void _AddCanvasRemotePanel(EASServerClientInfo client)
        {
            EASRemoteCanvasControl panel = NGUITools.AddChild(playgroundPanel.gameObject, canvasRemotePanelPrefab.gameObject).GetComponent<EASRemoteCanvasControl>();
            panel.socket = client.client;
            panel.Show();

            _RemovePanel(client);
            _activePanels[client] = panel;
        }

        private void _AddAToolRemote3DPanel(EASServerClientInfo client)
        {
            EASAToolRemote3DPanel panel = NGUITools.AddChild(playgroundPanel.gameObject, aToolRemote3DPanelPrefab.gameObject).GetComponent<EASAToolRemote3DPanel>();
            panel.socket = client.client;
            panel.Show();

            _RemovePanel(client);
            _activePanels[client] = panel;
        }

        private void _AddAToolRemotePetPanel(EASServerClientInfo client)
        {
            EASAToolRemotePetPanel panel = NGUITools.AddChild(playgroundPanel.gameObject, aToolRemotePetPanelPrefab.gameObject).GetComponent<EASAToolRemotePetPanel>();
            panel.socket = client.client;
            panel.userId = _clients.IndexOf(client) % 3;
            panel.userSeq = _clients.IndexOf(client) % 3;
            panel.Show();

            _RemovePanel(client);
            _activePanels[client] = panel;
        }

        private void _AddAToolRemotePetMotionPanel(EASServerClientInfo client)
        {
            EASAToolRemotePetMotionPanel panel = NGUITools.AddChild(playgroundPanel.gameObject, aToolRemotePetMotionPanelPrefab.gameObject).GetComponent<EASAToolRemotePetMotionPanel>();
            panel.socket = client.client;
            panel.userId = _clients.IndexOf(client) % 3;
            panel.userSeq = _clients.IndexOf(client) % 3;
            panel.Show();

            _RemovePanel(client);
            _activePanels[client] = panel;
        }

        private void _RemovePanel(EASServerClientInfo client)
        {
            AnimatablePanel panel = null;

            if (client != null)
            {
                _activePanels.TryGetValue(client, out panel);
            }

            if (panel != null)
            {
                panel.Hide();

                Destroy(panel.gameObject, 5.0f);
            }
        }

        private void _LayoutAllPanels()
        {
            float startPosX = -(clientCount - 1) * 0.5f * kWidthForClient;

            for (int i = 0; i < clientCount; i++)
            {
                var client = _clients[i];

                if (client != null && _activePanels.ContainsKey(client))
                {
                    var panel = _activePanels[client];

                    if (panel != null)
                    {
                        Vector3 pos = panel.cachedTransform.localPosition;
                        pos.x = startPosX + kWidthForClient * i;
                        pos.y = 0;
                        panel.cachedTransform.localPosition = pos;
                    }
                }
            }
        }
    }
}