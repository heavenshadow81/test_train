using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.Aquarium
{
    using Common;

    /// <summary>
    /// Simplified <seealso cref="MenuControl"/> for Aquarium scene which spawns touch boxes and handles menu events.
    /// </summary>
    public class AquariumMenuControl : MonoBehaviour
    {
        #region Enums
        public enum Menu
        {
            None = 0,
            Fish,
            Interaction
        }
        #endregion

        #region Public variables
        public GameObject loading;
        public GameObject touchBox;
        public GameObject AToolFish;
        public GameObject FishMenu;
        public GameObject FishPrint;
        public GameObject AquariumPrinter;
        #endregion

        #region Properties
        public static AquariumMenuControl sharedInstance { get; private set; }
        #endregion

        #region Private variables
        private int _userCount;
        private Dictionary<int, AquariumUserTouchArea> listUserInfo = new Dictionary<int, AquariumUserTouchArea>();
        private Dictionary<int, AquariumPrinter> _printerDict = new Dictionary<int, Aquarium.AquariumPrinter>();
        #endregion

        public void Awake()
        {
            if (sharedInstance == null)
                sharedInstance = this;
            else
                Destroy(this);
        }

        public void Start()
        {
            _userCount = Common.CommonSettings.maxUserCount;

            if (touchBox != null)
            {
                touchBox.SetActive(false);
                for (int i = 0; i < _userCount; i++)
                {
                    GameObject go = NGUITools.AddChild(touchBox.transform.parent.gameObject, touchBox);
                    go.SetActive(true);
                    go.transform.localPosition = GetUserPosition(i);

                    AquariumUserTouchArea info = go.GetComponent<AquariumUserTouchArea>();
                    info.SetUser(i);
                    listUserInfo.Add(i, info);
                }
            }
        }

        public void OnDestroy()
        {
            if (sharedInstance == this)
                sharedInstance = null;
        }

        public void ResetMenu()
        {
            loading.SetActive(true);
            StartCoroutine(_PerformResetMenu());
        }

        private IEnumerator _PerformResetMenu()
        {
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("BigBoradMainMenu");
            async.allowSceneActivation = false;

            float _time = 0.0f;
            while (_time < 2.0f || async.progress < 0.9f)
            {
                yield return null;
                _time += Time.unscaledDeltaTime;
            }

            async.allowSceneActivation = true;
        }
        
        private Vector3 GetUserPosition(int id)
        {
            float div1 = 1.0f / CommonSettings.maxUserCount;
            UIRoot root = UIRoot.list[0];
            float width = root.activeHeight * (float)Screen.width / (float)Screen.height;
            Vector3 pos = new Vector3(width * (-.5f + div1 * (id + 0.5f)), -root.activeHeight * 0.2f);
            return pos;
        }

        public GameObject ShowAToolFish(int userId)
        {
            if (userId >= 0)
            {
                GameObject go = NGUITools.AddChild(gameObject, AToolFish);
                go.SetActive(true);
                go.transform.localPosition = GetUserPosition(userId);
                AToolFishPanel panel = go.GetComponent<AToolFishPanel>();
                if (panel != null)
                    panel.userId = userId;
                return go;
            }
            return null;
        }

        public void Interaction(int userId)
        {
            Vector3 uiPos = GetUserPosition(userId);
            uiPos += (Vector3)(Random.insideUnitCircle * Random.Range(100.0f, 160.0f));
            Vector3 worldPos = UIRoot.list[0].transform.TransformPoint(uiPos);
            Vector3 viewportPos = UICamera.list[0].cachedCamera.WorldToViewportPoint(worldPos);
            viewportPos.z = 250;
            worldPos = Camera.main.ViewportToWorldPoint(viewportPos);

            List<PathExample> fishes = PathExample.GetList(FISH_SIZE.SMALL);

            // 목록 섞기
            for (int i = 0; i < fishes.Count; i++)
            {
                int idx1 = Random.Range(0, fishes.Count);
                int idx2 = Random.Range(0, fishes.Count);
                if (idx1 == idx2)
                {
                    idx2 = idx1 - 1;
                    if (idx2 < 0) idx2 = idx1 + 1;
                    if (idx2 >= fishes.Count) idx2 = idx1;
                }
                var t = fishes[idx1];
                fishes[idx1] = fishes[idx2];
                fishes[idx2] = t;
            }

            // 먹이주기 호출
            for (int i = 0, feed = 0; i < fishes.Count && feed < 1; i++)
            {
                PathExample fish = fishes[i];
                if (fish.status == STATUS.SWIM)
                {
                    fish.FeedAction(worldPos);
                    feed++;
                }
            }
        }

        public GameObject ShowFishMenu(int userId)
        {
            if (userId >= 0)
            {
                GameObject go = NGUITools.AddChild(gameObject, FishMenu);
                go.SetActive(true);
                go.transform.localPosition = GetUserPosition(userId);
                AToolFishMenuPanel panel = go.GetComponent<AToolFishMenuPanel>();
                if (panel != null)
                    panel.userId = userId;
                return go;
            }
            return null;
        }

        public GameObject ShowFishPrint(int userId)
        {
            if (userId >= 0)
            {
                GameObject go = NGUITools.AddChild(gameObject, FishPrint);
                go.SetActive(true);
                go.transform.localPosition = GetUserPosition(userId);
                AToolFishPrintPanel panel = go.GetComponent<AToolFishPrintPanel>();
                if (panel != null)
                    panel.Set(userId);
                return go;
            }
            return null;
        }

        public void Activate(int userID)
        {
            if (listUserInfo.ContainsKey(userID))
                listUserInfo[userID].Activate();
        }

        public AquariumPrinter GetPrinter(int userId)
        {
            if (!_printerDict.ContainsKey(userId))
            {
                GameObject go = Instantiate(AquariumPrinter);
                go.transform.position = new Vector3(userId * 100, -500);
                go.transform.rotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                AquariumPrinter printer = go.GetComponent<AquariumPrinter>();
                _printerDict[userId] = printer;
            }
            return _printerDict[userId];
        }
    }
}