using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace ShapePuzzle
{
    public class CardManager_SP : PlayManager_PlayGround
    {
        [Header("카드 설정")]
        private string[] hexCodes = new string[] { "#ff000d", "#0074ff", "#00ff12", "#ffffff" };
        public Sprite[] shapes = null;
        public Card_SP[] cards = null;

        [Header("힌트")]
        public TextMeshProUGUI hintText;

        [Header("정답")]
        [SerializeField] private Card_SP answerCard;

        protected override void Init()
        {
            base.Init();

            SettingCard();
        }

        private void SettingCard()
        {
            CreateCardData();
            DisplayCardHint();
        }

        void CreateCardData()
        {
            HashSet<string> usedCombinations = new HashSet<string>(); // 중복 체크를 위한 해시셋

            for (int i = 0; i < cards.Length; i++)
            {
                string cardHexCode;
                string shapeHexCode;
                Sprite shape;

                // 카드 색상과 모양 색상이 같지 않도록 설정
                do
                {
                    cardHexCode = hexCodes[Random.Range(0, hexCodes.Length)];
                    shapeHexCode = hexCodes[Random.Range(0, hexCodes.Length)];
                    shape = shapes[Random.Range(0, shapes.Length)];
                } while (cardHexCode == shapeHexCode || usedCombinations.Contains(cardHexCode + shapeHexCode + shape.name));

                // 선택된 HexCode -> Color로 변경
                Color cardColor = HexColor(cardHexCode);
                Color shapeColor = HexColor(shapeHexCode);

                // 카드 데이터 생성
                cards[i].SetCardData(cardColor, shapeColor, shape);

                // 조합을 해시셋에 추가
                usedCombinations.Add(cardHexCode + shapeHexCode + shape.name);
            }
        }

        void DisplayCardHint()
        {
            // 랜덤하게 카드 선택
            int randomIndex = Random.Range(0, cards.Length);
            answerCard = cards[randomIndex];

            // 카드 색상, 모양, 모양 색상 가져오기
            Color cardColor = answerCard.cardColor.color;
            Color shapeColor = answerCard.shape.color;
            Sprite shapeSprite = answerCard.shape.sprite;

            // 색상에 따른 텍스트 설정
            string cardColorName = GetColorName(cardColor);
            string shapeColorName = GetColorName(shapeColor);

            // 힌트 텍스트 설정
            hintText.text = $"{cardColorName} 카드, {shapeColorName} {shapeSprite.name}";
        }

        string GetColorName(Color color)
        {
            // 색상 코드에 따라 이름 반환
            if (color == HexColor("#ff000d"))
            {
                return "빨간";
            }
            else if (color == HexColor("#0074ff"))
            {
                return "파란";
            }
            else if (color == HexColor("#00ff12"))
            {
                return "초록";
            }
            else if (color == HexColor("#ffffff"))
            {
                return "하얀";
            }
            return "알 수 없음"; // 기본값
        }

        public Color HexColor(string hexCode)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(hexCode, out color))
            {
                return color;
            }

            Debug.LogError("[UnityExtension::HexColor]invalid hex code - " + hexCode);
            return Color.white;
        }

        public override void CorrectAnswer(GameObject touched)
        {
            throw new System.NotImplementedException();
        }

        private void CorrecAnswer()
        {
            SoundMGR.Instance.SoundPlay("PlayGround_Card(1)");

            foreach (var card in cards)
            {
                // 카드의 현재 회전을 초기화
                card.transform.DORotate(new Vector3(0, 180, 0), 0.5f).OnComplete(() =>
                {
                    // Y축을 기준으로 180도 회전하는 애니메이션 추가
                    card.transform.DORotate(Vector3.zero, 0f);
                });
            }

            ChangeGauge();
            SettingCard();
        }

        public override void HandleInput(Vector2 inputPosition)
        {
            // 터치/마우스 위치를 월드 좌표로 변환
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

            // 터치/마우스 위치에서 카드 찾기
            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            if (hit.collider != null)
            {
                Card_SP touchedCard = hit.collider.GetComponent<Card_SP>();

                // 터치한 카드가 "Green" 태그를 가지고 있는지 확인하고, 정답 카드인지 확인
                if (touchedCard != null && touchedCard.CompareTag(TeamNameString))
                {
                    if (touchedCard == answerCard)
                    {
                        // 정답 처리
                        CorrecAnswer();
                        GameObject particle = Instantiate(effect, hit.transform);
                        Destroy(particle, 1f);
                    }
                    else
                    {
                        // 오답 처리
                        WrongAnswer(touchedCard);
                    }
                    isTouchable = true;
                }
                else
                {
                    isTouchable = true;
                }
            }
            else
            {
                isTouchable= true;
            }
        }

        public override void WrongAnswer(GameObject touched)
        {
            throw new System.NotImplementedException();
        }

        private void WrongAnswer(Card_SP touchedCard)
        {
            SoundMGR.Instance.SoundPlay("PlayGround_WrongAnswer");

            // 카드의 현재 크기를 기준으로 0.8배로 줄이는 애니메이션 추가
            touchedCard.transform.DOScale(Vector3.one * 0.8f, 0.2f).OnComplete(() =>
            {
                // 애니메이션이 끝난 후 원래 크기로 복원
                touchedCard.transform.DOScale(Vector3.one, 0.2f);
            });
        }
    }

}

