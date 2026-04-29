using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Telephone
{
    public class GameManager : MonoBehaviour
    {
        // 체크 이미지와 책 스프라이트를 담을 배열들
        public GameObject[] checkImage;
        public Sprite[] bookSprite;

        // 사용자가 입력한 숫자를 보여줄 TextMeshProUGUI 객체
        public TextMeshProUGUI inputNumber;
        // 사용자가 입력한 숫자들을 저장할 리스트
        List<int> inputNumbers = new List<int>();
        // 현재 입력된 번호를 저장할 문자열
        string currentInput;

        // 정답 숫자를 보여줄 TextMeshProUGUI 객체
        public TextMeshProUGUI correctNumber;
        // 정답 숫자들을 저장할 리스트
        List<int> correctNumbers = new List<int>();

        // 입력된 번호가 정답인지 여부를 저장할 배열
        bool[] numberCheck = new bool[7];
        // 정답 여부를 저장할 변수
        bool correct;

        // 정답 처리 중인지 여부를 저장할 변수
        bool correctTime;

        public TextMeshProUGUI nameText;
        string[] names = new string[] { "이모", "사촌", "아빠", "친구", "할머니", "할아버지", "엄마", "삼촌" };

        // Start is called before the first frame update
        void Start()
        {
            // 7자리 정답 번호 생성
            for (int i = 0; i < 7; i++)
            {
                correctNumbers.Add(Random.Range(0, 10));

                // 4번째 자리에는 '-'를 추가
                if (correctNumbers.Count == 4)
                    correctNumber.text += "-";

                correctNumber.text += correctNumbers[i];
            }

            // 책 스프라이트를 랜덤하게 선택
            int random = Random.Range(0, bookSprite.Length);
            checkImage[2].GetComponent<Image>().sprite = bookSprite[random];
            nameText.text = names[random];
        }

        // Update is called once per frame
        void Update()
        {
            // 매 프레임마다 실행되는 로직이 현재는 없음
        }

        // 번호 버튼을 눌렀을 때 호출되는 함수
        public void InputNumberBtn(int number)
        {
            // 버튼 눌리는 소리 재생
            SoundMGR.Instance.SoundPlay($"{number}");

            // 3번째 숫자 후에 '-' 추가
            if (inputNumbers.Count == 3)
                inputNumber.text += "-";

            // 입력된 숫자가 7자리가 될 때까지 입력을 받음
            if (inputNumbers.Count < 7)
            {
                inputNumbers.Add(number);
                inputNumber.text += number;
            }

            // 현재 입력된 번호 갱신
            currentInput = inputNumber.text;
        }

        // 지우기 버튼을 눌렀을 때 호출되는 함수
        public void DeletNumberBtn()
        {
            // 버튼 눌리는 소리 재생
            SoundMGR.Instance.SoundPlay("전화기 버튼소리");

            // 입력된 번호가 있을 때만 삭제 처리
            if (inputNumbers.Count != 0)
            {
                inputNumbers.RemoveAt(inputNumbers.Count - 1);

                // 마지막 입력된 숫자 제거
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
                inputNumber.text = currentInput;

                // 4번째 자리의 '-'도 함께 제거
                if (inputNumbers.Count == 3)
                {
                    currentInput = currentInput.Substring(0, currentInput.Length - 1);
                    inputNumber.text = currentInput;
                }
            }

            // 입력된 번호가 모두 삭제된 경우 텍스트 초기화
            if (inputNumbers.Count <= 0)
                inputNumber.text = "";
        }

        // 통화 버튼을 눌렀을 때 호출되는 함수
        public void CallBtn()
        {
            // 정답 처리가 진행 중이 아닐 때만 처리
            if (!correctTime)
            {
                // 입력된 번호가 7자리가 아닌 경우
                if (inputNumbers.Count != 7)
                {
                    // 틀린 소리 재생
                    SoundMGR.Instance.SoundPlay("18.틀림");

                    // 체크 이미지를 확대 후 원래 크기로 되돌림
                    checkImage[0].transform.DOScale(1, 1f).OnComplete(() =>
                    checkImage[0].transform.DOScale(0, 1f));

                    // 입력된 번호 초기화
                    inputNumbers.Clear();
                    inputNumber.text = "";
                }
                else
                {
                    correct = true;

                    // 입력된 번호와 정답 번호를 비교
                    for (int i = 0; i < inputNumbers.Count; i++)
                    {
                        if (inputNumbers[i] != correctNumbers[i])
                            numberCheck[i] = false;
                        else
                            numberCheck[i] = true;

                        // 틀린 번호가 있으면 정답을 false로 설정
                        if (!numberCheck[i])
                        {
                            correct = false;
                        }
                    }

                    // 정답이 아닌 경우
                    if (!correct)
                    {
                        // 틀린 소리 재생
                        SoundMGR.Instance.SoundPlay("18.틀림");

                        // 체크 이미지를 확대 후 원래 크기로 되돌림
                        checkImage[0].transform.DOScale(1, 1f).OnComplete(() => checkImage[0].transform.DOScale(0, 1f));

                        // 입력된 번호 초기화
                        inputNumbers.Clear();
                        inputNumber.text = "";
                    }
                    // 정답인 경우
                    else
                    {
                        correctTime = true;
                        // 정답 소리 재생
                        SoundMGR.Instance.SoundPlay("18.정답");

                        // 정답 이미지를 확대 후 원래 크기로 되돌림
                        checkImage[1].transform.DOScale(1, 1f).OnComplete(() =>
                        {
                            checkImage[1].transform.DOScale(0, 1f).OnComplete(() => correctTime = false);
                        });

                        // 입력된 번호, 정답 번호, 텍스트 초기화
                        inputNumbers.Clear();
                        inputNumber.text = "";
                        correctNumbers.Clear();
                        correctNumber.text = "";

                        // 새로운 정답 번호 생성
                        for (int i = 0; i < 7; i++)
                        {
                            correctNumbers.Add(Random.Range(0, 10));

                            // 4번째 자리에 '-' 추가
                            if (correctNumbers.Count == 4)
                                correctNumber.text += "-";

                            correctNumber.text += correctNumbers[i];
                        }

                        // 책 스프라이트를 랜덤하게 변경
                        int random = Random.Range(0, bookSprite.Length);
                        checkImage[2].GetComponent<Image>().sprite = bookSprite[random];
                        nameText.text = names[random];
                    }
                }
            }
        }
    }
}
