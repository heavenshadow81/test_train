using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentsStoreView : MonoBehaviour
{
    #region Public variables
    public UIWidget loadingProgress;
    public UIScrollView contentsScrollView;
    public UIScrollBar contentsScrollBar;
    public UITable contentsTable;
    public UILabel messageText;
    public ContentsStoreItemDetailView detailView;
    public UIToggle[] contentTypeToggles;

    public ContentsStoreItemView itemViewPrefab;
    #endregion

    #region Private variables
    private bool _loading = false;
    
    private List<ContentsStoreItemInfo> _cachedContentsList = new List<ContentsStoreItemInfo>();
    private List<ContentsStoreItemView> _itemViews = new List<ContentsStoreItemView>();

    // The current selected content's type code
    private string _selectedType = "CC0101";
    #endregion

    #region Actions
    public void Select2DContentsList()
    {
        if (!SelectContentType(0))
        {
            _selectedType = "CC0101";
            Refresh();
        }
    }

    public void Select3DContentsList()
    {
        if (!SelectContentType(1))
        {
            _selectedType = "CC0102";
            Refresh();
        }
    }

    public void SelectTouchGameContentsList()
    {
        if (!SelectContentType(2))
        {
            _selectedType = "CC0103";
            Refresh();
        }
    }

    public void SelectInteractionGameContentsList()
    {
        if (!SelectContentType(3))
        {
            _selectedType = "CC0104";
            Refresh();
        }
    }

    public void SelectMotionGameContentsList()
    {
        if (!SelectContentType(4))
        {
            _selectedType = "CC0105";
            Refresh();
        }
    }

    public void SelectAquariumContentList()
    {
        if (!SelectContentType(5))
        {
            _selectedType = "CC0106";
            Refresh();
        }
    }

    public bool SelectContentType(int idx)
    {
        var contentTypes = BigboardServerDataManager.GetListUniqueContentsStoreItemType();
        if (contentTypes.Count > idx)
        {
            _selectedType = contentTypes[idx].typeCode;
            Refresh();

            return true;
        }

        return false;
    }
    #endregion

    #region Methods
    public void OnEnable()
    {
        // Set loading flag true so that loading progress bar should appears on Refresh().
        _loading = true;

        RefreshContentTypeTab();

        // Request the contents list to the server.
        BigboardServer.GetContentsList((conn, retCode, message) =>
        {
            _loading = false;

            bool success = conn == WWWUtil.ConnectionResult.Success && retCode.Equals("RC0000");
            if (success)
                messageText.text = "";
            else if (conn == WWWUtil.ConnectionResult.Success)
                messageText.text = message;
            else
                messageText.text = "연결을 실패하였습니다.";

            _cachedContentsList.Clear();
            _cachedContentsList.AddRange(BigboardServerDataManager.GetListAllContentsStoreItemInfo());

            Refresh();
            //Select2DContentsList();
        });

        Refresh();
    }

    public void OnDisable()
    {
        _cachedContentsList.Clear();
    }

    public void RefreshContentTypeTab()
    {
        List<ContentsStoreItemType> types = BigboardServerDataManager.GetListUniqueContentsStoreItemType();

        for (int i = 0; i < contentTypeToggles.Length; i++)
        {
            UIToggle toggle = contentTypeToggles[i];
            if (toggle != null)
            {
                if (types.Count > i)
                {
                    toggle.value = types[i].typeCode.Equals(_selectedType);
                }

                // Change the title of the toggle
                UILabel label = toggle.GetComponentInChildren<UILabel>();
                if (label != null)
                {
                    if (!_loading)
                    {
                        if (types.Count > i)
                            label.text = types[i].typeName;
                    }
                }
            }
        }
    }

    public void Refresh()
    {
        RefreshContentTypeTab();

        if (_loading)
        {
            // Shows the loading progress bar only
            contentsScrollView.gameObject.SetActive(false);
            contentsScrollBar.gameObject.SetActive(false);
            loadingProgress.gameObject.SetActive(true);
            messageText.cachedGameObject.SetActive(false);
        }
        else
        {
            // Hides the loading progress bar. If there's any messages which have to be shown, shows the message label.
            loadingProgress.gameObject.SetActive(false);
            if(messageText.text.Length > 0) messageText.cachedGameObject.SetActive(true);
            contentsScrollView.gameObject.SetActive(true);
            contentsScrollBar.gameObject.SetActive(true);

            // Filter contents using the selected content type
            List<ContentsStoreItemInfo> itemInfos = new List<ContentsStoreItemInfo>();
            for (int i = 0; i < _cachedContentsList.Count; i++)
            {
                ContentsStoreItemInfo info = _cachedContentsList[i];
                if (info.type.typeCode.Equals(_selectedType))
                {
                    itemInfos.Add(info);
                }
            }

            // Refresh the item views
            for (int i = 0; i < Mathf.Max(itemInfos.Count, _itemViews.Count); i++)
            {
                if (_itemViews.Count > i)
                {
                    // Reuse the game object
                    if (itemInfos.Count > i)
                    {
                        ContentsStoreItemView item = _itemViews[i];
                        item.name = string.Format("Item #{0}", i + 1);
                        item.item = itemInfos[i];
                    }
                    else
                    {
                        // Destroy the unnecessary game object
                        ContentsStoreItemView item = _itemViews[i];
                        _itemViews.RemoveAt(i);
                        NGUITools.Destroy(item.gameObject);
                        i--;
                    }
                }
                else
                {
                    // Make the new game object
                    ContentsStoreItemView newItem = NGUITools.AddChild(contentsTable.gameObject, itemViewPrefab.gameObject).GetComponent<ContentsStoreItemView>();
                    newItem.name = string.Format("Item #{0}", i + 1);
                    newItem.item = itemInfos[i];
                    newItem.gameObject.SetActive(true);
                    _itemViews.Add(newItem);
                }
            }

            contentsTable.Reposition();
            contentsScrollView.ResetPosition();
        }
    }

    public void ShowItemDetail(ContentsStoreItemInfo item)
    {
        if (detailView != null)
        {
            detailView.item = item;
            detailView.Show();
        }
    }
    #endregion
}
