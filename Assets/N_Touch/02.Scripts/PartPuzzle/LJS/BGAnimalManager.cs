using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 해상도에 따른 위치 확인
/// y -140 ~ -10
/// </summary>

public class BGAnimalManager : MonoBehaviour {

	public Sprite[] animalsImg;
	public Transform[] moveToPositions;
	Dictionary<int, bool> duplicationCheckdic = new Dictionary<int, bool>();

	int animalNumber;

	int[] ranArr;
	int ranLength;

	public GameObject moveAnimalPosPrefabs;
	public Transform targetPosGroup;

	int tempOrder;

	public Transform BGAnimalPos;

	public Transform Background_Back_PosGroupImg;
	public Transform Background_Middle_PosGroupImg;

	public Transform Background_Back_PosGroup;
	public Transform Background_Middle_PosGroup;
	void Awake()
    {
		//animalNumber = animalsImg.Length;
		//ranArr = new int[animalsImg.Length] ;
		//moveToPositions = new Transform[animalsImg.Length];
		//ranLength = animalsImg.Length;
		
		//float xPos = 64; //badcode 
		//float yPos = 650; //badcode 
		//for (int i = 0; i < animalsImg.Length; i++)
		//{
		//	GameObject go = Instantiate(moveAnimalPosPrefabs);
		//	moveToPositions[i] = go.transform;
		//	//go.transform.position = new Vector2(xPos, UnityEngine.Random.Range(600, 750));
		//	go.transform.position = new Vector2(xPos, yPos);
		//	go.transform.SetParent(targetPosGroup);
		//	xPos += Screen.width/animalsImg.Length;
		//}
		animalNumber = animalsImg.Length;
		ranArr = new int[5];
		moveToPositions = new Transform[5];
		ranLength = animalsImg.Length;

		float xPos = Screen.width / 5; //64 badcode 
		float yPos = 650; //badcode 
		for (int i = 0; i < 5; i++)
		{
			GameObject go = Instantiate(moveAnimalPosPrefabs);
			moveToPositions[i] = go.transform;
			//go.transform.position = new Vector2(xPos, UnityEngine.Random.Range(600, 750));
			go.transform.position = new Vector2(xPos, yPos)+ new Vector2(i * 50, 0);
			go.transform.SetParent(targetPosGroup);
			xPos += Screen.width / animalsImg.Length;
		}
	}

	void Start () {

		for (int i = 0; i < animalNumber; i++)
        {
			duplicationCheckdic.Add(i, false);
		}
		PositionChange();
	}
	
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
			foreach (var item in duplicationCheckdic)
			{
				print(item.Key+ " " + item.Value);
			}
			DuplicationCheckdicReset();
            for (int i = 0; i < BGAnimalPos.childCount; i++)
            {
				Destroy(BGAnimalPos.GetChild(i).gameObject);
			}
		}
    }
	//중복
    internal bool DuplicationCheck(int animalNum)
    {
		foreach (var item in duplicationCheckdic)
        {
            if (item.Key == animalNum && item.Value == false)
            {
				duplicationCheckdic.Remove(animalNum);
				duplicationCheckdic.Add(animalNum, true);
				return false;
            }
        }
		return true;
    }

	//갱신
	public void DuplicationCheckdicReset()
    {
		//for (int i = 0; i < BGAnimalPos.childCount; i++)
		//{
		//	Destroy(BGAnimalPos.GetChild(i).gameObject);
		//}

		//for (int i = 0; i < Background_Back_PosGroupImg.childCount; i++)
		//{
		//	Destroy(Background_Back_PosGroupImg.GetChild(i).gameObject);
		//}

		for (int i = 0; i < Background_Back_PosGroup.childCount; i++)
		{
			Background_Back_PosGroup.GetChild(i).gameObject.GetComponent<Image>().color = new Color(1,1,1,0);
		}

        if (SceneManager.GetActiveScene().name.Contains("Animal"))
        {
			//for (int i = 0; i < Background_Middle_PosGroupImg.childCount; i++)
			//{
			//	Destroy(Background_Middle_PosGroupImg.GetChild(i).gameObject);
			//}

			for (int i = 0; i < Background_Middle_PosGroup.childCount; i++)
			{
				Background_Middle_PosGroup.GetChild(i).gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
			}
        }


		foreach (var item in duplicationCheckdic.Keys.ToList())
        {
			duplicationCheckdic[item] = false;
		}

		tempOrder = 0;

		PositionChange();
	}

	// 위치 변경
	public void PositionChange()
    {
		// 동물수에따른 랜덤수
		//ranArr = Enumerable.Range(0, ranLength).ToArray();
		//for (int i = 0; i < ranLength; ++i)
		//{
		//	int ranIdx = UnityEngine.Random.Range(i, ranLength);
		//	int tmp = ranArr[ranIdx];
		//	ranArr[ranIdx] = ranArr[i];
		//	ranArr[i] = tmp;

		//	moveToPositions[i].gameObject.name =  ranArr[i].ToString();
		//}
		ranArr = Enumerable.Range(0, 5).ToArray();
		for (int i = 0; i < 5; ++i)
		{
			int ranIdx = UnityEngine.Random.Range(i, 5);
			int tmp = ranArr[ranIdx];
			ranArr[ranIdx] = ranArr[i];
			ranArr[i] = tmp;

			moveToPositions[i].gameObject.name = ranArr[i].ToString();
		}
	}

	//수정필요
	public Transform CheckMoveAnimalPos()
    {
		Transform pos = moveToPositions[tempOrder];

        for (int i = 0; i < targetPosGroup.childCount; i++)
        {
            if (targetPosGroup.GetChild(i).name == tempOrder.ToString())
            {
				pos = targetPosGroup.GetChild(i).transform;
				tempOrder++;
				return pos;
			}
        }
		return pos;
	}
}
