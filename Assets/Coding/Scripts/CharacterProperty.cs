using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터 속성 스크립트
public class CharacterProperty : MonoBehaviour
{
    Animator ani;

    private void Awake()
    {
        // 캐릭터 오브젝트 애니매이터 컴포넌트 들고옴
        ani = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // 생성과 함께 ATK3 Animation 1번 실행 후 Idle로 다시 변경
        StartCoroutine(ICreateCharacterObject());
    }

    IEnumerator ICreateCharacterObject()
    {
        int ran = Random.Range(2, 22);

        ani.SetInteger("animation", ran);

        yield return new WaitForSeconds(0.8f);

        ani.SetInteger("animation", 1);
    }
}
