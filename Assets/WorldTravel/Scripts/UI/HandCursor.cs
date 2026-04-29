using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace ML.MapoContents.WorldTravel
{
    using Kinect;

    public class HandCursor : MonoBehaviour
    {
        public RectTransform mainUICanvas;
        public Image hand;
        public Sprite handOpen, handClose;

        public RectTransform CursorTr;
        public Camera UIcam;

        public UIEvent gui;

        bool _isLoading;
        public bool _selectTheme, _selectCountry;
        float _percent, speed;
        int _themeIdx, _themePrevIdx;
        int _countryIdx, _countryPrevIdx;
        int _countryCnt;
        public Image _inCircle;
        public Image _outCircle;

        public void Start()
        {
            _countryCnt = 0;
            _selectTheme = false;
            _selectCountry = false;
            ResetCircle();

            if (hand == null)
                hand = GetComponentInChildren<Image>();
            KinectHelper helper = KinectHelper.instance;
        }
		public void ResetInit()
		{
			_countryCnt = 0;
			_selectTheme = false;
			_selectCountry = false;
			ResetCircle();
		}

        public Vector3 ch;
        public void Update()
        {           
            KinectHelper helper = KinectHelper.instance;
            hand.gameObject.SetActive(helper.trackingId != 0);
            if (hand.gameObject.activeSelf)
            {
               /* if (helper.rightHandState == Windows.Kinect.HandState.Open)
                {*/
                    Vector3 pos = hand.rectTransform.localPosition;
                    pos = Vector3.Lerp(pos, new Vector2(helper.uiSpaceRightHandPos.x - mainUICanvas.sizeDelta.x * 0.5f, mainUICanvas.sizeDelta.y * 0.5f - helper.uiSpaceRightHandPos.y), Time.deltaTime * 6.5f);
                    hand.rectTransform.localPosition = pos;
               // }
            }
            
            Ray ray = new Ray();
            ray.origin = hand.rectTransform.TransformPoint(Vector3.forward * UIcam.nearClipPlane);
            ray.direction = Vector3.forward;

            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
            if (hit.collider != null)
            {
                if (TravelManager.Instance._state == TravelManager.TravelState.SelectCountry)
                {
                    if (!_selectCountry)
                        SelectCountryState(hit.collider.name);
                }
                if (TravelManager.Instance._state == TravelManager.TravelState.SelectTheme)
                {
                    if (!_selectTheme)
                        SelectThemeState(hit.collider.name);
                }
                
            }
            if (TravelManager.Instance._state != TravelManager.TravelState.SelectCountry &&
               TravelManager.Instance._state != TravelManager.TravelState.SelectTheme)
            {
                hand.gameObject.SetActive(false);
            }


        }

        void SelectCountryState(string _countryname)
        {
            switch (_countryname)
            {
                case "Country1":
                    _countryIdx = 0;
                    break;
                case "Country2":
                    _countryIdx = 1;
                    break;
                case "Country3":
                    _countryIdx = 2;
                    break;
                case "Country4":
                    _countryIdx = 3;
                    break;
                case "Country5":
                    _countryIdx = 4;
                    break;
                case "Country6":
                    _countryIdx = 5;
                    break;
                case "Country7":
                    _countryIdx = 6;
                    break;
                case "Country8":
                    _countryIdx = 7;
                    break;
                case "Country9":
                    _countryIdx = 8;
                    break;
                case "CultureCountry":
                    _countryIdx = 10;
                    break;
                case "ArtCountry":
                    _countryIdx = 10;
                    break;
                case "CityCountry":
                    _countryIdx = 10;
                    break;
                case "TourCounty":
                    _countryIdx = 10;
                    break;
                case "NatureCountry":
                    _countryIdx = 10;
                    break;
                case "HistoryCountry":
                    _countryIdx = 10;
                    break;
                case "NextButton":
                    _countryIdx = 11;
                    break;
            }

            if (_countryIdx == 10)
                ResetCircle();
            else
            {
                if (_countryIdx >= 0)
                {
                    if (!_isLoading)
                    {
                        hand.sprite = handClose;
                        _isLoading = true;

                        Color tmp = _outCircle.color;
                        tmp.a = 1;
                        _outCircle.color = tmp;
                    }

                    if (_countryPrevIdx == _countryIdx)
                    {
                        //다음버튼이지만 선택된 여행지가 없다면
                        if (_countryIdx == 11 && TravelManager.Instance.CountTravel() < 1)
                        {
                            _percent = 0;
                            return;
                        }

                        //1초에걸쳐서 퍼센트 증가.
                        _percent += Time.deltaTime / 1.5f;
                        _inCircle.fillAmount = _percent;

                        if (_percent >= 1)
                        {
                            _percent = 1;
                            _inCircle.fillAmount = _percent;

                            //다음버튼이 아니라면(해당 나라 선택)
                            if (_countryIdx != 11)
                            {
                                if (TravelManager.Instance.OverlapCheck(_countryIdx))
                                {
                                    TravelManager.Instance.SelectCountry(_countryIdx);
                                    _countryCnt++;
                                }
                                if (_countryCnt >= 5)
                                {
                                    _selectCountry = true;
                                    TravelManager.Instance.CompleteCountry();
                                }
                            }                            
                            else
                            {
                                
                                for (int i = _countryCnt; i < 5; i++)
                                    TravelManager.Instance.SelectCountry(-1);

                                _selectCountry = true;
                                TravelManager.Instance.CompleteCountry();
                            }
                            ResetCircle();
                        }
                    }
                }

                _countryPrevIdx = _countryIdx;
            }
        }

        //테마 선택
        void SelectThemeState(string _name)
        {
            switch (_name)
            {
                case "Culture":
                    _themeIdx = 0;
                    break;
                case "Art":
                    _themeIdx = 1;
                    break;
                case "City":
                    _themeIdx = 2;
                    break;
                case "Tour":
                    _themeIdx = 3;
                    break;
                case "Nature":
                    _themeIdx = 4;
                    break;
                case "History":
                    _themeIdx = 5;
                    break;
                case "SelectTheme":
                    _themeIdx = 6;
                    break;
            }

            if (_themeIdx == 6)
            {
                gui.DefaulteTheme();
                ResetCircle();
            }
            else
            {
                if (!_isLoading)
                {
                    hand.sprite = handClose;
                    _isLoading = true;

                    Color tmp = _outCircle.color;
                    tmp.a = 1;
                    _outCircle.color = tmp;
                }

                if (_themePrevIdx == _themeIdx)
                {

                    //1초에걸쳐서 퍼센트 증가.
                    _percent += Time.deltaTime / 1.5f;
                    _inCircle.fillAmount = _percent;
                    gui.checkedTheme(_themeIdx);
                    if (_percent >= 1)
                    {
                        _percent = 1;
                        _inCircle.fillAmount = _percent;
                        TravelManager.Instance.CompleteTheme(_name, _themeIdx);
                        gui.SetCountryData(_name);
                        _selectTheme = true;
                        ResetCircle();
                    }
                }
                _themePrevIdx = _themeIdx;
            }
        }

        void ResetCircle()
        {
            if (_isLoading)
            {
                hand.sprite = handOpen;
                _countryIdx = -1;
                _countryPrevIdx = -2;
                _themeIdx = -1;
                _themePrevIdx = -2;
                _percent = 0;

                //로딩서클 0%로 초기화
                _inCircle.fillAmount = 0;

                //외각서클 알파값 비 활성화
                Color tmp = _outCircle.color;
                tmp.a = 0;
                _outCircle.color = tmp;
                _isLoading = false;
            }

        }

    }
}