using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkController : MonoBehaviour
{
    private AudioSource audioSource;
    public Animator ani;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        ani = GetComponent<Animator>();
    }
    public void SharkSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void Update()
    {
        if (FishGameManager.Instance.stateClass.state == GameState.GameResult)
        {
            gameObject.SetActive(false);
        }
    }
}
