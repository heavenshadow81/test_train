using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ML.MapoContents.WorldTravel;

//상태 전이에 따라 UI애니메이션 프로세스
public class UIEvent : MonoBehaviour {
    //타이틀 애니메이션
    public Animation Title_Anim;
    public Image Title_Img;

    //메인 배경이미지 애니메이션
    public Animation MainBackground;
    public bool _mainBackEvent;
    //테마선택 애니메이션
    public GameObject ThemeUI;
    public GameObject ThemeGuide;
    public Animation[] Theme_Anim;
    public Image[] Themes;
    public Sprite[] DefaulteThemeImage;
    public Sprite[] CheckedThemeImage;
    bool[] _chekedThmes;

    bool _themeEvent;

    //나라선택 애니메이션
    public GameObject CountryGuide;
    public GameObject NextButton;
    public Animator[] CountryAnimator;
    public GameObject[] Countrys;
    public GameObject[] CheckedCulture;
    public GameObject[] DefaulteCulture;

    public GameObject[] CheckedArt;
    public GameObject[] DefaulteArt;

    public GameObject[] CheckedCity;
    public GameObject[] DefaulteCity;

    public GameObject[] CheckedTour;
    public GameObject[] DefaulteTour;

    public GameObject[] CheckedNature;
    public GameObject[] DefaulteNature;

    public GameObject[] CheckedHistory;
    public GameObject[] DefaulteHistory;

    bool _countryEvent;

    //맵 애니메이션
    public GameObject FlyUI;
    public GameObject FlyMap;
    public GameObject FlyGuide;
    public Animation NextTravelMentAnim;
    public Animation Map;
    public Animation[] ContryClips;
    public Animation MapBack;
    public PinPoint pin;
    bool _flyEvent;

    //여행지 데이터
    public TravelData[] CountrysData;

    //플레이 UI
    public GameObject PlayUI;
    public Text CountryName;
    public Text TravelName;
    public RegionTimeUI TimeUI;
    public Image Timer_Back;
    public Image Timer_Front;
    public Text TimerText;
    public Image Earth;
    public Sprite[] Earth_Travel;
    bool _timerPlay;
    bool _warnning;

    //캡쳐이미지 뷰잉
    public Texture2D[] CaptureView;
    public RawImage[] View_image;
    public GameObject[] View_imageObj;
    public GameObject[] View_imageFrame;
    public Texture DefaulteImage;
    public CaptureManager capture;
    public GameObject ViewObj;
    public Animation ViewAnim;
    bool _isViewing;

    public Animation Ending;
    public GameObject EndingObj;
    bool _isEnding;
    
    public void EndingSet()
    {
        _isEnding = false;
        EndingObj.SetActive(false);
    }
    public void StartEnding()
    {
        if (!_isEnding)
            StartCoroutine(EndingPlay());
    }
    IEnumerator EndingPlay()
    {
        _isEnding = true;
        EndingObj.SetActive(true);
        Ending.Play("Ending");
        yield return new WaitForSeconds(6.5f);
        TravelManager.Instance.ResetTravel();

        MapBack.Play("DisapperMainBackground");
        yield return new WaitForSeconds(0.5f);
        
        Title_Anim.Play("AppearTitle");
        titleOn = true;
        _isEnding = false;
    }

    void Awake()
    {
        _chekedThmes = new bool[6];
        _mainBackEvent = true;
        FirstFly = true;
        //기본 초기화
        ViewerSet();
        ThemeSet();
        TitleSet();
        CountrySet();
        MainBackSet();
        DefaulteTheme();
        MapSet();
        ResetPlay();
        titleOn = true;
    }
	public void ResetUIAll()
	{
        titleOn = true;
        FirstFly = true;
		_chekedThmes = new bool[6];
		//기본 초기화
		ViewerSet();
		ThemeSet();
		TitleSet();
		CountrySet();
		MainBackSet();
		DefaulteTheme();
		MapSet();
		ResetPlay();
        _mainBackEvent = true;

        StopCoroutine (TimerGUI ());
		_timerPlay = false;
		_warnning = false;
		Timer_Front.fillAmount = 1;
        Timer_Front.color = Color.white;
        TimerText.text = "3:00";
        float timer = 180;
		float remain = timer / 180;

        for (int i = 0; i < View_image.Length; i++)
        {
            View_imageObj[i].SetActive(true);
            View_imageFrame[i].SetActive(true);
            View_image[i].texture = DefaulteImage;
        }
	}

    public void ViewerSet()
    {
        ViewObj.SetActive(false);
        _isViewing = false;
        ViewAnim.Play("DefaulteView");
    }
    public void StartCpatureView(int TravelCnt)
    {
        if(!_isViewing)
        {           
           
            ResetPlay();
            StartCoroutine(CaptureViewing(TravelCnt));
        }
    }
    IEnumerator CaptureViewing(int TravelCnt)
    {
        MapBack.Play("AppearMainBackground");
        yield return new WaitForSeconds(0.5f);
        MainBackSet();

        _isViewing = true;
        ViewObj.SetActive(true);
        CaptureView = capture.GetCpature();
       
        for (int i = 0; i < TravelCnt; i++)
        {
            View_image[i].texture = CaptureView[i];
        }
        for (int i = TravelCnt; i < View_image.Length; i++)
        {
            View_imageObj[i].SetActive(false);
            View_imageFrame[i].SetActive(false);
        }

        yield return new WaitForSeconds(0.5f);
        ViewAnim.Play("AppearView");
        yield return new WaitForSeconds(5f);

        ViewAnim.Play("DisAppearView");
        yield return new WaitForSeconds(2f);
        ViewerSet();
        TravelManager.Instance.Ending();
    }

    public void MainBackSet()
    {
        if (!_mainBackEvent)
        {
            _mainBackEvent = true;
            MainBackground.Play("AppearMainBackground");
        }   
    }
    public void MainBackEnd()
    {
        if(_mainBackEvent)
        {
            MainBackground.Play("DisapperMainBackground");
            _mainBackEvent = false;
        }
    }
    public bool titleOn;
    public void TitleFadeOut()
    {
        titleOn = false;
        Title_Anim.Play("TitleFadeOut");
    }
    public bool TitleCheck()
    {
        if (Title_Img.color.a >= 0.5f)
            return true;
        else
            return false;
    }
    public void TitleOff()
    {
        Color tmp = Title_Img.color;
        tmp.a = 0;
        Title_Img.color = tmp;
    }
    public void TitleSet()
    {
        Color tmp = Title_Img.color;
        tmp.a = 1;
        Title_Img.color = tmp;
    }

    public void ThemeSelect()
    {
        if (!_themeEvent)
            StartCoroutine(ThemeAppearAnim());
    }
    public void ThemeEnd()
    {
        if (!_themeEvent)
            StartCoroutine(ThemeDisappearAnim());
    }
    public void ThemeSet()
    {
        for (int i = 0; i < Theme_Anim.Length; i++)
        {
            Theme_Anim[i].Play("DefaluteTheme");
        }
        ThemeGuide.SetActive(false);
	}
	public void DefaulteTheme()
	{
		for (int i = 0; i < Themes.Length; i++)
		{
			_chekedThmes [i] = false;
			Themes [i].sprite = DefaulteThemeImage [i];
		}
	}
	public void checkedTheme(int _idx)
	{
		if (_idx == 6)
			return;
		if (!_chekedThmes [_idx]) 
		{
			DefaulteTheme ();
			_chekedThmes [_idx] = true;
			Themes [_idx].sprite = CheckedThemeImage [_idx];
		}
	}
    IEnumerator ThemeAppearAnim()
    {
        MapBack.Play("AppearMainBackground");
        ThemeUI.SetActive (true);
        _themeEvent = true;
        yield return new WaitForSeconds(2f);
        ThemeGuide.SetActive(true);

        for (int i = 0; i < Theme_Anim.Length; i++)
        {
            Theme_Anim[i].Play("AppearTheme");
        }
        _themeEvent = false;
		TravelManager.Instance._state = TravelManager.TravelState.SelectTheme;
        yield return null;
    }
    IEnumerator ThemeDisappearAnim()
	{
        _themeEvent = true;
        yield return new WaitForSeconds(0.5f);
        ThemeGuide.SetActive(false);

        for (int i = Theme_Anim.Length -1; i >= 0; i--)
        {
            Theme_Anim[i].Play("DisappearTheme");
		}

		yield return new WaitForSeconds(0.5f);
		ThemeUI.SetActive (false);
        _themeEvent = false;
		yield return new WaitForSeconds (1.5f);
		for(int i =0; i<Themes.Length; i++)
			Themes [i].sprite = DefaulteThemeImage [i];
        yield return null;
    }

    int _theme_idx;
    public void CountrySelect(int _themeIdx)
    {
        _theme_idx = _themeIdx;
        if (!_countryEvent)
            StartCoroutine(CountryApperAnim());
    }
    public void CountryEnd()
    {
        if (!_countryEvent)
            StartCoroutine(CountryDisapperAnim());
    }
    public void CountrySet()
    {
		for(int i = 0; i< Countrys.Length; i++)
		{
            Countrys[i].SetActive (false);
		}
        for (int i = 0; i < 9; i++)
        {
            DefaulteCulture[i].SetActive(true);
            CheckedCulture[i].SetActive(false);

            DefaulteArt[i].SetActive(true);
            CheckedArt[i].SetActive(false);

            DefaulteCity[i].SetActive(true);
            CheckedCity[i].SetActive(false);

            DefaulteTour[i].SetActive(true);
            CheckedTour[i].SetActive(false);

            DefaulteNature[i].SetActive(true);
            CheckedNature[i].SetActive(false);

            DefaulteHistory[i].SetActive(true);
            CheckedHistory[i].SetActive(false);
        }

        CountryGuide.SetActive(false);
        NextButton.SetActive(false);
    }
	public void CheckCountry(int _idx )
	{
        if (_idx >= 0)
        {
            switch (_theme_idx)
            {
                case 0:
                    DefaulteCulture[_idx].SetActive(false);
                    CheckedCulture[_idx].SetActive(true);
                    break;
                case 1:
                    DefaulteArt[_idx].SetActive(false);
                    CheckedArt[_idx].SetActive(true);
                    break;
                case 2:
                    DefaulteCity[_idx].SetActive(false);
                    CheckedCity[_idx].SetActive(true);
                    break;
                case 3:
                    DefaulteTour[_idx].SetActive(false);
                    CheckedTour[_idx].SetActive(true);
                    break;
                case 4:
                    DefaulteNature[_idx].SetActive(false);
                    CheckedNature[_idx].SetActive(true);
                    break;
                case 5:
                    DefaulteHistory[_idx].SetActive(false);
                    CheckedHistory[_idx].SetActive(true);
                    break;
            }
        }        
	}

    IEnumerator CountryApperAnim()
    {
        _countryEvent = true;
        Countrys[_theme_idx].SetActive(true);
        yield return new WaitForSeconds(2f);

        CountryGuide.SetActive(true);
        NextButton.SetActive(true);

        CountryAnimator[_theme_idx].SetTrigger("Appear");
        yield return new WaitForSeconds(2f);

        _countryEvent = false;
		TravelManager.Instance._state = TravelManager.TravelState.SelectCountry;
        yield return null;
    }
    IEnumerator CountryDisapperAnim()
	{
        _countryEvent = true;
        yield return new WaitForSeconds(0.5f);

        CountryGuide.SetActive(false);
        CountryAnimator[_theme_idx].SetTrigger("DisAppear");
        NextButton.SetActive(false);
        yield return new WaitForSeconds(1f);
        
        Countrys[_theme_idx].SetActive(false);        
        _countryEvent = false;
        yield return new WaitForSeconds(1.5f);
        FlyEvent();
        yield return null;
    }
    bool _nextTravelEventPlaying;
    public void NextTravelEvent()
    {
        if (!_nextTravelEventPlaying)
            StartCoroutine(NextTravel());
    }
    IEnumerator NextTravel()
    {
        _nextTravelEventPlaying = true;
        NextTravelMentAnim.Play("NextTravelMent");
        yield return new WaitForSeconds(2.2f);
        FlyEvent();
        ResetPlay();
    }

    public void MapSet()
    {
        FlyUI.SetActive(false);
        FlyMap.SetActive(false);
        FlyGuide.SetActive(false);
    }
    public void FlyEvent()
    {
        if (!_flyEvent)
            StartCoroutine(FlyAnim());
    }

    string SelectFlyingAnimation()
    {
        int _index = TravelManager.Instance._countryIdx;
        int _travelidx = TravelManager.Instance.SelectedCountry[_index];
        int FlyIndex = CountrysData[_travelidx].idx;
        string _anim = "DisappearMap";
        switch (FlyIndex)
        {
            case 0:
                _anim = "DisappearAsia";
                break;
            case 1:
                _anim = "DisappearAnkorwat";
                break;
            case 2:
                _anim = "DisappearIndia";
                break;
            case 3:
                _anim = "DisappearUAE";
                break;
            case 4:
                _anim = "DisappearIstanbul";
                break;
            case 5:
                _anim = "DisappearItaly";
                break;
            case 6:
                _anim = "DisappearFrance";
                break;
            case 7:
                _anim = "DisappearEngland";
                break;
            case 8:
                _anim = "DisappearLA";
                break;
            case 9:
                _anim = "DisappearNASA";
                break;
            case 10:
                _anim = "DisappearNewyok";
                break;
        }
        return _anim;
    }
    public GameObject TowerBridge;
    public bool FirstFly;
    IEnumerator FlyAnim()
    {
        if (FirstFly)
            FirstFly = false;
        else
            MapBack.Play("AppearMainBackground");

        FlyUI.SetActive(true);
        FlyMap.SetActive(true);
        
        _flyEvent = true;
        yield return new WaitForSeconds(1f);
        MainBackEnd();
        Map.Play("AppearMap");
        FlyGuide.SetActive(true);
        yield return new WaitForSeconds(1f);

        int _travelidx = TravelManager.Instance.SelectedCountry[TravelManager.Instance._countryIdx];
        int FlyIndex = CountrysData[_travelidx].idx;
        if (CountrysData[_travelidx].Name_kr == "타워 브릿지")
            TowerBridge.SetActive(true);
        else
            TowerBridge.SetActive(false);

        pin.SetCent(FlyIndex);
        pin.SetPin();
        yield return new WaitForSeconds(3f);

		Map.Play(SelectFlyingAnimation());
        yield return new WaitForSeconds(2f);

        pin.ResetPin();
        yield return new WaitForSeconds(1f);

        FlyMap.SetActive(false);
        FlyUI.SetActive(false);
        TravelManager.Instance.CompleteFlying();
        _flyEvent = false;
        _nextTravelEventPlaying = false;
        MapBack.Play("DisapperMainBackground");
    }
    public void SetCountryData(string _name)
    {
        //여행지 데이터를 json으로부터 불러옴. _name은 json의 Key
        CountrysData = JsonHelper.Instance.Load(_name);       
    }
    public void ResetPlay()
    {
        PlayUI.SetActive(false);
    }
    public Text Photo_TravelName;
    public Text Photo_Date;
    public void SetPlayUI(int idx)
    {
        System.DateTime currentTime = System.DateTime.Parse(System.DateTime.Now.ToString());
        string _date = string.Format("{0}/{1}/{2}", currentTime.Year, currentTime.Month, currentTime.Day);
        Photo_Date.text = _date;

        PlayUI.SetActive(true);
        string CountryNameis = string.Format(CountrysData[idx].CountryName_kr + "(" + CountrysData[idx].CountryName + ")");
        string TravelNameis = string.Format(CountrysData[idx].Name_kr + "(" + CountrysData[idx].Name + ")");
        Photo_TravelName.text = CountrysData[idx].CountryName;
        Earth.sprite = Earth_Travel[CountrysData[idx].idx];
        TravelName.text = TravelNameis;// CountrysData[idx].Name;
        CountryName.text = CountryNameis;// CountrysData[idx].CountryName;
        TimeUI.SetTimeZone(CountrysData[idx].CountryName);
        StartTimer();
    }
    public void StartTimer()
    {
        if (!_timerPlay)
            StartCoroutine(TimerGUI());
    }
    IEnumerator TimerGUI()
    {
        _timerPlay = true;
        _warnning = false;
        Timer_Front.fillAmount = 1;
        float timer = 180;
        float TimeOver_value = 180;
        float remain = timer / TimeOver_value;

        int min = (int)(timer / 3);
        int sec = (int)(timer % 3);
        string tim = string.Format("{0}:{1}", min, sec);
        while (true)
		{
			if (!_timerPlay)
				break;
            if(TravelManager.Instance._state == TravelManager.TravelState.Play)
                timer -= 1;
            remain = timer / TimeOver_value;
            Timer_Front.fillAmount = remain;
            min = (int)(timer / 60);
            sec = (int)(timer % 60);
            string s = sec.ToString();
            if (sec < 10)
                s = "0" + sec.ToString();
            tim = string.Format("{0}:{1}", min, s);
            TimerText.text = tim;
            if (timer <= 30 && !_warnning)
            {
                _warnning = true;
                Timer_Front.color = Color.red;
            }
            if (timer <= 0)
                break;
            yield return new WaitForSeconds(1f);
        }
        yield return new WaitForSeconds(1f);
        TravelManager.Instance.TimeOver();
    }
}
