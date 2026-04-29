using System;
using System.Collections.Generic;

/// <summary>
/// The situational information modeling settings.
/// </summary>
public class ContentsSIModelingInfo
{
    public enum StateTheme { None, SketchBook, PlayingJump, Creative, Avatars, WithCharacter }
    public enum StateTargetOfAge { None, Child, School }
    public enum StateContentsType { None, Competition, Cooperation }


    #region Public variables
    private LocalizationLanguage _localization;
    public LocalizationLanguage localization
    {
        get
        {
            return _localization;
        }
        set
        {
            _localization = (value);
        }
    }

    private string _theme;
    public StateTheme theme
    {
        get
        {
            return GetThemeToEnum(_theme);
        }
        set
        {
            _theme = GetThemeToString(value);
        }
    }

    private string _targetOfAge;
    public StateTargetOfAge targetOfAge
    {
        get
        {
            return GetTargetToEnum(_targetOfAge);
        }
        set
        {
            _targetOfAge = GetTargetToString(value);
        }
    }

    private string _contentsType;
    public StateContentsType contentsType
    {
        get
        {
            return GetContentsTypeToEnum(_contentsType);
        }
        set
        {
            _contentsType = GetContentsTypeToString(value);
        }
    }

    private bool _appliesRealtimeDaynight;
    public bool appliesRealtimeDaynight
    {
        get
        {
            return _appliesRealtimeDaynight;
        }
        set
        {
            _appliesRealtimeDaynight = value;
        }
    }

    private bool _appliesRealtimeWeather;
    public bool appliesRealtimeWeather
    {
        get
        {
            return _appliesRealtimeWeather;
        }
        set
        {
            _appliesRealtimeWeather = value;
        }
    }
    #endregion

    public ContentsSIModelingInfo()
    {
        _localization = LocalizationLanguage.Korean;
        _theme = "CC0401";
        _targetOfAge = "CC0501";
        _contentsType = "CC0601";
        _appliesRealtimeDaynight = false;
        _appliesRealtimeWeather = false;
    }

    public ContentsSIModelingInfo Clone()
    {
        return (ContentsSIModelingInfo)MemberwiseClone();
    }

    public Dictionary<string, object> GetJsonDictionary()
    {
        Dictionary<string, object> paramDict = new Dictionary<string, object>();

        paramDict["localization"] = _localization.ToString();
        if (!string.IsNullOrEmpty(_theme)) paramDict["theme"] = _theme;
        if (!string.IsNullOrEmpty(_targetOfAge)) paramDict["target"] = _targetOfAge;
        if (!string.IsNullOrEmpty(_contentsType)) paramDict["contents_type"] = _contentsType;
        paramDict["realtime_time_yn"] = _appliesRealtimeDaynight ? "Y" : "N";
        paramDict["realtime_weather_yn"] = _appliesRealtimeWeather ? "Y" : "N";

        return paramDict;
    }

    public void ReadJsond(Dictionary<string, object> state)
    {
        if (state != null)
        {
            if (state.ContainsKey("localization") && state["localization"] != null)
                _localization = (LocalizationLanguage)Enum.Parse(typeof(LocalizationLanguage), state["localization"].ToString());                
            if (state.ContainsKey("theme") && state["theme"] != null)
                _theme = state["theme"].ToString();
            if (state.ContainsKey("target") && state["target"] != null)
                _targetOfAge = state["target"].ToString();
            if (state.ContainsKey("contents_type") && state["contents_type"] != null)
                _contentsType = state["contents_type"].ToString();
            if (state.ContainsKey("realtime_time_yn") && state["realtime_time_yn"] != null)
                _appliesRealtimeDaynight = state["realtime_time_yn"].ToString().ToLower().Equals("y");
            if (state.ContainsKey("realtime_weather_yn") && state["realtime_weather_yn"] != null)
                _appliesRealtimeWeather = state["realtime_weather_yn"].ToString().ToLower().Equals("y");
        }
    }

    private string GetThemeToString(StateTheme state)
    {
        return string.Format("CC04{0:D2}", (int)state + 1);
    }

    private StateTheme GetThemeToEnum(string state)
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(StateTheme)).Length; i++)
            if (GetThemeToString((StateTheme)i).Equals(state))
                return (StateTheme)i;
        return StateTheme.None;
    }

    private string GetTargetToString(StateTargetOfAge state)
    {
        return string.Format("CC05{0:D2}", (int)state + 1);
    }
    private StateTargetOfAge GetTargetToEnum(string state)
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(StateTargetOfAge)).Length; i++)
            if (GetTargetToString((StateTargetOfAge)i).Equals(state))
                return (StateTargetOfAge)i;
        return StateTargetOfAge.None;
    }

    private string GetContentsTypeToString(StateContentsType state)
    {
        return string.Format("CC06{0:D2}", (int)state + 1);
    }
    private StateContentsType GetContentsTypeToEnum(string state)
    {
        for (int i = 0; i < System.Enum.GetValues(typeof(StateContentsType)).Length; i++)
            if (GetContentsTypeToString((StateContentsType)i).Equals(state))
                return (StateContentsType)i;
        return StateContentsType.None;
    }

}