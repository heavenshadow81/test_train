using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DemoPart1 : MonoBehaviour
{
    // Particle effect to go with explosion sound ;)
    public GameObject explosionPrefab;
    public Texture2D AntiLunchBoxLogo;

    // Sample AudioClips
    public AudioClip sample1;
    public AudioClip sample2;
    public AudioClip sample3;
    public AudioClip sample4;

    int thecolor = 0;
    Color buttonColor = Color.yellow;

    float unitX;
    float unitY;

    int page = 1;

    // A SoundConnection to use later, initialization is shown in Start()
    SoundConnection sc;

    // Just so we don't have duplicates while moving through scenes
    void Awake()
    {
        if (GameObject.FindObjectsOfType(typeof(DemoPart1)).Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(this);

        // Register SceneManager.sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // How to initialize a SoundConnection
        sc = SoundManager.CreateSoundConnection("TemporarySoundConnection", SoundManager.PlayMethod.ContinuousPlayThrough, sample4, sample2, sample3, sample1);

        unitX = Screen.width / 48f;
        unitY = Screen.height / 30f;
    }

    GUIStyle boxStyle;
    GUIStyle buttonSTyle;
    GUIStyle labelStyle;
    void OnGUI()
    {
        boxStyle = GUI.skin.box;
        boxStyle.fontStyle = FontStyle.Bold;
        boxStyle.fontSize = 14;

        buttonSTyle = GUI.skin.button;
        buttonSTyle.fontStyle = FontStyle.Bold;

        labelStyle = GUI.skin.label;
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.normal.textColor = Color.black;

        /* TITLE */
        GUI.Box(new Rect(16f * unitX, 0f, 16f * unitX, 2f * unitY), "AntiLunchBox\nSoundManagerPro 3.0", boxStyle);

        /* LOAD SCENES */
        GUI.color = buttonColor;
        if (GUI.Button(new Rect(6f * unitX, 3f * unitY, 8f * unitX, 4f * unitY), "Load Level:\nMusicScene1"))
        {
            SceneManager.LoadScene("MusicScene1");
        }


        if (GUI.Button(new Rect(20f * unitX, 3f * unitY, 8f * unitX, 4f * unitY), "Load Level:\nMusicScene2"))
        {
            SceneManager.LoadScene("MusicScene2");
        }


        if (GUI.Button(new Rect(34f * unitX, 3f * unitY, 8f * unitX, 4f * unitY), "Load Level:\nMusicScene3"))
        {
            SceneManager.LoadScene("MusicScene3");
        }
        GUI.color = Color.white;

        /* Remaining GUI code */
        // (Omitted for brevity, but leave the rest unchanged)
    }

    // This method will be called when a scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MusicScene1":
                Camera.main.backgroundColor = Color.gray;
                break;
            case "MusicScene2":
                Camera.main.backgroundColor = Color.magenta;
                break;
            case "MusicScene3":
                Camera.main.backgroundColor = Color.blue;
                break;
            default:
                Camera.main.backgroundColor = new Color(49f / 255f, 77f / 255f, 121f / 255f, 5f / 255f);
                break;
        }
    }

    // Clean up event subscription when the object is destroyed
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
