using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CsAvatarManager : MonoBehaviour {
	
	public Text txtDebug;

	public Transform Avatar1;
	public Transform Avatar2;
	
	public bool bCurAvatar1; //현재 아바타가 선택되어져 있는지 여부
	public bool bCurAvatar2; //현재 아바타가 선택되어져 있는지 여부
	public float fDisabledist;  //모델을 감추기 위해 이동시킬 거리
	
	void OnGUI() {
		int w = Screen.width / 2;
		int h = Screen.height / 2;

		//Button Click Event 처리.
		if(GUI.Button(new Rect(w*0.5f-60, h*1.7f, 120, 50), "TestButton1")) {
			txtDebug.text = "Button1 Clicked...";	
			
			Avatar1.RotateAround(Avatar1.transform.position, Vector3.up, 10);
//          UF_Test();
		};
		

		if(GUI.Button(new Rect(w*1.5f-60, h*1.7f, 120, 50), "TestButton2")) {
			txtDebug.text = "Button2 Clicked...";		
			Avatar1.RotateAround(Avatar1.transform.position, Vector3.up, -10);
			//UF_Test();
		};

		//GUI.Button(new Rect(w*1.5f-60, h*1.7f, 120, 50), "TestButton2");
		
	}	

	
	public void Visible(string visible)
	{
		txtDebug.text = "Visible Function Entered...";				
		
		if(visible == "AvatarPSY")
		{
			
			txtDebug.text = "Avatar1 Selected...";				
			
			if(bCurAvatar1 == false)
			{
				bCurAvatar1 = true;	 Avatar1.Translate((-1)*fDisabledist,0,0);
				bCurAvatar2 = false; Avatar2.Translate((+1)*fDisabledist,0,0);
			}
		}
			
		if(visible == "AvatarGirl1")
		{
			txtDebug.text = "Avatar2 Selected...";				

			if(bCurAvatar2 == false)
			{
				bCurAvatar2 = true;	 Avatar2.Translate((-1)*fDisabledist,0,0);
				bCurAvatar1 = false; Avatar1.Translate((+1)*fDisabledist,0,0);
			}
		}
			//			
//			
//		}
//		
//		
//		if(visible == "Avatar2")
//		{
//			
//		}
//		
			
		
//		txtDebug.text = "Visible Enter...";	
//
//		    Avatar1.Rotate(0, 0, 30);
//		
//		if(visible == "true")
//		{
//			txtDebug.text = "Visible Enter -->  visible == true";	
//			
//			Avatar2.Rotate(0, 0, 30);
//		}
	}
	
	
	
	public void CommandPose(string poseText)
	{
		txtDebug.text = "CommandPose is Called...";		
	}
		
	
	void UF_Test()
	{
		txtDebug.text = "LWY UF_Test is Called...";
	}	
	
	// Use this for initialization
	void Start () {
		
		fDisabledist = 100;
		
		bCurAvatar1 = true;
		bCurAvatar2 = false;

		if(Avatar2 != null) {
			Avatar2.Translate(fDisabledist,0,0);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
