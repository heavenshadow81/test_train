using UnityEngine;
using System.Collections.Generic;

public class ContentsSettingsSequenceItemView : UIDragDropItem
{
    #region Public variables
    public UILabel titleLabel;
    #endregion

    #region Properties
    private ContentsStoreItemType _type;
    public ContentsStoreItemType type
    {
        get
        {
            return _type;
        }
        set
        {
            _type = value;
            Refresh();
        }
    }
    #endregion

    public void Refresh()
    {
        if (_type != null)
            titleLabel.text = _type.typeName;
        else
            titleLabel.text = "?";
    }

    protected override void OnDragDropRelease(GameObject surface)
    {
        if (surface != null)
        {
            UIDragDropContainer container = NGUITools.FindInParents<UIDragDropContainer>(surface);
            if (container != null && container.name.Contains("Sequence"))
                base.OnDragDropRelease(surface);
            else
                base.OnDragDropRelease(null);
        }
        else
            base.OnDragDropRelease(surface);
    }
}