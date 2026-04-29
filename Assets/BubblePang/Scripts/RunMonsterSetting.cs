using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunMonsterSetting : MonoBehaviour
{

    private void Awake()
    {
        // 사라지는 동물은 터치 반응 없도록 설정
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<BoxCollider>().enabled = false;
        }
    }
}
