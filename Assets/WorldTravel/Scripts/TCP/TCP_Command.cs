using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.MapoContents.WorldTravel;
public class TCP_Command : MonoBehaviour
{
    public static TCP_Command Instance;
    public UIEvent gui;
    public HandCursor Cursor;
    public KinectCamera kinectcam;
    public int Travel_idx;
    bool _commandOn;
    string _command;

    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (_commandOn)
        {
            _commandOn = false;
            string[] splitData = _command.Split('/');
            switch (splitData[0])
            {
                case "Theme":
                    int Themeidx = int.Parse(splitData[1]);
                    CommandTheme(Themeidx);
                    break;
                case "Travel":
                    int Travelidx = int.Parse(splitData[1]);
                    CommandTravel(Travelidx);
                    break;
                case "Next":
                    CommandNext();
                    break;
                case "Return":
                    if (splitData[1] == "Home")
                        CommandReturnHome();
                    else if (splitData[1] == "Theme")
                        CommandReturnTheme();
                    break;
                case "Con":
                    CommandControll(splitData[1]);
                    break;
                case "Capture":
                    CommandCapture();
                    break;
            }
        }
    }

    public void SetCommand(string command)
    {
        _command = command;
        _commandOn = true;
    }
    #region Cilent_controller

    #endregion
    #region Server
    public void CommandTheme(int _idx)
    {
        string theme = "";
        switch (_idx)
        {
            case 0:
                theme = "Culture";
                break;
            case 1:
                theme = "Art";
                break;
            case 2:
                theme = "City";
                break;
            case 3:
                theme = "Tour";
                break;
            case 4:
                theme = "Nature";
                break;
            case 5:
                theme = "History";
                break;
        }

        TravelManager.Instance.CompleteTheme(theme, _idx);
        gui.SetCountryData(theme);
        Cursor._selectTheme = true;
    }
    public void CommandTravel(int _idx)
    {
        if (TravelManager.Instance.OverlapCheck(TravelManager.Instance._countryIdx))
        {
            TravelManager.Instance.SelectCountry(_idx);          
        }
        /*
        TravelManager.Instance.SelectedCountry[Travel_idx] = _idx;
        gui.CheckCountry(_idx);
        TravelManager.Instance._countryIdx++;
        Travel_idx++;*/
    }
    public void CommandReturnTheme()
    {
        TravelManager.Instance.ReturnTheme();
    }
    public void CommandReturnHome()
    {
        if(TravelManager.Instance._state != TravelManager.TravelState.Intro)
            TravelManager.Instance.ReturnHome();
    }
    public void CommandNext()
    {
        TravelManager.Instance.CompleteCountry();
    }
    public void CommandManagerControl()
    {
        //컨트롤 권한을 매니저에게 넘김
    }
    public void CommandMasterControl()
    {
        //컨트롤을 마스터pc에서 하도록
    }
   public TravelTTS tts;
    public void CommandCapture()
    {
        //캡쳐

        tts.StopTTS();
        CaptureManager.Instance.PhotoTime_Event();
    }
    public void CommandControll(string command)
    {
        switch (command)
        {
            case "RotateL":
                kinectcam.CommandBOdyTurn(5f);
                Debug.Log("Command Control" + command);
                break;
            case "RotateR":
                kinectcam.CommandBOdyTurn(-5f);
                Debug.Log("Command Control" + command);
                break;
            case "CloseUp":
                kinectcam.CloseUpDown(true);
                Debug.Log("Command Control" + command);
                break;
            case "CloseDown":
                kinectcam.CloseUpDown(false);
                Debug.Log("Command Control" + command);
                break;
        }
    }
    #endregion

}