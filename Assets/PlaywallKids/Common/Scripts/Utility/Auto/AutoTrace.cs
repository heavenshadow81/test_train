using UnityEngine;
using System.Collections;

public class AutoTrace : MonoBehaviour {
	public GameObject target;
	public Vector3 offset = new Vector3(0, 0, 0);
	
	private Transform _targetTransform;
	private Transform _transform;
	
	public bool fixedUpdate = false;
	
	// Use this for initialization
	void Start () {
		_transform = this.transform;
		if(target) {
			_targetTransform = target.transform;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(!fixedUpdate)
			_PerformUpdate();
	}
	
	void FixedUpdate() {
		if(fixedUpdate)
			_PerformUpdate();
	}
	
	void _PerformUpdate() {
		if(target != null) {
			Vector3 position = _targetTransform.position;
			_transform.position = position + offset;
		}
	}
}
