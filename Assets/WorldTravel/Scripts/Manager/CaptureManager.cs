using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using ML.MapoContents.WorldTravel;
public class CaptureManager : MonoBehaviour
{
    public static CaptureManager Instance;

    public GameObject[] Count_Obj;
    public GameObject PlayUI;
	public GameObject PhotoImage;
	public Image CountImage;
	public Image CountImage_shade;
    public Sprite[] Counts;
    public RawImage Shot_eff;
	public GameObject ChromaObj;
    public Texture2D[] UserScreenShots;
    public int _ScreenShotIdx;
    public bool Full_ScreenShot;
    public bool _isCapture;
    const string SCREENSHOT_DEFAULT_FOLDER = "ScreenShot";

    public RenderTexture[] test;
    public GameObject Cursor_obj;
    public string path = "";

    public int _timer;
    int _num;
    public TCP_Server server;
    public PhotoManager photomanager;
    private void Awake()
    {
        _isCapture = false;
        Full_ScreenShot = false;
        UserScreenShots = new Texture2D[5];
		_ScreenShotIdx = 0;
		Count_Obj[0].SetActive(false);
		Count_Obj[1].SetActive(false);
        Instance = this;
        _timer = 4;
        _num = 0;
        path = string.Format("{0}/{1}", Application.persistentDataPath, SCREENSHOT_DEFAULT_FOLDER);
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);
    }
	public void ResetInit()
	{
        _isCapture = false;
        Full_ScreenShot = false;
		UserScreenShots = new Texture2D[5];

		_ScreenShotIdx = 0;
		Count_Obj[0].SetActive(false);
		Count_Obj[1].SetActive(false);
	}

    public void PhotoTime_Event()
    {
        if (_timer >= 4)
            StartCoroutine(ScreenShot());
    }    

    IEnumerator ScreenShot()
	{
        _isCapture = true;
        Count_Obj[0].SetActive(true);
		Count_Obj[1].SetActive(true);
		ChromaObj.SetActive (true);
        PhotoImage.SetActive(false);
        Cursor_obj.SetActive(false);
        while (_timer >= 0)
        {
            CountImage.sprite = Counts[_timer];
			CountImage_shade.sprite = Counts [_timer];
            yield return new WaitForSeconds(1f);
			_timer -= 1;
        }

		Count_Obj[0].SetActive(false);
		Count_Obj[1].SetActive(false);
        PlayUI.SetActive(false);
        StartCoroutine(FadeOut_RawImage());
        yield return new WaitForSeconds(0.6f);      
        StartCoroutine(Capture_Image());
        
        yield return new WaitForSeconds(1f);
        _timer = 4;
    }
    IEnumerator Capture_Image()
    {
        yield return new WaitForEndOfFrame();
        Color fade_out = Shot_eff.color;
        fade_out.a = 0;
        Shot_eff.color = fade_out;

        int w = 3840;// Screen.width;
        int h = 1200;// Screen.height;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, true);
        RenderTexture.active = test[0];
        //0,0부터 1024,768까지의 사각형크기를 text의 0,0부터 읽어옴.
        tex.ReadPixels(new Rect(0, 0, test[0].width, test[0].height), 0, 0, true);
        RenderTexture.active = test[1];
        tex.ReadPixels(new Rect(0, 0, test[1].width, test[1].height), 1920, 0, true);
        tex.Apply();

        RenderTexture.active = null;
        Texture2D rt = ScaleTexture(tex, 3840, 1200);

        //보낼 캡쳐이미지를 배열에 저장
        photomanager.SetPhoto(rt);

        //server.SaveTexture(rt);

        /*  Texture2D Test_43 = new Texture2D(1024, 768, TextureFormat.RGBA32, true);
          Test_43.ReadPixels(new Rect(512, 0, 1536, 768), 0, 0, true);*/
          
        if (!Full_ScreenShot)
        {
            UserScreenShots[_ScreenShotIdx] = rt;
            _ScreenShotIdx++;
        }
            
        if (_ScreenShotIdx >= UserScreenShots.Length)
        {
            _ScreenShotIdx = 0;
            Full_ScreenShot = true;
        }
        #region 사진 저장
        System.DateTime currentTime = System.DateTime.Parse(System.DateTime.Now.ToString());
        string _date = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second);
        string path = $"{Application.persistentDataPath}/" + $"ScreenShot/{currentTime.Year}년{currentTime.Month}월{currentTime.Day}일";

        string Resize = $"{path}/{_date}.png";
        //폴더가 없으면...? 새로 생성
        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }


        using (System.IO.FileStream fs = new System.IO.FileStream(Resize, System.IO.FileMode.Create))
        {
            var picInformation = rt.EncodeToPNG();
            fs.Write(picInformation, 0, picInformation.Length);
            fs.Close();
        }
        #endregion
        ChromaObj.SetActive(false);
        PlayUI.SetActive(true);
        PhotoImage.SetActive(true);
        Cursor_obj.SetActive(true);
        yield return new WaitForSeconds(2f);
        TravelManager.Instance.NextTravel();
        _isCapture = false;
    }
    public Texture2D[] GetCpature()
    {
        return UserScreenShots;
    }

    Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    //찰칵 효과 페이드아웃
    IEnumerator FadeOut_RawImage()
    {
        Color fade_out = Shot_eff.color;
        fade_out.a = 1;
        while (fade_out.a >= 0)
        {
            fade_out.a -= 0.05f;
            Shot_eff.color = fade_out;
            yield return new WaitForSeconds(0.01f);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            StartCoroutine(ScreenShot());
        }
    }
}
