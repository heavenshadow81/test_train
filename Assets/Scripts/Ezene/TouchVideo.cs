using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchVideo : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    UnityEngine.Video.VideoPlayer videoplayer;

    [SerializeField]
    UnityEngine.UI.Slider slider;
    [SerializeField]
    UnityEngine.UI.Button[] button;
    //터치하면..?
    public void OnPointerDown(PointerEventData eventData)
    {
        //슬라이더 비 활성화 시 활성화!!
        if (!slider.gameObject.activeSelf&& videoplayer.isPlaying)
        {
            StartCoroutine(SetSlider());
        }
        else if (videoplayer.isPlaying && slider.gameObject.activeSelf)
        {
            videoplayer.Pause();
            print($"멈춘 프레임{videoplayer.frame}");
        }
        else
        {
            videoplayer.Play();
            print($"다시 재생하는 프레임{videoplayer.frame}");
        }
    }

    //슬라이더 활성/비활성 전환
    IEnumerator SetSlider()
    {
        slider.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        slider.gameObject.SetActive(false);
        yield break;
    }
    //비활성화될 때 뒤로가기 버튼 자동 바꾸기
    void OnDisable()
    {
        StopAllCoroutines();
        videoplayer.Stop();
        
        button[0].gameObject.SetActive(false);
        button[1].gameObject.SetActive(true);
    }
}
