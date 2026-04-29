using UnityEngine;

#if USE_TAG
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Keys = ML.PlaywallKids.Common.NFCConstants;

public struct NFCUserInfo
{
    public int seqNo;
    public string userName;

    public static NFCUserInfo GetNFCUserInfo(string _jsonStr)
    {
        if (string.IsNullOrEmpty(_jsonStr)) return new NFCUserInfo();

        Dictionary<string, object> _dic = MiniJSON.Json.Deserialize(_jsonStr) as Dictionary<string, object>;
        if (!_dic.ContainsKey(Keys.KEY_CONTENTS_SEQ) && !_dic.ContainsKey(Keys.KEY_CONTENTS_SEQ)) return new NFCUserInfo();

        string _userName = _dic[Keys.KEY_USER_NAME].ToString();
        int _seq = int.Parse(_dic[Keys.KEY_CONTENTS_SEQ].ToString());

        return new NFCUserInfo() { seqNo = _seq, userName = _userName };
    }

    public static void SendData(int _seq, int _score)
    {
        NFCClientSocket.instance.SendData(
                   new Dictionary<string, object>(){
                                {Keys.KEY_CONTENTS_SEQ, _seq.ToString()},
                                {Keys.KEY_RESULT_SCORE, _score.ToString()}
                            });
    }
}

public class StringObject
{
    public int Count
    {
        get
        {
            if (count < 0) count = stringList.Count;
            return count;
        }
    }

    List<string> stringList;
    int count;

    public StringObject()
    {
        stringList = new List<string>();
        count = 0;
    }

    ~StringObject()
    {
        stringList.Clear();
        stringList.TrimExcess();
        stringList = null;
    }

    public void SetStringValue(string _jsonStr)
    {
        if (!string.IsNullOrEmpty(_jsonStr))
        {
            stringList.Add(_jsonStr);
            ++count;
        }
    }

    public string GetStringValue()
    {
        if (stringList.Count > 0)
        {
            string _s = stringList[0];
            stringList.RemoveAt(0);
            --count;
            return _s;
        }
        return null;
    }
}
#endif

public class NFCClientSocket : MonoBehaviour
{
#if USE_TAG
    private static NFCClientSocket _instance = null;
    public static NFCClientSocket instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<NFCClientSocket>();

                try
                {
                    if (!_instance)
                    {
                        _instance = FindObjectOfType<NFCClientSocket>();
                        if (!_instance && !didDestroyed)
                        {
                            _instance = (new GameObject("NFCClientSocket")).AddComponent<NFCClientSocket>();
                            bool bConnected = _instance.DoConnecting(Keys.LOOPBACK_IP, Keys.NUMBER_OF_PORT);
                            //bool bConnected = _instance.DoConnecting("192.168.40.114", Keys.NUMBER_OF_PORT);
                            if (!bConnected)
                                Debug.LogError("Fail the Connecting Server");
                            else
                                Debug.Log("Success the Connecting Server");
                            notNull = true;
                        }
                    }

                }
                catch
                {
                    Debug.LogError("Failed generating Socket of client");
                }

            }
            return _instance;
        }
    }

    public static bool notNull;

    public string ContentType
    {
        get
        {
            return _registedContentType;
        }

        set
        {
            if (!string.IsNullOrEmpty(value) && clientSocket != null)
            {
                if (string.Compare(value, _registedContentType) != 0)
                {
                    _registedContentType = value;
                    SendData(new Dictionary<string, object>() { { Keys.KEY_CONTENT_TYPE, value } });
                }
            }
        }
    }

    public int Count
    {
        get
        {
            return stringObject.Count;
        }
    }

    public bool Conneted
    {
        get
        {
            if (clientSocket != null) return clientSocket.Connected;

            return false;
        }
    }

    public delegate void Callback_RecevieData(string _jsonString);

    private Callback_RecevieData OnReceiveDataSet;
    private StringObject stringObject;
    private static bool didDestroyed;
    private Socket clientSocket = null;
    private byte[] receiveDataArr = new byte[255];

    private List<string> jsonList;
    private List<string[,]> dataList;

    private Thread thread;
    private string _registedContentType;

    void Awake()
    {
        stringObject = new StringObject();
        DontDestroyOnLoad(this.gameObject);
        jsonList = new List<string>();
        dataList = new List<string[,]>();
        _registedContentType = "";
        Loom.Initialize();
    }

    void OnDestroy()
    {
        notNull = false;
        didDestroyed = true;
        CloseSocket();
        if (thread != null)
            thread.Abort();
        _instance = null;
    }

    public bool DoConnecting(string _ip, string _port)
    {
        try
        {
            if (clientSocket != null) clientSocket.Close();
            IPEndPoint _ipEndPoint = new IPEndPoint(IPAddress.Parse(_ip), Int32.Parse(_port));

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            { NoDelay = true };
            clientSocket.Connect(_ipEndPoint);

            if (thread != null)
            {
                thread.Abort();
                thread = null;
            }

            thread = new Thread(
               new ThreadStart(
            #region StartThread
                    () =>
                    {
                        while (true)
                        {
                            try
                            {
                                int _byteLength = clientSocket.Receive(receiveDataArr);
                                if (_byteLength > 0)
                                {
                                    ReceiveJsonData(receiveDataArr);
                                    if (jsonList.Count > 0)
                                    {
                                        try
                                        {
                                            string _value = string.Copy(jsonList[0]);
                                            jsonList.RemoveAt(0);
                                            stringObject.SetStringValue(_value);
                                            if (OnReceiveDataSet != null)
                                                OnReceiveDataSet(_value);
                                        }
                                        catch (Exception e)
                                        {
                                            Loom.QueueOnMainThread(() => Debug.Log("fail OnReceiveDataSet : " + e.Message));
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                if (thread != null)
                                {
                                    thread.Abort();
                                }
                                else
                                {
                                    Loom.QueueOnMainThread(() => Debug.LogError("NFCClientSocket was DoConnected - " + ex.Message));
                                }
                                CloseSocket();
                            }
                            Thread.Sleep(1);
                        }
                    }
                    #endregion
                )
            )
            { IsBackground = true };

            thread.Start();
        }
        catch (Exception e)
        {
            Loom.QueueOnMainThread(() => Debug.Log(e.Message));
            return false;
        }

        return clientSocket.Connected;
    }

    public string GetStringValue()
    {
        return stringObject.GetStringValue();
    }

    public void SetCallback(Callback_RecevieData _callback)
    {
        _instance.OnReceiveDataSet = _callback;
    }

    public void AddCallback(Callback_RecevieData _callback)
    {
        _instance.OnReceiveDataSet += _callback;
    }

    public void CloseSocket()
    {
        if (clientSocket != null)
            clientSocket.Close();
        clientSocket = null;
    }

    public bool SendData(Dictionary<string, object> _datas)
    {
        if (_datas == null) return false;
        byte[] _bytes = UtilityScript.ToByteArray(MiniJSON.Json.Serialize(_datas));
        if (_bytes != null && _bytes.Length > 0 && clientSocket != null && clientSocket.Connected)
        {
            int _returnValue = clientSocket.Send(_bytes);


            return true;
        }

        return false;
    }

    private void ReceiveJsonData(byte[] jsonArray)
    {
        int length = BitConverter.ToInt32(jsonArray, 0);
        string jsonStr = UtilityScript.ByteArryToStr(jsonArray, length);

        jsonList.Add(jsonStr);
    }
#endif
}