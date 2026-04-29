using System.Collections.Generic;
using UnityEngine;

namespace ML.MLBKids
{
    public class DelayedCall : MonoBehaviour
    {
        private class _CallInfo
        {
            public string id;
            public float time, currentTime;
            public System.Action<bool> handler;
        }

        private static DelayedCall _instance;
        private static DelayedCall instance
        {
            get
            {
                if (_instance == null && _isAvailable)
                {
                    GameObject go = new GameObject("_delayed_call");
                    go.hideFlags = HideFlags.HideInHierarchy;
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<DelayedCall>();
                }
                return _instance;
            }
        }

        private static bool _isAvailable = true;

        private Dictionary<string, _CallInfo> _calls = new Dictionary<string, _CallInfo>();

        #region Unity methods
        public void Start()
        {
            if (_instance != this)
            {
               Destroy(this);
            }
        }

        public void Update()
        {
            var keys = new List<string>(_calls.Keys);

            foreach (var key in keys)
            {
                var info = _calls[key];
                info.currentTime += Time.unscaledDeltaTime;
                if (info.currentTime >= info.time)
                {
                    if (info.handler != null)
                        info.handler(true);
                    _calls.Remove(key);
                }
            }
        }

        public void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                _isAvailable = false;
            }
        }
        #endregion

        public static void Begin(string id, float time, System.Action<bool> handler)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("id is null or empty!");
                return;
            }

            var calls = instance._calls;
            if (calls.ContainsKey(id))
            {
                Stop(id);
            }

            _CallInfo info = new _CallInfo();
            info.id = id;
            info.time = time;
            info.handler = handler;
            calls.Add(id, info);
        }

        public static bool IsWaiting(string id)
        {
            return id != null && instance._calls.ContainsKey(id);
        }

        public static void Stop(string id, bool callHandler = false)
        {

            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("id is null or empty!");
                return;
            }

            _CallInfo info = null;
            var calls = instance._calls;
            if (calls.TryGetValue(id, out info))
            {
                if (callHandler && info.handler != null)
                    info.handler(false);
                calls.Remove(id);
            }
        }
    }
}