using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Authoring Tool <br>
    /// Step 1, Character Selection
    /// </summary>
    public class AToolCharacterSelect : AToolCharacterStep
    {
        // Public variables
        public List<GameObject> characterPrefabs = new List<GameObject>();
        public Transform characterPos;

        public AToolItemList itemList;
        public UIAtlas uiAtlas;

        public UILabel characterNameLabel;

        // Properties
        private int _selectedCharacterIndex = 0;
        public int selectedCharacterIndex
        {
            get
            {
                return _selectedCharacterIndex;
            }
        }

        private GameObject _selectedCharacter = null;
        public GameObject selectedCharacter
        {
            get
            {
                return _selectedCharacter;
            }
        }

        public string selectedCharacterName
        {
            get
            {
                return Dragon.characterNames[selectedCharacterIndex];
            }
        }

        // Private properties
        private GameObject selectedCharacterInternal
        {
            set
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

                    _selectedCharacter = Instantiate(value);

                    DragonAnimationControl control = _selectedCharacter.GetComponent<DragonAnimationControl>();
                    if (control != null)
                        control.UseTemplete3D();

                    _selectedCharacter.transform.parent = characterPos;
                    _selectedCharacter.transform.localPosition = Vector3.zero;
                    _selectedCharacter.transform.localRotation = Quaternion.identity;
                    _selectedCharacter.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                    _SetupLayerForSelectedCharacter();
                    _DisableAnimationsOfSelectedCharacter();
                }

                string localizedCharacterName = Dragon.GetLocalizedCharacterName(selectedCharacterName);
                characterNameLabel.text = localizedCharacterName;

                AnimationPlay();
            }
        }

        private AutoRotate _autoRotateCharacter = null;
        private bool rotate
        {
            get
            {
                bool value = false;
                if (_autoRotateCharacter != null)
                {
                    value = _autoRotateCharacter.enabled;
                }
                return value;
            }
            set
            {
                if (_autoRotateCharacter == null)
                {
                    _autoRotateCharacter = selectedCharacter.GetComponent<AutoRotate>();
                    if (_autoRotateCharacter == null)
                    {
                        _autoRotateCharacter = selectedCharacter.gameObject.AddComponent<AutoRotate>();
                        _autoRotateCharacter.anglePerSecond = 180.0f;
                        _autoRotateCharacter.isLocal = true;
                        _autoRotateCharacter.axis = Vector3.up;

                        Vector3 angle = selectedCharacter.transform.localRotation.eulerAngles;
                        _autoRotateCharacter.initialAngle = angle.y;
                    }
                }
                _autoRotateCharacter.enabled = value;
            }
        }

        private List<UISprite> listFilters;
        private GameObject goTextures;
        private GameObject goIcons;

        public void OnEnable()
        {
            _SetupLayerForSelectedCharacter();
            _DisableAnimationsOfSelectedCharacter();

            MakeItemIcons();
        }

        public void OnDisable()
        {
            DestroyItemIcons();
        }

        public override void Reset()
        {
            if (characterPrefabs.Count == 0)
            {
                string[] characterNames = Dragon.characterNames;
                foreach (string character in characterNames)
                {
                    GameObject prefab = Dragon.LoadPrefab(character);
                    characterPrefabs.Add(prefab);
                }
            }
            selectedCharacterInternal = characterPrefabs[0];
            rotate = false;
            characterPos.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        }

        public override void Rotate()
        {
            rotate = true;
            _autoRotateCharacter.rotateType = AutoRotate.RotateType.Right;
        }

        public override void RotateLeft()
        {
            rotate = true;
            _autoRotateCharacter.rotateType = AutoRotate.RotateType.Left;
        }

        public override void RotateStop()
        {
            rotate = false;
        }

        public override void GoToNextStep()
        {
            selectedCharacterInternal = characterPrefabs[_selectedCharacterIndex];
            StartCoroutine(CallBaseNext());
        }

        private IEnumerator CallBaseNext()
        {
            yield return new WaitForEndOfFrame();
            base.GoToNextStep();
        }

        public void PrevCharacter()
        {
            GameObject go = characterPrefabs[(_selectedCharacterIndex == 0 ?
                                              characterPrefabs.Count - 1 :
                                              _selectedCharacterIndex - 1)];
            selectedCharacterInternal = go;
        }

        public void NextCharacter()
        {
            GameObject go = characterPrefabs[(_selectedCharacterIndex + 1) % characterPrefabs.Count];
            selectedCharacterInternal = go;
        }

        public void SelectCharacter(int idx)
        {
            GameObject go = characterPrefabs[idx % characterPrefabs.Count];
            selectedCharacterInternal = go;

            foreach (UISprite st in listFilters)
            {
                st.enabled = true;
            }
            listFilters[idx].enabled = false;

            // current filter off
            for (int i = 0; i < itemList.itemButtons.Length; i++)
                ActiveButtonFilter(i, true);
            ActiveButtonFilter(idx, false);
        }

        private void ActiveButtonFilter(int idx, bool active)
        {
            UITexture tex = itemList.itemButtons[idx % itemList.itemButtons.Length].GetComponentInChildren<UITexture>();
            if (tex != null)
            {
                UISprite curFilter = tex.GetComponentInChildren<UISprite>();
                if (curFilter != null)
                {
                    curFilter.enabled = active;
                }
            }
        }

        public void AnimationPlay()
        {
            rotate = false;
            TweenRotation.Begin(_autoRotateCharacter.gameObject, 0.25f, Quaternion.identity);
        }

        private void MakeItemIcons()
        {
            itemList.rotateItem = false;

            goTextures = new GameObject("DragonTextures");
            goTextures.transform.parent = transform;
            goTextures.transform.localScale = Vector3.one;
            goTextures.layer = gameObject.layer;

            goIcons = new GameObject("DragonIcons");

            listFilters = new List<UISprite>();
            List<GameObject> items = new List<GameObject>();
            for (int i = 0; i < Dragon.characterNames.Length; i++)
            {
                UISprite back = NGUITools.AddChild<UISprite>(goTextures);
                back.atlas = uiAtlas;
                back.spriteName = "a_bg";
                back.SetDimensions(150, 150);
                back.depth = 25;

                UITexture texture = NGUITools.AddChild<UITexture>(back.gameObject);
                texture.SetDimensions(125, 125);
                texture.depth = 30;

                UISprite filter = NGUITools.AddChild<UISprite>(texture.gameObject);
                filter.atlas = uiAtlas;
                filter.spriteName = "a_bg";
                filter.SetDimensions(150, 150);
                filter.depth = 35;
                filter.color = new Color(0f, 0f, 0f, 0.7f);

                listFilters.Add(filter);

                DragonIcon icon = NGUITools.AddChild<DragonIcon>(goIcons);
                icon.SetDragon(Dragon.LoadPrefab(Dragon.characterNames[i]), i);
                texture.mainTexture = icon.texture;

                items.Add(back.gameObject);
            }
            itemList.items = items.ToArray();
            itemList.onClick = (idx) =>
            {
                SelectCharacter(idx);
            };

            goTextures.SetActive(false);

            SelectCharacter(0);
        }

        private void DestroyItemIcons()
        {
            Destroy(goTextures);
            Destroy(goIcons);
        }



        private void _SetupLayerForSelectedCharacter()
        {
            if (_selectedCharacter != null)
            {
                int layer = LayerMask.NameToLayer("Template3D");

                Transform[] tfs = _selectedCharacter.GetComponentsInChildren<Transform>();
                foreach (Transform t in tfs)
                {
                    t.gameObject.layer = layer;
                }
                _selectedCharacter.layer = layer;
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
                    dragonAnimation.usesNavMesh = false;
                }

                DragonEffect effect = _selectedCharacter.GetComponent<DragonEffect>();
                if (effect != null)
                {
                    effect.enabled = false;
                }
            }
        }
    }
}