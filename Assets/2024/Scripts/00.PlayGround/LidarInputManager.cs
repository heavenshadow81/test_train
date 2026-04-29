using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidarInputManager : MonoBehaviour
{
    public static LidarInputManager Instance;

    private Vector2 latestDragPosition; // Lidar 좌표 저장

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 변경 시에도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject); // 이미 Instance가 있으면 중복 파괴
        }
    }

    public void UpdateDragPosition(float x, float y)
    {
        latestDragPosition = new Vector2(x, y);
    }

    public Vector2 GetLatestDragPosition()
    {
        return latestDragPosition;
    }
}
