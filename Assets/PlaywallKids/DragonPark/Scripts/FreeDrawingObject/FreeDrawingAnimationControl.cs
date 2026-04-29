using UnityEngine;

namespace ML.PlaywallKids.DragonPark
{
    public class FreeDrawingAnimationControl : DragonAnimationControl
    {
        #region Properties
        private bool _rising = false;
        public bool rising
        {
            get
            {
                return _rising;
            }
            private set
            {
                _DisableNavMesh();

                _rising = value;

                if (_rising)
                {
                    if (_type.Equals(FreeDrawingObjectType.Robot))
                    {
                        animator.SetBool("Fly", true);
                    }
                }

                _ResetMovingAlongPath();
            }
        }

        private bool _landing = false;
        public bool landing
        {
            get
            {
                return _landing;
            }
            private set
            {
                _DisableNavMesh();

                _landing = value;

                if (_landing || (!_rising && !_landing))
                {
                    if (_type.Equals(FreeDrawingObjectType.Robot))
                    {
                        animator.SetBool("Fly", false);
                    }
                }

                _ResetMovingAlongPath();
            }
        }

        private bool _cachedMovesAlongPath = false;
        public override bool movesAlongPath
        {
            get
            {
                return base.movesAlongPath;
            }
            set
            {
                if (!_landing && !_rising)
                {
                    base.movesAlongPath = value;
                }

                _cachedMovesAlongPath = value;
            }
        }

        private bool _dancing = false;
        public bool dancing
        {
            get
            {
                return _dancing;
            }
        }

        public DragonComeToFront comeToFront;
        #endregion

        #region Constants
        public const float kDanceTime = 3.0f;
        public const float kRobotActionTime = 15.0f;
        #endregion

        #region Private variables
        private bool _landAfterRise = false;
        private Vector3 _riseDestination = Vector3.zero;
        private FreeDrawingObjectType _type;
        private float _danceTime = 0.0f;

        private float _robotActionTime = 0.0f;
        private int _currentRobotAction = 0;
        #endregion

        #region Unity Methods
        public override void Awake()
        {
            base.Awake();

            _cachedMovesAlongPath = movesAlongPath;

            FreeDrawingObjectBone freeDrawingObject = GetComponent<FreeDrawingObjectBone>();
            _type = freeDrawingObject.objectType;

            comeToFront = gameObject.AddComponent<DragonComeToFront>();
        }

        public override void Start()
        {
            if (path == null)
            {
                if (_type == FreeDrawingObjectType.Airplane)
                {
                    path = gameObject.AddComponent<DragonSkyPath>();
                }
                else
                {
                    path = gameObject.AddComponent<FreeDrawingObjectPath>();
                }
                path.movesTarget = false;

                if (navMeshAgent != null)
                {
                    if (_type.Equals(FreeDrawingObjectType.Car))
                        navMeshAgent.radius = 1.5f;
                }

                movesAlongPath = false;
            }

            base.Start();
        }
        #endregion

        public override void Update()
        {
            base.Update();

            if (landing)
            {
                Vector3 from = transform.position;
                RaycastHit hitInfo;

                if (Physics.Raycast(from, Physics.gravity, out hitInfo))
                {
                    Vector3 to = hitInfo.point;

                    Vector3 dist = Vector3.Min(Physics.gravity * Time.deltaTime, (to - from) * 0.5f * Time.deltaTime);

                    transform.position += dist;

                    if ((from - to).sqrMagnitude < 0.01f)
                    {
                        landing = false;
                    }
                }
                else
                {
                    landing = false;
                }
            }
            else if (rising)
            {
                Vector3 from = transform.position;
                Vector3 to = _riseDestination;

                if ((from - to).sqrMagnitude < 0.01f)
                {
                    rising = false;

                    if (_landAfterRise)
                    {
                        Land();
                    }
                }
                else
                {
                    Vector3 dist = (to - from).normalized * 4.0f * Time.deltaTime;

                    transform.position += dist;
                }
            }
            else if (_dancing)
            {
                _danceTime += Time.deltaTime;
                if (_danceTime >= kDanceTime)
                {
                    _dancing = false;
                    animator.SetBool("Dance", false);
                    movesAlongPath = true;
                }
            }
            else if (_type.Equals(FreeDrawingObjectType.Robot))
            {
                if (movesAlongPath)
                    _robotActionTime += Time.deltaTime;

                if (_robotActionTime >= kRobotActionTime)
                {
                    _robotActionTime -= kRobotActionTime;

                    _currentRobotAction = (_currentRobotAction + 1) % 2;

                    if (_currentRobotAction == 0)
                    {
                        Rise(8.0f);
                    }
                    else
                    {
                        Dance();
                    }
                }
            }
        }

        #region Special Actions
        public void Rise(float height)
        {
            Rise(height, true);
        }

        public void Rise(float height, bool landAfterRise)
        {
            if (height < 0.001f) return;

            if (!landing)
            {
                _riseDestination = transform.position + Vector3.up * height;

                _landAfterRise = landAfterRise;

                rising = true;
            }
        }

        public void Land()
        {
            Vector3 from = transform.position;
            RaycastHit hitInfo;

            if (Physics.Raycast(from, Vector3.down, out hitInfo))
            {
                Vector3 to = hitInfo.point;
                if ((from - to).sqrMagnitude >= 0.1f)
                {
                    landing = true;
                }
                else
                {
                    landing = false;
                }
            }
            else
            {
                landing = false;
            }
        }
        #endregion

        #region Private methods
        private void _DisableNavMesh()
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = false;
            }
        }

        private void _EnableNavMesh()
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = true;
            }
        }

        public void _ResetMovingAlongPath()
        {
            if (!_landing && !_rising)
            {
                base.movesAlongPath = _cachedMovesAlongPath;

                _EnableNavMesh();
            }
            else
            {
                _cachedMovesAlongPath = base.movesAlongPath;
                base.movesAlongPath = false;
            }
        }

        public void Dance()
        {
            if (_type.Equals(FreeDrawingObjectType.Robot) && (!_landing && !_rising))
            {
                _dancing = true;
                _danceTime = 0.0f;
                animator.SetBool("Dance", true);

                movesAlongPath = false;
            }
        }
        #endregion
    }
}