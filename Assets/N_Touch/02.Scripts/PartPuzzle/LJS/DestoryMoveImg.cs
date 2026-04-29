using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryMoveImg : MonoBehaviour {
    public void DestoryImgs()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
