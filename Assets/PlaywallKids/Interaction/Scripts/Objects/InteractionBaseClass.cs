using UnityEngine;

namespace ML.PlaywallKids.Interaction
{
    namespace InteractionContentsEnum
    {
        public enum EState
        {
            NONE = -1, DEFAULT,
            NOTICE, CHECK_USER,
            EVENT0, EVENT1, EVENT2,
            APPEAR, APPEAR_EVENT0, APPEAR_EVENT1, APPEAR_EVENT2,
            READY, READY_EVENT0, READY_EVENT1, READY_EVENT2,
            PLAYING, PLAY_STATE0, PLAY_STATE1, PLAY_STATE2,
            BEGIN, BEGIN_EVENT0, BEGIN_EVENT1, BEGIN_EVENT2,
            DISAPPEAR, DISAPPEAR_EVENT0, DISAPPEAR_EVENT1, DISAPPEAR_EVENT2,
            CLOSE, CLOSE_EVENT0, CLOSE_EVENT1, CLOSE_EVENT2
        }
    }

    public abstract class InteractionBaseClass : MonoBehaviour
    {
        public abstract int numberOfPlayers { set; get; }
        public abstract void SetStringValue(string _jsonStr);
    }
}