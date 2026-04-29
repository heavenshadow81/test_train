using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightCreate : MonoBehaviour {
    public GameObject LightObj;
    public Transform mytr;
	// Use this for initialization
	void Start () {
        mytr = this.transform;
        // for (int x = -60; x <= 60; x += 20)
        // {            
        //  }
        float x = -30;
        for (int z = -100; z <= 100; z += 10)
        {
            Transform lightTr = Instantiate(LightObj, new Vector3(x, 35, z), LightObj.transform.rotation).transform;
            lightTr.name = "[Light]" + x + "/" + z;
            lightTr.parent = mytr;
        }
        x = 0;
        for (int z = -100; z <= 100; z += 10)
        {
            Transform lightTr = Instantiate(LightObj, new Vector3(x, 35, z), LightObj.transform.rotation).transform;
            lightTr.name = "[Light]" + x + "/" + z;
            lightTr.parent = mytr;
        }
        x = 30;
        for (int z = -100; z <= 100; z += 10)
        {
            Transform lightTr = Instantiate(LightObj, new Vector3(x, 35, z), LightObj.transform.rotation).transform;
            lightTr.name = "[Light]" + x + "/" + z;
            lightTr.parent = mytr;
        }
    }
}
