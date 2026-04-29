#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[AddComponentMenu("")]
[ExecuteInEditMode()]
public class EditorUpdateCheck : MonoBehaviour //이다인 서버오류 때문에 24.10.30 수정
{
    int currentVersion = 370;
    public bool respondInAlerts = false;
    public bool readyForAction = false;
    public bool readyToDestroy = false;
    public string falseMessage = "";
    public bool querying = false;
    double startTime;
    double timeout = 30;

    void Awake()
    {
        gameObject.hideFlags = HideFlags.HideAndDontSave;
        querying = true;
        startTime = EditorApplication.timeSinceStartup;
    }

    void Update()
    {
        if (readyForAction)
        {
            readyForAction = false;
            if (!string.IsNullOrEmpty(falseMessage) && EditorUtility.DisplayDialog("Update Available!", falseMessage, "Update Now!", "No Thanks"))
                Application.OpenURL("com.unity3d.kharma:content/9209");
            querying = false;
        }
        if (!querying)
        {
            StopChecking();
        }
        else
        {
            if (EditorApplication.timeSinceStartup > startTime + timeout)
            {
                if (respondInAlerts)
                    Debug.Log("SoundManagerPro update request timed out. Try again.");
                StopChecking();
            }
        }
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }

    // 서버 연결 없이도 오류 없이 동작하도록 수정된 QueryUpdates 메서드
    public void QueryUpdates(bool alerts)
    {
        // 연결된 서버가 없으므로 대체 메시지를 표시합니다.
        respondInAlerts = alerts;
        //Debug.Log("서버 업데이트 확인을 생략했습니다.");
        querying = false;  // 서버가 없으므로 즉시 querying을 false로 설정
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void StopChecking()
    {
        StopAllCoroutines();
        DestroyImmediate(gameObject);
    }
}
#endif