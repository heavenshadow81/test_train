using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BG_BatMove : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] batBody;
    [SerializeField] Sprite[] batBodySprite;

    [SerializeField] BatBody bodyAnim;
    [SerializeField] LeftBatWings leftAnim;
    [SerializeField] RightBatWings rightAnim;

    public void ColorChange()
    {
        //그린일 때
        if(gameObject.CompareTag("Green"))
        {
            BG_Manger.green--;
            BG_Manger.orange++;

            //색상 오렌지로 바꿔줌
            gameObject.tag = "Orange";

            for(int i = 0; i < batBody.Length; i++)
            {
                batBody[i].sprite= batBodySprite[i+3];
            }

            //박쥐 애니메이션 실행
            bodyAnim.ShakeBody();
            leftAnim.FlapWings();
            rightAnim.FlapWings();

            //크기가 커졌다가 작아짐
            gameObject.transform.DOScale(1, 0.5f).OnComplete(() =>
            {
                gameObject.transform.DOScale(0.3f, 0.5f);
            });
        }
        else
        {
            BG_Manger.green++;
            BG_Manger.orange--;

            //색상 그린으로 바꿔줌
            gameObject.tag = "Green";

            for (int i = 0; i < batBody.Length; i++)
            {
                batBody[i].sprite = batBodySprite[i];
            }

            //박쥐 애니메이션 실행
            bodyAnim.ShakeBody();
            leftAnim.FlapWings();
            rightAnim.FlapWings();

            //크기가 커졌다가 작아짐
            gameObject.transform.DOScale(1, 0.5f).OnComplete(() =>
            {
                gameObject.transform.DOScale(0.3f, 0.5f);
            });
        }
    }
}

