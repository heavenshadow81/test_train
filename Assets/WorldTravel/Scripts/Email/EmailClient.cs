using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using System.IO;
using System.Text;

public class EmailClient : MonoBehaviour
{

    Thread ClientThread;
    Socket server;
    bool EmailThread;
    private List<ConnectionInfo> _connections = new List<ConnectionInfo>();
    Queue<InfoQueue> queueReceiveData = new Queue<InfoQueue>();
    private Queue<InfoImage> _imgList = new Queue<InfoImage>();

    string pc_address = "";

    private class InfoQueue
    {
        public InfoQueue(Socket _server, byte[] d)
        {
            server = _server;
            data = d;
        }
        public Socket server;
        public byte[] data;
    }
    void Awake()
    {
        GetLocalIpAddress();
        GetTextFile();
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ClientThread = new Thread(client_th);
        ClientThread.Start();
        GetPhotoNumber = 0;
        path = string.Format("{0}/{1}", Application.persistentDataPath, "ScreenShotEmail");
        SendingImagePath = new string[10];
        for (int i = 0; i < SendingImagePath.Length; i++)
            SendingImagePath[i] = "Empty";
        if (Directory.Exists(path))
            Directory.Delete(path, true);
    }
    void OnDestroy()
    {
        SocketClose();
    }
    public void SocketClose()
    {
        if (ClientThread != null)
            ClientThread.Abort();

        if (server != null)
        {
            //if (_clientSocket.Connected)
            server.Close();
        }
    }
    public string _log;
    public Text logtext;
    void client_th()
    {
        Debug.Log("클라 스레드 시작");
        EmailThread = true;
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(pc_address), 9050); // ("xxx = IP ")  // 네트워크의 끝점을 IP번호와 포트번호 객체화 시킵니다. 여행 프로그램이 실행되는 PC의 IP주소가 필요
                                                                             //"192.168.102.66"
        try
        {
            Debug.Log("서버에 연결 시도");
            server.Connect(ipep);//server socket에 연결합니다.
        }
        catch (SocketException e)
        {
            Debug.Log(e.ToString());
        }
        while (EmailThread)
        {
            // Debug.Log("서버에서 리시브 시도");
            byte[] buffer = new byte[256];
            try
            {
                int bytesRead = server.Receive(buffer, SocketFlags.None);
                if (bytesRead > 0)
                {
                    lock (queueReceiveData)
                    {
                        Debug.Log("서버에서 데이터가 왔다!");
                        ReceiveData(buffer, bytesRead);
                    }
                }

            }
            catch (SocketException e)
            {
                Debug.Log(e.Message + " / " + e.ErrorCode);
                _log = e.Message + " / " + e.ErrorCode;
            }
        }

        server.Shutdown(SocketShutdown.Both);
        Debug.Log("서버에 종료 시도");
        server.Close();
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
                int receiveSize = server.Receive(rByte);
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
        queueReceiveData.Enqueue(new InfoQueue(server, data));

        if (returnData != null)
            ReceiveData(returnData, returnSize);
    }
    float AutoRefreshTime;
    public Email_UIEvent uievent;
    public bool EmailSending;
    void Update()
    {
        if (GetPhotoOn)
        {
            AutoRefreshTime += Time.deltaTime;
        }
        // logtext.text = _log;
        lock (queueReceiveData)
        {
            if (GetPhotoOn && AutoRefreshTime > 160)
            {
                uievent.Refresh();
                return;
            }
            if (queueReceiveData.Count > 0)
            {
                //이미 넘겨받은 이미지가 있고, 시간이 40초가 지났고, 이메일을 보내고있는 중이 아닌 상태에서 새로운 이미지가 들어올떄(다음사용자 사진이 들어올때)
                if (SendingImagePath[0] != "Empty" && AutoRefreshTime > 40 && !EmailSending)
                {
                    uievent.Refresh();
                    return;
                }
                Debug.Log("큐 개수 : " + queueReceiveData.Count);
                Debug.Log("큐에 데이터가 있다면");
                InfoQueue info = queueReceiveData.Dequeue();
                byte[] fullData = info.data;
                int size = BitConverter.ToInt32(fullData, 1);

                byte[] data = new byte[size];
                Array.Copy(fullData, 5, data, 0, size); // start + size = 5

                switch (fullData[0])
                {
                    case TCP.DATA_IMAGE:
                        Debug.Log("ReceiveImage");
                        ReceiveImage(data);
                        break;

                    default:
                        Debug.LogError("Wrong Dequeue Data");
                        break;
                }
            }

        }
        if (_imgList.Count > 0)
        {

            SetPhoto();
        }
    }
    private void ReceiveImage(byte[] buf)
    {
        MemoryStream stream = new MemoryStream();

        int index = BitConverter.ToInt32(buf, 0);
        int imgWidth = BitConverter.ToInt32(buf, 4);
        int imgHeight = BitConverter.ToInt32(buf, 8);

        int tmpLength = 4 + 4 + 4; // index width height
        int dataLenght = buf.Length - tmpLength; //
        byte[] imgData = new byte[dataLenght];
        Array.Copy(buf, tmpLength, imgData, 0, imgData.Length);

        stream.Write(imgData, 0, imgData.Length);

        ImageInfo info = new ImageInfo();
        info.id = index;
        info.width = imgWidth;
        info.height = imgHeight;
        info.data = stream.ToArray();
        Debug.Log("이미지 큐에 저장한다.");
        _imgList.Enqueue(new InfoImage(server, info));
    }

    public RawImage[] userPhoto;
    public Texture DefaulteImage;
    public int GetPhotoNumber;
    public int PhotoNumber;
    public string path = "";
    public string[] SendingImagePath;
    public bool GetPhotoOn;
    void SetPhoto()
    {
        GetPhotoOn = true;
        Debug.Log(GetPhotoNumber + "번째 이미지 가져오는중");
        Texture2D rt = _imgList.Dequeue().image.texture;
        userPhoto[GetPhotoNumber].texture = rt;

        // System.DateTime currentTime = System.DateTime.Parse(System.DateTime.Now.ToString());
        // string _date = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        string Resize = string.Format("{0}/{1}.jpg", path, PhotoNumber);
        PhotoNumber++;
        SendingImagePath[GetPhotoNumber] = Resize;
        Debug.Log(GetPhotoNumber + "   /   " + Resize);
        GetPhotoNumber++;
        File.WriteAllBytes(Resize, rt.EncodeToJPG());

    }

    public void ResetPhoto()
    {
        EmailSending = false;
        PhotoNumber = 0;
        AutoRefreshTime = 0;
        GetPhotoOn = false;
        Debug.Log("ResetPhoto");
        if (_imgList.Count > 0)
            _imgList.Clear();
        for (int i = 0; i < userPhoto.Length; i++)
        {
            userPhoto[i].texture = DefaulteImage;
        }

        SendingImagePath = new string[10];
        for (int i = 0; i < SendingImagePath.Length; i++)
            SendingImagePath[i] = "Empty";
        GetPhotoNumber = 0;
    }

    #region PC address얻기
    //출처: https://www.codegrepper.com/code-examples/csharp/c%23+get+pc+ip+address
    //
    void GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                pc_address = ip.ToString();
                print($"IPv4주소{pc_address}");
            }
        }
    }
    #endregion

    #region LoadFile
    void GetTextFile()
    {
        if (!System.IO.File.Exists("ip.txt"))
        {
            System.IO.File.Create("ip.txt");
            System.IO.File.WriteAllText("ip.txt", pc_address);
        }
        string ip = File.ReadAllText("ip.txt");

    }
    #endregion
}
