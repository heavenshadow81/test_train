using UnityEngine;

namespace ML.PlaywallKids.Interaction
{
    public class InteractionPaintBalllTarget : MonoBehaviour
    {
        public InteractionPaintBallManager paintBallController;
        [Range(0.1f, 3f)]
        public float ratio;
        new Collider collider;

        void Awake()
        {
            collider = GetComponent<Collider>();
            if (ratio == 0) ratio = 1f;
        }

        void OnTriggerEnter(Collider _other)
        {
            Vector2 closetPostion = collider.ClosestPointOnBounds(_other.transform.position);
            float _radius = collider.bounds.extents.x;
            float _distace = (closetPostion - (Vector2)this.transform.localPosition).magnitude;
            float _ratio = _distace / _radius;

            int num = 0;
            if (0.2f >= _ratio && _ratio >= 0f)
            {
                num = (int)(10 * ratio);
            }
            else if (0.2f < _ratio && _ratio <= 0.4f)
            {
                num = (int)(8 * ratio);
            }
            else if (0.4f < _ratio && _ratio <= 0.6f)
            {
                num = (int)(4 * ratio);
            }
            else if (0.6f < _ratio && _ratio <= 0.8f)
            {
                num = (int)(2 * ratio);
            }
            else
            {
                num = (int)(1 * ratio);
            }

            paintBallController.ColliderWithWalls(_other, num);
        }
    }
}