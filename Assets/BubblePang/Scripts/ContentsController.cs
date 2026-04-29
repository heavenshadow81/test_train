using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
//컨텐츠 전반 컨트롤..
public class ContentsController : MonoBehaviour
{
    #region 변수
    static ContentsController _instance;
    //싱글톤!!
    public static ContentsController Instance
    {
        get => _instance;
    }
    
    //초기화(게임 재시작 및 시작 시 부를 이벤트)
    public event EventHandler<ContentsParameter> Initialize;
    //컨텐츠 파라미터
    public ContentsParameter contentsParameter = new ContentsParameter();

    // Option 버튼 dropDown
    [SerializeField]
    DropValueSet dropValue;
    #endregion
    #region 유니티 함수
    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }
    private void OnEnable()
    {
        // 컨텐츠 타입 들고오기
        contentsParameter.contents = DinoSceneOptions.GetContentsType();

        // 컨텐츠 테마 들고오기
        contentsParameter.theme = DinoSceneOptions.GetTheme();

        // 컨텐츠 인원수 들고오기
        contentsParameter.person = DinoSceneOptions.GetPersons();

        // 컨텐츠 난이도 들고오기
        contentsParameter.difficult = DinoSceneOptions.GetDifficulty();

        // DropDown Value 에도 들고 온 값 적용 하기
        dropValue.SetDropdown((int)contentsParameter.contents, (int)contentsParameter.person, (int)contentsParameter.difficult);
    }
    #endregion

    #region 함수
    //초기화 이벤트 호출
    void InitializeContent()
    {
        Initialize?.Invoke(this, contentsParameter);
    }
    
    #endregion
    #region 이벤트 함수
    void Contents(object sender, ContentsParameter e)
    {
        //랜덤하게 띄우기용..
        var order = Enumerable.Range(0, UIResources.Instance.currentMesh.Length).ToArray();
        //랜덤..?
        e.shufflepart = BubblePang.UIController.Instance.ShuffleArray(order, UnityEngine.Random.Range(0, 60));
        for (int i = 0; i<e.shufflepart.Length; i++)
        {
            print($"{i}번째 {e.shufflepart[i]}");
        }
        print("컨텐츠 초기화");
    }
    //컨텐츠 초기화
    public void Init()
    {
        Initialize += Contents;
        Initialize?.Invoke(this, contentsParameter);
        Initialize -= Contents;
    }

    #endregion
}
