using UnityEngine;
using System.Collections;

public class Sample : MonoBehaviour {
	public GameObject[] Models = new GameObject[10];
	public string[] AniName = new string[10];
	public GameObject[] BackGroundModel = new GameObject[3];
	private GameObject SelectModel;
	private string SelectName;
	private int animationNumber = 0;
	private int ModelNumber = 0;

	void Start () {
		for(int i = 0; i < Models.Length; ++i) {
			for(int j = 0; j < AniName.Length; ++j) {
				Models[i].GetComponent<Animation>()[AniName[j]].wrapMode = WrapMode.Loop;
			}

			if(i != 0) Models[i].SetActive(false);
		}

		PlayAnimation();
	}
	
	void OnModelButtonNext() {
		if(ModelNumber >= 9) return;
		Models[ModelNumber].SetActive(false);
		ModelNumber++;
		Models[ModelNumber].SetActive(true);

		PlayAnimation();
	}

	void OnModelButtonPrev() {
		if(ModelNumber <= 0) return;
		Models[ModelNumber].SetActive(false);
		ModelNumber--;
		Models[ModelNumber].SetActive(true);
		PlayAnimation();
	}

	void OnBackGround() {
		for(int i = 0; i < BackGroundModel.Length; ++i) {
			BackGroundModel[i].SetActive(false);
		}

		switch(ModelNumber) {
		case 0 :
		case 1 :
		case 2 :
			BackGroundModel[0].SetActive(true);
			break;
		case 3 :
		case 4 :
		case 5 :
		case 6 :
			BackGroundModel[1].SetActive(true);
			break;
		case 7 :
		case 8 :
		case 9 :
			BackGroundModel[2].SetActive(true);
			break;
		}
	}

	void PlayAnimation() {
		OnBackGround();

		SelectModel = Models[ModelNumber];
		SelectName = AniName[animationNumber];

		SelectModel.GetComponent<Animation>().Play(SelectName);
	}

	public void OnAniButtonNext() {
		if(animationNumber >= 9) return;
		animationNumber++;
		PlayAnimation();
	}

	public void OnAniButtonPrev() {
		if(animationNumber <= 0) return;
		animationNumber--;
		PlayAnimation();
	}

	void OnGUI() {
		if (GUI.Button(new Rect(10,25,25,25), "<")) {
			OnModelButtonPrev();
		}

		if(SelectModel != null) {
			if (GUI.Button(new Rect(40,25,200,25), SelectModel.name)) {
				Debug.Log("Clicked");
			}
		}

		if (GUI.Button(new Rect(250,25,25,25), ">")) {
			OnModelButtonNext();
		}

		GUI.Label(new Rect(20,55,100,25), "Animations");

		if (GUI.Button(new Rect(10,80,25,25), "<")) {
			OnAniButtonPrev();
		}
		
		if (GUI.Button(new Rect(40,80,100,25), SelectName)) {
			Debug.Log("Clicked the button with an image");
		}
		
		if (GUI.Button(new Rect(150,80,25,25), ">")) {
			OnAniButtonNext();
		}
	}
}
