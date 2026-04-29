using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class BoidController : MonoBehaviour
    {
        static BoidController _instance;
        public static BoidController instance
        {

            get
            {
                if (!_instance)
                    _instance = GameObject.FindObjectOfType<BoidController>();
                return _instance;
            }
        }


        List<BoidAgent> _boids;
        List<BoidAgent> boids
        {
            get
            {
                if (_boids == null) _boids = new List<BoidAgent>();
                return _boids;
            }
            set { _boids = value; }
        }

        void OnDisable()
        {
            if (boids == null) return;
            for (int i = 0; i < boids.Count; ++i)
            {
                Destroy(boids[i]);
            }

            boids.Clear();
            boids = null;
        }

        public void SetBoid(BoidAgent _boid)
        {
            if (!_boid) return;
            if (!boids.Contains(_boid))
                boids.Add(_boid);
        }

        public void Update()
        {
            for (int i = 0, len = boids.Count; i < len; ++i)
            {
                boids[i].Move(boids);
            }
        }
    }
}