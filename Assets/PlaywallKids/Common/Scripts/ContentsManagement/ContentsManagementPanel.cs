using UnityEngine;

/// <summary>
/// 콘텐츠 관리 창
/// </summary>
public class ContentsManagementPanel : AnimatablePanel
{
    public void Close()
    {
        AdminMenuSelectionPanel adminMenu = FindObjectOfType<AdminMenuSelectionPanel>();
        if (adminMenu != null)
            adminMenu.ShowHiddenMenu();
        Hide();
    }
}