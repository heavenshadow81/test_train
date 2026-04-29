using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//이젠 커뮤니케이션 동영상 선택..!
public class EzeneSelectVideoes : MonoBehaviour
{
    [SerializeField]
    UnityEngine.Video.VideoPlayer videoplayer;
    [SerializeField]
    UnityEngine.UI.RawImage ims;
    [SerializeField]
    GameObject buttongroups;
    [SerializeField]
    EzeneFileWay fileway;
    [SerializeField]
    UnityEngine.UI.Slider slider;
    //비디오 선택!
    //비디오 프레임
    private void Start()
    {
        //이렇게 이벤트로 설정해두어야 프레임드롭 없이 자연스럽게 영상 재생이 됨
        videoplayer.prepareCompleted += (a) =>
        {
            slider.maxValue = videoplayer.frameCount;
            videoplayer.Play();
        };
        videoplayer.loopPointReached += (video) =>
        {
            videoplayer.Stop();
            ims.canvasRenderer.Clear();
            ims.gameObject.SetActive(false);
            buttongroups.SetActive(true);
        };
    }
    public void SelectVideo()
    {
        StopAllCoroutines();
        StartCoroutine(ReadyVideo());
        StartCoroutine(SliderSet());
    }
    //비디오 시작/멈춤
    IEnumerator ReadyVideo()
    {
        videoplayer.Stop();
        videoplayer.url = fileway.Path;
        videoplayer.Prepare();//비디오 준비...
        yield return new WaitUntil(() => videoplayer.isPrepared);
        yield break;
    }
    //진행상황 슬라이더 업데이트
    IEnumerator SliderSet()
    {
        while (ims.gameObject.activeSelf)
        {
            yield return new WaitForSeconds(0.05f);
            slider.value = videoplayer.frame;
        }
        yield break;
    }
    //영상 재생
    
    public void SetVideoFrame(UnityEngine.UI.Slider slider)
    {
        videoplayer.frame = (long)slider.value;
    }
}
