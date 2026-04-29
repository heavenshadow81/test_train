using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescaleInfo : MonoBehaviour {

	
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (gameObject.transform.localScale.y <= transform.localScale.x*1.4f)
		{
			gameObject.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y*1.4f, transform.localScale.z);
		}
	}
}
