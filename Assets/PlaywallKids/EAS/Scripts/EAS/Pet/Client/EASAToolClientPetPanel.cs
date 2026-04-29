using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;

    public class EASAToolClientPetPanel : EASAnimatablePanel
    {
        #region Public variables
        public EASAToolClientPetSelectPanel characterSelect;
        public EASAToolClientPetSizingPanel characterSizing;
        public EASAToolClientPetDrawingPanel characterDrawing;
        public EASAToolClientPetFreeDrawingPanel characterFreeDrawing;

        public EASClientModelControl modelControl;
        #endregion

        #region Properties
        #endregion

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

                GameObject go = modelControl.model;
                if (go != null)
                {
                    modelControl.model = null;
                    Destroy(go);
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