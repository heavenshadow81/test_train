using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateTest : MonoBehaviour {
    public int Cnt;
    public GameObject prefab;
	// Use this for initialization
	void Start () {
        for (int i = 0; i < Cnt; i++)
        {
            Instantiate(prefab, new Vector3(i, 0, 0), Quaternion.identity);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
