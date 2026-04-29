using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ML.MLBKids
{
    public class Ball : MonoBehaviour
    {
        public enum Type
        {
            Default, Bomb, Character
        }

        public Type type = Type.Default;
        public Vector3 from, to;
        public GameObject hitTrail;
        public GameObject effectPrefab;
        public GameObject effectPrefab2;
        public GameObject effectPrefab3;
        public Sprite[] characters;
        public float parabolaFactor;
        public float mistakeFactor;
        public float angle;
        public float speed;
        public float time;
        public bool hit;

        public static HashSet<Ball> list = new HashSet<Ball>();

        public void Awake()
        {
            if (hitTrail != null)
                hitTrail.SetActive(false);
            if (effectPrefab != null)
                effectPrefab.SetActive(false);
            if (effectPrefab2 != null)
                effectPrefab2.SetActive(false);
            if (effectPrefab3 != null)
                effectPrefab3.SetActive(false);
        }

        public void Start()
        {
            _SetPosition();
            list.Add(this);
        }

        public void OnEnable()
        {
            var spr = GetComponentInChildren<SpriteRenderer>(true);
            if (spr != null)
                spr.sprite = characters[Random.Range(0, characters.Length)];
        }

        public void OnDestroy()
        {
            list.Remove(this);
        }

        public static void Clear()
        {
            foreach (var ball in list)
            {
                DOTween.Kill(ball);
                Destroy(ball.gameObject);
            }
            list.Clear();
        }

        public void Update()
        {
            if (!hit)
            {
                time += Time.deltaTime * (1.0f - parabolaFactor) * speed;
                _SetPosition();
            }

            if (time >= 1.0f)
            {
                ScoreManager.AddScore(0, ScoreManager.ScoreType.Strike);
                Destroy(gameObject);
            }
        }

        private void _SetPosition()
        {
            Vector3 offset = (from - to);
            float magnitude = offset.magnitude;

            Vector3 direction = offset.normalized;
            Vector3 right = Vector3.Cross(direction, Vector3.up);
            Vector3 up = Vector3.Cross(right, direction);

            float radian = (90.0f + angle) * Mathf.PI / 180.0f;
            float parabolaLength = magnitude * 0.5f * parabolaFactor;
            Vector3 r = Mathf.Cos(radian) * right + Mathf.Sin(radian) * up;
            r = r * (parabolaLength * (1.0f - mistakeFactor) - parabolaLength * Mathf.Pow((time - 0.5f) * 2.0f, 2.0f));

            transform.position = Vector3.Lerp(from, to, time) + r;
            if (type != Type.Character)
                transform.Rotate(right, Time.deltaTime * 720.0f * speed);
        }

        public void Hit(Vector3 dir, float a, bool showsBat)
        {
            if (hit) return;

            hit = true;
            Collider c = gameObject.GetComponentInChildren<Collider>();
            Rigidbody rd = c.gameObject.AddComponent<Rigidbody>();
            rd.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rd.AddForce(dir * 36.0f * a, ForceMode.Impulse);

            if (showsBat)
            {
                Stadium stadium = Stadium.instance;
                InstantBat bat = Instantiate<InstantBat>(stadium.batPrefab);
                bat.dir = dir;
                bat.gameObject.SetActive(true);
            }

            if (effectPrefab != null)
            {
                var effect = Instantiate<GameObject>(effectPrefab);
                effect.SetActive(true);
                effect.transform.position = transform.position;
                Destroy(effect, 2.0f);
            }
            
            if (effectPrefab2 != null)
            {
                var effect = Instantiate<GameObject>(effectPrefab2);
                effect.SetActive(true);
                effect.transform.position = transform.position;
                Destroy(effect, 2.0f);
            }

            if (effectPrefab3 != null)
            {
                var effect = Instantiate<GameObject>(effectPrefab3);
                effect.SetActive(true);
                effect.transform.position = transform.position;
                Destroy(effect, 2.0f);
            }

            if (type == Type.Default || type == Type.Character)
            {
                if (a < 0.7f)
                {
                    SoundManager.PlaySFX("batting_hit");
                    ScoreManager.AddScore(ScoreManager.instance.ballHitScore, ScoreManager.ScoreType.Normal);
                }
                else
                {
                    SoundManager.PlaySFX("batting_Specal hit2");
                    ScoreManager.AddScore(ScoreManager.instance.ballHitNiceScore, ScoreManager.ScoreType.Nice);
                }

                if (hitTrail != null)
                    hitTrail.SetActive(true);
            }
            else
            {
                if (type == Type.Bomb)
                {
                    Stadium.instance.timer.AppendTime(2.0f);
                    SoundManager.PlaySFX("EXPLODE");
                }
                Destroy(gameObject);
            }
        }
    }
}