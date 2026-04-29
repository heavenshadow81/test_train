using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogManager<T> where T : MonoBehaviour
{
    public T Log;

    public int logCnt;

    public Transform parent;

    public Queue<T> queue = new Queue<T>();


    public LogManager(Transform _parent , T _Log , int _logCnt)
    { 
        Log = _Log;
        logCnt = _logCnt;
        parent = _parent;

        Init();
    }

    private void Init()
    {
        for (int i = 0; i < logCnt; i++) 
        {
            T log = GameObject.Instantiate(Log, parent);
            log.gameObject.SetActive(false);
            queue.Enqueue(log);
        }
    }


    public T GetLog()
    {
        if (queue.Count > 0)
        {
            T g = queue.Dequeue();
            g.gameObject.SetActive(true);
            return g;
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                T go = GameObject.Instantiate(Log, parent);
                go.gameObject.SetActive(false);
                queue.Enqueue(go);
            }
            T got = queue.Dequeue();
            got.gameObject.SetActive(true);

            return got;
        }
    }


    public void SetLog(T _Log)
    { 
        _Log.gameObject.SetActive(false);
        queue.Enqueue(_Log);
    }

}
