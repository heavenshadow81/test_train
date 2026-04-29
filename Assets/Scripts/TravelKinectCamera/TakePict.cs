using UnityEngine;
using UnityEngine.UI;
//사진찍을 카메라 지정
public class TakePict : MonoBehaviour
{

    [SerializeField]
    Renderer display;
    [SerializeField]
    RawImage display2;
    WebCamTexture camTexture;
    private int currentIndex = 0;
    // Use this for initialization
    void Start()
    {
        //뭔가의 오류로 얘가 있다면...?
        if(camTexture!= null)
        {
            display.material.mainTexture = null;
            display2.material.mainTexture = null;
            camTexture.Stop();
            camTexture = null;
        }
        print($"디바이스 {currentIndex}번 째");
        //해상도가 더 높은 카메라가 있다면..?
        if (WebCamTexture.devices.Length > 1)
        {
            for(int i = 0; i < WebCamTexture.devices.Length; i++)
            {
                print(WebCamTexture.devices[i].name);
                if (WebCamTexture.devices[i].kind == WebCamKind.WideAngle)
                {
                    currentIndex = i;
                    print($"디바이스{currentIndex}: {WebCamTexture.devices[currentIndex].name}");
                    break;
                }
            }
        }
        WebCamDevice device = WebCamTexture.devices[currentIndex];
        print(device.name);
        camTexture = new WebCamTexture(device.name);
        display.material.mainTexture = camTexture;
        display2.material.mainTexture = camTexture;
        camTexture.Play();
        
    }

    void OnDestroy()
    {
        if (camTexture != null)
        {
            display.material.mainTexture = null;
            display2.material.mainTexture = null;
            camTexture.Stop();
            camTexture = null;
        }
    }
}
