using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DummyGizmo : MonoBehaviour {
#if UNITY_EDITOR
	public Color color = Color.red;

	private static GUIStyle _labelStyle;
	public static GUIStyle labelStyle {
		get {
			if(_labelStyle == null) {
				_labelStyle = new GUIStyle();
				_labelStyle.fontSize = 14;
				_labelStyle.fontStyle = FontStyle.Bold;
			}
			return _labelStyle;
		}
	}

	public void OnDrawGizmos() {
		Handles.color = color;
		Handles.Label(transform.position + Vector3.up, name, labelStyle);
		
		Handles.ArrowHandleCap(-1, transform.position + Vector3.up, Quaternion.Euler(90.0f, 0.0f, 0.0f), 1.0f, EventType.TouchDown);
	} 
#endif
}
