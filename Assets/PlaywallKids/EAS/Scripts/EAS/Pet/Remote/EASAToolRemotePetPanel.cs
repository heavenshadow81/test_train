using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;
    public class EASAToolRemotePetPanel : EASAnimatablePanel
    {
        #region Public variables
        public EASAToolRemotePetSelectPanel characterSelect;
        public EASAToolRemotePetSizingPanel characterSizing;
        public EASAToolRemotePetDrawingPanel characterDrawing;
        public EASAToolRemotePetFreeDrawingPanel characterFreeDrawing;

        public EASRemoteModelControl modelControl;

        public int userId = 0;
        public int userSeq = 0;
        #endregion

        #region Properties
        #endregion

        public void Update()
        {
            if (connected && isShowing)
            {
                while (true)
                {
                    EASPacket packet = socket.Receive(EASPacket.kTypePet);

                    if (packet != null)
                    {
                        ProcessPacket(packet);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public void ProcessPacket(EASPacket packet)
        {
            if (packet != null)
            {
                if (characterSelect.isShowing)
                {
                    characterSelect.ProcessPacket(packet);
                }
                else if (characterSizing.isShowing)
                {
                    characterSizing.ProcessPacket(packet);
                }
                else if (characterDrawing.isShowing)
                {
                    characterDrawing.ProcessPacket(packet);
                }
                else if (characterFreeDrawing.isShowing)
                {
                    characterFreeDrawing.ProcessPacket(packet);
                }
            }
        }

        public override void BeginShow()
        {
            base.BeginShow();

            characterSelect.Deactive();
            characterSizing.Deactive();
            characterDrawing.Deactive();
            characterFreeDrawing.Deactive();

            characterSelect.mainPanel = this;
            characterSizing.mainPanel = this;
            characterDrawing.mainPanel = this;
            characterFreeDrawing.mainPanel = this;

            characterSelect.socket = socket;
            characterSizing.socket = socket;
            characterDrawing.socket = socket;
            characterFreeDrawing.socket = socket;

            characterSizing.modelControl = modelControl;
            characterDrawing.modelControl = modelControl;
            characterFreeDrawing.modelControl = modelControl;

            modelControl.type = EASPacket.kTypePet;
            modelControl.socket = socket;

            NextStep();
        }

        public void NextStep()
        {
            modelControl.wantsPaint = false;

            if (characterSelect.isShowing)
            {
                characterSelect.Hide();
                _InitForCharacter(characterSelect.selectedCharacter);
                characterSizing.dragon = modelControl.model.GetComponent<Dragon>();
                characterSizing.Show();
            }
            else if (characterSizing.isShowing)
            {
                characterSizing.Hide();
                modelControl.wantsPaint = connected;
                foreach (Template3D template in modelControl.templates)
                {
                    template.CleanMainTexture();
                }
                characterDrawing.Show();
            }
            else if (characterDrawing.isShowing)
            {
                characterDrawing.Hide();

                GameObject model = modelControl.model;

                if (model != null)
                {
                    ResourceManager.SaveTemplate3D(modelControl.templates[0], true);
                    Template3D template = SimpleInstantiatedTemplateControl.LoadTemplate(modelControl.templates[0].identifier);

                    template.transform.parent = EASServerMapManager.sharedInstance.dragonPark.transform;

                    modelControl.model = null;
                    Destroy(model);
                }
                characterFreeDrawing.Show();
            }
            else if (characterFreeDrawing.isShowing)
            {
                characterFreeDrawing.Hide();
                Hide();
            }
            else
            {
                characterSelect.Show();
            }
        }

        private void _InitForCharacter(GameObject go)
        {
            if (modelControl.model != null)
            {
                GameObject model = modelControl.model;
                modelControl.model = null;
                Destroy(model);
            }

            GameObject newGo = (GameObject)Instantiate(go);
            Transform[] tfs = go.GetComponentsInChildren<Transform>();
            foreach (Transform t in tfs)
            {
                t.gameObject.layer = LayerMask.NameToLayer("Template3D");
            }

            newGo.transform.position = go.transform.position;
            newGo.transform.rotation = go.transform.rotation;
            Vector3 firstCPScale = characterSelect.characterPos.transform.localScale;
            Vector3 newScale = Vector3.Scale(go.transform.lossyScale, characterSizing.characterPos.transform.localScale);
            newScale.x /= firstCPScale.x;
            newScale.y /= firstCPScale.y;
            newScale.z /= firstCPScale.z;
            newGo.transform.localScale = newScale;

            Template3D newTemplate = newGo.GetComponent<Template3D>();
            if (newTemplate == null)
            {
                newTemplate = newGo.AddComponent<Template3D>();
            }

            newTemplate.character = characterSelect.selectedCharacter.name;
            newTemplate.userId = userId;
            newTemplate.userSeq = userSeq;

            Dragon dragon = newGo.GetComponent<Dragon>();
            if (dragon == null)
            {
                dragon = newGo.AddComponent<Dragon>();
            }
            dragon.characterName = characterSelect.selectedCharacter.name;

            modelControl.model = newGo;
        }
    }
}