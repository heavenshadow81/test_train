using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContentsStoreItemDetailView : AnimatablePanel
{
    #region Public variables
    public UIScrollView scrollView;
    public UITable table;
    public UILabel titleText;
    public UILabel dateText;
    public UIButton buyButton;
    public UILabel buyButtonLabel;
    public UILabel priceText;
    public UILabel descriptionText;
    public UIButton buyButtonBottom;
    public UILabel buyButtonBottomLabel;
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
    public override void BeginShow()
    {
        base.BeginShow();
        scrollView.gameObject.SetActive(false);
    }

    public override void Active()
    {
        base.Active();
        scrollView.gameObject.SetActive(true);
    }

    public void Refresh()
    {
        if (item != null)
        {
            System.Globalization.CultureInfo korean = new System.Globalization.CultureInfo("ko-KR", false);
            titleText.text = item.name;
            dateText.text = string.Format("{0:yyyy. M. d.}", item.regDate);
            priceText.text = string.Format(korean, "{0:C}", item.price);
            if (item.IsBuy())
            {
                buyButton.isEnabled = buyButtonBottom.isEnabled = false;
                buyButtonLabel.text = buyButtonBottomLabel.text = LocalizationManager.GetData(LocalizationKey.SETTING_PUBLIC_PURCHASECOMPLETE);
            }
            else
            {
                buyButton.isEnabled = buyButtonBottom.isEnabled = true;
                if (item.price < 0.00001)
                    buyButtonLabel.text = buyButtonBottomLabel.text = "무료";
                else
                    buyButtonLabel.text = buyButtonBottomLabel.text = LocalizationManager.GetData(LocalizationKey.SETTING_PUBLIC_PURCHASE); ;
            }
            descriptionText.text = "\n" + item.description + "\n";
        }
        else
        {
            // fallback
            titleText.text = "?";
            dateText.text = "-";
            priceText.text = "0";
            buyButton.isEnabled = buyButtonBottom.isEnabled = false;
            buyButtonLabel.text = buyButtonBottomLabel.text = "";
            descriptionText.text = "\n???\n";
        }

        table.Reposition();
        scrollView.ResetPosition();
    }

    public void Buy()
    {
        if (item != null)
        {
            if (item.price > 0.0f)
            {
                // Consumable item
                BigboardAlertPanel.sharedInstance.Show(BigboardAlertPanel.ButtonType.OneButton, "지금은 과금 콘텐츠를 구매할 수 없습니다.");
            }
            else
            {
                // Free item
                buyButton.isEnabled = buyButtonBottom.isEnabled = false;
                buyButtonLabel.text = buyButtonBottomLabel.text = "처리중";
                DisableWidgets();

                BigboardServer.Buy(item, (conn, retCode, message, success) =>
                {
                    EnableWidgets();

                    if (conn == WWWUtil.ConnectionResult.Success && success)
                    {
                        BigboardAlertPanel.sharedInstance.Show(BigboardAlertPanel.ButtonType.OneButton, "아이템 구매를 성공하였습니다.");
                    }
                    else
                    {
                        BigboardAlertPanel.sharedInstance.Show(BigboardAlertPanel.ButtonType.OneButton, message);
                    }

                    Refresh();
                });
            }
        }
        else
        {
            BigboardAlertPanel.sharedInstance.Show(BigboardAlertPanel.ButtonType.OneButton, "아이템을 조회할 수 없습니다.");
        }
    }
    #endregion
}
