using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class InteractionOption : MonoBehaviour
{
    public UILabel Title;

    enum EPlayMode { AUTO_CONTINUE, MANUAL_SELECT }
    const string szCheckBoxTag = "TimeCheckBox";
    const string szPlayModeTag = "PlayButton";
    const string szPlayModeExplainTag = "ExplainLabel";
    const string szAutoMode = "next_on";
    const string szManualMode = "y_off";

    const float minutesUnit = 10f;

    List<UIToggle> toggleList;
    EPlayMode mCurrentPlayMode;
    EPlayMode currentPlayMode
    {
        set {
            mCurrentPlayMode = value;
            switch(value)
            {
                case EPlayMode.AUTO_CONTINUE:
                    playModeBtn.normalSprite = szAutoMode;
                    playModeLabel.text = LocalizationManager.GetData(LocalizationKey.SETTING_PREFERENCES_AUTORUNMODE);
                    playModeExplainLabel.text = LocalizationManager.GetData(LocalizationKey.SETTING_PREFERENCES_INTERACTION_AUTO_DESCRIPTION);
                    SettingsManager.enablesAutoPlayInteraction = true;
                    for (int i = 0; i < toggleList.Count; ++i)
                    { toggleList[i].gameObject.SetActive(true); }
                break;

                case EPlayMode.MANUAL_SELECT:
                    playModeBtn.normalSprite = szManualMode;
                    playModeLabel.text = LocalizationManager.GetData(LocalizationKey.SETTING_PREFERENCES_MANUALMODE);
                    playModeExplainLabel.text = LocalizationManager.GetData(LocalizationKey.SETTING_PREFERENCES_INTERACTION_MANUAL_DESCRIPTION);
                    SettingsManager.enablesAutoPlayInteraction = false;
                    for (int i = 0; i < toggleList.Count; ++i )
                    {   toggleList[i].gameObject.SetActive(false);  }

                    break;
            }
        }
        get {
            mCurrentPlayMode = SettingsManager.enablesAutoPlayInteraction ? EPlayMode.AUTO_CONTINUE : EPlayMode.MANUAL_SELECT;
            return mCurrentPlayMode; 
        }
    }
    
    UILabel mPlayModeLabel;
    UILabel playModeLabel
    {
        get
        {
            if (mPlayModeLabel == null)
            {
                UILabel[] btnsLabel = GetComponentsInChildren<UILabel>();
                for (int i = 0; i < btnsLabel.Length; ++i)
                {
                    if (btnsLabel[i].tag == szPlayModeTag)
                    {
                        mPlayModeLabel = btnsLabel[i];
                        btnsLabel = null;
                        break;
                    }
                }
            }
            return mPlayModeLabel;
        }
    }

    UILabel mPlayModeExplainLabel;
    UILabel playModeExplainLabel
    {
        get
        {
            if (mPlayModeExplainLabel == null)
            {
                UILabel[] btnsLabel = GetComponentsInChildren<UILabel>();
                for (int i = 0; i < btnsLabel.Length; ++i)
                {
                    if (btnsLabel[i].tag == szPlayModeExplainTag)
                    {
                        mPlayModeExplainLabel = btnsLabel[i];
                        btnsLabel = null;
                        break;
                    }
                }
            }
            return mPlayModeExplainLabel;
        }
    }

    UIButton mPlayModeBtn;
    UIButton playModeBtn
    {
        get
        {
            if(mPlayModeBtn == null)
            {
                UIButton[] btns = GetComponentsInChildren<UIButton>();
                for(int i = 0 ; i < btns.Length ; ++i)
                {
                    if( btns[i].tag == szPlayModeTag)
                    {
                        mPlayModeBtn = btns[i];
                        btns = null;
                        break;
                    }
                }
            }
            return mPlayModeBtn;
        }
    }
    
    void Awake()
    {
        toggleList = new List<UIToggle>(0);
        UIToggle[]  toggles = GetComponentsInChildren<UIToggle>();
        for(int i = 0 ; i < toggles.Length ; ++i)
        {
            if(toggles[i].tag == szCheckBoxTag)
            {   toggleList.Add(toggles[i]); }
        }
    }

    void OnEnable()
    {
        Title.text = BigboardServerDataManager.GetListAllContentsStoreItemInfo(ContentsStoreItemType.Mode.Interaction)[0].type.typeName;

        int index = (int)((SettingsManager.autoPlayMinuteInteraction - minutesUnit) / minutesUnit);
        index = Mathf.Clamp(index, 0, toggleList.Count - 1);
        for (int i = 0; i < toggleList.Count; ++i)
        { toggleList[i].value = (i == index); }

        ReplaceToggleButton();
        currentPlayMode = SettingsManager.enablesAutoPlayInteraction ? EPlayMode.AUTO_CONTINUE : EPlayMode.MANUAL_SELECT;
    }

    void OnDisable()
    {
        for (int i = 0; i < toggleList.Count; ++i)
        {
            if (toggleList[i].value)
            {
                SettingsManager.autoPlayMinuteInteraction = (i * minutesUnit) + minutesUnit;
                break;
            }
        }
    }

    public void ClickPlayMode()
    {
        ReplaceToggleButton();

        if(currentPlayMode == EPlayMode.AUTO_CONTINUE)
        {   currentPlayMode = EPlayMode.MANUAL_SELECT;  }
        else
        {   currentPlayMode = EPlayMode.AUTO_CONTINUE;   }
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }

    private void ReplaceToggleButton()
    {
        for (int i = 0; i < toggleList.Count; ++i)
        {
            UILabel label = toggleList[i].GetComponentInChildren<UILabel>();
            if (label != null)
                label.text = string.Format(LocalizationManager.GetData(LocalizationKey.SETTING_PREFERENCES_MINUTE), (i + 1) * minutesUnit);
        }
    }
}


