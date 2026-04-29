using UnityEngine;
using System.Collections.Generic;

public class PointInput : MonoBehaviour
{
    public List<Vector3> input = new List<Vector3>();

    public void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() &&
                UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == null)
            {
                if (input.Count == 0 || (input[input.Count - 1] - pos).sqrMagnitude >= 0.01f)
                {
                    input.Add(pos);
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            input.Clear();
        }
    }
}