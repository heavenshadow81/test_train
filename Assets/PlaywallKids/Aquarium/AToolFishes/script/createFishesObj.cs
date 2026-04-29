using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ML.PlaywallKids.Aquarium
{
    public class createFishesObj : MonoBehaviour
    {
        private static createFishesObj s_data = null;

        private List<GameObject> s_fishes = new List<GameObject>();
        
        private GameObject parentObj = null;
        private Dictionary<string, exAtoolFath> templateObjDict = new Dictionary<string, exAtoolFath>();

        int nScreenNo = 0;
        //최대 물고기 갯수
        public const int MAX_FISHES = 30;


        public GameObject SetFishesPath(int nTmpID, string strUuid, string strTmpName, Vector3 vPos, exAtoolFath.InitMode mode)
        {
            // 물고기 정보 찾기
            AToolFishInfo info = ResourceManager.LoadAToolFish(strUuid);
            if (info == null)
            {
                Debug.LogError("info is null.");
                return null;
            }
            return SetFishesPath(info, vPos, mode);
        }

        public GameObject SetFishesPath(AToolFishInfo info, Vector3 vPos, exAtoolFath.InitMode mode)
        {
            exAtoolFath fishComp = null;
            int nTmpID = -1;
            string strTmpName = "", strUuid = "";

            #region Validation
            if (info == null)
            {
                Debug.LogError("info is null.");
                return null;
            }
            else
            {
                nTmpID = info.userId;
                strTmpName = info.templateName;
                strUuid = info.identifier;
            }

            // 탬플릿 오브젝트를 캐시에서 받아오거나 scene에서 찾기
            if (templateObjDict.ContainsKey(strTmpName))
            {
                fishComp = templateObjDict[strTmpName];
                if (fishComp == null)
                {
                    Debug.LogError(string.Format("Couldn't find template object named \"{0}\" in cache.", strTmpName));
                    return null;
                }
            }
            else
            {
                GameObject objTemplate = GameObject.Find(strTmpName);
                if (objTemplate == null)
                {
                    Debug.LogError(string.Format("Couldn't find template object named \"{0}\".", strTmpName));
                    return null;
                }
                fishComp = objTemplate.GetComponent<exAtoolFath>();
                if (fishComp == null)
                {
                    Debug.LogError(string.Format("Couldn't find path script of template object named \"{0}\".", strTmpName));
                    return null;
                }
                templateObjDict[strTmpName] = fishComp;
            }

            // 부모 오브젝트를 참조하지 않으면 scene에서 찾기, 없으면 만들기
            if (parentObj == null)
            {
                parentObj = GameObject.Find("AFishes");
                if (parentObj == null)
                {
                    parentObj = new GameObject("AFishes");
                    parentObj.transform.position = new Vector3(0, 240, -450);
                    parentObj.transform.parent = GameObject.Find("AToolFishes").transform;
                }
            }
            #endregion
            
            /*
             * 물고기 개수를 제한하도록 설정 (성능 이슈) 
             */
            if (s_fishes.Count >= MAX_FISHES)
            {
                GameObject go = null;
                while (s_fishes.Count > 0)
                {
                    go = s_fishes[0];
                    s_fishes.RemoveAt(0);
                    if (go != null) break;
                }

                if (go != null)
                {
                    Destroy(go);
                    System.GC.Collect();
                }
            }

            exAtoolFath newFishObj = Instantiate(fishComp, parentObj.transform, false);
            newFishObj.name = string.Format("{0}({1})", strTmpName, strUuid);
            newFishObj.initStart(info, mode);
            s_fishes.Add(newFishObj.gameObject);

            return newFishObj.gameObject;
        }

        public GameObject GetRecentFish(int userId)
        {
            for (int i = s_fishes.Count - 1; i >= 0; i--)
            {
                GameObject fish = s_fishes[i];
                if (fish == null) continue;
                var script = fish.GetComponent<exAtoolFath>();
                if (script.userId == userId)
                    return fish;
            }
            return null;
        }

        private void OnDestroy()
        {
            for (int i = 0; i < s_fishes.Count; i++)
            {
                GameObject go = s_fishes[i];
                if (go != null)
                {
                    Destroy(go);
                }
            }
            System.GC.Collect();
        }

        public string GetRecentFishIdentifier(int userId)
        {
            for (int i = s_fishes.Count - 1; i >= 0; i--)
            {
                GameObject fish = s_fishes[i];
                if (fish == null) continue;
                var script = fish.GetComponent<exAtoolFath>();
                if (script.userId == userId)
                    return script.identifier;
            }
            return null;
        }

        Vector3 GetAtoolCoord(Vector3 org)
        {

            Vector2 rePos = new Vector2(Screen.width / 2.0f + org.x, Screen.height / 2.0f + org.y);

            if (rePos.x < 0) rePos.x = 0;
            if (rePos.x > Screen.width) rePos.x = Screen.width;

            if (rePos.y < 0) rePos.y = 0;
            if (rePos.y > Screen.height) rePos.y = Screen.height;

            print(org + " : " + rePos);

            float qurd = Screen.width / 4.0f;

            Vector3 dest = Vector3.zero;

            if (rePos.x >= 0 && rePos.x < qurd)
            {
                dest = new Vector3(45, 105, 220);
                nScreenNo = 0;
            }
            else if (rePos.x >= qurd && rePos.x < qurd * 2)
            {
                dest = new Vector3(15, 105, 220);
                nScreenNo = 1;
            }
            else if (rePos.x >= qurd * 2 && rePos.x < qurd * 3)
            {
                dest = new Vector3(-15, 105, 220);
                nScreenNo = 2;
            }
            else if (rePos.x >= qurd * 3 && rePos.x < qurd * 4)
            {
                dest = new Vector3(-45, 105, 220);
                nScreenNo = 3;
            }

            return dest;
        }

        public int GetScrNo(float fX)
        {
            float qurd = Screen.width / 4.0f;

            if (fX >= 0 && fX < qurd)
            {
                return 0;
            }
            else if (fX >= qurd && fX < qurd * 2)
            {
                return 1;
            }
            else if (fX >= qurd * 2 && fX < qurd * 3)
            {
                return 2;
            }
            else if (fX >= qurd * 3 && fX < qurd * 4)
            {
                return 3;
            }

            return -1;
        }
        /*	
            public void sendTexPath(int nID, string tmpName)
            {
                string buf = string.Empty;

                buf = nID.ToString() +","+ tmpName;

                networkView.RPC("SendToClientData", RPCMode.Others, buf);
            }


            [RPC] void SendToClientData(string buf) {}
        */

        public static createFishesObj Instance()
        {
            if (s_data == null)
            {
                s_data = FindObjectOfType(typeof(createFishesObj)) as createFishesObj;
            }
            if (s_data == null)
            {
                GameObject obj = new GameObject("createFishesObj");
                s_data = obj.AddComponent(typeof(createFishesObj)) as createFishesObj;
            }
            return s_data;
        }
    }
}





