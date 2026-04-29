using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShapeMatching
{
    public class RandomSpawner : MonoBehaviour
    {
        public GameObject[] box;
        List<Vector3> boxPostion;

        public GameObject[] shape;
        List<Vector3> shapePostion;

        // Start is called before the first frame update

        void OnEnable()
        {
            boxPostion = new List<Vector3>();
            shapePostion = new List<Vector3>();

            RandomPosition(box,boxPostion);
            RandomPosition(shape,shapePostion);
        }

        void RandomPosition(GameObject[] array, List<Vector3> arrayPosition)
        {
            HashSet<Vector3> uniquePositions = new HashSet<Vector3>();

            while (uniquePositions.Count < array.Length)
            {
                // box 오브젝트에서 랜덤한 위치를 선택
                int randomIndex = Random.Range(0, array.Length);
                Vector3 randomPosition = array[randomIndex].transform.position;

                // HashSet에 추가 (중복되는 경우 자동으로 걸러짐)
                uniquePositions.Add(randomPosition);
            }

            // HashSet을 List로 변환하여 boxPostion에 할당
            arrayPosition = new List<Vector3>(uniquePositions);

            for (int i = 0; i < array.Length; i++)
            {
                array[i].transform.position = arrayPosition[i];
            }
        }
    }
}