using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ML.MLBKids
{
    public class BallPitcher : MonoBehaviour
    {
        public Transform from, to;
        public GameObject ball;
        public GameObject bomb;
        public GameObject character;
        public Camera cam;

        [Range(0.01f, 0.5f)]
        public float maxParabolaFactor = 0.26f;

        [Range(0.01f, 1.0f)]
        public float maxMistakeFactor = 0.06f;

        [Range(0.0f, 180.0f)]
        public float angleRange = 114.0f;

        [Range(0.01f, 1.0f)]
        public float speedMin = 0.4f;
        [Range(0.01f, 2.0f)]
        public float speedMax = 0.6f;

        [Range(0.01f, 3.0f)]
        public float spawnTimeMin = 0.5f;
        [Range(0.01f, 5.0f)]
        public float spawnTimeMax = 3.0f;
        
        public Vector2 offsetRange = new Vector2(0.15f, 0.8f);

        [Range(0.0f, 0.5f)]
        public float bombRate = 0.1f;
        [Range(0.0f, 0.5f)]
        public float characterRate = 0.1f;

        public bool showsBat = false;

        private Tweener _camTween;
        private Coroutine _loop;
        private Transform _ballParent;
        private List<Touch> _touches = new List<Touch>();

        public void Awake()
        {
            ball.SetActive(false);
            bomb.SetActive(false);
            if (character != null)
                character.SetActive(false);
        }

        public void OnEnable()
        {
            if (_ballParent == null)
            {
                _ballParent = new GameObject("_balls").transform;
            }
            _loop = StartCoroutine(_BallLoop());
        }

        public void OnDisable()
        {
            if (_loop != null)
            {
                StopCoroutine(_loop);
                _loop = null;
            }
        }

        IEnumerator _BallLoop()
        {
            yield return null;

            while (true)
            {
                GameObject prefab = ball;
                if (bomb != null && Random.Range(0.0f, 1.0f) < bombRate) prefab = bomb;
                else if (character != null && Random.Range(0.0f, 1.0f - bombRate) < characterRate) prefab = character;
                
                GameObject go = Instantiate<GameObject>(prefab);
                go.SetActive(true);
                go.name = prefab.name;
                go.transform.parent = _ballParent;
                if (go.transform.GetChild(0) != null)
                    go.transform.GetChild(0).up = Random.insideUnitSphere;
                Ball ballScript = go.GetComponent<Ball>();
                if (ballScript == null)
                    ballScript = go.AddComponent<Ball>();

                Vector3 scrPoint = cam.WorldToScreenPoint(from.position);
                scrPoint.x = Random.Range(Screen.width * 0.1f, Screen.width * 0.9f);
                Vector3 worldPoint = cam.ScreenToWorldPoint(scrPoint);
                ballScript.from = worldPoint;
                scrPoint = cam.WorldToScreenPoint(to.position);
                scrPoint.x = Random.Range(Screen.width * 0.2f, Screen.width * 0.8f);
                worldPoint = cam.ScreenToWorldPoint(scrPoint);
                ballScript.to = worldPoint + Vector3.up * Random.Range(0.0f, offsetRange.y) + Vector3.right * Random.Range(-offsetRange.x, offsetRange.x);
                ballScript.angle = Random.Range(-angleRange, angleRange);
                ballScript.parabolaFactor = Random.Range(0.01f, maxParabolaFactor) * Mathf.Pow((angleRange - Mathf.Abs(ballScript.angle)) / angleRange, 1.74f);
                ballScript.mistakeFactor = Random.Range(0.0f, maxMistakeFactor);
                ballScript.speed = Random.Range(speedMin, speedMax);
                
                Debug.Log(string.Format("Created ball (angle : {0:0.0}, factor : {1:0.00})", ballScript.angle, ballScript.parabolaFactor));
                SoundManager.PlaySFX("batter_swing_weak");
                yield return new WaitForSeconds(Random.Range(spawnTimeMin, spawnTimeMax));
            }
        }

        private void Update()
        {
            if (Stadium.instance.isPlaying)
            {
                _ProcessInputs();
            }
        }
        
        private void _ProcessInputs()
        {
            _touches.Clear();
            Touch touch;

#if !UNITY_EDITOR
            if (!Input.touchSupported)
#endif
            {
                if (Input.GetMouseButtonDown(0))
                {
                    touch = new Touch();
                    touch.position = Input.mousePosition;
                    touch.phase = TouchPhase.Began;
                    _touches.Add(touch);
                }
            }
#if !UNITY_EDITOR
            else
#endif
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    touch = Input.GetTouch(i);
                    if (touch.phase == TouchPhase.Began)
                    {
                        _touches.Add(touch);
                    }
                }
            }

            for (int i = 0; i < _touches.Count; i++)
            {
                touch = _touches[i];
                RaycastHit hitInfo;

                if (Physics.Raycast(cam.ScreenPointToRay(touch.position), out hitInfo))
                {
                    Ball ballScript = hitInfo.collider.GetComponentInParent<Ball>();
                    if (ballScript != null && !ballScript.hit)
                    {
                        Vector3 normal = hitInfo.normal;
                        float accuracy = Vector3.Dot(cam.transform.forward, -normal);
                        if (ballScript.type == Ball.Type.Character)
                        {
                            Debug.Log("Character ball!");
                            normal = -cam.transform.forward;
                            accuracy = 1.0f;
                        }
                        float a = Mathf.Pow(accuracy, 1.65f);
                        ballScript.Hit((cam.transform.forward + cam.transform.up * 0.5f - normal * 0.5f).normalized, a, showsBat);

                        bool playTween = a >= 0.7f || ballScript.type == Ball.Type.Bomb;
                        if (playTween)
                        {
                            if (_camTween != null)
                            {
                                _camTween.Complete();
                                _camTween = null;
                            }
                            if (ballScript.type == Ball.Type.Bomb)
                            {
                                _camTween = cam.transform.DOShakePosition(1.0f, 0.4f, 40);
                            }
                            else
                            {
                                _camTween = cam.transform.DOShakePosition(a * 0.6f, accuracy * 0.166f, 16);
                            }
                        }

                        Stadium.instance.Test(cam, normal);

                        Debug.Log("accuracy : " + accuracy * 100);
                        _touches.RemoveAt(i--);
                    }
                }
            }
        }
    }
}