using UnityEngine;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    public class ObjectDictionaryPool<U, T> where T : Component
    {
        public T prefab;
        public bool addToParent;
        public Dictionary<U, T> activeDic { get; private set; }
        public Component parent;
        public int num
        {
            get
            {
                if (activeDic == null) return 0;
                return activeDic.Count;
            }
        }

        CObjectList<T> objPool;

        public ObjectDictionaryPool(T _prefab)
        {
            prefab = _prefab;
            Init(_prefab);
        }

        ~ObjectDictionaryPool()
        {
            activeDic.Clear();
            activeDic = null;
            Destroy();
        }

        public void Destroy()
        {
            if (activeDic != null && activeDic.Count > 0)
            {
                foreach (KeyValuePair<U, T> kv in activeDic)
                {
                    GameObject.Destroy(kv.Value.gameObject);
                }
            }

        }

        public void Init(T _prefab)
        {
            if (_prefab != null)
            {
                activeDic = new Dictionary<U, T>();
                objPool = new CObjectList<T>(
             0,
             () =>
             {
                 T clone;
                 if (addToParent)
                 { clone = NGUITools.AddChild(parent.gameObject, prefab) as T; }
                 else
                 { clone = GameObject.Instantiate(prefab) as T; }

                 clone.gameObject.SetActive(false);
                 return clone;
             },
             (T go) =>
             { return !go.gameObject.activeInHierarchy; }
             );
            }
        }

        public T GetObejct(U _id)
        {
            if (activeDic.ContainsKey(_id))
            {
                activeDic[_id].gameObject.SetActive(true);
                return activeDic[_id];
            }
            T go = objPool.GetObject();
            if (go == null) return null;

            activeDic.Add(_id, go);
            go.gameObject.SetActive(true);

            return go;
        }

        public void CheckActiveObjProcess()
        {
            if (activeDic.Count < 1) return;

            foreach (KeyValuePair<U, T> kv in activeDic)
            {
                if (!kv.Value.gameObject.activeInHierarchy)
                {
                    activeDic.Remove(kv.Key);
                }
            }
        }
    }
}