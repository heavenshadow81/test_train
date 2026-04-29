using UnityEngine;
using System.Collections.Generic;

public class ContentsSettingsSubsequenceItemView : UIDragDropItem
{
    #region Public variables
    public UILabel titleLabel;
    #endregion

    #region Properties
    private ContentsStoreItemInfo _info;
    public ContentsStoreItemInfo info
    {
        get
        {
            return _info;
        }
        set
        {
            _info = value;

            Refresh();
        }
    }
    #endregion

    #region Methods
    protected override void Start()
    {
        base.Start();

        ContentsSettingsView settingsView = GetComponentInParent<ContentsSettingsView>();
        if (settingsView != null)
        {
            UIToggle toggle = GetComponent<UIToggle>();
            if (toggle != null)
                toggle.onChange.Add(new EventDelegate(() =>
                {
                    settingsView.ShowMoveLeftOrRightButton();
                }));
        }
    }

    public void Refresh()
    {
        if (_info != null)
            titleLabel.text = _info.name;
        else
            titleLabel.text = "?";
    }

    protected override void OnDragDropStart()
    {
        base.OnDragDropStart();

        TweenScale.Begin(gameObject, 0.15f, Vector3.one * 1.5f);
    }

    protected override void OnDragDropRelease(GameObject surface)
    {

        TweenScale scale = GetComponent<TweenScale>();
        if (scale != null) scale.enabled = false;
        transform.localScale = Vector3.one;


        ContentsSettingsView settingsView = GetComponentInParent<ContentsSettingsView>();
        if (settingsView.ThemeAndContentsTypeCheck())
        {
            base.OnDragDropRelease(null);
            return;
        }

        int toggleGroup = 0;

        UIToggle toggle = GetComponent<UIToggle>();
        if (toggle != null) toggleGroup = toggle.group;

        // 176 : left
        // 177 : right
        if (toggleGroup == 176 || toggleGroup == 177)
        {
            if (surface != null)
            {
                UIDragDropContainer container = NGUITools.FindInParents<UIDragDropContainer>(surface);

                if (container != null)
                {
                    if ((container.name.Contains("Left") || container.name.Contains("Right")))
                    {
                        if (toggle != null)
                        {
                            if (container.name.Contains("Left")) toggle.group = 176;
                            else toggle.group = 177;

                            toggle.Set(false);
                        }

                        // The value will be changed in ContentsSettingsView._SaveCurrentSubOrderState()
                        /*
                        if (info != null)
                        {
                            if (container.name.Contains("Left"))
                                info.useYn = false;
                            else
                                info.useYn = true;
                        }
                        */
                            
                        base.OnDragDropRelease(surface);
                    }
                    else
                    {
                        base.OnDragDropRelease(null);
                    }
                }
                else
                {
                    base.OnDragDropRelease(null);
                }
            }
            else
            {
                base.OnDragDropRelease(null);
            }
        }
        else
        {
            base.OnDragDropRelease(null);
        }
    }
    #endregion
}
