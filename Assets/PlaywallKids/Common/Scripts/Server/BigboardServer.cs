using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text.RegularExpressions;


/// <summary>
/// This class manages the contents information, also, queries or buys the contents.
/// </summary>
public class BigboardServer : WWWUtil
{
    #region Properties
    private static int _userSeq = -1;
    public static int userSeq
    {
        get
        {
            return _userSeq;
        }
    }

    public static string dataRootPath
    {
        get { return ML.PlaywallKids.Common.CommonPath.GetDataRootPath(); }
    }
    
    private static ContentsSIModelingInfo _cachedSituationalInfo = null;
    public static ContentsSIModelingInfo cachedSituationalInfo
    {
        get
        {
            if (_cachedSituationalInfo == null)
            {
                _cachedSituationalInfo = new ContentsSIModelingInfo();     
#if BIGBOARD_STANDALONE
                _StateLoad();
#endif
            }
            return _cachedSituationalInfo.Clone();
        }
    }

    private static List<ContentsStoreItemType.Mode> _mainOrder = new List<ContentsStoreItemType.Mode>();
    public static List<ContentsStoreItemType.Mode> mainOrder
    {
        get
        {
            return new List<ContentsStoreItemType.Mode>(_mainOrder);
        }
    }

    private static Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>> _cachedContents = new Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>>();
    public static Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>> cachedContents
    {
        get
        {
            Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>> returnList = new Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>>(_cachedContents);

            List<BigboardContentMode> deleteMode = new List<BigboardContentMode>() {
                BigboardContentMode.Motion_Weightlessness
            };

            List<ContentsStoreItemInfo> deleteList = new List<ContentsStoreItemInfo>();
            foreach (KeyValuePair<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>> list in returnList)
            {
                for (int i = 0; i < list.Value.Count; i++)
                    for (int j = 0; j < deleteMode.Count; j++)
                        if (list.Value[i].seq == (int)deleteMode[j])
                        {
                            list.Value.RemoveAt(i);
                            i--;
                            break;
                        }
            }

            return returnList;
        }
    }


#if BIGBOARD_STANDALONE
    public static bool _bGetContentList = false;
    public static bool bGetContentList
    {
        get
        {
            return _bGetContentList;
        }
    }

    private static string strStandAloneFileName
    {
        get
        {
            string name = "StandaloneData";
            if (LocalizationManager.curLanguage == LocalizationLanguage.English)
                name += "_Eng";
            return name;
        }
    }
#endif

    #endregion

    #region Private variables
    private static Dictionary<string, string> _session = new Dictionary<string, string>();
    private static Dictionary<string, object> _jsonDictState;
    private static Dictionary<string, object> _jsonDictStandAlone;
       
    #endregion

    #region Handlers
    public delegate void ConfigHandler(ConnectionResult conn, 
        string returnCode, 
        string message,
        List<ContentsStoreItemType.Mode> mainOrder,
        Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>> subOrder);
    #endregion

    #region Constants
    private const string kBigboardServerURL = "http://www.mogencelab.net:8090/";
    private const string kPageLogin = "apis/setLogin";
    private const string kPageContentsList = "apis/getContentsList";
    private const string kPageMyContentsList = "apis/getBuyContentsList";
    private const string kPageBuyContents = "apis/buyContents";
    private const string kPageGetConfig = "apis/getConfig";
    private const string kPageSetConfig = "apis/setConfig";
    private const string kPageGetState = "apis/getState";
    private const string kPageSetState = "apis/setState";
    #endregion

    #region Login
    public static void Login(string id, string password, Action<ConnectionResult, string, string, bool> handler)
    {
        Dictionary<string, string> paramDict = new Dictionary<string,string>();
        paramDict["id"] = id;
        paramDict["pw"] = password;

#if BIGBOARD_STANDALONE
        if (handler != null) handler(ConnectionResult.Success, "RC0000", "", true);
#else
        HTTPRequest(kBigboardServerURL + kPageLogin, HTTPMethod.GET, paramDict, (conn, www) =>
        {
            bool success = false;
            string returnCode = "";
            string message = "";

            if (conn == ConnectionResult.Success)
            {
                try
                {
                    Dictionary<string, object> dict = MiniJSON.Json.Deserialize(www.text) as Dictionary<string, object>;
                    returnCode = dict["return_code"].ToString();
                    success = returnCode.Equals("RC0000");
                    if (success)
                        int.TryParse(dict["user_seq"].ToString(), out _userSeq);

                    // Check the cookie
                    if (www.responseHeaders.ContainsKey("SET-COOKIE"))
                    {
                        string[] separators = { ";" };
                        string[] tokens = www.responseHeaders["SET-COOKIE"].Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0, cnt = tokens.Length; i < cnt; i++)
                        {
                            string token = tokens[i];
                            if (string.IsNullOrEmpty(token)) continue;
                            else if (token.ToLower().Contains("sessionid"))
                            {
                                _session["Cookie"] = token;
                                break;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Debug.LogError("BigboardServer.Login() : Failed to parse result.");
                    Debug.LogException(e);
                }
            }

            if (!success)
            {
                _userSeq = -1;
                _cachedContents.Clear();
                _session.Clear();
            }

            if (handler != null) handler(conn, returnCode, message, success);
        });
#endif
    }
    #endregion


    #region Contents
    public static void GetContentsList(Action<ConnectionResult, string, string> handler)
    {
        Dictionary<string, string> paramDict = new Dictionary<string, string>();
        if(_userSeq > -1) paramDict["userSeq"] = _userSeq.ToString();

#if BIGBOARD_STANDALONE     
        _StandaloneDataLoad();

        if (handler != null) handler(ConnectionResult.Success, "RC0000", "");
#else
        HTTPRequest(kBigboardServerURL + kPageContentsList, _session, HTTPMethod.GET, paramDict, (conn, www) =>
        {
            string returnCode = "";
            string message = "";

            if (conn == ConnectionResult.Success)
            {
                try
                {
                    Dictionary<string, object> dict = MiniJSON.Json.Deserialize(www.text) as Dictionary<string, object>;
                    returnCode = dict["returnCode"].ToString();
                    message = dict["message"].ToString();
                    _ParseContentsRequestResponse(dict);
                }
                catch (Exception e)
                {
                    Debug.LogError("BigboardServer.GetContentsList() : Failed to parse result.");
                    Debug.LogException(e);
                }
            }

            if (handler != null) handler(conn, returnCode, message);
        });
#endif
    }

    public static void GetMyContents(Action<ConnectionResult, string, string> handler)
    {
        Dictionary<string, string> paramDict = new Dictionary<string, string>();
        paramDict["userSeq"] = _userSeq.ToString();

#if BIGBOARD_STANDALONE
        if (handler != null) handler(ConnectionResult.Success, "RC0000", "");
#else
        HTTPRequest(kBigboardServerURL + kPageMyContentsList, _session, HTTPMethod.GET, paramDict, (conn, www) =>
        {
            string returnCode = "";
            string message = "";

            if (conn == ConnectionResult.Success)
            {
                try
                {
                    Dictionary<string, object> dict = MiniJSON.Json.Deserialize(www.text) as Dictionary<string, object>;
                    returnCode = dict["returnCode"].ToString();
                    message = dict["message"].ToString();

                    if(returnCode.Equals("RC0000"))
                        _ParseContentsRequestResponse(dict);
                }
                catch (Exception e)
                {
                    Debug.LogError("BigboardServer.GetMyContents() : Failed to parse result.");
                    Debug.LogException(e);

                }
            }

            if (handler != null) handler(conn, returnCode, message);
        });
#endif
    }

    private static List<ContentsStoreItemInfo> _ParseContentsRequestResponse(Dictionary<string, object> dict)
    {
        List<ContentsStoreItemInfo> list = new List<ContentsStoreItemInfo>();

        if (_cachedContents != null) _cachedContents.Clear();

        if (dict != null)
        {
            List<object> allContentsList = dict["contents"] as List<object>;

            for (int i = 0; i < allContentsList.Count; i++)
            {
                // Enumerate the contents information per content type
                Dictionary<string, object> contentsDict = allContentsList[i] as Dictionary<string, object>;
                if (contentsDict != null)
                {
                    string typeName = contentsDict["type"].ToString();
                    string typeCode = contentsDict["type_code"].ToString();

                    // Parse the content type
                    ContentsStoreItemType contentType = new ContentsStoreItemType(typeCode, typeName);
                    // contentTypes.Add(contentType);

                    // Parse the contents
                    List<object> contentsList = contentsDict["list"] as List<object>;
                    if (contentsList != null)
                    {
                        List<ContentsStoreItemInfo> listItem = new List<ContentsStoreItemInfo>();
                        for (int j = 0; j < contentsList.Count; j++)
                        {
                            Dictionary<string, object> contentInfo = contentsList[j] as Dictionary<string, object>;
                            if (contentInfo != null)
                            {
                                ContentsStoreItemInfo itemInfo = new ContentsStoreItemInfo(contentType, contentInfo);
                                list.Add(itemInfo);

                                listItem.Add(itemInfo);
                            }
                        }

                        _cachedContents.Add(contentType.mode, listItem);
                    }
                }
            }

            // Sort all content types by type code
            foreach (KeyValuePair<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>> item in _cachedContents)
            {
                item.Value.Sort((a, b) =>
                {
                    return a.type.typeCode.CompareTo(b.type.typeCode);
                });
            }
        }

        return list;
    }
    
    public static void Buy(ContentsStoreItemInfo item, Action<ConnectionResult, string, string, bool> handler)
    {
        if (item == null)
        {
            Debug.LogError("BigboardServer.Buy() : failed to purchase. item is null.");
            if (handler != null) handler(ConnectionResult.Cancel, "", "", false);
            return;
        }

        Dictionary<string, string> paramDict = new Dictionary<string, string>();
        paramDict["userSeq"] = _userSeq.ToString();
        paramDict["contentsSeq"] = item.seq.ToString();

#if BIGBOARD_STANDALONE
        if (handler != null) handler(ConnectionResult.Success, "RC0000", "", true);
#else
        HTTPRequest(kBigboardServerURL + kPageBuyContents, _session, HTTPMethod.GET, paramDict, (conn, www) =>
        {
            string returnCode = "";
            string message = "";
            bool success = false;

            if (conn == ConnectionResult.Success)
            {
                try
                {
                    Dictionary<string, object> dict = MiniJSON.Json.Deserialize(www.text) as Dictionary<string, object>;
                    returnCode = dict["returnCode"].ToString();
                    message = dict["message"].ToString();

                    if (returnCode.Equals("RC0000"))
                    {
                        success = true;

                        ContentsStoreItemInfo itemInfo = BigboardServerDataManager.GetContentStoreItemInfo(item.seq);
                        itemInfo.ContentBuy();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("BigboardServer.GetMyContents() : Failed to parse result.");
                    Debug.LogException(e);
                }
            }

            if (handler != null) handler(conn, returnCode, message, success);
        });
#endif
    }

    public static int GetConfig(ConfigHandler handler)
    {
        Dictionary<string, string> paramDict = new Dictionary<string, string>();
        paramDict["userSeq"] = _userSeq.ToString();

#if BIGBOARD_STANDALONE
        List<ContentsStoreItemType.Mode> mainOrder = new List<ContentsStoreItemType.Mode>();
        Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>> subOrder = new Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>>();

        // Make MainOrder
        if (mainOrder.Count <= 0)
        {
            Array itemTypeArray = Enum.GetValues(typeof(ContentsStoreItemType.Mode));
            for (int i = 0; i < itemTypeArray.Length; i++)
                if ((ContentsStoreItemType.Mode)itemTypeArray.GetValue(i) != ContentsStoreItemType.Mode.None)
                    mainOrder.Add((ContentsStoreItemType.Mode)itemTypeArray.GetValue(i));
        }

        // Make SubOrder
        if (subOrder.Count <= 0)
        {
            for (int i = 0; i < Enum.GetValues(typeof(BigboardContentMode)).Length; i++)
            {
                BigboardContentMode mode = (BigboardContentMode)Enum.GetValues(typeof(BigboardContentMode)).GetValue(i);
                if (mode != BigboardContentMode.None)
                {
                    ContentsStoreItemType.Mode typeCode = _GetItemTypeModeToContentMode(mode);

                    if (subOrder.ContainsKey(typeCode) == false)
                        subOrder.Add(typeCode, new List<ContentsStoreItemInfo>());

                    ContentsStoreItemInfo info = BigboardServerDataManager.GetContentStoreItemInfo(mode);
                    if (info != null)
                        subOrder[typeCode].Add(info);
                }
            }
        }
        
        BigboardServerDataManager.ThemeAndContentsTypeSetting();

        if (handler != null)
            handler(ConnectionResult.Success, "RC0000", "", mainOrder, subOrder);

        return 0;
#else
        return HTTPRequest(kBigboardServerURL + kPageGetConfig, _session, HTTPMethod.GET, paramDict, (conn, www) =>
        {
            string returnCode = "";
            string message = "";

            if (conn == ConnectionResult.Success)
            {
                try
                {
                    Dictionary<string, object> dict = MiniJSON.Json.Deserialize(www.text) as Dictionary<string, object>;
                    returnCode = dict["returnCode"].ToString();
                    message = dict["message"].ToString();

                    if (returnCode.Equals("RC0000"))
                    {

                        List<object> typeOrderList = dict["typeOrderList"] as List<object>;
                        List<object> list = dict["list"] as List<object>;

                        // main the list items
                        for (int i = 0; i < typeOrderList.Count-1; i++)
                        {
                            for (int j = i+1; j < typeOrderList.Count; j++)
                            {
                                Dictionary<string, object> d1 = typeOrderList[i] as Dictionary<string, object>;
                                int m1 = int.Parse(d1["main_order_index"].ToString());

                                Dictionary<string, object> d2 = typeOrderList[j] as Dictionary<string, object>;
                                int m2 = int.Parse(d2["main_order_index"].ToString());

                                if( m1 > m2)
                                {
                                    object temp = typeOrderList[i];
                                    typeOrderList[i] = typeOrderList[j];
                                    typeOrderList[j] = temp;
                                }
                            }
                        }

                        // sort the list items
                        Dictionary<int, List<object>> sublist = new Dictionary<int, List<object>>();
                        for (int i = 0; i < list.Count; i++ )
                        {
                            Dictionary<string, object> d = list[i] as Dictionary<string, object>;
                            int m = int.Parse(d["main_order_index"].ToString());
                            if (sublist.ContainsKey(m) == false)
                                sublist.Add(m, new List<object>());

                            sublist[m].Add(list[i]);
                        }
                        foreach (KeyValuePair<int, List<object>> sub in sublist)
                        {
                            for (int i = 0; i < sub.Value.Count - 1; i++)
                            {
                                for (int j = i+1; j < sub.Value.Count; j++)
                                {
                                    Dictionary<string, object> d1 = sub.Value[i] as Dictionary<string, object>;
                                    Dictionary<string, object> d2 = sub.Value[j] as Dictionary<string, object>;

                                    int m1 = int.Parse(d1["sub_order_index"].ToString());
                                    int m2 = int.Parse(d2["sub_order_index"].ToString());

                                    if (m1 > m2)
                                    {
                                        object temp = sub.Value[i];
                                        sub.Value[i] = sub.Value[j];
                                        sub.Value[j] = temp;
                                    }
                                }
                            }
                        }

                        /*
                        // sort the list items
                        list.Sort((a, b) =>
                        {
                            Dictionary<string, object> d1 = a as Dictionary<string, object>;
                            Dictionary<string, object> d2 = b as Dictionary<string, object>;

                            if (d1 == null) return -1;
                            else if (d2 == null) return 1;

                            int m1 = int.Parse(d1["main_order_index"].ToString());
                            int s1 = int.Parse(d1["sub_order_index"].ToString());
                            int m2 = int.Parse(d2["main_order_index"].ToString());
                            int s2 = int.Parse(d2["sub_order_index"].ToString());

                            if (m1 != s1)
                                return m1 < m2 ? 1 : -1;
                            else if (s1 == 0)
                                return -1;
                            else if (s2 == 0)
                                return 1;
                            else if (s1 != s2)
                                return s1 < s2 ? 1 : -1;
                            else
                                return 0;
                        });
                        */

                        // type order
                        _mainOrder.Clear();
                        for (int i = 0; i < typeOrderList.Count; i++)
                        {
                            Dictionary<string, object> typeDict = typeOrderList[i] as Dictionary<string, object>;
                            if (typeDict != null)
                            {
                                string typeName = typeDict["type"].ToString();
                                string typeCode = typeDict["type_code"].ToString();
                                int mainOrderIndex = int.Parse(typeDict["main_order_index"].ToString());

                                ContentsStoreItemType type = new ContentsStoreItemType(typeCode, typeName);
                                _mainOrder.Add(type.mode);
                            }
                        }


                        // sub order
                        for (int i = 0; i < list.Count; i++)
                        {
                            Dictionary<string, object> infoDict = list[i] as Dictionary<string, object>;
                            if (infoDict != null)
                            {
                                int seq = int.Parse(infoDict["contents_seq"].ToString());
                                string typeCode = infoDict["type_code"].ToString();
                                int subIndex = int.Parse(infoDict["sub_order_index"].ToString());


                                ContentsStoreItemInfo info = BigboardServerDataManager.GetContentStoreItemInfo(seq);

                                if (infoDict.ContainsKey("name") && infoDict["name"] != null)
                                    info.SetName( infoDict["name"].ToString());
                                if (infoDict.ContainsKey("use_yn") && infoDict["use_yn"] != null)
                                {
                                    string useYn = infoDict["use_yn"].ToString();
                                    info.useYn = useYn.ToLower().Equals("y");
                                }

                                ContentsStoreItemType modeItem = info.type;
                                //if (mainOrderIndexToSequenceDict.ContainsKey(mainOrderIndex)) modeItem = mainOrderIndexToSequenceDict[mainOrderIndex];
                                

                                if (modeItem != null)
                                {
                                    if (subIndex > 0)
                                    {
                                        int index = _cachedContents[modeItem.mode].IndexOf(info);
                                        ContentsStoreItemInfo temp = _cachedContents[modeItem.mode][subIndex - 1];
                                        _cachedContents[modeItem.mode][subIndex - 1] = _cachedContents[modeItem.mode][index];
                                        _cachedContents[modeItem.mode][index] = temp;
                                    }
                                }
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("BigboardServer.GetConfig() : Failed to parse result.");
                    Debug.LogException(e);
                }
            }

            BigboardServerDataManager.ThemeAndContentsTypeSetting();

            if (handler != null)
                handler(conn, returnCode, message, mainOrder, cachedContents);
        });
#endif
    }

    public static int SetConfig(List<ContentsStoreItemType.Mode> mainOrder, Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>> subOrder,
        Action<ConnectionResult, string, string, bool> handler)
    {
        if (mainOrder == null || subOrder == null)
        {
            DebugUtil.DLog("BigboardServer.SetConfig() : Failed to request. One of given parameters is null.");
            return 0;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append("[");

        for (int i = 0; i < mainOrder.Count; i++)
        {
            sb.AppendFormat("{0}", i == 0 ? "" : ",");

            ContentsStoreItemType.Mode key = mainOrder[i];
            if (subOrder.ContainsKey(key) && subOrder[key] != null)
            {
                List<ContentsStoreItemInfo> list = subOrder[key];
                for (int j = 0; j < list.Count; j++)
                {
                    ContentsStoreItemInfo item = list[j];
                    sb.AppendFormat("{0}\"{1},{2},{3},{4}\"", j == 0 ? "" : ",",
                        item.seq, i + 1, item.useYn ? j + 1 : 0, item.useYn ? "Y" : "N");
                }
            }
        }

        sb.Append("]");
        
        Dictionary<string, string> paramDict = new Dictionary<string, string>();
        paramDict["userSeq"] = _userSeq.ToString();
        paramDict["data"] = sb.ToString();

        
#if BIGBOARD_STANDALONE

        Dictionary<int, object> data = null;
        if (_jsonDictStandAlone != null)
        {
            data = new Dictionary<int, object>();
            if (_jsonDictStandAlone.ContainsKey("contents") && _jsonDictStandAlone["contents"] != null)
            {
                List<object> allContentsList = _jsonDictStandAlone["contents"] as List<object>;
                for (int i = 0; i < allContentsList.Count; i++)
                {
                    Dictionary<string, object> contentsDict = allContentsList[i] as Dictionary<string, object>;
                    List<object> contentsList = contentsDict["list"] as List<object>;
                    if (contentsList != null)
                    {
                        for (int j = 0; j < contentsList.Count; j++)
                        {
                            Dictionary<string, object> contentInfo = contentsList[j] as Dictionary<string, object>;
                            if (contentInfo.ContainsKey("seq") && contentInfo["seq"] != null)
                            {
                                int seq = int.Parse(contentInfo["seq"].ToString());
                                if (BigboardServerDataManager.GetContentStoreItemInfo(seq) != null)
                                    if (contentInfo.ContainsKey("use_yn") && contentInfo["use_yn"] != null)
                                        contentInfo["use_yn"] = BigboardServerDataManager.GetContentStoreItemInfo(seq).useYn ? "Y" : "N";
                            }
                        }
                    }
                }
            }
        }

        _StandaloneDataSave();

        if (handler != null) handler(ConnectionResult.Success, "RC0000", "", true);
        return 0;
#else

        return HTTPRequest(kBigboardServerURL + kPageSetConfig, _session, HTTPMethod.GET, paramDict, (conn, www) =>
        {
            string returnCode = "";
            string message = "";
            bool success = false;

            if (conn == ConnectionResult.Success)
            {
                try
                {
                    Dictionary<string, object> dict = MiniJSON.Json.Deserialize(www.text) as Dictionary<string, object>;
                    returnCode = dict["returnCode"].ToString();
                    if (dict.ContainsKey("message") && dict["message"] != null)
                        message = dict["message"].ToString();

                    if(returnCode.Equals("RC0000"))
                        success = true;
                }
                catch (Exception e)
                {
                    Debug.LogError("BigboardServer.SetConfig() : Failed to parse result.");
                    Debug.LogException(e);
                }
            }

            if (handler != null)
                handler(conn, returnCode, message, success);
        });
#endif
    }



    public static void GetState(Action<ConnectionResult, string, string, ContentsSIModelingInfo> handler)
    {
        Dictionary<string, string> paramDict = new Dictionary<string, string>();
        paramDict["userSeq"] = _userSeq.ToString();
        
#if BIGBOARD_STANDALONE

        _StateLoad();

        if (handler != null) handler(ConnectionResult.Success, "RC0000", "", _cachedSituationalInfo.Clone());
#else
        HTTPRequest(kBigboardServerURL + kPageGetState, _session, HTTPMethod.GET, paramDict, (conn, www) =>
        {
            string returnCode = "";
            string message = "";

            if (conn == ConnectionResult.Success)
            {
                try
                {
                    Dictionary<string, object> dict = MiniJSON.Json.Deserialize(www.text) as Dictionary<string, object>;
                    returnCode = dict["returnCode"].ToString();
                    if (dict.ContainsKey("message") && dict["message"] != null)
                        message = dict["message"].ToString();

                    if (returnCode.Equals("RC0000"))
                    {
                        if (dict.ContainsKey("state"))
                        {
                            Dictionary<string, object> state = dict["state"] as Dictionary<string, object>;
                            _cachedSituationalInfo.ReadJsond(state);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("BigboardServer.GetState() : Failed to parse result.");
                    Debug.LogException(e);
                }
            }


            if (handler != null) handler(conn, returnCode, message, _cachedSituationalInfo.Clone());
        });
#endif
    }

    public static void SetState(ContentsSIModelingInfo info, Action<ConnectionResult, string, string, bool> handler)
    {
        if (info == null)
        {
            if (handler != null) handler(ConnectionResult.Cancel, "", "", false);
            return;
        }

        ContentsSIModelingInfo copiedInfo = info.Clone();
        _cachedSituationalInfo = copiedInfo;

        _jsonDictState = copiedInfo.GetJsonDictionary();
        
#if BIGBOARD_STANDALONE
        _StateSave();

        if (handler != null) handler(ConnectionResult.Success, "RC0000", "", true);
#else        
        Dictionary<string, string> paramDict = new Dictionary<string, string>();
        foreach (KeyValuePair<string, object> param in _jsonDictState)
            paramDict[param.Key] = param.Value.ToString();
        paramDict["userSeq"] = _userSeq.ToString();


        HTTPRequest(kBigboardServerURL + kPageSetState, _session, HTTPMethod.GET, paramDict, (conn, www) =>
        {
            string returnCode = "";
            string message = "";
            bool success = false;

            if (conn == ConnectionResult.Success)
            {
                try
                {
                    Dictionary<string, object> dict = MiniJSON.Json.Deserialize(www.text) as Dictionary<string, object>;
                    returnCode = dict["returnCode"].ToString();
                    if (dict.ContainsKey("message") && dict["message"] != null)
                        message = dict["message"].ToString();

                    success = returnCode.Equals("RC0000");

                    if (success)
                        _cachedSituationalInfo = copiedInfo;
                }
                catch (Exception e)
                {
                    Debug.LogError("BigboardServer.SetState() : Failed to parse result.");
                    Debug.LogException(e);
                }
            }

            if (handler != null) handler(conn, returnCode, message, success);
        });
#endif
    }
    #endregion


    #region PrivateMethod
    private static ContentsStoreItemType.Mode _GetItemTypeModeToContentMode(BigboardContentMode mode)
    {
        for (int i = 0; i < Enum.GetValues(typeof(ContentsStoreItemType.Mode)).Length; i++)
        {
            ContentsStoreItemType.Mode typeMode = (ContentsStoreItemType.Mode)Enum.GetValues(typeof(ContentsStoreItemType.Mode)).GetValue(i);
            if (typeMode != ContentsStoreItemType.Mode.None)
                if (mode.ToString().IndexOf(typeMode.ToString()) == 0)
                    return typeMode;
        }

        return ContentsStoreItemType.Mode.None;
    }

    private static void _StateLoad()
    {
        if (_jsonDictState == null)
        {
            string jsonText = "";
            try
            {
                jsonText = File.ReadAllText(string.Format("{0}/state.txt", dataRootPath));
                _jsonDictState = MiniJSON.Json.Deserialize(jsonText) as Dictionary<string, object>;
                Debug.Log("BigboardServer._StateLoad() : Load success!");
            }
            catch (IOException e)
            {
                Debug.Log("BigboardServer._StateLoad() : state.txt load failed. trying to make default file.");
                Debug.LogException(e);

                _jsonDictState = new Dictionary<string, object>();
                _StateSave();
            }
            catch (System.Exception e)
            {
                Debug.Log("BigboardServer._StateLoad() : state.txt load failed.");
                Debug.LogException(e);
            }
        }

        _cachedSituationalInfo.ReadJsond(_jsonDictState);
    }

    private static void _StateSave()
    {
        string jsonText = MiniJSON.Json.Serialize(_jsonDictState);
        try
        {
            File.WriteAllText(string.Format("{0}/state.txt", dataRootPath), jsonText);
        }
        catch (System.Exception e)
        {
            Debug.Log("BigboardServer._StateSave() : state.txt save failed.");
            Debug.LogException(e);
        }
    }

#if BIGBOARD_STANDALONE
    public static void StandaloneDataReload()
    {
        _jsonDictStandAlone = null;
        _cachedContents.Clear();
        _StandaloneDataLoad();
    }

    private static void _StandaloneDataLoad()
    {
        if (_jsonDictStandAlone == null)
        {
            string jsonText = "";
            try
            {
                jsonText = File.ReadAllText(string.Format("{0}/{1}.txt", dataRootPath, strStandAloneFileName));
                _jsonDictStandAlone = MiniJSON.Json.Deserialize(jsonText) as Dictionary<string, object>;
                Debug.Log("BigboardServer._StateLoad() : Load success!");
            }
            catch (IOException e)
            {
                Debug.Log("BigboardServer._StateLoad() : StandAloneData.txt load failed. trying to make default file.");
                Debug.LogException(e);

                Debug.Log(string.Format("BigboardServerData/{0}", strStandAloneFileName));
                TextAsset txtAsset = Resources.Load(string.Format("Data/BigboardServer_StandAlone/{0}",strStandAloneFileName), typeof(TextAsset)) as TextAsset;               
                _jsonDictStandAlone = MiniJSON.Json.Deserialize(txtAsset.text) as Dictionary<string, object>;

                _StandaloneDataSave();
            }
            catch (System.Exception e)
            {
                Debug.Log("BigboardServer._StateLoad() : StandAloneData.txt load failed.");
                Debug.LogException(e);
            }


            // Read standalone data
            Dictionary<int, object> data = null;
            if (_jsonDictStandAlone != null)
            {
                data = new Dictionary<int, object>();
                if (_jsonDictStandAlone.ContainsKey("contents") && _jsonDictStandAlone["contents"] != null)
                {
                    List<object> allContentsList = _jsonDictStandAlone["contents"] as List<object>;
                    for (int i = 0; i < allContentsList.Count; i++)
                    {
                        Dictionary<string, object> contentsDict = allContentsList[i] as Dictionary<string, object>;
                        List<object> contentsList = contentsDict["list"] as List<object>;
                        if (contentsList != null)
                        {
                            for (int j = 0; j < contentsList.Count; j++)
                            {
                                Dictionary<string, object> contentInfo = contentsList[j] as Dictionary<string, object>;
                                if (contentInfo.ContainsKey("seq") && contentInfo["seq"] != null)
                                {
                                    data.Add(int.Parse(contentInfo["seq"].ToString()), contentInfo);
                                    if (!contentInfo.ContainsKey("use_yn"))
                                    {
                                        contentInfo.Add("use_yn", "Y");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
                Debug.Log("_jsonDictStandAlone is Null");

            _StandaloneDataSave();

            // Set Read Data
            for (int i = 0; i < Enum.GetValues(typeof(BigboardContentMode)).Length; i++)
            {
                BigboardContentMode mode = (BigboardContentMode)Enum.GetValues(typeof(BigboardContentMode)).GetValue(i);
                if (mode != BigboardContentMode.None)
                {
                    ContentsStoreItemType.Mode typeCode = _GetItemTypeModeToContentMode(mode);
                    ContentsStoreItemType contentType = new ContentsStoreItemType(typeCode, typeCode.ToString());
                    ContentsStoreItemInfo itemInfo = new ContentsStoreItemInfo(contentType, mode);

                    // Set standalone data
                    if (data != null)
                    {
                        int dataSeq = (int)mode;
                        if (data.ContainsKey(dataSeq) && data[dataSeq] != null)
                        {
                            Dictionary<string, object> item = data[dataSeq] as Dictionary<string, object>;
                            if (item.ContainsKey("type") && item["type"] != null)
                            {
                                contentType = new ContentsStoreItemType(typeCode, item["type"].ToString());
                                itemInfo = new ContentsStoreItemInfo(contentType, mode);
                            }
                            itemInfo.StandAloneDataParse(item);
                        }
                    }

                    // Add data 
                    if (_cachedContents.ContainsKey(typeCode) == false)
                        _cachedContents.Add(typeCode, new List<ContentsStoreItemInfo>());

                    _cachedContents[typeCode].Add(itemInfo);
                }
            }
            // Sort all content types by type code
            foreach (KeyValuePair<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>> item in _cachedContents)
            {
                item.Value.Sort((a, b) =>
                {
                    return a.type.typeCode.CompareTo(b.type.typeCode);
                });
            } 
        }
    }

    private static void _StandaloneDataSave()
    {
        string jsonText = Regex.Unescape(MiniJSON.Json.Serialize(_jsonDictStandAlone));
        try
        {
            File.WriteAllText(string.Format("{0}/{1}.txt", dataRootPath, strStandAloneFileName), jsonText);
        }
        catch (System.Exception e)
        {
            Debug.Log("BigboardServer._StateSave() : state.txt save failed.");
            Debug.LogException(e);
        }
    }
#endif

    private class BigboardServerOrder
    {
        public List<ContentsStoreItemType.Mode> mMainOrder = new List<ContentsStoreItemType.Mode>();
        public Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>> mSubOrder = new Dictionary<ContentsStoreItemType.Mode, List<ContentsStoreItemInfo>>();
    }
    #endregion
}
