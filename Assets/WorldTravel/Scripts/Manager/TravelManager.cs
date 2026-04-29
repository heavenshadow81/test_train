using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ML.MapoContents.WorldTravel
{
    using Kinect;

    public class TravelManager : MonoBehaviour
    {
        public enum TravelState
        {
            Intro,              //기본 인트로. 타이틀화면
            Loading,            //인식 직후 로딩상태
            SelectTheme,        //테마선택 상태
            SelectCountry,      //나라선택 상태
            Transition,			//전환중 상태
            Flying,             //비행경로 상태
            Play,               //테마여행 플레이 상태
            Pause,              //일시정지 상태(아직 미정)
            CaptureView,        //엔딩전 합성사진 보여주기
            End                 //엔딩 상태           
        }
        public bool TabletControll;
        public KinectCamera kinectCamera;
        public static TravelManager Instance;
        public TravelState _state;
        public string SelectedTheme;
        public int ThemeIndex;
		public int[] SelectedCountry;
		public int _countryIdx;

        public Material[] CultureSkyBoxs;
        public Material[] ArtSkyBoxs;
        public Material[] CitySkyBoxs;
        public Material[] TourSkyBoxs;
        public Material[] NatureSkyBoxs;
        public Material[] HistorySkyBoxs;
        public TravelData[] CountrysData;
        //로딩서클 클래스
        public LoadingCircle mainLoading;

        //UI이벤트 클래스
        public UIEvent gui;
        //사진 캡쳐 매니저
        public CaptureManager capture;
        //키넥트 카메라
        public KinectCamera kinectcam;
        //키넥트 일반
        public KinectHelper kinecthelper;
        //손 커서
		public HandCursor hand;
        //
        public GameObject HandObj;
        //
		public GameObject LoadingPhoto;
        //
		public Image PhotoIn;
        
        public TumbnailEvent tumbnail;
        //이메일 보내는 용도의....
        public TCP_Server server;
        //
        public MainBackSlideShow slideshow;

        public void Awake()
        {
            tumbnail.ResetCheck();

            /*
            if (Display.displays.Length > 1)
                Display.displays[1].Activate();
            if (Display.displays.Length > 2)
                Display.displays[2].Activate();*/
            _countryIdx = 0;
            ThemeIndex = 0;
            SelectedCountry = new int[5];
			for (int i = 0; i < SelectedCountry.Length; i++)
				SelectedCountry [i] = -1;
            Instance = this;
            _state = TravelState.Intro;
			kinectCamera.enabled = false;
			PhotoIn.fillAmount = 0;
			LoadingPhoto.SetActive (false);
            TabletControll = false;
        }

        public void Start()
        {
            KinectHelper helper = KinectHelper.instance;
            if (helper.Init())
            {
                helper.enablesBodyTracking = true;
                kinectCamera.enabled = true;
            }
        }
        private void OnEnable()
        {
            InitializeTravel();
        }
        private void Update()
        {/*
            if (Input.GetKeyDown(KeyCode.I))
                server.SetImageArray(2);
            if (Input.GetKeyDown(KeyCode.L))
                server.Send();*/
            if (TabletControll)
                HandActive(false);
            else
                HandActive(true);

            if (_state == TravelState.SelectTheme)
            {
                if (gui.TitleCheck())
                    gui.TitleOff();
            }
        }

        public void HandActive(bool _active)
        {
            HandObj.SetActive(_active);
        }
        //사용자가 키넥트에 인식 되었을때. 로딩서클 활성화
        public void CatchPlayer()
        {
            if (_state == TravelState.Intro)
            {
                _state = TravelState.Loading;
                //mainLoading.StartLoading();
                CompleteLoading();
            }
        }
        
        //사용자가 키넥트에서 사라졌을 때. 일시정지 상태로 전이(미구현)
        public void LostPlayer()
        {
            //_state = TravelState.Pause;
        }

        //상태를 테마선택으로 전이. 전이에따라 전환효과 이벤트 호출
        public void CompleteLoading()
        {
            if(_state == TravelState.Loading)
            {
                gui.TitleFadeOut();
				_state = TravelState.Transition;
                gui.ThemeSelect();
            }
        }

        //테마선택 종료시점. 나라선택상태로 전이. 전환효과 이벤트 호출
		public void CompleteTheme(string _theme , int _themeIdx)
        {
            if (_state == TravelState.SelectTheme)
            {
                ThemeIndex = _themeIdx;
                pin.SetTheme(_themeIdx);
				SelectedTheme = _theme;
                gui.ThemeEnd();
				_state = TravelState.Transition;
                gui.CountrySelect(ThemeIndex);
                CountrysData = JsonHelper.Instance.Load(_theme);
            }
        }

        //테마선택 종료시점. 나라선택상태로 전이. 전환효과 이벤트 호출
        public void SelectThemeButton(int _themeIdx)
        {
            if (_state == TravelState.SelectTheme)
            {
                string _theme = "";
                switch (_themeIdx)
                {
                    
                    case 0:
                        _theme = "Culture";
                        break;
                    case 1:
                        _theme = "Art";
                        break;
                    case 2:
                        _theme = "City";
                        break;
                    case 3:
                        _theme = "Tour";
                        break;
                    case 4:
                        _theme = "Nature";
                        break;
                    case 5:
                        _theme = "History";
                        break;
                    case 6:
                        _theme = "SelectTheme";
                        break;
                }
                ThemeIndex = _themeIdx;
                pin.SetTheme(_themeIdx);
                SelectedTheme = _theme;
                gui.ThemeEnd();
                _state = TravelState.Transition;
                gui.CountrySelect(ThemeIndex);
                CountrysData = JsonHelper.Instance.Load(_theme);
            }
        }


        //여행지선택 -> 테마선택
        public void ReturnTheme()
        {
            Debug.Log("테마로 돌아가기" + _state);
            if(_state == TravelState.SelectCountry)
            {
                ThemeIndex = 0;
                pin.SetTheme(0);
                SelectedTheme = "";
                gui.ThemeSelect();
                _state = TravelState.SelectTheme;
                CountrysData = null;
                for (int i = 0; i > SelectedCountry.Length; i++)
                    SelectedCountry[i] = -1;
                _countryIdx = 0;
                gui.CountrySet();
            }
        }
        //인트로로 다시돌아옴
        public void ReturnHome()
        {
            
            slideshow.StartSlideShow();
            gui.ResetUIAll();
            tumbnail.ResetCheck();
            capture.ResetInit();
            pin.ResetIint();
            hand.ResetInit();
            gui.ResetUIAll();
            PhotoIn.fillAmount = 0;
            LoadingPhoto.SetActive(false);
            SelectedTheme = "";
            _countryIdx = 0;
            ThemeIndex = 0;
            SelectedCountry = new int[5];
            for (int i = 0; i < SelectedCountry.Length; i++)
                SelectedCountry[i] = -1;
            _state = TravelState.Intro;
        }
        public PinPoint pin;

        //중복된 여행지가 있는지 체크
        public bool OverlapCheck(int _country)
        {
            for (int i = 0; i < SelectedCountry.Length; i++)
            {
                if (_country == SelectedCountry[i])
                    return false;
            }
            return true;
        }
        //여행지 추가등록
		public void SelectCountry(int _country)
		{
			if (_state == TravelState.SelectCountry)
			{
				SelectedCountry[_countryIdx] = _country;
				gui.CheckCountry (_country);
				_countryIdx++;
            }
		}
        //선택한 나라 갯수
        public int CountTravel()
        {
            int cnt = 0;
            for (int i = 0; i < SelectedCountry.Length; i++)
            {
                if (SelectedCountry[i] != -1)
                    cnt++;
            }
            return cnt;
        }
        public PhotoManager photomanager;
        //나라선택 종료시점. 비행경로 상태로 전이. 전환효과 이벤트 호출
        public void CompleteCountry()
        {
            if (_state == TravelState.SelectCountry)
            {
                //보낼 이미지 개수 초기화
                server.SetImageArray(CountTravel()+1);
                photomanager.SetSize(CountTravel());
                for (int i =0; i<5; i++)
                    pin.SetCountry(SelectedCountry[i],i);
                gui.CountryEnd();
                _state = TravelState.Flying;
                //썸네일 개수 셋
                int cnt = 0 ;
                for (int i = 0; i < 5; i++)
                {
                    if (SelectedCountry[i] != -1)
                    {
                        cnt++;
                    }
                }
                tumbnail.SetImagePosition(cnt);
                for (int i = 0; i < 5; i++)
                {
                    if (SelectedCountry[i] != -1)
                    {
                        tumbnail.SetTravelimage(ThemeIndex, SelectedCountry[i], i);
                        //tumbnail.SetImage(ThemeIndex, SelectedCountry[i]);
                    }
                    
                }
                
                _countryIdx = 0;
            }
        }

        //비행경로 종료시점. 테마여행 플레이 상태로 전이. 전환효과 이벤트 호출
        public void CompleteFlying()
        {
            if (_state == TravelState.Flying)
            {
                _state = TravelState.Play;
                if (!kinectcam.BodyCheck)
                    kinectcam.BodyCheck = true;

                gui.SetPlayUI(SelectedCountry[_countryIdx]);
                ChangeSkyBox();
                _countryIdx++;
            }
        }

        public GameObject[] NameTags_Culture;
        public GameObject[] NameTags_Art;
        public GameObject[] NameTags_City;
        public GameObject[] NameTags_Tour;
        public GameObject[] NameTags_Nature;
        public GameObject[] NameTags_History;
        public TravelTTS tts;
        //선택된 여행지로 스카이박스 변경
        void ChangeSkyBox()
        {
            for (int i = 0; i < 9; i++)
            {
                if (i < NameTags_Culture.Length)
                    NameTags_Culture[i].SetActive(false);

                if (i < NameTags_Art.Length)
                    NameTags_Art[i].SetActive(false);

                if (i < NameTags_City.Length)
                    NameTags_City[i].SetActive(false);

                if (i < NameTags_Tour.Length)
                    NameTags_Tour[i].SetActive(false);

                if (i < NameTags_Nature.Length)
                    NameTags_Nature[i].SetActive(false);

                if (i < NameTags_History.Length)
                    NameTags_History[i].SetActive(false);
            }

            switch (ThemeIndex)
            {
                case 0:
                    RenderSettings.skybox = CultureSkyBoxs[SelectedCountry[_countryIdx]];
                    NameTags_Culture[SelectedCountry[_countryIdx]].SetActive(true);
                    break;
                case 1:
                    RenderSettings.skybox = ArtSkyBoxs[SelectedCountry[_countryIdx]];
                    NameTags_Art[SelectedCountry[_countryIdx]].SetActive(true);
                    break;
                case 2:
                    RenderSettings.skybox = CitySkyBoxs[SelectedCountry[_countryIdx]];
                    NameTags_City[SelectedCountry[_countryIdx]].SetActive(true);
                    break;
                case 3:
                    RenderSettings.skybox = TourSkyBoxs[SelectedCountry[_countryIdx]];
                    NameTags_Tour[SelectedCountry[_countryIdx]].SetActive(true);
                    break;
                case 4:
                    RenderSettings.skybox = NatureSkyBoxs[SelectedCountry[_countryIdx]];
                    NameTags_Nature[SelectedCountry[_countryIdx]].SetActive(true);
                    break;
                case 5:
                    RenderSettings.skybox = HistorySkyBoxs[SelectedCountry[_countryIdx]];
                    NameTags_History[SelectedCountry[_countryIdx]].SetActive(true);
                    break;
            }
            tts.TTSisPlay(ThemeIndex, SelectedCountry[_countryIdx]);

        }
		private float _percent;
		bool _isCapture;
        //캡쳐
        public void CaptureEvent()
		{
			if (!_isCapture)
			{
				LoadingPhoto.SetActive (true);
				if (PhotoIn.fillAmount < 1)
				{
					_percent += Time.deltaTime * 4;
					PhotoIn.fillAmount = _percent;	
				}
				else if(PhotoIn.fillAmount >= 1)
				{
                    tts.StopTTS();
					_isCapture = true;
					capture.PhotoTime_Event();		
					PhotoIn.fillAmount = 0;
					_percent = 0;
					LoadingPhoto.SetActive (false);
				}
			}
        }
		public void CancleCapture()
		{
			PhotoIn.fillAmount = 0;
			_percent = 0;
			LoadingPhoto.SetActive (false);
		}

        void CaptrueEventComplete()
        {
			_isCapture = false;
            kinecthelper.CheckUperHand = false;
        }

        //타이머가 종료되는 경우
        public void TimeOver()
        {
            if (_state == TravelState.Play)
            {                
                _state = TravelState.CaptureView;
                gui.MainBackSet();
                gui.StartCpatureView(_countryIdx);

                if (kinectcam.BodyCheck)
                    kinectcam.BodyCheck = false;
            }
        }
        
        //다음 여행지로 넘어가기(선택한 n개의 여행지를 모두 들렸다면 캡쳐뷰 상태로)
        public void NextTravel()
        {
            if (_state == TravelState.Play)
            {
                tumbnail.CheckedTravel();
                CaptrueEventComplete();
                if (_countryIdx >= SelectedCountry.Length)
                {
                    _state = TravelState.CaptureView;
                    gui.MainBackSet();
                    gui.StartCpatureView(_countryIdx);
                    if (kinectcam.BodyCheck)
                        kinectcam.BodyCheck = false;
                    return;
                }
                if (SelectedCountry[_countryIdx] == -1)
                {
                    _state = TravelState.CaptureView;
                    gui.MainBackSet();
                    gui.StartCpatureView(_countryIdx);
                    if (kinectcam.BodyCheck)
                        kinectcam.BodyCheck = false;

                    return;
                }

                //gui.FlyEvent();
                //gui.ResetPlay();
                gui.NextTravelEvent();
                if (kinectcam.BodyCheck)
                    kinectcam.BodyCheck = false;
                _state = TravelState.Flying;
            }
        }
        
        //캡쳐뷰가 끝난 후 엔딩.(이메일 안내 문구)
        public void Ending()
        {
            if (_state == TravelState.CaptureView)
            {
                //사진 전송
                server.Send();
                _state = TravelState.End;
				gui.StartEnding();
            }
        }
        //엔딩문구가 끝난 후 리셋 및 데이터 초기화.
        //사용자 키넥트 ID도 리셋해야함.
        public void ResetTravel()
        {
            if (_state == TravelState.End)
            {
                KinectHelper.instance.AVG_Point = false;
                KinectHelper.instance.cnt_avg = 0;

                _state = TravelState.Intro;
                slideshow.StartSlideShow();
                tumbnail.ResetCheck();
                capture.ResetInit ();
				pin.ResetIint ();
				hand.ResetInit ();
				gui.ResetUIAll ();
				PhotoIn.fillAmount = 0;
				LoadingPhoto.SetActive (false);
				SelectedTheme = "";
                _countryIdx = 0;
                ThemeIndex = 0;
                SelectedCountry = new int[5];
                for (int i = 0; i < SelectedCountry.Length; i++)
                    SelectedCountry[i] = -1;
                Instance = this;
            }
        }

        void InitializeTravel()
        {
            KinectHelper.instance.AVG_Point = false;
            KinectHelper.instance.cnt_avg = 0;

            _state = TravelState.Intro;
            slideshow.StartSlideShow();
            tumbnail.ResetCheck();
            capture.ResetInit();
            pin.ResetIint();
            hand.ResetInit();
            gui.ResetUIAll();
            PhotoIn.fillAmount = 0;
            LoadingPhoto.SetActive(false);
            SelectedTheme = "";
            _countryIdx = 0;
            ThemeIndex = 0;
            SelectedCountry = new int[5];
            for (int i = 0; i < SelectedCountry.Length; i++)
                SelectedCountry[i] = -1;
            Instance = this;
        }
    }

}