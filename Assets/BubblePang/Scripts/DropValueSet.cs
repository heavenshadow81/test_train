using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropValueSet : MonoBehaviour
{
    [SerializeField]
    Dropdown typeDrop;

    [SerializeField]
    Dropdown personDrop;

    [SerializeField]
    Dropdown difficultDrop;

    // 옵션 드롭다운 값 변경 메소드
    public void SetDropdown(int type, int person, int difficult)
    {
        // 타입
        typeDrop.value = type;

        // 인원수
        personDrop.value = person;

        // 난이도
        difficultDrop.value = difficult;
    }
}
