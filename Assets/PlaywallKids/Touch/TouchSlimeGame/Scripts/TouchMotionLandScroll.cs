using UnityEngine;
using System.Collections;

public class TouchMotionLandScroll : MonoBehaviour {
    public float fSpeed;
    public GameObject land;
    public Terrain terrian;
    
    private float fHeight;

    private Transform _cachedTransform;
    public Transform cachedTransform
    {
        get
        {
            if(_cachedTransform == null)
            {
                _cachedTransform = this.transform;
            }
            return _cachedTransform;
        }
    }

    void Awake()
    {
        if(fSpeed == 0)
        {
            fSpeed = 10f;
        }

        TerrainData data = terrian.terrainData;
        Debug.Log(data.size);
        
    }

    void FixedUpdate()
    {
        cachedTransform.Translate(Vector3.back * Time.fixedDeltaTime * fSpeed , Space.World);
        Debug.Log(cachedTransform.position.z);
    //    if(cachedTransform.position.z <= -1*fHeight)
        {
   //         cachedTransform.position = new Vector3(0, cachedTransform.position.y, 0);
        }
    }
}
