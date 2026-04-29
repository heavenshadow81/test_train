using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ContentsSIModelingView : MonoBehaviour
{
    #region Private variables
    private ContentsSIModelingInfo _info;
    #endregion

    #region Private variables
    private List<UIToggle> _localization = new List<UIToggle>();
    private List<UIToggle> _themes = new List<UIToggle>();
    private List<UIToggle> _targets = new List<UIToggle>();
    private List<UIToggle> _contentsTypes = new List<UIToggle>();
    private List<UIToggle> _times = new List<UIToggle>();
    private List<UIToggle> _weathers = new List<UIToggle>();

    private bool _initialized = false;

    #endregion

    public void Start()
    {
        _info = BigboardServer.cachedSituationalInfo;

        _Init();
        Refresh();
    }

    IEnumerator RefreshCoroutine()
    {
        yield return new WaitForEndOfFrame();
        Refresh();
    }

    private void _Init()
    {
        var toggles = UIToggle.list;

        for (int i = 0; i < toggles.size; i++)
        {
            UIToggle t = toggles[i];
            switch (t.group)
            {
                case 180:
                    _localization.Add(t);
                    break;
                case 181:
                    _themes.Add(t);
                    //EventDelegate.Add(_themes[_themes.Count - 1].onChange, ThemeToggleActive);
                    break;
                case 182:
                    _targets.Add(t);
                    break;
                case 183:
                    _contentsTypes.Add(t);
                    break;
                case 184:
                    _times.Add(t);
                    break;
                case 185:
                    _weathers.Add(t);
                    break;
            }
        }

        _localization.Sort((a, b) =>
        {
            return a.name.CompareTo(b.name);
        });
        _themes.Sort((a, b) =>
        {
            return a.name.CompareTo(b.name);
        });
        _targets.Sort((a, b) =>
        {
            return a.name.CompareTo(b.name);
        });
        _contentsTypes.Sort((a, b) =>
        {
            return a.name.CompareTo(b.name);
        });
        _times.Sort((a, b) =>
        {
            return a.name.CompareTo(b.name);
        });
        _weathers.Sort((a, b) =>
        {
            return a.name.CompareTo(b.name);
        });

        _initialized = true;
    }

    public void Refresh()
    {
        /*
        int themeIdx = 0, targetIdx = 0, contentsTypeIdx = 0;
        if (!string.IsNullOrEmpty(_info._theme)) int.TryParse(_info._theme.Substring(_info._theme.Length - 1, 1), out themeIdx);
        if (!string.IsNullOrEmpty(_info._target)) int.TryParse(_info._target.Substring(_info._target.Length - 1, 1), out targetIdx);
        if (!string.IsNullOrEmpty(_info._contentsType)) int.TryParse(_info._contentsType.Substring(_info._contentsType.Length - 1, 1), out contentsTypeIdx);

        if(themeIdx > 0) _themes[Mathf.Min(_themes.Count - 1, themeIdx - 1)].Set(true);
        if(targetIdx > 0) _targets[Mathf.Min(_targets.Count - 1, targetIdx - 1)].Set(true);
        if(contentsTypeIdx > 0) _contentsTypes[Mathf.Min(_contentsTypes.Count - 1, contentsTypeIdx - 1)].Set(true);
        _times[_info._appliesRealtimeDaynight ? 1 : 0].Set(true);
        _weathers[_info._appliesRealtimeWeather ? 1 : 0].Set(true);
        */

        _localization[(int)_info.localization].Set(true);
        _themes[(int)_info.theme].Set(true);
        _targets[(int)_info.targetOfAge].Set(true);
        _contentsTypes[(int)_info.contentsType].Set(true);

        _times[_info.appliesRealtimeDaynight ? 1 : 0].Set(true);
        _weathers[_info.appliesRealtimeWeather ? 1 : 0].Set(true);

        RefreshThemeAndContentsType();
    }

    void RefreshThemeAndContentsType()
    {
        if (BigboardServerDataManager.IsContentsTypeWorking(ContentsSIModelingInfo.StateContentsType.Competition ))
        {
            for (int i = 0; i < _contentsTypes.Count; i++)
            {
                _contentsTypes[i].enabled = true;

                UISprite sprite = _contentsTypes[i].GetComponent<UISprite>();
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
                sprite.GetComponentInChildren<UILabel>().color = Color.black;
            }
        }
        else
        {
            for (int i = 1; i < _contentsTypes.Count; i++)
            {
                _contentsTypes[i].enabled = false;
                _contentsTypes[i].Set(false);

                UISprite sprite = _contentsTypes[i].GetComponent<UISprite>();
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.6f);
                sprite.GetComponentInChildren<UILabel>().color = new Color(0f, 0f, 0f, 0.6f);
            }
            _contentsTypes[0].Set(true);
        }
    }

    public void SetLanguage()
    {
        BigboardAlertPanel.sharedInstance.Loading(true);
        Save();
#if BIGBOARD_STANDALONE
        BigboardServer.StandaloneDataReload();
#endif
        LocalizationManager.AllReplace();
        BigboardAlertPanel.sharedInstance.Loading(false);
    }

    public void ThemeToggleActive()
    {
        if (UIToggle.current.value)
        {
            Save();
            StartCoroutine( RefreshThemeAndContentsTypeCoroutine());
        }
    }

    IEnumerator RefreshThemeAndContentsTypeCoroutine()
    {
        yield return new WaitForEndOfFrame();
        RefreshThemeAndContentsType();
    }

    public void OnDisable()
    {
        StopAllCoroutines();

        Save();
    }
    

    public void Save()
    {
        _info.localization = (LocalizationLanguage)GetToggleActiveIndex(_localization); //LocalizationLanguage;
        _info.theme = (ContentsSIModelingInfo.StateTheme)GetToggleActiveIndex(_themes); //toggle.name;
        _info.targetOfAge = (ContentsSIModelingInfo.StateTargetOfAge)GetToggleActiveIndex(_targets); //toggle.name;
        _info.contentsType = (ContentsSIModelingInfo.StateContentsType)GetToggleActiveIndex(_contentsTypes); //toggle.name;
        _info.appliesRealtimeDaynight = GetToggleActiveIndex(_times) == 1;
        _info.appliesRealtimeWeather = GetToggleActiveIndex(_weathers) == 1;
        
        BigboardServer.SetState(_info, null);
    }

    private int GetToggleActiveIndex(List<UIToggle> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var toggle = list[i];
            if (toggle.value)
            {
                return i;
            }
        }
        Debug.LogError("Toggle Out of range");
        return -1;
    }
}
