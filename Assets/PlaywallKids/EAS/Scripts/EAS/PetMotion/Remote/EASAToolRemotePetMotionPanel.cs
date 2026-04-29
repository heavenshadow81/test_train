using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;
    public class EASAToolRemotePetMotionPanel : EASAnimatablePanel
    {
        #region Public variables
        public EASAToolRemotePetMotionSelectPanel characterSelect;
        public EASAToolRemotePetMotionMakerPanel motionMaker;

        public int userId = 0;
        public int userSeq = 0;
        #endregion

        #region Properties
        #endregion

        #region Private variables
        private static List<GameObject>[] _instantiatedPets = null;
        #endregion

        public void Update()
        {
            if (connected && isShowing)
            {
                while (true)
                {
                    EASPacket packet = socket.Receive(EASPacket.kTypePetMotion);

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
                else if (motionMaker.isShowing)
                {
                    motionMaker.ProcessPacket(packet);
                }
            }
        }

        public override void BeginShow()
        {
            base.BeginShow();

            if (_instantiatedPets == null)
            {
                _instantiatedPets = new List<GameObject>[10];
                for (int i = 0; i < _instantiatedPets.Length; i++)
                    _instantiatedPets[i] = new List<GameObject>();
            }

            characterSelect.Deactive();
            motionMaker.Deactive();

            characterSelect.mainPanel = this;
            motionMaker.mainPanel = this;

            characterSelect.socket = socket;
            motionMaker.socket = socket;

            socket.Clear(EASPacket.kTypePetMotion);

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

                if (motionMaker.animationControl.animList.Count > 0)
                {
                    if (_instantiatedPets.Length > userId && _instantiatedPets[userId] != null)
                    {
                        if (_instantiatedPets[userId].Count > 0)
                        {
                            GameObject prevDragon = _instantiatedPets[userId][0];
                            _instantiatedPets[userId].RemoveAt(0);
                            Destroy(prevDragon);
                        }
                    }

                    var newDragon = (GameObject)Instantiate(motionMaker.animationControl.gameObject);
                    newDragon.transform.parent = EASServerMapManager.sharedInstance.dragonPark.transform;
                    newDragon.transform.localPosition = new Vector3(14.0f, -0.1f, -3.65f + userId * 3.0f);
                    //newDragon.transform.localPosition = DragonComeToFront.GetDummyPosition("front", userId) + new Vector3(-5.0f, -0.1f, 1.0f);

                    newDragon.transform.LookAt(Camera.main.transform.position);
                    newDragon.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
                    newDragon.SetLayerRecursively(LayerMask.NameToLayer("MainScene"));

                    Destroy(newDragon.GetComponent<UnityEngine.AI.NavMeshAgent>());
                    Destroy(newDragon.GetComponent<Collider>());

                    var animationControl = newDragon.GetComponent<EASPetMotionAnimationControl>();
                    animationControl.playCount = 1;
                    animationControl.Play();
                    animationControl.onFinished = () =>
                    {
                        TweenScale.Begin(newDragon, 0.25f, Vector3.zero);

                        DragonAnimationControl control = newDragon.GetComponent<DragonAnimationControl>();
                        if (control != null)
                            control.effect.Pop();

                        Destroy(newDragon, 1f);
                    };

                    if (_instantiatedPets.Length > userId)
                    {
                        _instantiatedPets[userId].Add(newDragon);
                    }
                }

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

            DragonAnimationControl control = go.GetComponent<DragonAnimationControl>();
            if (control != null)
                control.UseTemplete3D();

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