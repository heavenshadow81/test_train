using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTest : MonoBehaviour {
	public RectTransform rt;
	public Vector3 rtpos;
	public Vector3 mospos;

	void Update()
	{
		Debug.Log ("hit1");
		Vector2 pos = Input.mousePosition;
		Vector3 theTouch = new Vector3(pos.x, pos.y, 0.0f);  
		mospos = theTouch;
		rtpos = rt.anchoredPosition;
		Ray ray2 = Camera.main.ScreenPointToRay(rtpos);

		RaycastHit2D hit2d = Physics2D.GetRayIntersection (ray2, Mathf.Infinity);
		if (hit2d.collider != null)
			Debug.Log ("2d hit");
			
	}		

}
