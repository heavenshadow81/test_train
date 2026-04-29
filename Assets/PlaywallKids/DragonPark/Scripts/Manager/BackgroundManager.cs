using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    public class BackgroundManager : MonoBehaviour
    {
        #region Enums
        public enum Weather
        {
            None,
            Sunny,
            Cloudy,
            Rainy,
            Snowy
        }

        public enum DayTime
        {
            None,
            Day,
            Night
        }
        #endregion

        #region Public variables
        public GameObject rainyPrefab;
        public GameObject snowyPrefab;
        public GameObject starsPrefab;
        public SimpleWeatherInfo sunnyWeatherInfo;
        public SimpleWeatherInfo cloudyWeatherInfo;
        public SimpleWeatherInfo nightWeatherInfo;

        public Light directionalLight;
        public Light evLight;

        public Renderer forestBackground;

        public Camera mainCamera;

        public TweenPosition[] btnTweens;
        #endregion

        #region Properties
        private static BackgroundManager __sharedInstance = null;
        public static BackgroundManager sharedInstance
        {
            get
            {
                return __sharedInstance;
            }
        }

        private Weather _weather = Weather.Sunny;
        public Weather weather
        {
            get
            {
                return _weather;
            }
            set
            {
                if (_weather != value && value != Weather.None)
                {
                    _weather = value;
                    _ApplyWeather();
                    Messenger<Weather>.Broadcast(kEventTypeWeatherChange, weather, MessengerMode.DONT_REQUIRE_LISTENER);
                }
            }
        }

        private DayTime _dayTime = DayTime.Day;
        public DayTime dayTime
        {
            get
            {
                return _dayTime;
            }
            set
            {
                if (_dayTime != value && value != DayTime.None)
                {
                    _dayTime = value;
                    _ApplyDayTime();
                    Messenger<DayTime>.Broadcast(kEventTypeDayTimeChange, dayTime, MessengerMode.DONT_REQUIRE_LISTENER);
                }
            }
        }
        #endregion

        #region Private variables
        private GameObject _currentWeatherEffect;
        private GameObject _currentDayTimeEffect;

        private Color _forestBackgroundMainColor;
        private Color _forestBackgroundSpecularColor;

        private float _currentWeatherUpdateTime = kWeatherUpdateTime;

        private float _currentWeatherTransitionTime = kWeatherTransitionTime;
        #endregion

        #region Constants
        public const float kWeatherUpdateTime = 1200.0f;
        public const float kWeatherTransitionTime = 1.0f;

        public const string kEventTypeWeatherChange = "background_manager_weather_change";
        public const string kEventTypeDayTimeChange = "background_manager_daytime_change";
        #endregion

        public void Start()
        {

            if (sharedInstance)
            {
                Debug.Log("BackgroundManager.Start() : Background Manager is already defined. Removing duplicated...");
                Destroy(gameObject);
            }
            else
            {
                __sharedInstance = this;
                if (forestBackground != null)
                {
                    _forestBackgroundMainColor = forestBackground.material.color;
                    _forestBackgroundSpecularColor = forestBackground.material.GetColor("_SpecColor");
                }
            }


            _ApplyWeather();
        }

        public void OnDestroy()
        {
            Debug.Log("BackgroundManager.OnDestroy()");

            BackgroundManager[] arr = FindObjectsOfType<BackgroundManager>();
            if (arr.Length == 1)
            {
                __sharedInstance = null;
            }
        }

        public void Update()
        {
            if (_currentWeatherUpdateTime >= kWeatherUpdateTime)
            {
                _currentWeatherUpdateTime -= kWeatherUpdateTime;

                _SetDayNight();
                _SetWheather();
            }

            _currentWeatherUpdateTime += Time.deltaTime;
        }

        private void _SetDayNight()
        {
            if (BigboardServer.cachedSituationalInfo.appliesRealtimeDaynight == true)
            {
                System.DateTime dt = System.DateTime.Now;
                if (dt.Hour >= 6 && dt.Hour <= 18)
                {
                    dayTime = DayTime.Day;
                }
                else
                {
                    dayTime = DayTime.Night;
                }
            }
        }

        private void _SetWheather()
        {
            if (BigboardServer.cachedSituationalInfo.appliesRealtimeWeather == true)
            {
                PlayWallDatabase.GetWeatherInfo((newWeather) =>
                {

                    weather = newWeather;
                    Debug.Log("current weather : " + newWeather);
                });
            }
        }

        private void _ApplyWeather()
        {
            if (_currentWeatherEffect != null)
            {
                Destroy(_currentWeatherEffect);
                _currentWeatherEffect = null;
            }

            _ApplyWeatherInfo(GetWeatherInfo());

            GameObject effectPrefab = null;
            if (weather == Weather.Rainy)
            {
                effectPrefab = rainyPrefab;
            }
            else if (weather == Weather.Snowy)
            {
                effectPrefab = snowyPrefab;
            }

            if (effectPrefab != null)
            {
                _currentWeatherEffect = (GameObject)Instantiate(effectPrefab);
                _currentWeatherEffect.SetActive(true);
                _currentWeatherEffect.transform.parent = transform;
                _currentWeatherEffect.transform.localPosition = Vector3.zero;
            }

            _SetWheather();

            _ApplyDayTime();
        }

        private void _ApplyDayTime()
        {
            if (_currentDayTimeEffect != null && dayTime != DayTime.Night && _currentWeatherEffect == null)
            {
                Destroy(_currentDayTimeEffect);
                _currentDayTimeEffect = null;
            }

            GameObject dayTimePrefab = null;
            if (dayTime == DayTime.Night)
            {
                if (forestBackground != null)
                {
                    forestBackground.material.color = Color.black;
                    forestBackground.material.SetColor("_SpecColor", Color.black);
                }
                _ApplyWeatherInfo(nightWeatherInfo);
                if (_currentDayTimeEffect == null && _currentWeatherEffect == null)
                {
                    dayTimePrefab = starsPrefab;
                }
            }
            else
            {
                if (forestBackground != null)
                {
                    forestBackground.material.color = _forestBackgroundMainColor;
                    forestBackground.material.SetColor("_SpecColor", _forestBackgroundSpecularColor);
                }
                _ApplyWeatherInfo(GetWeatherInfo());
            }

            if (dayTimePrefab != null)
            {
                _currentDayTimeEffect = (GameObject)Instantiate(dayTimePrefab);
                _currentDayTimeEffect.SetActive(true);
                _currentDayTimeEffect.transform.parent = transform;
                _currentDayTimeEffect.transform.position = new Vector3(-52.0f, 16.0f, 3.0f);
            }

            _SetDayNight();
        }

        public SimpleWeatherInfo GetWeatherInfo()
        {
            SimpleWeatherInfo info = sunnyWeatherInfo;
            if (weather != Weather.Sunny)
            {
                info = cloudyWeatherInfo;
            }

            return info;
        }

        private void _ApplyWeatherInfo(SimpleWeatherInfo info)
        {
            RenderSettings.fogColor = info.fogColor;
            RenderSettings.fogMode = info.fogMode;
            RenderSettings.fogStartDistance = info.linearStart;
            RenderSettings.fogEndDistance = info.linearEnd;
            RenderSettings.fogDensity = info.density;
            RenderSettings.ambientLight = info.ambientLight;
            if (info.directionalLightIntensity > 0)
            {
                directionalLight.color = info.directionalLight;
                directionalLight.intensity = info.directionalLightIntensity;
            }
            if (info.Ev_LightIntensity > 0)
            {
                evLight.color = info.Ev_Light;
                evLight.intensity = info.Ev_LightIntensity;
            }
            mainCamera.backgroundColor = info.backgroundColor;
        }

        public void OpenWeatherItem()
        {
            if (NGUITools.GetActive(btnTweens[btnTweens.Length - 1].gameObject))
                PlayWeatherItem(false);
            else
                PlayWeatherItem(true);
        }

        public void PlayWeatherItem(bool open)
        {
            if (open)
            {
                List<TweenPosition> listTwn = new List<TweenPosition>();
                for (int i = 0; i < btnTweens.Length - 1; i++)
                {
                    if (btnTweens[i].name.Contains(weather.ToString()) == false)
                        listTwn.Add(btnTweens[i]);
                }
                listTwn.Add(btnTweens[btnTweens.Length - 1]);

                for (int i = 0; i < listTwn.Count; i++)
                {
                    if (i != listTwn.Count - 1)
                        listTwn[i].to = new Vector3(0, -(200 + i * 160), 0);

                    NGUITools.SetActive(listTwn[i].gameObject, true);
                    listTwn[i].onFinished.Clear();
                    listTwn[i].PlayForward();
                }
            }
            else
            {
                for (int i = 0; i < btnTweens.Length; i++)
                    if (NGUITools.GetActive(btnTweens[i].gameObject))
                    {
                        StartCoroutine(Disable(btnTweens[i].gameObject, btnTweens[i].duration));
                        btnTweens[i].PlayReverse();
                    }
            }
        }

        IEnumerator Disable(GameObject go, float time)
        {
            yield return new WaitForSeconds(time);

            NGUITools.SetActive(go, false);
        }


        public void Rainy()
        {
            weather = Weather.Rainy;
            PlayWeatherItem(false);
        }

        public void Cloudy()
        {
            weather = Weather.Cloudy;
            PlayWeatherItem(false);
        }

        public void Snowy()
        {
            weather = Weather.Snowy;
            PlayWeatherItem(false);
        }

        public void Sunny()
        {
            weather = Weather.Sunny;
            PlayWeatherItem(false);
        }

        public void Day()
        {
            dayTime = DayTime.Day;
        }

        public void Night()
        {
            dayTime = DayTime.Night;
        }

        public void DayNight()
        {
            dayTime = (dayTime == DayTime.Day) ? DayTime.Night : DayTime.Day;
        }
    }
}