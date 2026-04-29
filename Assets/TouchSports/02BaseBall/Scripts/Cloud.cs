using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.T_Sports.BaseBall
{
    public class Cloud : MonoBehaviour
    {
        public Transform Mytr;
        public float end = -100;
        public float StartX;
        public float RandomSpeed;
        public float min, max;
        private void Awake()
        {
            Mytr = this.transform;
            RandomSpeed = Random.Range(min, max);
        }
        void Update()
        {
            Mytr.Translate(Vector3.back* Time.deltaTime*RandomSpeed);
            if (Mytr.position.x < end)
            {
                RandomSpeed = Random.Range(min, max);
                Mytr.position = new Vector3(StartX, 45f, Random.Range(70, 120));
            }
        }
    }
}
