using UnityEngine;
using DG.Tweening;

namespace ML.MLBKids
{
    public class BallPlayerPitcher : MonoBehaviour
    {
        public StrikeZone strikeZone;
        public PlayerBall playerBall;
        public GameObject table;
        public float speedFactor = 1.0f;

        public bool readyToPitch { get; private set; }

        public event System.Action onBallSpawn;
        public event System.Action onBallPitch;

        private Tween _pitchTween;
        private GameObject _tableBall;

        public void Awake()
        {
            playerBall.gravityScale = 0;
            playerBall.gameObject.SetActive(false);
            readyToPitch = true;
        }

        public void OnEnable()
        {
            if (table.gameObject.activeSelf)
            {
                if (_tableBall == null)
                {
                    PlayerBall tableBall = Instantiate<PlayerBall>(playerBall, playerBall.transform.parent, true);
                    _tableBall = tableBall.gameObject;
                    Destroy(tableBall);
                    _tableBall.name = "Ball(table)";
                    Rigidbody rigidbody = _tableBall.GetComponent<Rigidbody>();
                    rigidbody.isKinematic = true;
                }
                _tableBall.SetActive(true);
                readyToPitch = false;
                var scale = _tableBall.transform.localScale;
                _tableBall.transform.localScale = Vector3.zero;
                _pitchTween = _tableBall.transform.DOScale(scale, 0.5f).OnComplete(() =>
                {
                    readyToPitch = true;
                    if (onBallSpawn != null)
                        onBallSpawn();
                });
            }
            else
            {
                if (_tableBall != null)
                    _tableBall.gameObject.SetActive(false);
            }
        }

        public void OnDisable()
        {
            if (_pitchTween != null)
            {
                _pitchTween.Complete(true);
                _pitchTween = null;
            }
        }

        //public void Start()
        //{
        //    if (KinectHelper.instance != null)
        //        KinectHelper.instance.onPitch += _OnPitch;
        //}

        //public void OnDestroy()
        //{
        //    if (KinectHelper.instance != null)
        //        KinectHelper.instance.onPitch -= _OnPitch;
        //}

#if UNITY_EDITOR
        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _OnPitch(true, 1, new Vector2((Input.mousePosition.x - Screen.width * 0.5f) / (Screen.width * 0.5f),
                    (Input.mousePosition.y - Screen.height * 0.5f) / (Screen.height * 0.5f)), Vector3.forward + Vector3.up * Random.Range(0.2f, 0.5f) + Vector3.right * Random.Range(-0.2f, 0.2f), 1.0f);
            }
        }
#endif

        private void _OnPitch(bool isRightHand, ulong trackingId, Vector2 normalizedPos, Vector3 direction, float powerFactor)
        {
            if (!Stadium.instance.isPlaying || Stadium.instance.paused) return;
            if (!readyToPitch) return;

            Camera cam = Stadium.instance.cams[(int)Stadium.GameMode.Pitch];

            Vector3 screenPoint = (normalizedPos + Vector2.one) * 0.5f;
            screenPoint.x *= Screen.width;
            screenPoint.y *= Screen.height;
            screenPoint.z = 1.0f;
            Vector3 from = cam.ScreenToWorldPoint(screenPoint);

            PlayerBall newBall = Instantiate<PlayerBall>(playerBall, playerBall.transform.parent, true);
            newBall.gameObject.SetActive(true);
            if (!table.gameObject.activeSelf)
            {
                newBall.transform.position = from;
            }
            else
            {
                readyToPitch = false;
                var scale = _tableBall.transform.localScale;
                _tableBall.transform.localScale = Vector3.zero;
                SoundManager.PlaySFX("CHIMES2", false, 1.5f);
                _pitchTween = _tableBall.transform.DOScale(scale, 0.5f).SetDelay(1.5f).OnComplete(() =>
                {
                    readyToPitch = true;
                    if (onBallSpawn != null)
                        onBallSpawn();
                });
            }
            
            Vector3 distStrikeZone = (strikeZone.transform.position - strikeZone.transform.forward * 0.8f) - from;
            Debug.Log(distStrikeZone.ToString("0.000"));

            float basePower = Mathf.Abs(distStrikeZone.z);
            float height = Mathf.Abs(distStrikeZone.y);
            float time = 1.0f / speedFactor / powerFactor;
            
            newBall.gravityScale = 1.0f / (time * time);
            
            Rigidbody rigidbody = newBall.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
            rigidbody.useGravity = false;

            Vector3 force = Vector3.forward * basePower / time;
            force += Vector3.up * (Mathf.Abs(Physics.gravity.y) * 0.5f / time + height / time);
            Vector3 rd = Random.insideUnitSphere;
            rd.x *= Random.Range(0.0f, 4.0f);
            rd.y *= Random.Range(0.0f, 2.0f);
            force += rd;
            rigidbody.AddForce(force, ForceMode.Impulse);

            Debug.Log(force);
            
            SoundManager.PlaySFX("batter_swing_weak");

            if (onBallPitch != null)
                onBallPitch();
        }
    }
}