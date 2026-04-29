using UnityEngine;
using System.Collections;

public class DebugUtil {
	public static void Log(object logObj) {
		string log = logObj.ToString();
		do {
			string line = log.Substring(0, System.Math.Min(log.Length, 768));
			
			#if UNITY_EDITOR || UNITY_STANDALONE
			Debug.Log (line);
			#else
			System.Console.WriteLine(line);
			#endif
			
			log = log.Remove(0, System.Math.Min(log.Length, 768));
		}
		while(log.Length > 0);
	}

	public static void LogFormat(string format, params object[] list) {
		string log = string.Format(format, list);
		Log (log);
	}

	public static void DLog(object logObj) {
		if(Debug.isDebugBuild) {
			Log (logObj);
		}
	}

	public static void DLogFormat(string format, params object[] list) {
		string log = string.Format(format, list);
		DLog(log);
	}
}