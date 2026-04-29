using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace ML.MapoContents.WorldTravel
{
    public enum TimeZone
    {
        Utc,
        Korea,
        China,
        Italy,
        France,
        England,
        USA,
        Greece,
        Egypt,
        Japan,
        Turkey,
        UAE,
        Cambodia,
        India
    }

    public class GlobalTime : MonoBehaviour
    {
        public event Action onUpdate;
        private Coroutine _coroutine;
        private Dictionary<TimeZone, DateTime> _globalTimeDict = new Dictionary<TimeZone, DateTime>();

        public void Start()
        {
            _coroutine = StartCoroutine(_UpdateGlobalTime());
        }

        public void OnDestroy()
        {
            StopCoroutine(_coroutine);
        }

        public DateTime GetGlobalTimeNow(TimeZone timeZone)
        {
            DateTime dt = DateTime.UtcNow;
            _globalTimeDict.TryGetValue(timeZone, out dt);
            return dt;
        }

        private IEnumerator _UpdateGlobalTime()
        {
            // initialize time zones
            DateTime dt = DateTime.UtcNow;
            _globalTimeDict[TimeZone.Utc] = dt;

            DateTime kr = dt.AddHours(9);
            _globalTimeDict[TimeZone.Korea] = kr;
            _globalTimeDict[TimeZone.Japan] = kr;

            DateTime cn = dt.AddHours(8);
            _globalTimeDict[TimeZone.China] = cn;

            DateTime italy = dt.AddHours(2);
            _globalTimeDict[TimeZone.Italy] = italy;

            DateTime fr = dt.AddHours(1);
            _globalTimeDict[TimeZone.France] = fr;
            _globalTimeDict[TimeZone.England] = fr;
            _globalTimeDict[TimeZone.Egypt] = fr;

            DateTime harvard = dt.AddHours(-4);
            _globalTimeDict[TimeZone.USA] = harvard;

            DateTime greece = dt.AddHours(3);
            _globalTimeDict[TimeZone.USA] = greece;
            _globalTimeDict[TimeZone.Turkey] = greece;
            _globalTimeDict[TimeZone.Greece] = greece;

            DateTime uae = dt.AddHours(4);
            _globalTimeDict[TimeZone.UAE] = uae;

            DateTime cmabo = dt.AddHours(7);
            _globalTimeDict[TimeZone.Cambodia] = cmabo;

            DateTime india = dt.AddHours(5);
            india = dt.AddMinutes(30);
            _globalTimeDict[TimeZone.India] = india;

            if (onUpdate != null)
                onUpdate();

            // Add time every second.
            List<TimeZone> keys = new List<TimeZone>(_globalTimeDict.Keys);
            while (true)
            {
                yield return new WaitForSecondsRealtime(1.0f);
                DateTime dt2 = DateTime.UtcNow;
                double sec = (dt2 - dt).TotalSeconds;
                dt = dt2;
                foreach (var key in keys)
                {
                    var val = _globalTimeDict[key];
                    _globalTimeDict[key] = val.AddSeconds(sec);
                }
                if (onUpdate != null)
                    onUpdate();
            }
        }
    }
}