using System;

namespace ML.PlaywallKids.DragonPark
{
    [Obsolete("This class is no longer used.")]
    public class UserMotionInfo
    {
        public byte id = 0x31;

        public enum Type
        {
            None = 0x40,
            WaveHand = 0x41,
            RaiseLeftHand = 0x42,
            RaiseRightHand = 0x43,
            RaiseHands = 0x44,
            WaveHands = 0x45,
            HideEyes = 0x46
        }

        public Type type = Type.None;

        public bool hide = false;

        public UserMotionInfo(byte id)
        {
            this.id = id;
        }
    }
}