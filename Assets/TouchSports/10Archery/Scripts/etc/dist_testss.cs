using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dist_testss : MonoBehaviour {
    public Transform mytr;
    public Transform Targetr;
    public float dist;
	void Update () {
        dist = Vector3.Distance(mytr.position, Targetr.position);
	}
}
