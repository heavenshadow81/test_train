using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.Aquarium
{
    using Common;

    /// <summary>
    /// 아쿠아리움에서 물고기 생성 위치 등에 사용하는 유틸 클래스.
    /// </summary>
    public class AquariumDummyUtils : MonoBehaviour
    {
        #region Properties
        protected static AquariumDummyUtils _sharedInstance;
        protected static AquariumDummyUtils sharedInstance
        {
            get
            {
                if (_sharedInstance == null)
                {
                    GameObject obj = new GameObject("AquariumDummyUtils");
                    obj.hideFlags = HideFlags.HideInInspector;
                    _sharedInstance = obj.AddComponent<AquariumDummyUtils>();
                }
                return _sharedInstance;
            }
        }
        #endregion

        #region Private variables
        // caches
        private Transform _spawnPosTf;
        #endregion

        #region Unity methods
        public void OnDestroy()
        {
            _sharedInstance = null;
            _spawnPosTf = null;
        }
        #endregion

        /// <summary>
        /// 물고기를 생성할 위치를 얻는다.
        /// </summary>
        /// <param name="userId">사용자 ID</param>
        /// <param name="dummyName">SpawnPos의 child 오브젝트 이름. 없을 경우 SpawnPos 위치 사용.</param>
        /// <returns></returns>
        public static Vector3 GetSpawnPos(int userId, string dummyName = "")
        {
            Vector3 pos = Vector3.zero;

            if (sharedInstance._spawnPosTf == null)
            {
                GameObject obj = GameObject.Find("SpawnPos");
                if (obj == null)
                {
                    Debug.LogError("Couldn't find GameObject \"SpawnPos\".");
                    return pos;
                }
                else
                {
                    sharedInstance._spawnPosTf = obj.transform;
                }
            }

            // NGUI->WorldPos
            Vector3 uiPos = _GetUserUIPosition(userId);
            Vector3 worldPos = UIRoot.list[0].transform.TransformPoint(uiPos);
            Vector3 viewportPos = UICamera.list[0].cachedCamera.WorldToViewportPoint(worldPos);
            viewportPos.z = Vector3.Project(sharedInstance._spawnPosTf.position - Camera.main.transform.position, Camera.main.transform.forward).magnitude;
            worldPos = Camera.main.ViewportToWorldPoint(viewportPos);
            pos = worldPos;

            // Appends local position of child transform.
            if (!string.IsNullOrEmpty(dummyName))
            {
                Vector3 childPos = Vector3.zero;
                Transform childTf = sharedInstance._spawnPosTf.Find(dummyName);
                if (childTf != null)
                {
                    Vector3 childWorldPos = childTf.position;
                    Vector3 childViewportPos = Camera.main.WorldToViewportPoint(childWorldPos);
                    childViewportPos.x = viewportPos.x;
                    childViewportPos.z = Vector3.Project(childWorldPos - Camera.main.transform.position, Camera.main.transform.forward).magnitude;
                    childWorldPos = Camera.main.ViewportToWorldPoint(childViewportPos);
                    childPos = childWorldPos;
                }
                pos = childPos;
            }

            return pos;
        }

        private static Vector3 _GetUserUIPosition(int id)
        {
            float div1 = 1.0f / CommonSettings.maxUserCount;
            UIRoot root = UIRoot.list[0];
            float width = root.activeHeight * (float)Screen.width / (float)Screen.height;
            Vector3 pos = new Vector3(width * (-.5f + div1 * (id + 0.5f)), -root.activeHeight * 0.2f);
            return pos;
        }
    }
}