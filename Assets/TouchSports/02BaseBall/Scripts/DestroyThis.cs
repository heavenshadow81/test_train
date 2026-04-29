using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyThis : MonoBehaviour {

    private void Awake()
    {
        StartCoroutine(DestroySelf(1));
    }
    IEnumerator DestroySelf(float sec)
    {
        yield return new WaitForSeconds(sec);
        Destroy(this.gameObject);
    }
}
