using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.Interaction
{
    public class InteractionKickBall : MonoBehaviour
    {
        public TrailRenderer tailTemp;
        public Camera k_camera;
        public float tailStart = 7.5f;

        private float strength = 50f;

        #region Private variables
        private const int LAYER_KICKBALL = 16;
        private const float BALL_RADIUS = 7f;

        private const float POWER_MIN_Z = 0.75f;
        private const float POWER_MAX_Z = 1f;
        
        private InteractionKickBallMainController _mainController;
        public InteractionKickBallContentsController _contentsController;

        private Rigidbody _rigidbody;
        private TrailRenderer _trailRenderer;
        private Vector3 _shootDir;

        private int colCount = 0;

        #endregion


        void Start()
        {
            _contentsController = GetComponentInParent<InteractionKickBallContentsController>();
            k_camera = FindObjectOfType<Camera>();
            _mainController = GetComponentInParent<InteractionKickBallMainController>();
            _rigidbody = GetComponent<Rigidbody>();

            // Add Collider
            GameObject goCollider = new GameObject("Collidrer");
            goCollider.layer = LAYER_KICKBALL;
            goCollider.transform.parent = transform;

            goCollider.transform.localPosition = Vector3.zero;
            goCollider.transform.localRotation = Quaternion.identity;
            goCollider.transform.localScale = Vector3.one;

            SphereCollider collider = goCollider.AddComponent<SphereCollider>();
            collider.radius = BALL_RADIUS;
            collider.isTrigger = true;
            
            // Add Tail
            //GameObject goTail = new GameObject("Tail");

            GameObject goTail = (Instantiate(tailTemp.gameObject) as GameObject);

            goTail.SetActive(true);
            goTail.transform.parent = transform;

            goTail.transform.localPosition = Vector3.zero;
            goTail.transform.localRotation = Quaternion.identity;
            goTail.transform.localScale = Vector3.one;

            _trailRenderer = goTail.GetComponent<TrailRenderer>();//goTail.AddComponent<TrailRenderer>();
                                                                  //_trailRenderer.material = tailMat;
                                                                  //_trailRenderer.time = 0.15f;
            _trailRenderer.startWidth = tailStart * transform.localScale.x;
            //_trailRenderer.endWidth = 0.05f;


            InitBall();
        }

        void OnBecameVisible()
        {
            StopCoroutine(InitPositionCheck());
        }

        bool isRun = false;

        void OnEnable()
        {
            InitBall();
        }

        void OnDisable()
        {
            StopAllCoroutines();
            if(_mainController.gameObject.activeInHierarchy&&!gameObject.activeSelf)
            {
                _contentsController.SetStartingPoint(gameObject);
            }
        }

        IEnumerator InitPositionCheck()
        {
            if (isRun == false)
            {
                isRun = true;

                if (_shootDir != Vector3.zero)
                {
                    yield return new WaitForSeconds(2f);

                    _rigidbody.linearVelocity = Vector3.zero;
                    InitBall();

                    Transform[] posList = null;
                    if (_mainController.GetCurrentCollider() != null)
                    {
                        posList = _mainController.GetCurrentCollider().GetComponentsInChildren<Transform>();
                    }
                    if (posList != null && posList.Length > 0)
                        transform.position = posList[Random.Range(0, posList.Length)].position;

                    yield return new WaitForEndOfFrame();

                    //_rigidbody.AddForce(Vector3.down * strength);
                }

                isRun = false;
            }
        }

        void Update()
        {
            if (GetComponent<Renderer>().isVisible == false)
                StartCoroutine(InitPositionCheck());

            if (_shootDir != Vector3.zero)
            {
                Vector3 newDir = Vector3.right * -_shootDir.x;

                float pow = (1f - ((_shootDir.z - POWER_MIN_Z) / (POWER_MAX_Z - POWER_MIN_Z))) * 2f;
                _rigidbody.AddForce(newDir * strength * pow, ForceMode.Acceleration);
            }
            /*
            if (mapperView.motionType == MotionType.PunchMotion)
            {
                for (int i = 0; i < _rigidbodies.Length; i++)
                {
                    Rigidbody r = _rigidbodies[i];
                    if (r.transform.parent == _tf && r.velocity.sqrMagnitude >= strength * strength * 0.25f)
                    {
                        if (mainCamera != null)
                        {
                            Vector3 screenPoint = mainCamera.WorldToViewportPoint(r.transform.position);
                            mapperEffect.EffectPunch(-1, screenPoint.x , screenPoint.y );
                            Debug.Log("Punch! at "+i+"," + screenPoint.x  + "," + screenPoint.y );
                        }
                    }
                }
            }
            */
        }

        public void Shoot(bool inputLeft)
        {
            _shootDir = _mainController.mainCam.transform.right * Random.Range(0.05f, 0.1f)
                + _mainController.mainCam.transform.up * Random.Range(0.15f, 0.2f)
                + _mainController.mainCam.transform.forward * Random.Range(POWER_MIN_Z + 0.01f, POWER_MAX_Z);

            // 오른쪽으로 찼을때 왼쪽으로 공이 나가게끔
            if (inputLeft == false)
                _shootDir = new Vector3(-_shootDir.x, _shootDir.y, _shootDir.z);

            //dir = Vector3.forward + new Vector3(0, dir.y, dir.z);

            if (_rigidbody != null)
            {
                //Debug.Log("Shoot : " + _shootDir);
                _trailRenderer.gameObject.SetActive(true);
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.AddForce(_shootDir * strength * Random.Range(0.7f, 1f), ForceMode.Impulse);
            }
        }
        /*
        void OnCollisionEnter(Collision col)
        {
            if( _shootDir != Vector3.zero)
            {
                if (_manager.GetCurrentCollider().Equals(col.collider))
                {
                    colCount = 1;
                    _shootDir = _shootDir * 0.7f;
                    //_shootDir = Vector3.zero;
                    //_rigidbody.AddForce(Vector3.back * strength * 0.2f);
                }
            }
        }
        */
        /*
        void OnCollisionStay(Collision col)
        {
            if (colCount > 0)
            {
                Collider _col = _manager.GetCurrentCollider();
                if (!_col) return;
                if (_col.Equals(col.collider))
                {
                    colCount++;
                    if (colCount > 120)
                    {
                        if (isRun == false)
                        {
                            InitBall();
                            _rigidbody.AddForce((Vector3.back + Vector3.down) * strength * strength * 3f);
                        }
                    }
                }
            }
        }
        */
        void OnTriggerEnter(Collider col)
        {
            if (_mainController.isActiveAndEnabled)
            {
                if (col.name.Contains("GoalIn"))
                    _mainController.Goal();
            }
        }

        void InitBall()
        {
            colCount = 0;
            _shootDir = Vector3.zero;
            if (_trailRenderer != null)
                _trailRenderer.gameObject.SetActive(false);
        }
    }
}