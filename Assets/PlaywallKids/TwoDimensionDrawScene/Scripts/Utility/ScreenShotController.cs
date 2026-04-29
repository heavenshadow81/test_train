using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;

public class ScreenShotController : MonoBehaviour
{
    const string LOOPBACK_IP = "127.0.0.1";
    const string PORT = "8080";
    public GameObject buttonPrefab;
    public Camera printCamera;
    public UITable table;

    public UIButton okButton;

    public bool Active
    {
        get
        {
            return this.gameObject.activeInHierarchy;
        }

        set
        {
            this.gameObject.SetActive(value);
        }
    }

    private bool bTakeAPicture;
    private bool bSendData;
    private bool bGenerateList;
    private string nfcID;
    private string ipAdress;
    private string port;

    private EventDelegate callback;

    string FullPath
    {
        get
        {
            return string.Format("{0}/SettingFile/{1}", Application.dataPath, "settings.txt");
        }
    }

    void Awake()
    {
        string[] _file = null;
        if (System.IO.File.Exists(FullPath))
        {
            _file = System.IO.File.ReadAllLines(FullPath, System.Text.Encoding.UTF8);
            if (_file.Length > 0)
            {
                string[] _info = _file[0].Split(new char[] { ',' });
                try
                {
                    Debug.Log("Load settings file");
                    ipAdress = _info[0];
                    port = _info[1];
                }
                catch (Exception e)
                {
                    Debug.LogError("Fail writing settings file: " + e.Message + " Set default ip");
                }
            }
        }

        if (_file == null || _file.Length <= 0)
        {
            System.IO.Directory.CreateDirectory( string.Format( "{0}/SettingFile", Application.dataPath) );
            System.IO.File.WriteAllText(string.Format(FullPath), string.Format("{0},{1}", LOOPBACK_IP, PORT));
            ipAdress = LOOPBACK_IP;
            port = PORT;
        }
    }

    void OnEnable()
    {
        nfcID = "";
        bSendData = false;
        bGenerateList = false;

        if (okButton)
        {
            if (okButton.onClick.Count > 0)
                callback = okButton.onClick[0];
            else
            {
                UIEventTrigger _trigger = okButton.GetComponent<UIEventTrigger>();
                if(_trigger != null && _trigger.onClick.Count > 0)
                {
                    callback = _trigger.onClick[0];
                }
            }
            okButton.gameObject.SetActive(false);
        }

        UIButton _btn = GetComponentInChildren<UIButton>();
        if(_btn)
            _btn.onClick.Add(new EventDelegate(()=>_btn.gameObject.SetActive(false)) );
    }

	void LateUpdate()
    {
        if(bTakeAPicture)
        {
            bTakeAPicture = false;
            printCamera.gameObject.SetActive(true);
            
            const int width = 1920;
            const int height = 1080;
            const float ratio = 0.25f;
            int _width = (int)(width * printCamera.rect.width * ratio);
            int _height = (int)(height * printCamera.rect.height * ratio);

            RenderTexture rt = new RenderTexture((int)(width * ratio), (int)(height * ratio), 24);

            printCamera.targetTexture = rt;

            Texture2D screenShot = new Texture2D(_width, _height, TextureFormat.RGB24, false);
            printCamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, _width, _height), 0, 0);
            printCamera.targetTexture = null;
            RenderTexture.active = null;
                    
#if UNITY_EDITOR
//                test.mainTexture = Instantiate( screenShot) as Texture;
#endif
            Destroy(rt);
                
            string filePath = string.Format("{0}/ScreenShot", Application.dataPath);

            if(!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
                
            string fileName = string.Format("{0}/picture_{1}.png", filePath, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            byte[] bytes = screenShot.EncodeToPNG();
#if USE_TAG
            if (!bSendData)
            {
                bSendData = true;
                FormUpload.SendUserImage(bytes,ipAdress, port,  nfcID, ML.PlaywallKids.Common.NFCConstants.VALUE_3D_DRAWING, fileName);
            }
#endif 
            nfcID = "";
            File.WriteAllBytes( fileName, bytes);
            printCamera.gameObject.SetActive(false);

            if (callback != null)
            {
                callback.Execute();
            }else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void TakeAScreenshot(string _nfcID)
    {
        nfcID = _nfcID;
        bSendData = false;
        bTakeAPicture = true;
    }

#if USE_TAG
    public void GenerateList()
    {
        if (bGenerateList) return;
        bGenerateList = true;
        StartCoroutine(GetUserInforamtionsProcess());
    }
    
    IEnumerator GetUserInforamtionsProcess()
    {
        string _postURL = string.Format("http://{0}:{1}/getUserList?{2}={3}",ipAdress, port, ML.PlaywallKids.Common.NFCConstants.PARAM_CONTENT_TYPE, ML.PlaywallKids.Common.NFCConstants.VALUE_3D_DRAWING); // string.Format( "http://1.225.61.27:8096/getUserList?{0}={1}",ML.PlaywallKids.Common.LayerConstants.PARAM_CONTENT_TYPE, ML.PlaywallKids.Common.LayerConstants.VALUE_3D_DRAWING);
        WWW www = new WWW(_postURL);
        yield return www;

        List<Transform> _list = table.GetChildList();
        if(_list.Count > 0)
        {
            for(int i = 0 ,len = _list.Count; i<  len; ++i)
                Destroy(_list[i].gameObject);
        }

        yield return null;

        try
        {
            string _value = www.text;
            Dictionary<string, object> _dic = MiniJSON.Json.Deserialize(_value) as Dictionary<string, object>;
            if( _dic.ContainsKey(ML.PlaywallKids.Common.NFCConstants.KEY_USERLIST))
            {
                List<object> list  = _dic[ML.PlaywallKids.Common.NFCConstants.KEY_USERLIST] as List<object>;
                if (list.Count > 0)
                {
                    for (int i = 0, len = list.Count; i < len; ++i)
                    {
                        Dictionary<string, object> _info = list[i] as Dictionary<string, object>;
                        string _name = _info[ML.PlaywallKids.Common.NFCConstants.SUBKEY_USER_NAME].ToString();
                        string _nfcID = _info[ML.PlaywallKids.Common.NFCConstants.SUBKEY_NFCID].ToString();
                        GameObject _go = NGUITools.AddChild(table.gameObject, buttonPrefab);
                        _go.name = _nfcID;

                        UILabel _label = _go.GetComponentInChildren<UILabel>();
                        _label.text = _name;

                        UIButton _btn = _go.GetComponentInChildren<UIButton>();
                        EventDelegate _event = new EventDelegate(this, "TakeAScreenshot");
                        _event.parameters[0] = new EventDelegate.Parameter(_go.name);
                        _btn.onClick.Add(_event);

                        //_btn.onClick.Add(new EventDelegate(nextStep ?? ( ()=>this.gameObject.SetActive(false) )));

                    }
                    table.Reposition();
                }else
                {
                    okButton.gameObject.SetActive(true);
                }
            }
            else
            {
                okButton.gameObject.SetActive(true);
            }

        }catch(System.Exception e)
        {
            Debug.Log(e.Message);
        }
        bGenerateList = false;
    }

    //실제 전송 함수, 인자값 설정
    public static class FormUpload
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        public static void SendUserImage(byte[] dataSet, string _ip, string _port,  string _nfcID, string _contentsType, string _fullFilePath)
        {
            /*
             FileStream fs = new FileStream("파일 전체 경로", FileMode.Open, FileAccess.read);
             byte[] data = new byte[fs.Length];
             fs.Read(data, 0, data.Length);
             fs.Close();
             */
            Dictionary<string, object> postParameters = new Dictionary<string, object>()
            {
                {ML.PlaywallKids.Common.NFCConstants.PARAM_FILE, new FormUpload.FileParameter(dataSet, _fullFilePath , "image/Png")},
                {ML.PlaywallKids.Common.NFCConstants.PARAM_CONTENT_TYPE , _contentsType},
                {ML.PlaywallKids.Common.NFCConstants.PARAM_SHARE_OPTION , "N"},
                {ML.PlaywallKids.Common.NFCConstants.PARAM_NFC_ID, _nfcID}
            };

        //http://1.225.61.27:8096/
            string _postURL = string.Format("http://{0}:{1}/setPhoto", _ip, _port);//"http://1.225.61.27:8096/setPhoto";
            string userAgent = "";
            HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(_postURL, userAgent, postParameters);

            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
            string fullResponse = responseReader.ReadToEnd();
            if (!fullResponse.Contains(ML.PlaywallKids.Common.NFCConstants.RETURN_CODE_ZERO))
            {
                Loom.QueueOnMainThread(() => Debug.Log("Fail uploading to server : Return Code  ==" + fullResponse));
            }

            webResponse.Close();
        }

        public static HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent, Dictionary<string, object> postParameters)
        {
            string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;
            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

            return PostForm(postUrl, userAgent, contentType, formData);
        }

        private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // You could add authentication here as well if needed:
            // request.PreAuthenticate = true;
            // request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            // request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes("username" + ":" + "password")));

            // Send the form data to the request.
            try
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(formData, 0, formData.Length);
                    requestStream.Close();
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return request.GetResponse() as HttpWebResponse;
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.FileFormatType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();
            string s = Encoding.UTF8.GetString(formData);

            return formData;
        }

        public class FileParameter
        {
            public byte[] File { get; set; }
            public string FileName { get; set; }
            public string FileFormatType { get; set; }
            public FileParameter(byte[] file) : this(file, null) { }
            public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
            public FileParameter(byte[] file, string filename, string fileType)
            {
                File = file;
                FileName = filename;
                FileFormatType = fileType;
            }
        }
    }
#endif
}
