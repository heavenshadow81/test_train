using System;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ML.TouchModule.vCatchStation
{
    using Win32;

    /// <summary>
    /// VCatchStation 터치 모듈 연동 클래스.
    /// </summary>
    public class vCatchModule
    {
        #region Properties
        public bool Initialized
        {
            get { return middlewareHWnd != IntPtr.Zero && hWnd != IntPtr.Zero && SensorStatus == StatusCode.No_Error; }
        }

        public StatusCode SensorStatus { get; private set; }

        public ModuleID ModuleID { get; private set; }

        private static vCatchModule Instance { get; set; }
        #endregion

        #region Constants
        // VCatch Commands
        public const int vCatchStation_Command_Stop = 0;
        public const int vCatchStation_Command_SetTimeout = 0x00001000; // time(100ms unit) = 0~0xfff, ex) 5sec timeout = 0x1000 | (5*10) = 0x1032
        public static int vCatchStation_CMD_SetTimeout(int ms) { return vCatchStation_Command_SetTimeout | (ms / 100); }
        public const int vCatchStation_Command_Points_Reporter = 0x00002000; // time(10ms unit) = 0~0xff, ex) 100msec interval = 0x2000 | (100/10) = 0x200a
        public const int vCatchStation_Points_Report_Down = 0x00000100; // 
        public const int vCatchStation_Points_Report_Up = 0x00000200; // 
        public const int vCatchStation_Points_Report_Move = 0x00000400; // 
        public const int vCatchStation_Points_Report_MultiTouch = 0x00000800; // 
        public int vCatchStation_CMD_Points_Reporter(int flags, int interval) { return vCatchStation_Command_Points_Reporter | flags | (interval / 10); }

        // Constants
        public const string vCatchStation_WindowMessageName = "vCatchStation_BroadcastMessage";

        public const int WM_vCatchStation_Initialize_vCatch = (Win32.WM_USER + 0x3601);
        public const int WM_vCatchStation_Do_vCatch = (Win32.WM_USER + 0x3602);
        public const int WM_vCatchStation_Command_vCatch = (Win32.WM_USER + 0x3603);
        public const int WM_vCatchStation_Deinitialize_vCatch = (Win32.WM_USER + 0x3604);

        public const int WM_COPYDATA_DataType_ModuleStatus = 2000;
        public const int WM_COPYDATA_DataType_SensorStatus = 3000;
        public const int WM_COPYDATA_DataType_Stopped = 0;
        public const int WM_COPYDATA_DataType_Points = 1;
        public const int WM_COPYDATA_DataType_1D = 10;
        public const int WM_COPYDATA_DataType_2D = 20;
        public const int WM_COPYDATA_DataType_2DS = 21;
        #endregion

        #region Private variables
        private static IntPtr hWnd = IntPtr.Zero;
        private static IntPtr middlewareHWnd = IntPtr.Zero;
        private static IntPtr hInstance = IntPtr.Zero;
        private static int mainThreadId = 0;
        private static int processId = 0;
        private static IntPtr getMsgHook = IntPtr.Zero;
        private static IntPtr callWndProcHook = IntPtr.Zero;

        [SerializeField]
        private static uint MsgId_VCatchStation = 0;
        #endregion

        #region Events
        public delegate void OnMiddlewareReceived();
        public delegate void OnSensorStatus(vCatchResult_SensorStatus result);
        public delegate void OnStopped(vCatchResult_Stopped result);
        public delegate void OnResultPoints(vCatchResult_Points result);
        public delegate void OnResult1D(vCatchResult_1D result);
        public delegate void OnResult2D(vCatchResult_2D result);
        public delegate void OnResult2DS(vCatchResult_2DS result);
        public event OnMiddlewareReceived MiddlewareReceived;
        public event OnSensorStatus SensorStatusChanged;
        public event OnStopped Stopped;
        public event OnResultPoints ResultPoints;
        public event OnResult1D Result1D;
        public event OnResult2D Result2D;
        public event OnResult2DS Result2DS;
        #endregion

        #region Initialization
        public void Init(ModuleID NewModuleID)
        {
#if !UNITY_STANDALONE_WIN
            Debug.LogWarning("vCatchStation supports Win32 platform only.");
            return;
#endif
            // Select module
            if (Instance != null)
            {
                Debug.LogError("Another instance is already initialized.");
                return;
            }
            Instance = this;
            ModuleID = NewModuleID;

            // Process ID
            processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            Debug.Log("Process id : " + processId);

            // Main Thread ID
            mainThreadId = Win32.GetCurrentThreadId();
            Debug.Log("Main Thread id : " + mainThreadId);

            // Query hWnd
            hWnd = new IntPtr(Win32.GetActiveWindow().ToInt64());
            if (hWnd == IntPtr.Zero)
            {
                // Enumerate all hWnds
                Debug.LogError("Failed to get active window. Enumerating...");
                Win32.EnumWindows(WndEnumProc, IntPtr.Zero);
                if (hWnd == IntPtr.Zero)
                    Debug.LogError("Failed to get hWnd");
            }
            else
            {
                hInstance = new IntPtr(Win32.GetWindowLongPtr(hWnd, Win32.GWLP_HINSTANCE).ToInt64());
                Debug.Log("hInstance = " + hInstance);
                Debug.Log("hWnd = " + hWnd);
            }

            // hook
            getMsgHook = Win32.SetWindowsHookEx(Win32.WH_GETMESSAGE, GetMessageProc, IntPtr.Zero, mainThreadId);
            callWndProcHook = Win32.SetWindowsHookEx(Win32.WH_CALLWNDPROC, CallWndProc, IntPtr.Zero, mainThreadId);

            // Notify that app is started.
            if (MsgId_VCatchStation == 0)
            {
                MsgId_VCatchStation = Win32.RegisterWindowMessage(vCatchStation_WindowMessageName);
                Debug.Log("Registered window message : " + MsgId_VCatchStation);
                if (MsgId_VCatchStation != 0)
                {
                    uint dwRecipients = Win32.BSM_APPLICATIONS;
                    long result = Win32.BroadcastSystemMessage(Win32.BSF_POSTMESSAGE, ref dwRecipients, MsgId_VCatchStation, IntPtr.Zero, IntPtr.Zero);
                    Debug.Log("Broadcast system message : " + MsgId_VCatchStation + ", result : " + result);
                }
            }
        }

        private static bool WndEnumProc(IntPtr enumHWnd, IntPtr lParam)
        {
            StringBuilder title = new StringBuilder(256);
            Win32.GetWindowText(enumHWnd, title, 255);
            uint enumProcessId = 0;
            Win32.GetWindowThreadProcessId(enumHWnd, ref enumProcessId);

            if (Application.productName.Equals(title.ToString()) &&
                enumProcessId == processId)
            {
                hWnd = new IntPtr(enumHWnd.ToInt64());
                IntPtr enumHInst = Win32.GetWindowLongPtr(enumHWnd, Win32.GWLP_HINSTANCE);
                hInstance = new IntPtr(enumHInst.ToInt64());
                Debug.Log("hInstance = " + hInstance);
                Debug.Log("hWnd = " + hWnd);
                return false;
            }
            return true;
        }

        public void Cleanup()
        {
            // Unhook messages
            if (getMsgHook != IntPtr.Zero)
            {
                Win32.UnhookWindowsHookEx(getMsgHook);
                getMsgHook = IntPtr.Zero;
            }
            if (callWndProcHook != IntPtr.Zero)
            {
                Win32.UnhookWindowsHookEx(callWndProcHook);
                callWndProcHook = IntPtr.Zero;
            }

            // Deinitialize vCatch
            if (middlewareHWnd != IntPtr.Zero)
            {
                Win32.SendMessage(middlewareHWnd, WM_vCatchStation_Deinitialize_vCatch, IntPtr.Zero, hWnd);
                middlewareHWnd = IntPtr.Zero;
            }

            // Cleanup variables
            hWnd = IntPtr.Zero;
            hInstance = IntPtr.Zero;
            processId = mainThreadId = 0;
            SensorStatus = StatusCode.No_Error;
        }

        ~vCatchModule()
        {
            Cleanup();
        }
        #endregion

        #region Commands
        public void StartGame()
        {
            if (middlewareHWnd != IntPtr.Zero)
            {
                IntPtr result = Win32.SendMessage(middlewareHWnd, WM_vCatchStation_Do_vCatch, IntPtr.Zero, hWnd);
                if (result.ToInt32() == 1)
                {
                    Debug.Log("Sent WM_vCatchStation_Do_vCatch.");
                }
                else
                {
                    Debug.LogWarning("Failed to send WM_vCatchStation_Do_vCatch.");
                }
            }
        }

        public void StopGame()
        {
            if (middlewareHWnd != IntPtr.Zero)
            {
                IntPtr result = Win32.SendMessage(middlewareHWnd, WM_vCatchStation_Command_vCatch, new IntPtr(vCatchStation_Command_Stop), hWnd);
                if (result.ToInt32() == 1)
                {
                    Debug.Log("Sent WM_vCatchStation_Command_vCatch with vCatchStation_Command_Stop.");
                }
                else
                {
                    Debug.LogWarning("Failed to send WM_vCatchStation_Command_vCatch.");
                }
            }
        }

        public void Command(int command)
        {
            if (middlewareHWnd != IntPtr.Zero)
            {
                IntPtr result = Win32.SendMessage(middlewareHWnd, WM_vCatchStation_Command_vCatch, new IntPtr(command), hWnd);
                if (result.ToInt32() == 1)
                {
                    Debug.Log("Sent WM_vCatchStation_Command_vCatch with command " + command + ".");
                }
                else
                {
                    Debug.LogWarning("Failed to send WM_vCatchStation_Command_vCatch.");
                }
            }
        }
        #endregion

        #region Windows Messaging
        public static IntPtr GetMessageProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (Instance != null && code >= 0)
            {
                try
                {
                    MSG msg = (MSG)Marshal.PtrToStructure(lParam, typeof(MSG));
                    if (msg.message == MsgId_VCatchStation && msg.lParam != IntPtr.Zero)
                    {
                        if (middlewareHWnd == IntPtr.Zero)
                        {
                            // Initialize vCatch
                            middlewareHWnd = new IntPtr(msg.lParam.ToInt64());
                            Win32.SendMessage(middlewareHWnd, WM_vCatchStation_Initialize_vCatch, new IntPtr((int)Instance.ModuleID), hWnd);
                            Debug.Log(middlewareHWnd + " - WM_vCatchStation_Initialize_vCatch");
                            if (Instance.MiddlewareReceived != null)
                                Instance.MiddlewareReceived();
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            return Win32.CallNextHookEx(getMsgHook, code, wParam, lParam);
        }

        public static IntPtr CallWndProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (Instance != null && code >= 0)
            {
                try
                {
                    CWPSTRUCT msg = (CWPSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPSTRUCT));
                    if (msg.message == Win32.WM_COPYDATA)
                    {
                        COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(msg.lParam, typeof(COPYDATASTRUCT));
                        vCatchResult result = (vCatchResult)Marshal.PtrToStructure(cds.lpData, typeof(vCatchResult));
                        switch (cds.dwData.ToInt32())
                        {
                            case WM_COPYDATA_DataType_ModuleStatus:
                                break;
                            case WM_COPYDATA_DataType_SensorStatus:
                                Debug.Log(string.Format("SensorStatus - code({0})", result.sensor_status.code));
                                Instance.SensorStatus = result.sensor_status.code;
                                if (Instance.SensorStatusChanged != null)
                                    Instance.SensorStatusChanged(result.sensor_status);
                                break;
                            case WM_COPYDATA_DataType_Stopped:
                                Debug.Log(string.Format("Stopped - code({0})", result.stopped.code));
                                if (Instance.Stopped != null)
                                    Instance.Stopped(result.stopped);
                                break;
                            case WM_COPYDATA_DataType_Points:
                                Debug.Log(string.Format("DataType_Points - ms:{0} idPt:{1} status:{2} x:{3:0.00} y:{4:0.00}",
                                    result.resultPoints.msec, result.resultPoints.idPoint, result.resultPoints.status,
                                    result.resultPoints.posX, result.resultPoints.posY));
                                if (Instance.ResultPoints != null)
                                    Instance.ResultPoints(result.resultPoints);
                                break;
                            case WM_COPYDATA_DataType_1D:
                                Debug.Log(string.Format("DataType_1D - ms:{0} sensor:{1} pos:{2:0.00} width:{3:0.00}",
                                    result.result1D.msec, result.result1D.idSensor, result.result1D.pos, result.result1D.width));
                                if (Instance.Result1D != null)
                                    Instance.Result1D(result.result1D);
                                break;
                            case WM_COPYDATA_DataType_2D:
                                Debug.Log(string.Format("DataType_2D - ms:{0} pos:{1:0.00},{2:0.00}",
                                    result.result2D.msec, result.result2D.posX, result.result2D.posY));
                                if (Instance.Result2D != null)
                                    Instance.Result2D(result.result2D);
                                break;
                            case WM_COPYDATA_DataType_2DS:
                                Debug.Log(string.Format("DataType 2DS - ms:{0} pos:{1:0.00},{2:0.00} speed: {3:0.00}",
                                    result.result2DS.msec, result.result2DS.posX, result.result2DS.posY, result.result2DS.speed));
                                if (Instance.Result2DS != null)
                                    Instance.Result2DS(result.result2DS);
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            return Win32.CallNextHookEx(callWndProcHook, code, wParam, lParam);
        }
        #endregion
    }
}