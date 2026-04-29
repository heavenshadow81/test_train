using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WM_Spawner : MonoBehaviour
{
    [SerializeField] GameObject[] shapes; //도형 프리팹

    public List<GameObject> CorrectShapes = new List<GameObject>(); //정답 모양들
    public List<string> shapesName = new List<string>(); //정답 모양의 이름들
    public int random; //몇번째 도형인지 체크
    [SerializeField] Transform[] correctPos; //정답 모양 스폰 포지션

    List<GameObject> poneShapes = new List<GameObject>(); //정답 모양들
    [SerializeField] Transform[] PonePos; //폰 위의 도형 스폰 포지션

    void OnEnable()
    {
        CorrectSpawn();
        StartCoroutine(PoneSpawnRoutine());
    }

    void CorrectSpawn()
    {
        // 도형을 담을 리스트 생성 (중복 방지 위해)
        List<GameObject> availableShapes = new List<GameObject>(shapes);

        for (int i = 0; i < correctPos.Length; i++)
        {
            // 남은 도형 중에서 랜덤으로 선택
            int randomIndex = Random.Range(0, availableShapes.Count);

            GameObject shape = Instantiate(availableShapes[randomIndex], correctPos[i].transform);

            //쉐이프 박스콜라이더 비활성화
            shape.GetComponent<BoxCollider>().enabled = false;

            // 선택된 도형을 리스트에서 제거하여 중복 방지
            availableShapes.RemoveAt(randomIndex);

            // "(Clone)" 부분을 제거한 후 CorrectShapes 리스트에 저장
            string shapeNameWithoutClone = shape.name.Replace("(Clone)", "").Trim();
            shapesName.Add(shapeNameWithoutClone);
            CorrectShapes.Add(shape);
        }
    }

    private IEnumerator PoneSpawnRoutine()
    {
        while (true)  // 무한 반복, 필요하면 조건 추가
        {
            random = Random.Range(0, CorrectShapes.Count);
            // 정답 쉐이프중에 한가지 랜덤한 위치에 생성
            GameObject correctShape = Instantiate(CorrectShapes[random], PonePos[Random.Range(0, PonePos.Length)].transform);
            poneShapes.Add(correctShape);
            Destroy(correctShape, 4f);

            // 도형을 담을 리스트 생성 (중복 방지 위해)
            List<GameObject> availableShapes = new List<GameObject>();

            // CorrectShapes에 포함되지 않은 도형들만 availableShapes에 추가
            foreach (GameObject shape in shapes)
            {
                if (!shapesName.Contains(shape.name))
                {
                    availableShapes.Add(shape);
                }
            }

            for (int i = 0; i < PonePos.Length; i++)
            {
                int randomIndex = Random.Range(0, availableShapes.Count);

                // 정답 리스트 안에 없고 폰 포지션에 자식 객체가 없으면 도형 생성
                if (!CorrectShapes.Contains(availableShapes[randomIndex]) & PonePos[i].transform.childCount == 0)
                {
                    GameObject shape = Instantiate(availableShapes[randomIndex], PonePos[i].transform);
                    poneShapes.Add(shape);

                    // 선택된 도형을 리스트에서 제거하여 중복 방지
                    availableShapes.RemoveAt(randomIndex);

                    shape.GetComponent<BoxCollider>().enabled = false; //안보일 때 터치 방지

                    Destroy(shape, 4f);
                }
            }
            //생성된 correctShape를 1초 뒤에 BoxCollider 활성화하고 2초뒤에 비활성화
            yield return new WaitForSeconds(1f);
            PoneTrue();
            yield return new WaitForSeconds(2f);
            PoneFalse();
            // 다음 반복 전 1초 대기
            yield return new WaitForSeconds(1f);
        }
    }

    public void PoneFalse()
    {
        for(int i = 0;i < poneShapes.Count;i++)
        {
            if(poneShapes[i]!=null)
            poneShapes[i].GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void PoneTrue()
    {
        for (int i = 0; i < poneShapes.Count; i++)
        {
            if (poneShapes[i] != null)
                poneShapes[i].GetComponent<BoxCollider>().enabled = true;
        }
    }

    public void CorrectExtra()
    {
        // 도형을 담을 리스트 생성 (중복 방지 위해)
        List<GameObject> availableShapes = new List<GameObject>();

        // CorrectShapes에 포함되지 않은 도형들만 availableShapes에 추가
        foreach (GameObject shapes in shapes)
        {
            if (!shapesName.Contains(shapes.name))
            {
                availableShapes.Add(shapes);
            }
        }
        // 남은 도형 중에서 랜덤으로 선택
        int randomIndex = Random.Range(0, availableShapes.Count);
        
        GameObject shape = Instantiate(availableShapes[randomIndex], CorrectShapes[random].transform.parent.transform);
        Destroy(CorrectShapes[random]);
        CorrectShapes.RemoveAt(random);

        //쉐이프 박스콜라이더 비활성화
        shape.GetComponent<BoxCollider>().enabled = false;

        // 선택된 도형을 리스트에서 제거하여 중복 방지
        availableShapes.RemoveAt(randomIndex);

        // "(Clone)" 부분을 제거한 후 CorrectShapes 리스트에 저장
        string shapeNameWithoutClone = shape.name.Replace("(Clone)", "").Trim();
        shapesName.Add(shapeNameWithoutClone);
        CorrectShapes.Add(shape);
    }
}

