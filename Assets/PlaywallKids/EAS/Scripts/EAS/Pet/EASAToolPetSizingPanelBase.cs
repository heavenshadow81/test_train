using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    using DragonPark;

    public class EASAToolPetSizingPanelBase : EASAnimatablePanel
    {
        #region Public variables
        // character position
        public Transform characterPos;

        // item list
        public AToolItemList itemList;

        // handler
        public System.Action<GameObject> onAccessorySet;
        #endregion

        #region Properties
        private GameObject[] _accessoryPrefabs = null;
        public GameObject[] accessoryPrefabs
        {
            get
            {
                if (_accessoryPrefabs == null)
                {
                    // 임시로 머리만 
                    List<GameObject> accessories = new List<GameObject>(AccessoryManager.list[0]);

#if BIGBOARD_EVERLAND
                for (int i = 0; i < accessories.Count; i++)
                {
                    GameObject acc = accessories[i];

                    // Remove head accessories if selected character is related in Everland.
                    if (dragon != null && dragon.characterName.Contains("Everland_") &&
                        (acc.name.Contains("Hat") || acc.name.Contains("Cap") || acc.name.Contains("Helmet")))
                    {
                        accessories.RemoveAt(i--);
                    }
                }
#endif
                    _accessoryPrefabs = accessories.ToArray();
                }
                return _accessoryPrefabs;
            }
        }

        private Dragon _dragon = null;
        public Dragon dragon
        {
            get
            {
                return _dragon;
            }
            set
            {
                _dragon = value;
                if (_dragon != null && _dragon.gameObject != null)
                {
                    _dragon.transform.parent = characterPos;
                    TweenPosition.Begin(_dragon.gameObject, 0.25f, Vector3.zero);
                    TweenRotation.Begin(_dragon.gameObject, 0.25f, Quaternion.identity);
                }
            }
        }
        #endregion

        public override void BeginShow()
        {
            base.BeginShow();

            if (itemList != null)
            {
                if (itemList.items.Length == 0)
                {
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
                }
            }
        }

        public override void Active()
        {
            base.Active();

            if (itemList != null)
            {
                itemList.Refresh();
            }
        }

        public void SetAccessory(string newAccessoryName)
        {
            for (int i = 0; i < accessoryPrefabs.Length; i++)
            {
                GameObject acc = accessoryPrefabs[i];
                string accName = AccessoryManager.GetAccessoryName(acc);
                if (accName.Equals(newAccessoryName))
                {
                    SetAccessoryAtIndex(i);
                    break;
                }
            }
        }

        public void SetAccessoryAtIndex(int index)
        {
            if (dragon != null)
            {
                GameObject acc = (GameObject)Instantiate(accessoryPrefabs[index]);
                dragon.SetAccessory(acc);
                if (itemList != null)
                {
                    itemList.AttachTweens(AccessoryManager.GetMeshObject(acc), 0.0f, 0.35f);
                }
                if (onAccessorySet != null)
                {
                    onAccessorySet(acc);
                }
            }
        }
    }
}