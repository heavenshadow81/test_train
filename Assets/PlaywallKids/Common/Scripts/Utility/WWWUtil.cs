using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct WWWBinaryData {
    #region Public variables
    public string fieldName;

    public byte[] contents;

    public string fileName;

    public string mimeType;
    #endregion

    #region Constructors
    public WWWBinaryData(string newFieldName, byte[] newContents)
    {
        fieldName = newFieldName;
        contents = newContents;
        fileName = "";
        mimeType = "application/octet-stream";
    }
    #endregion
}

public class WWWRequest {
    #region Structs
    #endregion

    #region Public variables
    // URL
	public string url = "";

	// HTTP method
	public WWWUtil.HTTPMethod method = WWWUtil.HTTPMethod.GET;
	#endregion

	#region Properties
	// header
	private Dictionary<string, string> _header = new Dictionary<string, string>();
	public Dictionary<string, string> header {
		get {
			return _header;
		}
		set {
			if(value == null) {
				_header = new Dictionary<string, string>();
			}
			else {
				_header = value;
			}
		}
	}

	// parameters
	private Dictionary<string, string> _parameters = new Dictionary<string, string>();
	public Dictionary<string, string> parameters {
		get {
			return _parameters;
		}
		set {
			if(value == null) {
				_parameters = new Dictionary<string, string>();
			}
			else {
				_parameters = new Dictionary<string, string>(value);
			}
		}
	}

    // binary datas
    private List<WWWBinaryData> _binaryDatas = new List<WWWBinaryData>();
    public List<WWWBinaryData> binaryDatas
    {
        get
        {
            return _binaryDatas;
        }
        set
        {
            if (value == null)
            {
                _binaryDatas = new List<WWWBinaryData>();
            }
            else
            {
                _binaryDatas = new List<WWWBinaryData>(value);
            }
        }
    }

	// timeout
	private float _timeout = 10.0f;
	public float timeout {
		get {
			return _timeout;
		}
		set {
			_timeout = Mathf.Max(5.0f, _timeout);
		}
	}
	#endregion
}

public class WWWUtil : MonoBehaviour {
	#region Enums
	public enum HTTPMethod {
		GET = 0,
		POST = 1
	}
	
	public enum ConnectionResult {
		Timeout = -1,
		Success = 0,
		Error = 1,
        Cancel = 2
	}
	#endregion

	#region Private classes
	private class Connection {
		public WWWRequest request = null;

		public WWW www = null;

		public float time = 0.0f;

		public Handler handler = null;
		
		public float prevUploadProgress = 0.0f;
		public float prevProgress = 0.0f;

		public string url {
			get {
				return request.url;
			}
		}

		public bool isDone {
			get {
				return www.isDone;
			}
		}

		public float progress {
			get {
				return www.progress;
			}
		}

		public float uploadProgress {
			get {
				return www.uploadProgress;
			}
		}

		public Connection(WWWRequest request) {
			this.request = request;

			WWWForm form = new WWWForm();
			string requestUrl = request.url;
			var headers = form.headers;

            if (!request.header.ContainsKey("Content-Type") && request.binaryDatas.Count > 0)
                request.header["Content-Type"] = "multipart/form-data";

			if(request.header.Count > 0) {
				foreach(string key in request.header.Keys) {
					headers[key] = request.header[key];
				}
			}

            if (request.binaryDatas.Count > 0)
            {
                foreach (var binaryData in request.binaryDatas)
                {
                    if (!string.IsNullOrEmpty(binaryData.fieldName))
                        form.AddBinaryData(binaryData.fieldName, binaryData.contents, binaryData.fileName, binaryData.mimeType);
                }
            }
			
			if (request.parameters.Count > 0) {
				if(request.method == HTTPMethod.POST) {
					// set up parameters
					foreach (string key in request.parameters.Keys)
					{
						string val = request.parameters[key];
						if (val != null)
							form.AddField(key, val);
					}
				}
				else if(request.method == HTTPMethod.GET) {
					requestUrl = string.Format("{0}?{1}", request.url, _GetHTTPParamsString(request.parameters));
				}
			}

			if(request.method == HTTPMethod.GET)
            {
                byte[] rawData = form.data;
                if (rawData.Length > 0)
                    www = new WWW(requestUrl, rawData, headers);
                else if (headers.Count != form.headers.Count)
                {
                    // we need to add dummy data for adding headers
                    form.AddField("dummy", "1234");
                    rawData = form.data;
                    www = new WWW(requestUrl, rawData, headers);
                }
                else
                    www = new WWW(requestUrl);
			}
			else {
				www = new WWW(requestUrl, form);
			}

			// Show log(debug build)
			if (Debug.isDebugBuild)
			{
				string headerString = _GetHTTPHeaderString(request.header);
				string paramsString = _GetHTTPParamsString(request.parameters);
				
				DebugUtil.DLog(string.Format("Access to {0} ... -------------------------", request.url));
				if(!string.IsNullOrEmpty(headerString)) {
					DebugUtil.DLogFormat("Headers : {0}", headerString);
				}
				if(!string.IsNullOrEmpty(paramsString)) {
					DebugUtil.DLogFormat("Parameters : {0}", paramsString);
				}
			}
		}
		
		private string _GetHTTPHeaderString(Dictionary<string, string> headerData) {
			if(headerData == null)
				return "";
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			bool first = true;
			foreach (string key in headerData.Keys)
			{
				if (headerData[key] != null)
				{
					sb.AppendFormat("{0}[\"{1}\"]=\"{2}\"", (first?"":", "), key, headerData[key]);
					first = false;
				}
			}
			return sb.ToString();
		}
		
		private string _GetHTTPParamsString(Dictionary<string, string> parameters) {
			if(parameters == null)
				return "";
			
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			bool first = true;
			foreach (string key in parameters.Keys)
			{
				if (parameters[key] != null)
				{
					sb.AppendFormat("{0}{1}={2}", (first?"":"&"), key, parameters[key]);
					first = false;
				}
			}
			return sb.ToString();
		}
	}
	#endregion

	#region Properties
	private static WWWUtil __sharedInstance = null;
	public static WWWUtil sharedInstance {
		get {
			if(__sharedInstance == null) {
				GameObject go = new GameObject("WWWUtil");
				__sharedInstance = go.AddComponent<WWWUtil>();
				DontDestroyOnLoad(go);
			}
			return __sharedInstance;
		}
	}
	#endregion

	#region Private variables
	private Dictionary<int, Connection> _connections = new Dictionary<int, Connection>();

	private List<int> _endedConnectionCodes = new List<int>();
	#endregion

	#region Handler
	public delegate void Handler(ConnectionResult result, WWW www);
	#endregion

	#region Unity Methods
	public void Update() {
		var enumerator = _connections.Keys.GetEnumerator();

		while(enumerator.MoveNext()) {
			var key = enumerator.Current;

			Connection connection = _connections[key];

			if(connection.isDone) {
				_endedConnectionCodes.Add(key);
			}
			else if(connection.time >= connection.request.timeout) {
				connection.www.Dispose();
				connection.www = null;
				
				_endedConnectionCodes.Add(key);
			}
			else {
				if(connection.prevProgress.Equals(connection.progress) &&
				   connection.prevUploadProgress.Equals(connection.uploadProgress)) {
					connection.time += Time.deltaTime;
				}
				else {
					connection.time = 0.0f;
				}

				connection.prevProgress = connection.progress;
				connection.prevUploadProgress = connection.uploadProgress;
			}
		}

		for(int i = 0; i < _endedConnectionCodes.Count; i++) {
			int code = _endedConnectionCodes[i];
			Connection connection = _connections[code];
			WWW www = connection.www;
			Handler handler = connection.handler;

			if (www != null)
			{
				// Error?
				if (string.IsNullOrEmpty(www.error) == false)
				{
					DebugUtil.DLog(string.Format("WWWUtil._PerformHTTPRequest({0}) : Failed to connect. reason:{1}", connection.url, www.error));
					
					if (handler != null)
						handler(ConnectionResult.Error, www);
				}
				// Success
				else
				{
					DebugUtil.DLog(string.Format("WWWUtil._PerformHTTPRequest({0}) : Success.", System.IO.Path.GetFileNameWithoutExtension(connection.url)));
					
					if (handler != null)
						handler(ConnectionResult.Success, www);
				}
			}
			// Timeout
			else
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log(string.Format("WWWUtil._PerformHTTPRequest({0}) : Timeout.", connection.url));
				}
				
				if (handler != null)
					handler(ConnectionResult.Timeout, null);
			}

			_connections.Remove(code);
		}

		_endedConnectionCodes.Clear();
	}
	#endregion

	#region Request Control
	public static float GetProgress(int code) {
		return sharedInstance._PerformGetProgress(code);
	}
	
	private float _PerformGetProgress(int code) {
		if(_connections.ContainsKey(code)) {
			Connection connection = _connections[code];
			
			return connection.progress;
		}
		
		return 0.0f;
	}
	
	public static float GetUploadProgress(int code) {
		return sharedInstance._PerformGetUploadProgress(code);
	}
	
	private float _PerformGetUploadProgress(int code) {
		if(_connections.ContainsKey(code)) {
			Connection connection = _connections[code];
			
			return connection.uploadProgress;
		}
		
		return 0.0f;
	}

	public static void CancelRequest(int code) {
		sharedInstance._PerformCancelRequest(code);
	}

	private void _PerformCancelRequest(int code) {
		if(_connections.ContainsKey(code)) {
			Connection connection = _connections[code];
			
			connection.www.Dispose();
			connection.www = null;
			
			_connections.Remove(code);

			if(connection.handler != null) connection.handler(ConnectionResult.Cancel, null);
		}
	}
	#endregion

	#region Requests
	public static int HTTPRequest(string url, Handler handler) {
		WWWRequest request = new WWWRequest();
		request.url = url;
		request.method = HTTPMethod.GET;
		return sharedInstance._PerformHTTPRequest(request, handler);
	}

	public static int HTTPRequest(string url, HTTPMethod method, Dictionary<string, string> parameters, Handler handler) {
		WWWRequest request = new WWWRequest();
		request.url = url;
		request.method = method;
		request.parameters = parameters;
		return sharedInstance._PerformHTTPRequest(request, handler);
	}

	public static int HTTPRequest(string url, Dictionary<string, string> headers, HTTPMethod method, Dictionary<string, string> parameters, Handler handler) {
		WWWRequest request = new WWWRequest();
		request.url = url;
		request.method = method;
		request.header = headers;
		request.parameters = parameters;
		return sharedInstance._PerformHTTPRequest(request, handler);
	}
	
	public static int HTTPRequest(WWWRequest request, Handler handler) {
		return sharedInstance._PerformHTTPRequest(request, handler);
	}

	private int _PerformHTTPRequest(WWWRequest request, Handler handler) {
		if(request == null) {
			DebugUtil.Log("WWWUtil._PerformHTTPRequest() : invalid request. request is null.");

			if(handler != null) {
				handler(ConnectionResult.Cancel, null);
			}

			return 0;
		}

		Connection connection = new Connection(request);
		connection.handler = handler;

		int code = 1;

		while(true) {
			if(!_connections.ContainsKey(code) || _connections[code] == null) {
				_connections[code] = connection;
				break;
			}
			else {
				code += 1;
			}
		}

		return code;
	}
	#endregion
}
