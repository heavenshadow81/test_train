using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ML.PlaywallKids.Aquarium
{
    public class BoidAFishPath : MonoBehaviour
    {
        public static Dictionary<string, Transform> _dummyDict = new Dictionary<string, Transform>();

        public static Transform GetDummy(string pathName)
        {
            Transform t = null;
            _dummyDict.TryGetValue(pathName, out t);
            return t;
        }

        public void Awake()
        {
            iTweenPath[] iTweenPaths = GetComponents<iTweenPath>();
            for (int i = 0; i < iTweenPaths.Length; i++)
            {
                iTweenPath path = iTweenPaths[i];
                List<Vector3> pathNodeList = new List<Vector3>(path.nodes);

                for (int j = 1; j <= 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        pathNodeList.Add(pathNodeList[0]);
                        pathNodeList.RemoveAt(0);
                    }
                    string pathName = string.Format("{0}_{1}", path.pathName, j);
                    GameObject go = new GameObject("_" + pathName.ToLower());
                    go.transform.parent = transform;
                    go.transform.position = pathNodeList[0];
                    _dummyDict[pathName] = go.transform;

                    go.transform.DOPath(pathNodeList.ToArray(), 55, PathType.Linear, PathMode.Full3D, 10).SetLoops(-1);
                    /*
                    iTween.MoveTo(go, iTween.Hash("path", pathNodeList.ToArray(), "time", 55,
                        "orienttopath", true, "easetype", "linear", "looptype", "loop"));
                    */
                }
            }
        }
    }
}