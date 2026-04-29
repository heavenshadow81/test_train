using UnityEngine;
using System.Collections.Generic;

public class ContentsSettingsContentTypeSummaryView : MonoBehaviour
{
    #region Public variables
    public int idx = 0;
    public UILabel titleLabel;
    public UILabel countLabel;
    #endregion

    #region Properties
    public ContentsStoreItemType type
    {
        get
        {
            ContentsStoreItemType type = null;

            List<ContentsStoreItemType> list = BigboardServerDataManager.GetListUniqueContentsStoreItemType();
            if (list.Count > idx)
            {
                type = list[idx];
            }

            return type;
        }
    }
    #endregion

    #region Methods
    public void OnEnable()
    {
        if (type != null)
        {
            titleLabel.text = type.typeName;
            countLabel.text = BigboardServerDataManager.GetListMyContentsStoreItemInfo(type.mode).Count.ToString();
        }
        else
        {
            Debug.Log(BigboardServerDataManager.GetListUniqueContentsStoreItemType().Count);
            titleLabel.text = BigboardServerDataManager.GetListUniqueContentsStoreItemType()[idx-1].typeName;
            countLabel.text = "0";// idx + " - ?";
        }
    }

    public void ShowItemList()
    {
        ContentsSettingsView settingsView = GetComponentInParent<ContentsSettingsView>();
        ContentsStoreItemType itemType = type;

        if (settingsView != null && itemType != null)
        {
            settingsView.myContentsListView.Show(itemType, gameObject);
        }
    }
    #endregion
}
