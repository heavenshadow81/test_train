using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

public class EmailTest : MonoBehaviour {

    public string mail_adress;
	// Use this for initialization
	void Start () {
        string[] tmp = mail_adress.Split(' ');
        string newmail = "";
        for (int i = 0; i < tmp.Length; i++)
            newmail += tmp[i];
        Debug.Log(newmail);

        MailMessage mail = new MailMessage();
        

        //        Regex.Replace(mail_adress, " ", "");
        
        mail.From = new MailAddress("MapoCentralLib@gmail.com");
        mail.To.Add(newmail);
        mail.Subject = "[마포중앙도서관]전세계를 누빈 당신의 추억을 전해드립니다.";
        mail.Body = "세계명소 여행은 즐거우셨나요? \n\n'여행'은 듣기만 해도 지금바로 떠나고 싶은 단어이죠.\n\n또한, 인생에서 가족, 친구, 지인들과 함께하는 여행은 잊을 수 없는 가장 소중한 추억입니다.\n\n받으신 촬영사진들을 보시면서 다시한번 여행감성을 느껴보세요. \n\n마포중앙도서관 IT 체험실을 방문해주셔서 감사합니다.";

        string guid = System.Guid.NewGuid().ToString();
        string nowPath = string.Format("{0}/{1}/{2}", Application.persistentDataPath, "ScreenShotEmail", guid);

        if (!Directory.Exists(nowPath))
            Directory.CreateDirectory(nowPath);

       /* for (int i = 0; i < client.SendingImagePath.Length; i++)
        {
            if (client.SendingImagePath[i] != "Empty")
            {
                string copyFile = nowPath + "/" + Path.GetFileName(client.SendingImagePath[i]);
                File.Copy(client.SendingImagePath[i], copyFile);

                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(copyFile);
                mail.Attachments.Add(attachment);
            }
        }
        */

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential("MapoCentralLib@gmail.com", "2017@akvhwnddkd") as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
        delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        { return true; };

        try
        {

            smtpServer.Send(mail);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            return;
        }

        Debug.Log("Mail Success!!");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
