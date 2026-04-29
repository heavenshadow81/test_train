using System.Collections;
using System.Collections.Generic;

using UnityEngine;
//재시작 여부
public class TouchRetry : MonoBehaviour
{
    [SerializeField]
    GameObject[] resetdeactivates;
    [SerializeField]
    GameObject[] resetactivates;
    [SerializeField]
    BGAnimalManager bganimalManager;
    [SerializeField]
    DestoryShadowAnimalMoveImg shadowanimal;
    [SerializeField]
    DestoryShadowMoveImg shadowanimal2;
    /* NFC 관련 주석
    [SerializeField]
    I8nputPlayerName_NFC inputPlayerName_NFC;
    */
    [SerializeField]
    GameObject[] startactivates;
    [SerializeField]
    GameObject[] startdeactivates;

    #region 20221118 추가
    [SerializeField]
    GameObject LevelSettingBtn;
    private void Awake()
    {
        LevelSettingBtn = GameObject.Find("LevelSettingBtn");
    }
    #endregion
    //다시 시작하기!!!
    public void Reset()
    {
        //오브젝트 비활성화
        for(int i = 0; i<resetdeactivates.Length; i++)
        {
            resetdeactivates[i].SetActive(false);
        }
        //오브젝트 활성화
        for(int i = 0; i<resetactivates.Length; i++)
        {
            resetactivates[i].SetActive(true);
        }
        //백그라운드에 남은 동물 제거
        bganimalManager?.DuplicationCheckdicReset();
        //백그라운드에 남은 동물 제거
        shadowanimal?.DestoryImgs();

        //마찬가지
        shadowanimal2?.DestoryImgs();
        //입력된 플레이어 이름 초기화!!
        /* NFC 관련 주석
        inputPlayerName_NFC.Init();
        */
        //20221118 추가
        LevelSettingBtn.GetComponent<UnityEngine.UI.Image>().raycastTarget = true;
    }
    //시작버튼 누를 때 시작할 것
    public void TouchContentStart()
    {
        //시작시 활성화 될 오브젝트
        for(int i = 0; i<startactivates.Length; i++)
        {
            startactivates[i].SetActive(true);
        }
        //시작시 비활성화 될 오브젝트
        for(int i = 0; i < startdeactivates.Length; i++)
        {
            startdeactivates[i].SetActive(false);
        }
        //터치 콘텐츠 시작!!!
        TouchContentsManger.Instance.Init();
    }
}
