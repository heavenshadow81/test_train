using UnityEngine;
using System.Collections;

public class TouchAlphabetSpawner : MonoBehaviour {
    CObjectListToDic<int, GameObject> collectionOfAlphabets;
    public Camera gameCam;
    const string path = "TouchMotion/TouchAlphabetGame/Prefabs/";
    public GameObject alphabetPrefab;
    [HideInInspector]
    public bool bSpawn;
    private TouchMotionSmallObject[] prefabs;
    private int cullingLayer = 14;
   
    void Awake()
    {
        prefabs = new TouchMotionSmallObject[26];
        bSpawn = true;
        int index = (int)'A';
        for (int i = 0; i < prefabs.Length; ++i)
        {
            char fileName = (char)(index + i);
            GameObject alphabet = Resources.Load(path + fileName) as GameObject;
            GameObject go = Instantiate(alphabetPrefab) as GameObject;
            alphabet = Instantiate(alphabet) as GameObject;
            alphabet.layer = cullingLayer;
            alphabet.transform.parent = go.transform;
            alphabet.gameObject.SetActive(true);
            go.gameObject.SetActive(false);
            go.layer = cullingLayer;
            go.name = string.Format("{0}", fileName);
            prefabs[i] = go.GetComponent<TouchMotionSmallObject>();
        }

        collectionOfAlphabets = new CObjectListToDic<int, GameObject>(
            (int _index)=>
            {
                int length = prefabs.Length;
                while (_index >= length)
                {   _index -= length; };
                GameObject go = Instantiate(prefabs[_index].gameObject) as GameObject;
                return go;
            },
            (GameObject go)=>
            {
                if (go == null) return false;
                return !go.activeInHierarchy;  }
            );

        this.GetComponent<DoTweenMove>().DOPath(StopSpawn);
        StartCoroutine(SpawnProcess());
    }

    void StopSpawn()
    {
        bSpawn = false;
        StopCoroutine(SpawnProcess());
    }

    IEnumerator SpawnProcess()
    {
        float fWaitTime = 0;
        Transform parent = gameCam.transform.parent;
        do
        {
            fWaitTime =  Random.Range(0.5f, 1f);
            
            int len = Random.Range(7, 9);
            
            for (int i = 0; i < len; ++i)
            {
                GameObject go = collectionOfAlphabets.getObject(Random.Range(0, prefabs.Length));
                go.GetComponent<TouchMotionSmallObject>().cam = gameCam;

                float n = (i < len / 2) ? -1.2f : 1.2f;
                Vector3 pos = Vector3.zero;
                pos.x = (i % (len * 0.5f) + 0.5f) * n;
                pos.y = Random.Range(-2.5f, 0f);
                pos.z = Random.Range(-3.5f, 3.5f);
                go.transform.position = transform.position + pos;
                go.SetActive(true);
            }
            yield return new WaitForSeconds(fWaitTime);

        } while (bSpawn);
    }
}
