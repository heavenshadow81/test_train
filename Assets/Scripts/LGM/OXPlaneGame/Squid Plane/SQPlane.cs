using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SQPlane : MonoBehaviour
{
    public Vector2Int id;
    public bool isClick = false;
    protected SQManager manager;

    private void Awake()
    {
        manager = SQManager.Instance;
    }
    protected virtual void OnMouseDown()
    {
        manager.currentID = id;
    }
    private void OnDisable()
    {
        isClick = false;
    }
}
