using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    public class GrabCursorUI : MonoBehaviour
    {
        public UISprite handSprite;
        public UISprite progressSprite;

        private bool _grab = false;
        public bool grab
        {
            get
            {
                return _grab;
            }
            set
            {
                if (value == false)
                {
                    _currentGrabTime = 0.0f;
                }
                _grab = value;
                handSprite.spriteName = (_grab ? "fist" : "hand");
                animator.SetBool("grab", _grab);
            }
        }

        private Animator _animator = null;
        public Animator animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = GetComponent<Animator>();
                }
                return _animator;
            }
        }

        private float _grabTime = 1.0f;
        public float grabTime
        {
            get
            {
                return _grabTime;
            }
            set
            {
                _grabTime = Mathf.Clamp(value, 0.5f, 5.0f);
            }
        }

        private System.Action _onGrab = null;
        public System.Action onGrab
        {
            get
            {
                return _onGrab;
            }
            set
            {
                _onGrab = value;
            }
        }

        private float _currentGrabTime = 0.0f;

        void Start()
        {
            grab = false;
        }

        void Update()
        {
            if (grab)
            {
                if (_currentGrabTime >= grabTime)
                {
                    if (onGrab != null) onGrab();
                    grab = false;
                }
                else
                {
                    _currentGrabTime += Time.deltaTime;
                }

                progressSprite.fillAmount = _currentGrabTime / grabTime;
            }
        }
    }
}