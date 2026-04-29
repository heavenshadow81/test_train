using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//날씨 바꾸는 스크립트
public class ChangeWeather : MonoBehaviour
{
    #region 변수
    [SerializeField]
    ParticleSystem[] weather;
    [SerializeField]
    Light sunlight;
    //날씨를 얼마나 자주 바꿀 것인가?
    [SerializeField]
    [Tooltip("날씨를 얼마나 자주(초) 바꿀 것인가?")]
    int changeDelay;
    [SerializeField]
    [Range(0, 2)]
    int weatherparameter;
    public int Weather { 
        get => weatherparameter;
        set
        {
            weatherparameter = value;
            ChangeW();
        }
    }
    #endregion
    //
    #region 유니티 함수
    private void OnEnable()
    {
        weatherparameter = Random.Range(0, weather.Length);
        for(int i = 0; i<weather.Length; i++)
        {
            if (i == weatherparameter)
            {
                weather[i].Play();
            }
            else
            {
                weather[i].Stop();
            }
        }
        sunlight.intensity = weatherparameter == 0 ? 1 : 0.8f;
        StartCoroutine(WeatherChange());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    #endregion
    #region 함수
    IEnumerator WeatherChange()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeDelay);
            weatherparameter = Random.Range(0, weather.Length);
            for (int i = 0; i < weather.Length; i++)
            {
                if (i == weatherparameter)
                {
                    weather[i].Play();
                }
                else
                {
                    weather[i].Stop();
                }
            }
            sunlight.intensity = weatherparameter == 0 ? 1 : 0.8f;
        }
    }
    void ChangeW()
    {
        for (int i = 0; i < weather.Length; i++)
        {
            if (i == weatherparameter)
            {
                weather[i].Play();
            }
            else
            {
                weather[i].Stop();
            }
        }
        //sunlight.intensity = weatherparameter == 0 ? 1 : 0.8f;
    }
    private void OnValidate()
    {
        ChangeW();
    }
    #endregion
}
