using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class Win32TouchData
{
	public int x;
	public int y;
	public int id;
	public int timeMills;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RECT {
    public int left;
    public int top;
    public int right;
    public int bottom;
}

[StructLayout(LayoutKind.Sequential)]
public struct POINT {
    public int x;
    public int y;

    public POINT(int newX, int newY) { x = newX; y = newY; }
}

/// <summary>
/// Windows Touch Input Support Class.
/// Requires Windows 7 or later.
/// </summary>
public class Win32TouchInput {
	private static bool _initialized = false;
	public static bool initialized {
		get {
			return _initialized;
		}
	}

#if UNITY_STANDALONE_WIN
	[DllImport("TouchOverlay", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
	private static extern int Initialise(string Str);
	
	[DllImport("TouchOverlay")]
	private static extern int GetTouchPointCount();

	[DllImport ("TouchOverlay")]
	private static extern void GetTouchPoint(int i, Win32TouchData n);

	[DllImport ("user32.dll")]
	private static extern IntPtr GetActiveWindow();
	
	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

    [DllImport("user32.dll")]
    private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

    private static IntPtr hWnd;
#endif

	public static int touchCount {
		get {
			if(!_initialized) {
				return 0;
			}

#if UNITY_STANDALONE_WIN
			return GetTouchPointCount();
#else
			return 0;
#endif
		}
	}

	private static List<Win32TouchData> _cachedTouchDataList = new List<Win32TouchData>(32);
	private static BetterList<Win32TouchData> _cachedTouches = new BetterList<Win32TouchData>();

	private static Win32TouchData[] _emptyTouches = new Win32TouchData[0];
	public static Win32TouchData[] touches {
		get {
#if UNITY_STANDALONE_WIN
			// remove touch list and 
			// caches memory to buffer
			for(int i = 0; i < _cachedTouches.size; i++) {
				Win32TouchData touch = _cachedTouches[i];
				_cachedTouchDataList.Add(touch);
			}
			_cachedTouches.Clear();

			// generates new touch list
			for(int i = 0, cnt = touchCount; i < cnt; i++) {
				Win32TouchData touch = null;

				// if buffer is zero-length, creates new instance
				if(_cachedTouchDataList.Count == 0) {
					touch = new Win32TouchData();
					_cachedTouchDataList.Add(touch);
				}

				// pops instance from buffer
				touch = _cachedTouchDataList[0];
				_cachedTouchDataList.RemoveAt(0);

				// get touch point
				GetTouchPoint(i, touch);

                // divide by 100 
                touch.x = touch.x / 100;
                touch.y = touch.y / 100;

                // adjust
                //Debug.Log("from -> " + touch.x + "," + touch.y);
                POINT point = new POINT(touch.x, touch.y);
                if (ScreenToClient(hWnd, ref point))
                {
                    //Debug.Log("converting...");
                    touch.x = point.x; touch.y = point.y;
                } 
                //Debug.Log("to -> " + touch.x + "," + touch.y);

				// append to the list
				_cachedTouches.Add(touch);
			}

			// for zero-length
			if(_cachedTouches.size == 0) {
				return _emptyTouches;
			}

			return _cachedTouches.ToArray();
#else
            return _emptyTouches;
#endif
		}
	}

	public static bool Initialize() {
#if UNITY_STANDALONE_WIN
		hWnd = GetActiveWindow();
		string str = "Sample_Stage";

		StringBuilder sb = new StringBuilder(255);
		GetWindowText(hWnd, sb, 255);
		str = sb.ToString();

		int returnCode = Initialise(str);

		Debug.Log ("Win32TouchInput.Initialize() : Return Code : " + returnCode);

		if(returnCode > -1) {
			Debug.Log ("Win32TouchInput.Initialize() : Initialization Success");
			_initialized = true;
		}
		else {
			Debug.Log ("Win32TouchInput.Initialize() : Initialization Failed");
		}

		
		return (returnCode > -1);
#else
		Debug.Log("This platform does not support.");

        return false;
#endif
	}




}
