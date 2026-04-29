using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.EAS
{
    using TwoDimensionDrawScene;

    public class EASClientTwoDimensionSandDraw : TwoDimensionSandDraw
    {
        #region Public variables
        public UIButton eventButton;
        #endregion

        #region Properties
        public EASSocket socket
        {
            get
            {
                if (_admin != null)
                {
                    return _admin.socket;
                }

                return null;
            }
        }
        #endregion

        #region Private variables
        private EASClientTwoDimensionAdmin _admin = null;

        private bool _generateEvent = false;

        private bool bPrevEvent = false;

        private bool _sentEventReadyOnPlayStart = false;
        #endregion

        #region Constants
        public const string kType2DSandDraw = "2d.sand_draw";
        #endregion

        #region Methods
        public override bool PlayStart()
        {
            _generateEvent = false;
            bPrevEvent = bEvent;

            return base.PlayStart();
        }

        public void OnEnable()
        {
            if (_admin == null)
                _admin = GetComponentInParent<EASClientTwoDimensionAdmin>();

            if (_admin == null)
            {
                _admin = GameObject.FindObjectOfType<EASClientTwoDimensionAdmin>();
            }

            eventButton.gameObject.SetActive(EASClientManager.currentManager.root);
            eventButton.isEnabled = true;

            if (!_sentEventReadyOnPlayStart)
            {
                _sentEventReadyOnPlayStart = true;
                _SendEventReady();
            }
        }

        public void OnDisable()
        {
            _admin = null;

            _sentEventReadyOnPlayStart = false;
        }

        public void RequestGenerateEvent()
        {
            if (socket != null && socket.connected && EASClientManager.currentManager.root)
            {
                EASPacket packet = new EASPacket();
                packet.type = kType2DSandDraw;
                packet.Set("data/event", true);
                socket.Send(packet);

                eventButton.isEnabled = false;
            }
        }

        public virtual void Update()
        {
            if (socket != null && socket.connected)
            {
                _CheckEventIsFinished();

                EASPacket packet = socket.Receive(kType2DSandDraw);
                if (packet != null)
                {
                    if (packet.Get("data/event") != null)
                    {
                        if (packet.GetBool("data/event/result"))
                        {
                            _generateEvent = true;
                        }

                        eventButton.isEnabled = true;
                    }
                }

                if (_generateEvent && !bEvent)
                {
                    GenerateEvent();
                    _generateEvent = false;
                }

                bPrevEvent = bEvent;
            }
        }

        private void _CheckEventIsFinished()
        {
            if (bPrevEvent && !bEvent)
            {
                _SendEventReady();
            }
        }

        private void _SendEventReady()
        {
            EASPacket packet = new EASPacket();
            packet.type = kType2DSandDraw;
            packet.Set("data/event_ready", true);

            socket.Send(packet, (flag) =>
            {
                if (!flag)
                    _SendEventReady();
            });
        }
        #endregion
    }
}