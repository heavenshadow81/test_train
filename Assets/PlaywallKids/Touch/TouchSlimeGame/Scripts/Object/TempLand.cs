using UnityEngine;
using System.Collections;

public class TempLand : MonoBehaviour {
    
    public GameObject prefab;
    public Transform parent;
    public Vector3 origin;
    public float width;
    public float height;
    public float depth;
    public int low;
    public int column;
    public int z;

    void Awake()
    {
        for(int i = 0 ; i < low ; ++i)
        {
            for(int j = 0 ; j < column; ++j)
            {
                for(int k = 0 ; k < z ; ++k)
                {
                    GameObject go = Instantiate(prefab) as GameObject;
                    Transform _transform = go.transform;
                    if (parent != null)
                        _transform.parent = parent;

                    Vector3 _pos = origin;
                    _pos.x += width * i;
                    _pos.y += height * j;
                    _pos.z += depth * k;
                    _transform.localPosition = _pos;
                }
            }
        }
    }
	
}
