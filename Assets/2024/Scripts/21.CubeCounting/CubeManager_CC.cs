using System.Collections;
using UnityEngine;

public class CubeManager_CC : MonoBehaviour
{
    public GameObject[,,] cubes = new GameObject[3, 3, 3]; // 큐브들을 저장할 3차원 배열
    private Vector3[,,] initialPositions = new Vector3[3, 3, 3]; // 큐브들의 초기 위치를 저장할 배열
    int activeCubeCount = 0; // 활성화된 큐브의 총 개수
    [SerializeField] GameObject cubeParent; // 큐브들이 자식으로 있는 부모 오브젝트

    private void Start()
    {
        AssignCubes(); // 큐브들을 배열에 담기
    }

    // 큐브들을 배열에 할당하는 메서드
    private void AssignCubes()
    {
        int index = 0;
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    if (index < cubeParent.transform.childCount)
                    {
                        GameObject cube = cubeParent.transform.GetChild(index).gameObject;
                        cubes[x, y, z] = cube;
                        initialPositions[x, y, z] = cube.transform.position; // 초기 위치 저장
                        index++;
                    }
                }
            }
        }
    }

    // 초기화 메서드
    public void Init()
    {
        DeactivateAllCubes(); // 모든 큐브 비활성화
        activeCubeCount = ActivateRandomCubes(); // 랜덤하게 큐브들을 활성화하고, 활성화된 큐브의 개수를 반환받음
    }

    // 모든 큐브 비활성화 하는 메서드
    public void DeactivateAllCubes()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    if (cubes[x, y, z] != null)
                    {
                        cubes[x, y, z].transform.position = initialPositions[x, y, z]; // 큐브를 초기 위치로 이동
                        cubes[x, y, z].SetActive(false); // 각 큐브를 비활성화
                    }
                }
            }
        }
    }

    // 랜덤하게 큐브를 활성화하는 메서드
    public int ActivateRandomCubes()
    {
        System.Random random = new System.Random();
        int activationCount = random.Next(3, 28); // 3~27 사이의 랜덤한 숫자 생성

        int activatedCubes = 0;
        while (activatedCubes < activationCount)
        {
            int x = random.Next(0, 3);
            int y = random.Next(0, 3);
            int z = random.Next(0, 3);

            if (cubes[x, y, z] != null && !cubes[x, y, z].activeSelf) // 이미 활성화되지 않은 큐브를 찾아서
            {
                cubes[x, y, z].SetActive(true); // 큐브 활성화
                activatedCubes++;
            }
        }

        return activatedCubes; // 총 활성화된 큐브의 개수 반환
    }

    // 활성화된 큐브의 개수를 반환하는 메서드
    public int GetActiveCubeCount()
    {
        return activeCubeCount;
    }
}
