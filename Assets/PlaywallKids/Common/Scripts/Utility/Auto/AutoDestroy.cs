using UnityEngine;
using System.Collections;

public class AutoDestroy : MonoBehaviour {
	public float time = 1.0f;
	private float _elapsedTime = 0.0f;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		_elapsedTime += Time.deltaTime;
		if(_elapsedTime >= time) {
			Destroy(this.gameObject);
		}
	}
}
