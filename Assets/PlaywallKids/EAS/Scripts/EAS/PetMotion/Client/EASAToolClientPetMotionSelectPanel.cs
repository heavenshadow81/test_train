using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;
    public class EASAToolClientPetMotionSelectPanel : EASAToolPetSelectPanelBase
    {
        #region Properties
        public override List<GameObject> characterPrefabs
        {
            get
            {
                List<string> characterNames = new List<string>(Dragon.characterNames);
                characterNames.Remove("Arrow");

                List<GameObject> list = new List<GameObject>();
                foreach (string characterName in characterNames)
                {
                    GameObject prefab = Dragon.LoadPrefab(characterName);
                    if (prefab != null)
                        list.Add(prefab);
                }

                return list;
            }
        }

        private System.WeakReference _mainPanel = null;
        public EASAToolClientPetMotionPanel mainPanel
        {
            get
            {
                if (_mainPanel == null)
                {
                    return null;
                }
                return _mainPanel.Target as EASAToolClientPetMotionPanel;
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
                packet.type = EASPacket.kTypePetMotion;
                packet.Set("data/model", selectedCharacter.name);
                socket.Send(packet);
            };
            onCharacterSelected();
        }

        public void Update()
        {
            if (connected)
            {
                EASPacket packet = socket.Receive(EASPacket.kTypePetMotion, false);
                if (packet != null)
                {
                    if (packet.GetBool("data/command/next_step"))
                    {
                        _PerformNextStep();

                        socket.Receive(EASPacket.kTypePetMotion);
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
                packet.type = EASPacket.kTypePetMotion;
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