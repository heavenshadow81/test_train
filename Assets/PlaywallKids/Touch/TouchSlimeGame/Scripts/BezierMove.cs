using UnityEngine;
using System.Collections;

public class BezierMove : MonoBehaviour
{


    #region Public Value
    public enum EType { DESTROY, DISABLE, REWIND, STOP, TEST, }
    public Vector3 wayPoint0;
    public Vector3 wayPoint1;
    public Vector3 _target;
    public EType disableType;
    public bool usePosition;
    [Range(0.01f, 10f)]
    public float fSpeed;
    #endregion

    Vector3 originPos;

    #region Property
    public bool IsArrived
    {
        get
        {
            if (this == null) return true;
            return fTime >= 1f && (disableType == EType.DISABLE || disableType == EType.STOP);
        }
    }

    Transform cachedTransform
    {
        get
        {
            if(_transform == null)
            { _transform = this.transform; }
            return _transform;
        }
    }
    
    public Vector3 origin
    {
        set
        {
            originPos = value;
            if (usePosition)
            {   cachedTransform.position = value; }
            else
            {   cachedTransform.localPosition = value;  }
        }

		get{
			if(usePosition) return cachedTransform.position;
			else return cachedTransform.localPosition;
		}
    }

    public Vector3 target
    {
        set
        {
            fTime = 0;
            _target = value;
            bezier = new Bezier(origin, wayPoint0, wayPoint1, _target);
        }
        get
        {   return _target;  }
    }
    #endregion

    Transform _transform;
    Bezier bezier;
    float fTime;

    void Awake()
    {
        fTime = 0;
        if (fSpeed == 0) fSpeed = 0.5f;
    }
    void OnEnable()
    {
        fTime = 0;
    }

    void FixedUpdate()
    {
        if (bezier == null) return;

        fTime += Time.fixedDeltaTime * fSpeed;
        if (usePosition)
            cachedTransform.position = bezier.GetPointAtTime(fTime);
        else
            cachedTransform.localPosition = bezier.GetPointAtTime(fTime);

        if (fTime > 1f)
        {
            switch(disableType)
            {
                case EType.DESTROY: 
                    if (gameObject != null)
                    Destroy(this.gameObject);break;
                case EType.DISABLE:
                    fTime = 1f;
                    this.gameObject.SetActive(false);
                    break;
                case EType.TEST:
                    fTime = 0;
                    bezier = new Bezier(Vector3.zero, wayPoint0, wayPoint1, _target);
                    break;
                case EType.STOP:
                    if (usePosition) { cachedTransform.position = target; }
                    else { cachedTransform.localPosition = target; }
                    fTime = 1f;
                    bezier = null;
                    break;
                case EType.REWIND:
                    if(usePosition) {   cachedTransform.position = originPos;  }
                    else {  cachedTransform.localPosition = originPos; }
                    bezier = null;
                    break;

            }
        }
    }

    Vector3 GetPosition()
    {
        return usePosition ? cachedTransform.position : cachedTransform.localPosition;
    }
}
