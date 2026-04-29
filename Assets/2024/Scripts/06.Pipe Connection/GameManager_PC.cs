using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_PC : MonoBehaviour
{
    public static GameManager_PC Instance { get; private set; }

    public Transform[] spawnPositions;
    public GameObject[] pipes;

    public GameObject failUI;

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Init();
    }

    public void Init()
    {
        for (int i = 0; i < pipes.Length; i++)
        {
            GetRandomSpawnPosition(pipes[i], i);
        }
    }

    private void GetRandomSpawnPosition(GameObject pipe, int index)
    {
        int randomIndex;

        // 첫 번째 pipe일 때
        if (index == 0)
        {
            // 1번 인덱스부터 마지막 인덱스까지의 범위에서 랜덤 선택
            randomIndex = Random.Range(1, spawnPositions.Length - 1);
        }
        else
        {
            // 전체 범위에서 랜덤 선택
            randomIndex = Random.Range(0, spawnPositions.Length);
        }

        // 선택된 스폰 위치의 Position 가져오기
        float randomPosX = spawnPositions[randomIndex].position.x;

        // pipe의 위치를 업데이트 (y값은 기존 y값 유지)
        Vector3 newPosition = pipe.transform.position; // 기존 위치 가져오기
        newPosition.x = randomPosX; // 랜덤으로 선택된 x값으로 업데이트
        pipe.transform.position = newPosition; // 위치 설정
    }

    public void GameOver()
    {
        Debug.Log("게임오버!!");
        int random = Random.Range(0, 2);
        string[] clipName = { "실패화면1", "실패화면2" };
        SoundMGR.Instance.SoundPlay(clipName[random]);
        failUI.SetActive(true);

        // 모든 셀 파괴
        foreach (GameObject cell in CellManager_PC.Instance.cells)
        {
            Destroy(cell); // 셀 파괴
        }

    }

    public void GameSuccess()
    {
        Debug.Log("게임 성공!!");
        StartCoroutine(HandleGameSuccess());
    }

    private IEnumerator HandleGameSuccess()
    {
        SoundMGR.Instance.SoundPlay("연결성공");

        // 각 셀의 이미지 알파 값을 0으로 설정
        foreach (GameObject cell in CellManager_PC.Instance.cells)
        {
            Image cellImage = cell.GetComponent<Image>(); // 셀의 Image 컴포넌트 가져오기
            if (cellImage != null)
            {
                Color color = cellImage.color; // 현재 색상 가져오기
                color.a = 0; // 알파 값을 0으로 설정
                cellImage.color = color; // 변경된 색상 적용
            }
        }

        // 1초 대기
        yield return new WaitForSeconds(1f);

        // 모든 셀 파괴
        foreach (GameObject cell in CellManager_PC.Instance.cells)
        {
            Destroy(cell); // 셀 파괴
        }

        Init();
        CellManager_PC.Instance.SpawnCells();
        WaterFlowManager_PC.Instance.Init();
    }
}
