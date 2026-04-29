using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalParameter : MonoBehaviour
{
    static TotalParameter _instance;

    public static TotalParameter Instance { get => _instance; }

    // 화살표 버튼 총 개수
    [Tooltip("화살표 종류 총 개수")]
    public int arrowTotalCount;

    // 플레이 인원수
    [Tooltip("총 플레이 인원수")]
    public int persons;

    // 캐릭터 완료 버튼 클릭 수
    [Tooltip("캐릭터 완료 버튼 클릭 개수")]
    public int readyCount;

    // 화살표 방향 종류
    public enum Arrow
    {
        none,
        right,
        left,
        front,
        back
    }

    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
        }
    }
}

