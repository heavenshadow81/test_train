using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//메뉴 되돌리기
public class Turnback : MonoBehaviour
{
    #region 변수
    //
    [SerializeField]
    GameObject[] buttonGroup;
    [SerializeField]
    UnityEngine.UI.Button backbutton;
    #endregion
    #region 버튼에서 쓰일 함수
    public void Revert()
    {
        for(int i =0; i<buttonGroup.Length; i++)
        {
            if (buttonGroup[i].activeSelf)
            {
                if (i > 0)
                {
                    buttonGroup[i - 1].SetActive(true);
                    buttonGroup[i].SetActive(false);
                    if (i - 1 == 0)
                    {
                        backbutton.gameObject.SetActive(false);
                    }
                    return;
                }
            }
        }
    }
    #endregion
}
