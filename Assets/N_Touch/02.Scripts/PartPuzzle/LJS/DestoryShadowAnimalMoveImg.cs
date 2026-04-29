using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestoryShadowAnimalMoveImg : MonoBehaviour {

	public Transform Background_Back_PosGroupImg;
	public Transform Background_Middle_PosGroupImg;

	public Transform Background_Back_PosGroup;
	public Transform Background_Middle_PosGroup;


	public void DestoryImgs()
    {
		for (int i = 0; i < Background_Back_PosGroupImg.childCount; i++)
		{
			Destroy(Background_Back_PosGroupImg.GetChild(i).gameObject);
		}

		for (int i = 0; i < Background_Middle_PosGroupImg.childCount; i++)
		{
			Destroy(Background_Middle_PosGroupImg.GetChild(i).gameObject);
		}

		for (int i = 0; i < Background_Back_PosGroup.childCount; i++)
		{
			Background_Back_PosGroup.GetChild(i).gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
		}
		for (int i = 0; i < Background_Middle_PosGroup.childCount; i++)
		{
			Background_Middle_PosGroup.GetChild(i).gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
		}
	}
}
