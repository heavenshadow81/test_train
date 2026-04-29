using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalArrowAction : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Image image;
    [SerializeField]
    Vector3 defaultPos;
    void FixedUpdate()
    {
        image.rectTransform.localPosition = defaultPos * Mathf.PingPong(Time.time * 3, 2);
    }
    
}
