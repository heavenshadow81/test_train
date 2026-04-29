using UnityEngine;
using System.Collections.Generic;

public class ContentsSettingsMyContentsListView : MonoBehaviour
{
    #region Public variables
    // title
    public UILabel titleLabel;

    // message
    public UILabel messageLabel;

    // arrow
    public UISprite arrow;

    // scroll view
    public UIScrollView scrollView;
    public UIGrid grid;

    // prefab
    public ContentsSettingsMyContentsListItemView itemViewPrefab;
    #endregion

    #region Private variables
    private ContentsStoreItemType _contentType;

    private List<ContentsSettingsMyContentsListItemView> _itemViewList = new List<ContentsSettingsMyContentsListItemView>();
    #endregion

    #region Methods
    public void Show(ContentsStoreItemType type, GameObject sender = null)
    {
        _contentType = type;

        if (sender != null && arrow != null)
        {
            Vector3 pos = arrow.cachedTransform.position;
            pos.y = sender.transform.position.y;
            arrow.cachedTransform.position = pos;
        }

        gameObject.SetActive(true);

        Refresh();
    }

    public void Refresh()
    {
        if (_contentType == null) return;
        
        titleLabel.text = string.Format(LocalizationManager.GetData(LocalizationKey.SETTING_CONTENTSSETTING_CONTENTS), _contentType.typeName);

        List<ContentsStoreItemInfo> list = BigboardServerDataManager.GetListMyContentsStoreItemInfo(_contentType.mode);

        messageLabel.text = list.Count > 0 ? "" : LocalizationManager.GetData(LocalizationKey.SETTING_CONTENTSSETTING_EMPTYCONTENTS);

        for (int i = 0; i < Mathf.Max(list.Count, _itemViewList.Count); i++)
        {
            if (_itemViewList.Count > i)
            {
                if (list.Count > i)
                {
                    // Reuse
                    ContentsSettingsMyContentsListItemView itemView = _itemViewList[i];
                    ContentsStoreItemInfo item = list[i];
                    itemView.name = string.Format("Item_{0:000000}", item.seq);
                    itemView.item = item;
                }
                else
                {
                    // Destroy
                    ContentsSettingsMyContentsListItemView itemView = _itemViewList[i];
                    _itemViewList.RemoveAt(i);
                    NGUITools.Destroy(itemView.gameObject);
                    i--;
                }
            }
            else
            {
                // Make new instance
                ContentsSettingsMyContentsListItemView itemView = NGUITools.AddChild(grid.gameObject, itemViewPrefab.gameObject).GetComponent<ContentsSettingsMyContentsListItemView>();
                ContentsStoreItemInfo item = list[i];
                itemView.item = item;
                itemView.name = string.Format("Item_{0:000000}", item.seq);
                itemView.gameObject.SetActive(true);
                _itemViewList.Add(itemView);
            }
        }

        // Reset the grid and the scroll view position
        grid.Reposition();
        scrollView.ResetPosition();
    }
    #endregion
}
