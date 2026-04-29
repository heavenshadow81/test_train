using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 동물 테마에만 존재하는 배경 동물 - TouchEvent Class
public class BackAnimalsTouch : MonoBehaviour
{
    GameObject chat;

    AudioSource sound;

    private void Awake()
    {
        // 동물 이름 Canvas
        chat = transform.GetChild(2).gameObject;
        
        // TTS sound
        sound = GetComponent<AudioSource>();
    }

    // 백그라운드 오브젝트를 터치하면
    public void BackMonsterTouch()
    {
        // 대화창 띄우는 메소드 호출
        viewChat();
    }

    public void viewChat()
    {
        // 대화창과 사운드 재생하는 코루틴 함수 호출
        StartCoroutine(Chating());
    }

    // 오브젝트 이름 대화창 보여주고 2초 후 닫음
    IEnumerator Chating()
    {
        // TTS Sound 재생
        sound.Play();

        // 이름 보여주기
        chat.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        chat.SetActive(false);
    }
}
