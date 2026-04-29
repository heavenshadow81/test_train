using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletController : MonoBehaviour {
    public int SelectedTheme;
    public int[] SelectedTravel;
    public GameObject AnyTapArear;
    int TravelIndex;

    public enum TravelState
    {
        Intro,
        Theme,
        Travel,
        Fly,
        Play,
        End
    }
    public TabletUIEvent ui;
    public TravelState _state;

    private void Awake()
    {        
        ResetInit();
    }
    public void ResetInit()
    {
        SelectedTheme = -1;
        SelectedTravel = new int[5];
        for (int i = 0; i < SelectedTravel.Length; i++)
            SelectedTravel[i] = -1;
        TravelIndex = 0;
        _state = TravelState.Intro;
        AnyTapArear.SetActive(true);
    }


    //대기화면 -> 테마
    public void AnyTap()
    {
        //대기화면이고, 사용자가 입장해있을때 배경을 클릭한다면
        if (_state == TravelState.Intro && Client.client._isPlayerIn)
        {
            
            AnyTapArear.SetActive(false);
            _state = TravelState.Theme;
            ui.TitleToTheme();
        }
    }

    //테마 선택
    public void SelectTheme(int ThemeIdx)
    {
        if (_state == TravelState.Theme)
        {
            _state = TravelState.Travel;
            SelectedTheme = ThemeIdx;
            string _theme = "Theme/" + SelectedTheme+"/";
            Client.client.SendMessage_Sever(_theme);
            ui.SelectTheme(SelectedTheme);
            ui.DisappearTheme();
        }
    }

    public bool OverlapCheck(int _TravelIndex)
    {
        for (int i = 0; i < SelectedTravel.Length; i++)
        {
            if (_TravelIndex == SelectedTravel[i])
            {
                ui.CallError("이미 선택된 여행지 입니다.");
                return false;
            }
        }
        return true;
    }

    //여행지 선택
    public void SelectTravel(int _TravelIndex)
    {
        if (TravelIndex >= SelectedTravel.Length)
        {
            ui.CallError("여행지 5개가 모두 선택되었습니다.");
            return;
        }
        SelectedTravel[TravelIndex] = _TravelIndex;
        string _travel = "Travel/" + _TravelIndex+"/";
        Client.client.SendMessage_Sever(_travel);
        TravelIndex++;
    }

    public int CountTravel()
    {
        int cnt = 0;
        for (int i = 0; i < SelectedTravel.Length; i++)
        {
            if (SelectedTravel[i] != -1)
                cnt++;
        }
        return cnt;
    }

    //여행지 캔슬하는경우(안쓰이는중)
    public void CancleSelectTravel(int _TravelIndex)
    {
        for (int i = 0; i < SelectedTravel.Length; i++)
        {
            //취소 요청이 들어온 번호의 위치를 찾음.
            if (SelectedTravel[i] == _TravelIndex)
            {
                //i번째 인덱스를 초기화 -1
                SelectedTravel[i] = -1;

                //순서를 당기기위한 임시저장공간
                int[] tmp = new int[4 - i];
                for (int j = 0; j < tmp.Length; j++)
                    tmp[j] = -1;
                int t = 0;

                //i뒤로 -1이 아닌 넘버(스케줄)이 있다면 임시공간tmp에 저장.
                for (int j = i; j < SelectedTravel.Length; j++)
                {
                    if (SelectedTravel[j] != -1)
                    {
                        tmp[t] = SelectedTravel[j];
                        SelectedTravel[j] = -1;
                        t++;
                    }
                }

                t = 0;
                //i 위치부터 남은 공간동안 뒤쪽 스케쥴을 붙여쓰기.
                for (int k = i; k < SelectedTravel.Length; k++)
                {
                    if (t < tmp.Length)
                    {
                        SelectedTravel[k] = tmp[t];
                        t++;
                    }
                }
            }
        }
        TravelIndex--;
        if (TravelIndex <= -1)
            TravelIndex = 0;

        for (int i = 0; i < SelectedTravel.Length; i++)
        {
            Debug.Log(i + "번째 스케줄 :  " + SelectedTravel[i]);
        }
    }

    //여행지 -> 테마로 돌아가는경우.
    public void CancleSelectTravelAll()
    {
        Client.client.SendMessage_Sever("Return/Theme/");
        for (int i = 0; i < SelectedTravel.Length; i++)
            SelectedTravel[i] = -1;
        TravelIndex = 0;
    }

    //여행지가 1개 이상 선택되었는지 확인.
    public bool TravelSchduleCheck()
    {
        int check = 0;
        for (int i = 0; i < SelectedTravel.Length; i++)
        {
            if (SelectedTravel[i] != -1)
                check++;
        }
        if (check == 0)
            return false;

        return true;
    }

    //여행지가 1개 이상 선택되었을 때 Next버튼을 누르면 호출. 상태전환
    public void CompleteTravelSchdule()
    {
        if (_state == TravelState.Travel)
        {
            _state = TravelState.Play;
            Client.client.SendMessage_Sever("Next/Fly/");
        }
    }


}

