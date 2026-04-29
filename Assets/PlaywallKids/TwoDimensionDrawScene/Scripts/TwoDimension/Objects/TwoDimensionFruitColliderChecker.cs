#define Rigidbody2D
using UnityEngine;
using System.Collections;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class TwoDimensionFruitColliderChecker : MonoBehaviour
    {
        public Vector3 dir;
#if Rigidbody2D
        void OnCollisionEnter2D(Collision2D _other)
        {
            CollisionCheck(_other.collider.gameObject.GetComponent<FruitObject>());
        }

        void OnCollisionStay2D(Collision2D _other)
        {
            CollisionCheck(_other.collider.gameObject.GetComponent<FruitObject>());
        }
#else
#endif

        void CollisionCheck(FruitObject _fruit)
        {
            Vector3 _dir = dir;
            if (_fruit == null) return;
            float _strength = Random.Range(_fruit.jumpStrength * 0.9f, _fruit.jumpStrength * 2f);
            if (dir.x == 0f) _dir.x = Random.Range(-0.5f, 0.5f);
            _fruit.Jump(_dir * _strength);
        }
    }
}