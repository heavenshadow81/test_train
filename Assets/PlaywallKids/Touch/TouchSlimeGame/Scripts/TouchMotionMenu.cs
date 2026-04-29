using UnityEngine;
using System.Collections;

public class TouchMotionMenu : MonoBehaviour {

    string szTag = "PlayButton";
    string szPaly = "imgIconPlay";
    string szPause = "imgIconPause";
    UIButton mImgPlayBtn;
    UIButton imgPlayBtn
    {
        get
        {
            if(mImgPlayBtn == null)
            {
                UIButton[] imgs = gameObject.GetComponentsInChildren<UIButton>();
                for(int i = 0 ; i< imgs.Length ; ++i)
                {
                    if(szTag == imgs[i].tag)
                    {
                        mImgPlayBtn = imgs[i];
                        break;
                    }
                    if (mImgPlayBtn == null)
                    { mImgPlayBtn = imgs[0]; }
                }
            }
            return mImgPlayBtn;
        }
    }
    bool bPause;
    float fTime;

    void OnEnable()
    {
        bPause = false;
        fTime = 0;
    }

    void Update()
    {
        if(fTime > 0)
        {   fTime -= Time.fixedDeltaTime; }
    }

	public void PlayPauseButton()
    {
        if (fTime <= 0)
        {
            fTime = 0.5f;
            bPause = !bPause;
            Time.timeScale = (bPause ? 0 : 1f);
            imgPlayBtn.normalSprite = bPause ? szPause : szPaly;
        }
    }

    public void OnDisable()
    {
        if (Time.timeScale != 1f)
        { Time.timeScale = 1f; }
    }

    public void ComeBackToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("BigBoardMainMenu");
    }
}
