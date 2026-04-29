using UnityEngine;
using System.Collections.Generic;
using System;

public class BigboardServerDataManager
{
    public static bool TestLogin(Action<bool> handler = null)
    {
        bool success = false;
        BigboardServer.Login("admin", "admin", (conn, retCode, message, flag) =>
        {
            if (flag)
            {
                BigboardServer.GetContentsList((_conn, _retCode, _message) =>
                {
                    BigboardServer.GetConfig((_conn2, _retCode2, _message2, mainOrder, subOrder) =>
                    {
                        BigboardServer.GetState((_conn3, _retCode3, _message3, _info) =>
                        {
                            Debug.Log("Return Code : " + _retCode + " messgae is : " + _message);
                            if(handler != null)
                                handler(true);
                        });
                    });

                });
            }
        });

        return success;
    }

    public static List<ContentsStoreItemType> GetListUniqueContentsStoreItemType()
    {
        List<ContentsStoreItemType> tempList = GetListAllContentsStoreItemType();
        List<ContentsStoreItemType> list = new List<ContentsStoreItemType>();

        for(int i=0; i<tempList.Count; i++)
        {
            bool check = true;
            for(int j=0; j<list.Count; j++)
                if (tempList[i].mode == list[j].mode)
                    check = false;
            if (check)
                list.Add(tempList[i]);
        }

        return list;
    }
    public static int GetContentsStoreItemTypeCount()
    {
        return GetListUniqueContentsStoreItemType().Count;
    }

    // GetContentsTypeList
    public static List<ContentsStoreItemType> GetListAllContentsStoreItemType()
    {
        List<ContentsStoreItemInfo> listItem = GetListAllContentsStoreItemInfo();
        List<ContentsStoreItemType> list = new List<ContentsStoreItemType>();

        for (int i = 0; i < listItem.Count; i++)
            list.Add(listItem[i].type);

        return list;
    }

    public static List<ContentsStoreItemType> GetListMyContentsStoreItemType()
    {
        List<ContentsStoreItemInfo> listItem = GetListMyContentsStoreItemInfo();
        List<ContentsStoreItemType> list = new List<ContentsStoreItemType>();

        for (int i = 0; i < listItem.Count; i++)
            list.Add(listItem[i].type);

        return list;
    }

#region All Contents List
    /// <summary>
    /// All List
    /// </summary>
    /// <returns></returns>
    public static List<ContentsStoreItemInfo> GetListAllContentsStoreItemInfo()
    {
        List<ContentsStoreItemInfo> list = new List<ContentsStoreItemInfo>();
        foreach (KeyValuePair<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>> item in BigboardServer.cachedContents)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                list.Add(item.Value[i]);
            }
        }

        list.Sort((a, b) =>
            {
                return a.type.typeCode.CompareTo(b.type.typeCode);
            });

        return list;
    }


    /// <summary>
    /// Mode All List
    /// </summary>
    /// <returns></returns>
    public static List<ContentsStoreItemInfo> GetListAllContentsStoreItemInfo(ContentsStoreItemType.Mode mode)
    {
        if (mode == ContentsStoreItemType.Mode.None)
            return null;
        return BigboardServer.cachedContents[mode];
    }
#endregion

#region Buy Contents List
    /// <summary>
    /// All My Contents List
    /// </summary>
    /// <returns></returns>
    public static List<ContentsStoreItemInfo> GetListMyContentsStoreItemInfo()
    {
        return _SelectOwnedItems(GetListAllContentsStoreItemInfo());
    } 

    /// <summary>
    /// Mode My Contents List
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static List<ContentsStoreItemInfo> GetListMyContentsStoreItemInfo(ContentsStoreItemType.Mode mode)
    {
        if (BigboardServer.cachedContents.ContainsKey(mode) == false)
            return new List<ContentsStoreItemInfo>();
        return _SelectOwnedItems(BigboardServer.cachedContents[mode]); ;
    }
#endregion
    
#region Use Contents List
    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static List<ContentsStoreItemInfo> GetListUseContentsStoreItemInfo()
    {
        return _SelectUsedItem(GetListAllContentsStoreItemInfo());
    }
     
    /// <summary>
    /// Mode My Contents List
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static List<ContentsStoreItemInfo> GetListUseContentsStoreItemInfo(ContentsStoreItemType.Mode mode)
    {
        if (mode == ContentsStoreItemType.Mode.None)
            return null;
        return _SelectUsedItem(BigboardServer.cachedContents[mode]);
    }
#endregion


    // GetItem
    public static ContentsStoreItemInfo GetContentStoreItemInfo(int seq)
    {
        foreach (KeyValuePair<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>> item in BigboardServer.cachedContents)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                if (item.Value[i].seq == seq)
                    return item.Value[i];
            }
        }
        return null;
    }
    public static ContentsStoreItemInfo GetContentStoreItemInfo(BigboardContentMode mode)
    {
        int seq = (int)mode;

        foreach (KeyValuePair<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>> item in BigboardServer.cachedContents)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                if (item.Value[i].seq == seq)
                    return item.Value[i];
            }
        }
        return null;
    }

    public static void ThemeAndContentsTypeSetting()
    {
        ThemeSetting();

        ContentsTypeSetting();
    }

    public static void ThemeSetting()
    {
        // Theme Setting
        BigboardContentMode[] list = _GetCurrentThemeList();
        if (list != null)
        {
            List<ContentsStoreItemInfo> unUseList = GetListMyContentsStoreItemInfo();
            for (int i = 0; i < unUseList.Count; i++)
                unUseList[i].useYn = false;

            for (int i = 0; i < list.Length; i++)
            {
                ContentsStoreItemInfo item = GetContentStoreItemInfo(list[i]);
                if (item != null)
                {
                    if (item.IsBuy())
                        item.useYn = true;
                }
                else
                    Debug.Log(list[i] +" is Null");
            }
        }
    }

    public static void ContentsTypeSetting()
    {
        // ContentsType Setting
        List<BigboardContentMode> listContentsType = _GetContentsTypeList();
        if (listContentsType.Count > 0)
        {
            List<ContentsStoreItemInfo> useList;
            useList = GetListMyContentsStoreItemInfo();
            if (BigboardServer.cachedSituationalInfo.theme != ContentsSIModelingInfo.StateTheme.None)
                useList = GetListUseContentsStoreItemInfo();

            for (int i = 0; i < useList.Count; i++)
                useList[i].useYn = listContentsType.Contains((BigboardContentMode)useList[i].seq);
        }
    }

    public static bool IsContentsTypeWorking(ContentsSIModelingInfo.StateContentsType state)
    {
        ThemeSetting();

        if (BigboardServer.cachedSituationalInfo.theme == ContentsSIModelingInfo.StateTheme.None)
            return true;

        List<ContentsStoreItemInfo> useList = GetListUseContentsStoreItemInfo();
        List<ContentsStoreItemInfo> tempList = new List<ContentsStoreItemInfo>(useList);

        List<BigboardContentMode> listContentsType = _GetContentsTypeList(state);
        for (int i = 0; i < useList.Count; i++)
            if (listContentsType.Contains((BigboardContentMode)useList[i].seq))
                tempList.Remove(GetContentStoreItemInfo(useList[i].seq));

        if (useList.Count == tempList.Count || tempList.Count == 0)
            return false;

        return true;
    }

    #region Private Method
    private static List<ContentsStoreItemInfo> _SelectOwnedItems(List<ContentsStoreItemInfo> list)
    {
        List<ContentsStoreItemInfo> result = new List<ContentsStoreItemInfo>();

        for (int i = 0; i < list.Count; i++)
            if (list[i].IsBuy() == true)
                result.Add(list[i]);

        return result;
    }

    private static List<ContentsStoreItemInfo> _SelectUsedItem(List<ContentsStoreItemInfo> list)
    {
        List<ContentsStoreItemInfo> buyList = _SelectOwnedItems(list);
        List<ContentsStoreItemInfo> result = new List<ContentsStoreItemInfo>();

        for (int i = 0; i < buyList.Count; i++)
            if (buyList[i].useYn == true)
                result.Add(buyList[i]);

        return result;
    }

    private static BigboardContentMode[] _GetCurrentThemeList()
    {
        switch(BigboardServer.cachedSituationalInfo.theme)
        {
            case ContentsSIModelingInfo.StateTheme.SketchBook:
                return new BigboardContentMode[] {
                    BigboardContentMode.Drawing2D_Aircap, BigboardContentMode.Drawing2D_Note, BigboardContentMode.Drawing2D_SandDraw, BigboardContentMode.Drawing2D_SandPrint, BigboardContentMode.Drawing2D_HandPrint, BigboardContentMode.Drawing2D_FruitPrint, BigboardContentMode.Drawing2D_NeonDraw
                };

            case ContentsSIModelingInfo.StateTheme.PlayingJump:
                return new BigboardContentMode[] {
                    BigboardContentMode.Interaction_Heart, BigboardContentMode.Interaction_Lamplight, BigboardContentMode.Interaction_Dish, BigboardContentMode.Interaction_Paints,
                    BigboardContentMode.Motion_Weightlessness, BigboardContentMode.Motion_JumpGame
                };

            case ContentsSIModelingInfo.StateTheme.Creative:
                return new BigboardContentMode[] {
                    BigboardContentMode.Drawing2D_Note, 
                    BigboardContentMode.Touch_AlphabetGame
                };

            case ContentsSIModelingInfo.StateTheme.Avatars:
                return new BigboardContentMode[] { 
                    BigboardContentMode.Motion_Weightlessness
                };

            case ContentsSIModelingInfo.StateTheme.WithCharacter:
                return new BigboardContentMode[] { 
                    //BigboardContentMode.Drawing3D_Pet, BigboardContentMode.Drawing3D_Fruit, BigboardContentMode.Drawing3D_Car, BigboardContentMode.Drawing3D_Robot, BigboardContentMode.Drawing3D_Balloon,
                    BigboardContentMode.Drawing3D_Dragon, BigboardContentMode.Drawing3D_FreeDrawing, BigboardContentMode.Drawing3D_PetMotion, BigboardContentMode.Drawing3D_SketchBook,
                    BigboardContentMode.Touch_Slime
                };

            default:
                return null;
        }
    }

    private static List<BigboardContentMode> _GetContentsTypeList(ContentsSIModelingInfo.StateContentsType state = ContentsSIModelingInfo.StateContentsType.None)
    {
        if(state == ContentsSIModelingInfo.StateContentsType.None)
            state = BigboardServer.cachedSituationalInfo.contentsType;

        List<BigboardContentMode> list = new List<BigboardContentMode>();
        BigboardContentMode[] competition = 
            new BigboardContentMode[] {
                BigboardContentMode.Motion_Weightlessness, BigboardContentMode.Motion_JumpGame,
                BigboardContentMode.Touch_Slime
            };

        switch (state)
        {
            case ContentsSIModelingInfo.StateContentsType.Competition:
                for(int i=0; i<competition.Length; i++)
                    list.Add(competition[i]);
                break;

            case ContentsSIModelingInfo.StateContentsType.Cooperation:
                Array contentList = Enum.GetValues(typeof(BigboardContentMode));
                for (int i = 0; i < contentList.Length; i++)
                    list.Add((BigboardContentMode)contentList.GetValue(i));

                for (int i = 0; i < competition.Length; i++)
                    list.Remove(competition[i]);

                break;
        }

        return list;
    }

    #endregion
}
