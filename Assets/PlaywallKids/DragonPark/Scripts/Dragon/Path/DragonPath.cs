#define USE_DOTWEEN

using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Base class of path on which dragon will move.
    /// Actually, it moves the dummy game object called "target". Then animation controller moves along its target.
    /// Subclasses can override the some methods to move the target on the custom paths.
    /// Also, it handles some animation states like sleep/wake.
    /// </summary>
    public class DragonPath : MonoBehaviour
    {
        #region Properties
        private System.WeakReference _dragonAnimation = null;
        /// <summary>
        /// Gets the animation controller.
        /// </summary>
        public DragonAnimationControl dragonAnimation
        {
            get
            {
                if (_dragonAnimation == null)
                {
                    DragonAnimationControl dragonAnimation = GetComponent<DragonAnimationControl>();
                    if (dragonAnimation != null)
                    {
                        _dragonAnimation = new System.WeakReference(dragonAnimation);
                    }
                }
                return (DragonAnimationControl)_dragonAnimation.Target;
            }
        }

        private GameObject _target = null;
        /// <summary>
        /// Dummy game object for moving along path.
        /// </summary>
        /// <value>The target.</value>
        public GameObject target
        {
            get
            {
                if (_target == null)
                {
                    GameObject pathTargets = GameObject.Find("DragonPath_Targets");
                    if (pathTargets == null)
                    {
                        pathTargets = new GameObject("DragonPath_Targets");
                    }

                    _target = new GameObject("target_" + name);
                    Transform t = _target.transform;

                    t.parent = pathTargets.transform;
                    t.position = transform.position;
                    t.localRotation = Quaternion.identity;
                    t.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }
                return _target;
            }
        }

        private Transform _cachedTransform = null;
        /// <summary>
        /// Gets the cached transform.
        /// </summary>
        public Transform cachedTransform
        {
            get
            {
                if (_cachedTransform == null)
                {
                    _cachedTransform = transform;
                }
                return _cachedTransform;
            }
        }

        private Transform _targetTransform = null;
        /// <summary>
        /// Cached target's transform.
        /// </summary>
        /// <value>The target transform.</value>
        public Transform targetTransform
        {
            get
            {
                if (_targetTransform == null)
                {
                    _targetTransform = target.transform;
                }
                return _targetTransform;
            }
        }

        /// <summary>
        /// target's position. (global)
        /// </summary>
        /// <value>The target position.</value>
        public Vector3 targetPosition
        {
            get
            {
                return targetTransform.position;
            }
        }

        private bool _movesTarget = false;
        public bool movesTarget
        {
            get
            {
                return _movesTarget;
            }
            set
            {
                if ((value && !_movesTarget) || !value)
                {
                    Vector3 pos = transform.position;
                    targetTransform.position = pos;
                }

                _movesTarget = value;
                if (_movesTarget)
                {
                    MovePath();
                }
                else
                {
                    StopPath();
                }
            }
        }
        #endregion

        #region Private variables
        protected bool _touched = false;
        protected float _frontLookTime = 0.0f;
        protected float _sleepTime = 0.0f;

        private Vector3 _prevDeadlockCheckPos = Vector3.zero;
        private float _currentDeadlockCheckTime = kDeadlockCheckTime;
        private bool _isDeadlockState = false;
        #endregion

        #region Constants
        public const float kWakeTime = 180.0f;
        public const float kDeadlockCheckTime = 6.0f;
        #endregion

        #region Unity Methods
        public virtual void OnDestroy()
        {
            if (_target != null)
            {
                StopPath();
                Destroy(_target);
                _target = null;
            }

            _movesTarget = false;
        }

        public virtual void Update()
        {
            if (_movesTarget)
            {
                _ProcessDeadlock();
                _ProcessInputs();
            }
            // touch state
            else if (_touched)
            {
                // Move again if action is over.
                if (dragonAnimation.willIdle || dragonAnimation.willMove)
                {
                    _touched = false;
                    movesTarget = true;
                }
                // views the front screen smoothly.
                else
                {
                    Vector3 cameraPos = Camera.main.transform.position;
                    cachedTransform.forward = Vector3.Slerp(cachedTransform.forward, (cameraPos - cachedTransform.position).normalized, _frontLookTime);
                    _frontLookTime += Time.deltaTime;
                }
            }

            if (dragonAnimation.isSleeping)
            {
                _sleepTime += Time.deltaTime;
                if (_sleepTime >= kWakeTime)
                {
                    dragonAnimation.Wake(false);
                    MovePath();
                }
                else if (dragonAnimation.speed > 2.0f)
                {
                    dragonAnimation.Wake(true);
                    MovePath();
                }
            }
            else
            {
                _sleepTime = 0.0f;
            }
        }
        #endregion

        #region Move/Stop
        /// <summary>
        /// Starts moving the dummy.
        /// </summary>
        public virtual void MovePath()
        {
            string pathName = GetRandomPathName();
            float speed = GetRandomSpeed(pathName);

            MovePath(pathName, speed, "pingpong");

            if (speed == 0.0f)
            {
                if (!this.gameObject.activeInHierarchy)
                    this.gameObject.SetActive(true);
                StartCoroutine(Sleep());
            }
        }

        /// <summary>
        /// Detailed version of <see cref="DragonPath.MovePath"/>. It has additional parameters (path name, speed, loop type). 
        /// </summary>
        /// <param name="pathName">Path name.</param>
        /// <param name="speed">Speed.</param>
        /// <param name="loopType">Loop type.</param>
        public virtual void MovePath(string pathName, float speed, string loopType)
        {
            // Debug.Log (string.Format("DragonPath.MovePath() : Moving {0} to path({1})", name, pathName));

            // Use DOTween based movement which is faster than iTween based.
#if USE_DOTWEEN
            var path = iTweenPath.GetPath(pathName);
            if (path != null)
                targetTransform.DOPath(path, speed, PathType.CatmullRom, PathMode.Full3D, 10).SetSpeedBased(true).SetId(GetInstanceID()).OnComplete(OnMovePathFinish);
#else
		iTween.MoveTo (target, iTween.Hash ("path", iTweenPath.GetPath (pathName),
		                                    "speed", speed,
		                                    "easetype", iTween.EaseType.linear,
		                                    "oncomplete", "OnMovePathFinish",
		                                    "oncompletetarget", gameObject,
		                                    "looptype", loopType,
		                                    "orienttopath", true));
#endif
        }

        /// <summary>
        /// Stops moving the dummy.
        /// </summary>
        public virtual void StopPath()
        {
#if USE_DOTWEEN
            DOTween.Kill(GetInstanceID());
#else
        if(_target != null)
            iTween.Stop(_target);
#endif
        }

        /// <summary>
        /// This method will be called when dummy has stopped or has finished to move.
        /// </summary>
        public virtual void OnMovePathFinish()
        {
            if (_movesTarget)
            {
                MovePath();
            }
        }

        /// <summary>
        /// Gets the random name of the path.
        /// </summary>
        /// <returns>The random path name.</returns>
        public string GetRandomPathName()
        {
            // path name
            string pathName = "";

            // random range (If the screen size is really large, expand the range)
            int range = 14;
            if (Camera.main.aspect > 3.555555f) range = 22;

            // Gets the path number randomly
            int pathNumber = 1 + Random.Range(0, range);

            // 1 ~ 7 : walk path
            if (pathNumber < 8)
            {
                pathName = string.Format("walkpath_{0}", pathNumber);
            }
            // 8 ~ 14 : run path
            else if (pathNumber < 15)
            {
                pathName = string.Format("runpath_{0}", pathNumber - 7);
            }
            // 15 ~ 18 : walk path (2 x 6)
            else if (pathNumber < 19)
            {
                pathName = string.Format("2x6_walkpath_{0}", pathNumber - 14);
            }
            // 19 ~ 22 : run path (2 x 6)
            else
            {
                pathName = string.Format("2x6_runpath_{0}", pathNumber - 18);
            }

            return pathName;
        }

        /// <summary>
        /// Gets the random movement speed of the path.
        /// </summary>
        /// <returns>The random speed.</returns>
        /// <param name="pathName">Path name.</param>
        public float GetRandomSpeed(string pathName)
        {
            if (!string.IsNullOrEmpty(pathName))
            {
                if (pathName.ToLower().Contains("walk"))
                {
                    Vector3 screenPos = Camera.main.WorldToScreenPoint(cachedTransform.position);

                    // We should validate that the dragon can sleep or not.
                    // There's little chance that the dragon can sleep. However, if the dragon is already sleeping, We have to wake it up.
                    // And we should not get the dragon to fall asleep when the dragon is out of the screen.
                    if (Random.Range(0.0f, 1.0f) >= 0.045f ||
                        dragonAnimation.isSleeping ||
                        new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.9f, Screen.height * 0.9f).Contains(screenPos))
                    {
                        return Random.Range(1.0f, 2.0f);
                    }
                    else
                    {
                        return 0;
                    }
                }
                else if (pathName.ToLower().Contains("run"))
                {
                    return Random.Range(3.0f, 4.0f);
                }
            }

            return Random.Range(1.0f, 2.0f);
        }
        #endregion

        #region Actions
        private void _ProcessDeadlock()
        {
            if (_currentDeadlockCheckTime >= kDeadlockCheckTime)
            {
                _currentDeadlockCheckTime -= kDeadlockCheckTime;
                _prevDeadlockCheckPos = cachedTransform.position;

                if (_isDeadlockState)
                {
                    MovePath();
                }

                _isDeadlockState = true;
            }
            else
            {
                _currentDeadlockCheckTime += Time.deltaTime;
                if (((_prevDeadlockCheckPos - cachedTransform.position).sqrMagnitude >= Mathf.Min(1.0f, dragonAnimation.maxSpeed * 0.5f) &&
                    dragonAnimation.movesAlongPath) || dragonAnimation.speed < 0.0001f)
                {
                    _currentDeadlockCheckTime = kDeadlockCheckTime;
                    _isDeadlockState = false;
                }
            }
        }

        private void _ProcessInputs()
        {
            for (int i = 0; i < CustomInput.touchCount; i++)
            {
                TouchInfo touch = CustomInput.GetTouch(i);

                // only act in press
                if (touch.phase != TouchInfo.Phase.Begin) continue;

                // UI Ray
                Ray uiRay = new Ray();
                if (UICamera.currentCamera != null)
                {
                    uiRay = UICamera.currentCamera.ScreenPointToRay(touch.position);
                }

                // Main Ray
                Vector3 point = touch.position;
                if (BackgroundManager.sharedInstance != null && BackgroundManager.sharedInstance.mainCamera != null)
                {
                    BarrelDistortionEffect distortionEffect = BackgroundManager.sharedInstance.mainCamera.GetComponent<BarrelDistortionEffect>();

                    if (distortionEffect != null)
                    {
                        point = distortionEffect.GetDistoredScreenPosFromOriginal(point);
                    }
                }
                Ray ray = Camera.main.ScreenPointToRay(point);

                // Hit info
                RaycastHit hit;
                RaycastHit uiHit = new RaycastHit();

                // UI hit check
                if (Physics.Raycast(uiRay, out uiHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("NGUI")))
                {
                    if (uiHit.collider.tag.Equals("MenuTouchArea"))
                        uiHit = default(RaycastHit);
                }

                // If user hits the dragon and don't hit the UI.
                if (Physics.Raycast(ray, out hit) && uiHit.collider == null)
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        // if dragon is sleeping, wake it up
                        if (dragonAnimation.isSleeping)
                        {
                            dragonAnimation.Wake(true);
                            MovePath();
                        }
                        // touch action
                        else if (!_touched && dragonAnimation.speed > 0)
                        {
                            _touched = true;
                            movesTarget = false;
                            _frontLookTime = 0.0f;
                            dragonAnimation.Touch_Test();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Makes dragon sleep. It is delayed by few seconds.
        /// </summary>
        public IEnumerator Sleep()
        {
            yield return new WaitForSeconds(5.0f);

            if (_movesTarget)
            {
                dragonAnimation.Sleep();
            }
        }
        #endregion
    }
}