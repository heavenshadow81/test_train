using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//컨텐츠 불러오기//


public class LauncherLoading : MonoBehaviour
{
    //외부 공유 변수
    #region Public variables
    public Text messageText;
    public Text submessangeText;
    public Text versionsText;
    public Sprite[] preloadIcons;
    public Sprite preloadDummySprite;
    public GameObject[] preloadPrefabs;

    #endregion
    //일반 변수
    #region Private variables
    string _id, _pw;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //주 제목/소제목 초기화(비우기)
        messageText.text = submessangeText.text = "";
#if BIGBOARD_STANDALONE
        messageText.text = "콘텐츠 개발자용";
        submessangeText.text = "확인용입니다";

#elif UNITY_EDITOR
        messageText.text = "Editor Mode";
        submessangeText.text = "유니티 에디터에서 실행중입니다";
#endif
        //버전 표시
        versionsText.text = $"v{Application.version}";
        //세팅된 값 불러오기
        //ContentsSettingManager를 만들어서 그 함수를 통해 텍스트/json텍스트 불러오기
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
