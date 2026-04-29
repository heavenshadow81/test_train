using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ML.NFCPlugin;
using System.Net;
using System.IO;
//NFC 연계 !!!

class NFCPlayerInfo
{
    //이름
    public string Name { get; set; }
    //콘텐츠 이름과 콘텐츠 통계
    public IDictionary<System.DateTime, ContentsUseSet> ContentSet { get; set; } = new Dictionary<System.DateTime, ContentsUseSet>();
    
}
class ContentsUseSet
{
    //콘텐츠 이름
    public string ContentsName { get; set; }
    //콘텐츠 시작 시각
    public System.DateTime ContentsBeginTime { get; set; }
    //콘텐츠 끝낸 시각
    public System.DateTime ContentsEndTime { get; set; }
    //콘텐츠 점수
    public int ContentsPoints { get; set; } = 0;
}

public class NFCPlayerContentsCheck : MonoBehaviour
{
    #region 변수
    [SerializeField]
    UnityEngine.UI.Text nfcInformation;
    System.DateTime Time;
    List<NFCPlayerInfo> playerInfo = new List<NFCPlayerInfo>();
    #endregion
    #region 프로퍼티

    #endregion
    #region 함수
    void NFCChange(bool state)
    {
        if (state)
        {
            if (NFCPluginInterface.Open())
            {
                print("NFC 소켓 활성화");
                //nfcInformation.text = "NFC 소켓 상태: 열림";
            }
            else
            {
                print("NFC 소켓 비활성화");
            }
        }
        else
        {
            NFCPluginInterface.Close();
            print("NFC 소켓 닫기");
            //nfcInformation.text = "NFC 소켓 상태: 닫힘";
        }
    }
    //서버에서 가져오기
    string ResponseName(string cardNfc)
    {

        WebRequest request = WebRequest.Create($"http://grownshare.co.kr:19001/api/getKinderStudentInfoByRFNo?rfNo= {cardNfc}".Trim());
        request.Credentials = CredentialCache.DefaultCredentials;
        //웹 결과값 받아오기
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        print(response.StatusDescription);
        Stream dataStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(dataStream);
        string responseFromServer = reader.ReadToEnd();
        print(responseFromServer);
        reader.Close();
        dataStream.Close();
        response.Close();
        
        return responseFromServer;
    }
    #endregion

    #region 유니티 함수
    private void OnEnable()
    {
        NFCChange(true);
        Time = System.DateTime.Now;
    }

    private void FixedUpdate()
    {
        Packet packet = NFCPluginInterface.Receive();
        if (packet != null)
        {
            if(packet.Type == Packet.PacketType.CardAdded)
            {
                Packet.CardAddedPacketData CardAdded = packet.Data as Packet.CardAddedPacketData;
                print($"카드 UID : {CardAdded.UID}");
                ResponseName(CardAdded.UID);
                print(ResponseName(CardAdded.UID));
                Time = System.DateTime.Now;
                //카드 UID로 서버에 접속 > 해당 유저 정보 읽어오기!!!> key를 넘겨 그 값을 받아오는 방법 찾기
                //자료구조를 구성하여 해당 값을 바탕으로 유저 이름과 날짜, 콘텐츠 이름, 점수 등을 넣기!!!

            }
        }
    }
    private void OnDisable()
    {
        NFCChange(false);
    }
    #endregion
}
