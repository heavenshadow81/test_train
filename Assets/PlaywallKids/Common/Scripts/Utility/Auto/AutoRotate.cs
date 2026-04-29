using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public enum RotateType { Right, Left }

	public float initialAngle = 0.0f;
	
	public Vector3 axis = Vector3.up;
	
	public bool isLocal = false;
	
	private Transform _t;

    private float _anglePerSecond = 90.0f;
    public float anglePerSecond
    {
        get { return _anglePerSecond; }
        set
        {
            _anglePerSecond = value;
        }
    }

    private RotateType _rotateType = RotateType.Right;
    public RotateType rotateType
    {
        get { return _rotateType; }
        set {
            _rotateType = value;
        }
    }
	
	// Use this for initialization
	void Start () {
		_t = this.transform;
		Quaternion q = Quaternion.AngleAxis(initialAngle, axis);
		if(isLocal) {
			_t.localRotation = q;
		}
		else {
			_t.rotation = q;
		}
	}
	
	// Update is called once per frame
	void Update () {
        float angle = anglePerSecond;
        switch( rotateType)
        {
            case RotateType.Left:
                angle = -anglePerSecond;
                break;
            case RotateType.Right:
                angle = anglePerSecond;
                break;
                
        }

        if (isLocal)
        {
            _t.Rotate(axis, Time.deltaTime * angle);
        }
        else
        {
            _t.Rotate(_t.TransformDirection(axis), Time.deltaTime * angle);
        }
	}
}
