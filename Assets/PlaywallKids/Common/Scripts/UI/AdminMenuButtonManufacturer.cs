using UnityEngine;
using System.Collections.Generic;
using EMode = ContentsStoreItemType.Mode;

public class AdminMenuButtonManufacturer : MonoBehaviour
{
    public UIGrid gridButtonGroup;
    public UIAtlas iconAtlas;
    public GameObject tweenTarget;
    public GameObject buttonPrefab;
    public EventDelegate[] callbackFunc;

    string[] iconImageName = new string[] { "2D_DROWING", "3D_PET", "TOUCH_GAME", "INTERACTION", "MOTION_GAME", "Aquarium_Fish" };
    LocalizationKey[] modeLocalization = new LocalizationKey[] {
        LocalizationKey.MANAGEMENT_MENU_2D,
        LocalizationKey.MANAGEMENT_MENU_DRAGONPARK,
        LocalizationKey.MANAGEMENT_MENU_TOUCH,
        LocalizationKey.MANAGEMENT_MENU_INTERACTION,
        LocalizationKey.MANAGEMENT_MENU_MOTION,
        LocalizationKey.MANAGEMENT_MENU_AQUARIUM
    };

    void OnEnable()
    {
        BigboardGlobal.currentMode = EMode.None;

#if UNITY_EDITOR
        BigboardServerDataManager.TestLogin(null);
#endif

        for (int index = (int)EMode.None + 1, len = (int)EMode.Aquarium; index <= len; ++index)
        {
            List<ContentsStoreItemInfo> contents = BigboardServerDataManager.GetListMyContentsStoreItemInfo((EMode)index);
            for (int cnt = 0, contentsLen = contents.Count; cnt < contentsLen; ++cnt)
            {
                if (contents[cnt].useYn)
                {
                    GameObject obj = NGUITools.AddChild(gridButtonGroup.gameObject, buttonPrefab) as GameObject;
                    CommonContentsButton btnObj = obj.GetComponent<CommonContentsButton>();
                    btnObj.sprite.atlas = iconAtlas;
                    btnObj.sprite.spriteName = iconImageName[index - 1];
                    btnObj.sprite.width = 360;
                    btnObj.sprite.height = 360;
                    btnObj.label = LocalizationManager.GetData(modeLocalization[index - 1]);
                    UILabel label = btnObj.GetComponentInChildren<UILabel>();
                    label.fontSize = 50;
                    Vector3 pos = label.transform.localPosition;
                    pos.y -= btnObj.sprite.height * 0.5f * 0.35f;
                    label.transform.localPosition = pos;
                    label.width = 400;
                    btnObj.button.onClick.Add(callbackFunc[index - 1]);
                    UIPlayTween tween = obj.AddComponent<UIPlayTween>();
                    tween.tweenTarget = tweenTarget;
                    tween.trigger = AnimationOrTween.Trigger.OnClick;
                    tween.playDirection = AnimationOrTween.Direction.Reverse;
                    tween.disableWhenFinished = AnimationOrTween.DisableCondition.DisableAfterReverse;

                    break;
                }
            }
        }
        gridButtonGroup.Reposition();
    }

    void OnDisable()
    {
        List<Transform> list = gridButtonGroup.GetChildList();
        for (int i = 0; i < list.Count; ++i)
        {
            Destroy(list[i].gameObject);
        }
    }
}