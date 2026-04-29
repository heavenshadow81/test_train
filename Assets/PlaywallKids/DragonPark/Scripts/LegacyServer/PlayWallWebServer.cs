using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PlayWallWebServer {
	public const string kRootUrl = "http://1.215.249.42:9004";

	public static void GetName(int userSeq, System.Action<string> handler) {
		List<int> list = new List<int>();
		list.Add(userSeq);
		GetNames(list, (names)=>{
			if(handler != null) {
				handler(names[0]);
			}
		});
	}

	public static void GetNames(List<int> userSeqs, System.Action<List<string>> handler) {
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		sb.AppendFormat("{0}/user/userSearch.do?", kRootUrl);
		for(int i = 0; i < userSeqs.Count; i++) {
			sb.AppendFormat("userSeq={0}{1}", userSeqs[i], (i + 1 == userSeqs.Count) ? "" : "&");
		}

		WWWUtil.HTTPRequest(sb.ToString(), WWWUtil.HTTPMethod.GET, null, (connection, www)=>{
			List<string> userNames = new List<string>();
			for(int i = 0; i < userSeqs.Count; i++) {
				userNames.Add("");
			}

			if(connection == WWWUtil.ConnectionResult.Success) {
				string jsonText = www.text;
				Dictionary<string, object> dict = MiniJSON.Json.Deserialize(jsonText) as Dictionary<string, object>;
				bool success = false;
				if(dict.ContainsKey("success") && dict["success"] != null) {
					success = bool.Parse(dict["success"].ToString());
				}
				if(success) {
					List<object> list = dict["data"] as List<object>;
					foreach(object obj in list) {
						Dictionary<string, object> info = obj as Dictionary<string, object>;
						string userName = info["name"].ToString();
						int userSeq = int.Parse(info["userSeq"].ToString());
						int index = userSeqs.IndexOf(userSeq);

						if(index > -1) {
							userNames[index] = userName;
						}
					}
				}
			}

			if(handler != null) {
				handler(userNames);
			}
		});
	}

	public static void GetPicture(int userSeq, System.Action<Texture2D> handler) {
		_GetPictureInfo(userSeq, (dict)=>{
			if(dict.ContainsKey("userPhtflNm") && dict["userPhtflNm"] != null) {
				string url = dict["userPhtflNm"].ToString();

				WWWUtil.HTTPRequest(url, WWWUtil.HTTPMethod.GET, null, (connection, www)=>{
					Texture2D texture = null;
					if(connection == WWWUtil.ConnectionResult.Success) {
						texture = www.texture;
					}
					if(handler != null) {
						handler(texture);
					}
				});
			}
		});
	}
	
	public static void GetAvatarPicture(int userSeq, System.Action<Texture2D> handler) {
		_GetPictureInfo(userSeq, (dict)=>{
			if(dict.ContainsKey("userFacePhtflNm") && dict["userFacePhtflNm"] != null) {
				string url = dict["userFacePhtflNm"].ToString();
				
				WWWUtil.HTTPRequest(url, WWWUtil.HTTPMethod.GET, null, (connection, www)=>{
					Texture2D texture = null;
                    if (connection == WWWUtil.ConnectionResult.Success)
                    {
						texture = www.texture;
					}
					if(handler != null) {
						handler(texture);
					}
				});
			}
		});
	}

	private static void _GetPictureInfo(int userSeq, System.Action<Dictionary<string, object>> handler) {
		Dictionary<string, object> dict = new Dictionary<string, object>();

		// Invalid user sequence id
		if(userSeq < 1) {
			if(handler != null) {
				handler(dict);
			}
		}
		// request
		else {
			string url = string.Format("{0}/user/imageUrl.do?userSeq={1}", kRootUrl, userSeq);
			WWWUtil.HTTPRequest(url, WWWUtil.HTTPMethod.GET, null, (connection, www)=>{
                if (connection == WWWUtil.ConnectionResult.Success)
                {
					string jsonText = www.text;
					dict = MiniJSON.Json.Deserialize(jsonText) as Dictionary<string, object>;
				}
				
				if(handler != null) {
					handler(dict);
				}
			});
		}
	}
}
