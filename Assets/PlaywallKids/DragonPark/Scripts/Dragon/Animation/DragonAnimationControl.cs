using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    /// <summary>
    /// Base class of animating dragon object.
    /// It moves along its "dragonPath" path.
    /// </summary>
    public class DragonAnimationControl : MonoBehaviour
    {
        #region Properties
        private System.WeakReference _dragon = null;
        /// <summary>
        /// Gets the "Dragon" component.
        /// </summary>
        /// <value>The "Dragon" component.</value>
        public Dragon dragon
        {
            get
            {
                if (_dragon == null)
                {
                    _dragon = new System.WeakReference(GetComponent<Dragon>());
                }
                return (Dragon)_dragon.Target;
            }
        }

        private DragonPath _path = null;
        /// <summary>
        /// Current path component.
        /// </summary>
        /// <value>The path.</value>
        public DragonPath path
        {
            get
            {
                if (_path == null)
                {
                    _path = GetComponent<DragonPath>();
                    if (_path != null)
                        _path.movesTarget = movesAlongPath;
                }
                return _path;
            }
            set
            {
                _path = value;
                _path.movesTarget = movesAlongPath;
            }
        }

        private DragonEffect _effect;
        public DragonEffect effect
        {
            get
            {
                if (_effect == null)
                    _effect = GetComponent<DragonEffect>();
                return _effect;
            }
        }

        [SerializeField, HideInInspector]
        private float _speed = 0.0f;
        /// <summary>
        /// Current moving speed. This value will be applied in animator automatically.
        /// </summary>
        public float speed
        {
            get
            {
                return _speed;
            }
        }

        [SerializeField, HideInInspector]
        private float _maxSpeed = 3.5f;
        /// <summary>
        /// Maximum speed of dragon.
        /// </summary>
        /// <value>The maximum speed.</value>
        public float maxSpeed
        {
            get
            {
                return _maxSpeed;
            }
            set
            {
                _maxSpeed = Mathf.Clamp(value, 1.0f, 8.0f);
                if (navMeshAgent != null)
                {
                    navMeshAgent.speed = _maxSpeed;
                }
            }
        }

        [SerializeField, HideInInspector]
        private bool _movesAlongPath = true;
        /// <summary>
        /// Is dragon moving along the path?
        /// </summary>
        public virtual bool movesAlongPath
        {
            get
            {
                return _movesAlongPath;
            }
            set
            {
                _movesAlongPath = value;
                if (path != null)
                {
                    path.movesTarget = _movesAlongPath;
                }
            }
        }

        /// <summary>
        /// Is dragon idle?
        /// </summary>
        /// <value>Returns <c>true</c> if it is idle; otherwise, <c>false</c>.</value>
        public bool isIdle
        {
            get
            {
                AnimatorClipInfo[] infos = animator.GetCurrentAnimatorClipInfo(0);
                if (infos != null)
                {
                    if (infos.Length > 0 && infos[0].clip != null)
                    {
                        return infos[0].clip.name.ToLower().Contains("idle") && !infos[0].clip.name.ToLower().Contains("grooming");
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Checks if the dragon will be idle.
        /// </summary>
        public bool willIdle
        {
            get
            {
                AnimatorClipInfo[] infos = animator.GetNextAnimatorClipInfo(0);
                if (infos != null)
                {
                    if (infos.Length > 0 && infos[0].clip != null)
                    {
                        return infos[0].clip.name.ToLower().Contains("idle") && !infos[0].clip.name.ToLower().Contains("grooming");
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Is dragon moving?
        /// </summary>
        public bool isMoving
        {
            get
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                return stateInfo.IsName("Move");
            }
        }

        /// <summary>
        /// Checks if the dragon will move.
        /// </summary>
        public bool willMove
        {
            get
            {
                AnimatorStateInfo stateInfo = animator.GetNextAnimatorStateInfo(0);
                return stateInfo.IsName("Move");
            }
        }

        /// <summary>
        /// Is dragon sleeping?
        /// </summary>
        public bool isSleeping
        {
            get
            {
                bool sleep = false;
                if (animatorHasSleepProperty)
                    sleep = animator.GetBool("sleep");
                return sleep;
            }
        }

        private Animator _animator = null;
        /// <summary>
        /// Gets the animator.
        /// </summary>
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

        [SerializeField, HideInInspector]
        private bool _usesNavMesh = true;
        /// <summary>
        /// Will animation controller use nav mesh agent?
        /// Nav mesh can only be used in the pre-baked nav mesh ground and grounded dragon.
        /// If dragon needs to be flying(for example, Hansen), this flag must be false.
        /// </summary>
        /// <value><c>true</c> if uses nav mesh; otherwise, <c>false</c>.</value>
        public bool usesNavMesh
        {
            get
            {
                return _usesNavMesh;
            }
            set
            {
                _usesNavMesh = value;
                if (!_usesNavMesh)
                {
                    if (_navMeshAgent == null)
                        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
                    if (_navMeshAgent != null)
                        Destroy(_navMeshAgent);
                    _navMeshAgent = null;
                }
            }
        }

        [SerializeField, HideInInspector]
        private UnityEngine.AI.NavMeshAgent _navMeshAgent = null;
        /// <summary>
        /// Nav Mesh Agent.
        /// </summary>
        public UnityEngine.AI.NavMeshAgent navMeshAgent
        {
            get
            {
                if (_usesNavMesh)
                {
                    if (_navMeshAgent == null)
                    {
                        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

                        if (_navMeshAgent == null)
                        {
                            _navMeshAgent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
                        }

                        // initialize
                        _navMeshAgent.radius = 0.4f;
                        _navMeshAgent.speed = maxSpeed;
                    }
                }
                return _navMeshAgent;
            }
        }

        /*
         * Animator Parameters
         */
        public bool animatorHasSpeedProperty { get; protected set; }
        public bool animatorHasSleepProperty { get; protected set; }
        public bool animatorHasStandUpProperty { get; protected set; }
        public bool animatorHasFlyProperty { get; protected set; }
        #endregion

        #region Private Properties
        /// <summary>
        /// Internal property of "speed" with setter.
        /// </summary>
        /// <value>The speed of dragon.</value>
        private float _speed_Internal
        {
            get
            {
                return speed;
            }
            set
            {
                _speed = value;

                // tolerance error correction
                if (_speed < 0.01f)
                {
                    _speed = 0.0f;
                }

                // set animator value
                if (animatorHasSpeedProperty)
                    animator.SetFloat(_animatorSpeedHash, _speed * 0.333f, 0.5f, Time.deltaTime);
            }
        }
        #endregion

        #region Private vairables
        /*
         * Animator Hashes (Transition)
         */
        private int _animatorSpeedHash = -1;
        private int _animatorEatHash = -1;
        private int _animatorSleepHash = -1;
        private int _animatorStandUpHash = -1;
        private int _animatorCharmingActHash = -1;
        private int _animatorTouch1Hash = -1;
        private int _animatorTouch2Hash = -1;
        private int _animatorWinHash = -1;
        private int _animatorLoseHash = -1;
        private int _animatorSitHash = -1;
        private int _animatorFlyHash = -1;

        // Eat!
        private GameObject _food = null;

        // Prev Position (for calculating speed)
        private Vector3 _prevPos;


        private bool bUseTemplete3D = false;
        #endregion

        #region Unity methods
        // Use this for initialization
        public virtual void Awake()
        {
            _InitAnimatorHashes();
            _CheckAnimatorParameters();
        }

        public virtual void Start()
        {
            if (path == null)
            {
                // Makes default path (ground random path)
                path = gameObject.AddComponent<DragonPath>();

                // Move automatically
                path.movesTarget = _movesAlongPath;
            }

            _prevPos = transform.position;
            _prevPos.y = 0;
        }

        // Update is called once per frame
        public virtual void Update()
        {
            if (movesAlongPath && (path != null && path.movesTarget))
            {
                if (bUseTemplete3D == false)
                    this.SetDestination(path.targetPosition);
            }

            // Calculate speed
            Vector3 currentPos = transform.position;
            currentPos.y = 0;
            _speed_Internal = (currentPos - _prevPos).magnitude / Time.deltaTime;
            _prevPos = currentPos;

            if (Input.GetKeyDown(KeyCode.T))
                Touch_Test();
        }

        public virtual void OnEnable()
        {
            if (_food != null)
            {
                Eat(_food);
            }
        }
        #endregion

        #region Initialization
        protected virtual void _InitAnimatorHashes()
        {
            _animatorSpeedHash = Animator.StringToHash("speed");
            _animatorEatHash = Animator.StringToHash("eat");
            _animatorSleepHash = Animator.StringToHash("sleep");
            _animatorStandUpHash = Animator.StringToHash("stand_up");
            _animatorCharmingActHash = Animator.StringToHash("charming_act");
            _animatorTouch1Hash = Animator.StringToHash("touch1");
            _animatorTouch2Hash = Animator.StringToHash("touch2");
            _animatorWinHash = Animator.StringToHash("win");
            _animatorLoseHash = Animator.StringToHash("lose");
            _animatorSitHash = Animator.StringToHash("sit");
            _animatorFlyHash = Animator.StringToHash("fly");

        }

        protected virtual void _CheckAnimatorParameters()
        {
            foreach (var parameter in animator.parameters)
            {
                if (parameter.nameHash == _animatorSpeedHash)
                    animatorHasSpeedProperty = true;
                else if (parameter.nameHash == _animatorSleepHash)
                    animatorHasSleepProperty = true;
                else if (parameter.nameHash == _animatorStandUpHash)
                    animatorHasStandUpProperty = true;
                else if (parameter.nameHash == _animatorFlyHash)
                    animatorHasFlyProperty = true;
            }
        }

        public void UseTemplete3D()
        {
            bUseTemplete3D = true;
            Destroy(GetComponent<Collider>());
        }
        #endregion

        #region Moving
        public virtual void SetDestination(Vector3 target)
        {
            if (_usesNavMesh && navMeshAgent != null)
            {
                if (navMeshAgent.isActiveAndEnabled)
                    navMeshAgent.SetDestination(target);
            }
            else
            {
                Vector3 velocity = (target - transform.position) * 0.5f;
                Vector3 direction = velocity.normalized;

                velocity = velocity.magnitude <= maxSpeed ? velocity : direction * maxSpeed;
                float magnitude = velocity.magnitude;

                transform.forward = Vector3.Slerp(transform.forward, direction, Time.deltaTime);

                if (magnitude > 0)
                {
                    transform.position += velocity * Time.deltaTime;
                }
            }
        }

        public virtual void Stop()
        {
            if (_usesNavMesh && navMeshAgent != null)
            {
                navMeshAgent.isStopped = true;
            }
        }
        #endregion

        #region Actions
        public virtual void Eat(GameObject food)
        {
            animator.SetTrigger(_animatorEatHash);
            _food = food;
        }

        /// <summary>
        /// This method will be called by the "Eat_..." animation.
        /// </summary>
        public virtual void PerformEatFood()
        {
            if (_food != null)
            {
                Rigidbody rigidbody = _food.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    float headScale = dragon.GetBoneScale(BoneObject.kHeadBone);

                    //rigidbody.AddForce(new Vector3(-0.75f, 5.28f, 0.0f), ForceMode.VelocityChange);
                    rigidbody.AddForce(new Vector3(0f, Mathf.Lerp(5.28f, 5.76f, headScale - 1.0f), 15f), ForceMode.VelocityChange);
                    Destroy(rigidbody.GetComponent<Collider>());
                }

                Destroy(_food, 0.85f);
            }
        }

        public virtual void Sleep()
        {
            if (animatorHasSleepProperty)
                animator.SetBool(_animatorSleepHash, true);
        }

        public virtual void CharmingAct()
        {
            animator.SetTrigger(_animatorCharmingActHash);
        }

        public virtual void Wake(bool standUp)
        {
            if (animatorHasStandUpProperty)
                animator.SetBool(_animatorStandUpHash, standUp);
            if (animatorHasSleepProperty)
                animator.SetBool(_animatorSleepHash, false);
        }


        public virtual void Touch_Test()
        {
            transform.LookAt(Camera.main.transform);
            if (Random.Range(0.0f, 1.0f) >= 0.5f)
            {
                animator.SetTrigger(_animatorTouch1Hash);
            }
            else
            {
                animator.SetTrigger(_animatorTouch2Hash);
            }
        }

        public void Win(bool flag = true)
        {
            animator.SetBool(_animatorWinHash, flag);
        }

        public void Lose(bool flag = true)
        {
            animator.SetBool(_animatorLoseHash, flag);
        }

        public void Sit()
        {
            animator.SetBool(_animatorSitHash, true);
        }
        #endregion
    }
}

public class DragonAnimationControl : ML.PlaywallKids.DragonPark.DragonAnimationControl { }
