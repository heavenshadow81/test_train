using UnityEngine;
using System.Collections.Generic;

public class ContentsSettingsView : MonoBehaviour
{
    #region Public variables

    // My contents view
    public ContentsSettingsMyContentsListView myContentsListView;

    // Containers
    public UIWidget contentsTypesSummaryContainer;
    public UIWidget contentsSequencesContainer;

    // Loading, Message
    public UILabel messageLabel;

    // Sequence views
    public UIScrollView contentsSequenceScrollView;
    public UIGrid contentsSequenceGrid;

    // Subsequence Views
    public UITable subsequenceTabTable;
    public UIScrollView ownedContentsScrollView;
    public UITable ownedContentsTable;
    public UIScrollView appliedContentsScrollView;
    public UITable appliedContentsTable;


    public UIButton subsequenceMoveLeftButton, subsequenceMoveRightButton;

    // prefabs
    public ContentsSettingsSequenceItemView sequenceItemViewPrefab;
    public ContentsSettingsSubsequenceItemView subsequenceItemViewPrefab;
    #endregion

    #region Private variables
    // Cached data from server
    private List<ContentsStoreItemType.Mode> _cachedMainOrder = new List<ContentsStoreItemType.Mode>();
    private Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>> _cachedSubOrder = new Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>>();

    // Current configured data
    private Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>> _currentSubOrder = new Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>>();

    // Reference game object
    private ContentsSettingsSequenceItemView _referenceSubsequenceTabView;

    // Selected subsequence tab info
    private ContentsStoreItemType.Mode _selectedSubsequenceTabInfo;

    // Request codes
    public int _getConfigRequestCode = 0;
    public int _setConfigRequestCode = 0;
    #endregion

    public void OnEnable()
    {

        if(myContentsListView.gameObject != null)
            myContentsListView.gameObject.SetActive(false);

        contentsTypesSummaryContainer.cachedGameObject.SetActive(true);
        contentsSequencesContainer.cachedGameObject.SetActive(false);

        _cachedMainOrder.Clear();
        _cachedSubOrder.Clear();

        BigboardAlertPanel.sharedInstance.Loading(true);
        messageLabel.cachedGameObject.SetActive(true);
        messageLabel.text = "";

        _getConfigRequestCode = BigboardServer.GetConfig((conn, returnCode, message, mainOrder, subOrder) =>
        {
            _getConfigRequestCode = 0;
            BigboardAlertPanel.sharedInstance.Loading(false);

            if (conn == WWWUtil.ConnectionResult.Success)
            {
                if (returnCode.Equals("RC0000"))
                {
                    contentsSequencesContainer.gameObject.SetActive(true);
                    messageLabel.text = "";

                    _cachedMainOrder = mainOrder;
                    _cachedSubOrder = subOrder;


                    _RevertCurrentSubOrderState();

                    Refresh();
                }
                else
                {
                    messageLabel.text = message;
                }
            }
            else if(conn != WWWUtil.ConnectionResult.Cancel)
            {
                messageLabel.text = "환경설정을 가져오지 못했습니다";
            }
        });
    }

    public void OnDisable()
    {
        if (_getConfigRequestCode > 0)
        {
            BigboardServer.CancelRequest(_getConfigRequestCode);
            _getConfigRequestCode = 0;
        }
        if (_setConfigRequestCode > 0)
        {
            BigboardServer.CancelRequest(_setConfigRequestCode);
            _setConfigRequestCode = 0;
        }

        _selectedSubsequenceTabInfo = ContentsStoreItemType.Mode.None;
    }

    public void Refresh()
    {
        ReloadContentsSequence();
        ReloadSubsequenceTabs();

        // ReloadContentsSubsequence() will be called by ReloadSubsequenceTabs().
    }

    public void ReloadContentsSequence()
    {
        List<Transform> sequence = contentsSequenceGrid.GetChildList();

        // Re-arrange the items. Make new item or destroy game objects if necessary.
        for (int i = 0; i < _cachedMainOrder.Count; i++)
        {
            ContentsSettingsSequenceItemView item = null;
            if (sequence.Count > i)
            {
                Transform t = sequence[i];
                item = t.GetComponent<ContentsSettingsSequenceItemView>();
                if (item == null)
                {
                    Destroy(t.gameObject);
                    sequence.RemoveAt(i--);
                    continue;
                }
            }
            else
            {
                item = NGUITools.AddChild(contentsSequenceGrid.gameObject, sequenceItemViewPrefab.gameObject).GetComponent<ContentsSettingsSequenceItemView>();
                item.gameObject.SetActive(true);
                if (sequence.Count > 0)
                    item.transform.localPosition = sequence[sequence.Count - 1].transform.localPosition + Vector3.right * 100.0f;
                sequence.Add(item.transform);
            }

            if (item != null)
            {
                List<ContentsStoreItemInfo> list = BigboardServerDataManager.GetListAllContentsStoreItemInfo(_cachedMainOrder[i]);
                if (list.Count > 0)
                {
                    item.type = _cachedMainOrder.Count > i ? list[0].type : null;
                    item.name = _cachedMainOrder.Count > i ? string.Format("Item({0})", list[0].type.typeCode) : "Item_null";
                }
            }
        }

        // Re-position sequence items
        if (contentsSequenceGrid.onReposition == null)
        {
            contentsSequenceGrid.onReposition = () =>
            {
                contentsSequenceScrollView.ResetPosition();
            };
        }
        contentsSequenceGrid.repositionNow = true;
    }

    public void ReloadContentSubsequence()
    {
        List<Transform> ownedViews = ownedContentsTable.GetChildList();
        List<Transform> appliedViews = appliedContentsTable.GetChildList();

        List<ContentsStoreItemInfo> contents = BigboardServerDataManager.GetListMyContentsStoreItemInfo(_selectedSubsequenceTabInfo);
        //if (_currentSubOrder.ContainsKey(_selectedSubsequenceTabInfo))
        //    contents = _currentSubOrder[_selectedSubsequenceTabInfo];

        // split the list to owned, applied content list
        List<ContentsStoreItemInfo> ownedContentsInfos = new List<ContentsStoreItemInfo>();
        List<ContentsStoreItemInfo> appliedContentsInfos = new List<ContentsStoreItemInfo>();
        for (int i = 0; i < contents.Count; i++)
        {
            // true : applied, false : owned but not applied
            if (contents[i].useYn)
                appliedContentsInfos.Add(contents[i]);
            else
                ownedContentsInfos.Add(contents[i]);
        }

        _ReloadSubsequenceItemList(ownedContentsScrollView, ownedContentsTable, ownedViews, ownedContentsInfos);
        _ReloadSubsequenceItemList(appliedContentsScrollView, appliedContentsTable, appliedViews, appliedContentsInfos);

        ShowMoveLeftOrRightButton();
    }

    private void _ReloadSubsequenceItemList(UIScrollView scrollView, UITable table, List<Transform> prevViews, List<ContentsStoreItemInfo> list)
    {
        for (int i = 0; i < Mathf.Max(prevViews.Count, list.Count); i++)
        {
            ContentsSettingsSubsequenceItemView itemView = null;
            if (prevViews.Count > i)
            {
                itemView = prevViews[i].GetComponent<ContentsSettingsSubsequenceItemView>();
                if (itemView != null && list.Count > i)
                {
                    itemView.info = list[i];
                    itemView.name = string.Format("Item_{0:000000}", i + 1);
                }
                else
                {
                    Destroy(prevViews[i].gameObject);
                    prevViews.RemoveAt(i--);
                    itemView = null;
                }
            }
            else
            {
                itemView = NGUITools.AddChild(table.gameObject, subsequenceItemViewPrefab.gameObject).GetComponent<ContentsSettingsSubsequenceItemView>();
                itemView.transform.localPosition = Vector3.down * i * 20.0f;
                if (prevViews.Count > 0)
                    itemView.transform.localPosition += prevViews[prevViews.Count - 1].transform.localPosition;
                itemView.gameObject.SetActive(true);
                itemView.name = string.Format("Item_{0:000000}", i + 1);
                itemView.info = list[i];
            }

            if (itemView != null)
            {
                UIToggle toggle = itemView.GetComponent<UIToggle>();
                if (toggle != null)
                {
                    if (table == ownedContentsTable) toggle.group = 176;
                    else if (table == appliedContentsTable) toggle.group = 177;
                    toggle.Set(false);
                }
            }
        }

        table.Reposition();
        scrollView.ResetPosition();
    }

    private void ReloadSubsequenceTabs()
    {
        // Find the reference game object if it is not initialized
        List<Transform> tabs = subsequenceTabTable.GetChildList();
        for (int i = 0; i < tabs.Count; i++)
        {
            Transform t = tabs[i];
            var item = t.GetComponent<ContentsSettingsSequenceItemView>();
            if (item == null || _referenceSubsequenceTabView != null)
            {
                Destroy(t.gameObject);
                tabs.RemoveAt(i--);
            }
            else
            {
                _referenceSubsequenceTabView = item;
                _referenceSubsequenceTabView.name = "__Reference(SubsequenceTab)";
                _referenceSubsequenceTabView.gameObject.SetActive(false);
            }
        }

        // And reset the tabs
        if (_referenceSubsequenceTabView != null)
        {
            List<ContentsStoreItemType> list = BigboardServerDataManager.GetListUniqueContentsStoreItemType();
            list.Sort((a, b) =>
            {
                return a.typeCode.CompareTo(b.typeCode);
            });

            for (int i = 0; i < list.Count; i++)
            {
                var tab = NGUITools.AddChild(subsequenceTabTable.gameObject, _referenceSubsequenceTabView.gameObject).GetComponent<ContentsSettingsSequenceItemView>();
                tab.name = string.Format("Tab_{0}", i + 1);
                tab.gameObject.SetActive(true);
                tab.type = list[i];

                UIToggle toggle = tab.GetComponent<UIToggle>();
                if (toggle != null) {
                    toggle.onChange.Clear();
                    toggle.value = i == 0;
                    toggle.onChange.Add(new EventDelegate(() =>
                    {
                        if (toggle.value)
                        {
                            _SaveCurrentSubOrderState();
                            _selectedSubsequenceTabInfo = tab.type.mode;
                            ReloadContentSubsequence();
                        }
                    }));
                }
            }

            subsequenceTabTable.repositionNow = true;
        }
    }

    private void _SaveCurrentSubOrderState()
    {
        if (_selectedSubsequenceTabInfo != ContentsStoreItemType.Mode.None && _currentSubOrder.ContainsKey(_selectedSubsequenceTabInfo))
        {
            List<ContentsStoreItemInfo> original = _currentSubOrder[_selectedSubsequenceTabInfo];
            List<ContentsStoreItemInfo> list = new List<ContentsStoreItemInfo>();

            List<Transform> ownedViews = ownedContentsTable.GetChildList();
            List<Transform> appliedViews = appliedContentsTable.GetChildList();

            for (int i = 0; i < appliedViews.Count; i++)
            {
                ContentsSettingsSubsequenceItemView itemView = appliedViews[i].GetComponent<ContentsSettingsSubsequenceItemView>();
                if (itemView != null && itemView.info != null)
                {
                    var info = itemView.info;

                    // change the item info's usage
                    info.useYn = true;

                    // add to list
                    list.Add(info);
                }
            }

            for (int i = 0; i < ownedViews.Count; i++)
            {
                ContentsSettingsSubsequenceItemView itemView = ownedViews[i].GetComponent<ContentsSettingsSubsequenceItemView>();
                if (itemView != null && itemView.info != null)
                {
                    var info = itemView.info;

                    // change the item info's usage
                    info.useYn = false;

                    // add to list
                    list.Add(info);
                }
            }

            if (original.Count == list.Count)
            {
                _currentSubOrder[_selectedSubsequenceTabInfo] = list;
            }
            else
            {
                // Something went wrong!
                DebugUtil.DLog("ContentsSettingsView._SaveCurrentSubOrderState() : Failed to save the state.");
            }
        }
    }

    private void _RevertCurrentSubOrderState()
    {
        _currentSubOrder.Clear();

        foreach (var key in _cachedSubOrder.Keys)
        {
            List<ContentsStoreItemInfo> list = new List<ContentsStoreItemInfo>();

            List<ContentsStoreItemInfo> cachedList = _cachedSubOrder[key];
            foreach (var item in cachedList)
            {
                list.Add(item.Clone());
            }

            _currentSubOrder[key] = list;
        }
    }

    public void Save()
    {
        if (_setConfigRequestCode > 0) return;

        _SaveCurrentSubOrderState();

        List<ContentsStoreItemType.Mode> currentMainOrder = new List<ContentsStoreItemType.Mode>();
        List<Transform> sequence = contentsSequenceGrid.GetChildList();
        for (int i = 0; i < sequence.Count; i++)
        {
            Transform t = sequence[i];
            ContentsSettingsSequenceItemView itemView = t.GetComponent<ContentsSettingsSequenceItemView>();
            if (itemView != null && itemView.type != null)
                currentMainOrder.Add(itemView.type.mode);
        }


        BigboardAlertPanel.sharedInstance.Loading(true);

        BigboardServer.SetConfig(currentMainOrder, _currentSubOrder, (conn, retCode, message, flag) =>
        {
            if (conn != WWWUtil.ConnectionResult.Cancel)
            {
                BigboardAlertPanel.sharedInstance.Loading(false);

                if (flag)
                {
                    _cachedMainOrder = currentMainOrder;
                    _cachedSubOrder = _currentSubOrder;
                    _currentSubOrder = new Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>>();
                    _RevertCurrentSubOrderState();

                    BigboardAlertPanel.sharedInstance.Show(BigboardAlertPanel.ButtonType.OneButton, "저장을 완료하였습니다.");
                }
                else if(message.Length > 0)
                    BigboardAlertPanel.sharedInstance.Show(BigboardAlertPanel.ButtonType.OneButton, string.Format("저장을 실패하였습니다. ({0})", message));
            }
        });

    }

    public void Cancel()
    {
        _RevertCurrentSubOrderState();

        ReloadContentsSequence();
        ReloadContentSubsequence();
    }

    public void ShowMoveLeftOrRightButton()
    {
        UIToggle leftItem = UIToggle.GetActiveToggle(176);
        UIToggle rightItem = UIToggle.GetActiveToggle(177);

        subsequenceMoveRightButton.isEnabled = leftItem != null;
        subsequenceMoveLeftButton.isEnabled = rightItem != null;
    }

    public void SubsequenceMoveLeft()
    {
        if (ThemeAndContentsTypeCheck())
            return;

        UIToggle activeToggle = UIToggle.GetActiveToggle(177);
        if (activeToggle != null)
        {
            ContentsSettingsSubsequenceItemView itemView = activeToggle.GetComponent<ContentsSettingsSubsequenceItemView>();
            if (itemView != null)
            {
                // Ignore already dragging item(s)
                if (NGUITools.FindInParents<UIDragDropRoot>(activeToggle.gameObject) == null)
                {
                    activeToggle.group = 176;
                    activeToggle.Set(false);

                    itemView.transform.parent = ownedContentsTable.transform;
                    itemView.transform.localPosition = new Vector3(0.0f, -10000.0f);
                    NGUITools.MarkParentAsChanged(itemView.gameObject);
                    ownedContentsTable.repositionNow = true;
                    appliedContentsTable.repositionNow = true;

                    ShowMoveLeftOrRightButton();
                }
            }
        }
    }

    public void SubsequenceMoveRight()
    {
        if (ThemeAndContentsTypeCheck())
            return;

        UIToggle activeToggle = UIToggle.GetActiveToggle(176);
        if (activeToggle != null)
        {
            ContentsSettingsSubsequenceItemView itemView = activeToggle.GetComponent<ContentsSettingsSubsequenceItemView>();
            if (itemView != null)
            {
                // Ignore already dragging item(s)
                if (NGUITools.FindInParents<UIDragDropRoot>(activeToggle.gameObject) == null)
                {
                    activeToggle.group = 177;
                    activeToggle.Set(false);

                    itemView.transform.parent = appliedContentsTable.transform;
                    itemView.transform.localPosition = new Vector3(0.0f, -10000.0f);
                    NGUITools.MarkParentAsChanged(itemView.gameObject);
                    ownedContentsTable.repositionNow = true;
                    appliedContentsTable.repositionNow = true;

                    ShowMoveLeftOrRightButton();
                }
            }
        }
    }

    public bool ThemeAndContentsTypeCheck()
    {
        if (BigboardServer.cachedSituationalInfo.theme != ContentsSIModelingInfo.StateTheme.None)
        {
            BigboardAlertPanel.sharedInstance.Show(BigboardAlertPanel.ButtonType.OneButton, "테마 선택시 적용콘텐츠 설정이 불가능합니다.");
            return true;
        }

        if (BigboardServer.cachedSituationalInfo.contentsType != ContentsSIModelingInfo.StateContentsType.None)
        {
            BigboardAlertPanel.sharedInstance.Show(BigboardAlertPanel.ButtonType.OneButton, "콘텐츠 타입 선택시 적용콘텐츠 설정이 불가능합니다.");
            return true;
        }

        return false;
    }
}
