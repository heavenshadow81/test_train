using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FC_ColorManager : MonoBehaviour
{
    public static FC_ColorManager instance;

    public Image frame; //현재 모아야하는 색상
    public List<Sprite> frameColor; //현재 모아야하는 색상
    public int stage; //현재 몇벚째 색상인지 체크

    public  GameObject[] colors; //찾은 색상의 수 UI

    Dictionary<string, Sprite[]> colorSpriteDict; //frame 색상에 따라 바뀌는 colors 이미지 스프르라이트
   
    public Sprite[] redColors;
    public Sprite[] greenColors;
    public Sprite[] blueColors;
    public Sprite[] yellowColors;
    public Sprite[] pinkColors;
    public Sprite[] violetColors;
    public Sprite[] orangeColors;

    void Awake()
    {
        // 인스턴스가 이미 존재하는지 확인
        if (instance == null)
        {
            instance = this; // 인스턴스가 없으면 현재 오브젝트를 인스턴스로 설정
            DontDestroyOnLoad(gameObject); // 게임 오브젝트가 씬이 로드될 때 파괴되지 않도록 설정
        }
        else if (instance != this)
        {
            Destroy(gameObject); // 인스턴스가 이미 존재하면 새로운 인스턴스를 파괴
        }
    }

    void OnEnable()
    {
        //리스트의 순서를 랜덤하게 바꿈
        ShuffleArray(frameColor);
        //리스트 첫번째 순서의 색상으로 바꿈
        frame.sprite = frameColor[stage];

        colorSpriteDict = new Dictionary<string, Sprite[]>()
        {
            { "Red", redColors },
            { "Pink", pinkColors },
            { "Blue", blueColors },
            { "Yellow", yellowColors },
            { "Violet", violetColors },
            { "Orange", orangeColors },
            { "Green", greenColors }
        };
    }

    void ShuffleArray(List<Sprite> array)
    {
        // 피셔-예이츠 셔플 알고리즘
        for (int i = array.Count - 1; i > 0; i--)
        {
            // 0부터 i까지의 범위에서 랜덤 인덱스 선택
            int randomIndex = Random.Range(0, i + 1);

            // 배열의 i번째 요소와 랜덤 인덱스 요소를 교환
            Sprite temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    private void Update()
    {
        if (colorSpriteDict.ContainsKey(frame.sprite.name))
        {
            Sprite[] selectedSprites = colorSpriteDict[frame.sprite.name];

            for (int i = 0; i < colors.Length && i < selectedSprites.Length; i++)
            {
                colors[i].GetComponent<Image>().sprite = selectedSprites[i];
            }
        }
    }
}
    
