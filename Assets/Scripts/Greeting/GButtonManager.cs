using Bax.P0.Client.UnityWorld.PictureGame;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Settings;

namespace WorldCulture
{
    public class GButtonManager : MonoBehaviour
    {
        [SerializeField]
        GameObject start; //게임시작 오브젝트

        public TextMeshProUGUI mainText; //자막 텍스트
        public TextMeshProUGUI subText; //서브 텍스트

        public GameObject BG; //초기화면 오브젝트

        public GameObject krBtn; //한국화면 오브젝트

        public GameObject ukBtn; //영국화면 오브젝트

        public GameObject cnBtn; //중국화면 오브젝트

        public GameObject jpBtn; //일본화면 오브젝트

        public GameObject spBtn; //스페인화면 오브젝트

        public GameObject homeBtn; //홈버튼 오브젝트

        GameObject sound; //사운드 매니저 변수


        public EnumClass stateClass;
        public GameUI gameUI;
        public ScreenProsess screenProsess;
        public ZoZoBasePatton<GButtonManager> zozo;


        private void Awake()
        {
            stateClass = new EnumClass();
            #region 공용 스테이트 패턴 

            ActionProcess.Enter_StateListener(null, () => 
            {
                homeBtn.SetActive(false); //홈버튼 비활성화
                sound = GameObject.Find("SoundManager"); //사운드 매니저 오브젝트를 찾음
            }, null, null);

            zozo = new ZoZoBasePatton<GButtonManager>();
            zozo.Init(stateClass, screenProsess, new ReadyProcess(screenProsess), new ResultProcess(screenProsess));
            #endregion
        }

        void OnEnable()
        {
        //    BG.SetActive(true); //시작하면 초기화면 활성화
          //  start.SetActive(true); //시작화면 활성화

           
        }

        #region 한국
        public void KrBtn() //한국버튼 함수
        {
            mainText.text = "한국"; //자막 텍스트에 한국 표시
            subText.text = "Korea"; //서브 텍스트에 코리아 표시

            krBtn.SetActive(true); //한국화면 오브젝트 활성화
            ukBtn.SetActive(false); //영국화면 오브젝트 비활성화
            cnBtn.SetActive(false); //중국화면 오브젝트 비활성화
            jpBtn.SetActive(false); //일본화면 오브젝트 비활성화
            spBtn.SetActive(false); //스페인화면 오브젝트 비활성화

            BG.SetActive(false); //초기화면 비활성화
            homeBtn.SetActive(true); //홈버튼 활성화

            sound.GetComponent<GsoundManager>().KrPlay(); //한국 사운드 재생
        }

        
        public void KrFood() //한국 음식버튼 함수
        {
            mainText.text = "김치"; //자막 텍스트에 김치 표시
            subText.text = "Kimchi"; //서브 텍스트에 김치 표시
            sound.GetComponent<GsoundManager>().KrFoodPlay(); //김치 사운드 재생
        }
        public void KrFlower() //한국 꽃버튼 함수
        {
            mainText.text = "무궁화"; //자막 텍스트에 무궁화 표시
            subText.text = "Rose of Sharon"; //서브 텍스트에 로즈 오브 샤론 표시
            sound.GetComponent<GsoundManager>().KrFlowerPlay(); //무궁화 사운드 재생
        }
        public void KrAnimal() //한국 동물버튼 함수
        {
            mainText.text = "호랑이"; //자막 텍스트에 호랑이 표시
            subText.text = "Tiger"; //서브 텍스트에 타이거 표시
            sound.GetComponent<GsoundManager>().KrAnimalPlay(); //호랑이 사운드 재생 
        }
        public void KrStructure() //한국 건물버튼 함수
        {
            mainText.text = "경복궁"; //자막 텍스트에 경복궁 표시
            subText.text = "Gyeongbokgung"; //서브 텍스트에 경복궁 표시
            sound.GetComponent<GsoundManager>().KrStructurePlay(); //경복궁 사운드 재생
        }

        #endregion

        #region 영국
        public void UKBtn() //영국 버튼 함수
        {
            mainText.text = "유나이티드 킹덤(영국)"; //영국 텍스트 표시
            subText.text = "United Kingdom"; //유나이티드 킹덤 표시

            krBtn.SetActive(false); //한국 오브젝트 비활성화
            ukBtn.SetActive(true); //영국 오브젝트 비활성화
            cnBtn.SetActive(false); //중국 오브젝트 비활성화
            jpBtn.SetActive(false); //일본 오브젝트 비활성화
            spBtn.SetActive(false); //스페인 오브젝트 비활성화

            BG.SetActive(false); //초기화면 오브젝트 비활성화

            homeBtn.SetActive(true); //홈버튼 활성화
            sound.GetComponent<GsoundManager>().UkPlay(); //영국 사운드 재생
        }
        public void UKFood() //영국 음식버튼 함수
        {
            mainText.text = "피쉬앤칩스(생선 감자 튀김)"; //피쉬앤칩스 텍스트 표시
            subText.text = "Fish and Chips"; //피쉬앤칩스 텍스트 표시
            sound.GetComponent<GsoundManager>().UkFoodPlay(); //피쉬앤칩스 사운드 재생
        }
        public void UKFlower() //영국 꽃버튼 함수
        {
            mainText.text = "로즈(장미)"; //장미 텍스트 표시
            subText.text = "Rose"; //로즈 텍스트 표시
            sound.GetComponent<GsoundManager>().UkFlowerPlay(); //로즈 사운드 재생
        }
        public void UKAnimal() //영국 동물버튼 함수
        {
            mainText.text = "이글(독수리)"; //독수리 텍스트 표시
            subText.text = "Eagle"; //이글 텍스트 표시
            sound.GetComponent<GsoundManager>().UkAnimalPlay(); //이글 사운드 재생
        }
        public void UKStructure() //영국 건물버튼 함수
        {
            mainText.text = "런던 브릿지(런던 다리)"; //런던 브릿지 텍스트 표시
            subText.text = "London Bridge"; //런던 브릿지 텍스트 표시
            sound.GetComponent<GsoundManager>().UkStructurePlay(); //런던 브릿지 사운드 재생
        }

        #endregion

        #region 중국
        public void CnBtn() //중국 버튼 함수
        {
            mainText.text = "쭝구워(중국)"; //중국 텍스트 표시
            subText.text = "中国 。"; //중국 텍스트 표시

            krBtn.SetActive(false); //한국 오브젝트 비활성화
            ukBtn.SetActive(false); //영국 오브젝트 비활성화
            cnBtn.SetActive(true); //중국 오브젝트 활성화
            jpBtn.SetActive(false); //일본 오브젝트 비활성화
            spBtn.SetActive(false); //스페인 오브젝트 비활성화

            BG.SetActive(false); //초기화면 오브젝트 비활성화

            homeBtn.SetActive(true); //홈버튼 활성화
            sound.GetComponent<GsoundManager>().CnPlay(); //중국 사운드 재생
        }
        public void CnFood() //중국 음식버튼 함수
        {
            mainText.text = "만토우(만두)"; //만두 텍스트 표시
            subText.text = "馒头"; //만토우 텍스트 표시
            sound.GetComponent<GsoundManager>().CnFoodPlay(); //만두 사운드 재생
        }
        public void CnFlower() //중국 꽃버튼 함수
        {
            mainText.text = "메이화(매화)"; //매화 텍스트 표시
            subText.text = "梅花 。"; //메이화 텍스트 표시
            sound.GetComponent<GsoundManager>().CnFlowerPlay(); //매화 사운드 재생
        }
        public void CnAnimal() //중국 동물버튼 함수
        {
            mainText.text = "슝마오(판다)"; //판다 텍스트 표시
            subText.text = "熊猫"; //슝마오 텍스트 표시
            sound.GetComponent<GsoundManager>().CnAnimalPlay(); //판다 사운드 재생
        }
        public void CnStructure()
        {
            mainText.text = "완리창청(만리장성)"; //만리장성 텍스트 표시
            subText.text = "万里长城"; //완리창청 텍스트 표시
            sound.GetComponent<GsoundManager>().CnStructurePlay(); //만리장성 사운드 재생
        }

        #endregion

        #region 일본
        public void JpBtn() //일본 버튼 함수
        {
            mainText.text = "니혼(일본)"; //일본 텍스트 표시
            subText.text = "日本"; //니혼 텍스트 표시

            krBtn.SetActive(false); //한국 오브젝트 비활성화
            ukBtn.SetActive(false); //영국 오브젝트 비활성화
            cnBtn.SetActive(false); //중국 오브젝트 비활성화
            jpBtn.SetActive(true); //일본 오브젝트 활성화
            spBtn.SetActive(false); //스페인 오브젝트 비활성화

            BG.SetActive(false); //초기화면 오브젝트 비활성화
            
            homeBtn.SetActive(true); //홈버튼 활성화
            sound.GetComponent<GsoundManager>().JpPlay(); //일본 사운드 재생
        }
        public void JpFood() //일본 음식버튼 함수
        {
            mainText.text = "스시(초밥)"; //초밥 텍스트 표시
            subText.text = "寿司"; //스시 텍스트 표시
            sound.GetComponent<GsoundManager>().JpFoodPlay(); //스시 사운드 재생
        }
        public void JpFlower() //일본 꽃버튼 함수
        {
            mainText.text = "사쿠라(벚꽃)"; //벚꽃 텍스트 표시
            subText.text = "<b>さくら</b>"; //사쿠라 텍스트 두껍게 표시
            sound.GetComponent<GsoundManager>().JpFlowerPlay(); //사쿠라 사운드 재생
        }
        public void JpAnimal() //일본 동물버튼 함수
        {
            mainText.text = "네코(고양이)"; //고양이 텍스트 표시
            subText.text = "<b>ねこ</b>"; //네코 텍스트 두껍게 표시 
            sound.GetComponent<GsoundManager>().JpAnimalPlay(); //네코 사운드 재생
        }
        public void JpStructure() //일본 건물버튼 함수
        {
            mainText.text = "후지산(후지산)"; //후지산 텍스트 표시 
            subText.text = "<b>ふじさん</b>"; //후지산 텍스트 두껍게 표시 
            sound.GetComponent<GsoundManager>().JpStructurePlay(); //후지산 사운드 재생
        }

        #endregion

        #region 스페인
        public void SpBtn() //스페인버튼 함수
        {
            mainText.text = "에스빠냐(스페인)"; //스페인 텍스트 표시
            subText.text = "España"; //에스파냐 텍스트 표시
            krBtn.SetActive(false); //한국 오브젝트 비활성화
            ukBtn.SetActive(false); //영국 오브젝트 비활성화
            cnBtn.SetActive(false); //중국 오브젝트 비활성화
            jpBtn.SetActive(false); //일본 오브젝트 비활성화
            spBtn.SetActive(true); //스페인 오브젝트 활성화

            BG.SetActive(false); //초기화면 오브젝트 비활성화

            homeBtn.SetActive(true); //홈버튼 활성화
            sound.GetComponent<GsoundManager>().SpPlay(); //스페인 사운드 재생
        }
        public void SpFood() //스페인 음식버튼 함수
        {
            mainText.text = "빠에야(스페인 비빔밥)"; //빠에야 텍스트 표시
            subText.text = " paella"; //빠에야 텍스트 표시
            sound.GetComponent<GsoundManager>().SpFoodPlay(); //빠에야 사운드 재생
        }
        public void SpFlower() //스페인 꽃버튼 함수
        {
            mainText.text = "나란하(오렌지)"; //오렌지 텍스트 표시
            subText.text = "naranja"; //나란하 텍스트 표시
            sound.GetComponent<GsoundManager>().SpFlowerPlay(); //나란하 사운드 재생
        }
        public void SpAnimal() //스페인 동물버튼 함수
        {
            mainText.text = "토로(황소)"; //황소 텍스트 표시
            subText.text = "toro"; //토로 텍스트 표시
            sound.GetComponent<GsoundManager>().SpAnimalPlay(); //토로 사운드 재생
        }
        public void SpStructure() //스페인 건물 표시
        {
            mainText.text = "알함브라(알함브라 궁전)"; //알함브라 궁전 텍스트 표시
            subText.text = "Alhambra"; //알함브라 텍스트 표시
            sound.GetComponent<GsoundManager>().SpStructurePlay(); //알함브라 사운드 재생
        }

        #endregion

        public void HomeBtn() //홈버튼 함수
        {
            GameObject.Find("SoundManager").GetComponent<GsoundManager>().HomeBtn(); //홈버튼 사운드 재생

            krBtn.SetActive(false); //한국 오브젝트 비활성화
            ukBtn.SetActive(false); //영국 오브젝트 비활성화
            cnBtn.SetActive(false); //중국 오브젝트 비활성화
            jpBtn.SetActive(false); //일본 오브젝트 비활성화
            spBtn.SetActive(false); //스페인 오브젝트 비활성화

            homeBtn.SetActive(false); //홈버튼 비활성화

            BG.SetActive(true); //초기화면 오브젝트 활성화

            mainText.text = ""; //자막 텍스트 표시안함
            subText.text = ""; //서브 텍스트 표시안함
        }

        public void gameStart() //게임 시작 함수
        {
            start.SetActive(false); //시작화면 비활성화
        }
    }
}
