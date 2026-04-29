using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 오브젝트 타켓 정보 가지고 있는 스크립트
public class MonsterPath : MonoBehaviour
{
    [System.Serializable]
    public class PathWaypoint
    {
        // 경로 이름
        public string clossLoadNum;
        // 경로 Transform
        public Transform[] loadTr;
        // 경로 position
        public Vector3[] loadPos;
    }

    // 지상, 해양, 공중 경로
    public Transform[] groundCreateTr, seaCreateTr, flyCreateTr;

    // 싱글톤
    static MonsterPath _instant;

    // 외부 수정 불가, 읽기만 가능 하도록
    public static MonsterPath Instance { get => _instant; }

    // 지상 경로, 해양 경로, 하늘 경로, 백그라운드 오브젝트 경로
    public PathWaypoint[] groundWaypoint, seaWaypoint, flyWaypoint, backWaypoint;

    private void Awake()
    {
        if (!_instant)
        {
            _instant = this;
        }
    }
}
