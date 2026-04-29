using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    public class AToolAvatarMainPanel : MonoBehaviour
    {
        #region Public variables
        public GameObject girlAvatarPrefab;
        public GameObject manAvatarPrefab;

        public Transform characterPos;

        public AToolItemList itemList;

        public GameObject[] accessoryPrefabs;
        #endregion

        #region Properties
        private Avatar _avatar = null;
        public Avatar avatar
        {
            get
            {
                if (_avatar == null)
                {
                    _InitForAvatar();
                }
                return _avatar;
            }
        }

        private string _gender = "f";
        public string gender
        {
            get
            {
                return _gender;
            }
            set
            {
                if (string.IsNullOrEmpty(value) ||
                   (!value.ToLower().Equals("m") && !value.ToLower().Equals("f")))
                {
                    value = "f";
                }

                if (_gender != value)
                {
                    _gender = value;
                    _InitAccessoryListUI();
                    _InitForAvatar();
                }
            }
        }

        private Texture2D _headImage = null;
        public Texture2D headImage
        {
            get
            {
                return _headImage;
            }
            set
            {
                _headImage = value;
                avatar.SetHeadImage(_headImage);
            }
        }
        #endregion

        #region Private variables
        private AutoRotate _autoRotate = null;
        #endregion

        #region Constants
        // for Debug
        public const bool kShowsAllAccessories = false;
        #endregion

        // Use this for initialization
        void Start()
        {
            _InitForAvatar();
            _InitAccessoryListUI();
        }

        public void Rotate()
        {
            if (_autoRotate == null)
            {
                _autoRotate = characterPos.GetComponent<AutoRotate>();
                if (_autoRotate == null)
                {
                    _autoRotate = characterPos.gameObject.AddComponent<AutoRotate>();
                }
                _autoRotate.axis = Vector3.up;
                _autoRotate.isLocal = true;
                _autoRotate.initialAngle = characterPos.transform.localRotation.eulerAngles.y;
                _autoRotate.anglePerSecond = 90.0f;
            }
            _autoRotate.enabled = true;
        }

        public void RotateStop()
        {
            _autoRotate.enabled = false;
        }

        public void OK()
        {
            gameObject.SetActive(false);
        }

        private void _InitForAvatar()
        {
            if (_avatar != null)
            {
                Destroy(_avatar.gameObject);
                _avatar = null;
            }

            GameObject prefab = girlAvatarPrefab;
            if (gender.Equals("m"))
            {
                prefab = manAvatarPrefab;
            }

            GameObject go = (GameObject)Instantiate(prefab);
            go.SetLayerRecursively("NGUI");
            go.transform.parent = characterPos;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            _avatar = go.GetComponent<Avatar>();
            if (_avatar == null)
            {
                _avatar = go.AddComponent<Avatar>();
            }

            if (_headImage != null)
            {
                _avatar.SetHeadImage(_headImage);
            }
            if (accessoryPrefabs != null && accessoryPrefabs.Length > 0)
            {
                SetAccessoryAtIndex(0);
            }
        }

        private void _InitAccessoryListUI()
        {
            if (kShowsAllAccessories)
            {
                accessoryPrefabs = AccessoryManager.avatarAccessories;
            }
            else
            {
                List<GameObject> prefabList = new List<GameObject>();
                for (int i = 0, cnt = AccessoryManager.avatarAccessories.Length; i < cnt; i++)
                {
                    GameObject prefab = AccessoryManager.avatarAccessories[i];
                    if (gender.Equals("m") && prefab.name.ToLower().Contains("man"))
                    {
                        prefabList.Add(prefab);
                    }
                    else if (gender.Equals("f") && prefab.name.ToLower().Contains("girl"))
                    {
                        prefabList.Add(prefab);
                    }
                }
                accessoryPrefabs = prefabList.ToArray();
            }

            GameObject[] icons = new GameObject[accessoryPrefabs.Length];
            for (int i = 0, cnt = icons.Length; i < cnt; i++)
            {
                icons[i] = AccessoryManager.GetMeshObject(accessoryPrefabs[i]);
            }

            itemList.items = icons;

            itemList.onClick = (idx) =>
            {
                SetAccessoryAtIndex(idx);
            };

            if (accessoryPrefabs.Length > 0)
            {
                SetAccessoryAtIndex(0);
            }
        }

        public void SetAccessoryAtIndex(int index)
        {
            GameObject acc = Instantiate(accessoryPrefabs[index]);
            avatar.SetAccessory(acc, true);

            itemList.AttachTweens(AccessoryManager.GetMeshObject(acc), 0.0f, 0.35f);
        }
    }
}