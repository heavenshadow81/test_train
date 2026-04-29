using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExtraUI : MonoBehaviour
{
    public GameObject setting;
    public GameObject[] settingMenu;

    public GameObject[] menuBtn;
    public Sprite[] menuImage;

    public GameObject soundBtn;
    public Sprite[] soundImage;

    bool soundCheck;
    public TextMeshProUGUI settingTitle;
    public TextMeshProUGUI soundTitle;


    private void Start()
    {
        if (soundBtn != null)
        {
            if (AudioListener.volume == 0)
            {
                soundBtn.GetComponent<Image>().sprite = soundImage[1];
                soundTitle.text = "소리 켜기";
                soundCheck = true;
            }
            else
            {
                soundBtn.GetComponent<Image>().sprite = soundImage[0];
                soundTitle.text = "소리 끄기";
                soundCheck = false;
            }
        }
    }

    public void Home()
    {
        SoundMGR.Instance.SoundPlay("0.설정_홈");
        SoundMGR.Instance.bgmSource.Play();
        SceneManager.LoadSceneAsync(0);
    }

    public void Home_PlayGround()
    {
        //SoundMGR.Instance.SoundPlay("PlayGround_Click");
        //SoundMGR.Instance.bgmSource.Play();
        //SceneManager.LoadSceneAsync("00.PlayGroundMenu");
        Home();
    }

    public void Home_TouchPang()
    {
        //SoundMGR.Instance.SoundPlay("PlayGround_Click");
        //SoundMGR.Instance.bgmSource.Play();
        //SceneManager.LoadSceneAsync(1);

        //UIManager_PlayGround.pageNum = 0; //페이지 넘버 초기화
        Home();
    }

    public void Setting()
    {
        SoundMGR.Instance.SoundPlay("0.설정_버튼");
        setting.SetActive(true);
    }

    public void Setting_PlayGround()
    {
        SoundMGR.Instance.SoundPlay("PlayGround_Click");
        setting.SetActive(true);
    }

    public void MenuOpen(int menu)
    {
        SoundMGR.Instance.SoundPlay("0.설정_버튼");

        for (int i = 0; i < settingMenu.Length; i++)
        {
            settingMenu[i].SetActive(false);
            menuBtn[i].SetImage(menuImage[0]);
        }

        settingMenu[menu].SetActive(true);
        menuBtn[menu].SetImage(menuImage[1]);

        if(menu == 0)
        {
            settingTitle.text = "설정";
        }
        else
        {
            settingTitle.text = "문의";
        }
    }

    public void MenuOpen_PlayGround(int menu)
    {
        SoundMGR.Instance.SoundPlay("PlayGround_Click2");

        for (int i = 0; i < settingMenu.Length; i++)
        {
            settingMenu[i].SetActive(false);
            menuBtn[i].SetImage(menuImage[0]);
        }

        settingMenu[menu].SetActive(true);
        menuBtn[menu].SetImage(menuImage[1]);

        if (menu == 0)
        {
            settingTitle.text = "설정";
        }
        else
        {
            settingTitle.text = "문의";
        }
    }

    public void SettingOff()
    {
        SoundMGR.Instance.SoundPlay("0.설정_닫기");
        setting.SetActive(false);
    }

    public void SettingOff_PlayGround()
    {
        SoundMGR.Instance.SoundPlay("PlayGround_Click");
        setting.SetActive(false);
    }

    public void EndGame()
    {
        SoundMGR.Instance.SoundPlay("0.설정_종료");
        Application.Quit();
    }

    public void EndGame_PlayGround()
    {
        SoundMGR.Instance.SoundPlay("PlayGround_Click2");
        Application.Quit();
    }

    public void Sound()
    {
        SoundMGR.Instance.SoundPlay("0.설정_버튼");
        if (!soundCheck)
        {
            AudioListener.volume = 0;
            soundBtn.GetComponent<Image>().sprite = soundImage[1];
            soundTitle.text = "소리 켜기";
            soundCheck = true;
        }
        else
        {
            AudioListener.volume = 1;
            soundBtn.GetComponent<Image>().sprite = soundImage[0];
            soundTitle.text = "소리 끄기";
            soundCheck = false;
        }
    }

    public void Sound_PlayGround()
    {
        SoundMGR.Instance.SoundPlay("PlayGround_Click2");
        if (!soundCheck)
        {
            AudioListener.volume = 0;
            soundBtn.GetComponent<Image>().sprite = soundImage[1];
            soundTitle.text = "소리 켜기";
            soundCheck = true;
        }
        else
        {
            AudioListener.volume = 1;
            soundBtn.GetComponent<Image>().sprite = soundImage[0];
            soundTitle.text = "소리 끄기";
            soundCheck = false;
        }
    }
}
