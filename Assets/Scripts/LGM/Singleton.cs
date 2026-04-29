using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    GameObject manager = new GameObject();
                    instance = manager.AddComponent(typeof(T)) as T;
                    manager.name = typeof(T).ToString();
                }
            }
            return instance;
        }
    }

   

}
