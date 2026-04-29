using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System;

#if MAC_CERTIFICATION
using ML.MACCertification;
#endif

namespace ML.PlaywallKids.Launcher
{
    /// <summary>
    /// 플레이월키스 초기화 및 실행 (진입부)
    /// </summary>
    public class LauncherTouch : MonoBehaviour
    {
        #region Public variables
        public UILabel messageText;
        public UILabel submessageText;
        public UILabel versionsText;
        public UIAtlas[] preloadAtlases;
        public UISprite preloadDummySprite;
        public GameObject[] preloadPrefabs;
        
        #endregion

        #region Private variables
        private string _id, _pw;
        [SerializeField] string nextSceneName;
        
        #endregion

        IEnumerator Start()
        {
            messageText.text = submessageText.text = "";
            
            
#if UNITY_EDITOR
            messageText.text = "Editor Mode";
            submessageText.text = "에디터에서 실행하였습니다";
#elif MAC_CERTIFICATION || LICENSEKEY_CERTIFICATION
            
            messageText.text = LocalizationManager.GetData(LocalizationKey.MANAGEMENT_CERTIFICATIONMODE);
            submessageText.text = LocalizationManager.GetData(LocalizationKey.MANAGEMENT_REQUIRECERTIFICATION);
            
#elif BIGBOARD_STANDALONE
            messageText.text = LocalizationManager.GetData(LocalizationKey.MANAGEMENT_STANDALONEMODE);
            submessageText.text = LocalizationManager.GetData(LocalizationKey.MANAGEMENT_ALLCONTENTACCESS);

#endif

            versionsText.text = string.Format("v{0}", Application.version);

            // Loads settings.txt
            SettingsManager.Load();

            // Parse command line arguments and get user's ID and password
            _ParseCommandLineArguments();

#if UNITY_EDITOR
            _id = "admin"; _pw = "admin";
#elif UNITY_STANDALONE
            _id = "standalone"; _pw = "standalone";
#elif UNITY_ANDROID
    _id = "androidUser"; _pw = "androidPassword";
#endif

            if (string.IsNullOrEmpty(_id) || string.IsNullOrEmpty(_pw))
            {
                _ShowErrorMessageAndQuit("세션 값이 넘어오지 않았습니다. ??");
                yield break;
            }

            yield return new WaitForSeconds(1.0f);

            // 1. Login
            BigboardServer.Login(_id, _pw, (conn1, retCode1, msg1, flag) =>
            {
                if (flag)
                {
                    // 2. Contents List
                    BigboardServer.GetContentsList((conn2, retCode2, msg2) =>
                    {
                        if (conn2 == WWWUtil.ConnectionResult.Success)
                        {
                            // 3. Config
                            BigboardServer.GetConfig((conn3, retCode3, msg3, mainOrder, subOrder) =>
                            {
                                if (conn3 == WWWUtil.ConnectionResult.Success)// && mainOrder.Count > 0 && subOrder.Count > 0)
                                {
                                    // 4. SI Modeling Info
                                    BigboardServer.GetState((conn4, retCode4, msg4, siInfo) =>
                                    {
                                        if (conn4 == WWWUtil.ConnectionResult.Success)
                                        {
                                            // Complete!
#if MAC_CERTIFICATION
                                            if (MACCertified.IsPlayable())
                                            {
                                                StartCoroutine(_GoToNextScene());
                                            }
                                            else
                                            {
                                                submessageText.text = "인증이 되지 않은 PC 입니다. ";
                                                _ShowErrorMessageAndQuit("(주)플레이월로 문의부탁드립니다.\nTel : 02-538-0058\nEmail : lsgc@playwall.kr");
                                            }
#elif LICENSEKEY_CERTIFICATION
                                            LicenseChecker.CheckLicense((isPlayable, msg) =>
                                            {
                                                if (isPlayable)
                                                {
                                                    print("인증되었음");
                                                    StartCoroutine(_GoToNextScene());
                                                }
                                                else
                                                {
                                                    submessageText.text = "인증이 되지 않은 PC 입니다. ";
                                                    _ShowErrorMessageAndQuit("(주)벡스 인텔리전스로 문의부탁드립니다.\nTel : 070-4066-0962\nEmail : jhc@bax.co.kr");
                                                }
                                            });
#elif LICENSEKEY_CERTIFICATION_MACONLY
                                            //string macString = null;

                                            List<string> macList = new List<string>();
                                            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                                            {
                                                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                                                {
                                                    string mac = nic.GetPhysicalAddress().ToString();
                                                    string macString = $"{mac.Substring(0, 2)}-{mac.Substring(2, 2)}-{mac.Substring(4, 2)}-{mac.Substring(6, 2)}-{mac.Substring(8, 2)}-{mac.Substring(10, 2)}";
                                                    macList.Add(macString);
                                                }
                                            }

                                            //if (macList.Contains("70-85-C2-05-C4-01") || macList.Contains("3C-7C-3F-F3-E3-68")|| macList.Contains("D0-50-99-A8-B5-7A")|| macList.Contains("7C-83-34-B9-DC-20") || macList.Contains("D8-BB-C1-D4-38-12"))
                                           // if (macList.Contains("3C-7C-3F-F3-E3-4F")) //진우컴
                                            if (macList.Contains("9C-6B-00-0E-95-05")|| macList.Contains("3C-7C-3F-F3-E3-4F")|| macList.Contains("10-FF-E0-95-A6-18") || macList.Contains("10-FF-E0-95-A6-16")) // 두레정보통신 // 아로씽킹(동대전도서관)

                                            //DateTime startDate = new DateTime(2024, 11, 30);
                                            //DateTime endDate = new DateTime(2024, 12, 06);

                                            //bool IsWithinFiveDays(DateTime currentDate, DateTime targetDate)
                                            //{
                                            //    // 현재 날짜의 시간 부분을 제거 (날짜만 비교)
                                            //    DateTime today = DateTime.Now.Date; // 오늘의 날짜 (시간 제외)
                                            //    DateTime start = currentDate.Date;  // 시작 날짜 (시간 제외)
                                            //    DateTime end = targetDate.Date;    // 끝 날짜 (시간 제외)

                                            //    return today >= start && today <= end;
                                            //}

                                            //if (IsWithinFiveDays(startDate, endDate))
                                            //{
                                                StartCoroutine(_GoToNextScene());
                                          //  }
                                            else
                                            {
                                                submessageText.text = "인증이 되지 않은 PC 입니다. ";
                                                _ShowErrorMessageAndQuit("(주)벡스 인텔리전스로 문의부탁드립니다.\nTel : 070-4066-0962\nEmail : jhc@bax.co.kr");
                                            }
#else
                                            StartCoroutine(_GoToNextScene());
#endif
                                        }
                                        else _ShowErrorMessageAndQuit(retCode1, msg1, "상황정보 모델링 설정을 가져오지 못했습니다.");
                                    }); // 4
                                }
                                else _ShowErrorMessageAndQuit(retCode1, msg1, "콘텐츠 설정을 가져오지 못했습니다.");
                            }); // 3
                        }
                        else _ShowErrorMessageAndQuit(retCode1, msg1, "콘텐츠 목록을 가져오지 못했습니다.");
                    }); // 2
                }
                else _ShowErrorMessageAndQuit(retCode1, msg1, "로그인을 실패하였습니다.");
            }); // 1
                //
                // end of Start()
        }

        private void _ShowErrorMessageAndQuit(string alert)
        {
            _ShowErrorMessageAndQuit(null, null, alert);
        }

        private void _ShowErrorMessageAndQuit(string retCode, string message, string alert)
        {
            alert = string.Format("{0} {1}{2}", alert,
                !string.IsNullOrEmpty(message) ? string.Format("({0},{1}", retCode, message) : "", retCode != null ? "\n인터넷 연결 후 다시 실행하세요." : "");
            BigboardAlertPanel.sharedInstance.Show(BigboardAlertPanel.ButtonType.OneButton, alert, "확인", () => { Application.Quit(); });
        }

        private void _ParseCommandLineArguments()
        {
            string[] args = System.Environment.GetCommandLineArgs();
            string currentCommand = "";

            for (int i = 1; i < args.Length; i++)
            {
                string arg = args[i];

                if (arg[0] == '-')
                    currentCommand = arg;
                else
                    currentCommand = "";

                switch (currentCommand)
                {
                    case "-contents-management-enable":
                    case "-contents_management-enable":
                        AdminMenuSelectionPanel.switchContentsManagement = true;
                        break;

                    case "-bigboard-fullscreen":
                        int fullscreen = 0;
                        if (int.TryParse(arg, out fullscreen))
                            Screen.fullScreen = fullscreen != 0;
                        break;
                    case "-bigboard-id":
                        _id = arg;
                        break;
                    case "-bigboard-pw":
                        _pw = arg;
                        break;
                    default:
                        break;
                }

            }
        }

        private IEnumerator _GoToNextScene()
        {
            yield return StartCoroutine(_PreloadAssets());

            // discards old user-made contents
            ResourceManager.RemoveOldTemplate3Ds();
            ResourceManager.RemoveOldAquariumTemplates();

            // Load SpaceIntro scene.
            yield return new WaitForSeconds(1.0f);


            //StartCoroutine(_LoadLevelDelayed(menuCommonMode, 2.0f));
            //BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Drawing2D;
            //yield return new WaitForSeconds(2.0f);
#if KINECTPANG
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Interaction");
            BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Interaction;
#else
            if (nextSceneName.Contains("3D"))
            {
                BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Drawing3D;
            }
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(1);
            
#endif
            
            //UnityEngine.SceneManagement.SceneManager.LoadScene("Aquarium");
            yield return null;
            yield break;
        }

        private IEnumerator _PreloadAssets()
        {
            yield return new WaitForEndOfFrame();

            yield return StartCoroutine(_PreloadAtlases());

            yield return new WaitForEndOfFrame();

            yield return StartCoroutine(_PreloadPrefabs());
        }

        private IEnumerator _PreloadAtlases()
        {
            // Preloads all atlases. Set the one of sprite to UISprite object to load texture on memory.
            for (int i = 0; i < preloadAtlases.Length; i++)
            {
                UIAtlas atlas = preloadAtlases[i];
                if (atlas != null)
                {
#if UNITY_EDITOR
                    Debug.Log("Launcher - Preloading atlas \"" + atlas.name + "\"...");
#elif UNITY_STANDALONE
                System.Console.WriteLine("Launcher - Preloading atlas \"{0}\"...", atlas.name);
#endif

                    var list = atlas.GetListOfSprites();
                    if (list.size > 0)
                    {
                        string spriteName = list[0];
                        preloadDummySprite.atlas = atlas;
                        preloadDummySprite.spriteName = spriteName;

                        // Make sure to draw at least one frame
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
        }

        private IEnumerator _PreloadPrefabs()
        {
            List<GameObject> list = new List<GameObject>(128);
            list.AddRange(preloadPrefabs);

            for (int i = 0; i < System.Enum.GetValues(typeof(AccessoryManager.AccessoryType)).Length; i++)
            {
                GameObject[] accessoryPrefabs = AccessoryManager.list[(AccessoryManager.AccessoryType)i];
                list.AddRange(accessoryPrefabs);
            }
            GameObject[] avatarAccessoryPrefabs = AccessoryManager.avatarAccessories;
            list.AddRange(avatarAccessoryPrefabs);
            GameObject[] fruits = Resources.LoadAll<GameObject>("Fruits");
            list.AddRange(fruits);
            GameObject[] template3Ds = Resources.LoadAll<GameObject>("Template3D");
            list.AddRange(template3Ds);
            GameObject[] freeDrawingObjects = Resources.LoadAll<GameObject>("FreeDrawingObjects");
            list.AddRange(freeDrawingObjects);

            for (int i = 0; i < list.Count; i++)
            {
                GameObject prefab = list[i];
                if (prefab != null)
                {
#if UNITY_EDITOR
                    Debug.Log("Launcher - Preloading prefab \"" + prefab.name + "\"...");
#elif UNITY_STANDALONE
                System.Console.WriteLine("Launcher - Preloading prefab \"{0}\"...", prefab.name);
#endif

                    GameObject dummy = (GameObject)Instantiate(prefab);
                    dummy.SetActive(true);

                    DragonAnimationControl dragonAnimation = dummy.GetComponent<DragonAnimationControl>();
                    if (dragonAnimation != null) dragonAnimation.usesNavMesh = false;

                    yield return new WaitForEndOfFrame();
                    Destroy(dummy, 1.0f);
                }
            }
        }
    }
}