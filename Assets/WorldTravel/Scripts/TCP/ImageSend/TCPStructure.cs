using UnityEngine;
using System.Net.Sockets;
using System.Threading;


public class ConnectionInfo
{
    public Socket Socket;
    public Thread Thread;

    public string _model = null;
    public string _id = null;
    public int _type = -1;
    public int _conn_type = -1;
    public bool _responce = false;
}

public class InfoData
{
    public InfoData(ConnectionInfo c, string d) { connection = c; data = d; }
    public ConnectionInfo connection;
    public string data;
}

public class InfoImage
{
    public InfoImage(Socket _server, ImageInfo i)
    {
        server = _server;
        image = i;
    }
    public Socket server;
    public ConnectionInfo connection;
    public ImageInfo image;
}

public class TCP
{
    public const int BUFFER_SIZE = 4096;

    public const byte DATA_STRING = 0x02;
    public const byte DATA_IMAGE = 0x20;

    public const int DATA_END = 0x03;

    public static bool IsStart(byte b)
    {
        return b == DATA_STRING || b == DATA_IMAGE;
    }

}
public class ImageInfo
{
    public int id;
    public int width;
    public int height;
    public byte[] data;

    private Texture2D _texture;
    public Texture2D texture
    {
        get
        {
            if (_texture == null)
            {
                _texture = new Texture2D(width, height, TextureFormat.RGB24, false);
                _texture.LoadImage(data);
            }
            return _texture;
        }
    }
}

public class FunctionInfo
{
    private const string KEY_FUNCNAME = "FuncName";
    private const string KEY_DATA = "Data";

    string strFunc = null;
    JSONObject jsonData = null;

    private FunctionInfo()
    {

    }

    public static FunctionInfo Send(string strAPI, int n)
    { return Send(strAPI, new JSONObject(n));  }

    public static FunctionInfo Send(string strAPI, float n)
    { return Send(strAPI, new JSONObject(n)); }

    public static FunctionInfo Send(string strAPI, bool b)
    { return Send(strAPI, new JSONObject(b)); }

    public static FunctionInfo Send(string strAPI, string str)
    { return Send(strAPI, new JSONObject(){type = JSONObject.Type.STRING, str = str}); }

    /// <summary>
    /// TCP Function 전송
    /// </summary>
    /// <param name="strAPI"> 호출 함수명</param>
    /// <param name="data"> 전송할 데이터</param>
    /// <returns></returns>
    public static FunctionInfo Send(string strAPI, JSONObject data = null)
    {
        FunctionInfo func = new FunctionInfo();
        func.strFunc = strAPI;
        func.jsonData = data;

        return func;
    }
    
    public static FunctionInfo ReadJSON(JSONObject json)
    {
        if( json != null)
        {
            JSONObject name = json.GetField(KEY_FUNCNAME);
            if( name != null)
            {
                FunctionInfo func = new FunctionInfo();
                func.strFunc = name.str;

                func.jsonData = json.GetField(KEY_DATA);

                return func;
            }
            Debug.LogWarning("FuncName is NULL!!");
        }
        return null;
    }

    public JSONObject GetJSONObject()
    {
        if( strFunc != null)
        {
            JSONObject json = new JSONObject();
            json.AddField(KEY_FUNCNAME, strFunc);

            if (jsonData != null)
                json.AddField(KEY_DATA, jsonData);

            return json;
        }
        return null;
    }

    public JSONObject Receive(string strAPI)
    {
        if (strFunc.Contains(strAPI))
        {
            if (jsonData == null)
                jsonData = new JSONObject();
            return jsonData;
        }
        return null;
    }
    

    public FunctionInfo Clone()
    {
        FunctionInfo info = new FunctionInfo();

        info.strFunc = strFunc;
        info.jsonData = jsonData.Copy();

        return info;
    }
}