using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LowtoHigh
{
    public class GameManager : PlayManager_PlayGround //상속 받은 클래스를 나중에 만들어서 중복되는 기능이 많음
    {
        public GameObject[] effects; // 파티클 효과 배열
        public TextMeshProUGUI stageText; // 현재 스테이지를 표시하는 텍스트
        public GameObject[] victory; // 승리 화면 관련 오브젝트 배열

        int stage; // 현재 스테이지를 나타내는 변수

        List<int> greenNumber = new List<int>(); // Green 텍스트에 표시될 랜덤 숫자를 저장할 리스트
        public TextMeshProUGUI[] greenText; // Green 텍스트 배열
        public GameObject[] greenGage; // Green 게이지를 표시하는 오브젝트 배열
        public Sprite[] greenSprite; // Green 게이지에 표시될 스프라이트 배열
        int green; // 남은 Green 발판 수
        int greenScore; // Green 점수
        bool greenPerfact; // Green 퍼펙트 여부

        List<int> orangeNumber = new List<int>(); // Orange 텍스트에 표시될 랜덤 숫자를 저장할 리스트
        public TextMeshProUGUI[] orangeText; // Orange 텍스트 배열
        public GameObject[] orangeGage; // Orange 게이지를 표시하는 오브젝트 배열
        public Sprite[] orangeSprite; // Orange 게이지에 표시될 스프라이트 배열
        int orange; // 남은 Orange 발판 수
        int orangeScore; // Orange 점수
        bool orangePerfact; // Orange 퍼펙트 여부

        public GameObject[] perfects; // 퍼펙트 시 나타나는 파티클 오브젝트 배열
        public GameObject[] perfectPos; // 퍼펙트 파티클의 위치를 지정하는 오브젝트 배열

        // Start는 스크립트가 활성화될 때 호출됨
        void OnEnable()
        {
            greenNumber.Clear(); // Green 숫자 리스트 초기화
            orangeNumber.Clear(); // Orange 숫자 리스트 초기화

            CountDown.gameStart = false; // 카운트다운 시작 비활성화

            stage = 1; // 스테이지를 1로 설정
            green = 16; orange = 16; // 발판 수를 16으로 초기화
            greenScore = 0; orangeScore = 0; // 점수 초기화

            CountDown.OnCountdownFinished += HideNumber; // 카운트다운 완료 시 HideNumber 메서드 호출
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CountDown.OnCountdownFinished -= HideNumber;
        }

        // 배열을 랜덤하게 섞는 함수 (피셔-예이츠 셔플 알고리즘)
        void ShuffleArray(TextMeshProUGUI[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1); // 0부터 i까지의 범위에서 랜덤 인덱스 선택
                TextMeshProUGUI temp = array[i]; // 현재 요소를 임시 변수에 저장
                array[i] = array[randomIndex]; // 랜덤 인덱스의 요소를 현재 요소로 대체
                array[randomIndex] = temp; // 임시 변수에 저장된 요소를 랜덤 인덱스에 대입
            }
        }

        // 숫자 및 UI 세팅을 초기화하는 함수
        public void SettingNumber()
        {
            for (int i = 0; i < greenText.Length; i++)
            {
                greenText[i].transform.parent.transform.DOScale(0.9f, 0.2f); // Green 텍스트의 부모 오브젝트 크기 조정
                orangeText[i].transform.parent.transform.DOScale(0.9f, 0.2f); // Orange 텍스트의 부모 오브젝트 크기 조정
            }

            greenPerfact = true; orangePerfact = true; // 퍼펙트 여부 초기화
        }

        // 텍스트 및 숫자를 숨기는 함수
        public void HideNumber()
        {
            ShuffleArray(greenText); // Green 텍스트 배열 셔플
            ShuffleArray(orangeText); // Orange 텍스트 배열 셔플

            int minRange = 1;
            int maxRange = 4;

            for (int i = 0; i < greenText.Length; i++)
            {
                greenNumber.Add(Random.Range(minRange, maxRange)); // Green 숫자를 랜덤하게 리스트에 추가
                orangeNumber.Add(Random.Range(minRange, maxRange)); // Orange 숫자를 랜덤하게 리스트에 추가

                greenText[i].text = greenNumber[i].ToString(); // Green 텍스트에 숫자 표시
                orangeText[i].text = orangeNumber[i].ToString(); // Orange 텍스트에 숫자 표시

                orangeText[i].transform.parent.transform.transform.GetComponent<SpriteRenderer>().color = Color.white; // Orange 배경 색상 초기화
                greenText[i].transform.parent.transform.transform.GetComponent<SpriteRenderer>().color = Color.white; // Green 배경 색상 초기화

                minRange = maxRange; // 최소 범위를 최대값으로 변경
                maxRange += 3; // 최대값을 3 증가

                greenText[i].transform.parent.transform.DOScale(0f, 0.2f); // Green 텍스트 크기 0으로 줄이기
                orangeText[i].transform.parent.transform.DOScale(0f, 0.2f); // Orange 텍스트 크기 0으로 줄이기
            }

            if (greenPerfact || orangePerfact) // 퍼펙트 여부에 따라 딜레이 설정 후 숫자 재설정
            {
                Invoke("SettingNumber", 1.7f);
                
            }   
            else if(!greenPerfact & !orangePerfact)
            {
                Invoke("SettingNumber", 0.5f);
            }          
        }

        // 재시작 시 현재 씬을 다시 로드하는 함수
        public void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬을 다시 로드
        }

        // 플레이어의 입력을 처리하는 함수
        public override void HandleInput(Vector2 inputPosition)
        {
            isTouchable = true; // 터치 가능 상태로 설정

            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);

            RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

            // 레이캐스트가 이 오브젝트와 충돌했는지 확인합니다.
            if (hit.collider != null)
            {
                if (hit.transform.tag == "Green") //터치한 오브젝트의 태그가 Green 이면
                {
                    if (hit.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text == greenNumber[0].ToString()) //터치한 객체의 텍스트가 가장 작은 수 인지 체크
                    {
                        SoundMGR.Instance.SoundPlay("PlayGround_Click");

                        hit.collider.enabled = false; //더블 터치 방지 콜라이더 비활성화

                        greenNumber.RemoveAt(0); //가장 작은 수 리스트에서 삭제
                                                 // DOTween 애니메이션 실행
                        hit.transform.gameObject.GetComponent<DOTweenAnimation>().DOPlay();
                        GameObject particel = Instantiate(effects[0], hit.transform.position, Quaternion.identity); //터치한 위치에 파티클 생성

                        hit.transform.DOScale(0, 0.01f).SetDelay(0.2f).OnComplete(() =>
                        { 
                            hit.transform.GetComponent<SpriteRenderer>().color = Color.white; 
                            hit.collider.enabled = true; //터치한 오브젝트 0.2초 뒤에 작아진 뒤 콜라이더 활성화
                        }); 

                      
                        Destroy(particel, 1f); //생성한 파티크 1초 뒤 삭제

                        green--;
                    }
                    else //틀렸을 때
                    {
                        greenPerfact = false;
                        SoundMGR.Instance.SoundPlay("PlayGround_WrongAnswer");
                        float posX = hit.transform.position.x; //오브젝트 포지션 x값 저장
                                                               //hit.transform.DOMoveX(posX - 0.1f, 0.1f).OnComplete(() => hit.transform.DOMoveX(posX, 0.1f)); //포지션 좌우로 왔다갔다
                        hit.transform.DOScale(0.7f, 0.1f).OnComplete(() => hit.transform.DOScale(0.9f, 0.1f)); //크기 작아졌다 돌아옴
                        //print(greenNumber[0].ToString());
                    }

                }
                else if (hit.transform.tag == "Orange")
                {
                    if (hit.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text == orangeNumber[0].ToString())
                    {
                        SoundMGR.Instance.SoundPlay("PlayGround_Click");
                        hit.collider.enabled = false;

                        orangeNumber.RemoveAt(0);
                        // DOTween 애니메이션 실행
                        hit.transform.gameObject.GetComponent<DOTweenAnimation>().DOPlay();
                        GameObject particel = Instantiate(effects[1], hit.transform.position, Quaternion.identity); //마우스 클릭한 위치에 파티클 생성

                        hit.transform.DOScale(0, 0.01f).SetDelay(0.2f).OnComplete(() =>
                        {
                            hit.transform.GetComponent<SpriteRenderer>().color = Color.white;
                            hit.collider.enabled = true; //터치한 오브젝트 0.2초 뒤에 작아진 뒤 콜라이더 활성화
                        });

                        Destroy(particel, 1f);

                        orange--;
                    }
                    else //틀렸을 때
                    {
                        orangePerfact = false;
                        SoundMGR.Instance.SoundPlay("PlayGround_WrongAnswer");
                        float posX = hit.transform.position.x;
                        //hit.transform.DOMoveX(posX - 0.1f, 0.1f).OnComplete(() => hit.transform.DOMoveX(posX, 0.1f));
                        hit.transform.DOScale(0.7f, 0.1f).OnComplete(() => hit.transform.DOScale(0.9f, 0.1f));

                        //print(orangeNumber[0].ToString());

                    }
                }
            }

            if (orange == 0)
            {
                greenNumber.Clear();
                orangeNumber.Clear();

                SoundMGR.Instance.SoundPlay("PlayGround_GameReset");
                stage += 1;
                stageText.text = stage.ToString();

                orange = 16; green = 16; //발판 수 초기화
                orangeScore += 1;

                if (orangeScore >= 3)
                {
                    SoundMGR.Instance.SoundPlay("PlayGround_Victory");
                    victory[1].SetActive(true);
                    CountDown.gameStart = false;
                    gameCanvas.SetActive(false);
                }
                else
                {
                    if (orangePerfact)
                    {
                        GameObject particle = Instantiate(perfects[1], perfectPos[1].transform);
                        Destroy(particle, 2f);
                        Invoke("HideNumber", 0.3f);
                    }
                    else
                    {
                        HideNumber();
                    }
                }
            }

            if (green == 0)
            {
                greenNumber.Clear();
                orangeNumber.Clear();

                SoundMGR.Instance.SoundPlay("PlayGround_GameReset");
                stage += 1;
                stageText.text = stage.ToString();

                orange = 16; green = 16; //발판 수 초기화
                greenScore += 1;

                if (greenScore >= 3)
                {
                    SoundMGR.Instance.SoundPlay("PlayGround_Victory");
                    victory[0].SetActive(true);
                    CountDown.gameStart = false;
                    gameCanvas.SetActive(false);
                }
                else
                {
                    if (greenPerfact)
                    {
                        GameObject particle = Instantiate(perfects[0], perfectPos[0].transform);
                        Destroy(particle, 2f);
                        Invoke("HideNumber", 0.3f);
                    }
                    else
                    {
                        HideNumber();
                    }
                    
                }
            }

            for (int i = 0; i < greenGage.Length; i++)
            {
                if (stage >= 2)
                {
                    if (greenScore - 1 == i)
                    {
                        greenGage[i].transform.GetComponent<Image>().sprite = greenSprite[greenScore - 1];
                    }

                    if (orangeScore - 1 == i)
                    {
                        orangeGage[i].transform.GetComponent<Image>().sprite = orangeSprite[orangeScore - 1];
                    }
                }
            }
        }
        

        public override void CorrectAnswer(GameObject touched)
        {
            throw new System.NotImplementedException();
        }

        public override void WrongAnswer(GameObject touched)
        {
            throw new System.NotImplementedException();
        }
    }
}