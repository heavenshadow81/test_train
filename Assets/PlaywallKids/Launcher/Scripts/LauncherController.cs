using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if MAC_CERTIFICATION
using ML.MACCertification;
#endif

namespace ML.PlaywallKids.Launcher
{
    /// <summary>
    /// 플레이월키스 초기화 및 실행 (진입부)
    /// </summary>
    public class LauncherController : MonoBehaviour
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
#endregion

        IEnumerator Start()
        {
            messageText.text = submessageText.text = "";

#if MAC_CERTIFICATION || LICENSEKEY_CERTIFICATION
            messageText.text = LocalizationManager.GetData(LocalizationKey.MANAGEMENT_CERTIFICATIONMODE);
            submessageText.text = LocalizationManager.GetData(LocalizationKey.MANAGEMENT_REQUIRECERTIFICATION);
#elif BIGBOARD_STANDALONE
            messageText.text = LocalizationManager.GetData(LocalizationKey.MANAGEMENT_STANDALONEMODE);
            submessageText.text = LocalizationManager.GetData(LocalizationKey.MANAGEMENT_ALLCONTENTACCESS);
#elif UNITY_EDITOR
            messageText.text = "Editor Mode";
            submessageText.text = "에디터에서 실행하였습니다";        
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
#endif

            if (string.IsNullOrEmpty(_id) || string.IsNullOrEmpty(_pw))
            {
                _ShowErrorMessageAndQuit("세션 값이 넘어오지 않았습니다.");
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
                                                    StartCoroutine(_GoToNextScene());
                                                }
                                                else
                                                {
                                                    submessageText.text = "인증이 되지 않은 PC 입니다. ";
                                                    _ShowErrorMessageAndQuit("(주)벡스 인텔리전스로 문의부탁드립니다.\nTel : 070-4066-0962\nEmail : jhc@bax.co.kr");
                                                }
                                            });
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
            //BigboardGlobal.currentMode = ContentsStoreItemType.Mode.Aquarium;
            //yield return new WaitForSeconds(2.0f);
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("BigBoardMainMenu");
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