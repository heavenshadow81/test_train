using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    AudioSource theAudio; //효과음 재생

    void Start()
    {
        theAudio = GetComponent<AudioSource>();
    }

    public void Play()
    {
        theAudio.Play();
    }
}
