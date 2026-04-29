using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
//using Newtonsoft.Json;
using System.Net;


/// <summary>
/// json Data 처리
/// 자료구조 확인
/// </summary>
class PlayeInfo
{
    public string uid;
    public string name;
    public int animalPuzzle;
    public int shadowPuzzle;

    public PlayeInfo(string uid, string name, int animalPuzzle, int shadowPuzzle)
    {
        this.uid = uid;
        this.name = name;
        this.animalPuzzle = animalPuzzle;
        this.shadowPuzzle = shadowPuzzle;
    }
}

public class JsonUpdate : MonoBehaviour
{

    //Dictionary,List 추후에 기획에따라 변경
    // UID 배열
    public List<string> uidArray = new List<string>();
    // 참가자 이름
    public List<string> uidArrayNames = new List<string>();

    public int temp = 0;

    //json list
    List<PlayeInfo> pInfo = new List<PlayeInfo>();

    public int playerNum;

    //터치확인에따른 UID 일치 확인
    public string CheckDuplicate(string uid)
    {

        // 현재 배열에 중복 uid 확인 - 2번 터치한경우
        if (uidArray.Contains(uid))
        {
            return "exist";
        }
        else
        {
            uidArray.Add(uid);
            playerNum++;
            return "added";
        }

    }

    //Json 파일 갱신
    bool RecordGamePlay(string uid)
    {

        string jdata = File.ReadAllText(Application.dataPath + "/playerData.json");
        pInfo = JsonUtility.FromJson<List<PlayeInfo>>(jdata);
        //pInfo = JsonConvert.DeserializeObject<List<PlayeInfo>>(jdata);

        // 추후 검색량이 많아진다면 정렬로직 변경 필요
        for (int i = 0; i < pInfo.Count; i++)
        {
            if (pInfo[i].uid == uid)
            {
                uidArrayNames[temp] = pInfo[i].name;
                temp++;

                switch (SceneManager.GetActiveScene().name)
                {
                    case "AnimalPuzzle":
                        pInfo[i].animalPuzzle += 1;
                        break;

                    case "ShadowPuzzle":
                        pInfo[i].shadowPuzzle += 1;
                        break;

                    default:
                        break;
                }

                //File.WriteAllText(Application.dataPath + "/playerData.json", JsonConvert.SerializeObject(pInfo));
                File.WriteAllText(Application.dataPath + "/playerData.json", JsonUtility.ToJson(pInfo));
                return true;
            }
        }
        return false;
    }

    private void OnEnable()
    {
        var jdata = File.ReadAllText(Application.dataPath + "/playerData.json");
        pInfo = JsonUtility.FromJson<List<PlayeInfo>>(jdata);
    }
}