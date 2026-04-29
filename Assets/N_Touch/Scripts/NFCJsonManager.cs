using System.Collections;
using System.IO;
using System.Net;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  NFC카드 판정결과 표시
/// </summary>
namespace ML.NFCPlugin
{
    class NFCStudentInfo
    {
        public string result;
        public string msg;
        public string kinderNm;
        public string kinderClassNm;
        public string studentNm;
        //생성자
        public NFCStudentInfo(string result, string msg, string kinderNm, string kinderClassNm, string studentNm)
        {
            this.result = result;
            this.msg = msg;
            this.kinderNm = kinderNm;
            this.kinderClassNm = kinderClassNm;
            this.studentNm = studentNm;
        }

    }

    [RequireComponent(typeof(JsonUpdate))]
    public class NFCJsonManager : MonoBehaviour
    {
        public Text nfcUIDTxt;
        public int maxPlayerNum = 6;
        public int playerNum;

        // Static 클래스로 변경필요
        string[] conditionMsg = {
            "카드 터치해주세요.",
            "등록 되었습니다.", 
            "터치한 사용자입니다.", 
            "사람이 꽉찼어요!", 
            "등록되지않은 유저입니다." 
        };

        public void Start() { nfcUIDTxt.text = conditionMsg[0]; }

        public void Update()
        {
            Packet packet = NFCPluginInterface.Receive();
            if (packet != null)
            {
                if (packet.Type == Packet.PacketType.CardAdded)
                {
                    Packet.CardAddedPacketData CardAdded = packet.Data as Packet.CardAddedPacketData;
                    Debug.Log("UID : " + CardAdded.UID);
                    //여기에 웹 리퀘스트 추가!!!
                    var info = ResponseName(CardAdded.UID);
                    print(info);
                    var infoclass = JsonUtility.FromJson<NFCStudentInfo>(info);

                    if (infoclass.msg.Contains("성공"))
                    {
                        print(infoclass.studentNm);
                        // 플레이어 추가
                        playerNum += 1;
                        nfcUIDTxt.text = conditionMsg[1];
                        if (playerNum > maxPlayerNum)
                        {
                            nfcUIDTxt.text = conditionMsg[3];
                            PlayerCheckManager.instance.OnDestroy();
                        }
                        else
                        {
                            string condition = GetComponent<JsonUpdate>().CheckDuplicate(CardAdded.UID);

                            switch (condition)
                            {
                                case "add":
                                    StartCoroutine(ShowConditionTxt(conditionMsg[1]));
                                    break;
                                case "exist":
                                    playerNum -= 1;
                                    StartCoroutine(ShowConditionTxt(conditionMsg[2]));
                                    break;
                            }
                        }
                    }
                    else
                    {

                    }
                    
                }
            }
        }

        // 상태표시 Text
        IEnumerator ShowConditionTxt(string txt)
        {
            nfcUIDTxt.text = txt;
            yield return new WaitForSeconds(1f);
            nfcUIDTxt.text = conditionMsg[0];
            StopCoroutine(ShowConditionTxt(""));
        }
        //카드 값을 통해 이름 서버에서 불러오기
        string ResponseName(string cardUID)
        {
            string message = string.Format("http://grownshare.co.kr:19001/api/getKinderStudentInfoByRFNo?rfNo={0}", cardUID);
            WebRequest request = WebRequest.Create(message);
            request.Method = "GET";

            request.Credentials = CredentialCache.DefaultCredentials;
            //웹 결과값 받아오기
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            print(response.StatusDescription);
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }
    }
}