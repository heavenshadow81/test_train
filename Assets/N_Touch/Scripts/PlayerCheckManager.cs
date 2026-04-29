using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.NFCPlugin;

/// <summary>
/// Socket통신 Open Close
/// </summary>
public class PlayerCheckManager : MonoBehaviour
{
	[SerializeField]
	NFCJsonManager nfcJsonManaager;
	[SerializeField]
	JsonUpdate jsonUpdate;

	static PlayerCheckManager _instance;
	public static PlayerCheckManager instance
	{
		get { return _instance; }
	}
	void Awake()
	{
		if (!_instance)
		{
			_instance = this;
		}
	}

	void Start()
	{
		//OpenSocket();
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
			// 초기화
			// 1. NFCJsonManager 의 playerNum
			nfcJsonManaager.playerNum = 0;
            
			// 2. JsonUpdate 의 uidArray,uidArrayNames,temp
            for (int i = 0; i < jsonUpdate.uidArray.Count; i++)
            {
				jsonUpdate.uidArray[i] = "";
				jsonUpdate.uidArrayNames[i] = "";
			}
			jsonUpdate.temp = 0;
			jsonUpdate.playerNum = 0;
			nfcJsonManaager.nfcUIDTxt.text = "Start 눌러주세요.";
			//OpenSocket();
		}
	}

	//Open
	public void OpenSocket()
	{
		if (NFCPluginInterface.Open())
		{
			Debug.Log("Socket is open");
		}
		else
		{
			Debug.Log("Socket open 실패");
		}
	}

	//Close
	//
	public void OnDestroy()
	{
		Debug.Log("Socket is closed");
		NFCPluginInterface.Close();
	}

	//임의 추가
	void OnDisable()
    {
		Debug.Log("Socket is closed");
		NFCPluginInterface.Close();
	}
	//초기화 함수
	public void Init()
    {
		// 초기화
		// 1. NFCJsonManager 의 playerNum
		nfcJsonManaager.playerNum = 0;

		// 2. JsonUpdate 의 uidArray,uidArrayNames,temp
		for (int i = 0; i < jsonUpdate.uidArray.Count; i++)
		{
			jsonUpdate.uidArray[i] = "";
			jsonUpdate.uidArrayNames[i] = "";
		}
		jsonUpdate.temp = 0;
		jsonUpdate.playerNum = 0;
		nfcJsonManaager.nfcUIDTxt.text = "Start 눌러주세요.";
		//OpenSocket();
	}
}