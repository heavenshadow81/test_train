using UnityEngine;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;
    public class EASAToolClientPetMotionPanel : EASAnimatablePanel
    {
        #region Public variables
        public EASAToolClientPetMotionSelectPanel characterSelect;
        public EASAToolClientPetMotionMakerPanel motionMaker;
        #endregion

        #region Properties
        #endregion

        public override void BeginShow()
        {
            base.BeginShow();

            characterSelect.Deactive();
            motionMaker.Deactive();

            characterSelect.mainPanel = this;
            motionMaker.mainPanel = this;

            characterSelect.socket = socket;
            motionMaker.socket = socket;

            NextStep();
        }

        public void NextStep()
        {
            if (characterSelect.isShowing)
            {
                characterSelect.Hide();
                _InitForCharacter(characterSelect.selectedCharacter);
                motionMaker.Show();
            }
            else if (motionMaker.isShowing)
            {
                motionMaker.Hide();
                Hide();
            }
            else
            {
                characterSelect.Show();
            }
        }

        private void _InitForCharacter(GameObject go)
        {
            GameObject newGo = (GameObject)Instantiate(go);

            newGo.transform.position = go.transform.position;
            newGo.transform.rotation = go.transform.rotation;
            Vector3 firstCPScale = characterSelect.characterPos.transform.localScale;
            Vector3 newScale = Vector3.Scale(go.transform.lossyScale, motionMaker.characterPos.transform.localScale);
            newScale.x /= firstCPScale.x;
            newScale.y /= firstCPScale.y;
            newScale.z /= firstCPScale.z;
            newGo.transform.localScale = newScale;

            Dragon dragon = newGo.GetComponent<Dragon>();
            if (dragon == null)
            {
                dragon = newGo.AddComponent<Dragon>();
            }
            dragon.characterName = characterSelect.selectedCharacter.name;

            motionMaker.dragon = dragon;
        }
    }
}