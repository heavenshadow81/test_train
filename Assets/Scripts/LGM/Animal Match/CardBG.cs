using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBG : MonoBehaviour
{
    [HideInInspector]
    public Transform target;
    void Update()
    {
        if (target == null)
            return;
        transform.position = target.position;
    }

    public void AtiveTrue(Transform _target)
    {
        // 만약 카드 배경이 비활성화면 활성화
        if (!gameObject.activeSelf)
        {
            // 활성화 시 자기짝(카드 A or B) 위치로 이동
            transform.position = _target.position;
            gameObject.SetActive(true);
        }
        target = _target;    // 카드 선택 배경 이동
    }
    // 비활성화
    public void ActiveFalse()
    {
        target = null;
        gameObject.SetActive(false);
    }
}
