using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ML.MapoContents.WorldTravel;

public class PinPoint : MonoBehaviour
{
    public Animation[] CulturePin;
    public Animation[] ArtPin;
    public Animation[] CityPin;
    public Animation[] TourPin;
    public Animation[] NaturePin;
    public Animation[] HistoryPin;
    public Animation InceonPin;

    public int _theme;
    public int[] _countryIdx;
    public int PlayIdx;
    public RectTransform MyPin;

    public RectTransform[] LinePoint;
    public GameObject[] LinePointObj;
    public Vector2[] LinePath;
    public UIEvent gui;
    public Text FlyTimer;
    public RectTransform timer;
    public GameObject timerobj;


    public Vector2[] CentPos;
    public RectTransform Cent, Mover;
    public int CentIdx;
    float BezierSpeed;

    void Awake()
    {
        timerobj.SetActive(false);
        CentPos = new Vector2[11];
        CentPos[0] = new Vector2(-91.6f, 72.2f);
        CentPos[1] = new Vector2(-255, 100);
        CentPos[2] = new Vector2(-270, 165);
        CentPos[3] = new Vector2(-320, 215);
        CentPos[4] = new Vector2(-320, 215);
        CentPos[5] = new Vector2(-285, 230);
        CentPos[6] = new Vector2(-320, 215);
        CentPos[7] = new Vector2(-320, 240);
        CentPos[8] = new Vector2(100, 240);
        CentPos[9] = new Vector2(100, 240);
        CentPos[10] = new Vector2(100, 240);
        BezierSpeed = 0.1f;

        LinePath = new Vector2[LinePoint.Length];
        _theme = -1;
        _countryIdx = new int[5];
        for (int i = 0; i < _countryIdx.Length; i++)
            _countryIdx[i] = -1;
        ResetPin();
        ResetPoint();
    }
    public void SetCent(int _idx)
    {
        CentIdx = _idx;
        Cent.anchoredPosition = CentPos[_idx];

    }

    Vector2 BezierCurve(float t, Vector2 p0, Vector2 p1)
    {
        return ((1 - t) * p0) + ((t) * p1);
    }

    Vector2 BezierCurve(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        Vector2 pa = BezierCurve(t, p0, p1);
        Vector2 pb = BezierCurve(t, p1, p2);
        return BezierCurve(t, pa, pb);
    }

    string SetFlyingTime()
    {

        int _index = TravelManager.Instance._countryIdx;
        int _travelidx = TravelManager.Instance.SelectedCountry[_index];
        int FlyIndex = gui.CountrysData[_travelidx].idx;
        string flytime = "1시간 20분";

        for (int i = 0; i < LinePoint.Length; i++)
            LinePoint[i].localScale = new Vector3(0.35f, 0.35f, 0.35f);
        switch (FlyIndex)
        {
            case 0:
                for (int i = 0; i < LinePoint.Length; i++)
                    LinePoint[i].localScale = new Vector3(0.175f, 0.175f, 0.175f);
                if (gui.CountrysData[_travelidx].CountryName == "Korea")
                    flytime = "비행시간 1시간 10분\n인천공항 -> " + gui.CountrysData[_travelidx].Name_kr;
                else
                    flytime = "비행시간 2시간 10분\n인천공항 -> " + gui.CountrysData[_travelidx].Name_kr;
                break;
            case 1:
                flytime = "비행시간 5시간 25분\n인천공항 -> " + gui.CountrysData[_travelidx].Name_kr;
                break;
            case 2:
                flytime = "비행시간 7시간 35분\n인천공항 -> " + gui.CountrysData[_travelidx].Name_kr;
                break;
            case 3:
                flytime = "비행시간 9시간 30분\n인천공항 -> " + gui.CountrysData[_travelidx].Name_kr;
                break;
            case 4:
                flytime = "비행시간 11시간 20분\n인천공항 -> " + gui.CountrysData[_travelidx].Name_kr;
                break;
            case 5:
                flytime = "비행시간 12시간 05분\n인천공항-> " + gui.CountrysData[_travelidx].Name_kr;
                break;
            case 6:
                flytime = "비행시간 11시간 55분\n인천공항 -> " + gui.CountrysData[_travelidx].Name_kr;
                break;
            case 7:
                flytime = "비행시간 11시간 55분\n인천공항 -> " + gui.CountrysData[_travelidx].Name_kr;
                break;
            case 8:
                flytime = "비행시간 13시간 40분\n인천공항 -> " + gui.CountrysData[_travelidx].Name_kr;
                break;
            case 9:
                flytime = "비행시간 14시간 00분\n인천공항 -> " + gui.CountrysData[_travelidx].Name_kr;
                break;
            case 10:
                flytime = "비행시간 14시간 00분\n인천공항 -> " + gui.CountrysData[_travelidx].Name_kr;
                break;
        }
        return flytime;
    }

    void ResetPoint()
    {
        for (int i = 0; i < LinePointObj.Length; i++)
            LinePointObj[i].SetActive(false);
    }
    public Vector2 TargetPin2;
    /*  private void Update()
      {
          if (Input.GetKeyDown(KeyCode.A))
          {
              LinePath = new Vector2[LinePoint.Length];
              RectTransform target = ArtPin[0].transform.parent.GetComponent<RectTransform>();
              Debug.Log("targe t: " + target.name);
              FindPath(target.anchoredPosition);
              Cent.anchoredPosition = CentPos[6];
              Mover.anchoredPosition = CentPos[0];
              StartCoroutine(DrawPath());
          }
          if (Input.GetKeyDown(KeyCode.S))
          {
              LinePath = new Vector2[LinePoint.Length];
              RectTransform target = ArtPin[4].transform.parent.GetComponent<RectTransform>();
              Debug.Log("targe t: " + target.name);
              FindPath(target.anchoredPosition);
              Cent.anchoredPosition = CentPos[5];
              Mover.anchoredPosition = CentPos[0];
              StartCoroutine(DrawPath());
          }
      }*/
    void FindPath(Vector2 TargetPin)
    {
        //TargetPin2 = TargetPin;
        BezierSpeed = 0.1f;
        LinePath = new Vector2[LinePoint.Length];
        //A에서 B로 이동할 경로 생성자(Mover)를 A 위치(인천공항)으로 초기화
        Mover.anchoredPosition = CentPos[0];

        //이동 경로가 아시아인경우 직선 베지어 공식 사용해 LinePath에 저장
        if (CentIdx == 0)
        {
            for (int i = 0; i < LinePath.Length; i++)
            {
                LinePath[i] = new Vector2();
                Mover.anchoredPosition = BezierCurve(BezierSpeed, Mover.anchoredPosition, TargetPin);
                LinePath[i] = Mover.anchoredPosition;
                BezierSpeed = 0.1f * i;
            }
        }
        else
        {//이동경로가 아시아가 아닌경우 곡선 베지어 공식
            for (int i = 0; i < LinePath.Length; i++)
            {
                LinePath[i] = new Vector2();
                Cent.anchoredPosition = BezierCurve(BezierSpeed, Cent.anchoredPosition, TargetPin);
                Mover.anchoredPosition = BezierCurve(BezierSpeed, Mover.anchoredPosition, Cent.anchoredPosition, TargetPin);
                LinePath[i] = Mover.anchoredPosition;
                BezierSpeed = 0.1f * i;
            }
        }
        //비행시간을 경로 3번 위치에 적용
        Vector2 txt = LinePath[3];
        txt.y += 50;
        timerobj.SetActive(true);
        timer.anchoredPosition = txt;
        FlyTimer.text = SetFlyingTime();
    }

    IEnumerator DrawPath()
    {
        //곡선의 path에 포인트를 찍어주는 효과
        for (int i = 0; i < LinePoint.Length; i++)
        {
            LinePointObj[i].SetActive(true);
            LinePoint[i].anchoredPosition = LinePath[i];
            yield return new WaitForSeconds(0.15f);
        }

        yield return null;
    }
    //미사용.
    //A와 B 사이의 중앙점을 반환하는 함수
    Vector2 FindCenterAtoB(Vector2 A, Vector2 B)
    {
        Vector2 center = new Vector2();
        center = (A + B) / 2;
        return center;
    }

    public void ResetIint()
    {
        PlayIdx = 0;
        LinePath = new Vector2[LinePoint.Length];
        _theme = -1;
        _countryIdx = new int[5];
        for (int i = 0; i < _countryIdx.Length; i++)
            _countryIdx[i] = -1;
        ResetPin();
        ResetPoint();
    }

    public void SetTheme(int _SelectTheme)
    {
        _theme = _SelectTheme;
    }
    //선택된 여행지를 순서대로 저장
    public void SetCountry(int _SelectCountry, int _idx)
    {
        _countryIdx[_idx] = _SelectCountry;
    }
    public void SetPin()
    {
        if (PlayIdx >= 5)
            return;
        switch (_theme)
        {
            case 0:
                SetCulturePin(_countryIdx[PlayIdx]);
                break;
            case 1:
                SetArtPin(_countryIdx[PlayIdx]);
                break;
            case 2:
                SetCityPin(_countryIdx[PlayIdx]);
                break;
            case 3:
                SetTourPin(_countryIdx[PlayIdx]);
                break;
            case 4:
                SetNaturePin(_countryIdx[PlayIdx]);
                break;
            case 5:
                SetHistoryPin(_countryIdx[PlayIdx]);
                break;
        }
        PlayIdx++;
    }

    public void ResetPin()
    {
        Mover.anchoredPosition = MyPin.anchoredPosition;
        timerobj.SetActive(false);
        ResetPoint();
        InceonPin.Play("DefaultePinpoint");
        for (int i = 0; i < CulturePin.Length; i++)
            CulturePin[i].Play("DefaultePinpoint");

        for (int i = 0; i < ArtPin.Length; i++)
            ArtPin[i].Play("DefaultePinpoint");

        for (int i = 0; i < CityPin.Length; i++)
            CityPin[i].Play("DefaultePinpoint");

        for (int i = 0; i < TourPin.Length; i++)
            TourPin[i].Play("DefaultePinpoint");

        for (int i = 0; i < NaturePin.Length; i++)
            NaturePin[i].Play("DefaultePinpoint");

        for (int i = 0; i < HistoryPin.Length; i++)
            HistoryPin[i].Play("DefaultePinpoint");
    }

    public void SetCulturePin(int _idx)
    {
        ResetPin();
        InceonPin.Play("AppearPinPoint");
        CulturePin[_idx].Play("AppearPinPoint");
        RectTransform target = CulturePin[_idx].transform.parent.GetComponent<RectTransform>();
        FindPath(target.anchoredPosition);
        StartCoroutine(DrawPath());
    }
    public void SetArtPin(int _idx)
    {
        ResetPin();
        InceonPin.Play("AppearPinPoint");
        ArtPin[_idx].Play("AppearPinPoint");
        RectTransform target = ArtPin[_idx].transform.parent.GetComponent<RectTransform>();
        FindPath(target.anchoredPosition);
        StartCoroutine(DrawPath());
    }
    public void SetCityPin(int _idx)
    {
        ResetPin();
        InceonPin.Play("AppearPinPoint");
        CityPin[_idx].Play("AppearPinPoint");
        RectTransform target = CityPin[_idx].transform.parent.GetComponent<RectTransform>();

        FindPath(target.anchoredPosition);
        StartCoroutine(DrawPath());
    }
    public void SetTourPin(int _idx)
    {
        ResetPin();
        InceonPin.Play("AppearPinPoint");
        TourPin[_idx].Play("AppearPinPoint");
        RectTransform target = TourPin[_idx].transform.parent.GetComponent<RectTransform>();
        FindPath(target.anchoredPosition);
        StartCoroutine(DrawPath());
    }
    public void SetNaturePin(int _idx)
    {
        ResetPin();
        InceonPin.Play("AppearPinPoint");
        NaturePin[_idx].Play("AppearPinPoint");
        RectTransform target = NaturePin[_idx].transform.parent.GetComponent<RectTransform>();
        FindPath(target.anchoredPosition);
        StartCoroutine(DrawPath());
    }
    public void SetHistoryPin(int _idx)
    {
        ResetPin();
        InceonPin.Play("AppearPinPoint");
        HistoryPin[_idx].Play("AppearPinPoint");
        RectTransform target = HistoryPin[_idx].transform.parent.GetComponent<RectTransform>();
        FindPath(target.anchoredPosition);
        StartCoroutine(DrawPath());
    }
}
