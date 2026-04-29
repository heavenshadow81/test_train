using UnityEngine;
using System.Collections;

public class Sample2 : MonoBehaviour {
	public string[] AniName = new string[10];

	private int AniNumber = 0;
	private string AniStr = "";

	void Start () {
		Animation[] animations = GetComponentsInChildren<Animation>();
		foreach( Animation anis in animations ){
			for(int i = 0; i < AniName.Length; ++i) {
				anis.GetComponent<Animation>()[AniName[i]].wrapMode = WrapMode.Loop;
			}
		}

		PlayAnimation();
	}

	void PlayAnimation() {
		Animation[] animations = GetComponentsInChildren<Animation>();
		foreach( Animation anis in animations ){
			AniStr = AniName[AniNumber];
			anis.Play(AniName[AniNumber]);
		}
	}

	void OnButtonNext(){
		if(AniNumber >= 9) return;
		AniNumber++;
		PlayAnimation();
	}
	
	void OnButtonBack(){
		if(AniNumber <= 0) return;
		AniNumber--;
		PlayAnimation();
	}

	void OnGUI() {
		if (GUI.Button(new Rect(150,40,25,25), "<")) {
			OnButtonBack();
		}
		
		if (GUI.Button(new Rect(185,40,200,25), AniStr)) {
			Debug.Log("Clicked");
		}
		
		if (GUI.Button(new Rect(395,40,25,25), ">")) {
			OnButtonNext();
		}
	}
}
