using System.IO;
using UnityEngine;

namespace ML.PlaywallKids.Common
{
    public static class CommonSettings
    {
        private class _Field
        {
            public float ui_scale = 1.0f;
            public string dist = "";
            public int max_user_count = 0;
            public bool mouse_input = false;
            public bool printer_enabled = true;
            public bool prefer_compute_shader = true;
        }

        private static _Field _field = null;

        /// <summary>
        /// Relative UI scale. (default = 1.0: 100%)
        /// </summary>
        public static float uiScale
        {
            get
            {
                Load();
                return _field.ui_scale;
            }
        }

        /// <summary>
        /// Distribution target. (possible values: mapo, ...)
        /// </summary>
        public static string dist
        {
            get
            {
                Load();
                return _field.dist;
            }
        }

        /// <summary>
        /// Represents the maximum allowed number of users. If the value is 0, application uses default value via screen aspect raio.
        /// </summary>
        public static int maxUserCount
        {
            get
            {
                Load();

                int count = _field.max_user_count;
                if (count == 0)
                {
                    var screenType = ScreenUtil.screenType;
                    switch (screenType)
                    {
                        case ScreenType.Bigboard2x3: return 5;
                        case ScreenType.Bigboard2x4: return 6;
                        case ScreenType.Bigboard2x6: return 10;
                        default: return 2;
                    }
                }
                return count;
            }
        }

        /// <summary>
        /// A bool value indicating whether the application must receives mouse input.
        /// </summary>
        public static bool receivesMouseInput
        {
            get
            {
                Load();

                bool mouseInput = _field.mouse_input;
#if UNITY_EDITOR
                mouseInput = true;
#endif
                return mouseInput;
            }
        }

        /// <summary>
        /// A boolean value indicating whether the printer device is enabled and usable.
        /// </summary>
        public static bool printerEnabled
        {
            get
            {
                Load();
                return _field.printer_enabled;
            }
            set
            {
                Load();
                _field.printer_enabled = value;
                Save();
            }
        }

        /// <summary>
        /// A boolean value indicating whether the application uses compute shaders for GPGPU.
        /// </summary>
        public static bool prefersComputeShaders
        {
            get
            {
                Load();
                return _field.prefer_compute_shader;
            }
        }

        #region Load/Save
        public static void Load()
        {
            if (_field == null)
            {
                string rootPath = CommonPath.GetDataRootPath();
                try
                {
                    string jsonText = File.ReadAllText(string.Format("{0}/common.txt", rootPath));
                    _field = JsonUtility.FromJson<_Field>(jsonText);
                }
                catch (IOException e)
                {
                    Debug.Log("CommonSettings.Load() : common.txt load failed. trying to make default file.");
                    Debug.LogException(e);
                    _field = new _Field();
                    Save();
                }
            }
        }

        public static void Save()
        {
            if (_field == null)
                Load();

            string rootPath = CommonPath.GetDataRootPath();
            string jsonText = JsonUtility.ToJson(_field, true).Replace("\n", "\r\n");
            try
            {
                File.WriteAllText(string.Format("{0}/common.txt", rootPath), jsonText);
            }
            catch (System.Exception e)
            {
                Debug.Log("CommonSettings.Save() : common.txt save failed.");
                Debug.LogException(e);
            }
        }
        #endregion
    }
}