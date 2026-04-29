using UnityEngine;
using System.Collections;

public class TouchalphabetColliderManager : MonoBehaviour {
    GameObject[][] colliders;
    GameObject boxCollider;
    void Awake()
    {
        boxCollider = new GameObject();
        BoxCollider box = boxCollider.AddComponent<BoxCollider>();
        box.size = new Vector3(7, 5, 1);
        boxCollider.name = "colliderPrfab";
        boxCollider.SetActive(false);
    }

    void Start()
    {
        int index = 0;
        int total = iTweenPath.paths.Keys.Count;
        colliders = new GameObject[iTweenPath.paths.Keys.Count][];
        foreach(var key in iTweenPath.paths.Keys)
        {
            Vector3[] pos = iTweenPath.GetPath(key);
            GameObject[] objs = new GameObject[pos.Length];
            for (int i = 0,len = pos.Length; i <len ; ++i )
            {
                objs[i] = Instantiate(boxCollider) as GameObject;
                objs[i].transform.position = pos[i];
                objs[i].name = string.Format("collider_{0}_{1}", index, i);
                objs[i].SetActive(true);
                if(i!=0 || i != len-1) {objs[i].transform.localEulerAngles = new Vector3(0, 0, Random.Range(-45f,45f) );}
            }
            colliders[index++] = objs;

        }
        Debug.Log(colliders.Length);
    }
}
