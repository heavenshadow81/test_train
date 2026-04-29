using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Cell2
{
    public List<GameObject> x;

    // 함정 랜덤 설치
    public void SettingTrap_Random()
    {
        int random = UnityEngine.Random.Range(0, x.Count);
        x[random].SetTrap(true);
    }

    // 함정 초기화
    public void SettingTrap_Init()
    {
        for(int i = 0; i < x.Count; i++)
        {
            x[i].SetTrap(false);
        }
    }

    // 추가
    public void Add(GameObject _obj)
    {
        x.Add(_obj);
    }

    // 현재 스크립트를 보유한 오브젝트 활성화 / 비활성화
    public bool SetActive(int index, bool active)
    {
        if (x[index].TryGetComponent(out Transform _transform))
        {
            _transform.gameObject.SetActive(active);
            return true;
        }
        return false;
    }

    // 현재 라인 활성화 / 비활성화
    public bool ActiveSelf(bool active)
    {
        foreach(GameObject _x in x)
        {
            if (_x.GetComponent<Transform>() == null) 
            {
                return false;
            }
            _x.GetComponent<Transform>().gameObject.SetActive(active);
        }
        return true;
    }
    public bool ActiveSelf(bool active, GameObject effect)
    {
        foreach (GameObject _x in x)
        {
            Transform temp = _x.GetComponent<Transform>();
            if (temp == null)
            {
                return false;
            }
            temp.gameObject.SetActive(active);
            MonoBehaviour.Instantiate(effect, temp.position, Quaternion.identity);
        }
        return true;
    }

    public void Clear(GameObject _obj)
    {
        x.Remove(_obj);
        MonoBehaviour.Destroy(_obj);
    }
    // 삭제
    public void Clear(int index)
    {
        x.RemoveAt(index);
        MonoBehaviour.Destroy(x[index]);
    }
    // 전부 삭제
    public void ClearAll()
    {
        foreach(var _x in x)
        {
            MonoBehaviour.Destroy(_x);
        }
        x.RemoveRange(0, x.Count);
    }
}

[Serializable]
public class SQLife
{
    private int count;  // 목숨 갯수
    public Transform parent;    // 목숨 부모 오브젝트
    public GameObject prefab;   // 목숨 프리팹
    [HideInInspector]
    public List<GameObject> lifeObj;   // 목숨 오브젝트 리스트

    public int Count
    {
        get
        {
            return count;
        }
        set
        {
            int temp = value - count;
            if (lifeObj != null && temp > 0)    // temp가 0보다 적으면 추가 생성
            {
                AddLife(value - count);
            }
            else if (lifeObj != null && temp < 0)   // temp가 0보다 많으면 차이만큼 감소
            {
                ReductionLif(Mathf.Abs(temp));
            }
            count = value;
        }
    }

    // 목숨 증가
    public void AddLife(int _count)
    {
        for (int i = 0; i < _count; i++)
        {
            GameObject temp = MonoBehaviour.Instantiate(prefab, parent);
            lifeObj.Add(temp);
        }
    }
    // 목숨 감소
    public void ReductionLif(int _count)
    {
        for(int i = 0; i < _count; i++)
        {
            
            MonoBehaviour.Destroy(lifeObj[0]);
            lifeObj.RemoveAt(0);
            if (Count <= 1) // 목숨 다할 시 중지
            {
                EndingEvent();
                return;
            }
        }
    }
    private void EndingEvent()
    {
        SQManager manager = SQManager.Instance;
        manager.gameOverUI.SetActive(true);
        manager.gameStart = false;
    }
}

public class SQManager : Singleton<SQManager>
{
    public int revers = -1;
    public int count;
    public SQLife life;    // 플레이어 목숨
    public Sprite[] planeType;  // 발판 종류
    public List<Cell2> cellY = new List<Cell2>();   // 맵
   
    public GameObject gameOverUI;
    public GameObject gameClearUI;
    public GameObject gameStartUI; 
    public GameObject effect;

    public Vector2Int currentID = Vector2Int.zero;

    [HideInInspector]
    public bool gameStart = false;  // 게임 시작 체크

    private void Start()
    {
        gameStartUI.SetActive(true);
        gameOverUI.SetActive(false);
        gameClearUI.SetActive(false);
        // 목숨 초기화
        life.Count = count;
        // 오브젝트 비활성화
        for(int i = 0; i < cellY.Count; i++)
        {
            cellY[i].ActiveSelf(false);
        }
        // 맵의 발판 랜덤 배치
        for (int i = 0; i < cellY.Count; i++) 
        {
            for(int j = 0; j < cellY[i].x.Count; j++)
            {
                // 보물 칸은 제외하고 랜덤한 발판으로 배치
                if (i < cellY.Count - 1) 
                    cellY[i].x[j].GetSpriteRenderer().RandomChange(planeType, 8);

                cellY[i].x[j].GetComponent<SQPlane>().id = new Vector2Int(i, j);
            }
            cellY[i].SettingTrap_Random();
        }
    }
    
    private void Update()
    {
        // 게임 시작
        if (!gameStart && Input.GetMouseButtonDown(0))
        {
            gameStartUI.SetActive(false);
            gameStart = true;
            cellY[currentID.x].ActiveSelf(true, effect);
            revers *= -1;
        }
    }

    // 함정 발판 이벤트
    public void TrapEvent()
    {
        life.Count--;   // 목숨 감소
        cellY[currentID.x].SetActive(currentID.y, false);
    }
    public void PlaneDownEvent()
    {
        // 게임 클리어
        if (revers == -1 && currentID.x == 0) 
        {
            ClearEvent();   // 클리어 이벤트 실행
            return;
        }

        // 첫 발판 클릭 시
        if (currentID.x == 0)
        {
            cellY[(currentID.x + revers)].ActiveSelf(true, effect);
            return;
        }
        // currentLine값 증가 후 currentLine의 오브젝트 활성화
        if (currentID.x != 5)
        {
            cellY[(currentID.x - revers)].ActiveSelf(false, effect);
            
        }
        cellY[(currentID.x + revers)].ActiveSelf(true, effect);
    }

    // 보물 발판 이벤트
    public void TreasurePlaneDownEvent()
    {
        /*// 보물 발판 이외의 모든 발판 비활성화
        for (int i = 0; i < cellY.Count - 1; i++)
        {
            cellY[i].ActiveSelf(false, effect);
        }*/
        //cellY[currentLine - 2].ActiveSelf(false, effect);
        revers *= -1;   // 돌아가도록 진행 방향 변경
        // 다음 발판 활성화
        cellY[currentID.x].ActiveSelf(false);
        cellY[currentID.x].ActiveSelf(true, effect);
        // 함정 재 배치
        for (int i = 0; i < cellY.Count; i++)
        {
            cellY[i].SettingTrap_Init();    // 함정 지우기
            cellY[i].SettingTrap_Random();  // 함정 설치
        }
    }

    // 클리어 이벤트
    public void ClearEvent()
    {
        gameClearUI.SetActive(true);
    }
}
