using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandSound : MonoBehaviour
{
    public AudioClip Button;
    public AudioClip True;
    public AudioClip False;
    public AudioClip trash;
    public AudioClip rainingSand;
    public AudioClip dalkack;

    AudioSource sound;

    private void OnEnable()
    { 
        sound = GetComponent<AudioSource>();
    }

    public void Btn()
    {
        StartCoroutine(raining());
    }
    public void TrueSound()
    {
        sound.clip = True;
        sound.Play();
    }
    public void FalseSound()
    {
        sound.clip = False;
        sound.Play();
    }
    public void Trash()
    {
        sound.PlayOneShot(trash);
    }
    public void Raining()
    {
        StartCoroutine (rainingSound());
    }

    IEnumerator raining()
    {
        sound.PlayOneShot(Button);
        yield return new WaitForSeconds(0.3f);
        sound.PlayOneShot(dalkack);
    }

    IEnumerator rainingSound()
    {
        yield return new WaitForSeconds(0.4f);
        sound.PlayOneShot(rainingSand);
    }
}
