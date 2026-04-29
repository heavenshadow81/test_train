using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Authoring Tool <br>
    /// Step 3, Accessories
    /// </summary>
    public class AToolCharacterEquip : AToolCharacterStep
    {
        #region Public variables
        // character position
        public Transform characterPos;

        // item list
        public AToolItemList itemList;
        public Transform toggleParent;
        #endregion

        #region Properties
        private ModelSizeControl[] _modelSizeControls;
        public ModelSizeControl[] modelSizeControls
        {
            get
            {
                if (_modelSizeControls == null ||
                   _modelSizeControls.Length == 0)
                {
                    _modelSizeControls = GetComponentsInChildren<ModelSizeControl>();
                }
                return _modelSizeControls;
            }
        }
        #endregion

        public void OnEnable()
        {
            _InitAccessoryListUI();
        }

        // Update is called once per frame
        public override void Reset()
        {
            if (defaultList == null)
            {
                // Sets default list of accessories
                defaultList = new Dictionary<AccessoryManager.AccessoryType, GameObject[]>(AccessoryManager.list);
            }

            Template3D template = mainPanel.currentTemplate;
            if (template != null)
            {
                if (template.cachedTransform.parent != characterPos)
                {
                    template.cachedTransform.parent = characterPos;

                    TweenPosition.Begin(template.gameObject, 0.25f, Vector3.zero);
                    TweenRotation.Begin(template.gameObject, 0.25f, Quaternion.identity);
                }
            }
            _InitAccessoryListUI();
        }

        public override void Rotate()
        {
            SimpleModelControl modelControl = mainPanel.modelControl;
            modelControl.RotateAndType(true, AutoRotate.RotateType.Right);
        }

        public override void RotateLeft()
        {
            SimpleModelControl modelControl = mainPanel.modelControl;
            modelControl.RotateAndType(true, AutoRotate.RotateType.Left);
        }

        public override void RotateStop()
        {
            SimpleModelControl modelControl = mainPanel.modelControl;
            modelControl.rotate = false;
        }

        public static Dictionary<AccessoryManager.AccessoryType, GameObject[]> defaultList;

        private AccessoryManager.AccessoryType? _accessoryType = null;
        private AccessoryManager.AccessoryType? accessoryType
        {
            get { return _accessoryType; }
            set
            {
                if (_accessoryType != value)
                {
                    List<GameObject> list = new List<GameObject>();
                    for (int i = 0; i < defaultList[value.Value].Length; i++)
                        list.Add(AccessoryManager.GetMeshObject(defaultList[value.Value][i]));

                    itemList.items = list.ToArray();
                    itemList.onClick = (idx) =>
                    {
                        SetAccessoryAtIndex(idx);
                    };
                }
                _accessoryType = value;
            }
        }

        private void _InitAccessoryListUI()
        {
            Dictionary<AccessoryManager.AccessoryType, GameObject[]> managerList = new Dictionary<AccessoryManager.AccessoryType, GameObject[]>(AccessoryManager.list);

#if BIGBOARD_EVERLAND
        /*
        for (int i = 0; i < accessories.Count; i++)
        {
            GameObject acc = accessories[i];

            // Remove head accessories if selected character is related in Everland.
            if (mainPanel.characterSelect.selectedCharacterName.Contains("Everland_") &&
                (acc.name.Contains("Hat") || acc.name.Contains("Cap") || acc.name.Contains("Helmet")))
            {
                accessories.RemoveAt(i--);
            }
        }
        */
#endif


            defaultList = new Dictionary<AccessoryManager.AccessoryType, GameObject[]>();
            foreach (KeyValuePair<AccessoryManager.AccessoryType, GameObject[]> item in managerList)
            {
                defaultList.Add(item.Key, new GameObject[item.Value.Length]);
                for (int i = 0; i < item.Value.Length; i++)
                    defaultList[item.Key][i] = item.Value[i];
            }
        }

        public void SetAccessoryAtIndex(int index)
        {
            GameObject acc = (GameObject)Instantiate(defaultList[accessoryType.Value][index]);// accessoryPrefabs[index]);
            Dragon dragon = mainPanel.currentTemplate.GetComponent<Dragon>();
            dragon.SetAccessory(acc);

            itemList.AttachTweens(AccessoryManager.GetMeshObject(acc), 0.0f, 0.35f);
        }

        public void SelectToggle()
        {
            UIToggle[] list = toggleParent.GetComponentsInChildren<UIToggle>();
            for (int i = 0; i < list.Length; i++)
                if (list[i].value)
                {
                    AccessoryManager.AccessoryType newType = (AccessoryManager.AccessoryType)i;
                    if (newType != accessoryType)
                        accessoryType = newType;
                }
        }
    }
}