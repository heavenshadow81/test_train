using UnityEngine;
using System.Collections.Generic;
using System.IO;
using ML.PlaywallKids.Common;

/// <summary>
/// Settings manager.  <br>
/// Loads settings value from "settings.txt".
/// </summary>
public class SettingsManager
{
    #region Properties
    /// <summary>
    /// Returns sync interface IP.
    /// </summary>
    /// <value>The sync interface I.</value>
    public static string syncInterfaceIP
    {
        get
        {
            Load();

            string ip = "127.0.0.1";

            if (_jsonDict.ContainsKey(kKeyNameSyncInterfaceIP) && _jsonDict[kKeyNameSyncInterfaceIP] != null)
            {
                ip = _jsonDict[kKeyNameSyncInterfaceIP].ToString();
            }
            else
            {
                _jsonDict[kKeyNameSyncInterfaceIP] = ip;
                Save();
            }

            return ip;
        }
    }

    /// <summary>
    /// Main port number.
    /// </summary>
    public static int port
    {
        get
        {
            Load();

            int port = 5000;

            if (_jsonDict.ContainsKey(kKeyNamePort) && _jsonDict[kKeyNamePort] != null)
            {
                int.TryParse(_jsonDict[kKeyNamePort].ToString(), out port);
            }
            else
            {
                _jsonDict[kKeyNamePort] = port;
                Save();
            }

            return port;
        }
    }

    /// <summary>
    /// Is client single mode?
    /// </summary>
    public static bool singleMode
    {
        get
        {
            Load();

            bool singleMode = true;

            if (_jsonDict.ContainsKey(kKeyNameSingleMode) && _jsonDict[kKeyNameSingleMode] != null)
            {
                bool.TryParse(_jsonDict[kKeyNameSingleMode].ToString(), out singleMode);
            }
            else
            {
                _jsonDict[kKeyNameSingleMode] = singleMode;
                Save();
            }

            return singleMode;
        }
    }

    /// <summary>
    /// Enables or disables auto-play in two dimension.
    /// </summary>
    public static bool enablesAutoPlay2D
    {
        get
        {
            Load();

            bool flag = false;

            if (_jsonDict.ContainsKey(kKeyNameEnablesAutoPlay2D) && _jsonDict[kKeyNameEnablesAutoPlay2D] != null)
                bool.TryParse(_jsonDict[kKeyNameEnablesAutoPlay2D].ToString(), out flag);
            else
                enablesAutoPlay2D = flag;

            return flag;
        }
        set
        {
            _jsonDict[kKeyNameEnablesAutoPlay2D] = value;
            Save();
        }
    }

    /// <summary>
    /// Gets or sets play time (minute) in two dimension (only affects auto-play mode).
    /// </summary>
    public static float autoPlayMinute2D
    {
        get
        {
            Load();

            float time = 10.0f;
            if (_jsonDict.ContainsKey(kKeyNameAutoPlayMinute2D) && _jsonDict[kKeyNameAutoPlayMinute2D] != null)
                float.TryParse(_jsonDict[kKeyNameAutoPlayMinute2D].ToString(), out time);
            else
                autoPlayMinute2D = time;

            return time;
        }
        set
        {
            _jsonDict[kKeyNameAutoPlayMinute2D] = value;
            Save();
        }
    }


    /// <summary>
    /// Enables or disables auto-play in interaction.
    /// </summary>
    public static bool enablesAutoPlayInteraction
    {
        get
        {
            Load();

            bool flag = false;

            if (_jsonDict.ContainsKey(kKeyNameEnablesAutoPlayInteraction) && _jsonDict[kKeyNameEnablesAutoPlayInteraction] != null)
                bool.TryParse(_jsonDict[kKeyNameEnablesAutoPlayInteraction].ToString(), out flag);
            else
                enablesAutoPlayInteraction = flag;

            return flag;
        }
        set
        {
            _jsonDict[kKeyNameEnablesAutoPlayInteraction] = value;
            Save();
        }
    }

    /// <summary>
    /// Gets or sets play time (minute) in interaction (only affects auto-play mode).
    /// </summary>
    public static float autoPlayMinuteInteraction
    {
        get
        {
            Load();

            float time = 10.0f;
            if (_jsonDict.ContainsKey(kKeyNameAutoPlayMinuteInteraction) && _jsonDict[kKeyNameAutoPlayMinuteInteraction] != null)
                float.TryParse(_jsonDict[kKeyNameAutoPlayMinuteInteraction].ToString(), out time);
            else
                autoPlayMinuteInteraction = time;

            return time;
        }
        set
        {
            _jsonDict[kKeyNameAutoPlayMinuteInteraction] = value;
            Save();
        }
    }

    public static string ftpAddress
    {
        get
        {
            Load();
            string ip = "127.0.0.1";
            if (_jsonDict.ContainsKey(kKeyNameFTPAddress) && _jsonDict[kKeyNameFTPAddress] != null)
            {
                ip = _jsonDict[kKeyNameFTPAddress].ToString();
            }
            else
            {
                _jsonDict[kKeyNameFTPAddress] = ip;
                Save();
            }
            return ip;
        }
    }

    public static string ftpUsername
    {
        get
        {
            Load();
            string username = "anonymous";
            if (_jsonDict.ContainsKey(kKeyNameFTPUsername) && _jsonDict[kKeyNameFTPUsername] != null)
            {
                username = _jsonDict[kKeyNameFTPUsername].ToString();
            }
            else
            {
                _jsonDict[kKeyNameFTPUsername] = username;
                Save();
            }
            return username;
        }
    }

    public static string ftpPassword
    {
        get
        {
            Load();
            string password = "";
            if (_jsonDict.ContainsKey(kKeyNameFTPPassword) && _jsonDict[kKeyNameFTPPassword] != null)
            {
                password = _jsonDict[kKeyNameFTPPassword].ToString();
            }
            else
            {
                _jsonDict[kKeyNameFTPPassword] = password;
                Save();
            }
            return password;
        }
    }

    public static void SaveScenario(int idx)
    {
        _Load();
        //_jsonDict.Add("scenario", idx);
        string tt = MiniJSON.Json.Serialize(_jsonDict);
    }

    public static string GetScreenShotPath()
    {
        string path_ = "";
        _Load();

        if (_jsonDict.ContainsKey(KEY_SCREENSHOT_PATH) && _jsonDict[KEY_SCREENSHOT_PATH] != null)
        {
            _jsonDict[KEY_SCREENSHOT_PATH].ToString();
            Debug.Log(" set new path0");
        }
        else
        {
            if (_jsonDict.ContainsKey(KEY_SCREENSHOT_PATH))
            {
                Debug.Log(" set new path");
                _jsonDict[KEY_SCREENSHOT_PATH] = "C:/Users/Convensia2/AppData/Roaming/Mogencelab/ScreenShot";
            }
            else
            {
                Debug.Log(" set new path2222");
                _jsonDict.Add(KEY_SCREENSHOT_PATH, "C:/Users/Convensia2/AppData/Roaming/Mogencelab/ScreenShot");
            }

            _Save();
            return "C:/Users/Convensia2/AppData/Roaming/Mogencelab/ScreenShot";
        }
        return path_;
    }


    public static int Width
    {
        get
        {
            _Load();
            int w = 3840;

            if (_jsonDict.ContainsKey(KEY_SCREEN_WIDTH) && _jsonDict[KEY_SCREEN_WIDTH] != null)
            {
                int.TryParse(_jsonDict[KEY_SCREEN_WIDTH].ToString(), out w);
            }
            else
            {
                _jsonDict[KEY_SCREEN_WIDTH] = w;
                _Save();
            }
            return w;
        }
        set
        {
            if (_jsonDict.ContainsKey(KEY_SCREEN_WIDTH))
                _jsonDict[KEY_SCREEN_WIDTH] = value;
            else
                _jsonDict.Add(KEY_SCREEN_WIDTH, value);
            _Save();
        }
    }

    public static int Height
    {
        get
        {
            _Load();

            int h = 2160;

            if (_jsonDict.ContainsKey(KEY_SCREEN_HEIGHT) && _jsonDict[KEY_SCREEN_HEIGHT] != null)
            {
                int.TryParse(_jsonDict[KEY_SCREEN_HEIGHT].ToString(), out h);
            }
            else
            {
                _jsonDict[KEY_SCREEN_HEIGHT] = h;
                _Save();
            }
            return h;
        }
        set
        {
            if (_jsonDict.ContainsKey(KEY_SCREEN_HEIGHT))
                _jsonDict[KEY_SCREEN_HEIGHT] = value;
            else
                _jsonDict.Add(KEY_SCREEN_HEIGHT, value);
            _Save();
        }
    }

    #endregion

    #region Private variables
    private static Dictionary<string, object> _jsonDict;
    #endregion

    #region Constants
    public const string kKeyNameSyncInterfaceIP = "sync_interface_ip";
    public const string kKeyNamePort = "port";
    public const string kKeyNameSingleMode = "single_mode";

    public const string kKeyNameEnablesAutoPlay2D = "auto_play_2d";
    public const string kKeyNameAutoPlayMinute2D = "auto_play_minute_2d";

    public const string kKeyNameEnablesAutoPlayInteraction = "auto_play_interaction";
    public const string kKeyNameAutoPlayMinuteInteraction = "auto_play_minute_interaction";

    public const string kKeyNameFTPAddress = "ftp_address";
    public const string kKeyNameFTPUsername = "ftp_username";
    public const string kKeyNameFTPPassword = "ftp_password";


    public const string KEY_SCREENSHOT_PATH = "screenshot_path";
    public const string KEY_IP = "ip";
    public const string KEY_PORT = "port";
    public const string KEY_SCREEN_WIDTH = "screen_width";
    public const string KEY_SCREEN_HEIGHT = "screen_height";
    public const string KEY_WEBCAM_WIDTH = "webcam_width";
    public const string KEY_WEBCAM_HEIGHT = "webcam_height";
    public const string KEY_VSYNC_COUNT = "v_sync_cnt";
    public const string KEY_TARGET_FPS = "target_frame";
    public const string KEY_USE_AVPRO_CAMERA = "use_avpro_camera";
    public const string KEY_ENABLE_IMAGE_MARKER = "enable_image_marker";
    public const string KEY_ENABLE_SSAO = "enable_ssao";
    public const string KEY_ENABLE_AA = "enable_anti_aliasing";
    public const string KEY_ENABLE_COMPUTE_SHADER = "enable_compute_shader";
    public const string KEY_ENABLE_DEBUG_WINDOW = "enable_debug_window";
    public const string KEY_ENABLE_DEBUG_CONSOLE = "enable_debug_console";
    public const string KEY_ANIMAL_INDEX = "NextIndex";


    public const string KEY_THEME = "theme";
    public const string KEY_COUNTRY = "country";
    #endregion

    public static object Load(string _key)
    {
        if (_jsonDict == null) Load();
        if (_jsonDict.ContainsKey(_key)) return _jsonDict[_key];
        else return null;
    }

    public static void Load()
    {
        if (_jsonDict == null)
        {
            string jsonText = "";
            try
            {
                jsonText = File.ReadAllText(string.Format("{0}/settings.txt", CommonPath.GetDataRootPath()));
                _jsonDict = MiniJSON.Json.Deserialize(jsonText) as Dictionary<string, object>;
                Debug.Log("SettingsManager.Load() : Load success!");
            }
            catch (IOException e)
            {
                Debug.Log("SettingsManager.Load() : settings.txt load failed. trying to make default file.");
                Debug.LogException(e);

                _jsonDict = new Dictionary<string, object>();
                Save();
            }
            catch (System.Exception e)
            {
                Debug.Log("SettingsManager.Load() : settings.txt load failed.");
                Debug.LogException(e);
            }

            CustomInput.receivesMouseInput = CommonSettings.receivesMouseInput;
            CustomInput.maxUserCount = CommonSettings.maxUserCount;

            Template3D.prefersComputeShaders = CommonSettings.prefersComputeShaders;
            TCCamera.prefersComputeShaders = CommonSettings.prefersComputeShaders;
            Canvas_.prefersComputeShaders = CommonSettings.prefersComputeShaders;
        }
    }

    public static void Save(string _key, object _value)
    {
        if (_jsonDict == null) Load();

        if (_jsonDict.ContainsKey(_key)) _jsonDict[_key] = _value;
        else _jsonDict.Add(_key, _value);

        Save();
    }

    public static void Save()
    {
        if (_jsonDict == null) Load();
        string jsonText = MiniJSON.Json.Serialize(_jsonDict);
        try
        {
            File.WriteAllText(string.Format("{0}/settings.txt", CommonPath.GetDataRootPath()), jsonText);
        }
        catch (System.Exception e)
        {
            Debug.Log("SettingsManager.Save() : settings.txt save failed.");
            Debug.LogException(e);
        }
    }

    public static T Get<T>(string key, T defaultValue)
    {
        _Load();

        if (_jsonDict.ContainsKey(key) && _jsonDict[key] != null)
            return (T)_jsonDict[key];
        else
        {
            _jsonDict[key] = defaultValue;
            _Save();
        }

        return defaultValue;
    }

    public static void Set<T>(string key, T value)
    {
        if (_jsonDict.ContainsKey(key))
            _jsonDict[key] = value;
        else
            _jsonDict.Add(key, value);
        _Save();
    }

    private static void _Load()
    {
        if (_jsonDict == null)
        {
            string jsonText = "";
            try
            {
                jsonText = File.ReadAllText("./settings.txt");
                _jsonDict = MiniJSON.Json.Deserialize(jsonText) as Dictionary<string, object>;
                Debug.Log("SettingsManager._Load() : Load success!");
            }
            catch (IOException e)
            {
                Debug.Log("SettingsManager._Load() : failed to load \"settings.txt\". trying to make default file.");
                Debug.Log(e);

                _jsonDict = new Dictionary<string, object>();
                _Save();
            }
            catch (System.Exception e)
            {
                Debug.Log("SettingsManager._Load() : failed to load \"settings.txt\".");
                Debug.Log(e);

                _jsonDict = new Dictionary<string, object>();
                _Save();
            }
        }
    }

    private static void _Save()
    {
        string jsonText = MiniJSON.Json.Serialize(_jsonDict);
        try
        {
            File.WriteAllText("./settings.txt", jsonText);
            Debug.Log("SettingsManager._Save() : Save success!");
        }
        catch (System.Exception e)
        {
            Debug.Log("SettingsManager._Save() : failed to save \"settings.txt\".");
        }
    }
}