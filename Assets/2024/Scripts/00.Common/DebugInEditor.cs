using UnityEngine;

public static class DebugInEditor
{
    public static bool EnableDebugLogs = true; // 디버그 로그 활성화 여부
    public static bool EnableDebugDraw = true; // 디버그 드로우 활성화 여부

    // 로그 출력
    [HideInCallstack]
    public static void Log(string message)
    {
#if UNITY_EDITOR
        if (EnableDebugLogs)
        {
            UnityEngine.Debug.Log(message);
        }
#endif
    }

    public static void LogWarning(string message)
    {
#if UNITY_EDITOR
        if (EnableDebugLogs)
        {
            UnityEngine.Debug.LogWarning(message);
        }
#endif
    }

    public static void LogError(string message)
    {
#if UNITY_EDITOR
        if (EnableDebugLogs)
        {
            UnityEngine.Debug.LogError(message); 
        }
#endif
    }

    // 드로우 레이
    public static void DrawRay(Vector3 start, Vector3 direction, Color color, float duration = 0f)
    {
#if UNITY_EDITOR
        if (EnableDebugDraw)
        {
            UnityEngine.Debug.DrawRay(start, direction, color, duration);
        }
#endif
    }

    // 드로우 라인
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0f)
    {
#if UNITY_EDITOR
        if (EnableDebugDraw)
        {
            UnityEngine.Debug.DrawLine(start, end, color, duration);
        }
#endif
    }
}
