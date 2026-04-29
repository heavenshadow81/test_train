using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObj : MonoBehaviour {
    Transform Mytr;
    public Transform Target;

    void Start()
    {
        Mytr = this.transform;
    }

	
	void Update () {
        Mytr.LookAt(Target);
	}
}
