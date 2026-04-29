using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyPower : MonoBehaviour {
    public Transform Mytr;
    public float speed;
    private void Awake()
    {
        Mytr = this.transform;
    }

    // Update is called once per frame
    void Update () {
        Mytr.Rotate(Vector3.forward * Time.deltaTime * speed);
	}
}
