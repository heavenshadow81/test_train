using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ML.PlaywallKids.TwoDimensionDrawScene
{
    /// <summary>
    /// 메모리 풀 Wrapping class
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        /// <summary>
        /// 복제 될 목표 객체
        /// </summary>
        public GameObject prefab;
        /// <summary>
        /// 현재 GameObject의 자식 객체로 등록 판별
        /// </summary>
        public bool addToParent;
        /// <summary>
        /// 현재 동작 중인 객체
        /// </summary>
        public List<GameObject> ActiveObjectsList { get; private set; }

        public int Count
        {
            get
            {
                if (ActiveObjectsList == null)
                    return 0;
                return ActiveObjectsList.Count;
            }
        }

        bool bActive;
        /// <summary>
        /// 현재 동작 중인 코루틴 함수 참조 변수
        /// </summary>
        Coroutine checkCoroutine;
        /// <summary>
        /// 메모리 풀 클래스
        /// </summary>
        CObjectList<GameObject> objPool;

        /// <summary>
        /// CobjectList class 초기화(Memory Pool) 함수
        /// </summary>
        public void Initialize()
        {
            if (objPool != null) return;

            if (ActiveObjectsList != null)
                DeleteActiveObject(false);
            else
                ActiveObjectsList = new List<GameObject>();

            if (prefab != null)
            {
                objPool = new CObjectList<GameObject>(
             0,
             () =>
             {
                 GameObject clone;
                 if (addToParent)
                 { clone = NGUITools.AddChild(this.gameObject, prefab) as GameObject; }
                 else
                 { clone = Instantiate(prefab) as GameObject; }

                 clone.SetActive(false);
                 return clone;
             },
             (GameObject go) =>
             { return !go.activeInHierarchy; }
             );
            }
        }

        /// <summary>
        /// 현재 활성 화 된 객체들 삭제
        /// </summary>
        void DeleteActiveObject(bool memoryClean = true)
        {
            if (ActiveObjectsList != null)
            {
                for (int i = 0; i < ActiveObjectsList.Count; ++i)
                    Object.Destroy(ActiveObjectsList[i]);
                if (memoryClean)
                {
                    ActiveObjectsList.TrimExcess();
                    ActiveObjectsList = null;
                }
            }
        }

        void OnEnable()
        {
            bActive = this.gameObject.activeInHierarchy;
            Initialize();
        }

        void OnDisable()
        {
            bActive = false;
            StopCheckProcess();
            checkCoroutine = null;
            for (int i = 0; i < objPool.count; ++i)
            {
                Destroy(objPool.GetObject(i));
            }
            DeleteActiveObject();
            objPool.Destroy();
            objPool = null;
        }

        /// <summary>
        /// 메모리 풀에서 객체 Pop 함수
        /// </summary>
        /// <returns></returns>
        public GameObject GetObejct()
        {
            GameObject go = objPool.GetObject();
            if (go == null) return null;

            go.SetActive(true);
            ActiveObjectsList.Add(go);
            return go;
        }

        /// <summary>
        /// 비활성 중인 객체 검색 코루틴 trigger 함수
        /// </summary>
        /// <returns></returns>
        public bool CheckObjectsState()
        {
            if (prefab == null) return false;
            checkCoroutine = StartCoroutine(CheckObjectsStateProcess());
            return true;
        }

        public void StopCheckProcess()
        {
            if (checkCoroutine != null)
                StopCoroutine(checkCoroutine);
        }

        /// <summary>
        /// 비활성화 된 객체 검색 및 ActiveObjectsList의 Item 참조 관리
        /// </summary>
        /// <returns></returns>
        IEnumerator CheckObjectsStateProcess()
        {
            do
            {
                yield return new WaitForSeconds(0.5f);
                int num = ActiveObjectsList.Count;
                for (int i = 0; i < num;)
                {
                    if (i >= num) break;

                    if (!ActiveObjectsList[i].activeInHierarchy)
                    {
                        int last = num - 1;
                        if (last < 0) break;

                        ActiveObjectsList[i] = ActiveObjectsList[last];
                        ActiveObjectsList.RemoveAt(last);
                        --num;
                    }
                    else
                    { ++i; }
                }

            } while (bActive);
        }
    }
}