using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.DragonPark
{
    using Common;
    /// <summary>
    /// Main menu controller of Dragon Park.
    /// </summary>
    public class MenuControl : MonoBehaviour
    {
        #region Enums
        public enum Menu
        {
            None = 0,
            Dragon,
            Avatar,
            MotionGame,
            Drawing,
            FreeDrawing,
            PetMotion,
            Reset
        }
        #endregion

        #region Public variables
        public GameObject mainMenu;
        public GameObject mainPanel;
        public GameObject AToolStart;
        public GameObject AvatarPanel;
        public GameObject Canvas;
        public GameObject MotionPanel;
        public GameObject FreeDrawingPanel;
        public GameObject FreeDrawingMenuPanel;
        public GameObject PetMotionPanel;
        public GameObject PetMenuPanel;
        public GameObject PetPrintPanel;
        public GameObject DragonPrinter;
        public GameObject PetFeedPanel;

        public GameObject touchBox;
        public GameObject loading;

        public UIButton optionButton;
        public UIButton dayNightButton;

        public new Light light;
        #endregion

        #region Properties
        private static MenuControl __sharedInstance = null;
        public static MenuControl sharedInstance
        {
            get
            {
                return __sharedInstance;
            }
        }

        private Menu _menu = Menu.None;
        public Menu menu
        {
            get
            {
                return _menu;
            }
            set
            {
                if (_menu == value) return;

                HideMenu(_menu);
                _menu = value;
                ShowMenu(_menu);
            }
        }

        private static int _userCount = 0;
        public static int userCount
        {
            get
            {
                return _userCount;
            }
            set
            {
                _userCount = Mathf.Clamp(value, 0, CommonSettings.maxUserCount);
                userSeqs = _userSeqs;
            }
        }

        private static int[] _userSeqs = new int[0];
        public static int[] userSeqs
        {
            get
            {
                return _userSeqs;
            }
            set
            {
                if (value != null)
                {
                    _userSeqs = new int[userCount];
                    for (int i = 0; i < _userSeqs.Length; i++)
                    {
                        if (value.Length > i)
                        {
                            _userSeqs[i] = value[i];
                        }
                    }
                }
            }
        }

        public static int leftUserSeq = 0;
        public static int rightUserSeq = 0;
        public static int leftUserId = 0;
        public static int rightUserId = 0;
        #endregion

        #region Private variables
        // Camera
        private Camera _uiCamera = null;
        private Rect _cameraRect;

        // Start/End Positions
        protected Vector3 startPos;
        protected Vector3 endPos;

        // Instantiated Menus
        private GameObject _mainMenu;
        private GameObject _simpleMotion;
        private AToolStartPanel _AToolStartPanel = null;
        private AvatarGamePanel _avatarPanel = null;
        private FindPetStartPanel _motionPanel = null;
        private Dictionary<int, SimpleCanvasControl> _canvases = new Dictionary<int, SimpleCanvasControl>();
        private Dictionary<int, ATool3DPanel> _freeDrawingPanels = new Dictionary<int, ATool3DPanel>();
        private Dictionary<int, ATool3DMenuPanel> _freeDrawingMenuPanels = new Dictionary<int, ATool3DMenuPanel>();
        private Dictionary<int, AToolPetMotionPanel> _petMotionPanels = new Dictionary<int, AToolPetMotionPanel>();
        private Dictionary<int, AToolPetPrintPanel> _petPrintPanels = new Dictionary<int, AToolPetPrintPanel>();
        private Dictionary<int, AToolPetFeedPanel> _petFeedPanels = new Dictionary<int, AToolPetFeedPanel>();
        private Dictionary<int, AToolPetMenuPanel> _petMenuPanels = new Dictionary<int, AToolPetMenuPanel>();
        private Dictionary<int, DragonPrinter> _dragonPrinters = new Dictionary<int, DragonPrinter>();

        // Signle Menu
        private Dictionary<int, DragonParkUserInfo> listUserInfo = new Dictionary<int, DragonParkUserInfo>();

        #endregion

        #region Initialization Methods
        public void Awake()
        {
            __sharedInstance = this;

            userCount = CommonSettings.maxUserCount;

            loading.SetActive(false);

            // default day settings
            OnWeatherChange(BackgroundManager.Weather.Sunny);
            OnDayTimeChange(BackgroundManager.DayTime.Day);

            // register to messenger
            Messenger<BackgroundManager.Weather>.AddListener(BackgroundManager.kEventTypeWeatherChange, OnWeatherChange);
            Messenger<BackgroundManager.DayTime>.AddListener(BackgroundManager.kEventTypeDayTimeChange, OnDayTimeChange);
        }

        public void Start()
        {
            _uiCamera = NGUITools.FindCameraForLayer(mainPanel.layer);
            _cameraRect = new Rect(0, 0, Screen.width, Screen.height);

            startPos = _uiCamera.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
            endPos = _uiCamera.ScreenToWorldPoint(new Vector3((float)Screen.width, (float)Screen.height, 0f));

            // To run this code, you need to run "PlayWallSyncInterface" program fitst.
            /*
            PlayWallServer server = PlayWallServer.sharedInstance;
            Debug.Log ("Starting server...");
            */

            if (touchBox != null)
            {
                touchBox.SetActive(false);
                for (int i = 0; i < userCount; i++)
                {
                    GameObject go = NGUITools.AddChild(touchBox.transform.parent.gameObject, touchBox);
                    go.SetActive(true);
                    go.transform.localPosition = GetUserPosition(i);

                    DragonParkUserInfo info = go.GetComponent<DragonParkUserInfo>();
                    info.SetUser(i);
                    listUserInfo.Add(i, info);
                }
                Destroy(touchBox);
            }
        }

        public void OnDestroy()
        {
            __sharedInstance = null;
            Messenger<BackgroundManager.Weather>.RemoveListener(BackgroundManager.kEventTypeWeatherChange, OnWeatherChange);
            Messenger<BackgroundManager.DayTime>.RemoveListener(BackgroundManager.kEventTypeDayTimeChange, OnDayTimeChange);
        }
        #endregion

        #region Menu
        public void HideMenu(int userID)
        {
            HideCanvas(userID);
            HideDragon(userID);
            HideFreeDrawing(userID);
            HidePetMotion(userID);
        }

        public void HideMenu(Menu menu)
        {
            switch (menu)
            {
                case Menu.None:
                    HideMainMenu();
                    break;
                case Menu.Dragon:
                    HideDragon();
                    break;
                case Menu.Avatar:
                    HideAvatar();
                    break;
                case Menu.MotionGame:
                    HideMotion();
                    break;
                case Menu.Drawing:
                    HideCanvas();
                    break;
                case Menu.FreeDrawing:
                    HideFreeDrawing();
                    break;
                case Menu.PetMotion:
                    HidePetMotion();
                    break;
            }
            for (int i = 0; i < listUserInfo.Count; i++)
                Activate(i);
        }

        public void ShowMenu(Menu menu)
        {

            for (int i = 0; i < listUserInfo.Count; i++)
                listUserInfo[i].Deactivate();

            switch (menu)
            {
                case Menu.Dragon:
                    ShowDragon();
                    break;
                case Menu.Avatar:
                    ShowAvatar();
                    break;
                case Menu.MotionGame:
                    ShowMotion();
                    break;
                case Menu.Drawing:
                    ShowCanvas();
                    break;
                case Menu.FreeDrawing:
                    ShowFreeDrawing();
                    break;
                case Menu.PetMotion:
                    ShowPetMotion();
                    break;
                case Menu.Reset:
                    ResetMenu();
                    break;
            }
        }

        public void MenuSprit()
        {
            if (menu != Menu.None)
            {
                HideMenu(menu);
            }

            _menu = Menu.None;
            if (_mainMenu == null)
            {
                _mainMenu = NGUITools.AddChild(mainPanel, mainMenu);

                float height = UIRoot.list[0].activeHeight;
                float width = height * ((float)Screen.width / (float)Screen.height);

                TweenPosition tp = _mainMenu.GetComponent<TweenPosition>();
                float offset = Mathf.Abs((tp.to - tp.from).x);

                Vector3 position = _mainMenu.transform.localPosition;
                position.x = -width * 0.5f - offset + 125.0f;
                _mainMenu.transform.localPosition = position;

                tp.from = position;
                tp.to = position + new Vector3(offset, 0, 0);

                UIButtonMessage[] messages = this.gameObject.GetComponentsInChildren<UIButtonMessage>();
                foreach (UIButtonMessage message in messages)
                    message.target = this.gameObject;
            }
        }

        #region Canvas
        public void ShowCanvas()
        {
            if (menu != Menu.Drawing)
            {
                menu = Menu.Drawing;
            }
            else
            {
                if (_canvases.Count == 0)
                {
                    HideMainMenu();

                    _menu = Menu.Drawing;

                    for (int i = 0; i < CommonSettings.maxUserCount; i++)
                        ShowCanvas(i);
                }
            }

            HideMainMenu();
        }

        public GameObject ShowCanvas(int userID)
        {
            if (userID >= 0)
            {
                SimpleCanvasControl canvas = NGUITools.AddChild(mainPanel, Canvas).GetComponent<SimpleCanvasControl>();
                canvas.transform.localPosition = GetUserPosition(userID);
                canvas.gameObject.SetActive(true);
                _canvases.Add(userID, canvas);

                return canvas.gameObject;
            }

            return null;
        }

        public void HideCanvas()
        {
            while (_canvases.Count > 0)
            {
                List<int> list = new List<int>(_canvases.Keys);
                if (list.Count > 0)
                    HideCanvas(list[0]);
            }
            _menu = Menu.None;
        }

        public void HideCanvas(int userID)
        {
            if (_canvases.ContainsKey(userID))
            {
                if (_canvases[userID] != null)
                {
                    Destroy(_canvases[userID].gameObject);
                }
                _canvases.Remove(userID);
            }
        }
        #endregion

        #region Dragon
        public void ShowDragon()
        {
            if (menu != Menu.Dragon)
            {
                menu = Menu.Dragon;
            }
            else
            {
                if (_AToolStartPanel == null)
                {
                    _AToolStartPanel = NGUITools.AddChild(mainPanel, AToolStart).GetComponent<AToolStartPanel>();
                    _AToolStartPanel.gameObject.SetActive(true);
                    _AToolStartPanel.transform.localPosition = Vector3.zero;
                    _AToolStartPanel.Show(userCount, userSeqs, listUserInfo, GetUserPosition);
                }
            }

            HideMainMenu();
        }

        public GameObject ShowDragon(int userID)
        {
            if (_AToolStartPanel == null)
            {
                _AToolStartPanel = NGUITools.AddChild(mainPanel, AToolStart).GetComponent<AToolStartPanel>();
                _AToolStartPanel.gameObject.SetActive(true);
                _AToolStartPanel.transform.localPosition = Vector3.zero;
            }
            return _AToolStartPanel.Show(userID, GetUserPosition);
        }

        public void HideDragon()
        {
            if (_AToolStartPanel != null)
            {
                Destroy(_AToolStartPanel.gameObject);
                _AToolStartPanel = null;
            }

            _menu = Menu.None;
        }

        public void HideDragon(int userID)
        {
            if (_AToolStartPanel != null)
            {
                _AToolStartPanel.DeleteATool(userID);
            }

            _menu = Menu.None;
        }

        public void HideMainMenu()
        {
            if (_mainMenu != null)
            {
                Destroy(_mainMenu);
                _mainMenu = null;
            }
        }
        #endregion

        #region FreeDrawing

        public void ShowFreeDrawing()
        {
            if (menu != Menu.FreeDrawing)
            {
                menu = Menu.FreeDrawing;
            }
            else
            {
                for (int i = 0; i < CommonSettings.maxUserCount; i++)
                    ShowFreeDrawing(i);
            }

            HideMainMenu();
        }

        public GameObject ShowFreeDrawing(int userID)
        {
            ATool3DPanel panel = NGUITools.AddChild(mainPanel, FreeDrawingPanel).GetComponent<ATool3DPanel>();
            UIRoot root = UIRoot.list[0];
            float width = root.activeHeight * (float)Screen.width / (float)Screen.height;
            Vector3 pos = GetUserPosition(userID);
            panel.userId = userID;
            panel.transform.localPosition = pos;
            panel.Show();

            if (_freeDrawingPanels.ContainsKey(userID))
                _freeDrawingPanels[userID] = panel;
            else
                _freeDrawingPanels.Add(userID, panel);

            return panel.gameObject;
        }

        public void HideFreeDrawing()
        {
            while (_freeDrawingPanels.Count > 0)
            {
                List<int> list = new List<int>(_freeDrawingPanels.Keys);
                if (list.Count > 0)
                    HideFreeDrawing(list[0]);
            }
            _freeDrawingPanels.Clear();

            _menu = Menu.None;
        }

        public void HideFreeDrawing(int userID)
        {
            if (_freeDrawingPanels.ContainsKey(userID))
            {
                _freeDrawingPanels[userID].Hide();
            }
        }
        #endregion

        #region Avatar
        public void ShowAvatar()
        {
            if (menu != Menu.Avatar)
            {
                menu = Menu.Avatar;
            }
            else
            {
                if (_avatarPanel == null)
                {
                    _avatarPanel = NGUITools.AddChild(mainPanel, AvatarPanel).GetComponent<AvatarGamePanel>();
                    _avatarPanel.transform.localPosition = Vector3.zero;
                    _avatarPanel.gameObject.SetActive(true);
                }
            }

            HideMainMenu();
        }

        public void HideAvatar()
        {
            if (_avatarPanel != null)
            {
                Destroy(_avatarPanel.gameObject);
                _avatarPanel = null;
            }
            _menu = Menu.None;
        }
        #endregion

        #region Motion
        public void ShowMotion()
        {
            if (menu != Menu.MotionGame)
            {
                menu = Menu.MotionGame;
            }
            else
            {
                if (_motionPanel == null)
                {
                    _motionPanel = NGUITools.AddChild(mainPanel, MotionPanel).GetComponent<FindPetStartPanel>();
                    _motionPanel.transform.localPosition = Vector3.zero;
                    _motionPanel.gameObject.SetActive(true);
                }
            }

            HideMainMenu();
        }

        public void HideMotion()
        {
            if (_motionPanel != null)
            {
                Destroy(_motionPanel.gameObject);
                _motionPanel = null;
            }
            _menu = Menu.None;
        }
        #endregion

        #region PetMotion
        public void ShowPetMotion()
        {
            if (menu != Menu.PetMotion)
            {
                menu = Menu.PetMotion;
            }
            else
            {
                for (int i = 0; i < CommonSettings.maxUserCount; i++)
                    ShowPetMotion(i);
            }

            HideMainMenu();
        }

        public GameObject ShowPetMotion(int userID)
        {
            AToolPetMotionPanel panel = NGUITools.AddChild(mainPanel, PetMotionPanel).GetComponent<AToolPetMotionPanel>();
            UIRoot root = UIRoot.list[0];
            float width = root.activeHeight * (float)Screen.width / (float)Screen.height;
            Vector3 pos = GetUserPosition(userID);
            panel.userId = userID;
            panel.transform.localPosition = pos;
            panel.Show();
            if (_petMotionPanels.ContainsKey(userID))
                _petMotionPanels[userID] = panel;
            else
                _petMotionPanels.Add(userID, panel);

            return panel.gameObject;
        }


        public void HidePetMotion()
        {
            while (_petMotionPanels.Count > 0)
            {
                List<int> list = new List<int>(_petMotionPanels.Keys);
                if (list.Count > 0)
                    HidePetMotion(list[0]);
            }
            _petMotionPanels.Clear();

            _menu = Menu.None;
        }

        public void HidePetMotion(int userID)
        {
            if (_petMotionPanels.ContainsKey(userID))
            {
                if (_petMotionPanels[userID] != null)
                    Destroy(_petMotionPanels[userID].gameObject);
                _petMotionPanels.Remove(userID);
            }
        }
        #endregion

        #region Pet Menu
        /// <summary>
        /// Shows a character menu with user id.
        /// </summary>
        public GameObject ShowPetMenu(int userId)
        {
            AToolPetMenuPanel panel = null;
            if (!IsPetMenuOpen(userId))
            {
                panel = NGUITools.AddChild(mainPanel, PetMenuPanel).GetComponent<AToolPetMenuPanel>();
                _petMenuPanels[userId] = panel;
                Vector3 pos = GetUserPosition(userId);
                panel.transform.localPosition = pos;
                panel.userId = userId;
            }
            else
            {
                panel = _petMenuPanels[userId];
                ActivatePetMenu(userId, true);
            }
            panel.Show();
            return panel.gameObject;
        }

        /// <summary>
        /// Activate or deactive a character menu. menu will not be destroyed.
        /// </summary>
        public void ActivatePetMenu(int userId, bool activate)
        {
            if (IsPetMenuOpen(userId))
            {
                var panel = _petMenuPanels[userId];
                if (activate)
                    panel.Show();
                else
                    panel.Hide();
            }
        }

        /// <summary>
        /// Checks whether a character menu is available.
        /// </summary>
        public bool IsPetMenuOpen(int userId)
        {
            return _petMenuPanels.ContainsKey(userId);
        }

        /// <summary>
        /// Destroys a character menu of user.
        /// </summary>
        public void HidePetMenu(int userId)
        {
            if (_petMenuPanels.ContainsKey(userId))
            {
                if (_petMenuPanels[userId] != null)
                    Destroy(_petMenuPanels[userId].gameObject);
                _petMenuPanels.Remove(userId);
            }
        }

        public GameObject ShowPetPrint(int userId)
        {
            if (!_petPrintPanels.ContainsKey(userId))
            {
                AToolPetPrintPanel panel = NGUITools.AddChild(mainPanel, PetPrintPanel).GetComponent<AToolPetPrintPanel>();
                _petPrintPanels[userId] = panel;
                Vector3 pos = GetUserPosition(userId);
                panel.transform.localPosition = pos;
                panel.dragonPrinter = GetDragonPrinter(userId);
                panel.Set(userId);
                return panel.gameObject;
            }
            return _petPrintPanels[userId].gameObject;
        }

        public void HidePetPrint(int userId)
        {
            if (_petPrintPanels.ContainsKey(userId))
            {
                if (_petPrintPanels[userId] != null)
                    Destroy(_petPrintPanels[userId].gameObject);
                _petPrintPanels.Remove(userId);
            }
        }

        public GameObject ShowPetFeed(int userId)
        {
            if (!_petFeedPanels.ContainsKey(userId))
                _petFeedPanels[userId] = NGUITools.AddChild(mainPanel, PetFeedPanel).GetComponent<AToolPetFeedPanel>();
            var panel = _petFeedPanels[userId];
            panel.transform.localPosition = GetUserPosition(userId);
            panel.userId = userId;
            panel.Show();
            return panel.gameObject;
        }

        public void HidePetFeed(int userId)
        {
            if (_petFeedPanels.ContainsKey(userId))
                _petFeedPanels[userId].Hide();
        }

        public GameObject ShowFreeDrawingMenu(int userId)
        {
            if (!_freeDrawingMenuPanels.ContainsKey(userId))
                _freeDrawingMenuPanels[userId] = NGUITools.AddChild(mainPanel, FreeDrawingMenuPanel).GetComponent<ATool3DMenuPanel>();
            var panel = _freeDrawingMenuPanels[userId];
            panel.transform.localPosition = GetUserPosition(userId);
            panel.Set(userId);
            panel.Show();
            return panel.gameObject;
        }

        public void HideFreeDrawingMenu(int userId)
        {
            if (_freeDrawingMenuPanels.ContainsKey(userId))
                _freeDrawingMenuPanels[userId].Hide();
        }
        #endregion

        #region Printer
        public DragonPrinter GetDragonPrinter(int userId)
        {
            if (!_dragonPrinters.ContainsKey(userId))
            {
                GameObject go = Instantiate(DragonPrinter);
                go.name = string.Format("_Printing(#{0:00})", userId);
                go.transform.position = new Vector3(userId * 100, -100);
                go.transform.rotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                DragonPrinter newPrinter = go.GetComponent<DragonPrinter>();
                _dragonPrinters.Add(userId, newPrinter);
            }
            return _dragonPrinters[userId];
        }
        #endregion

        public void ResetMenu()
        {
            if (menu != Menu.Reset)
            {
                HideMenu(menu);
                _menu = Menu.Reset;
            }

            loading.SetActive(true);

            StartCoroutine(_PerformResetMenu());
        }

        private IEnumerator _PerformResetMenu()
        {
            
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("BigBoardMainMenu");
            async.allowSceneActivation = false;

            float _time = 0.0f;
            while (_time < 2.0f || async.progress < 0.9f)
            {
                yield return null;
                _time += Time.unscaledDeltaTime;
            }

            async.allowSceneActivation = true;
        }

        public void ShowFindPetGame_Test()
        {
            if (menu != Menu.MotionGame)
            {
                menu = Menu.MotionGame;
            }
            else
            {
                if (MapManager.dragonPark.activeSelf)
                {
                    MapManager.dragonPark.SetActive(false);
                    if (MapManager.findPet == null)
                    {
                        StartCoroutine(MapManager.LoadFindPet());
                    }
                    else
                    {
                        MapManager.findPet.SetActive(true);
                    }
                }
                else
                {
                    MapManager.findPet.SetActive(false);
                    MapManager.dragonPark.SetActive(true);
                }
            }

            HideMainMenu();
        }
        #endregion

        #region Weather Change
        public void OnWeatherChange(BackgroundManager.Weather weather)
        {
            switch (weather)
            {
                case BackgroundManager.Weather.Cloudy:
                    optionButton.normalSprite = "cloud";
                    break;
                case BackgroundManager.Weather.Rainy:
                    optionButton.normalSprite = "rain";
                    break;
                case BackgroundManager.Weather.Snowy:
                    optionButton.normalSprite = "snow";
                    break;
                default:
                    optionButton.normalSprite = "serenity";
                    break;
            }
        }

        public void OnDayTimeChange(BackgroundManager.DayTime dayTime)
        {
            if (dayTime == BackgroundManager.DayTime.Day)
            {
                dayNightButton.normalSprite = "night";
                light.gameObject.SetActive(false);
            }
            else
            {
                dayNightButton.normalSprite = "serenity";
                light.gameObject.SetActive(true);
            }
        }
        #endregion

        public void Activate(int userID)
        {
            if (listUserInfo.ContainsKey(userID))
                listUserInfo[userID].Activate();
        }

        public void Deactivate(int userID)
        {
            if (listUserInfo.ContainsKey(userID))
                listUserInfo[userID].Deactivate();
        }

        private Vector3 GetUserPosition(int id)
        {
            float div1 = 1.0f / CommonSettings.maxUserCount;
            UIRoot root = UIRoot.list[0];
            float width = root.activeHeight * (float)Screen.width / (float)Screen.height;
            Vector3 pos = new Vector3(width * (-.5f + div1 * (id + 0.5f)), -root.activeHeight * 0.2f);
            return pos;
        }

        public DragonParkUserInfo GetTouchArea(int userId)
        {
            if (listUserInfo.ContainsKey(userId))
                return listUserInfo[userId];
            return null;
        }

    }
}