using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Dragon Park Free-Drawing Panel
    /// Step 1: Model selection
    /// </summary>
    public class ATool3DModelList : AnimatablePanel
    {
        #region Public variables
        public UIScrollView scrollView;
        public UICenterOnChild centerOnChild;
        public UISprite backingSelectedModelSprite;
        public UILabel modelNameLabel;
        #endregion
        #region Properties
        private System.WeakReference _mainPanel;
        public ATool3DPanel mainPanel
        {
            get
            {
                if (_mainPanel != null)
                {
                    return _mainPanel.Target as ATool3DPanel;
                }

                return null;
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

        private UISprite[] _items;
        public UISprite[] items
        {
            get
            {
                if (_items == null || _items.Length == 0)
                {
                    _items = centerOnChild.GetComponentsInChildren<UISprite>();
                }
                return _items;
            }
        }

        public int selectedIndex
        {
            get
            {
                for (int i = 0, cnt = items.Length; i < cnt; i++)
                {
                    if (items[i] != null && items[i].cachedGameObject == centerOnChild.centeredObject)
                    {
                        return i;
                    }
                }
                return -1;
            }
            set
            {
                if (value >= 0 && value < items.Length)
                {
                    var item = items[value];
                    if (!item.cachedGameObject.activeInHierarchy) item.cachedGameObject.SetActive(true);
                    centerOnChild.CenterOn(item.cachedTransform);
                }

                if (onSelectedChanged != null)
                {
                    onSelectedChanged();
                }
            }
        }

        public string selectedModelName
        {
            get
            {
                if (_modelNames.Length > selectedIndex && selectedIndex > -1)
                {
                    return _modelNames[selectedIndex];
                }
                else
                {
                    return "?";
                }
            }
            set
            {
                int idx = IndexOf(value);
                if (idx > -1)
                {
                    selectedIndex = idx;
                }
            }
        }

        public System.Action onSelectedChanged = null;
        #endregion

        #region Private variables
        private string[] _modelNames = { "car", "robot", "airplane" };
        #endregion

        public void Start()
        {
            centerOnChild.onCenter = (newCenterObject) =>
            {
            // Select appropriate localization of model.
            LocalizationKey localizationKey = LocalizationKey.NONE;
                switch (selectedModelName)
                {
                    case "car": localizationKey = LocalizationKey.DRAWING3D_FREEDRAWING_CAR; break;
                    case "robot": localizationKey = LocalizationKey.DRAWING3D_FREEDRAWING_ROBOT; break;
                    case "airplane": localizationKey = LocalizationKey.DRAWING3D_FREEDRAWING_AIRPLANE; break;
                }
                if (localizationKey != LocalizationKey.NONE)
                    modelNameLabel.text = LocalizationManager.GetData(localizationKey);
                else
                    modelNameLabel.text = selectedModelName.ToUpper();

            // Calls callback
            if (onSelectedChanged != null)
                    onSelectedChanged();
            };
        }

        void OnEnable()
        {
            for (int i = 1; i < items.Length; ++i)
            {
                items[i].cachedGameObject.SetActive(false);
            }
        }

        public override void BeginShow()
        {
            base.BeginShow();

            selectedIndex = 0;
        }

        public override void EnableWidgets()
        {
            base.EnableWidgets();

            backingSelectedModelSprite.cachedGameObject.SetActive(false);
            scrollView.gameObject.SetActive(true);

            RefreshInnerButtonActiveState();
        }

        public override void DisableWidgets()
        {
            base.DisableWidgets();

            _SetBackingSelectedModelSpriteContent();

            backingSelectedModelSprite.cachedGameObject.SetActive(true);
            scrollView.gameObject.SetActive(false);
        }

        private void _SetBackingSelectedModelSpriteContent()
        {
            GameObject centeredObject = centerOnChild.centeredObject;
            if (centeredObject != null)
            {
                UISprite background = centeredObject.GetComponent<UISprite>();
                if (background == null)
                {
                    background = centeredObject.GetComponentInChildren<UISprite>();
                }

                if (background != null)
                {
                    backingSelectedModelSprite.atlas = background.atlas;
                    backingSelectedModelSprite.spriteName = background.spriteName;
                }
                else
                {
                    backingSelectedModelSprite.atlas = null;
                    backingSelectedModelSprite.spriteName = "";
                }
            }
        }

        public int IndexOf(string modelName)
        {
            int idx = -1;
            for (int i = 0, cnt = _modelNames.Length; i < cnt; i++)
            {
                if (_modelNames[i].Equals(modelName))
                {
                    idx = i;
                    break;
                }
            }
            return idx;
        }

        public void SelectPrevModel()
        {
            selectedIndex = (selectedIndex + items.Length - 1) % items.Length;
        }

        public void SelectNextModel()
        {
            selectedIndex = (selectedIndex + 1) % items.Length;
        }

        public void NextStep()
        {
            mainPanel.NextStep();
        }
    }
}