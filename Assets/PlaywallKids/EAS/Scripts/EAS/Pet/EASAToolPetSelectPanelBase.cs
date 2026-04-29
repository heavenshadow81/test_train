using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;
    public class EASAToolPetSelectPanelBase : EASAnimatablePanel
    {
        #region Public variables
        public Transform characterPos;

        public UILabel characterNameLabel;

        public System.Action onCharacterSelected = null;
        #endregion

        #region Properties
        private List<GameObject> _characterPrefabs = new List<GameObject>();
        public virtual List<GameObject> characterPrefabs
        {
            get
            {
                if (_characterPrefabs.Count == 0)
                {
                    _characterPrefabs.AddRange(Resources.LoadAll<GameObject>("Template3D"));
                }
                return _characterPrefabs;
            }
        }

        private GameObject _selectedCharacter = null;
        public GameObject selectedCharacter
        {
            get
            {
                if (_selectedCharacter == null)
                {
                    selectedCharacter = characterPrefabs[0];
                }
                return _selectedCharacter;
            }
            private set
            {
                if (value == null)
                {
                    Debug.LogWarning("AToolCharacterSelect.selectedCharacterInternal : value is null.");
                    return;
                }
                int idx = characterPrefabs.IndexOf(value);
                if (idx > -1)
                {
                    _selectedCharacterIndex = idx;

                    if (_selectedCharacter != null)
                    {
                        Destroy(_selectedCharacter);
                    }

                    _selectedCharacter = (GameObject)Instantiate(value);

                    DragonAnimationControl control = _selectedCharacter.GetComponent<DragonAnimationControl>();
                    if (control != null)
                        control.UseTemplete3D();

                    _selectedCharacter.name = value.name;
                    _selectedCharacter.SetActive(true);
                    _selectedCharacter.transform.parent = characterPos;
                    _selectedCharacter.transform.localPosition = Vector3.zero;
                    _selectedCharacter.transform.localRotation = Quaternion.identity;
                    _selectedCharacter.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                    _SetupLayerForSelectedCharacter();
                    _DisableAnimationsOfSelectedCharacter();

                    if (characterNameLabel != null)
                    {
                        string characterName = Dragon.GetLocalizedCharacterName(_selectedCharacter.name);
                        characterNameLabel.text = characterName;
                    }

                    if (onCharacterSelected != null)
                    {
                        onCharacterSelected();
                    }
                }
            }
        }

        private int _selectedCharacterIndex = 0;
        public int selectedCharacterIndex
        {
            get
            {
                return _selectedCharacterIndex;
            }
        }
        #endregion

        public override void BeginShow()
        {
            base.BeginShow();

            selectedCharacter = characterPrefabs[0];
        }

        public void PrevCharacter()
        {
            GameObject go = characterPrefabs[(_selectedCharacterIndex == 0 ?
                                              characterPrefabs.Count - 1 :
                                              _selectedCharacterIndex - 1)];
            selectedCharacter = go;
        }

        public void NextCharacter()
        {
            GameObject go = characterPrefabs[(_selectedCharacterIndex + 1) % characterPrefabs.Count];
            selectedCharacter = go;
        }

        public void SelectCharacter(string characterName)
        {
            for (int i = 0; i < characterPrefabs.Count; i++)
            {
                if (characterPrefabs[i].name.Equals(characterName))
                {
                    selectedCharacter = characterPrefabs[i];
                    break;
                }
            }
        }

        private void _SetupLayerForSelectedCharacter()
        {
            if (_selectedCharacter != null)
            {
                _selectedCharacter.SetLayerRecursively("Template3D");
            }
        }

        private void _DisableAnimationsOfSelectedCharacter()
        {
            if (_selectedCharacter != null)
            {
                Animation characterAnimation = _selectedCharacter.GetComponent<Animation>();
                if (characterAnimation == null)
                {
                    characterAnimation = _selectedCharacter.GetComponentInChildren<Animation>();
                }

                if (characterAnimation != null)
                {
                    characterAnimation.enabled = false;
                }

                Animator characterAnimator = _selectedCharacter.GetComponent<Animator>();
                if (characterAnimator == null)
                {
                    characterAnimator = _selectedCharacter.GetComponentInChildren<Animator>();
                }

                if (characterAnimator != null)
                {
                    characterAnimator.enabled = false;
                }

                DragonAnimationControl dragonAnimation = _selectedCharacter.GetComponent<DragonAnimationControl>();
                if (dragonAnimation != null)
                {
                    dragonAnimation.movesAlongPath = false;
                }
            }
        }
    }
}