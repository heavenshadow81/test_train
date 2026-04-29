#define QUANTITIVE_EVALUATION_TEST

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The special input manager class for Dragon Park.
/// This class collects all kinds of touch or mouse inputs(especially, Windows touch events).
/// <br>
/// <remarks>Known issues : Currently, it is not implemented completely for mobile devices. So that, It should not work correctly.</remarks>
/// </summary>
public class CustomInput : MonoBehaviour
{
#if QUANTITIVE_EVALUATION_TEST
    public static  Dictionary<int, System.DateTime>startTimeDic = new Dictionary<int, System.DateTime>();
#endif

	#region Properties
	private static CustomInput __sharedInstance = null;
	/// <summary>
	/// Shared instance of this class.
	/// </summary>
	/// <value>The shared instance.</value>
	public static CustomInput sharedInstance {
		get {
			if(__sharedInstance == null) {
				__sharedInstance = GameObject.FindObjectOfType<CustomInput>();
				if(__sharedInstance == null) {
					GameObject go = new GameObject("CustomInput");
					DontDestroyOnLoad(go);
					__sharedInstance = go.AddComponent<CustomInput>();
				}
			}
			return __sharedInstance;
		}
	}

	/// <summary>
	/// Does running device support any touch input?
	/// </summary>
	/// <value><c>true</c> if supports touch; otherwise, <c>false</c>.</value>
	public static bool supportsTouch {
		get {
#if UNITY_STANDALONE_WIN
			if(!Win32TouchInput.initialized) {
				if(!Win32TouchInput.Initialize()) {
					sharedInstance._supportsWin32Touch = false;
				}
			}
			return sharedInstance._supportsWin32Touch;
#elif UNITY_IPHONE || UNITY_ANDROID
			return true;
#else
			return false;
#endif
            }
	}

	/// <summary>
	/// Gets the touch count.
	/// </summary>
	/// <value>The touch count.</value>
	public static int touchCount {
		get {
			return sharedInstance._touches.Count;
		}
	}

	private static TouchInfo[] _emptyTouches = new TouchInfo[0];	// for performance

	/// <summary>
	/// Gets the touches.
	/// </summary>
	public static TouchInfo[] touches {
		get {
			TouchInfo[] ret = sharedInstance._touches.ToArray();
			if(ret == null) {
				ret = _emptyTouches;
			}
			return ret;
		}
	}

    /// <summary>
    /// Receives mouse input?
    /// </summary>
#if UNITY_EDITOR
    private static bool _receivesMouseInput = true;
#else
    private static bool _receivesMouseInput = false;
#endif
    public static bool receivesMouseInput
    {
        get
        {
            return _receivesMouseInput;
        }
        set
        {
            _receivesMouseInput = value;
        }
    }

    /// <summary>
    /// The max user count.
    /// </summary>
    private static int _maxUserCount = 5;
    public static int maxUserCount
    {
        get
        {
            return _maxUserCount;
        }
        set
        {
            _maxUserCount = Mathf.Clamp(value, 1, 10);
        }
    }

    /// <summary>
    /// This flag enables the touch error correction on the current big board.
    /// </summary>
    private static bool _enablesErrorCorrection = true;
    public static bool enablesErrorCorrection {
        get {
            if (supportsTouch)
            {
                return _enablesErrorCorrection;
            }

            return false;
        }
        set {
            _enablesErrorCorrection = value;
        }
    }
	#endregion

	#region Private variables
	/// <summary>
	/// List of all touches.
	/// </summary>
    private List<TouchInfo> _touches = new List<TouchInfo>(32);

	/// <summary>
	/// All touches per user ID.
	/// </summary>
    private static Dictionary<int, List<TouchInfo>> _touchesPerUserIDDict = new Dictionary<int, List<TouchInfo>>();

    private List<TouchInfo> _errorCorrectedTouches = new List<TouchInfo>(32);

	/*
	 * For Mouse inputs
	 */
	private const float kMouseTouchTapCountWaitTime = 1.0f;
	private int _mouseTouchIndex = -1;
	private TouchInfo _cachedMouseTouchInfo;
	private float _lastMouseTouchTime = kMouseTouchTapCountWaitTime;
	private int _lastMouseTouchTapCount = 0;

#if UNITY_STANDALONE_WIN
	/// <summary>
	/// (Windows)Does running device support touch input?
	/// </summary>
	private bool _supportsWin32Touch = true;
	private HashSet<int> _win32TouchesSet = new HashSet<int>();
	private HashSet<int> _prevWin32TouchesSet = new HashSet<int>();
	private Dictionary<int, TouchInfo> _cachedWin32TouchInfos = new Dictionary<int, TouchInfo>();
#endif

	#endregion

	#region Unity methods
	void Update () {
		ProcessTouches();
	}
	#endregion

	#region Touch Process
	/// <summary>
	/// This method collects all touch or mouse inputs and adds to list <see cref="CustomInput._touches"/>.
	/// </summary>
	public virtual void ProcessTouches() {
		// post-process all touches
		_PostProcessTouches();
		
		// find all touches
        if (receivesMouseInput)
        {
            _ProcessMouseInput();
        }
#if UNITY_STANDALONE_WIN
		_ProcessWin32Touches();
#else
		_ProcessUnityTouches();
#endif

        // pre-process all touches
        _PreProcessTouches();
	}

	private void _PreProcessTouches() {
        if (enablesErrorCorrection)
        {
            _errorCorrectedTouches.Clear();
            for (int i = 0, cnt = touchCount; i < cnt; i++)
            {
              //  Debug.Log(" Win32touchInput : " + Win32TouchInput.GetInputElapsedTime());
                TouchInfo touch = GetTouch(i);
                int userId = touch.userId;

#if UNITY_EDITOR
                //Test start
                int id = touch.id;
                if (startTimeDic.ContainsKey(id)) //Test
                { startTimeDic[id] = System.DateTime.Now; }
                else
                { startTimeDic.Add(id, System.DateTime.Now); }
                //Test End
#endif

                if (_touchesPerUserIDDict.ContainsKey(userId))
                {
                    var list = _touchesPerUserIDDict[userId];
                    for (int j = 0, cnt2 = list.Count; j < cnt2; j++)
                    {
                        if (list[j].id == touch.id &&
                            (list[j].type == TouchInfo.Type.Touch && touch.type == TouchInfo.Type.Touch) &&
                            (list[j].phase == TouchInfo.Phase.Move || list[j].phase == TouchInfo.Phase.Stay) &&
                            (touch.phase == TouchInfo.Phase.Move || touch.phase == TouchInfo.Phase.Stay) &&
                            (list[j].position - touch.position).sqrMagnitude >= Mathf.Pow(Mathf.Min(Screen.width, Screen.height) / Mathf.Min(3, maxUserCount), 2) * 0.5625f) // magnitude == min(width, height) / max(3, maxUserCount) * 0.75f
                        {
                            TouchInfo error = list[j];
                            error.phase = TouchInfo.Phase.Cancel;
                            _errorCorrectedTouches.Add(error);
                            list[j] = error;

                            touch.phase = TouchInfo.Phase.Begin;
                            _touches[i] = touch;

                            break;
                        }
                    }
                }
            }

            _touches.InsertRange(0, _errorCorrectedTouches);
        }

        // align all touches per users
        for (int i = 0; i < maxUserCount; i++)
        {
            if (_touchesPerUserIDDict.ContainsKey(i))
            {
                _touchesPerUserIDDict[i].Clear();
            }
        }

		for(int i = 0, cnt = touchCount; i < cnt; i++) {
			TouchInfo touch = GetTouch(i);
			int userId = touch.userId;
			if(!_touchesPerUserIDDict.ContainsKey(userId)) {
				List<TouchInfo> list = new List<TouchInfo>(5);
				_touchesPerUserIDDict[userId] = list;
			}
			_touchesPerUserIDDict[userId].Add(touch);
		}
	}

	private void _PostProcessTouches() {
		// Remove all end/cancel-phased touches 
        /*
		for(int i = 0, cnt = touchCount; i < cnt; i++) {
			TouchInfo touch = GetTouch(i);
			if(touch.type != TouchInfo.Type.Custom) {
				if(touch.phase == TouchInfo.Phase.End ||
				   touch.phase == TouchInfo.Phase.Cancel) {
					_touches.RemoveAt(i--);
					cnt--;
				}
			}
		}
        */

		// calculate elapsed time since last mouse event occured.
		if(_mouseTouchIndex == -1) {
			if(_lastMouseTouchTime >= kMouseTouchTapCountWaitTime) {
				_lastMouseTouchTime = kMouseTouchTapCountWaitTime;
				_lastMouseTouchTapCount = 0;
			}
			_lastMouseTouchTime += Time.deltaTime;
		}

		// clear touch list
		_touches.Clear();
	}

	/// <summary>
	/// This method tracks mouse input.
	/// Currently, it tracks left button only (other buttons is not important).
	/// </summary>
	private void _ProcessMouseInput() {
		bool mouse = Input.GetMouseButton(0);

		if(mouse || _mouseTouchIndex > -1) {
			TouchInfo touch = new TouchInfo();
			touch.id = 0;	// mouse doesn't have own id
			touch.type = TouchInfo.Type.Mouse;
			touch.position = Input.mousePosition;
			touch.userId = _cachedMouseTouchInfo.userId;
			touch.tapCount = _lastMouseTouchTapCount + 1;

			if(mouse) {
				if(Input.GetMouseButtonDown(0)) {
					touch.userId = FindUserId(touch.axisX);
					touch.phase = TouchInfo.Phase.Begin;
				}
				else {
					touch.phase = TouchInfo.Phase.Move;
				}

				_mouseTouchIndex = _touches.Count;
			}
			else {
				if(Input.GetMouseButtonUp(0)) {
					touch.phase = TouchInfo.Phase.End;
				}
				else {
					touch.phase = TouchInfo.Phase.Cancel;
				}

				_mouseTouchIndex = -1;
				_lastMouseTouchTime = 0.0f;
			}

			_cachedMouseTouchInfo = touch;
			_touches.Add(touch);
		}
	}

	/// <summary>
	/// This method tracks all touch inputs on mobile device.
	/// On desktop, this process will be discarded.
	/// </summary>
	private void _ProcessUnityTouches() {
		if(Input.touchCount > 0) {			
			// caches touch to dictionary
			for(int i = 0, cnt = Input.touchCount; i < cnt; i++) {
				// Unity touch
                Touch unityTouch = Input.GetTouch(i);

                // touch id : finger id + 1 ( 0 : mouse id )
                int touchId = unityTouch.fingerId + 1;

                // Make touch info
				TouchInfo touch = new TouchInfo();
				touch.type = TouchInfo.Type.Touch;
				touch.position = unityTouch.position;
                touch.id = touchId;
				touch.userId = FindUserId(touch.axisX);
				touch.tapCount = unityTouch.tapCount;

                // Touch phase
                switch (unityTouch.phase)
                {
                    case TouchPhase.Began:
                        touch.phase = TouchInfo.Phase.Begin;
                        break;
                    case TouchPhase.Moved:
                        touch.phase = TouchInfo.Phase.Move;
                        break;
                    case TouchPhase.Stationary:
                        touch.phase = TouchInfo.Phase.Stay;
                        break;
                    case TouchPhase.Canceled:
                        touch.phase = TouchInfo.Phase.Cancel;
                        break;
                    case TouchPhase.Ended:
                        touch.phase = TouchInfo.Phase.End;
                        break;
                }

                // Add to touches
				_touches.Add(touch);
			}
		}
	}

	/// <summary>
	/// This method tracks all touch inputs on Windows.
	/// It uses special class named Win32TouchInput.
	/// <seealso cref="Win32TouchInput"/>
	/// </summary>
	private void _ProcessWin32Touches() {
#if UNITY_STANDALONE_WIN
		if(_supportsWin32Touch) {
			if(!Win32TouchInput.initialized) {
				if(!Win32TouchInput.Initialize()) {
					_supportsWin32Touch = false;
				}
			}
			else {
				_prevWin32TouchesSet.Clear();
                for (var win32TouchSet_Enumerator = _win32TouchesSet.GetEnumerator(); win32TouchSet_Enumerator.MoveNext();)
                    _prevWin32TouchesSet.Add(win32TouchSet_Enumerator.Current);
				_win32TouchesSet.Clear();

				Win32TouchData[] win32Touches = Win32TouchInput.touches; //OS로 부터 획득된 터치 이벤트
				for(int i = 0, cnt = Win32TouchInput.touchCount; i < cnt; i++) {
					Win32TouchData touch = win32Touches[i];
					int id = touch.id;

        #if QUANTITIVE_EVALUATION_TEST
                    //Test start
                    if (startTimeDic.ContainsKey(id)) //터치 ID별로 시작 시간 저장
                    { startTimeDic[id] = System.DateTime.Now; } //이 컴퓨터의 현재 날짜와 시간으로 설정되고 현지 시간으로 표시되는 DateTime 개체를 가져옵니다.
                    else
                    { startTimeDic.Add(id, System.DateTime.Now); } //.NET Framework 4.6 and 4.5 static property
                    //Test End
        #endif
					bool active = _prevWin32TouchesSet.Contains(id);

					TouchInfo touchInfo = new TouchInfo();
					touchInfo.position = new Vector2(touch.x, Screen.height - touch.y);
					touchInfo.type = TouchInfo.Type.Touch;
					touchInfo.tapCount = 1;
					touchInfo.id = id;

					if(!active) {
						touchInfo.phase = TouchInfo.Phase.Begin;
						touchInfo.userId = FindUserId(touchInfo.axisX);
					}
					else {
						touchInfo.phase = TouchInfo.Phase.Move;
						if(_cachedWin32TouchInfos.ContainsKey(id)) {
							touchInfo.userId = _cachedWin32TouchInfos[id].userId;
						}
						else {
							touchInfo.userId = FindUserId(touchInfo.axisX);
						}
					}
					
					_win32TouchesSet.Add(id);
					_cachedWin32TouchInfos[id] = touchInfo;
					_touches.Add(touchInfo);
				}

				HashSet<int>.Enumerator e = _prevWin32TouchesSet.GetEnumerator();
				while(e.MoveNext()) {
					int id = e.Current;
					if(!_win32TouchesSet.Contains(id)) {
						if(_cachedWin32TouchInfos.ContainsKey(id)) {
							TouchInfo touchInfo = _cachedWin32TouchInfos[id];
							touchInfo.phase = TouchInfo.Phase.End;
							_touches.Add(touchInfo);
						}
					}
				}
			}
		}
#endif
	}
	#endregion

	#region Get Touch(es)
	/// <summary>
	/// Gets the touch information at index
	/// </summary>
	/// <returns>The touch information.</returns>
	/// <param name="idx">The index of the touch list.</param>
	public static TouchInfo GetTouch(int idx) {
		return sharedInstance._touches[idx];
	}

	/// <summary>
	/// Gets the touch count of user.
	/// </summary>
	/// <returns>The touch count of user.</returns>
	/// <param name="id">User ID.</param>
	public static int GetTouchCountOfUser(int id) {
		int ret = 0;
		for(int i = 0, cnt = touchCount; i < cnt; i++) {
			TouchInfo touch = GetTouch(i);
			if(touch.userId == id) {
				ret++;
			}
		}
		return ret;
	}

	/// <summary>
	/// Gets the all touches of the specific user.
	/// </summary>
	/// <returns>The touches of user.</returns>
	/// <param name="id">User ID.</param>
	public static TouchInfo[] GetTouchesOfUser(int id) {
		TouchInfo[] touches = null;
		if(_touchesPerUserIDDict.ContainsKey(id)) {
			touches = _touchesPerUserIDDict[id].ToArray();
		}
		return touches;
	}
	#endregion

	#region Utility Methods
	/// <summary>
	/// Gets the user id of specific screen's X position.
	/// </summary>
	/// <returns>User ID.</returns>
	/// <param name="axisX">X position of the screen</param>
	public static int FindUserId(int axisX)
	{
		int width = Screen.width;

		int rt = Mathf.Clamp(axisX / (width / maxUserCount), 0, maxUserCount);

		return rt;
	}
	#endregion
}
