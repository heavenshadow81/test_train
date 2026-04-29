using UnityEngine;

public class BigboardAlertPanel : AnimatablePanel
{
    #region Enums
    public enum ButtonType
    {
        OneButton,
        TwoButtons
    }
    #endregion

    #region Public variables
    // container
    public AnimatablePanel containerPanel;
    public GameObject loading;

    // message
    public UILabel messageLabel;

    // one button
    public UIButton oneButtonOkButton;

    // two buttons
    public UIButton twoButtonsOkButton;
    public UIButton twoButtonsCancelButton;
    #endregion

    #region Properties
    private static BigboardAlertPanel _sharedInstance = null;
    public static BigboardAlertPanel sharedInstance
    {
        get
        {
            if (_sharedInstance == null)
            {
                _sharedInstance = FindObjectOfType<BigboardAlertPanel>();
                if (_sharedInstance == null)
                {
                    BigboardAlertPanel prefab = Resources.Load<BigboardAlertPanel>("Common/BigboardAlertPanel");
                    _sharedInstance = NGUITools.AddChild(UIRoot.list[0].gameObject, prefab.gameObject).GetComponent <BigboardAlertPanel>();
                    
                }
            }
            return _sharedInstance;
        }
    }
    #endregion

    #region Private variables
    private ButtonType _buttonType = ButtonType.OneButton;
    private System.Action _okHandler = null;
    private System.Action _cancelHandler = null;
    private bool _isHiding = false;
    #endregion


    public void OnEnable()
    {
        if (!isShowing)
        {
            string ok = "", cancel = "";

            if(_buttonType == ButtonType.OneButton)
            {
                UILabel label = oneButtonOkButton.GetComponentInChildren<UILabel>();
                if(label != null) ok = label.text;
            }
            else
            {
                UILabel label = twoButtonsOkButton.GetComponentInChildren<UILabel>();
                if (label != null) ok = label.text;
                label = twoButtonsCancelButton.GetComponentInChildren<UILabel>();
                if (label != null) cancel = label.text;
            }

            Show(_buttonType, messageLabel.text, ok, _okHandler, cancel, _cancelHandler);
        }
    }

    public void OnDestroy()
    {
        _sharedInstance = null;
    }

    public void Show(ButtonType buttonType, string message, string ok = "", System.Action okHandler = null, string cancel = "", System.Action cancelHandler = null)
    {
        base.Show();
        containerPanel.Show();

        _buttonType = buttonType;

        if (message == null) message = "";
        messageLabel.text = message;

        oneButtonOkButton.gameObject.SetActive(_buttonType == ButtonType.OneButton);
        twoButtonsOkButton.gameObject.SetActive(_buttonType == ButtonType.TwoButtons);
        twoButtonsCancelButton.gameObject.SetActive(_buttonType == ButtonType.TwoButtons);

        if (buttonType == ButtonType.OneButton)
        {
            if (string.IsNullOrEmpty(ok)) ok = @"확인";

            UILabel label = oneButtonOkButton.GetComponentInChildren<UILabel>();
            if (label != null) label.text = ok;
        }
        else
        {
            if (string.IsNullOrEmpty(ok)) ok = @"확인";
            if (string.IsNullOrEmpty(cancel)) cancel = @"취소";

            UILabel label = twoButtonsOkButton.GetComponentInChildren<UILabel>();
            if (label != null) label.text = ok;
            label = twoButtonsCancelButton.GetComponentInChildren<UILabel>();
            if (label != null) label.text = cancel;
        }

        _okHandler = okHandler;
        _cancelHandler = cancelHandler;
    }

    public void Loading(bool active)
    {
        loading.SetActive(active);
        if( active )
        {
            base.Show();
            
        }
        else
        {
            base.Hide();

        }
    }

    public override void EnableWidgets()
    {
        base.EnableWidgets();
        oneButtonOkButton.isEnabled = _buttonType == ButtonType.OneButton;
        twoButtonsOkButton.isEnabled = _buttonType == ButtonType.TwoButtons;
        twoButtonsCancelButton.isEnabled = _buttonType == ButtonType.TwoButtons;
    }

    public new void Hide()
    {
        containerPanel.Hide();
        _isHiding = true;

        base.Hide();
    }

    public override void Deactive()
    {
        containerPanel.Deactive();

        _isHiding = false;

        base.Deactive();
    }

    public void OK()
    {
        if (_okHandler != null) _okHandler();
        Hide();
    }

    public void Cancel()
    {
        if (_cancelHandler != null) _cancelHandler();
        Hide();
    }
}
