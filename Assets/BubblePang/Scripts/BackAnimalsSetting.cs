using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 동물 테마에만 존재하는 배경 동물 - 언어 모드에 따라 Canvas Text, TTS Sound 변경
public class BackAnimalsSetting : MonoBehaviour
{
    // 모드 - 숫자, 한글 -> koreanName & 영어 -> englishName (몬스터 터치시 canvas에서 보여주는 몬스터 이름)
    [SerializeField]
    string[] englishName, koreanName;

    // 모드 - 숫자, 한글 -> koreanSound & 영어 -> englishName (몬스터 생성시 재생되는 TTS Sound)
    [SerializeField]
    AudioClip[] englishSound, koreanSound;


    void Start()
    {
        // 현재 모드에 따라 보여지는 몬스터이름, 재생 될 TTS Sound 할당

        // 한글 모드
        if(ContentsController.Instance.contentsParameter.contents == BubblePang.ContentsType.Korean)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = koreanName[i];
                // TTS sound
                transform.GetChild(i).GetComponent<AudioSource>().clip = koreanSound[i];
            }
        }
        // 영어모드
        else if (ContentsController.Instance.contentsParameter.contents == BubblePang.ContentsType.Alphabet)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = englishName[i];
                // TTS sound
                transform.GetChild(i).GetComponent<AudioSource>().clip = englishSound[i];
            }
        }
        // 기타 - 숫자모드
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = koreanName[i];
                // TTS sound
                transform.GetChild(i).GetComponent<AudioSource>().clip = koreanSound[i];
            }
        }
        
    }
}
