using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimalCard
{
    public class Board : MonoBehaviour
    {
        [SerializeField]
        GameObject cardPrefab; //생성할 카드 프리팹

        [SerializeField]
        Sprite[] Animals; //동물 카드 이미지 배열

        List<int> cardIDList = new List<int>(); //카드 아이디 리스트

        List<CardMove> cardList = new List<CardMove>(); //카드 리스트

        // Start is called before the first frame update
        void OnEnable()
        {
            GenerateCardID(); //카드 리스트 생성 함수 실행
            ShuffleCardID(); //카드 리스트 안의 숫자를 섞는 함수 실행
            InitBoard(); //카드 생성 함수 실행
        }

        void GenerateCardID() //카드 리스트 생성 함수
        {
            for(int i = 0; i < Animals.Length; i++)
            {
                cardIDList.Add(i); //리스트에 0~5까지 추가
                cardIDList.Add(i); //리스트에 0~5 다시 한번 더 추가
            }
        }

        void ShuffleCardID() //카드 리스트 안의 숫자를 섞음
        {
            int cardCount = cardIDList.Count; //카드 리스트의 길이

            for(int i = 0;i < cardCount; i++)
            {
                int randomIndex = Random.Range(i, cardCount); //랜덤한 숫자 지정
                int temp = cardIDList[randomIndex]; //카드 리스트 중에 랜덤한 위치 저장
                cardIDList[randomIndex] = cardIDList[i]; //랜덤한 위치의 리스트를 변수 번째 리스트로 변경
                cardIDList[i] = temp; //변수 번째 리스트를 랜덤한 위치의 리스트로 변경
            }
        }

        void InitBoard() //카드 생성 함수
        {
            int horizon = 3; //카드의 가로 개수
            int vertical = 4; //카드의 세로 개수

            float spaceX = 4.5f; //카드의 세로 간격
            float spaceY = 3.3f; //카드의 가로 간격

            int cardIndex = 0;

            for (int hor = 0; hor < horizon; hor++)
            {
                for (int ver = 0; ver < vertical; ver++)
                {
                    float posX = (ver - (vertical / 2)) *spaceX +(spaceX/2); //변수-(카드 수/2)*세로 간격 + (세로간격/2);
                    float posY = (hor - (int)(horizon/2))*spaceY; //변수-(카드 수/2)*가로 간격

                    Vector3 pos = new Vector3 (posX, posY, 0); //카드 생성 위치
                    GameObject cardObject = Instantiate(cardPrefab, pos, Quaternion.identity); //카드를 생성 위치에 생성

                    CardMove card = cardObject.GetComponent<CardMove>(); //카드무브 스크립트를 가져옴
                    
                    int cardID = cardIDList[cardIndex++]; //카드 아이디 리스트에 있는 숫자를 저장함
                    card.SetCardID(cardID); //카드 리스트의 숫자를 카드 아이디에 부여

                    card.SetAnimal(Animals[cardID]); //카드무브의 카드 이미지 가져오는 함수를 애니몰즈 배열 안에 있는 이미지로 실행
                    cardList.Add(card); //카드 리스트에 카드 저장
                }
            }
        }
        public List<CardMove> GetCards() //카드 리스트 가져오기 함수
        {
            return cardList; //카드리스트 가져옴
        }
    }
 
}
