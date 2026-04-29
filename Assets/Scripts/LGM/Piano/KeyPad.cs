using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyPad : MonoBehaviour, IPointerDownHandler,IPointerExitHandler,IPointerUpHandler
{
    public Transform notePos;
    public Sprite[] clickSprite;
    private Image image;
    private AudioSource audioSource;

    private void Awake()
    {
        image = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       
        PianoManager.Instance.ButtonEvent(notePos); // 건반 이벤트 
        audioSource.PlayOneShot(audioSource.clip);  // 사운드 출력(소리 중첩 가능)
        //audioSource.Play();
        image.sprite = clickSprite[1];  // 건반 클릭 시 클릭 색상으로 변경 (Button컴퍼런트를 사용할 수 없음)
    }
    // 원래 이미지로 복구
    public void OnPointerExit(PointerEventData eventData){ image.sprite = clickSprite[0]; }
    public void OnPointerUp(PointerEventData eventData){ image.sprite = clickSprite[0]; }
}