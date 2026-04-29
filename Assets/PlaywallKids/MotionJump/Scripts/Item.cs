using UnityEngine;

namespace ML.PlaywallKids.MotionJump
{
    public enum EItemType
    {
        NONE, SPEED, COIN, FREEZE,
    }

    public class Item : MonoBehaviour
    {
        public EItemType type;
        public float value;
        public float strength;
        public float activeTime;
    }
}

