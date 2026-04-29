using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullingMGR<T> where T : MonoBehaviour
{
    public T CheckPrefabs;

    public int idx = 10;

    public Transform parent;
    public Queue<T> CheckQueue = new Queue<T>();

    /// <summary>
    /// T ÇÁ¸®ÆÕ , »ý¼º°¹¼ö
    /// </summary>
    /// <param name="_checkPrefabs"></param>
    /// <param name="_idx"></param>
    public PullingMGR(T _checkPrefabs , int _idx)
    { 
        this.CheckPrefabs = _checkPrefabs;
        this.idx = _idx;


        for (int i = 0; i < idx; i++)
        {
            T b = CreatePulling();
            CheckQueue.Enqueue(b);
        }
    }
    public PullingMGR(T _checkPrefabs,Transform _parent , int _idx)
    {
        this.CheckPrefabs = _checkPrefabs;
        this.idx = _idx;
        this.parent = _parent;

        for (int i = 0; i < idx; i++)
        {
            T b = CreatePulling();
            b.transform.SetParent(parent);
            CheckQueue.Enqueue(b);
        }
    }


    private T CreatePulling()
    { 
        T newobj = Object.Instantiate(CheckPrefabs);
        newobj.gameObject.SetActive(false);
        return newobj;
    }



    public void ReturnObj(T ob)
    {
        if (ob != null)
        { 
            ob.gameObject.SetActive(false);
            CheckQueue.Enqueue(ob);
        }
    }


 

    public T GetObj()
    {
        if (CheckQueue.Count > 0)
        {
            T g = CheckQueue.Dequeue();
            g.gameObject.SetActive(true);
            return g;
        }
        else
        {
            for (int i = 0; i < 5; i++)
            { 
                T go = CreatePulling();
                go.transform.SetParent(parent);
                go.gameObject.SetActive(false);
                CheckQueue.Enqueue(go);
            }
            T got = CheckQueue.Dequeue();
            got.gameObject.SetActive(true);

            return got;
        }
    }




}
