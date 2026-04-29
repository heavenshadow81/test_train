using UnityEngine;
using System.Collections;

public class ContentsStoreItemView : MonoBehaviour
{
    #region Public variables
    public UITexture iconUITex;
    public UILabel titleText;
    public UILabel descriptionText;
    public UILabel priceText;
    public UIWidget newMark;
    public UIButton buyButton;
    public UILabel buyButtonLabel;

    public Texture2D noimageTex;
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

    public void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        System.Globalization.CultureInfo korean = new System.Globalization.CultureInfo("ko-KR", false);

        if (item != null)
        {
            iconUITex.mainTexture = noimageTex;
            titleText.text = item.name;
            descriptionText.text = item.descriptionShort;
            priceText.text = string.Format(korean, "{0:C}", item.price);
            newMark.cachedGameObject.SetActive(true);
            if (item.IsBuy())
            {
                buyButton.isEnabled = false;
                buyButtonLabel.text = LocalizationManager.GetData(LocalizationKey.SETTING_PUBLIC_PURCHASECOMPLETE);
            }
            else
            {
                buyButton.isEnabled = true;
                if (item.price < 0.00001)
                    buyButtonLabel.text = "무료";
                else
                    buyButtonLabel.text = LocalizationManager.GetData(LocalizationKey.SETTING_PUBLIC_PURCHASE);
            }
        }
        else
        {
            // fallback
            iconUITex.mainTexture = noimageTex;
            titleText.text = "?";
            descriptionText.text = "...";
            priceText.text = string.Format(korean, "{0:C}", 0);
            newMark.cachedGameObject.SetActive(false);
            buyButton.isEnabled = false;
            buyButtonLabel.text = "";
        }
    }

    public void ShowDetail()
    {
        var storeView = GetComponentInParent<ContentsStoreView>();
        if (storeView != null)
        {
            storeView.ShowItemDetail(item);
        }
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
                buyButton.isEnabled = false;
                buyButtonLabel.text = "처리중";

                BigboardServer.Buy(item, (conn, retCode, message, success) =>
                {
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
}
