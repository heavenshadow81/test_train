using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FinalTriggers : MonoBehaviour
{
    //플레이어1인가?
    [SerializeField]
    bool player1;

    #region 유니티 함수
    private void OnEnable()
    {
        Coding.ContentsController.Instance.Final += SetActor;
        Coding.ContentsController.Instance.Final += Disa;
    }
    private void OnDisable()
    {
        Coding.ContentsController.Instance.Final -= SetActor;
        Coding.ContentsController.Instance.Final -= Disa;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            Coding.ContentsController.Instance.player1 = LayerMask.LayerToName(other.gameObject.layer) == "Note";
            Coding.ContentsController.Instance.Finalization();
        }
    }
    #endregion
    #region 함수
    //특정 캐릭터 행동 거대화!!!
    void SetActor()
    {
        int index = Coding.ContentsController.Instance.player1 ? 0 : 1;
        //기본 UI 비활성화(화살표 선택칸)
        Coding.UIController.Instance.gamesceneUI.gameObject.SetActive(false);
        
        for(int i = 0; i< Coding.UIController.Instance.characterView.Length; i++)
        {
            //승리 UI 활성
            //print(index);
            Coding.UIController.Instance.characterVictroy[i].gameObject.SetActive(i == index);
            //UI비활성화!
            Coding.UIController.Instance.characterView[i].gameObject.SetActive(false);
            Coding.UIController.Instance.uibackground[i].gameObject.SetActive(false);
        }
        print("플레이어 종료");
    }
    //오브젝트 비활성화
    void Disa()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
