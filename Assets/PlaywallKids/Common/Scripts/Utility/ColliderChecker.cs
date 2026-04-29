using UnityEngine;
using System.Collections;

public class ColliderChecker : MonoBehaviour {
    public EventDelegate callFunc;

    public delegate void Func3D(Collider _col);
    public Func3D callback3D;

    public delegate void Func2D(Collider2D _col);
    public Func2D callback2D;

    void OnCollisionEnter(Collision _other)
    {
        if (callFunc != null)
        {
            if (callFunc.parameters.Length > 0)
                callFunc.parameters[0] = new EventDelegate.Parameter(_other.collider);
            callFunc.Execute();
        }
        else if (callback3D != null)
        {
            callback3D(_other.collider);
        }
    }

    void OnTriggerEnter(Collider _other)
    {
        if (callFunc != null)
        {
            if (callFunc.parameters.Length > 0)
                callFunc.parameters[0] = new EventDelegate.Parameter(_other);
            callFunc.Execute();
        }
        else if (callback3D != null)
        {
            callback3D(_other);
        }
    }

    void OnCollisionEnter2D(Collision2D _other)
    {
        if (callFunc!= null)
        {
            if (callFunc.parameters.Length > 0)
                callFunc.parameters[0] = new EventDelegate.Parameter(_other);
            callFunc.Execute();
        } if (callback2D != null)
        {
            callback2D(_other.collider);
        }
    }

    void OnTriggerEnter2D(Collider2D _other)
    {
        if (callFunc != null)
        {
            if (callFunc.parameters.Length > 0)
                callFunc.parameters[0] = new EventDelegate.Parameter(_other);
            callFunc.Execute();
        } if (callback2D != null)
        {
            callback2D(_other);
        }
    }
}
