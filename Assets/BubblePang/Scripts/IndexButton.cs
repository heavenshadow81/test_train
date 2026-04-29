using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//순차 버튼 스크립트
public class IndexButton : MonoBehaviour, ITouchButton, IPointerUpHandler
{
    #region
    //클릭..!
    bool click;
    //버튼 표시(글자): 숫자, 한글, 영문 모두 여기에 표시
    [SerializeField]
    Image buttonshow, bubble;
    // 현재 플레이어
    PersonalUI personalUI;
    //버튼..!
    Button b;

    AudioSource audioSource;
    
    #endregion

    #region 인터페이스 ITouchbutton
    #endregion
    //버튼 초기화
    public void SetButton(int index)
    {
        //텍스트 > 한글 숫자 영문 중에 랜덤으로
        print($"현재 플레이어 셔플 길이{personalUI.playerParameter.shuffleindex.Length}");
        #region 추가된 부분
        //추가된 부분
        if (transform.GetSiblingIndex()<personalUI.playerParameter.shuffleindex.Length)
        {
            print($"현재 불러오는 리소스 갯수{UIResources.Instance.currentString.Length}, 현재 랜덤으로 돌릴 플레이어 UI의 랜덤 배열 갯수 {personalUI.playerParameter.shuffleindex.Length}, 현재 버튼의 위치{transform.GetSiblingIndex()}");

            buttonshow.sprite = UIResources.Instance.currentString[personalUI.playerParameter.shuffleindex[transform.GetSiblingIndex()]];
            bubble.sprite = UIResources.Instance.bubble[Random.Range(0, UIResources.Instance.bubble.Length)];
            buttonshow.color = Color.white;
            bubble.enabled = true;
            buttonshow.enabled = true;
            b.enabled = true;
            transform.parent.GetComponentInChildren<ButtonPos>().SetButtonPos(personalUI, this);
        }
        else
        {
            buttonshow.enabled = false;
            bubble.enabled = false;
            b.enabled = false;
        }
    }
    //버튼 활성/비활성
    public void Activate(bool state)
    {
        gameObject.SetActive(state);
    }
    #endregion
    #region 인터페이스 IPointerDownHandler
    // 버튼을 순서에 맞게 누르면 파티클 생성 & 재생
    public void OnPointerUp(PointerEventData eventData)
    {
        print($"{personalUI.playerParameter.shuffleindex[transform.GetSiblingIndex()]}버튼 클릭! {personalUI.playerParameter.buttonindex}눌러야 함");
        //버튼 인덱스가 현재 버튼의 횟수와 같으면....
        if (personalUI.playerParameter.buttonindex == personalUI.playerParameter.shuffleindex[transform.GetSiblingIndex()])
        {
            // Good 이미지 보여주기
            TouchEvent.Instance.goodView(transform);

            // 버튼 TTS 재생
            audioSource.PlayOneShot(UIResources.Instance.currentLanguageTTSClip[personalUI.playerParameter.shuffleindex[transform.GetSiblingIndex()]]);

            //버튼 순서 다음으로 넘기고
            personalUI.playerParameter.buttonindex++;
            //배경 거품 지우기
            bubble.enabled = false;
            // 버튼 지우기
            buttonshow.enabled = false;

            // Object Material Color Change Method
            personalUI.changeColor();

            //플레이어 UI에 있는 버튼 다 누른 상태면...?
            if (personalUI.playerParameter.buttonindex >= personalUI.playerParameter.indexmax)
            {
                // 해당 결과 오브젝트 보이기
                CompletedObjects.Instance.ShowObject(personalUI.playerParameter.index, transform.position);
                print("셔플 인덱스는2?" + personalUI.playerParameter.index);
                //다음 오브젝트 파라미터 지정
                personalUI.playerParameter.index++;
                //파라미터에 맞게 UI 수정
                personalUI.Change();
            }

            var a = Instantiate(UIResources.Instance.ClickEffect[0]);
            a.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane));
            a.Play();
        }
        // 순서 틀렸을때
        else
        {
            // Bad 이미지 보여주기
            TouchEvent.Instance.badView(transform);
            // 경고음 재생
            audioSource.PlayOneShot(UIResources.Instance.ClickFailSound[0]);
        }
    }
    #endregion
    #region 유니티 함수
    void Awake()
    {
        //객체 지정
        personalUI = GetComponentInParent<PersonalUI>();
        b = GetComponent<Button>();
        audioSource = transform.parent.GetComponent<AudioSource>();
    }
    
    void OnEnable()
    {
        //오브젝트 추가
        personalUI.Add(this);
        print($"오브젝트{name} 위치 {transform.GetSiblingIndex()}, 인덱스: {personalUI.playerParameter.buttonindex}");
    }
    void OnDisable()
    {
        //오브젝트 삭제
        personalUI.RemoveButton(this);
    }
    #endregion
    #region 함수

    //버튼 비활성화!!
    public void ActiveState(bool state)
    {
        buttonshow.enabled = state;
        bubble.enabled = state;
        b.enabled = state;
    }
    
    #endregion



}
