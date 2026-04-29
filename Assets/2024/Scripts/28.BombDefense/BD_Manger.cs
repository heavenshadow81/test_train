using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class BD_Manger : PlayManager_PlayGround
{
    public int score; //스코어
    [SerializeField] SpriteRenderer[] bomberman; 


    bool cooltime; //터치쿨타임

    protected override void Init()
    {
        base.Init();

        for(int i = 0; i < bomberman.Length; i++)
        {
            bomberman[i].gameObject.SetActive(true);
        }
    }

    public override void HandleInput(Vector2 inputPosition)
    {
        isTouchable = true;

        if (!cooltime)
        {
            // 터치/마우스 위치를 월드 좌표로 변환
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

            // 터치/마우스 위치에서 카드 찾기
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (hit.collider != null)
            {
                // 터치한 카드가 어떤 태그를 가지고 있는지 확인
                if (hit.collider.CompareTag(TeamNameString))
                {
                    SoundMGR.Instance.SoundPlay("PlayGround_Click");

                    cooltime = true;
                    GameObject particle = Instantiate(effect,hit.collider.transform.position,Quaternion.identity);
                    Destroy(particle, 1f);

                    for (int i = 0; i < bomberman.Length; i++)
                    {
                        if (hit.collider.name == i.ToString())
                        {
                            //폭탄 현재 방향에서 전환하여 이동
                            if (hit.collider.tag=="Green")
                            {
                                bomberman[i].flipX=false;
                                bomberman[i].gameObject.transform.DOMoveX(7, 3);
                            }
                            else if (hit.collider.tag == "Orange")
                            {
                                bomberman[i].flipX = true;
                                bomberman[i].gameObject.transform.DOMoveX(-7, 3);
                            }
                        }
                    }

                    Invoke("CooltimeReset", 0.2f);

                }
            }
        }
    }

    void CooltimeReset()
    {
        cooltime =false;
    }

    public override void CorrectAnswer(GameObject touched)
    {
    }

    public override void WrongAnswer(GameObject touched)
    {
    }
}
