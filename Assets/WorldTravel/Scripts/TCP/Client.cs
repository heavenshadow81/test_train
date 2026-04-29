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
public class Client : MonoBehaviour
{
    static public Client client;
    bool _isThreading;
    Thread th;

    string SendMessage;
    bool _sending;
    string data;
    string input;
    string ipdata;
    string portdata;
    Socket server;
    public bool _isPlayerIn;
    bool _ServerOn;
    void Awake()
    {
        client = this;
        _sending = false;
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        th = new Thread(client_th);
        th.Start();
        _isPlayerIn = true;
    }
    void Update()
    {
        if (!_ServerOn && _isThreading)
        {
            _ServerOn = true;
            StartCoroutine(LiveCallBack());
        }
    }
    void OnApplicationQuit()
    {
        SendMessage_Sever("OutClient/");
        th.Join();
    }
    public void SendMessage_Sever(string Message)
    {
        if (!_sending)
        {
            _sending = true;
            SendMessage = Message;
            Debug.Log(Message);
        }
    }
    IEnumerator LiveCallBack()
    {        
        while (_ServerOn)
        {
            SendMessage_Sever("ImLive/");
            yield return new WaitForSeconds(2f);
        }
        Debug.Log("Server Down");
    }
    void client_th()
    {
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("192.168.1.22"), 9050); // ("xxx = IP ")  // 네트워크의 끝점을 IP번호와 포트번호 객체화 시킵니다.
        
        try
        {
            server.Connect(ipep);//server socket에 연결합니다.
            _isThreading = true;
        }
        catch (SocketException e)
        {
            Debug.Log(e.ToString());
        }
        while (_isThreading)
        {
            if (_sending)
            {
                if (SendMessage != "Empty")
                {
                    Debug.Log(SendMessage + " / Sending");
                    byte[] msg = Encoding.UTF8.GetBytes(SendMessage);
                    server.Send(msg);
                    string State_Message = SendMessage.Split('/')[0] ;
                    if (State_Message == "OutClient")
                    {
                        Debug.Log("Client Closed.");
                        _isThreading = false;
                        _ServerOn = false;
                    }
                    SendMessage = "Empty";
                    _sending = false;
                }
            }
            Thread.Sleep(33);
        }
        //읽고 쓰기위한 스트림을 생성합니다.
        /*
        NetworkStream ns = new NetworkStream(server);
        StreamReader sr = new StreamReader(ns);
        StreamWriter sw = new StreamWriter(ns);      
        data = sr.ReadLine();
        Debug.Log(data);
        Console.WriteLine(data);
        ipdata = sr.ReadLine();
        portdata = sr.ReadLine();
        
        sw.Flush();
        while (_isThreading)
        {
           // yield return new WaitForSeconds(0.1f);
            
            if (input == "exit") break;
            if (SendMessage != "Empty")
            {
                sw.WriteLine(SendMessage);
                sw.Flush();
                SendMessage = "Empty";
                _sending = false;
            }
            Debug.Log("data get");
            data = sr.ReadLine();
            Debug.Log("Server Receive {0}-{1} << {2} "+ ipep.Address+ ipep.Port+ data);
            sw.Flush();

            try
            {
                data = sr.ReadLine();//client에서 받은 string을 data에 담습니다.
                Debug.Log(data);
            }
            catch (IOException e)
            {
                Debug.Log(e);
            }
            catch (SocketException e)
            {
                Debug.Log(e);
            }

        
        }        
        ns.Close();
        sw.Close();
        ns.Close();*/
        server.Shutdown(SocketShutdown.Both);
        server.Close();
       // yield return null;
    }
}

