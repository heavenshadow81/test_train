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
    public class LauncherTouchLocal : MonoBehaviour
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

            messageText.text = LocalizationManager.GetData(LocalizationKey.MANAGEMENT_CERTIFICATIONMODE);
            submessageText.text = LocalizationManager.GetData(LocalizationKey.MANAGEMENT_REQUIRECERTIFICATION);
            versionsText.text = string.Format("v{0}", Application.version);

            // Loads settings.txt
            SettingsManager.Load();

            // Parse command line arguments and get user's ID and password
            _ParseCommandLineArguments();

            yield return new WaitForSeconds(1.0f);

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

            if (macList.Contains("3C-7C-3F-F3-E3-68") || macList.Contains("00-26-66-4F-EA-B8") || macList.Contains("58-86-94-FB-C8-E4"))
            {
                StartCoroutine(_GoToNextScene());
            }
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