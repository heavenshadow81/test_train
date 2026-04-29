using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace AnimalCard
{
    public class CardMove : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        SpriteRenderer cardRenderer; //카드 렌더러 창

        [SerializeField]
        Sprite animalSprite; //동물 카드 이미지

        [SerializeField]
        Sprite backSprite; //키드 뒷면 이미지

        public bool isFlipped = false; //카드가 뒤집혔는지 확인
        public bool isFliping = false; //카드가 뒤집히는 중인지 확인
        public bool isMatched = false; //카드가 매치되었는지 확인

        public int cardID; //카드 ID 변수

        public void SetAnimal(Sprite sprite) //카드를 해당 이미지로 변경하는 함수
        {
            animalSprite = sprite; //동물 카드 이미지를 해당 이미지로 변경
        }

        public void SetCardID(int id) //카드에 아이디 부여 함수
        {
            cardID = id; //카드 아이디를 입력한 값으로 변환
        }

        public void SetMatched() //카드 매치 체크 함수
        {
            isMatched = true; //카드 매치를 true로
        }

        void OnEnable()
        {
            isFlipped = false; //시작할 때 뒤집힘 false
            isFliping = false; //시작할 때 뒤집히는 중 false
            isMatched = false; //시작할 때 카드 매치 false
        }

        public void FlipCard() //카드 뒤집기 함수
        {
            isFliping = true; //카드 뒤집히는 중 true

            transform.DOScaleY(0, 0.2f).OnComplete(() => //스케일 Y값을 0.2초 동안 0으로 만들고 그 것이 다 끝나면
            {
                isFlipped = !isFlipped; //카드 뒤집힘이게 true면 false로, false면 true로 바꿈

                if (isFlipped) //카드가 뒤집혔을 때
                {
                    GameObject.Find("SoundManager").GetComponent<SoundManager>().Flip(); //카드 뒤집기 사운드 재생
                    cardRenderer.sprite = animalSprite; //카드 이미지를 동물 카드 이미지로 바꿈
                }
                else //아니라면
                {
                    cardRenderer.sprite = backSprite; //카드 이미지를 뒷면 이미지로 바꿈
                }

                transform.DOScaleY(1, 0.2f); //스케일 Y값을 0.2초동안 1로 만듦
                isFliping = false; //뒤집히는 중을 false로
            });
        }


        public void Click()
        {
             if (!isFliping && !isMatched &&!isFlipped) //뒤집히는 중이 아니고, 매치가 되어있지 않으며, 현재 뒤집힌 상태가 아니라면
                 GameManager.instance.CardClicked(this);//카드 뒤집기 함수 실행
        }

        public void OnPointerDown(PointerEventData eventData) //터치 했을 때
        {
           // if (!isFliping && !isMatched &&!isFlipped) //뒤집히는 중이 아니고, 매치가 되어있지 않으며, 현재 뒤집힌 상태가 아니라면
           //     GameManager.instance.CardClicked(this);//카드 뒤집기 함수 실행
        }
    }
}
