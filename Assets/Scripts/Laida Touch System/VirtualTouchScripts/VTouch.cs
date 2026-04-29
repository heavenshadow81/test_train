using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using TouchInjection;
using static TouchInjection.TouchInjector;

namespace VirtualTouch
{
    public class VTouch
    {
        private static PointerTouchInfo createPointer(uint id)
        {
            var pointer = new PointerTouchInfo();
            //We can add different additional touch data 
            pointer.TouchMasks = TouchMask.PRESSURE;
            pointer.Pressure = 100;


            //Pointer ID is for gesture tracking 
            pointer.PointerInfo.PointerId = id;
            pointer.PointerInfo.pointerType = PointerInputType.TOUCH;

            return pointer;
        }


        public static bool Tap(float xf, float yf, int left, int top, int width, int height, uint idx = 1)
        {
            bool result;
            int x = left + (int)(xf * width);
            int y = top + (int)(yf * height);
            var c = new[] { createPointer(idx) };

            // Touch contact down
            c[0].PointerInfo.PtPixelLocation.X = x;
            c[0].PointerInfo.PtPixelLocation.Y = y;
            c[0].PointerInfo.PointerFlags = PointerFlags.INRANGE | PointerFlags.INCONTACT | PointerFlags.DOWN;
            result = InjectTouchInput(1, c);

            // Touch contact up and transition to hover
            //c[0].PointerInfo.PointerFlags = PointerFlags.INRANGE | PointerFlags.UP;
            //result = InjectTouchInput(1, c);

            return result;
        }

        #region original Tap Function
        public static void Tap(int x, int y)
        {
            var c = new[] { createPointer(1), createPointer(2) };

            // Touch contact down
            c[0].PointerInfo.PtPixelLocation.X = x;
            c[0].PointerInfo.PtPixelLocation.Y = y;
            c[0].PointerInfo.PointerFlags = PointerFlags.INRANGE | PointerFlags.INCONTACT | PointerFlags.DOWN;
            c[1].PointerInfo.PtPixelLocation.X = x + 110;
            c[1].PointerInfo.PtPixelLocation.Y = y;
            c[1].PointerInfo.PointerFlags = PointerFlags.INRANGE | PointerFlags.INCONTACT | PointerFlags.DOWN;
            InjectTouchInput(2, c);

            // Touch contact up and transition to hover
            c[0].PointerInfo.PointerFlags = PointerFlags.INRANGE | PointerFlags.UP;
            c[1].PointerInfo.PointerFlags = PointerFlags.INRANGE | PointerFlags.UP;
            InjectTouchInput(2, c);

        }
        #endregion
    }
}
