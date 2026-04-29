using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

[Serializable]
public class PageArray
{
    public GameObject[] pages = null;
}

public class UIManager : MonoBehaviour
{
    public GameObject[] menu;
    public GameObject select;
    public GameObject[] selectPosition;
    public Button[] buttons;
    public GameObject barMask;

    public GameObject black;

    public PageArray[] PageArrays = null;
    public UIManager_PlayGround subManager = null;
    public static int prevMenuNumber; //테마 번호
    public Button[] topButtons = null;

    // Start is called before the first frame update
    void Start()
    {
        select.transform.position = selectPosition[0].transform.position;
        black.SetActive(false);
        subManager.SetBaseBoardsList(PageArrays[prevMenuNumber]);

        MenuOpen(prevMenuNumber);
    }

    public void MenuOpen(int menuNumber)
    {
        //메뉴 전체 비활성화
        for(int i = 0; i < menu.Length; i++)
        {
            menu[i].transform.DOScale(0f, 0f);
        }

        //테마 바꿀 때 페이지 넘버 0으로 초기화
        if (menuNumber != prevMenuNumber)
        {
            UIManager_PlayGround.pageNum = 0; 
        }

        SetTopButtonsEnable(false);
        prevMenuNumber = menuNumber;

        InitPages(menuNumber);
        //menuNumber = 3;  <<진우님 여기 번호 바꾸면 메뉴 변경됨
        menu[menuNumber].transform.DOScale(1, 0.3f).OnComplete(() => SetTopButtonsEnable(true));
        select.transform.DOMove(selectPosition[menuNumber].transform.position, 0.3f);
        SoundMGR.Instance.SoundPlay("0.설정_버튼");

        subManager.SetBaseBoardsList(PageArrays[menuNumber]);
    }

    //메뉴바 때문에 만들어놨던 이제 안씀
    //public void MenuRight() 
    //{
    //    buttons[0].interactable = true;
    //    buttons[1].interactable = false;

    //    barMask.GetComponent<RectMask2D>().padding = new Vector4(800,0,0,0);
    //    barMask.transform.position = selectPosition[8].transform.position;

    //    for (int i = 0; i < menu.Length; i++)
    //    {
    //        menu[i].transform.DOScale(0f, 0.3f);
    //    }

    //    menu[4].transform.DOScale(1, 0.3f);
    //    select.transform.DOMove(selectPosition[0].transform.position, 0.3f);
    //    SoundMGR.Instance.SoundPlay("0.설정_버튼");
    //}

    //public void MenuLeft()
    //{
    //    buttons[0].interactable = false;
    //    buttons[1].interactable = true;

    //    barMask.GetComponent<RectMask2D>().padding = new Vector4(0, 0, 800, 0);
    //    barMask.transform.position = menuPosition;

    //    for (int i = 0; i < menu.Length; i++)
    //    {
    //        menu[i].transform.DOScale(0f, 0.3f);
    //    }

    //    menu[0].transform.DOScale(1, 0.3f);
    //    select.transform.DOMove(selectPosition[0].transform.position, 0.3f);
    //    SoundMGR.Instance.SoundPlay("0.설정_버튼");
    //}

    public void EndGame()
    {
        Application.Quit();
    }

    public void SceneChange(string sceneName)
    {
        SoundMGR.Instance.SoundPlay("0.타이틀입장");
        SoundMGR.Instance.bgmSource.Stop();
        black.SetActive(true);
        black.GetComponent<Image>().DOFade(1, 0.5f).OnComplete(() => SceneManager.LoadSceneAsync(sceneName));        
    }

    private void SetTopButtonsEnable(bool enable)
    {
        foreach (Button button in topButtons)
        {
            button.enabled = enable;
        }
    }

    private void InitPages(int pageIdx)
    {
        //페이지 넘버가 페이지 수보다 커도 0으로 초기화
        if (UIManager_PlayGround.pageNum >= PageArrays[pageIdx].pages.Length)
        {
            UIManager_PlayGround.pageNum = 0;
        }

        if (PageArrays[pageIdx].pages.Length > 0)
        {
            for (int i = 0; i < PageArrays[pageIdx].pages.Length; i++)
            {
                PageArrays[pageIdx].pages[i].GetComponent<CanvasGroup>().alpha = 1;
                PageArrays[pageIdx].pages[i].transform.localPosition = Vector3.zero;
                PageArrays[pageIdx].pages[i].SetActive(false);
            }

            PageArrays[pageIdx].pages[UIManager_PlayGround.pageNum].SetActive(true);
        }
    }
}
