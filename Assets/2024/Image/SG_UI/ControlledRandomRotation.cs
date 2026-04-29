using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlledRandomAxisRotation : MonoBehaviour
{
   
    public GameObject targetObject; // 회전할 오브젝트를 인스펙터에서 지정

   
    public float rotationSpeed = 10f; // 회전 속도를 인스펙터에서 설정

  
    public float axisChangeInterval = 2f; // 회전 축을 변경할 시간 간격

    private Vector3 rotationDirection;
    private float timeSinceLastChange;

    void Start()
    {
        SetRandomRotationAxis(); // 처음 회전 축을 랜덤하게 설정
    }

    void Update()
    {
        // 시간 간격이 지났을 때 회전 축 변경
        timeSinceLastChange += Time.deltaTime;
        if (timeSinceLastChange >= axisChangeInterval)
        {
            SetRandomRotationAxis();
            timeSinceLastChange = 0f;
        }

        // 지정된 오브젝트가 있는 경우에만 회전 적용
        if (targetObject != null)
        {
            targetObject.transform.Rotate(rotationDirection * rotationSpeed * Time.deltaTime);
        }
        else
        {
            Debug.LogWarning("회전할 오브젝트가 지정되지 않았습니다.");
        }
    }

    void SetRandomRotationAxis()
    {
        // X, Y, Z 축 중 하나를 랜덤하게 선택하여 단일 축 회전 설정
        int axis = Random.Range(0, 3); // 0 = X, 1 = Y, 2 = Z

        if (axis == 0)
            rotationDirection = Vector3.right; // X축 회전
        else if (axis == 1)
            rotationDirection = Vector3.up;    // Y축 회전
        else
            rotationDirection = Vector3.forward; // Z축 회전
    }
}
