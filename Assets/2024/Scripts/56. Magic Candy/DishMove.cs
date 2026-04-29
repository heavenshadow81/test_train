using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


public class DishMove : TouchManager_3DTouch
{
    [SerializeField] Transform dish; //접시 오브젝트
    

    public override void HandleInput(Vector2 pos)
    {
        isTouchable = true;
        // 마우스 또는 터치 입력을 사용하여 Ray 생성
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;

        // Raycast로 오브젝트 감지
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null) //콜라이더가 존재하고 터치값이 true일때
            {
                if (hit.collider.tag == "target") //레이에 맞은 콜라이더의 태그가 target라면
                {
                    dish.DOMove(hit.collider.transform.position,1f);

                    SoundMGR.Instance.SoundPlay("srrk");
                }
            }
            else
            {
                print("콜라이더 없음");
            }
        }
    }
}
