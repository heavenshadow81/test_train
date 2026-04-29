using UnityEngine;
using System.Collections;

public class TouchMotionAlphabetSoundManager : MonoBehaviour {

    public string fileName;
    public string filePath;
    //const string filePath = "TouchMotion/TouchAlphabetGame/Sound/";
    AudioClip[] audioClipArr;
    AudioSource audioSource ;

    public static TouchMotionAlphabetSoundManager alphabetSound;

    void Awake()
    {
        alphabetSound = this;
       
        audioSource = new AudioSource();
        
        
        InitializeAudio();
    }

    void InitializeAudio()
    {
        if(audioClipArr ==null)
        {
            audioClipArr = new AudioClip[26];
            char initial = 'A';
            for (int i = 0; i < audioClipArr.Length; ++i)
            {
                char fileIndex = (char)((int)initial + i);
                //Debug.Log(fileIndex);
                audioClipArr[i] = Resources.Load(filePath + fileName + fileIndex) as AudioClip;
            }
        }
    }

    public AudioClip GetClip(int index)
    {
        InitializeAudio();
        if (audioClipArr.Length <= index)
            return null;

        return audioClipArr[index];
    }

    public void PlayAlphabetSound(int _index)
    {
        if (_index >= audioClipArr.Length) return;
         AudioSource.PlayClipAtPoint(audioClipArr[_index] , Vector3.zero);
    }
}
