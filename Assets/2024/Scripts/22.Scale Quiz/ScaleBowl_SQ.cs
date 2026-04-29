using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleBowl_SQ : MonoBehaviour
{
    private RectTransform bowlTransform;
    private RectTransform barTransform;

    private void Awake()
    {
        bowlTransform = GetComponent<RectTransform>();
        barTransform = GetComponentInParent<RectTransform>();
    }

    private void LateUpdate()
    {
        Quaternion targetRotation = Quaternion.Euler(0, 0, -barTransform.eulerAngles.z);
        bowlTransform.rotation = Quaternion.Lerp(bowlTransform.rotation, targetRotation, Time.deltaTime * 10f); 
    }

}
