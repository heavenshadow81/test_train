using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;
    public class EASAToolRemotePetMotionSelectPanel : EASAToolPetSelectPanelBase
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
        public EASAToolRemotePetMotionPanel mainPanel
        {
            get
            {
                if (_mainPanel == null)
                {
                    return null;
                }
                return _mainPanel.Target as EASAToolRemotePetMotionPanel;
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

        public void ProcessPacket(EASPacket packet)
        {
            if (packet != null)
            {
                if (packet.Get("data/model") != null)
                {
                    string modelName = packet.GetString("data/model");
                    SelectCharacter(modelName);
                }
                if (packet.GetBool("data/command/next_step"))
                {
                    EASPacket response = new EASPacket();
                    response.type = EASPacket.kTypePetMotion;
                    response.Set("data/command/next_step", true);
                    socket.Send(response);

                    mainPanel.NextStep();
                }
            }
        }
    }
}