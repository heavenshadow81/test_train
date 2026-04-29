using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Email_UIEvent : MonoBehaviour {
    //타이틀. 인트로
   // public Animation Title;
   // public GameObject AnyTap_button;


    public EmailClient client;
    public InputField Email_Input;
    public Emailing email;

    public Text EmailCheckText;
    public GameObject EmailCheck;

    public GameObject EmailObj;
    public GameObject ImageViewObj;
    public Animation ImageviewAnim;

    public GameObject ExitWindow;
    public void ExitWindowOn()
    {
        ExitWindow.SetActive(true);
    }
    public void ExitProgram()
    {
        string path = string.Format("{0}/{1}", Application.persistentDataPath, "ScreenShotEmail");
        Texture2D empty = new Texture2D(100, 100, TextureFormat.RGBA32, true);

        /*
        for (int i = 0; i < 7; i++)
        {            
            string Resize = string.Format("{0}/{1}.png", path, i);
            System.IO.File.WriteAllBytes(Resize, empty.EncodeToPNG());
        }*/

        Application.Quit();
    }
    void Start ()
    {
       // Title.Play("AppearTitle");//TitleFadeOut
    }

    public void EmailObjOn()
    {
        EmailObj.SetActive(true);
        ImageViewObj.SetActive(false);
    }

    public void Refresh()
    {
        if (!client.GetPhotoOn)
        {
            return;
        }
        Debug.Log("Resets");
        client.ResetPhoto();
        EmailObj.SetActive(false);
        ImageViewObj.SetActive(true);
        ImageviewAnim.Play("AppearMailPhoto");
    }

    public void InputuserMail()
    {
        if (!client.GetPhotoOn)
        {
            return;
        }
        ImageviewAnim.Play("EmailReady");
        Invoke("EmailObjOn", 1.2f);
    }

    public void MailCheck()
    {
        string _mail = "이메일 주소 [" + Email_Input.text + "]가 맞습니까?";
        EmailCheckText.text = _mail;
        EmailCheck.SetActive(true);
    }

    public void SendMail()
    {
        email.SendMail(Email_Input.text);
    }
    /*
    public void ReturnHome()
    {
        Title.Play("AppearTitle");
        AnyTap_button.SetActive(true);
    }
    public void AnyTap()
    {
        Title.Play("TitleFadeOut");
        AnyTap_button.SetActive(false);
    }*/
}
