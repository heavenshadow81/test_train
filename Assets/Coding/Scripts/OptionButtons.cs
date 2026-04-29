using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//컨텐츠 난이도, 사람 숫자 조절 : 옵션 변경 버튼에서 호출할 함수 모음
public class OptionButtons : MonoBehaviour
{
    [SerializeField]
    bool one;
    private void OnEnable()
    {
        if (one)
        {
            ContentsOptions.SetDifficult(0);
        }
    }
    //플레이어 숫자 조절
    public void SetPlayer(UnityEngine.UI.Dropdown dropdown)
    {
        ContentsOptions.SetPlayer(dropdown.value);
    }
    //난이도 조절
    public void SetDifficult(UnityEngine.UI.Dropdown dropdown)
    {
        ContentsOptions.SetDifficult(dropdown.value);
    }
}
