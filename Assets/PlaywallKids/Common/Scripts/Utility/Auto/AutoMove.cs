using UnityEngine;
using System.Collections;

public class AutoMove : MonoBehaviour {
	/// <summary>
	/// If translate is true, it will use Translate(). Otherwise, it will move position directly.
	/// </summary>
	public bool translate = false;
	
	/// <summary>
	/// If fixedMove is true, it will runs on FixedUpdate(). Otherwise, this will runs on Update().
	/// </summary>
	public bool fixedMove = false;
	
	/// <summary>
	/// The move axis. It will automatically normalize at first update.
	/// </summary>
	public Vector3 axis = new Vector3(0, 0, 1.0f);
	
	/// <summary>
	/// The start time. Default is 0.
	/// </summary>
	public float startTime = 0.0f;
	
	/// <summary>
	/// Moving length in one second. Default is 1.
	/// </summary>
	public float length = 1.0f;
	
	/// <summary>
	/// The moving duration. Default is one second.
	/// </summary>
	public float duration = 1.0f;
	
	float _currentTime = 0.0f;
	
	Transform _t;
	
	// Use this for initialization
	void Start () {
		axis = Vector3.Normalize(axis);
		_t = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(fixedMove) return;
		float deltaTime = Time.deltaTime;
		
		if(_currentTime >= startTime) {
			if(_currentTime >= startTime + duration) {
				Destroy(this);
				return;
			}
			
			if(deltaTime + _currentTime > duration) {
				deltaTime = duration - _currentTime;
			}
			
			if(translate) {
				_t.Translate(deltaTime * length * axis);
			}
			else {
				_t.position += deltaTime * length * axis;
			}
		}
		
		_currentTime += Time.deltaTime;
	}
	
	void FixedUpdate() {
		if(!fixedMove) return;
		float fixedDeltaTime = Time.fixedDeltaTime;
		
		if(_currentTime >= startTime) {
			if(_currentTime >= startTime + duration) {
				Destroy(this);
				return;
			}
			
			if(fixedDeltaTime + _currentTime > duration) {
				fixedDeltaTime = duration - _currentTime;
			}
		
			if(translate) {
				_t.Translate(fixedDeltaTime * length * axis);
			}
			else {
				_t.position += fixedDeltaTime * length * axis;
			}
		}
		
		_currentTime += Time.fixedDeltaTime;
	}
}
