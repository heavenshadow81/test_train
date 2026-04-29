using System.Collections.Generic;
using UnityEngine;

namespace ML.MLBKids
{
    public class PlayerBall : MonoBehaviour
    {
        public bool hit = false;
        public float gravityScale = 1.0f;
        public GameObject effectPrefab, effectPrefab2;
        public static HashSet<PlayerBall> list = new HashSet<PlayerBall>();
        private Rigidbody _rigidbody;

        public void Start()
        {
            _rigidbody = GetComponentInChildren<Rigidbody>();
            list.Add(this);
        }

        public void OnDestroy()
        {
            list.Remove(this);
        }

        public void FixedUpdate()
        {
            var gravity = Physics.gravity;
            if (_rigidbody != null)
                _rigidbody.AddForce(gravity * gravityScale, ForceMode.Acceleration);
        }

        public static void Clear()
        {
            foreach (var ball in list)
            {
                DG.Tweening.DOTween.Kill(ball);
                Destroy(ball.gameObject);
            }
            list.Clear();
        }
    }
}