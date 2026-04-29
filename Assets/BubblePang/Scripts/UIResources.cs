using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//UI의 그림, 글자 등을 모두 들고 있을 클래스...
public class UIResources : MonoBehaviour
{
    #region 변수
    static UIResources _instance;
    public static UIResources Instance
    {
        get => _instance;
    }
    //자동차, 동물,버튼에 표시할 그림
    [SerializeField]
    Sprite[] Number, Alphabet, Korean;

    [SerializeField]
    AudioClip[] AnimalClips, AnimalKoreanTTSClips, AnimalEnglishTTSClips, DinosourClips, DinosourKoreanTTSClips, DinosourEnglishTTSClips;

    [SerializeField]
    AudioClip[] NumberTTSClips, AlphabetTTSClips, KoreanTTSCllps;

    //현재 버튼에 띄울 글귀, 뒷거품배경
    public Sprite[] currentString, bubble;

    [SerializeField]
    Mesh [] Dinos, Animals;

    [SerializeField]
    Material[] DinosMaterials_1, DinosMaterials_2, AnimalMaterials_1, AnimalMaterials_2;

    // 프레팹
    public GameObject [] currentPrefabs, DinoPrefabs, AnimalPrefabs;

    // 현재 메시필터
    public Mesh[] currentMesh;

    // 현재 메테리얼 player_1, player_2 따로 사용
    public Material[] currentMaterial_1, currentMaterial_2;

    //현재 오디오클립
    public AudioClip[] currentclip;

    // 현재 TTS 클립
    public AudioClip[] currentTTSClip;

    // 현재 언어 모드 TTS 클립
    public AudioClip[] currentLanguageTTSClip;

    // 현재 언어 모드
    public string[] currentLanguageString;

    // 클릭 실패 사운드
    public AudioClip[] ClickFailSound;

    //클릭 이펙트
    public ParticleSystem[] ClickEffect;

    // 생성 이펙트
    public ParticleSystem[] createEffect;

    // 클릭시 Good 또는 Bad
    public GameObject[] ClickImage;

    // 버튼 배경
    public Sprite[] backGroundSprite;

    // 현재 언어 모드
    public string[] dinoKoreanString, dinoEnglishString, animalKoreanString, AnimalEnglishString;


    #endregion
    #region 유니티 함수
    //싱글턴 객체 지정
    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }
    private void Start()
    {
        ContentsController.Instance.Initialize += ResourceSet;
    }
    #endregion
    #region 함수
    //컨텐츠 시작 시 ...
    void ResourceSet(object sender, ContentsParameter e)
    {
        //그림은 랜덤으로 선택하기...
        
        switch (e.theme)
        {
            case Theme.Car:

                break;
            // 동물
            case Theme.Animal:
                currentMesh = Animals;
                currentMaterial_1 = AnimalMaterials_1;
                currentMaterial_2 = AnimalMaterials_2;
                currentPrefabs = AnimalPrefabs;

                currentclip = AnimalClips;
                break;
            case Theme.Ocean:

                break;
            // 공룡
            case Theme.Dino:
                currentMesh = Dinos;
                currentMaterial_1 = DinosMaterials_1;
                currentMaterial_2 = DinosMaterials_2;
                currentPrefabs = DinoPrefabs;

                currentclip = DinosourClips;
                break;
        }
        //컨텐츠 타입에 맞게...
        switch (e.contents)
        {
            // 숫자모드
            case BubblePang.ContentsType.Number:
                currentString = Number;
                currentLanguageTTSClip = NumberTTSClips;
                // 숫자 + 공룡
                if (e.theme == Theme.Dino)
                {
                    currentTTSClip = DinosourKoreanTTSClips;
                    for(int i = 0;  i < DinoPrefabs.Length; i++)
                    {
                        DinoPrefabs[i].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = dinoKoreanString[i];
                    }
                    
                }
                // 숫자 + 동물
                else if (e.theme == Theme.Animal)
                {
                    currentTTSClip = AnimalKoreanTTSClips;
                    for (int i = 0; i < AnimalPrefabs.Length; i++)
                    {
                        AnimalPrefabs[i].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = animalKoreanString[i];
                    }
                }
                break;
            // 영어모드
            case BubblePang.ContentsType.Alphabet:
                currentString = Alphabet;
                currentLanguageTTSClip = AlphabetTTSClips;
                // 영어 + 공룡
                if (e.theme == Theme.Dino)
                {
                    currentTTSClip = DinosourEnglishTTSClips;
                    for (int i = 0; i < DinoPrefabs.Length; i++)
                    {
                        DinoPrefabs[i].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = dinoEnglishString[i];
                        print("너의이름은 " + DinoPrefabs[i].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text);
                    }
                }
                // 영어 + 동물
                else if (e.theme == Theme.Animal)
                {
                    currentTTSClip = AnimalEnglishTTSClips;
                    for (int i = 0; i < AnimalPrefabs.Length; i++)
                    {
                        AnimalPrefabs[i].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = AnimalEnglishString[i];
                    }

                }
                break;
            // 한글모드
            case BubblePang.ContentsType.Korean:
                currentString = Korean;
                currentLanguageTTSClip = KoreanTTSCllps;
                // 한글 + 공룡
                if (e.theme == Theme.Dino)
                {
                    currentTTSClip = DinosourKoreanTTSClips;
                    for (int i = 0; i < DinoPrefabs.Length; i++)
                    {
                        DinoPrefabs[i].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = dinoKoreanString[i];
                    }

                }
                // 한글 + 동물
                else if (e.theme == Theme.Animal)
                {
                    currentTTSClip = AnimalKoreanTTSClips;
                    for (int i = 0; i < AnimalPrefabs.Length; i++)
                    {
                        AnimalPrefabs[i].transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = animalKoreanString[i];
                    }
                }
                break;
        }
    }
    #endregion

}
