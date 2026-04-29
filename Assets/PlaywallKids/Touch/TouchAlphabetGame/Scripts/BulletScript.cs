using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {
    public float speed;
    public bool destroy;
    public Transform cachedTransform;
	public bool usePosition;
    float distanceScope;
    
    [HideInInspector]
    public Vector3 dir;

    void Awake()
    {
        cachedTransform = this.transform;
        if (speed == 0) speed = 5f;
    }

    void OnEnable()
    {
        distanceScope = 700f;
    }

    void Update()
    {
		if(usePosition)
        cachedTransform.position += dir * speed * Time.deltaTime;
		else
			cachedTransform.localPosition += dir * speed * Time.deltaTime;
       // cachedTransform.localRotation = Quaternion.Euler(dir);
        distanceScope -= speed * Time.deltaTime;
        if (distanceScope <= 0)
        {
            if (destroy)
                Destroy(this.gameObject);
            else
                this.gameObject.SetActive(false);
        }
    }

}
