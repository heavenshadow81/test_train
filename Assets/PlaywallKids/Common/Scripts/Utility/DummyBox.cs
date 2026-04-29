using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class DummyBox : MonoBehaviour {
#if UNITY_EDITOR
    public Color color = Color.red;

    private static GUIStyle _labelStyle;
    public static GUIStyle labelStyle
    {
        get
        {
            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle();
                _labelStyle.fontSize = 14;
                _labelStyle.fontStyle = FontStyle.Bold;
            }
            return _labelStyle;
        }
    }

    public BoxCollider boxCollider;

    public void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void OnDrawGizmos()
    {
        Handles.color = color;
        Handles.Label(transform.position + Vector3.up, name, labelStyle);
        if(boxCollider != null) {
            Gizmos.color = color;
            Gizmos.DrawCube(transform.position + boxCollider.center, boxCollider.size);
        }
    }
#endif
}
