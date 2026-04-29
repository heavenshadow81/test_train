using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASAToolClientPetSelectPanel : EASAToolPetSelectPanelBase
    {
        #region Properties
        private System.WeakReference _mainPanel = null;
        public EASAToolClientPetPanel mainPanel
        {
            get
            {
                if (_mainPanel == null)
                {
                    return null;
                }
                return _mainPanel.Target as EASAToolClientPetPanel;
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
        #endregion

        public override void BeginShow()
        {
            base.BeginShow();
            onCharacterSelected = () =>
            {
                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kTypePet;
                packet.Set("data/model", selectedCharacter.name);
                socket.Send(packet);
            };
            onCharacterSelected();
        }

        public void Update()
        {
            if (connected)
            {
                EASPacket packet = socket.Receive(EASPacket.kTypePet, false);
                if (packet != null)
                {
                    if (packet.GetBool("data/command/next_step"))
                    {
                        _PerformNextStep();

                        socket.Receive(EASPacket.kTypePet);
                    }
                }
            }
        }

        public void NextStep()
        {
            if (connected)
            {
                EASClientManager.ShowLoading();

                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kTypePet;
                packet.Set("data/command/next_step", true);
                socket.Send(packet);
            }
            else
            {
                _PerformNextStep();
            }
        }

        private void _PerformNextStep()
        {
            EASClientManager.HideLoading();

            mainPanel.NextStep();
        }
    }
}