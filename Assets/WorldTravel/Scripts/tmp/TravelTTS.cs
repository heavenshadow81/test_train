using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelTTS : MonoBehaviour {

    public AudioSource TTSAudio;
    public AudioClip[] Culture;
    public AudioClip[] Art;
    public AudioClip[] City;
    public AudioClip[] Tour;
    public AudioClip[] Nature;
    public AudioClip[] History;

    public void StopTTS()
    {
        TTSAudio.Stop();
    }
    public void TTSisPlay(int theme, int travel)
    {
        switch (theme)
        {
            case 0:
                TTSAudio.clip = Culture[travel];
                break;
            case 1:
                TTSAudio.clip = Art[travel];
                break;
            case 2:
                TTSAudio.clip = City[travel];
                break;
            case 3:
                TTSAudio.clip = Tour[travel];
                break;
            case 4:
                TTSAudio.clip = Nature[travel];
                break;
            case 5:
                TTSAudio.clip = History[travel];
                break;
        }
        TTSAudio.Play();
    }

}
