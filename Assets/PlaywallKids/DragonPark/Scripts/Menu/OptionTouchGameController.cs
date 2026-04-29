using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EtargetOfAge = ContentsSIModelingInfo.StateTargetOfAge;

public class OptionTouchGameController : MonoBehaviour
{
    public UILabel Title;

    public UILabel currentContent;
    public int[] toggleIndexNo;
    public UIToggle[] toggleGroup;

    enum ETogglGroup:byte { TIME_CHECK = 0, TARGET_AGE =1 }

    int currentContentSequenceNo;
    
    public void SetInfomation(GameObject go)
    {
        CommonContentsButton button = go.GetComponent<CommonContentsButton>();
        if(button != null)
        {
			string contentName = ((BigboardContentMode) button.contentIndex).ToString();
            string name = button.label;
            currentContent.text = name;
            currentContentSequenceNo = button.contentIndex;
			object info = SettingsManager.Load(contentName);
            if(info == null)
            { 
				SettingsManager.Save(((BigboardContentMode) button.contentIndex).ToString(), 0);
            }else{
				object obj = SettingsManager.Load(contentName);
                for (int i = 0; i < toggleGroup.Length; ++i)          {
                    toggleGroup[i].value = (i == int.Parse(obj.ToString()));
                }
			}
		}
    }

	public void ClickChecBox()
	{
		if(currentContentSequenceNo < 0) return;
		int i = 0 ;
		for( int len = toggleGroup.Length; i< len; ++i)
		{ if(toggleGroup[i].value) break; }

		string contentName = ((BigboardContentMode)currentContentSequenceNo).ToString();
		Debug.Log(contentName);
		SettingsManager.Save(contentName, i);
	}

	void Awake()
	{
		currentContentSequenceNo = -1;
		for(int i = 0, len  =toggleGroup.Length ; i< len ; ++i)
		{toggleGroup[i].onChange.Add(new EventDelegate(ClickChecBox)); 	}
	}

    void OnEnable()
    {
        Title.text = BigboardServerDataManager.GetListAllContentsStoreItemInfo(ContentsStoreItemType.Mode.Touch)[0].type.typeName;

        ContentsStoreItemInfo item = BigboardServerDataManager.GetContentStoreItemInfo(currentContentSequenceNo);
        if( item != null)       {           
            currentContent.text = item.name;
        }

        for (int i = 0; i < toggleGroup.Length; ++i)
        {
            UILabel label = toggleGroup[i].GetComponentInChildren<UILabel>();
            if (label != null)
                label.text = string.Format(LocalizationManager.GetData(LocalizationKey.SETTING_PREFERENCES_MINUTE), (i + 1));
        }
    }
}