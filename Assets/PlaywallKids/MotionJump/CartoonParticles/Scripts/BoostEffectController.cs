using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoostEffectController : MonoBehaviour {

    public GameObject flowerEffectPrefab;

    CObjectList<GameObject> effectsList;
    void Awake()
    {
        effectsList = new CObjectList<GameObject>(5,
            () =>
            {
                return null;
            },
            (GameObject go) =>
            {
                return true;
            }
            );
    }
    
    
    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    IEnumerator ExplosionProcess()
    {
            
        yield return new WaitForEndOfFrame();

    }

    
}
