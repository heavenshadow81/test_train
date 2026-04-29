using System;
using System.Runtime.InteropServices;

namespace ML.PlaywallKids.DragonPark
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Obsolete("This class is no longer used.")]
    public class MotionPacketBase
    {
        public byte stx = 0x02;
        public byte pid = 0x11;
        public byte command;
        public byte user;

        public virtual byte[] ToByteArray()
        {
            byte[] arr = new byte[4];
            arr[0] = stx;
            arr[1] = pid;
            arr[2] = command;
            arr[3] = user;
            return arr;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Obsolete("This class is no longer used.")]
    public class MotionPacketClient : MotionPacketBase
    {
        public byte data;
        public byte etx = 0x03;

        public override byte[] ToByteArray()
        {
            byte[] b = base.ToByteArray();
            byte[] arr = new byte[6];
            System.Array.Copy(b, arr, 4);
            arr[4] = data;
            arr[5] = etx;
            return arr;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Obsolete("This class is no longer used.")]
    public class MotionPacketServer : MotionPacketBase
    {
        public byte data;
        public byte flag;
        public byte etx = 0x03;

        public MotionPacketServer()
        {
            pid = 0x12;
        }

        public override byte[] ToByteArray()
        {
            byte[] b = base.ToByteArray();
            byte[] arr = new byte[7];
            System.Array.Copy(b, arr, 4);
            arr[4] = data;
            arr[5] = flag;
            arr[6] = etx;
            return arr;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Obsolete("This class is no longer used.")]
    public class MotionPacketFind : MotionPacketBase
    {
        public int pointX;
        public int pointY;
        public byte etx;
    }
}