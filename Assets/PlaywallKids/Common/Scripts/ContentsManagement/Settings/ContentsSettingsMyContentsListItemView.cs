using UnityEngine;
using System.Collections;

public class ContentsSettingsMyContentsListItemView : MonoBehaviour
{
    #region Public variables
    public UITexture iconTex;
    public UILabel titleLabel;
    public UILabel priceLabel;
    public UILabel dateLabel;
    #endregion

    #region Properties
    private ContentsStoreItemInfo _item;
    public ContentsStoreItemInfo item
    {
        get
        {
            return _item;
        }
        set
        {
            _item = value;

            Refresh();
        }
    }
    #endregion

    #region Methods
    public void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        System.Globalization.CultureInfo korean = new System.Globalization.CultureInfo("ko-KR", false);

        if (_item != null)
        {
            titleLabel.text = _item.name;
            priceLabel.text = _item.price > 0 ? string.Format(korean, "{0:C}", _item.price) : "무료";
            dateLabel.text = _item.regDate.ToString("yyyy.M.d.");
        }
        else
        {
            titleLabel.text = "?";
            priceLabel.text = "";
            dateLabel.text = "";
        }
    }
    #endregion
}
