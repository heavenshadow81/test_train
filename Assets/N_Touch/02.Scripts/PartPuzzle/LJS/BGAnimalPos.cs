using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGAnimalPos : MonoBehaviour {
	public GameObject posPrefabs;
	public Transform MoveImgGroup;
	public float height = 640;
	public float imageWidth = 85;
	int temp = 0;


	internal void SetPosition(int animalcount)
    {	
        for (int i = 0; i < animalcount; i++)
        {
			GameObject go = Instantiate(posPrefabs);
			go.transform.SetParent(MoveImgGroup);
			//go.transform.position = new Vector2(Random.Range(0,Screen.width), 540);
			//go.transform.position = new Vector2(Screen.width / animalcount+(Screen.width/animalcount*i), 540);
			float calWidth = Screen.width / animalcount;
            if (animalcount==6) temp = -140; //bad code
            else temp = -100; //bad code
			go.transform.position = new Vector2(calWidth + temp+(calWidth * i) , height);
			go.transform.name = "Pos" + i;
		}
    }
}
