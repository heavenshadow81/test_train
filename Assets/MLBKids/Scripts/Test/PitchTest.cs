using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.MLBKids
{
    public class PitchTest : MonoBehaviour
    {
        public GameObject ball;
        
        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                GameObject go = Instantiate<GameObject>(ball);
                go.SetActive(true);
                var rigidbody = go.GetComponent<Rigidbody>();
                rigidbody.AddForce((Vector3.up + Vector3.forward) * 10.0f, ForceMode.Impulse);
            }
        }
    }
}