using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class ColliderCheckerWithParam : MonoBehaviour
    {
        public EventDelegate callback;

        void OnTriggerEnter(Collider _other)
        { Execute(_other); }

        void OnTriggerEnter2D(Collider2D _other)
        { Execute(_other); }

        void OnCollisionEnter(Collision _other)
        { Execute(_other); }

        void OnCollisionEnter(Collision2D _other)
        { Execute(_other); }

        void Execute(object _other = null)
        {
            if (callback == null) return;
            callback.parameters[0] = new EventDelegate.Parameter(_other);
            callback.Execute();
        }
    }
}