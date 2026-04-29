using UnityEngine;

[RequireComponent(typeof(TouchMotionGameController))]
public class TouchMotionAlphabetController : MonoBehaviour {
    const string path = "TouchMotion/TouchAlphabetGame/Prefabs/";

    public GameObject alphabetPrefab;
        
    private int cullingLayer = 14;
    private TouchMotionAlphabetSoundManager alphabetSoundsManager;
    private TouchMotionGameController gameController;
    void Awake()
    {
        alphabetSoundsManager = GetComponent<TouchMotionAlphabetSoundManager>();
        gameController = GetComponent<TouchMotionGameController>();
        TouchMotionSmallObject[] objects = new TouchMotionSmallObject[26];

        int index = (int)'A';
        for(int i = 0 ; i < objects.Length ; ++i )
        {
            char fileName = (char)(index + i);
            GameObject alphabet = Resources.Load(path + fileName) as GameObject;
            GameObject go = Instantiate(alphabetPrefab) as GameObject;
            alphabet = Instantiate(alphabet) as GameObject;
            alphabet.layer = cullingLayer;
            alphabet.transform.parent = go.transform;
          //  go.AddComponent<BezierMove>();
            go.AddComponent<TouchAlphabetObject>();

            if (alphabetSoundsManager != null)
            {
                go.GetComponent<TouchMotionAlphabetObj>().alphabetSound = alphabetSoundsManager.GetClip(i);
                
            }

            alphabet.gameObject.SetActive(true);
            go.gameObject.SetActive(false);
            go.layer = cullingLayer;
            go.name = string.Format("{0}", fileName);
            objects[i] = go.GetComponent<TouchMotionSmallObject>();
            objects[i].deadSound = alphabetSoundsManager.GetClip(i);
        }
        gameController.smallObjPrefabs = objects;
        objects = null;
     }
    
    

}
