using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.EAS
{
    public class EASATool3DModelListPanel : EASAnimatablePanel
    {
        #region Public variables
        public UIScrollView scrollView;
        public UICenterOnChild centerOnChild;
        public UISprite backingSelectedModelSprite;
        public UILabel modelNameLabel;
        #endregion

        #region Properties
        private System.WeakReference _mainPanel;
        public EASAToolClient3DPanel mainPanel
        {
            get
            {
                if (_mainPanel != null)
                {
                    return _mainPanel.Target as EASAToolClient3DPanel;
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
        private string[] _modelNames = { "car", "robot" };
        #endregion

        public void Start()
        {
            centerOnChild.onCenter = (newCenterObject) =>
            {
                if (!modelNameLabel.text.Equals(selectedModelName.ToUpper()))
                {
                    modelNameLabel.text = selectedModelName.ToUpper();
                    if (onSelectedChanged != null)
                    {
                        onSelectedChanged();
                    }
                }
            };
        }

        public override void BeginShow()
        {
            base.BeginShow();
        }

        public override void EnableWidgets()
        {
            base.EnableWidgets();

            backingSelectedModelSprite.cachedGameObject.SetActive(false);
            scrollView.gameObject.SetActive(true);

            selectedIndex = 0;

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
            if (socket != null)
            {
                EASClientManager.ShowLoading();

                EASPacket packet = new EASPacket();
                packet.type = EASPacket.kType3D;
                packet.Set("data/model", selectedModelName);
                packet.Set("data/command/next_step", true);
                socket.Send(packet, (flag) =>
                {
                    EASClientManager.HideLoading();

                    if (flag)
                    {
                        mainPanel.NextStep();
                    }
                });
            }
            else
            {
                mainPanel.NextStep();
            }
        }
    }
}