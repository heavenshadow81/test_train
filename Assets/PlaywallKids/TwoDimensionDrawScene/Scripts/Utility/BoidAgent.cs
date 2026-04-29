using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class BoidAgent : MonoBehaviour
    {
        public enum EBoidType { NONE = -1, FLOCKING_ELEMENT = 1, OBSTACLE, CHASSEUR, ALONE }
        public EBoidType type;
        public float space;
        public float sight;
        public float border;
        public Vector3 speed;
        [HideInInspector]
        public Vector3 dir;

        GameObject obj;
        Transform cachedTransform;
        bool bMove;

        Vector3 position
        {
            get
            {
                if (!cachedTransform) cachedTransform = this.transform;
                return cachedTransform.localPosition;
            }
            set { cachedTransform.localPosition = value; }
        }

        public bool activeInHierarchy
        {
            get { return obj.activeInHierarchy; }
        }

        void Awake()
        {
            obj = this.gameObject;
            cachedTransform = this.transform;
        }

        void OnEnable()
        {
            bMove = false;
        }

        public void Move(List<BoidAgent> boids)
        {
            if (!bMove) return;

            switch (type)
            {
                default: return;
                case EBoidType.FLOCKING_ELEMENT:
                    Flock(boids);
                    break;
                case EBoidType.ALONE:
                    Avoidance(boids);
                    break;
                case EBoidType.CHASSEUR:
                    Chase(boids);
                    break;
            }

            CheckBoundary();
            CheckSpeed();
            transform.localPosition += dir;
        }

        public void MoveStart()
        {
            bMove = (type != EBoidType.OBSTACLE && EBoidType.NONE != type);
        }

        public void MoveStop()
        {
            bMove = false;
        }

        private void CheckSpeed()
        {
            Vector3 _speed = speed;
            if (type == EBoidType.CHASSEUR)
            {
                _speed *= 0.5f;
            }

            float _currentSpeed = UtilityScript.Distance(Vector3.zero, dir);
            float _limitedSpeed = UtilityScript.Distance(Vector3.zero, _speed);
            if (_currentSpeed > _limitedSpeed)
            {
                dir.x *= _speed.x;
                dir.y *= _speed.y;
                dir.z *= _speed.z;
                dir = dir / _currentSpeed;

                if (dir.x > 10f) dir.x *= 0.1f;
                if (dir.y > 10f) dir.y *= 0.1f;
                if (dir.z > 10f) dir.z *= 0.1f;
            }
        }

        private void Flock(List<BoidAgent> _boids)
        {
            for (int i = 0, len = _boids.Count; i < len; ++i)
            {
                BoidAgent _boid = _boids[i];
                float _sqrtDistance = UtilityScript.SqrtDistance(position, _boid.position);
                if (_sqrtDistance != 0f && _boid.type == EBoidType.FLOCKING_ELEMENT)
                {
                    if (_sqrtDistance < space * space)
                    {
                        dir += (position - _boid.position);
                    }
                    else if (_sqrtDistance < sight * sight)
                    {
                        float _gap = -0.05f;
                        dir += (_boid.position - position) * _gap;
                    }

                    if (_sqrtDistance < sight * sight)
                    {
                        dir += _boid.dir * 0.5f;
                    }
                }
                else if (_sqrtDistance < sight * sight)
                {
                    dir += (position - _boid.position);
                }
            }
        }

        private void Avoidance(List<BoidAgent> _boids)
        {
            for (int i = 0, len = _boids.Count; i < len; ++i)
            {
                dir += Avoidance(_boids[i].position);
            }
        }

        private Vector3 Avoidance(Vector3 _position)
        {
            if (UtilityScript.SqrtDistance(position, _position) < sight * sight)
            {
                return (position - _position);
            }
            return Vector3.zero;
        }

        private Vector3 Avoidance(Vector3 _position, float _sqrDistance)
        {
            if (_sqrDistance < sight * sight)
            {
                return (position - _position);
            }

            return Vector3.zero;
        }

        private void Chase(List<BoidAgent> _boids)
        {
            float _range = float.MaxValue;
            BoidAgent _target = null;

            for (int i = 0, len = _boids.Count; i < len; ++i)
            {
                BoidAgent _boid = _boids[i];
                if (this != _boid && _boid.type == EBoidType.FLOCKING_ELEMENT)
                {
                    float _sqrDistance = UtilityScript.SqrtDistance(position, _boid.position);
                    if (_sqrDistance < (sight * sight) && _sqrDistance < _range)
                    {
                        _range = _sqrDistance;
                        _target = _boid;
                    }
                }
                else
                {
                    dir += Avoidance(_boid.position);
                }
            }

            if (_target)
                dir += _target.position - position;
        }

        private void CheckBoundary()
        {
            float border_w = (UtilityScript.width - border) * 0.5f - border;
            float border_h = (UtilityScript.height - border) * 0.5f - border;

            if (position.x > border_w) { dir.x += border_w - position.x; }
            else if (position.x < -border_w) { dir.x += (-1 * border_w) - position.x; }
            if (position.y > border_h) { dir.y += border_h - position.y; }
            else if (position.y < -border_h) { dir.y += (-1 * border_h) - position.y; }
        }
    }
}