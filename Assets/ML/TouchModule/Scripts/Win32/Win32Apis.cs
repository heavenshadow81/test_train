using System;
using System.Text;
using System.Runtime.InteropServices;

namespace ML.TouchModule.Win32
{
    public delegate IntPtr HOOKPROC(int code, IntPtr wParam, IntPtr lParam);
    public delegate bool WNDENUMPROC(IntPtr hWnd, IntPtr lParam);

    public class Win32
    {
        #region Win32 Functions
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern long BroadcastSystemMessage(uint flags, ref uint lpInfo, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string className, string windowName);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(WNDENUMPROC lpEnumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HOOKPROC proc, IntPtr hmod, int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();
        #endregion

        #region Win32 Constants
        // Window messages
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_USER = 0x0400;
        public const int WM_COPYDATA = 0x004A;

        // GetWindowLongPtr flags
        public const int GWLP_HINSTANCE = -6;

        // Broadcast special message flags
        public const uint BSF_POSTMESSAGE = 0x00000010;
        public const uint BSM_APPLICATIONS = 0x00000008;

        // hook value
        public const int WH_KEYBOARD = 2;
        public const int WH_GETMESSAGE = 3;
        public const int WH_CALLWNDPROC = 4;
        public const int WH_MOUSE = 7;
        public const int WH_KEYBOARD_LL = 13;
        #endregion
    }

    public struct POINT
    {
        public long x, y;
    }

    public struct MSG
    {
        public IntPtr hWnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public POINT pt;
        public uint lPrivate;
    }

    public struct CWPSTRUCT
    {
        public IntPtr lParam;
        public IntPtr wParam;
        public uint message;
        public IntPtr hwnd;
    }

    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public uint cbData;
        public IntPtr lpData;
    }
}