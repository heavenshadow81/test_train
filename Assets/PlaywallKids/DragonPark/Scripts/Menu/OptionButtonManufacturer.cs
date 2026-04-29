using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OptionButtonManufacturer : MonoBehaviour
{
    public ContentsStoreItemType.Mode currentMode;
    public UIAtlas atlas;
    public UIGrid grid;
    public GameObject buttonPrefab;
    public List< EventDelegate > onClickFunction;

    List<ContentsStoreItemInfo> contents;

    const string atlasPath = "Atlas/";
    const string prefixAtlasWord = "IconAtlas_";
    const string prafabPath = "Common/";
    const string fileName = "UIButtonPrefab";

    void Awake()
    {
#if UNITY_EDITOR
        BigboardServerDataManager.TestLogin(null);
#endif
        if (contents == null) contents = BigboardServerDataManager.GetListMyContentsStoreItemInfo(currentMode);
        if (buttonPrefab == null) 
        {
            buttonPrefab = Resources.Load(prafabPath + fileName) as GameObject;
        }
        if(atlas == null && contents.Count>0)
        {
            string subfixAtlasWord = contents[0].type.typeCode;
            GameObject file = Resources.Load(atlasPath + prefixAtlasWord + subfixAtlasWord) as GameObject;
            atlas = file.GetComponent<UIAtlas>();
        }
    }

    void Start()
    {
        for (int i = 0, len = contents.Count; i < len; ++i)
        {
            BigboardContentMode content = (BigboardContentMode)contents[i].seq;
            CommonContentsButton button = NGUITools.AddChild(grid.gameObject, buttonPrefab).GetComponent<CommonContentsButton>();

            button.transform.parent = grid.transform;
            UISprite sprite = button.sprite;
            sprite.atlas = atlas;
            sprite.spriteName = content.ToString();
            button.gameObject.SetActive(true);
            button.label = contents[i].name;
            button.contentIndex = contents[i].seq;
            if (onClickFunction != null && onClickFunction.Count > 0)
            {
                if (i == 0)
                {
                    onClickFunction[0].parameters[0] = new EventDelegate.Parameter(button.gameObject);
                    EventDelegate.Execute(onClickFunction);
                }

                button.button.onClick.Add(new EventDelegate(() =>
                {
                    onClickFunction[0].parameters[0] = new EventDelegate.Parameter(button.gameObject);
                    EventDelegate.Execute(onClickFunction);
                }

                    )

                    );
            }
        }
        grid.Reposition();
    }

    void OnEnable()
    {
        contents = BigboardServerDataManager.GetListMyContentsStoreItemInfo(currentMode);
        CommonContentsButton[] list = grid.GetComponentsInChildren<CommonContentsButton>();
        for( int i=0; i<list.Length; i++)
        {
            list[i].label = contents[i].name;
        }
    }
}
