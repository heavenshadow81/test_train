using UnityEngine;
using System.Collections.Generic;

namespace ML.T_Sports.Common
{
    /// <summary>
    /// 속성 타입
    /// </summary>
    public enum ContentsPropertyType
    {
        /// <summary>
        /// 시간 (seconds)
        /// </summary>
        Time,
        /// <summary>
        /// 기회 (횟수)
        /// </summary>
        Chance,
        /// <summary>
        /// 인원 (명)
        /// </summary>
        Player,
        /// <summary>
        /// 배경음, 0.0~1.0 (%)
        /// </summary>
        BGM,
        /// <summary>
        /// 효과음, 0.0~1.0 (%)
        /// </summary>
        SFX,
        /// <summary>
        /// 0 : 싱글, 1 : 팀
        /// </summary>
        GameMode,
        /// <summary>
        /// 높이 (cm)
        /// </summary>
        Height
    }

    #region Property Data Source
    public class ContentsProperties
    {
        /// <summary>
        /// 속성 - 최소값, 최대값, 초기값, 현재값
        /// </summary>
        public struct Property<T>
        {
            public T min, max, initial, value;
        }

        private Dictionary<ContentsPropertyType, Property<float>> _properties = new Dictionary<ContentsPropertyType, Property<float>>();

        /// <summary>
        /// 속성이 설정되어 있는지 확인한다.
        /// </summary>
        /// <param name="type">확인하려는 속성 타입</param>
        /// <returns>속성 지정 여부</returns>
        public bool HasProperty(ContentsPropertyType type)
        {
            return _properties.ContainsKey(type);
        }

        /// <summary>
        /// 새로운 속성을 설정한다.
        /// </summary>
        /// <param name="newType">추가하려는 속성 타입</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        /// <param name="initialValue">초기값</param>
        public void InitProperty(ContentsPropertyType newType, float min, float max, float initialValue)
        {
            if (!HasProperty(newType))
            {
                Property<float> newProp = new Property<float>();
                newProp.min = min;
                newProp.max = max;
                newProp.initial = newProp.value = initialValue;
                _properties[newType] = newProp;
            }
        }

        /// <summary>
        /// 속성 값을 가져온다.
        /// </summary>
        /// <param name="type">속성 타입</param>
        /// <returns>속성 값 (속성이 존재하지 않을 경우 0)</returns>
        public float GetPropertyValue(ContentsPropertyType type)
        {
            if (HasProperty(type))
                return _properties[type].value;
            return 0;
        }

        /// <summary>
        /// 속성 값을 새로 설정한다.
        /// </summary>
        /// <param name="type">속성 타입</param>
        /// <returns>속성 값 (속성이 존재하지 않을 경우 0)</returns>
        public float SetPropertyValue(ContentsPropertyType type, float newValue)
        {
            if (HasProperty(type))
            {
                var prop = _properties[type];
                prop.value = Mathf.Clamp(newValue, prop.min, prop.max);
                _properties[type] = prop;
                return prop.value;
            }
            return 0;
        }

        /// <summary>
        /// 모든 속성 값을 초기값으로 초기화한다.
        /// </summary>
        public void ResetPropertiesToDefaults()
        {
            var prevProps = new Dictionary<ContentsPropertyType, Property<float>>(_properties);
            var e = prevProps.GetEnumerator();
            while (e.MoveNext())
            {
                var type = e.Current.Key;
                var prop = e.Current.Value;
                prop.value = prop.initial;
                _properties[type] = prop;
            }
        }

        public float this[ContentsPropertyType type]
        {
            get { return GetPropertyValue(type); }
            set { SetPropertyValue(type, value); }
        }

        /// <summary>
        /// 속성 값을 저장한다.
        /// </summary>
        /// <param name="name">구분 이름</param>
        public void SavePropertyValues(string name)
        {
            var e = _properties.GetEnumerator();
            while (e.MoveNext())
            {
                var type = e.Current.Key;
                var prop = e.Current.Value;
                PlayerPrefs.SetFloat(string.Format("{0}_{1}", name, type.ToString()), prop.value);
            }
        }

        /// <summary>
        /// 저장된 모든 속성 값을 불러온다.
        /// </summary>
        /// <param name="name">구분 이름</param>
        public void LoadPropertyValues(string name)
        {
            List<ContentsPropertyType> keys = new List<ContentsPropertyType>(_properties.Keys);
            foreach (var type in keys)
                LoadPropertyValue(name, type);
        }

        /// <summary>
        /// 저장된 속성 값을 불러온다.
        /// </summary>
        /// <param name="name">구분 이름</param>
        /// <param name="type">속성 종류</param>
        public void LoadPropertyValue(string name, ContentsPropertyType type)
        {
            if (_properties.ContainsKey(type))
            {
                var prop = _properties[type];
                prop.value = PlayerPrefs.GetFloat(string.Format("{0}_{1}", name, type.ToString()), prop.initial);
                _properties[type] = prop;
            }
        }
    }
    #endregion

    #region Callback Interface
    public interface IContentsManagerListener
    {
        void OnPlay();
        void OnStop();
        void OnReady();
    }
    #endregion

    #region Base Class
    public class ContentsManagerBase : MonoBehaviour
    {
        #region Properties
        // Manager Name (It should be unique!)
        public virtual string ContentsName
        {
            get
            {
                string className = GetType().Name;
                if (className.EndsWith("Manager"))
                    className = className.Remove(className.Length - 7, 7);
                return className;
            }
        }

        // Singleton Instance
        public static ContentsManagerBase Current { get; private set; }

        // Properties
        private ContentsProperties Properties { get; set; }

        // Shared Properties
        private ContentsProperties SharedProperties { get; set; }
        
        /// <summary>
        /// 현재 플레이 중인지 확인한다. (일시정지하더라도 true)
        /// </summary>
        public virtual bool IsPlaying
        {
            get { return _IsPlaying; }
            protected set
            {
                if (!_IsReady && value)
                {
                    Debug.LogError("준비가 되지 않은 상태에서 게임을 시작할 수 없습니다.");
                    return;
                }

                bool prevValue = _IsPlaying;
                _IsPlaying = value;

                if (prevValue && !_IsPlaying)
                {
                    IsReady = !NeedsReady;
                    for (int i = 0; i < Listeners.Count; i++)
                        Listeners[i].OnStop();
                }
                else if (!prevValue && _IsPlaying)
                {
                    for (int i = 0; i < Listeners.Count; i++)
                        Listeners[i].OnPlay();
                }
            }
        }

        /// <summary>
        /// 일시중지 여부
        /// </summary>
        public virtual bool IsPaused { get; protected set; }

        /// <summary>
        /// 준비상태가 필요한 콘텐츠의 경우 이 값을 true로 오버라이드해야 한다.
        /// </summary>
        public bool NeedsReady { get; protected set; }

        /// <summary>
        /// 준비가 완료되었는지 확인한다. (NeedsReady가 false이면 무조건 true)
        /// </summary>
        public bool IsReady
        {
            get
            {
                if (!NeedsReady)
                    _IsReady = true;
                return _IsReady;
            }
            set
            {
                bool prevValue = _IsReady;
                _IsReady = value;
                if (!NeedsReady)
                {
                    _IsReady = true;
                }
                else if (!prevValue && _IsReady)
                {
                    for (int i = 0; i < Listeners.Count; i++)
                        Listeners[i].OnReady();
                }
            }
        }
        #endregion

        #region Private Variables
        private List<IContentsManagerListener> Listeners;
        private bool _IsPlaying;
        private bool _IsReady;
        #endregion

        #region Base Methods
        public virtual void Awake()
        {
            Current = this;
            Properties = new ContentsProperties();
            Listeners = new List<IContentsManagerListener>();

            if (SharedProperties == null)
            {
                SharedProperties = new ContentsProperties();

                // BGM/SFX
                InitSharedProperty(ContentsPropertyType.BGM, 0, 1, 0.5f);
                InitSharedProperty(ContentsPropertyType.SFX, 0, 1, 0.5f);
            }

            Init();
            Ready();
        }

        public virtual void OnDestroy()
        {
            Current = null;

            Properties.SavePropertyValues(ContentsName);
            SharedProperties.SavePropertyValues("Shared");

            Cleanup();
        }
        #endregion

        #region Property Settings
        public void InitProperty(ContentsPropertyType type, float min, float max, float initialValue)
        {
            Properties.InitProperty(type, min, max, initialValue);
            Properties.LoadPropertyValue(ContentsName, type);
        }

        public void InitSharedProperty(ContentsPropertyType type, float min, float max, float initialValue)
        {
            SharedProperties.InitProperty(type, min, max, initialValue);
            SharedProperties.LoadPropertyValue("Shared", type);
        }

        public void ResetPropertiesToDefaults()
        {
            Dictionary<ContentsPropertyType, float> prevValues = new Dictionary<ContentsPropertyType, float>();
            foreach (ContentsPropertyType propertyType in System.Enum.GetValues(typeof(ContentsPropertyType)))
            {
                if (Properties.HasProperty(propertyType))
                    prevValues[propertyType] = Properties.GetPropertyValue(propertyType);
            }

            Properties.ResetPropertiesToDefaults();

            foreach (ContentsPropertyType propertyType in prevValues.Keys)
            {
                float prevValue = prevValues[propertyType];
                float newValue = Properties.GetPropertyValue(propertyType);
                if (Mathf.Abs(prevValue - newValue) > 0.001f)
                    OnChangePropertyValue(propertyType, prevValue, newValue);
            }
        }

        public void ResetSharedPropertiesToDefaults()
        {
            Dictionary<ContentsPropertyType, float> prevValues = new Dictionary<ContentsPropertyType, float>();
            foreach (ContentsPropertyType propertyType in System.Enum.GetValues(typeof(ContentsPropertyType)))
            {
                if (SharedProperties.HasProperty(propertyType))
                    prevValues[propertyType] = SharedProperties.GetPropertyValue(propertyType);
            }

            SharedProperties.ResetPropertiesToDefaults();

            foreach (ContentsPropertyType propertyType in prevValues.Keys)
            {
                float prevValue = prevValues[propertyType];
                float newValue = SharedProperties.GetPropertyValue(propertyType);
                if (Mathf.Abs(prevValue - newValue) > 0.001f)
                    OnChangeSharedPropertyValue(propertyType, prevValue, newValue);
            }
        }

        public bool HasProperty(ContentsPropertyType type)
        {
            return Properties.HasProperty(type);
        }

        public bool HasSharedProperty(ContentsPropertyType type)
        {
            return SharedProperties.HasProperty(type);
        }

        public float GetPropertyValue(ContentsPropertyType type)
        {
            return Properties.GetPropertyValue(type);
        }

        public float GetPropertyValueFloat(ContentsPropertyType type)
        {
            return Properties.GetPropertyValue(type);
        }

        public int GetPropertyValueInt(ContentsPropertyType type)
        {
            return Mathf.RoundToInt(Properties.GetPropertyValue(type) + 0.001f);
        }

        public float GetSharedPropertyValue(ContentsPropertyType type)
        {
            return SharedProperties.GetPropertyValue(type);
        }

        public float GetSharedPropertyValueFloat(ContentsPropertyType type)
        {
            return SharedProperties.GetPropertyValue(type);
        }

        public float GetSharedPropertyValueInt(ContentsPropertyType type)
        {
            return Mathf.RoundToInt(SharedProperties.GetPropertyValue(type) + 0.001f);
        }

        public void SetPropertyValue(ContentsPropertyType type, float newValue)
        {
            bool hasProperty = Properties.HasProperty(type);
            if (hasProperty)
            {
                float prevValue = Properties.GetPropertyValue(type);
                float newValueClamped = Properties.SetPropertyValue(type, newValue);
                if (Mathf.Abs(prevValue - newValueClamped) > 0.001f)
                    OnChangePropertyValue(type, prevValue, newValueClamped);
            }
        }

        public void SetSharedPropertyValue(ContentsPropertyType type, float newValue)
        {
            bool hasProperty = SharedProperties.HasProperty(type);
            if (hasProperty)
            {
                float prevValue = SharedProperties.GetPropertyValue(type);
                float newValueClamped = SharedProperties.SetPropertyValue(type, newValue);
                if (Mathf.Abs(prevValue - newValueClamped) > 0.001f)
                    OnChangeSharedPropertyValue(type, prevValue, newValueClamped);
            }
        }
        #endregion

        #region Game Logic
        public virtual void Init()
        {
            // 콘텐츠 초기화, 서브클래스에서 수행.
        }

        public virtual void Play()
        {
            /*
             * 서브클래스에서 다음과 같이 오버라이드
             * if (!IsPlaying) {
             *     base.Play();
             *     // 필요한 행동
             * }
             */
            IsPlaying = true;
        }

        public virtual void Stop()
        {
            /*
             * 서브클래스에서 다음과 같이 오버라이드
             * if (IsPlaying) {
             *     // 필요한 행동
             *     base.Stop();
             * }
             */
            IsPlaying = false;
        }

        public virtual void Pause()
        {
            /* 콘텐츠 일시정지 혹은 재개. 오버라이드할 때 IsPaused 값을 이용하여 필요한 기능 구현 */
            IsPaused = !IsPaused;
            Time.timeScale = IsPaused ? 0.0f : 1.0f;
        }

        public virtual void Cleanup()
        {
            // 콘텐츠 종료 전 정리, 서브클래스에서 수행.
        }

        public virtual void Ready()
        {
            /*
             * 서브클래스에서 다음과 같이 오버라이드
             * base.Reset();
             * // 필요한 행동
             */
            IsReady = true;
        }
        #endregion

        #region Delegates
        public virtual void OnChangePropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            // 속성 값이 바뀌었을 경우 호출됨. 서브클래스가 알아서 처리.
        }

        public virtual void OnChangeSharedPropertyValue(ContentsPropertyType type, float prevValue, float newValue)
        {
            // 속성 (공용) 값이 바뀌었을 경우 호출됨. 서브클래스가 알아서 처리.
        }
        #endregion

        #region Listeners
        public void AddListener(IContentsManagerListener listener)
        {
            if (!Listeners.Contains(listener))
                Listeners.Add(listener);
        }

        public void RemoveListener(IContentsManagerListener listener)
        {
            Listeners.Remove(listener);
        }
        #endregion
    }
    #endregion
}