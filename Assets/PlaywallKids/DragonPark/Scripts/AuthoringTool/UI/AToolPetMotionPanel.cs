using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    public class AToolPetMotionPanel : AnimatablePanel
    {
        #region Public variables
        public AToolPetMotionSelect characterSelect;
        public AToolPetMotionMaker motionMaker;

        public int userId = 0;
        public int userSeq = 0;
        #endregion

        #region Private variables
        private bool _usesCurrentTemplate = false;
        private static GameObject[] _instantiatedPets = new GameObject[5];
        #endregion

        public override void BeginShow()
        {
            base.BeginShow();

            characterSelect.Deactive();
            motionMaker.Deactive();

            characterSelect.mainPanel = this;
            motionMaker.mainPanel = this;

            _usesCurrentTemplate = false;
            NextStep();
        }

        public void NextStep()
        {
            if (characterSelect.isShowing)
            {
                characterSelect.Hide();
                _InitForCharacter(Instantiate(characterSelect.selectedCharacter));
                motionMaker.Show();
            }
            else if (motionMaker.isShowing)
            {
                var motions = motionMaker.GetMotionsInMotionTimeBox();
                motionMaker.Hide();

                if (motions.Count > 0)
                {
                    GameObject newDragon = null;
                    DragonAnimationControl control = null;
                    DragonMotionList motionList = null;

                    if (_usesCurrentTemplate)
                    {
                        newDragon = SimpleInstantiatedTemplateControl.GetCurrentTemplate(userId);
                        control = newDragon.GetComponent<DragonAnimationControl>();
                    }
                    else
                    {
                        if (_instantiatedPets.Length > userId && _instantiatedPets[userId] != null)
                        {
                            Destroy(_instantiatedPets[userId]);
                            _instantiatedPets[userId] = null;
                        }

                        newDragon = Instantiate(motionMaker.dragon.gameObject);

                        control = newDragon.GetComponent<DragonAnimationControl>();
                        if (control != null)
                            control.UseTemplete3D();

                        newDragon.transform.parent = null;
                        newDragon.transform.localPosition = DragonComeToFront.GetDummyPosition("petmotion", userId);
                        newDragon.transform.LookAt(Camera.main.transform.position);
                        newDragon.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                        newDragon.SetLayerRecursively(LayerMask.NameToLayer("MainScene"));
                    }

                    motionList = newDragon.GetComponent<DragonMotionList>();
                    if (motionList == null)
                        motionList = newDragon.gameObject.AddComponent<DragonMotionList>();
                    motionList.Set(motions);

                    if (!_usesCurrentTemplate)
                    {
                        motionList.onFinished = () =>
                        {
                            TweenScale.Begin(newDragon, 0.25f, Vector3.zero);
                            if (control != null)
                                control.effect.Pop();
                            Destroy(newDragon, 1f);
                        };
                    }
                    motionList.Play();

                    if (_instantiatedPets.Length > userId)
                    {
                        _instantiatedPets[userId] = newDragon;
                    }
                }

                MenuControl.sharedInstance.HidePetMotion(userId);
                if (MenuControl.sharedInstance.IsPetMenuOpen(userId))
                    MenuControl.sharedInstance.ShowPetMenu(userId);
                else
                    MenuControl.sharedInstance.Activate(userId);
            }
            else
            {
                characterSelect.Show();
            }
        }

        public void UseCurrentTemplate()
        {
            GameObject currentObj = SimpleInstantiatedTemplateControl.GetCurrentTemplate(userId);
            Template3D template = currentObj.GetComponent<Template3D>();
            if (template != null)
            {
                _usesCurrentTemplate = true;
                string identifier = template.identifier;
                Template3D newTemplate = ResourceManager.LoadTemplate3D(identifier);
                newTemplate.gameObject.SetLayerRecursively(gameObject.layer);
                newTemplate.CleanMainTexture();

                characterSelect.Hide();
                _InitForCharacter(newTemplate.gameObject);
                motionMaker.Show();
            }
            else
                Debug.LogError(string.Format("There's no instantiated template of user #{0}!", userId));
        }

        private void _InitForCharacter(GameObject go)
        {
            DragonAnimationControl control = go.GetComponent<DragonAnimationControl>();
            if (control != null)
                control.UseTemplete3D();

            Dragon dragon = go.GetComponent<Dragon>();
            if (dragon == null)
            {
                dragon = go.AddComponent<Dragon>();
                dragon.characterName = characterSelect.selectedCharacter.name;
            }
            motionMaker.dragon = dragon;
        }
    }
}