using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
//플레이어 개인 버튼 : 버튼들
public class PersonalUI : MonoBehaviour, IPersonal
{
    #region
    
    //순차 버튼
    List<ITouchButton> buttons = new List<ITouchButton>();
    
    //이름 표시..?
    Text Name;

    //그림을 들고 있을 예정..??..
    [SerializeField]
    Image buttonbackground;

    // Object Mesh
    [SerializeField]
    MeshFilter buttonMesh;

    // Object MeshRenderer
    [SerializeField]
    MeshRenderer buttonMeshRenderer;

    // Object Material
    Material buttonMaterial;

    // 버튼 뒷 배경
    [SerializeField]
    Image backImg;

    // 게임 방법 Image
    [SerializeField]
    GameObject howGameObject;

    [SerializeField]
    int PlayerNumber;

    // Material Color 조절값
    float colorNumber;

    public PlayerParameter playerParameter = new PlayerParameter();
    //개별 초기화...?
    public event EventHandler<PlayerParameter> Initialize;

    #endregion

    #region
    //버튼 인덱스
    public int ButtonIndex
    { 
        get => playerParameter.index;
        set
        {
            playerParameter.index = value;

        }
    }
    //버튼 추가
    public void Add(ITouchButton button)
    {
        buttons.Add(button);
        playerParameter.buttonCount = buttons.Count;
    }
    //버튼 제거
    public void RemoveButton(ITouchButton button)
    {
        buttons.Remove(button);
        playerParameter.buttonCount = buttons.Count;
    }
    //비활성화
    public void Disactive()
    {
        gameObject.SetActive(false);
    }
    //버튼 바꾸기
    public void Change()
    {
        StartCoroutine(PlayerSetUI());
    }
    //
    IEnumerator PlayerSetUI()
    {
        yield return new WaitForSeconds(0.05f);
        Initialize?.Invoke(this, playerParameter);
        yield break;
    }

    //버튼들 모두 바꾸기...>>>
    void ChangeButtonSet(object sender, PlayerParameter e)
    {
        //숫자 비교...> 
        int a = ContentsController.Instance.contentsParameter.difficult == Difficulty.Easy ? 5 : 8;
        //버튼 맞추는 숫자 최대치> 1~9까지면 버튼 총 갯수, 1~6까지면 6...
        e.indexmax = ContentsController.Instance.contentsParameter.difficult == Difficulty.Easy ? 6 : e.buttonCount;
        // 맞춰야 하는 버튼의 갯수에 따라 colorNumber 값 계산
        colorNumber = (float)(1.0 / e.indexmax);
        print("colorNumber? " + colorNumber);
        if (e.index < a)
        {
            e.buttonindex = 0;
            var temp = ContentsController.Instance.contentsParameter.difficult == Difficulty.Easy ? Enumerable.Range(0, 6).ToArray() :Enumerable.Range(0, UIResources.Instance.currentString.Length).ToArray();
            //var temp = Enumerable.Range(0, a).ToArray();
            e.shuffleindex = BubblePang.UIController.Instance.ShuffleArray(temp, UnityEngine.Random.Range(0, 200));
            foreach (var button in buttons)
            {
                button.SetButton(0);
                print("버튼 설정");
            }

            // 맞출 오브젝트 실루엣 설정
            buttonMesh.mesh = UIResources.Instance.currentMesh[ContentsController.Instance.contentsParameter.shufflepart[e.index]];

            // 지금 오브젝트의 이름에 따라서 currentMaterial을 구분해서 들고 옴
            if (gameObject.name == "PlayerUI")
            {
                buttonMaterial = buttonMeshRenderer.material = UIResources.Instance.currentMaterial_1[ContentsController.Instance.contentsParameter.shufflepart[e.index]];
            }
            else
            {
                buttonMaterial = buttonMeshRenderer.material = UIResources.Instance.currentMaterial_2[ContentsController.Instance.contentsParameter.shufflepart[e.index]];
            }

            // 오브젝트 실루엣 색깔 검정으로 초기화
            buttonMaterial.color = new Color(0, 0, 0);

            print("셔플 인덱스는?"+ContentsController.Instance.contentsParameter.shufflepart[e.index]);

            // 배경이미지 색깔 랜덤으로 변경
            backImg.sprite = UIResources.Instance.backGroundSprite[UnityEngine.Random.Range(0, UIResources.Instance.backGroundSprite.Length)];
        }
        // 게임 플레이가 종료 되었다면
        else
        {
            // 배경 이미지 비활성화
            backImg.gameObject.SetActive(false);
            // 오브젝트 실루엣 비활성화
            buttonMesh.gameObject.SetActive(false);
            // 게임방법 비활성화
            howGameObject.gameObject.SetActive(false);
        }
    }
    
    // 숫자 맞출때마다 조절 할 Object Material Color값 변경 메소드
    public void changeColor()
    {
        buttonMaterial.color += new Color(colorNumber, colorNumber, colorNumber);
    }

    #endregion
    #region 유니티 함수
    private void Awake()
    {
        Initialize += ChangeButtonSet;
    }
    
    void OnEnable()
    {
        Change();
    }
    void Start()
    {
        BubblePang.UIController.Instance.Add(this);
    }
    #endregion
}
