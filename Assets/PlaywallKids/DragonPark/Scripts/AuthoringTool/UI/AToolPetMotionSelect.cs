using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    public class AToolPetMotionSelect : AnimatablePanel
    {
        #region Public variables
        public Transform characterPos;

        public UILabel characterNameLabel;

        public System.Action onCharacterSelected = null;
        #endregion

        #region Properties
        private List<GameObject> _characterPrefabs = new List<GameObject>();
        public List<GameObject> characterPrefabs
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
        public AToolPetMotionPanel mainPanel
        {
            get
            {
                if (_mainPanel == null)
                {
                    return null;
                }
                return _mainPanel.Target as AToolPetMotionPanel;
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
                    _SetEnableAnimationsOfSelectedCharacter(false);
                    _DisableDragonAnimationPathfinding();


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

        public void NextStep()
        {
            _SetEnableAnimationsOfSelectedCharacter(true);
            mainPanel.NextStep();
        }

        private void _SetupLayerForSelectedCharacter()
        {
            if (_selectedCharacter != null)
            {
                _selectedCharacter.SetLayerRecursively("Template3D");
            }
        }

        private void _SetEnableAnimationsOfSelectedCharacter(bool val)
        {
            if (_selectedCharacter != null)
            {
                Animator characterAnimator = _selectedCharacter.GetComponent<Animator>();
                if (characterAnimator != null)
                    characterAnimator.enabled = val;
            }
        }

        private void _DisableDragonAnimationPathfinding()
        {
            if (_selectedCharacter != null)
            {
                DragonAnimationControl dragonAnimation = _selectedCharacter.GetComponent<DragonAnimationControl>();
                if (dragonAnimation != null)
                    dragonAnimation.movesAlongPath = false;
            }
        }
    }
}