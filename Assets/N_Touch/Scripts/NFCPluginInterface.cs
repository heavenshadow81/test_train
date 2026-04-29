using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using UnityEngine;

namespace ML.NFCPlugin
{
    /// <summary>
    /// NFC 인식 프로그램(NFC-plugin.exe)와 상호 통신을 하기 위한 클래스.
    /// </summary>
    public static class NFCPluginInterface
    {
        private static Socket socket;
        private const string IP = "127.0.0.1";

        private const int port = 14630;     // Just named; 14:N, 6:F, 3:C, 0:\0
        private static byte[] buffer = new byte[256];
        private static int offset = 0;
        private static Process nfcBackgroundProgram;

        public static bool Open()
        {
            if (nfcBackgroundProgram == null)
            {
                nfcBackgroundProgram = new Process();
                string nfcBackgroundProgramFilename = Application.dataPath + "/NFC-plugin.exe";
#if UNITY_EDITOR
                if (!System.IO.File.Exists(nfcBackgroundProgramFilename))
                    nfcBackgroundProgramFilename = Application.dataPath + "/NFC-plugin.exe";
#endif
                nfcBackgroundProgram.StartInfo.FileName = nfcBackgroundProgramFilename;
                //nfcBackgroundProgram.Start();
            }

            if (socket == null)
            {
                try
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
                    socket.Connect(remoteEndPoint);
                    socket.Blocking = false;
                    socket.NoDelay = true;
                    return true;
                }
                catch
                {
                    socket = null;
                }
            }

            return false;
        }

        public static void Close()
        {
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
            offset = 0;

            //if (nfcBackgroundProgram != null)
            //{
            //    if (!nfcBackgroundProgram.HasExited)
            //        nfcBackgroundProgram.Kill();
            //    nfcBackgroundProgram = null;
            //}
        }

        public static Packet Receive()
        {
            if (socket != null)
            {
                try
                {
                    int recv = 0;

                    if (offset < 4)
                    {
                        recv = socket.Receive(buffer, 8 - offset, SocketFlags.None);
                        offset += recv;
                    }

                    if (offset >= 8)
                    {
                        byte[] buffer4 = new byte[4];
                        Array.Copy(buffer, 4, buffer4, 0, 4);
                        Array.Reverse(buffer4);
                        int len = BitConverter.ToInt32(buffer4, 0);
                        {
                            recv = socket.Receive(buffer, offset, Math.Max(0, len - offset), SocketFlags.None);
                            offset += recv;
                            if (offset >= len)
                                offset -= len;
                            return new Packet(buffer);
                        }
                    }
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode != SocketError.WouldBlock)
                    {
                        Close();
                    }
                }
            }
            return null;
        }
    }
}