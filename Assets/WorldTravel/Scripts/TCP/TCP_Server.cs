using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.IO;
using System.Text;
using ML.MapoContents.WorldTravel;
public class TCP_Receiver
{
    TCP_Server sever;
    Thread thread_cl;
    Socket client;
    public Queue<byte[]> queueReceiveData = new Queue<byte[]>();

    int CloseCheck = 0;
    bool _isThreading;


    public void SetClient(Socket cl, TCP_Server sv)
    {
        client = cl;
        sever = sv;
    }
    public void StartRecive()
    {
        _isThreading = true;
        thread_cl = new Thread(Receivedata);
        thread_cl.Start();
    }
    public void Close_Thread()
    {
        _isThreading = false;
        // thread_cl.Join();
        //  ImageSendThread.Join();
    }
    public static int SendReceiveTest2(Socket server)
    {
        byte[] msg = Encoding.UTF8.GetBytes("This is a test");
        byte[] bytes = new byte[256];
        try
        {
            int byteCount = server.Receive(bytes, SocketFlags.None);
            if (byteCount > 0)
                Debug.Log(Encoding.UTF8.GetString(bytes));
            //Console.WriteLine(Encoding.UTF8.GetString(bytes));
        }
        catch (SocketException e)
        {
            Debug.Log(e.Message + " / " + e.ErrorCode);
            return (e.ErrorCode);
        }
        return 0;
    }
    public int ReceiveCheck()
    {
        return CloseCheck;
    }

    void ReceiveData(byte[] receiveData, int size)
    {
        if (size == 0)
            return;

        Debug.Log("size : " + size);
        if (TCP.IsStart(receiveData[0]) == false)
            return;

        int length = BitConverter.ToInt32(receiveData, 1);
        int byteLength = 1 + 4 + length + 1; // start + length + data + end

        byte[] returnData = null;
        int returnSize = 0;

        byte[] data = new byte[byteLength];
        if (size == byteLength)
        {
            Array.Copy(receiveData, data, byteLength);
        }
        else if (size < byteLength)
        {
            Array.Copy(receiveData, data, size);

            byte[] rByte = new byte[TCP.BUFFER_SIZE];
            int scarceSize = byteLength - size;
            int nextIndex = size;
            while (scarceSize > 0)
            {
                //Debug.Log("리시브중");
                int receiveSize = client.Receive(rByte);
                if (receiveSize == 0)
                    break;

                int readScarceSize = receiveSize;
                if (scarceSize < receiveSize)
                    readScarceSize = scarceSize;

                Array.Copy(rByte, 0, data, nextIndex, readScarceSize);
                scarceSize -= readScarceSize;
                nextIndex += readScarceSize;

                if (readScarceSize != TCP.BUFFER_SIZE)
                {
                    returnSize = receiveSize - readScarceSize;
                    returnData = new byte[returnSize];
                    Array.Copy(rByte, readScarceSize, returnData, 0, returnSize);
                }

                Thread.Sleep(1);
            }
        }
        else
        {
            returnSize = size - byteLength;
            Array.Copy(receiveData, 0, data, 0, byteLength);

            returnData = new byte[returnSize];
            Array.Copy(receiveData, byteLength, returnData, 0, returnSize);
        }

        Debug.Log("큐에 담는다!");
        queueReceiveData.Enqueue(data);

        if (returnData != null)
            ReceiveData(returnData, returnSize);
    }
    void Receivedata()
    {
        CloseCheck = 0;
        string data = "";
        string input;

        //client.Blocking = false;
        IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
        Debug.Log("Connected with " + clientep.Address + " at port " + clientep.Port);
        while (_isThreading)
        {
            string receiveData = "";
            byte[] bytes = new byte[256];
            string[] splitdata;
            string StateMessage = "";
            try
            {
                int byteCount = client.Receive(bytes, SocketFlags.None);

                if (byteCount > 0)
                {
                   /* lock (queueReceiveData)
                    {
                        Debug.Log("서버에서 데이터가 왔다!");
                        ReceiveData(bytes, byteCount);
                    }*/

                     receiveData = Encoding.UTF8.GetString(bytes);
                     //Debug.Log(receiveData);
                     CloseCheck = 0;
                     splitdata = receiveData.Split('/');
                     StateMessage = splitdata[0];
                     Debug.Log("TabletControl is "+TravelManager.Instance.TabletControll);
                     if (TravelManager.Instance.TabletControll)
                     {
                         TCP_Command.Instance.SetCommand(receiveData);
                     }
                }
                if (StateMessage == "OutClient")
                 {
                     _isThreading = false;
                     Debug.Log("DisConnected Client And Recever Close");
                     Debug.Log("Controll Have [Kincet]");
                     TravelManager.Instance.TabletControll = false;
                     break;
                 }
                 if (StateMessage == "ImLive")
                 {
                     Debug.Log("Live Client" + clientep.Address + " at port " + clientep.Port);
                     CloseCheck = 0;
                 }
                 if (StateMessage == "TabletControll")
                 {
                     Debug.Log("Controll Have [Tablet]");
                     TravelManager.Instance.TabletControll = true;
                     CloseCheck = 0;
                 }
                 if (StateMessage == "KinectControl")
                 {
                     Debug.Log("Controll Have [Kincet]");
                     TravelManager.Instance.TabletControll = false;
                     CloseCheck = 0;
                 }
            }
            catch (SocketException e)
            {
                Debug.Log(e.Message + " / " + e.ErrorCode);
            }
            Thread.Sleep(1);
        }

        client.Shutdown(SocketShutdown.Both);//소켓을 비활성화 시킵니다.
        client.Close();//소켓을 닫습니다.
                       // Close_Thread();

        Thread.Sleep(1);
    }

    public void Send(Texture2D[] SendImages)
    {
        _isThreading = true;
      //  Debug.Log("getdata : " + SendImages[0].width);
        List<ImageInfo> _list = new List<ImageInfo>();
        //   currentImage = textures[_index];
        for (int _index = 0; _index < SendImages.Length; _index++)
        {
            if (SendImages[_index] == null)
            {
                break;
            }

            ImageInfo info = new ImageInfo()
            {
                id = _index,
                width = SendImages[_index].width,
                height = SendImages[_index].height,
                data = SendImages[_index].EncodeToPNG()
            };
            _list.Add(info);
        }
        SendImage(_list);
    }
    public void SendImage(List<ImageInfo> infos)
    {

        Thread ImageSendThread = new Thread(() => SendImageThread(infos));
        ImageSendThread.Start();
    }

    void SendImageThread(List<ImageInfo> infos) //byte[] imageBytes, int index, int width, int height)
    {
        IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
        Debug.Log("Connected with " + clientep.Address + " at port " + clientep.Port);

        string ftpPath = $"ftp://{sever.pc_address}/WorldTravel/";//실행 컴퓨터의 ftp(이메일 아닌 키넥트와 연결된 컴퓨터의 LAN IP가 필요): 윈도우 10에서 FTP서비스 열어주어야 함//192.168.0.7
        string user = "TravelLibraryFTP";
        string pwd = "Travel0%FTP";
        /*
        string user = "root";
        string pwd = "smartmice12#";*/
        //string inputFIle = "C:/Users/mogencelab/AppData/LocalLow/DefaultCompany/AugementedReality/ScreenShot/2017_8_30_9_52_2.png";

        int ftp_idx = 0; ;

        try
        {
            foreach (ImageInfo info in infos)
            {
                //Debug.Log(ftp_idx +" 번째 사진 ");
                if (!_isThreading)
                    break;
                // index width height data 
                int length = 4 + 4 + 4 + 4 + info.data.Length;
                int fullLength = 1 + 4 + length + 1; // start + size + data + end
                byte[] sendData = new byte[fullLength];
                //
                byte[] lengthData = BitConverter.GetBytes(length);

                byte[] indexData = BitConverter.GetBytes(info.id);
                byte[] indexWidth = BitConverter.GetBytes(info.width);
                byte[] indexHeight = BitConverter.GetBytes(info.height);

                sendData[0] = TCP.DATA_IMAGE;

                System.Array.Copy(lengthData, 0, sendData, 1, 4); // 사이즈 길이

                System.Array.Copy(indexData, 0, sendData, 5, 4); //이미지 Index
                System.Array.Copy(indexWidth, 0, sendData, 9, 4); //이미지 Width
                System.Array.Copy(indexHeight, 0, sendData, 13, 4); //이미지 Height

                System.Array.Copy(info.data, 0, sendData, 17, info.data.Length);

                sendData[sendData.Length - 1] = TCP.DATA_END;
                Send(sendData);
                //Debug.Log(ftp_idx + " 번째 사진 전송");
                WebClient web = new WebClient();
                
               
                System.DateTime currentTime = System.DateTime.Parse(System.DateTime.Now.ToString());        
                string _date = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second, ftp_idx);
                string Resize = string.Format("{0}.png", _date);
                
                Uri Check = new Uri(ftpPath + Resize);
                web.Credentials = new System.Net.NetworkCredential(user, pwd);
                //Debug.Log("ftp 업데이트");
                web.UploadData(Check, info.data);
                //Debug.Log("ftp 업데이트완료");
                ftp_idx++;
                Thread.Sleep(1);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("netClient:SendImageThread() - " + ex.Message);
        }

        Debug.Log("이미지 전송 스레드 종료");
    }

    private bool Send(byte[] data)
    {
        if (client != null)
        {
            if (client.Connected == true)
            {
                if (data != null)
                {
                    client.Send(data);
                    return true;
                }
            }
        }
        return false;
    }
}

public class TCP_Server : MonoBehaviour
{
    Thread AceptThread;
    bool Acepting;
    bool imageAcepting;

    TCP_Receiver Email_cl;
    public bool Email;

    TCP_Receiver tablet_cl;
    public bool Tablet;
    public string pc_address = "";
    IEnumerator LiveCheck(TCP_Receiver receiver)
    {
        int CloseCheck = 0;
        while (true)
        {
            if (receiver.ReceiveCheck() == 0)
                CloseCheck = 0;
            CloseCheck += 1;
            if (CloseCheck >= 10)
                break;
            yield return new WaitForSeconds(1f);
            Debug.Log("CloseCheck Client : " + CloseCheck + " sec");
        }

        receiver.Close_Thread();
        Debug.Log("Time Out!! DisConnect Client And Recever Close");
    }
    //  Socket newsock;
    IPEndPoint ipep;
    // 네트워크의 끝점을 IP번호와 포트번호 객체화 시킵니다.
    Socket newsock;  // 이제 소켓을 생성 시키는데 생성시킬때의 파라미터가 AddressFamily가 뜻하는 것은 Socket의 주소지정 스키마를 정의 하는 것입니다. interNetwork 는 기존 IP주소체계인 IP 버전 4.에 대한 주소입니다.  SocketType은 말그대로 소켓의 종류를 가져 오는 것인데 Stream은 데이터 중복이나 경계 유지 없이 신뢰성 있는 양방향 연결 기반의 바이트 스트림을 지원합니다 ProtocolType은 정해진 통신 규약을 정의 하는 것인데 이경우 신뢰성이 있는 TCP로 통신함을 의미합니다.
    /*
    void Update()
    {
        if (tablet_cl != null)
        {
            lock (tablet_cl.queueReceiveData)
            {

                if (tablet_cl.queueReceiveData.Count > 0)
                {
                    Debug.Log("큐 개수 : " + tablet_cl.queueReceiveData.Count);
                    Debug.Log("큐에 데이터가 있다면");
                    byte[] StringData = tablet_cl.queueReceiveData.Dequeue();

                    string receiveData = Encoding.UTF8.GetString(StringData);
                    //Debug.Log(receiveData);
                    string[] splitdata = receiveData.Split('/');
                    string StateMessage = splitdata[0];
                    Debug.Log("TabletControl is " + TravelManager.Instance.TabletControll);
                    if (TravelManager.Instance.TabletControll)
                    {
                        TCP_Command.Instance.SetCommand(receiveData);
                    }
                    if (StateMessage == "OutClient")
                    {
                        Debug.Log("DisConnected Client And Recever Close");
                        Debug.Log("Controll Have [Kincet]");
                        TravelManager.Instance.TabletControll = false;
                    }
                    if (StateMessage == "ImLive")
                    {
                        Debug.Log("Live Tablet Client");
                        //     CloseCheck = 0;
                    }
                    if (StateMessage == "TabletControll")
                    {
                        Debug.Log("Controll Have [Tablet]");
                        TravelManager.Instance.TabletControll = true;
                        //   CloseCheck = 0;
                    }
                    if (StateMessage == "KinectControl")
                    {
                        Debug.Log("Controll Have [Kincet]");
                        TravelManager.Instance.TabletControll = false;
                        //  CloseCheck = 0;
                    }
                }
            }
        }

        

    }*/
    void Awake()
    {
        GetLocalIpAddress();
        print(pc_address);
        ipep = new IPEndPoint(IPAddress.Parse(pc_address), 9050);
        
        newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        OnEnable();
        
    }
    private void OnEnable()
    {
        print($"접속하려는 주소 :{ipep}");
        newsock.Bind(ipep);//정의된 네트워크의 끝점으로 newsock를 bind시킵니다.
        newsock.Listen(10);//newsock을 수신대기 상태로 놓습니다.

        /*AceptThread = new Thread(AceptImageClinet);
        imageAcepting = true;
        AceptThread.Start();*/
        _ImageLength = 0;

        AceptThread = new Thread(AceptClinet);
        Acepting = true;
        AceptThread.Start();
        _ImageLength = 0;
    }

    public void ResetTCPValue()
    {
        SendImages = null;
        _ImageLength = 0;
    }
    public Texture2D[] SendImages;
    int _ImageLength;
    public void SetImageArray(int _length)
    {
        Debug.Log("Set Image Count : " + _length);
        SendImages = null;
        SendImages = new Texture2D[_length];
        _ImageLength = 0;
    }
    public void SaveTexture(Texture2D _capture)
    {
        SendImages[_ImageLength] = _capture;
        Debug.Log("------------------Save Image Count : " + _ImageLength);
        _ImageLength++;
    }

    public void Send()
    {
        bool ch_image = false;
        if (Email_cl != null)
        {
            for (int i = 0; i < SendImages.Length; i++)
            {
                if (SendImages[i] != null)
                {
                    ch_image = true;
                }
            }
        }
        if(ch_image)
            Email_cl.Send(SendImages);
        /*
        List<ImageInfo> _list = new List<ImageInfo>();
        //   currentImage = textures[_index];
        for (int _index = 0; _index < SendImages.Length; _index++)
        {
            ImageInfo info = new ImageInfo()
            {
                id = _index,
                width = SendImages[_index].width,
                height = SendImages[_index].height,
                data = SendImages[_index].EncodeToPNG()
            };
            _list.Add(info);
        }       
        SendImage(_list);    */
    }
    Socket _ImageclientSocket;

    //newsock.Blocking = false;
    void AceptClinet()
    {
        while (Acepting)
        {
            //커넥팅 대기
            Debug.Log("------Waiting for client...  --------");

            Socket client = newsock.Accept();//client에서 수신을 요청하면 접속합니다.
            IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
            // client의 네트워크 끝점을 가져옵니다.
            //Console.WriteLine("Connected with {0} at port {1}", clientep.Address, clientep.Port);
            Debug.Log("Connected with " + clientep.Address + " at port " + clientep.Port);
            /*if (clientep.Address.ToString().Contains("192.168.3."))//3
            {
                Debug.Log("------This Tablet Connecting!! -------");
                tablet_cl = new TCP_Receiver();
                tablet_cl.SetClient(client, this);
                tablet_cl.StartRecive();
                //  tablet.StartRecive();

            }
            else*/ if (clientep.Address.ToString().Contains("192.168."))//5
            {
                Debug.Log("------This Email Connecting!! -------");
                Email_cl = new TCP_Receiver();
                Email_cl.SetClient(client, this);

            }
            /*
            if (!Recive)
            {
                Recive = true;
                receive = new TCP_Receiver();
                receive.SetClient(client, this);
                receive.StartRecive();
               // Acepting = false;
            }*/

            Thread.Sleep(1);
        }
    }
    void AceptImageClinet()
    {
        bool ImageClientConnect = false; ;
        Socket client;
        while (imageAcepting)
        {
            //리시버가 돌고있지 않을때 대기.

            //이미지 클라이언트가 접속되지 않은 상태라면

            Debug.Log("------Waiting for a Email client...");
            client = newsock.Accept();
            IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
            Debug.Log("Connected with " + clientep.Address + " at port " + clientep.Port);
            ImageClientConnect = true;
            _ImageclientSocket = client;
            Thread.Sleep(1);
        }
    }
    void OnApplicationQuit()
    {
        if (Email_cl != null)
            Email_cl.Close_Thread();

        if (tablet_cl != null)
            tablet_cl.Close_Thread();
        
        newsock.Close();
    }

    #region PC address얻기
    //출처: https://www.codegrepper.com/code-examples/csharp/c%23+get+pc+ip+address
    //
    void GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if(ip.AddressFamily == AddressFamily.InterNetwork)
            {
                pc_address = ip.ToString();
                print(pc_address);
            }
        }
    }
    #endregion
}

