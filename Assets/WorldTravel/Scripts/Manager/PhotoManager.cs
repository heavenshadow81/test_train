using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Threading.Tasks;

public class PhotoManager : MonoBehaviour
{
    public RenderTexture AllPhoto;
    public RenderTexture TiketPhoto;

    public GameObject[] PhotoObj;

    public RawImage[] Photo5;
    public RawImage[] Photo4;
    public RawImage[] Photo3;
    public RawImage[] Photo2;
    public RawImage Photo1;
    public int PhotoCnt;
    public int PhotoSize;

    public RawImage Ticket;

    public Texture2D TicketPhotoCapture;
    public Texture2D AllPhotoCapture;
    public TCP_Server server;

    [SerializeField]
    Camera[] takePic;
    public void SetSize(int Size)
    {
        PhotoCnt = 0;
        PhotoSize = Size;
        for (int i = 0; i < PhotoObj.Length; i++)
            PhotoObj[i].SetActive(false);
        PhotoObj[Size - 1].SetActive(true);
    }
    public void SetPhoto(Texture2D photo)
    {
        switch (PhotoSize)
        {
            case 1:
                Photo1.texture = photo;
                break;
            case 2:
                if (PhotoCnt < Photo2.Length)
                    Photo2[PhotoCnt].texture = photo;
                break;
            case 3:
                if (PhotoCnt < Photo3.Length)
                    Photo3[PhotoCnt].texture = photo;
                break;
            case 4:
                if (PhotoCnt < Photo4.Length)
                    Photo4[PhotoCnt].texture = photo;
                break;
            case 5:
                if (PhotoCnt < Photo5.Length)
                    Photo5[PhotoCnt].texture = photo;
                break;
        }

        Ticket.texture = photo;
        PhotoCnt++;
        Debug.Log(PhotoCnt + " / " + PhotoSize);
        StartCoroutine(Capture_TicketImage());

    }
    IEnumerator Capture_TicketImage()
    {
        Debug.Log("티켓 캡쳐");
        yield return new WaitForEndOfFrame();

        
        //RenderTexture.active = TiketPhoto;
        takePic[0].Render();
        //UnityEngine.Rendering.AsyncGPUReadback.Request(takePic[0].targetTexture, 0, TextureFormat.ARGB32, OnCompleteTextureCopy);
        Texture2D temp = new Texture2D(takePic[0].targetTexture.width, takePic[0].targetTexture.height);
        //RenderTexture.active = null;
        temp.ReadPixels(new Rect(0, 0, takePic[0].targetTexture.width, takePic[0].targetTexture.height), 0, 0);
        temp.Apply();
        TicketPhotoCapture = temp;
        //TicketPhotoCapture.ReadPixels(new Rect(0, 0, takePic[0].targetTexture.width, takePic[0].targetTexture.height), 0, 0);
        //TicketPhotoCapture.Apply();
        yield return new WaitForEndOfFrame();
        //RenderTexture.active = null;
        yield return new WaitForFixedUpdate();
        //0,0부터 1024,768까지의 사각형크기를 text의 0,0부터 읽어옴.
        //그래픽카드 쓰는 법...?

        //이 방식으로 하니 미니맵이 다르다고 거부....
        //확인해보자..!
        //Tickettex.ReadPixels(new Rect(0, 0, TiketPhoto.width, TiketPhoto.height), 0, 0, true);
        //Tickettex.Apply();
        yield return new WaitForEndOfFrame();
        //yield return new WaitForSeconds(0.2f);
        server.SaveTexture(TicketPhotoCapture);
        #region 사진을 파일로 저장
        yield return new WaitForFixedUpdate();
        //파일로 저장
        //System.DateTime currentTime = System.DateTime.Parse(System.DateTime.Now.ToString());
        //string _date = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second);
        //string path = $"{Application.persistentDataPath}/" + $"ScreenShot/{currentTime.Year}년{currentTime.Month}월{currentTime.Day}일";

        //string Resize = $"{path}/{_date}.png";
        ////폴더가 없으면...? 새로 생성
        //if (!System.IO.Directory.Exists(path))
        //{
        //    System.IO.Directory.CreateDirectory(path);
        //}


        //using (System.IO.FileStream fs = new System.IO.FileStream(Resize, System.IO.FileMode.Create))
        //{
        //    var picInformation = temp.EncodeToPNG();
        //    fs.Write(picInformation, 0, picInformation.Length);
        //    fs.Close();
        //}
        yield return new WaitForSeconds(0.05f);
        
        #endregion
        yield return new WaitForSeconds(2f);
        if (PhotoCnt >= PhotoSize)
        {
            StartCoroutine(Capture_AllImage());
        }
        yield break;
    }

    IEnumerator Capture_AllImage()
    {
        Debug.Log("폴라로이드 캡쳐");
        yield return new WaitForEndOfFrame();

        //RenderTexture.active = AllPhoto;?이거 활성화 안 되면... 검게 나오는 것...?
        takePic[1].Render();
        Texture2D temp = new Texture2D(takePic[1].targetTexture.width, takePic[1].targetTexture.height);
        temp.ReadPixels(new Rect(0, 0, takePic[1].targetTexture.width, takePic[1].targetTexture.height), 0, 0, true);
        temp.Apply();
        //UnityEngine.Rendering.AsyncGPUReadback.Request(takePic[1].targetTexture, 0, TextureFormat.ARGB32, OnCompleteTextureCopy);
        AllPhotoCapture = temp;
        
        yield return new WaitForEndOfFrame();
        RenderTexture.active = null;
        //0,0부터 1024,768까지의 사각형크기를 text의 0,0부터 읽어옴.
        yield return new WaitForFixedUpdate();
        server.SaveTexture(AllPhotoCapture);
        #region 사진 파일로 저장하는 부분
        //파일로 저장..?
        //System.DateTime currentTime = System.DateTime.Parse(System.DateTime.Now.ToString());
        //string _date = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second);
        //string path = $"{Application.persistentDataPath}/" + $"ScreenShot/{currentTime.Year}년{currentTime.Month}월{currentTime.Day}일";
        //string Resize = $"{path}/{_date}.png";
        ////폴더가 없으면...? 새로 생성
        //if (!System.IO.Directory.Exists(path))
        //{
        //    System.IO.Directory.CreateDirectory(path);
        //}
        //using (System.IO.FileStream fs = new System.IO.FileStream(Resize, System.IO.FileMode.Create))
        //{
        //    var picInformation = temp.EncodeToPNG();
        //    fs.Write(picInformation, 0, picInformation.Length);
        //    fs.Close();
        //}
        yield return new WaitForSeconds(0.05f);
        //System.IO.File.WriteAllBytes(Resize, picInformation);
        #endregion
        
        yield break;
    }
}
