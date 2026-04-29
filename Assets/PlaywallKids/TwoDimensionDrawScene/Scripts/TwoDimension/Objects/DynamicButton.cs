using UnityEngine;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 튜브 버튼 애미메이터 및 이동  제어 클래스
    /// </summary>
    public class DynamicButton : MonoBehaviour
    {
        public EventDelegate callbackFunc;
        Transform cachedTransform;
        bool bToggle;
        EState _state;
        public EState currentState
        {
            set
            {
                if (_state != value)
                {
                    switch (value)
                    {
                        case EState.IDLE: break;
                        case EState.EVENT:
                            animator.SetTrigger("Click");
                            break;
                    }
                    _state = value;
                }
            }
            get => _state; 
        }

        Animator animator;
        float width;

        void Awake()
        {
            width = 0f;

            cachedTransform = this.transform;
            animator = GetComponent<Animator>();
        }

        void OnEnable()
        {
            bToggle = false;
            currentState = EState.IDLE;
        }

        void FixedUpdate()
        {
            switch (currentState)
            {
                case EState.IDLE:
                    float distance = 20 * (bToggle ? -1 * Time.fixedDeltaTime : Time.fixedDeltaTime);
                    width += distance;
                    if (width < -50f || width > 50f) bToggle = !bToggle;
                    cachedTransform.localPosition += new Vector3(distance, 0f, 0f);
                    break;
            }
        }

        public void Touch()
        {
            currentState = EState.EVENT;
            if (callbackFunc != null)
            { callbackFunc.Execute(); }
        }
    }
}