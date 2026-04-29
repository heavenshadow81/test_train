using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabletUIEvent : MonoBehaviour
{
    public GameObject ThemeObj;
    public Animation[] Themes;
    public Animation Title;
    public int Theme_Idx;

    public GameObject TravelObj;
    public GameObject[] TravelsObj;
    public GameObject[] ChecksObj;
    public bool[] Checks;

    public Animator TravelAnimator;
    public Image[] Travels;
    public Text[] TravelText;
    public Sprite[] culture;
    public Sprite[] art;
    public Sprite[] city;
    public Sprite[] tour;
    public Sprite[] nature;
    public Sprite[] history;
    public Sprite emptyimage;

    public TabletController control;
    public JsonHelper json;
    public TravelData[] data;

    public Text ErrorText;
    public GameObject ErrorWindow;


    public GameObject PlayGUI;
    bool _tabletControll;

    public GameObject NextPrevObj;
    
    void Awake()
    {
        _tabletControll = false;
        ResetInit();
    }
    void ResetInit()
    {
        Count_Travel = 0;
        Checks = new bool[9];
        for (int i = 0; i < Checks.Length; i++)
            Checks[i] = false;
        ResetTitle();
        ResetTheme();
        ResetTravel();
        DisAppearPlay();
    }
    
    #region Title
    public void ResetTitle()
    {
        Title.Play("AppearTitle");
    }
    public void TitleToTheme()
    {
        Title.Play("TitleFadeOut");
        StartCoroutine(TitleOffAndTheme());
    }
    IEnumerator TitleOffAndTheme()
    {
        yield return new WaitForSeconds(1.6f);
        AppearTheme();
    }
    #endregion
    #region Theme
    public void ResetTheme()
    {
        for (int i = 0; i < Themes.Length; i++)
            Themes[i].Play("DefaluteTheme");
        ThemeObj.SetActive(false);
    }
    public void AppearTheme()
    {
        ThemeObj.SetActive(true);
        for (int i = 0; i < Themes.Length; i++)
            Themes[i].Play("AppearTheme");
    }
    public void DisappearTheme()
    {
        for (int i = 0; i < Themes.Length; i++)
            Themes[i].Play("DisappearTheme");
    }
    public void SelectTheme(int idx)
    {
        Theme_Idx = idx;
        StartCoroutine(ThemeToTravel());
    }
    IEnumerator ThemeToTravel()
    {
        yield return new WaitForSeconds(1f);
        SetTravle();
    }
    #endregion
    #region Travel
    void ResetTravel()
    {
        for (int i = 0; i < Travels.Length; i++)
        {
            Travels[i].sprite = emptyimage;
     //       TravelsObj[i].SetActive(true);
            ChecksObj[i].SetActive(false);
        }
        TravelObj.SetActive(false);
        NextPrevObj.SetActive(false);
    }
    public void DisAppearTravel()
    {
        //TravelAnimator.SetTrigger("DisAppear");
        NextPrevObj.SetActive(false);
    }
    IEnumerator TravelToPlay()
    {
        ResetTravel();
        yield return new WaitForSeconds(1.6f);
        AppearPlay();
    }
    public void SetTravle()
    {
        NextPrevObj.SetActive(true);
        TravelObj.SetActive(true);
        string _name = "";
        switch (Theme_Idx)
        {
            case 0:
                _name = "Culture";
                for (int i = 0; i < Travels.Length; i++)
                {
                    Travels[i].sprite = culture[i];
                }
                break;
            case 1:
                _name = "Art";
                for (int i = 0; i < Travels.Length; i++)
                {
                    Travels[i].sprite = art[i];
                }
                break;
            case 2:
                _name = "City";
                for (int i = 0; i < Travels.Length; i++)
                {
                    Travels[i].sprite = city[i];
                }
                break;
            case 3:
                _name = "Tour";
                for (int i = 0; i < Travels.Length; i++)
                {
                    Travels[i].sprite = tour[i];
                }
                break;
            case 4:
                _name = "Nature";
                for (int i = 0; i < Travels.Length; i++)
                {
                    Travels[i].sprite = nature[i];
                }
                break;
            case 5:
                _name = "History";
                for (int i = 0; i < Travels.Length; i++)
                {
                    Travels[i].sprite = history[i];
                }
                break;
        }

        data = JsonHelper.Instance.Load(_name);
        for (int i = 0; i < data.Length; i++)
        {
            TravelText[i].text = string.Format("{0}\n({1})", data[i].Name_kr, data[i].CountryName_kr);
        }
        TravelAnimator.SetTrigger("Appear");     
    }
    public void SelectTravel(int selectIndex)
    {
        if (Checks[selectIndex])
        {
            CallError("이미 선택된 여행지 입니다.");
            return;
        }

        if (control.CountTravel() >= 5)
        {
            CallError("여행지 5개가 모두 선택되었습니다.");
            return;
        }

        Checks[selectIndex] = !Checks[selectIndex];

        if (Checks[selectIndex])
        {
            //선택
     //       TravelsObj[selectIndex].SetActive(false);
            ChecksObj[selectIndex].SetActive(true);
            if(control.OverlapCheck(selectIndex))
                control.SelectTravel(selectIndex);
        }
        else
        {
            //선택 취소
     //       TravelsObj[selectIndex].SetActive(true);
            ChecksObj[selectIndex].SetActive(false);
            control.CancleSelectTravel(selectIndex);
        }
    }
    #endregion

    #region Play
    public void AppearPlay()
    {
        PlayGUI.SetActive(true);
    }

    public void Ending()
    {
        DisAppearPlay();
    }

    void DisAppearPlay()
    {
        PlayGUI.SetActive(false);
    }
    public void CallError(string errorMessage)
    {
        ErrorWindow.SetActive(true);
        ErrorText.text = errorMessage;
    }
    #endregion


    #region Button
    public GameObject Exitwindw;
    public GameObject HomeWindow;
    public Sprite ConnectOn;
    public Sprite DisConnect;
    public Image Connecting;

    public void ExitWindowOn()
    {
        Exitwindw.SetActive(true);
    }
    public void ExitCancle()
    {
        Exitwindw.SetActive(false);
    }
    public void ExitProgram()
    {
        Application.Quit();
    }

    public void HomeWindowOn()
    {
        HomeWindow.SetActive(true);
    }
    public void ReturnHome()
    {
        Client.client.SendMessage_Sever("Return/Home/");
        ResetInit();
        HomeWindow.SetActive(false);
        control.ResetInit();
    }
    public void PrevButton()
    {
        if (control._state == TabletController.TravelState.Intro)
            return;
        else if (control._state == TabletController.TravelState.Theme)
        {
            //인트로로 돌아가기
            ReturnHome();
           /* Client.client.SendMessage_Sever("Return/Home/");
            control._state = TabletController.TravelState.Intro;
            DisappearTheme();
            ResetTitle();*/
        }
        else if (control._state == TabletController.TravelState.Travel)
        {
            ReturnHome();
            //테마선택으로 돌아가기
            /*Client.client.SendMessage_Sever("Return/Theme/");
            control._state = TabletController.TravelState.Theme;
            control.CancleSelectTravelAll();
            ResetTravel();
            StartCoroutine(TitleOffAndTheme());*/
        }
    }
    public void NextButton()
    {
        if (control._state == TabletController.TravelState.Travel)
        {
            if (control.TravelSchduleCheck())
            {
                DisAppearTravel();
                control.CompleteTravelSchdule();
                Client.client.SendMessage_Sever("Next/Fly/");
                //플레이로 이동
                StartCoroutine(TravelToPlay());
            }
            else
            {
                CallError("여행지가 선택되지 않았습니다.");                
            }
        }
    }
    public void TabletControl()
    {
        if (!_tabletControll)
        {
            _tabletControll = true;
            Connecting.sprite = ConnectOn;
            Client.client.SendMessage_Sever("TabletControll/");
        }
        else
        {
            _tabletControll = false;
            Connecting.sprite = DisConnect;
            Client.client.SendMessage_Sever("KinectControl/");
        }
    }
    bool _isPress;
    public void PressedButton(int vect)
    {
        if (Count_Travel < control.CountTravel())
            Count_Travel++;
        else
        {
            return;
        }
        //vect
        //0 : 좌로 회전
        //1 : 우로 회전
        //2 : 확대
        //3 : 축소

        if (!_isPress)
            StartCoroutine(PressButton(vect));
    }   
    IEnumerator PressButton(int _idx)
    {
        _isPress = true;
        string _controll = "";
        switch (_idx)
        {
            case 0:
                _controll = "Con/RotateL/";
                break;
            case 1:
                _controll = "Con/RotateR/";
                break;
            case 2:
                _controll = "Con/CloseUp/";
                break;
            case 3:
                _controll = "Con/CloseDown/";
                break;
        }

        while (_isPress)
        {
            Client.client.SendMessage_Sever(_controll);
            yield return new WaitForSeconds(0.1f);
        }        
    }
    public void PressOff()
    {
        _isPress = false;
    }
    public int Count_Travel;
    public void Capture()
    {
        if (Count_Travel < control.CountTravel())
            Count_Travel++;
        else
        {
            return;
        }
        Debug.Log("플레이어(PC)에게 캡쳐명령 전송");
        Client.client.SendMessage_Sever("Capture/true/");
    }

   
    #endregion
}
