using UnityEngine;
using System.Net.Sockets;
using System;
using UnityEngine.UI;

namespace ML.PlaywallKids.Aquarium
{
    public class EventData
    {
        public int user = 0;
        public int cmd = 0;
        public float val = 0;

        public EventData(int _user, int _cmd, float _val)
        {
            user = _user;
            cmd = _cmd;
            val = _val;
        }
    }

    public class ClientSocket : MonoBehaviour
    {
        //public UserFish user;
        public UserFish user = null;
        public int CMD_SIZE = 12;

        // 포지션 사이즈가 추가되었습니다. - 모젼스랩
        public int POS_SIZE = 83;

        string serverIP = "127.0.0.1";
        string port = "5000";
        private Socket sock = null;
        //private float timer = 0.0f;
        private int counter = 0;
        private byte[] sendBuffer = new byte[1];

        // 버퍼 사이즈를 늘렸습니다. - 모젼스랩
        private byte[] recvBuffer = new byte[2048];

        private Vector3 Postion;
        public UserInfo userInfo;
        private string message = "Ready to receiving";

        public CustomInput extInput;

        /*
        bool tmCheck = false;
        float curTime = 0;
        float oldTime = 0;
        bool firstTime = false;
        bool firstTime2 = false;
        */

        // Use this for initialization
        void Start()
        {
            Invoke("Init", 2);
            //Init ();
        }

        void Init()
        {

            sock = new Socket(
                    AddressFamily.InterNetwork,
                     SocketType.Stream,
                     ProtocolType.Tcp);
            sock.Connect(serverIP, Convert.ToInt32(port));
            if (sock.Connected)
            {
                ConsoleMessage("Connected");

                sock.BeginReceive(
                    recvBuffer,
                    0,
                    recvBuffer.Length,
                    SocketFlags.None,
                    new AsyncCallback(ReceiveComplete),
                    null);
            }
            else
            {
                ConsoleMessage("Fail to connect");
            }
        }

        void OnGUI()
        {
            if (GetComponent<Text>().enabled)
            {
                GetComponent<Text>().text = message;
                //guiText.text = "curTime" + curTime.ToString();
            }
        }


        void SendMessage(byte type)
        {

            String tmp = String.Format("{0} times try: ", counter);
            if (sock != null)
            {
                try
                {
                    sendBuffer[0] = type;
                    sock.BeginSend(
                           sendBuffer,
                           0,
                           1,
                           SocketFlags.None,
                           new AsyncCallback(SendComplete),
                           null);

                    tmp += "Success";
                    ConsoleMessage(tmp);
                }
                catch (Exception e)
                {
                    tmp += "Exception: " + e.Message;
                    ConsoleMessage(tmp);

                    Shutdown();
                }
            }
            counter++;
        }

        void OnApplicationQuit()
        {
            Shutdown();
        }

        void ReceiveComplete(IAsyncResult ar)
        {
            try
            {
                if (null == sock)
                    return;

                int receive_size = 0;
                receive_size = sock.EndReceive(ar);

                if (receive_size == 0)
                {
                    Shutdown();
                }
                else
                {
                    int total_read_size = 0;
                    sock.BeginReceive(
                              recvBuffer,
                              0,
                              recvBuffer.Length,
                              SocketFlags.None,
                              new AsyncCallback(ReceiveComplete),
                              null);
                    total_read_size = 0;
                    while (receive_size > 0)
                    {
                        // 수정 - 모젼스랩
                        int read_size = Receive(recvBuffer, total_read_size, receive_size);
                        receive_size -= read_size;
                        total_read_size += read_size;
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleMessage("Exception: " + e.Message);
                Shutdown();
            }
        }

        void Update()
        {
            /*
            if (firstTime)
            {
                firstTime = false;
                firstTime2 = true;
                oldTime = Time.time;
            }

            if (tmCheck)
            {
                tmCheck = false;
                float curT = Time.time;
                //if (curTime < curT - oldTime)
                {
                    curTime = curT - oldTime;
                }
                oldTime = Time.time;

                Debug.Log ("time=" + curTime);
            }
            //*/
        }

        public int Receive(byte[] buffer, int total_read_size, int receive_size)
        {
            if (buffer[0] == 0x02 && buffer[1] == 0x20)
            {
                if (receive_size < CMD_SIZE) return receive_size;

                // 사용자 ID 1~4
                int UserID = buffer[total_read_size + 2];

                // 제스츄어 1~4
                int Cmd = buffer[total_read_size + 3];

                // x 좌표값 입니다.
                int x = BitConverter.ToInt32(buffer, total_read_size + 4);
                // z 좌표값 입니다.
                int y = BitConverter.ToInt32(buffer, total_read_size + 8);

                // print
                string msg = string.Format("User:{0:x2}, Cmd:{1:x2}, X:{2}, Y:{3}",
                    buffer[total_read_size + 2], buffer[total_read_size + 3], x, y);

                Vector2 Position;
                Position.x = x;
                Position.y = y;

                //ksy debug
                //extInput.SetMotionInfo(UserID, x, y, Cmd);

                //EventData evt = new EventData(UserID, Cmd, value1);
                //user.SendMessage("Interation", evt);
                //user.Interation(evt);

                //userInfo.SetUserID(UserID);
                //userInfo.SetToolPosition(0,Position);

                //ConsoleMessage("action " + msg);

                return CMD_SIZE;
            }
            // 위치 정보
            // 1개씩 이벤트 발생마다 보내던걸 8개 모두 묶어서 보냅니다.
            // 전송 텀은 기본적으로 초당 4회로 잡혀있고 테스트 후 수정하겠습니다.
            // TYPE 가 2인 것은 터치가 띄어졌다는 것이므로 이후 새 데이터(입력)가 들어오기 전까진 초기화 하지 않습니다.
            else if (buffer[0] == 0x02 && buffer[1] == 0x30)
            {
                /*
                tmCheck = true;
                if (firstTime == false && firstTime2 == false)
                    firstTime = true;
                //*/

                // 기존 소스 시작 - 모젼스랩
                /*
                // click 정보
                int clickVal = buffer[total_read_size + 3];

                // x 좌표값 입니다.
                int value1 = BitConverter.ToInt32(buffer, total_read_size + 4);
                // z 좌표값 입니다.
                int value2 = BitConverter.ToInt32(buffer, total_read_size + 8);
                // r 입니다.
                int r = buffer[total_read_size + 12];

                if ((value1 >= 0 && value1 <= 4320) && (value2 >= 0 && value2 <= 1920))
                {
                    // print
                    string msg = string.Format("User:{0:x2}, Cmd:{1:x2}, X:{2}, Y:{3}, R:{4}",
                        buffer[total_read_size + 2], clickVal, value1, value2, r);

                    int Ch = buffer[total_read_size + 2];
                    int Cmd = buffer[total_read_size + 3];
                    Vector2 Position;
                    Position.x = value1;
                    Position.y = value2;

                    //ConsoleMessage("touch " + msg);         

                    //ksy debug
                    int userId = extInput.FindUserId(value1);

                    if (extInput.IsDuplicateInput(userId, Ch, value1, value2) == false)
                    {
                        int inputCh = extInput.GetChannel(Ch);
                        extInput.SetTouchInfo(inputCh, userId, value1, value2, r, Cmd);
                    }

                    //Debug.Log("what2 " + userId);
                    ////////////////////////

                    //EventData evt = new EventData(UserID, Cmd, value1);
                    //ConsoleMessage(UserID + " " + Cmd + " " + value1);
                    //user.SendMessage("Interation", evt);
                    //user.Interation(evt);

                    userInfo.SetUserID(userId);
                    userInfo.SetToolPosition(0,Position);
                }

                return CMD_SIZE + 1;
                //*/
                // 기존 소스 종료 - 모젼스랩

                ///////////////////////////////////////////////////////////////

                // 신규소스 - 모젼스랩
                // for문을 돌면서 데이터를 읽습니다.
                // i값이 유저 아이디로 1인당 두포인트를 갖고 있습니다.
                // 기존의 채널아이디로 사용하셔도 될 것 같습니다.
                // 1p 0~1, 3p 2~3, 3p 4~5, 4p 6~7
                // 데이터 처리 부분을 Array에 맞게 수정이 필요 할 것 같습니다.

                //if (receive_size < POS_SIZE) return receive_size;

                for (int i = 0; i < 8; i++)
                {
                    int type = (int)(buffer[total_read_size + 2 + i * 10]);
                    // x 좌표값 입니다.
                    int x = BitConverter.ToInt32(buffer, total_read_size + 3 + i * 10);
                    // y 좌표값 입니다.
                    int y = BitConverter.ToInt32(buffer, total_read_size + 7 + i * 10);
                    // depth 입니다.
                    int r = (int)buffer[total_read_size + 11 + i * 10];

                    if ((x >= 0 && x <= 4320 + 80) && (y >= 0 && y <= 1920))
                    {
                        Vector2 Position;
                        Position.x = x;
                        Position.y = y;

                        //extInput.SetTouchInfo(i, x, y, r, type);

                        //userInfo.SetUserID(userId);
                        //userInfo.SetToolPosition(0, Position);

                        /*
                        // print
                        if (i == 1)
                        {
                            string msg = string.Format("User:{0:x2}, Type:{1:x2}, X:{2}, Y:{3}, R:{4}",
                                        i, type, x, y, r);
                            ConsoleMessage("touch " + msg);
                        }
                        //*/
                    }
                }

                return POS_SIZE;
                // 신규소스 종료 - 모젼스랩
            }

            return CMD_SIZE;
        }

        private void SendComplete(IAsyncResult ar)
        {
            try
            {
                if (null == sock)
                    return;

                int len = sock.EndSend(ar);
                if (len == 1)
                {
                    ConsoleMessage("Send success");
                }
            }
            catch (Exception e)
            {
                ConsoleMessage("Exception: " + e.Message);
                Shutdown();
            }
        }

        private void ConsoleMessage(string msg)
        {
            message = msg;
            Debug.Log(message);
        }

        private void Shutdown()
        {
            ConsoleMessage("Disconnect");
            if (sock != null)
            {
                sock.Shutdown(SocketShutdown.Both);
                sock = null;
            }
        }
    }
}