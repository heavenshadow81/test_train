using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VirtualTouch
{
    public class VMouse
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        private static TimeSpan _interval;// = TimeSpan.FromMilliseconds(100);
        private readonly ManualResetEvent _stoppingEvent = new ManualResetEvent(false);

        public VMouse()
        {
            _interval = TimeSpan.FromMilliseconds(100);
            _stoppingEvent.Reset();
        }

        public void Tap(float xf, float yf, int left, int top, int width, int height)
        {
            int x = left + (int)(xf * width);
            int y = top + (int)(yf * height);

            int stride = 65535;

            int dx, dy;
            dx = stride * x / width;
            dy = stride * y / height;

            try
            {
                //SetCursorPos(x, y);
                //_stoppingEvent.WaitOne(_interval);

                mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN, dx, dy, 0, 0);
                //_stoppingEvent.WaitOne(_interval);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                //_stoppingEvent.WaitOne(_interval);

                //MouseEvent(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                //_stoppingEvent.WaitOne(_interval);
                //MouseEvent(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                //_stoppingEvent.WaitOne(_interval);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        public void TapDown(float xf, float yf, int left, int top, int width, int height)
        {
            int x = left + (int)(xf * width);
            int y = top + (int)(yf * height);

            int stride = 65535;

            int dx, dy;
            dx = stride * x / width;
            dy = stride * y / height;

            try
            {
                //SetCursorPos(x, y);
                //_stoppingEvent.WaitOne(_interval);

                mouse_event(MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN, dx, dy, 0, 0);
                //_stoppingEvent.WaitOne(_interval);
                //mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                //_stoppingEvent.WaitOne(_interval);

                //MouseEvent(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                //_stoppingEvent.WaitOne(_interval);
                //MouseEvent(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                //_stoppingEvent.WaitOne(_interval);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }

        public void TapOut()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
    }
}
