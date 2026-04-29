using System;
using UnityEngine;
using UnityEngine.UI;

namespace ML.MapoContents.WorldTravel
{
    public class RegionTimeUI : MonoBehaviour
    {
        public GlobalTime globalTime;
        public TimeZone region;
        public Text regionName, regionTime, localTime;

        string country;
        void Awake()
        {
            country = "Korea";
           /* DateTime rt = GetTimeZone(country);
            Debug.Log(country + "시간은 : " + rt.ToString("HH : mm"));

            country = "China";
            rt = GetTimeZone(country);
            Debug.Log(country + "시간은 : " + rt.ToString("HH : mm"));

            country = "Italy";
            rt = GetTimeZone(country);
            Debug.Log(country + "시간은 : " + rt.ToString("HH : mm"));

            country = "France";
            rt = GetTimeZone(country);
            Debug.Log(country + "시간은 : " + rt.ToString("HH : mm"));

            country = "England";
            rt = GetTimeZone(country);
            Debug.Log(country + "시간은 : " + rt.ToString("HH : mm"));

            country = "USA";
            rt = GetTimeZone(country);
            Debug.Log(country + "시간은 : " + rt.ToString("HH : mm"));

            country = "Greece";
            rt = GetTimeZone(country);
            Debug.Log(country + "시간은 : " + rt.ToString("HH : mm"));

            country = "Egypt";
            rt = GetTimeZone(country);
            Debug.Log(country + "시간은 : " + rt.ToString("HH : mm"));

            country = "Japan";
            rt = GetTimeZone(country);
            Debug.Log(country + "시간은 : " + rt.ToString("HH : mm"));

            country = "Turkey";
            rt = GetTimeZone(country);
            Debug.Log(country + "시간은 : " + rt.ToString("HH : mm"));

            country = "UAE";
            rt = GetTimeZone(country);
            Debug.Log(country + "시간은 : " + rt.ToString("HH : mm"));

            country = "Cambodia";
            rt = GetTimeZone(country);
            Debug.Log(country + "시간은 : " + rt.ToString("HH : mm"));

            country = "India";
            rt = GetTimeZone(country);
            Debug.Log(country + "시간은 : " + rt.ToString("HH : mm"));*/
        }
        public void Start()
        {
            globalTime.onUpdate += _OnUpdate;
            //_OnUpdate();
        }

        private void _OnUpdate()
        {
            //DateTime rt = globalTime.GetGlobalTimeNow(region);
           // DateTime lt = globalTime.GetGlobalTimeNow(TimeZone.Korea);
            //localTime.text = string.Format("{0}", lt.ToString("HH : mm"));
        }

        public Text Photo_Time;
        void Update()
        {
            DateTime rt = GetTimeZone(country);
            string ampm = "am";
            int rthour = rt.Hour;
            if (rthour > 12)
            {
                rthour -= 12;
                ampm = "PM.";
            }
            else
                ampm = "AM.";

            string hour_rt;
            string min_rt;
            if (rthour < 10)
                hour_rt = "0" + rthour.ToString();
            else
                hour_rt = rthour.ToString();

            if (rt.Minute < 10)
                min_rt = "0" + rt.Minute.ToString();
            else
                min_rt = rt.Minute.ToString();


            string region = ampm + rthour + ":"+ min_rt;       
            regionTime.text = region;//string.Format("{0}{1}{3}", ampm, rthour, rt.Minute);
            Photo_Time.text = region;

            DateTime lt = globalTime.GetGlobalTimeNow(TimeZone.Korea);
            ampm = "am";
            int lthour = lt.Hour;
            if (lthour > 12)
            {
                lthour -= 12;
                ampm = "PM.";
            }
            else
                ampm = "AM.";

            string hour_reg;
            string min_reg;
            if (lthour < 10)
                hour_reg = "0" + lthour.ToString();
            else
                hour_reg = lthour.ToString();

            if (lt.Minute < 10)
                min_reg = "0" + lt.Minute.ToString();
            else
                min_reg = lt.Minute.ToString();

            string local = ampm + hour_reg + ":" + min_reg;
            localTime.text = local;// string.Format("{0}{1}{2}", ampm, lthour, lt.Minute );// lt.ToString("HH : mm")
        }

        public void SetTimeZone(string count)
        {
            country = count;
            //DateTime rt = GetTimeZone(country);
            //regionTime.text = string.Format("{0}", rt.ToString("HH : mm"));
        }

        DateTime GetTimeZone(string Country)
        {
            DateTime _time = globalTime.GetGlobalTimeNow(TimeZone.Utc);
            switch (Country)
            {
                case "Korea":
                    _time = globalTime.GetGlobalTimeNow(TimeZone.Korea);
                    break;
                case "China":
                    _time = globalTime.GetGlobalTimeNow(TimeZone.China);
                    break;
                case "Italy":
                    _time = globalTime.GetGlobalTimeNow(TimeZone.Italy);
                    break;
                case "France":
                    _time = globalTime.GetGlobalTimeNow(TimeZone.France);
                    break;
                case "England":
                    _time = globalTime.GetGlobalTimeNow(TimeZone.England);
                    break;
                case "USA":
                    _time = globalTime.GetGlobalTimeNow(TimeZone.USA);
                    break;
                case "Greece":
                    _time = globalTime.GetGlobalTimeNow(TimeZone.Greece);
                    break;
                case "Egypt":
                    _time = globalTime.GetGlobalTimeNow(TimeZone.Egypt);
                    break;
                case "Japan":
                    _time = globalTime.GetGlobalTimeNow(TimeZone.Japan);
                    break;
                case "Turkey":
                    _time = globalTime.GetGlobalTimeNow(TimeZone.Turkey);
                    break;
                case "UAE":
                    _time = globalTime.GetGlobalTimeNow(TimeZone.UAE);
                    break;
                case "Cambodia":
                    _time = globalTime.GetGlobalTimeNow(TimeZone.Cambodia);
                    break;
                case "India":
                    _time = globalTime.GetGlobalTimeNow(TimeZone.India);
                    break;
            }
            return _time;
        }
    }
}