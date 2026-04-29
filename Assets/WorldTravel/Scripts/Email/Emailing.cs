using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.IO;
using System;
public class Emailing : MonoBehaviour {

    public EmailClient client;
    Thread EmialThread;
    bool Threading;
    bool EmailngProcessStart;

    public GameObject EmailingWindow;
    public GameObject EmailingOk;
    public GameObject ErrorWindow;
    public Text ErrorText;

    public Text EmailLog;
    public Text Debuglog;

    string guid;
    string nowPath;

    string emailsender = "officeplaywall@gmail.com";
    string emailpw = "playwall12!";

    private void Awake()
    {
        Threading = true;
        EmailngProcessStart = false;


        EmialThread = new Thread(EmailStart);
        EmialThread.Start();
        guid = System.Guid.NewGuid().ToString();
        nowPath = $"{Application.persistentDataPath}/{"ScreenShotEmail"}/{guid}";
    }
    

    void OnDestroy()
    {
        SocketClose();
    }
    public void SocketClose()
    {
        if (EmialThread != null)
        {
            Threading = false;
            EmialThread.Join();
        }
        
    }
    public string _filename;
    void EmailStart()
    {
        while (Threading)
        {
            if (EmailngProcessStart)
            {
                string[] tmp = to_Email.Split(' ');
                string newmail = "";
                for (int i = 0; i < tmp.Length; i++)
                    newmail += tmp[i];

                client.EmailSending = true;
                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(emailsender);
                mail.To.Add(to_Email);
                mail.Subject = "[플레이월]전세계를 누빈 당신의 추억을 전해드립니다.";
                mail.Body = "세계명소 여행은 즐거우셨나요? \n\n'여행'은 듣기만 해도 지금바로 떠나고 싶은 단어이죠.\n\n또한, 인생에서 가족, 친구, 지인들과 함께하는 여행은 잊을 수 없는 가장 소중한 추억입니다.\n\n받으신 촬영사진들을 보시면서 다시한번 여행감성을 느껴보세요. \n\n ";

                //guid = System.Guid.NewGuid().ToString();
                //nowPath = string.Format("{0}/{1}/{2}", Application.persistentDataPath, "ScreenShotEmail", guid);

                if (!Directory.Exists(nowPath))
                    Directory.CreateDirectory(nowPath);

                for (int i = 0; i < client.SendingImagePath.Length; i++)
                {
                    if (client.SendingImagePath[i] != "Empty")
                    {
                        string copyFile = nowPath + "/" + Path.GetFileName(client.SendingImagePath[i]);
                        File.Copy(client.SendingImagePath[i], copyFile);

                        Attachment attachment;
                        attachment = new System.Net.Mail.Attachment(copyFile);
                        mail.Attachments.Add(attachment);
                    }
                }
                
                
                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                smtpServer.Port = 587;
                smtpServer.Credentials = new NetworkCredential(emailsender, emailpw) as ICredentialsByHost;
                smtpServer.EnableSsl = true;
                ServicePointManager.ServerCertificateValidationCallback =
                delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };
                try
                {
                    smtpServer.Send(mail);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                    ch_thread = "Fail";
                    EmailngProcessStart = false;
                    client.EmailSending = false;
                    return;
                }                
               // Directory.Delete(nowPath, true);
                //Thread_smtp.Send(_mailing);           
                ch_thread = "Success";
                EmailngProcessStart = false;
                client.EmailSending = false;
            }
        }

    }
    SmtpClient Thread_smtp;
    MailMessage _mailing;
    string ch_thread;
    string to_Email;
    private void Update()
    {
        if (ch_thread == "Success")
        {
            ch_thread = "empty";
            EmailLog.text = "이메일 전송을 완료하였습니다.";
            EmailingOk.SetActive(true);
        }
        if (ch_thread == "Fail")
        {
            ch_thread = "empty";
            ErrorText.text = "이메일 전송을 실패하였습니다.";
            EmailingWindow.SetActive(false);
            ErrorWindow.SetActive(true);
        }
    }
    public void SendMail(string ToEmail)
    {
        to_Email = ToEmail;
        EmailLog.text = "이메일을 전송하였습니다.";
        EmailingOk.SetActive(true);
        EmailingWindow.SetActive(true);
        EmailngProcessStart = true;       
    }
}
