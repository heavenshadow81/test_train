using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/Example/Level Changer")]
public class SGT_LevelChanger : SGT_Singleton<SGT_LevelChanger>
{
    [SerializeField]
    private bool ignoreSceneZero;

    [SerializeField]
    private bool autoSwitch;

    /*[SerializeField]*/
    private bool removeCameraMessage;

    public bool IgnoreSceneZero
    {
        set
        {
            ignoreSceneZero = value;
        }

        get
        {
            return ignoreSceneZero;
        }
    }

    public bool AutoSwitch
    {
        set
        {
            autoSwitch = value;
        }

        get
        {
            return autoSwitch;
        }
    }

    new public void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(this);

        // Register the SceneManager.sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void Start()
    {
        if (removeCameraMessage == true)
        {
            RemoveMessages();
        }

        if (autoSwitch == true)
        {
            LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    // This method will be called when a scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (removeCameraMessage == true)
        {
            RemoveMessages();
        }
    }

    public void OnGUI()
    {
        var rectSize = new Vector2((float)Screen.width * 0.1f, (float)Screen.height * 0.05f);

        var rect1 = new Rect(10, 50, rectSize.x, rectSize.y);
        var rect2 = new Rect(10, 10 + rect1.yMax, rectSize.x, rectSize.y);
        var rect3 = new Rect(10, 10 + rect2.yMax, rectSize.x, rectSize.y);

        if (GUI.Button(rect1, "Prev"))
        {
            LoadLevel(SceneManager.GetActiveScene().buildIndex - 1);
        }

        if (GUI.Button(rect2, "Next"))
        {
            LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if (removeCameraMessage == false)
        {
            if (GUI.Button(rect3, "Hide Text"))
            {
                removeCameraMessage = true;

                RemoveMessages();
            }
        }
    }

    private void LoadLevel(int index)
    {
        var minIndex = ignoreSceneZero == true ? 1 : 0;

        if (index < minIndex)
        {
            index = SceneManager.sceneCount - 1;
        }

        if (index >= SceneManager.sceneCount)
        {
            index = minIndex;
        }
        SceneManager.LoadSceneAsync(index);
    }

    private void RemoveMessages()
    {
        if (Application.isPlaying == true)
        {
            var cameraMessages = FindObjectsOfType(typeof(SGT_CameraMessage)) as SGT_CameraMessage[];

            if (cameraMessages != null)
            {
                foreach (var cameraMessage in cameraMessages)
                {
                    Destroy(cameraMessage);
                }
            }
        }
    }

    private void OnDestroy()
    {
        // Unregister the event when the object is destroyed to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
