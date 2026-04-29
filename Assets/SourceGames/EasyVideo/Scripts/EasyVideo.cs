using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(RawImage))]
public class EasyVideo : MonoBehaviour
{
    public enum Mode
    {
        file,
        url
    }

    [Header("Mode")]
    public Mode source;
    public VideoClip file;
    public string url;


    [Header("Configurations")]
    public bool autoPlay;
    public bool autoCloseOnEnd;
    public bool repeat;
    public int repeatTimes;
    public Vector2 videoResolution;

    RenderTexture render;
    VideoPlayer videoPlayer;
    RawImage rawImage;
    int times = 0;

    // Start is called before the first frame update
    void Start()
    {
        rawImage = GetComponent<RawImage>();
        GameObject go = new GameObject("VideoPlayer");
        go.transform.parent = transform;
        go.AddComponent<VideoPlayer>();

        videoPlayer = go.GetComponent<VideoPlayer>();
        if (source == Mode.url)
        {
            videoPlayer.url = url;
        }
        else
        {
            videoPlayer.clip = file;
        }

        videoPlayer.playOnAwake = autoPlay;
        videoPlayer.isLooping = repeat;

        render = (RenderTexture)Resources.Load("RenderTexture");
        render.width = (int)videoResolution.x;
        render.height = (int)videoResolution.y;
        videoPlayer.targetTexture = render;

        if (autoCloseOnEnd)
        {
            StartCoroutine(controlClose());
        }

        rawImage.texture = render;
    }

    IEnumerator controlClose()
    {
        yield return new WaitForSeconds((float)videoPlayer.length);

        if (repeat)
        {
            if(times < repeatTimes)
            {
                times++;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

}