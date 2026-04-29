using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour {
    public Animation MyAnim;
    public Transform CameraTr;
    public int Center;
    public int Min, Max;
    public bool _isOn;
    public AudioClip MyAudio;
    public AudioSource TTS_Audio;
    Vector3 rot;
    
    void Start()
    {
        Min = Center - 40;
        Max = Center + 40;
    }
    void Update()
    {
        //-15 40
        rot = CameraTr.rotation.eulerAngles;
        if (rot.y > Min && rot.y < Max)
        {
            //HUD가 나타나있지 않고, 캡쳐모드에 들어가지 않았을때만 등장
            if (!_isOn && !CaptureManager.Instance._isCapture)
            {
                _isOn = true;
                MyAnim.Play("AppearNameTag");
                /*if (TTS_Audio != null)
                {
                    TTS_Audio.clip = MyAudio;
                    TTS_Audio.Play();
                }*/
            }
            //범위 내 이지만 캡쳐모드라면 제거(등장한 상태에서 캡처에 들어갈때 제거함.)
            if (_isOn && CaptureManager.Instance._isCapture)
            {
                _isOn = false;
                MyAnim.Play("DisAppearNameTag");
               /* if (TTS_Audio != null)
                {
                    TTS_Audio.Stop();
                }*/
            }
        }
        else
        {
            //HUD가 나타나있다면 제거
            if (_isOn)
            {
                _isOn = false;
                MyAnim.Play("DisAppearNameTag");
               /* if (TTS_Audio != null)
                {
                    TTS_Audio.Stop();
                }*/
            }
        }

    }

}
