using UnityEngine;
using ML.PlaywallKids.Common;

/// <summary>
/// 프린터 설정 창
/// </summary>
public class PrinterManagementPanel : AnimatablePanel
{
    public UIToggle printerEnabledToggle;

    public override void BeginShow()
    {
        base.BeginShow();
        printerEnabledToggle.Set(CommonSettings.printerEnabled, false);
    }

    public void Close()
    {
        AdminMenuSelectionPanel adminMenu = FindObjectOfType<AdminMenuSelectionPanel>();
        if (adminMenu != null)
            adminMenu.ShowHiddenMenu();
        Hide();
    }

    public void TogglePrinterEnabled()
    {
       // CommonSettings.printerEnabled = printerEnabledToggle.value;
    }
}